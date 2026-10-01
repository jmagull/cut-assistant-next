using System.Net.Http;
using System.Xml.Linq;
namespace CutAssistantNext.App.Services;
internal sealed record CutlistUploadResult(
    string CutlistId,
    string Message);


internal sealed class CutlistServerClient
{
    private readonly HttpClient _httpClient;

    internal CutlistServerClient(
        HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(
            httpClient);

        _httpClient =
            httpClient;
    }

    internal static Uri BuildSearchUri(
        string personalServerUrl,
        string movieName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            personalServerUrl);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            movieName);

        var baseUri =
            new Uri(
                personalServerUrl,
                UriKind.Absolute);

        var encodedMovieName =
            Uri.EscapeDataString(
                movieName);

        return new Uri(
            baseUri,
            "getxml.php?name="
            + encodedMovieName);
    }

    internal static Uri BuildDownloadUri(
        string personalServerUrl,
        string cutlistId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            personalServerUrl);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            cutlistId);

        var baseUri =
            new Uri(
                personalServerUrl,
                UriKind.Absolute);

        var encodedCutlistId =
            Uri.EscapeDataString(
                cutlistId);

        return new Uri(
            baseUri,
            "getfile.php?id="
            + encodedCutlistId);
    }

    internal async Task<byte[]> DownloadBytesAsync(
        string personalServerUrl,
        string cutlistId,
        CancellationToken cancellationToken = default)
    {
        var uri =
            BuildDownloadUri(
                personalServerUrl,
                cutlistId);

        using var response =
            await _httpClient.GetAsync(
                uri,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(
            cancellationToken);
    }

    internal static CutlistUploadResult ParseUploadResponse(
        string responseBody)
    {
        ArgumentNullException.ThrowIfNull(
            responseBody);

        var lines =
            responseBody
                .Replace(
                    "\r\n",
                    "\n",
                    StringComparison.Ordinal)
                .Split('\n')
                .Select(
                    line =>
                        line.Trim())
                .Where(
                    line =>
                        line.Length > 0)
                .ToArray();

        var cutlistId =
            string.Empty;

        foreach (var line in lines)
        {
            var separatorIndex =
                line.IndexOf(
                    '=');

            if (separatorIndex <= 0)
            {
                continue;
            }

            var name =
                line[..separatorIndex].Trim();

            var value =
                line[(separatorIndex + 1)..].Trim();

            if (string.Equals(
                    name,
                    "id",
                    StringComparison.OrdinalIgnoreCase) &&
                long.TryParse(
                    value,
                    out _))
            {
                cutlistId =
                    value;
            }
        }

        var message =
            lines.Length > 0
                ? lines[^1]
                : string.Empty;

        return new CutlistUploadResult(
            cutlistId,
            message);
    }

    internal async Task<string> UploadBytesAsync(
        string personalServerUrl,
        string cutlistFileName,
        byte[] cutlistBytes,
        string applicationVersion,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            personalServerUrl);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            cutlistFileName);

        ArgumentNullException.ThrowIfNull(
            cutlistBytes);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            applicationVersion);

        var uploadUri =
            new Uri(
                personalServerUrl,
                UriKind.Absolute);

        var userId =
            uploadUri.AbsolutePath.Trim('/');

        using var content =
            new MultipartFormDataContent();

        content.Add(
            new StringContent(
                "1587200"),
            "MAX_FILE_SIZE");

        content.Add(
            new StringContent(
                "True"),
            "confirm");

        content.Add(
            new StringContent(
                "blank"),
            "type");

        content.Add(
            new StringContent(
                userId),
            "userid");

        content.Add(
            new StringContent(
                "CutAssistantNext"),
            "app");

        content.Add(
            new StringContent(
                applicationVersion),
            "version");

        using var fileContent =
            new ByteArrayContent(
                cutlistBytes);

        content.Add(
            fileContent,
            "userfile[]",
            cutlistFileName);

        using var response =
            await _httpClient.PostAsync(
                uploadUri,
                content,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(
            cancellationToken);
    }

    internal async Task<CutlistUploadResult> UploadAsync(
        string personalServerUrl,
        string cutlistFileName,
        byte[] cutlistBytes,
        string applicationVersion,
        CancellationToken cancellationToken = default)
    {
        var responseBody =
            await UploadBytesAsync(
                personalServerUrl,
                cutlistFileName,
                cutlistBytes,
                applicationVersion,
                cancellationToken);

        return ParseUploadResponse(
            responseBody);
    }

    internal async Task<string> SearchRawAsync(
        string personalServerUrl,
        string movieName,
        CancellationToken cancellationToken = default)
    {
        var uri =
            BuildSearchUri(
                personalServerUrl,
                movieName);

        using var response =
            await _httpClient.GetAsync(
                uri,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(
            cancellationToken);
    }
    internal async Task<IReadOnlyList<CutlistSearchResult>> SearchAsync(
        string personalServerUrl,
        string movieName,
        CancellationToken cancellationToken = default)
    {
        var responseBody =
            await SearchRawAsync(
                personalServerUrl,
                movieName,
                cancellationToken);

        if (string.IsNullOrWhiteSpace(
                responseBody))
        {
            return Array.Empty<CutlistSearchResult>();
        }

        var document =
            XDocument.Parse(
                responseBody);

        var results =
            new List<CutlistSearchResult>();

        if (document.Root is null)
        {
            return results;
        }

        foreach (var element in document.Root.Elements())
        {
            var cutlistFileName =
                GetElementValue(
                    element,
                    "name");

            if (!cutlistFileName.EndsWith(
                    ".cutlist",
                    StringComparison.OrdinalIgnoreCase))
            {
                cutlistFileName +=
                    ".cutlist";
            }

            results.Add(
                new CutlistSearchResult
                {
                    Id =
                        GetElementValue(
                            element,
                            "id"),

                    CutlistFileName =
                        cutlistFileName,

                    Rating =
                        GetElementValue(
                            element,
                            "rating"),

                    RatingCount =
                        GetElementValue(
                            element,
                            "ratingcount"),

                    RatingByAuthor =
                        GetElementValue(
                            element,
                            "ratingbyauthor"),

                    Author =
                        GetElementValue(
                            element,
                            "author"),

                    SuggestedMovieName =
                        GetElementValue(
                            element,
                            "filename"),

                    UserComment =
                        GetElementValue(
                            element,
                            "usercomment"),

                    ActualContent =
                        GetElementValue(
                            element,
                            "actualcontent"),

                    Cuts =
                        GetElementValue(
                            element,
                            "cuts"),

                    Duration =
                        GetElementValue(
                            element,
                            "duration"),

                    DownloadCount =
                        GetElementValue(
                            element,
                            "downloadcount"),

                    Errors =
                        GetElementValue(
                            element,
                            "errors")
                });
        }

        return results
            .OrderByDescending(
                result =>
                    long.TryParse(
                        result.Id,
                        out var numericId)
                        ? numericId
                        : long.MinValue)
            .ToList();
    }

    private static string GetElementValue(
        XElement element,
        string elementName)
    {
        return element.Element(
                   elementName)?.Value
               ?? string.Empty;
    }
}