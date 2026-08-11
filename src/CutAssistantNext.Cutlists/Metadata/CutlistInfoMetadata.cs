using CutAssistantNext.Core.Metadata;

namespace CutAssistantNext.Cutlists.Metadata;

public sealed class CutlistInfoMetadata
{
    public string? SuggestedMovieName { get; init; }

    public string? UserComment { get; init; }
    public string? Author { get; init; }

    public int RatingByAuthor { get; init; } = 5;

    public bool EpgError { get; init; }

    public string? ActualContent { get; init; }

    public bool MissingBeginning { get; init; }

    public bool MissingEnding { get; init; }

    public bool MissingVideo { get; init; }

    public bool MissingAudio { get; init; }
public bool OtherError { get; init; }

    public string? OtherErrorDescription { get; init; }

    public static CutlistInfoMetadata Create(
        string? suggestedMovieName,
        string? userComment,
        IReadOnlyCollection<TechnicalNotice> technicalNotices,
        string? author = null)
    {
        ArgumentNullException.ThrowIfNull(technicalNotices);

        var technicalMetadata =
            FromTechnicalNotices(technicalNotices);

        return new CutlistInfoMetadata
        {
            SuggestedMovieName = suggestedMovieName,
            UserComment = userComment,
            Author = author,
            OtherError = technicalMetadata.OtherError,
            OtherErrorDescription =
                technicalMetadata.OtherErrorDescription
        };
    }

    public static CutlistInfoMetadata FromTechnicalNotices(
        IReadOnlyCollection<TechnicalNotice> notices)
    {
        ArgumentNullException.ThrowIfNull(notices);

        var descriptions = notices
            .Select(notice => notice.Description)
            .Where(description =>
                !string.IsNullOrWhiteSpace(description))
            .ToArray();

        return new CutlistInfoMetadata
        {
            OtherError = descriptions.Length > 0,
            OtherErrorDescription =
                descriptions.Length > 0
                    ? string.Join(
                        " ",
                        descriptions)
                    : null
        };
    }
}
