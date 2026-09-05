using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistServerClientTests
{
    [Fact]
    public void BuildSearchUri_UsesPersonalServerUrlAndEncodedMovieName()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var movieName =
            "Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE";

        var uri =
            CutlistServerClient.BuildSearchUri(
                personalServerUrl,
                movieName);

        Assert.Equal(
            personalServerUrl
            + "getxml.php"
            + "?name=Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE",
            uri.AbsoluteUri);
    }
    [Fact]
    public void BuildSearchUri_EncodesMovieName()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var movieName =
            "Brilliant Minds - Ärger & Spaß";

        var uri =
            CutlistServerClient.BuildSearchUri(
                personalServerUrl,
                movieName);

        Assert.Equal(
            personalServerUrl
            + "getxml.php"
            + "?name=Brilliant%20Minds%20-%20%C3%84rger%20%26%20Spa%C3%9F",
            uri.AbsoluteUri);
    }
    [Fact]
    public async Task SearchRawAsync_SendsGetAndReturnsResponseBody()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var movieName =
            "Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE";

        const string responseBody =
            "<cutlists></cutlists>";

        using var handler =
            new RecordingHttpMessageHandler(
                responseBody);

        using var httpClient =
            new HttpClient(
                handler);

        var client =
            new CutlistServerClient(
                httpClient);

        var result =
            await client.SearchRawAsync(
                personalServerUrl,
                movieName);

        Assert.Equal(
            responseBody,
            result);

        Assert.Equal(
            HttpMethod.Get,
            handler.RequestMethod);

        Assert.Equal(
            personalServerUrl
            + "getxml.php"
            + "?name=Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE",
            handler.RequestUri?.AbsoluteUri);
    }

    [Fact]
    public async Task SearchAsync_ParsesCutlistSearchResults()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var movieName =
            "Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE";

        const string responseBody =
            """
            <cutlists>
              <cutlist>
                <id>4711</id>
                <name>Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE.HQ.mp4.cutlist</name>
                <rating>5</rating>
                <ratingcount>3</ratingcount>
                <ratingbyauthor>5</ratingbyauthor>
                <author>joerg</author>
                <filename>Brilliant Minds</filename>
                <usercomment>Werbung vollständig entfernt.</usercomment>
                <actualcontent>Brilliant Minds</actualcontent>
                <cuts>3</cuts>
                <duration>2387.72</duration>
                <downloadcount>17</downloadcount>
                <errors>000000</errors>
              </cutlist>
            </cutlists>
            """;

        using var handler =
            new RecordingHttpMessageHandler(
                responseBody);

        using var httpClient =
            new HttpClient(
                handler);

        var client =
            new CutlistServerClient(
                httpClient);

        var results =
            await client.SearchAsync(
                personalServerUrl,
                movieName);

        var result =
            Assert.Single(
                results);

        Assert.Equal(
            "4711",
            result.Id);

        Assert.Equal(
            "Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE.HQ.mp4.cutlist",
            result.CutlistFileName);

        Assert.Equal(
            "5",
            result.Rating);

        Assert.Equal(
            "3",
            result.RatingCount);

        Assert.Equal(
            "5",
            result.RatingByAuthor);

        Assert.Equal(
            "joerg",
            result.Author);

        Assert.Equal(
            "Brilliant Minds",
            result.SuggestedMovieName);

        Assert.Equal(
            "Werbung vollständig entfernt.",
            result.UserComment);

        Assert.Equal(
            "Brilliant Minds",
            result.ActualContent);
        Assert.Equal(
            "3",
            result.Cuts);

        Assert.Equal(
            "2387.72",
            result.Duration);

        Assert.Equal(
            "17",
            result.DownloadCount);

        Assert.Equal(
            "000000",
            result.Errors);
    }

    [Fact]
    public async Task SearchAsync_ParsesMultipleCutlistSearchResults()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var movieName =
            "Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE";

        const string responseBody =
            """
            <cutlists>
              <cutlist>
                <id>4711</id>
                <name>Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE.HQ.mp4.cutlist</name>
                <rating>5</rating>
                <ratingcount>3</ratingcount>
                <ratingbyauthor>5</ratingbyauthor>
                <author>joerg</author>
                <filename>Brilliant Minds</filename>
                <usercomment>Werbung vollständig entfernt.</usercomment>
                <actualcontent>Brilliant Minds</actualcontent>
              </cutlist>
              <cutlist>
                <id>8150</id>
                <name>Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE.HQ.mp4.cutlist</name>
                <rating>4.5</rating>
                <ratingcount>8</ratingcount>
                <ratingbyauthor>5</ratingbyauthor>
                <author>andererautor</author>
                <filename>Brilliant Minds</filename>
                <usercomment>Alternative Schnittfassung.</usercomment>
                <actualcontent>Brilliant Minds</actualcontent>
              </cutlist>
            </cutlists>
            """;

        using var handler =
            new RecordingHttpMessageHandler(
                responseBody);

        using var httpClient =
            new HttpClient(
                handler);

        var client =
            new CutlistServerClient(
                httpClient);

        var results =
            await client.SearchAsync(
                personalServerUrl,
                movieName);

        Assert.Equal(
            2,
            results.Count);

        Assert.Equal(
            "8150",
            results[0].Id);

        Assert.Equal(
            "andererautor",
            results[0].Author);

        Assert.Equal(
            "4.5",
            results[0].Rating);

        Assert.Equal(
            "4711",
            results[1].Id);

        Assert.Equal(
            "joerg",
            results[1].Author);
    }

[Fact]
public async Task SearchAsync_ReturnsEmptyListForEmptyResponse()
{
    var personalServerUrl =
        "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

    var movieName =
        "CAN_DIESEN_FILM_GIBT_ES_GARANTIERT_NICHT_987654321";

    using var handler =
        new RecordingHttpMessageHandler(
            string.Empty);

    using var httpClient =
        new HttpClient(
            handler);

    var client =
        new CutlistServerClient(
            httpClient);

    var results =
        await client.SearchAsync(
            personalServerUrl,
            movieName);

    Assert.Empty(
        results);
}

    [Fact]
    public void BuildSearchUri_PreservesFullOriginalFileName()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var originalFileName =
            "Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE.HQ.mp4";

        var uri =
            CutlistServerClient.BuildSearchUri(
                personalServerUrl,
                originalFileName);

        Assert.Equal(
            personalServerUrl
            + "getxml.php"
            + "?name=Brilliant_Minds_26.08.21_21-05_5plus_60_TVOON_DE.HQ.mp4",
            uri.AbsoluteUri);
    }

    private sealed class RecordingHttpMessageHandler
        : HttpMessageHandler
    {
        private readonly string _responseBody;

        internal RecordingHttpMessageHandler(
            string responseBody)
        {
            _responseBody =
                responseBody;
        }

        internal HttpMethod? RequestMethod { get; private set; }

        internal Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestMethod =
                request.Method;

            RequestUri =
                request.RequestUri;

            var response =
                new HttpResponseMessage(
                    System.Net.HttpStatusCode.OK)
                {
                    Content =
                        new StringContent(
                            _responseBody)
                };

            return Task.FromResult(
                response);
        }
    }
}
