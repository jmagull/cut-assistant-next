using System.Net.Http;
using System.Xml.Linq;
namespace CutAssistantNext.App.Services;

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