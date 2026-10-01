using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistServerClientTests
{
    [Theory]
    [InlineData("http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    public void BuildSearchUri_UsesPersonalServerUrlAndEncodedMovieName(string personalServerUrl)
    {

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

    [Theory]
    [InlineData("http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    public void BuildDownloadUri_UsesPersonalServerUrlAndCutlistId(string personalServerUrl)
    {

        var cutlistId =
            "2078205";

        var uri =
            CutlistServerClient.BuildDownloadUri(
                personalServerUrl,
                cutlistId);

        Assert.Equal(
            personalServerUrl
            + "getfile.php"
            + "?id=2078205",
            uri.AbsoluteUri);
    }

    [Fact]
    public async Task DownloadBytesAsync_SendsGetAndReturnsResponseBytes()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var cutlistId =
            "2078205";

        var responseBytes =
            new byte[]
            {
                0x5B,
                0x47,
                0x65,
                0x6E,
                0x5D,
                0x0D,
                0x0A,
                0xE4,
                0xF6,
                0xFC
            };

        using var handler =
            new RecordingByteHttpMessageHandler(
                responseBytes);

        using var httpClient =
            new HttpClient(
                handler);

        var client =
            new CutlistServerClient(
                httpClient);

        var result =
            await client.DownloadBytesAsync(
                personalServerUrl,
                cutlistId);

        Assert.Equal(
            responseBytes,
            result);

        Assert.Equal(
            HttpMethod.Get,
            handler.RequestMethod);

        Assert.Equal(
            personalServerUrl
            + "getfile.php"
            + "?id=2078205",
            handler.RequestUri?.AbsoluteUri);
    }
    [Theory]
    [InlineData("http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    public async Task UploadBytesAsync_SendsLegacyMultipartRequestToPersonalServerUrl(string personalServerUrl)
    {

        var cutlistFileName =
            "Test_26.09.08_20-15_sender_60_TVOON_DE.HQ.mp4.cutlist";

        var cutlistBytes =
            new byte[]
            {
                0x5B,
                0x47,
                0x65,
                0x6E,
                0x65,
                0x72,
                0x61,
                0x6C,
                0x5D,
                0x0D,
                0x0A
            };

        using var handler =
            new RecordingUploadHttpMessageHandler();

        using var httpClient =
            new HttpClient(
                handler);

        var client =
            new CutlistServerClient(
                httpClient);

        var response =
            await client.UploadBytesAsync(
                personalServerUrl,
                cutlistFileName,
                cutlistBytes,
                "1.0.0");

        Assert.Equal(
            HttpMethod.Post,
            handler.RequestMethod);

        Assert.Equal(
            personalServerUrl,
            handler.RequestUri?.AbsoluteUri);

        Assert.StartsWith(
            "multipart/form-data;",
            handler.ContentType);

        Assert.Equal(
            "1587200",
            handler.FormFields["MAX_FILE_SIZE"]);

        Assert.Equal(
            "True",
            handler.FormFields["confirm"]);

        Assert.Equal(
            "blank",
            handler.FormFields["type"]);

        Assert.Equal(
            "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
            handler.FormFields["userid"]);

        Assert.Equal(
            "CutAssistantNext",
            handler.FormFields["app"]);

        Assert.Equal(
            "1.0.0",
            handler.FormFields["version"]);

        Assert.Equal(
            "userfile[]",
            handler.FileFieldName);

        Assert.Equal(
            cutlistFileName,
            handler.FileName);

        Assert.Equal(
            cutlistBytes,
            handler.FileBytes);

        Assert.Equal(
            "id=2079999\nUpload erfolgreich",
            response);
    }

    [Fact]
    public void ParseUploadResponse_ReturnsCutlistIdAndLastNonEmptyMessage()
    {
        const string responseBody =
            "result=ok\r\n" +
            "id=2079999\r\n" +
            "Upload erfolgreich\r\n";

        var result =
            CutlistServerClient.ParseUploadResponse(
                responseBody);

        Assert.Equal(
            "2079999",
            result.CutlistId);

        Assert.Equal(
            "Upload erfolgreich",
            result.Message);
    }

    [Fact]
    public async Task UploadAsync_SendsUploadAndReturnsParsedResult()
    {
        var personalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var cutlistFileName =
            "Test_26.09.08_20-15_sender_60_TVOON_DE.HQ.mp4.cutlist";

        var cutlistBytes =
            new byte[]
            {
                0x5B,
                0x47,
                0x65,
                0x6E,
                0x65,
                0x72,
                0x61,
                0x6C,
                0x5D
            };

        using var handler =
            new RecordingUploadHttpMessageHandler();

        using var httpClient =
            new HttpClient(
                handler);

        var client =
            new CutlistServerClient(
                httpClient);

        var result =
            await client.UploadAsync(
                personalServerUrl,
                cutlistFileName,
                cutlistBytes,
                ApplicationVersion.NumericVersion);

        Assert.Equal(
            ApplicationVersion.NumericVersion,
            handler.FormFields["version"]);

        Assert.Equal(
            "2079999",
            result.CutlistId);

        Assert.Equal(
            "Upload erfolgreich",
            result.Message);

        Assert.Equal(
            HttpMethod.Post,
            handler.RequestMethod);

        Assert.Equal(
            personalServerUrl,
            handler.RequestUri?.AbsoluteUri);
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

    private sealed class RecordingByteHttpMessageHandler
        : HttpMessageHandler
    {
        private readonly byte[] _responseBody;

        internal RecordingByteHttpMessageHandler(
            byte[] responseBody)
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
                        new ByteArrayContent(
                            _responseBody)
                };

            return Task.FromResult(
                response);
        }
    }

    private sealed class RecordingUploadHttpMessageHandler :
        HttpMessageHandler
    {
        internal HttpMethod? RequestMethod { get; private set; }

        internal Uri? RequestUri { get; private set; }

        internal string? ContentType { get; private set; }

        internal Dictionary<string, string> FormFields { get; } =
            new(StringComparer.Ordinal);

        internal string? FileFieldName { get; private set; }

        internal string? FileName { get; private set; }

        internal byte[] FileBytes { get; private set; } =
            [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestMethod =
                request.Method;

            RequestUri =
                request.RequestUri;

            ContentType =
                request.Content?.Headers.ContentType?.ToString();

            if (request.Content is MultipartFormDataContent multipartContent)
            {
                foreach (var part in multipartContent)
                {
                    var contentDisposition =
                        part.Headers.ContentDisposition;

                    var fieldName =
                        contentDisposition?.Name?.Trim('"');

                    if (string.IsNullOrWhiteSpace(
                            fieldName))
                    {
                        continue;
                    }

                    var fileName =
                        (contentDisposition?.FileNameStar
                         ?? contentDisposition?.FileName)?.Trim('"');

                    if (!string.IsNullOrWhiteSpace(
                            fileName))
                    {
                        FileFieldName =
                            fieldName;

                        FileName =
                            fileName;

                        FileBytes =
                            await part.ReadAsByteArrayAsync(
                                cancellationToken);

                        continue;
                    }

                    FormFields[fieldName] =
                        await part.ReadAsStringAsync(
                            cancellationToken);
                }
            }

            return new HttpResponseMessage(
                System.Net.HttpStatusCode.OK)
            {
                Content =
                    new StringContent(
                        "id=2079999\nUpload erfolgreich")
            };
        }
    }
}
