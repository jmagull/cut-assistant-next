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
        return Create(
            suggestedMovieName,
            userComment,
            technicalNotices,
            ratingByAuthor: 5,
            author: author);
    }

    public static CutlistInfoMetadata Create(
        string? suggestedMovieName,
        string? userComment,
        IReadOnlyCollection<TechnicalNotice> technicalNotices,
        int ratingByAuthor,
        string? author = null,
        bool epgError = false,
        string? actualContent = null,
        bool missingBeginning = false,
        bool missingEnding = false,
        bool missingVideo = false,
        bool missingAudio = false,
        bool otherError = false,
        string? otherErrorDescription = null)
    {
        ArgumentNullException.ThrowIfNull(technicalNotices);
        if (ratingByAuthor is < 0 or > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ratingByAuthor),
                ratingByAuthor,
                "Die Bewertung muss zwischen 0 und 5 liegen.");
        }


        var technicalMetadata =
            FromTechnicalNotices(technicalNotices);

        var descriptions =
            new[]
            {
                otherError
                    ? otherErrorDescription
                    : null,
                technicalMetadata.OtherErrorDescription
            }
            .Where(description =>
                !string.IsNullOrWhiteSpace(description))
            .ToArray();

        var combinedOtherErrorDescription =
            descriptions.Length > 0
                ? string.Join(
                    " ",
                    descriptions)
                : null;

        return new CutlistInfoMetadata
        {
            SuggestedMovieName = suggestedMovieName,
            UserComment = userComment,
            Author = author,
            RatingByAuthor = ratingByAuthor,
            EpgError = epgError,
            ActualContent = actualContent,
            MissingBeginning = missingBeginning,
            MissingEnding = missingEnding,
            MissingVideo = missingVideo,
            MissingAudio = missingAudio,
            OtherError =
                otherError
                || technicalMetadata.OtherError,
            OtherErrorDescription =
                combinedOtherErrorDescription
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
