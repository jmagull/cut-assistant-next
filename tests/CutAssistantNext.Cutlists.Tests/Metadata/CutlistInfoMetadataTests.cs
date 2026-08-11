using CutAssistantNext.Core.Metadata;
using CutAssistantNext.Cutlists.Metadata;

namespace CutAssistantNext.Cutlists.Tests.Metadata;

public sealed class CutlistInfoMetadataTests
{
    [Fact]
    public void FromTechnicalNotices_WithNotice_SetsOtherError()
    {
        var notices = new[]
        {
            new TechnicalNotice(
                "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.")
        };

        var metadata =
            CutlistInfoMetadata.FromTechnicalNotices(
                notices);

        Assert.True(metadata.OtherError);
        Assert.Equal(
            "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.",
            metadata.OtherErrorDescription);
    }

    [Fact]
    public void FromTechnicalNotices_WithNoNotices_ClearsOtherError()
    {
        var metadata =
            CutlistInfoMetadata.FromTechnicalNotices(
                Array.Empty<TechnicalNotice>());

        Assert.False(metadata.OtherError);
        Assert.Null(metadata.OtherErrorDescription);
    }

    [Fact]
    public void FromTechnicalNotices_WithMultipleNotices_CombinesDescriptions()
    {
        var notices = new[]
        {
            new TechnicalNotice(
                "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF."),
            new TechnicalNotice(
                "Zusätzlicher technischer Hinweis.")
        };

        var metadata =
            CutlistInfoMetadata.FromTechnicalNotices(
                notices);

        Assert.True(metadata.OtherError);
        Assert.Equal(
            "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF. Zusätzlicher technischer Hinweis.",
            metadata.OtherErrorDescription);
    }

    [Fact]
    public void Create_WithSuggestedMovieNameAndUserComment_PreservesValues()
    {
        var metadata = CutlistInfoMetadata.Create(
            suggestedMovieName:
                "Tatort S2026E01 - Borowski und das Haupt der Medusa [13.08.2026]",
            userComment:
                "Mit Cut Assistant Next geschnitten.",
            technicalNotices:
                Array.Empty<TechnicalNotice>());

        Assert.Equal(
            "Tatort S2026E01 - Borowski und das Haupt der Medusa [13.08.2026]",
            metadata.SuggestedMovieName);

        Assert.Equal(
            "Mit Cut Assistant Next geschnitten.",
            metadata.UserComment);

        Assert.False(metadata.OtherError);
        Assert.Null(metadata.OtherErrorDescription);
    }

    [Fact]
    public void Create_WithUserValuesAndTechnicalNotice_PreservesAllValues()
    {
        var notices = new[]
        {
            new TechnicalNotice(
                "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.")
        };

        var metadata = CutlistInfoMetadata.Create(
            suggestedMovieName:
                "Frieren E06 - Der Held des Dorfes [13.08.2026]",
            userComment:
                "Werbung vollständig entfernt.",
            technicalNotices:
                notices);

        Assert.Equal(
            "Frieren E06 - Der Held des Dorfes [13.08.2026]",
            metadata.SuggestedMovieName);

        Assert.Equal(
            "Werbung vollständig entfernt.",
            metadata.UserComment);

        Assert.True(metadata.OtherError);

        Assert.Equal(
            "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.",
            metadata.OtherErrorDescription);
    }

    [Fact]
    public void Create_WithAuthor_PreservesAuthor()
    {
        var metadata = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: "Mit Cut Assistant Next geschnitten.",
            technicalNotices: [],
            author: "joerg");

        Assert.Equal(
            "joerg",
            metadata.Author);
    }

    [Fact]
    public void Create_UsesClassicDefaultInfoValues()
    {
        var metadata = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: []);

        Assert.Equal(
            5,
            metadata.RatingByAuthor);

        Assert.False(
            metadata.EpgError);

        Assert.Null(
            metadata.ActualContent);

        Assert.False(
            metadata.MissingBeginning);

        Assert.False(
            metadata.MissingEnding);

        Assert.False(
            metadata.MissingVideo);

        Assert.False(
            metadata.MissingAudio);
    }
}
