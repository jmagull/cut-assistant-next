using System.Net;
using System.Net.Http;
using System.Text.Json;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistRatingTests : IDisposable
{
    internal const string Fred = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
    internal const string Url = "https://sniplist.mepaso.net/" + Fred + "/";
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "can-rating-" + Guid.NewGuid().ToString("N"));
    public CutlistRatingTests() => Directory.CreateDirectory(_directory);
    public void Dispose() => Directory.Delete(_directory, recursive: true);
    private string FilePath => Path.Combine(_directory, "ratings.json");
    internal static CutlistRatingSource Source(string id = "12345", string? url = null, string author = "Community") =>
        CutlistRatingSource.Create(url ?? Url, id, author, "Aufnahme.mp4.cutlist")!;
    private static Task<CutlistRatingResult> Accepted(CutlistRatingSource s, int r, CancellationToken t) =>
        Task.FromResult(new CutlistRatingResult(CutlistRatingState.Accepted, "Bewertung an den Server übermittelt."));

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task AllSixGradesUseRootPostNumericIdAndFredHeader(int rating)
    {
        var calls = 0;
        using var http = new HttpClient(new Handler(async (request, token) =>
        {
            calls++;
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://sniplist.mepaso.net/api/set-rating", request.RequestUri!.AbsoluteUri);
            Assert.Equal("Bearer " + Fred, request.Headers.GetValues("X-Authorization").Single());
            Assert.DoesNotContain(Fred, request.RequestUri.AbsoluteUri);
            Assert.Contains("CutAssistantNext", request.Headers.UserAgent.ToString());
            Assert.Equal("application/json", request.Content!.Headers.ContentType!.MediaType);
            using var json = JsonDocument.Parse(await request.Content.ReadAsStringAsync(token));
            Assert.Equal(12345, json.RootElement.GetProperty("id").GetInt64());
            Assert.Equal(rating, json.RootElement.GetProperty("rating").GetInt32());
            return new(HttpStatusCode.OK) { Content = new StringContent("{}") };
        }));
        var result = await new CutlistServerClient(http).RateAsync(Source(), rating);
        Assert.True(result.Accepted);
        Assert.Equal("Bewertung an den Server übermittelt.", result.Message);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void CutlistAtKeepsConfiguredOriginAndRootEndpoint()
    {
        var source = Source(url: "http://cutlist.at/" + Fred + "/");
        Assert.Equal("http://cutlist.at/api/set-rating", source.Endpoint.AbsoluteUri);
        Assert.Equal(Fred, source.Fred);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData(" 5")]
    [InlineData("1&rating=5")]
    [InlineData("abc")]
    [InlineData("9223372036854775808")]
    public void InvalidIdsCannotCreateRatingContext(string id) => Assert.Null(CutlistRatingSource.Create(Url, id, "Autor", "Liste"));

    [Theory]
    [InlineData("https://evil.invalid/")]
    [InlineData("https://sniplist.mepaso.net/short/")]
    [InlineData("https://sniplist.mepaso.net:8443/")]
    [InlineData("https://user@sniplist.mepaso.net/")]
    [InlineData("https://sniplist.mepaso.net//")]
    public void InvalidServerAddressesCannotCarryCredentials(string prefix) =>
        Assert.Null(CutlistRatingSource.Create(prefix + Fred + "/", "1", "Autor", "Liste"));

    [Theory]
    [InlineData("")]
    [InlineData("{}")]
    [InlineData("true")]
    [InlineData("OK")]
    [InlineData("{\"error\":0}")]
    [InlineData("{\"error\":false}")]
    [InlineData("{\"errors\":[]}")]
    [InlineData("{\"error\":null}")]
    [InlineData("error=0")]
    public async Task Http200WithoutExplicitErrorIsAcceptedLikeWebClient(string body)
    {
        using var http = Http(HttpStatusCode.OK, body);
        Assert.True((await new CutlistServerClient(http).RateAsync(Source(), 4)).Accepted);
    }

    [Theory]
    [InlineData("{\"error\":\"already rated\"}")]
    [InlineData("{\"error\":\"own cutlist\"}")]
    [InlineData("{\"errors\":[\"missing id\"]}")]
    [InlineData("{\"success\":false}")]
    [InlineData("{\"ok\":false}")]
    [InlineData("{\"status\":\"error\"}")]
    [InlineData("error")]
    [InlineData("Error denied")]
    [InlineData("Fehler: abgelehnt")]
    [InlineData("<html><title>Error</title></html>")]
    [InlineData("error=2")]
    [InlineData("error: denied")]
    [InlineData("false")]
    public async Task ExplicitErrorsAreNotIgnoredOrEchoedEvenOnHttp200(string body)
    {
        using var http = Http(HttpStatusCode.OK, body.Replace("denied", Fred));
        var result = await new CutlistServerClient(http).RateAsync(Source(), 4);
        Assert.False(result.Accepted);
        Assert.Equal(CutlistRatingState.Rejected, result.State);
        Assert.DoesNotContain(Fred, result.Message);
        Assert.DoesNotContain(body, result.Message);
    }

    [Theory]
    [InlineData(204)]
    [InlineData(302)]
    [InlineData(400)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(409)]
    [InlineData(422)]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(503)]
    public async Task Only200IsAcceptedAndNoInventedServerErrorCodesAreUsed(int status)
    {
        using var http = Http((HttpStatusCode)status, Fred);
        var result = await new CutlistServerClient(http).RateAsync(Source(), 3);
        Assert.False(result.Accepted);
        Assert.Contains($"HTTP {status}", result.Message);
        Assert.DoesNotContain(Fred, result.Message);
    }

    [Theory]
    [InlineData("timeout")]
    [InlineData("cancel")]
    [InlineData("network")]
    public async Task UnclearTransportOutcomeIsSanitizedAndNeverRetried(string reason)
    {
        var calls = 0;
        using var http = new HttpClient(new Handler((r, t) =>
        {
            calls++;
            return Task.FromException<HttpResponseMessage>(reason switch
            {
                "network" => new HttpRequestException(Url),
                "timeout" => new TaskCanceledException(Url),
                _ => new OperationCanceledException(Url)
            });
        }));
        var result = await new CutlistServerClient(http).RateAsync(Source(), 2);
        Assert.Equal(CutlistRatingState.Unknown, result.State);
        Assert.DoesNotContain(Fred, result.Message);
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public async Task InvalidGradesNeverSend(int rating)
    {
        using var http = new HttpClient(new Handler((r, t) => throw new Xunit.Sdk.XunitException("No request allowed")));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new CutlistServerClient(http).RateAsync(Source(), rating));
    }

    [Fact]
    public async Task DoubleClickAndRestartCannotSendAnotherRequest()
    {
        var pending = new TaskCompletionSource<CutlistRatingResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        var store = new CutlistRatingSettingsStore(FilePath);
        var service = new CutlistRatingService(store, (s, r, t) =>
        {
            Interlocked.Increment(ref calls); entered.SetResult(); return pending.Task;
        });
        var first = service.SubmitAsync(Source(), 4);
        await entered.Task;
        Assert.False((await service.SubmitAsync(Source(), 5)).Accepted);
        Assert.Equal(CutlistRatingState.Attempted, new CutlistRatingSettingsStore(FilePath).Get(Source().Key));
        pending.SetResult(new(CutlistRatingState.Accepted, "Bewertung an den Server übermittelt."));
        Assert.True((await first).Accepted);
        var restarted = new CutlistRatingService(new CutlistRatingSettingsStore(FilePath), (s, r, t) =>
        {
            calls++; return Accepted(s, r, t);
        });
        Assert.False((await restarted.SubmitAsync(Source(), 4)).Accepted);
        Assert.Equal(1, calls);
        Assert.Equal(CutlistRatingState.Accepted, new CutlistRatingSettingsStore(FilePath).Get(Source().Key));
        var persisted = await File.ReadAllTextAsync(FilePath);
        Assert.DoesNotContain(Fred, persisted); Assert.DoesNotContain("http", persisted);
        Assert.DoesNotContain(Fred, Source().ToString());
    }

    [Fact]
    public async Task UnknownOutcomeSurvivesRestartWithoutNewOfferOrSending()
    {
        var service = new CutlistRatingService(new CutlistRatingSettingsStore(FilePath),
            (s, r, t) => throw new HttpRequestException(Url));
        Assert.Equal(CutlistRatingState.Unknown, (await service.SubmitAsync(Source(), 4)).State);
        var workflow = new CutlistRatingWorkflow(new CutlistRatingSettingsStore(FilePath));
        workflow.Use(Source(), "Film.mp4", "Ich", "Community");
        Assert.True(await workflow.CanLeaveAsync(_ => throw new Exception("No prompt"), _ => throw new Exception("No resend")));
    }

    [Fact]
    public void ServerAccountAndCutlistIdHaveIndependentHistoryKeys()
    {
        var original = Source();
        Assert.NotEqual(original.Key, Source("12346").Key);
        Assert.NotEqual(original.Key, Source(url: "http://cutlist.at/" + Fred + "/").Key);
        Assert.NotEqual(original.Key, Source(url: Url.Replace(Fred, new string('a', 64))).Key);
        Assert.Equal(original.Key, Source("00012345").Key);
    }

    [Fact]
    public async Task CannotPersistBeforeSendingMeansNoRequest()
    {
        await File.WriteAllTextAsync(FilePath, "{broken");
        var calls = 0;
        var result = await new CutlistRatingService(new CutlistRatingSettingsStore(FilePath), (s, r, t) =>
        { calls++; return Accepted(s, r, t); }).SubmitAsync(Source(), 4);
        Assert.False(result.Accepted); Assert.Equal(0, calls);
        Assert.Contains("nichts gesendet", result.Message);
    }

    [Fact]
    public async Task FailedFinalSaveLeavesDurableAttemptAndPreventsResend()
    {
        var store = new CutlistRatingSettingsStore(FilePath);
        var service = new CutlistRatingService(new FailingCompletionStore(store), Accepted);
        var result = await service.SubmitAsync(Source(), 4);
        Assert.True(result.Accepted); Assert.Contains("nicht gespeichert", result.Message);
        Assert.Equal(CutlistRatingState.Attempted, new CutlistRatingSettingsStore(FilePath).Get(Source().Key));
        Assert.False((await new CutlistRatingService(new CutlistRatingSettingsStore(FilePath), Accepted).SubmitAsync(Source(), 4)).Accepted);
    }

    [Fact]
    public async Task CanceledBeforeSubmissionDoesNotReserveOrSend()
    {
        var store = new CutlistRatingSettingsStore(FilePath);
        using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
        var result = await new CutlistRatingService(store, (s, r, t) => throw new Exception("No send"))
            .SubmitAsync(Source(), 4, cancellation.Token);
        Assert.False(result.Accepted); Assert.Null(store.Get(Source().Key));
    }

    [Fact]
    public async Task TwoIndependentServicesCannotSendSameCutlistTwice()
    {
        var count = 0;
        Task<CutlistRatingResult> Send(CutlistRatingSource s, int r, CancellationToken t)
        { Interlocked.Increment(ref count); return Accepted(s, r, t); }
        await Task.WhenAll(
            new CutlistRatingService(new CutlistRatingSettingsStore(FilePath), Send).SubmitAsync(Source(), 4),
            new CutlistRatingService(new CutlistRatingSettingsStore(FilePath), Send).SubmitAsync(Source(), 4));
        Assert.Equal(1, count);
    }

    private sealed class FailingCompletionStore(ICutlistRatingStore store) : ICutlistRatingStore
    {
        public CutlistRatingState? Get(string key) => store.Get(key);
        public bool TryBegin(string key) => store.TryBegin(key);
        public void Complete(string key, CutlistRatingState state) => throw new IOException("disk full");
    }
    private static HttpClient Http(HttpStatusCode code, string body) => new(new Handler((r, t) =>
        Task.FromResult(new HttpResponseMessage(code) { Content = new StringContent(body) })));
    private sealed class Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handle) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => handle(request, token);
    }
}
