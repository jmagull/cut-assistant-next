namespace CutAssistantNext.App.Services;

internal sealed class CutlistSearchResult
{
    public string Id { get; init; } =
        string.Empty;

    public string CutlistFileName { get; init; } =
        string.Empty;

    public string Rating { get; init; } =
        string.Empty;

    public string RatingCount { get; init; } =
        string.Empty;

    public string RatingByAuthor { get; init; } =
        string.Empty;

    public string Author { get; init; } =
        string.Empty;

    public string SuggestedMovieName { get; init; } =
        string.Empty;

    public string UserComment { get; init; } =
        string.Empty;

    public string ActualContent { get; init; } =
        string.Empty;
    public string Cuts { get; init; } =
        string.Empty;

    public string Duration { get; init; } =
        string.Empty;

    public string DownloadCount { get; init; } =
        string.Empty;

    public string Errors { get; init; } =
        string.Empty;
    public string DisplayFormat
    {
        get
        {
            var container =
                CutlistFileName.EndsWith(
                    ".mp4.cutlist",
                    StringComparison.OrdinalIgnoreCase)
                    ? "MP4"
                    : CutlistFileName.EndsWith(
                        ".avi.cutlist",
                        StringComparison.OrdinalIgnoreCase)
                        ? "AVI"
                        : "Unbekannt";

            if (container == "Unbekannt")
            {
                return container;
            }

            var quality =
                CutlistFileName.Contains(
                    ".HQ.",
                    StringComparison.OrdinalIgnoreCase)
                    ? " HQ"
                    : CutlistFileName.Contains(
                        ".HD.",
                        StringComparison.OrdinalIgnoreCase)
                        ? " HD"
                        : string.Empty;

            return container + quality;
        }
    }
}
