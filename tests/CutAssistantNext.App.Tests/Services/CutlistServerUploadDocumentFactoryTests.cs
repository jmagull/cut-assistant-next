using CutAssistantNext.App.Services;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistServerUploadDocumentFactoryTests
{
    [Fact]
    public void Create_CreatesServerCompatibleCopyWithoutChangingOriginal()
    {
        var general =
            new CutlistGeneralMetadata
            {
                Application = "Cut Assistant Next",
                Version = "1.2.3",
                FramesPerSecond = 25,
                DisplayAspectRatio = "16:9",
                IntendedCutApplicationName = "MP4Box",
                IntendedCutApplication = "mp4box.exe",
                IntendedCutApplicationVersion = "26.07",
                IntendedCutApplicationOptions = "-splitx",
                NoOfCuts = 0,
                ApplyToFile =
                    "Test_26.09.08_20-15_sender_60_TVOON_DE.HQ.mp4",
                OriginalFileSizeBytes = 123456789
            };

        var info =
            new CutlistInfoMetadata
            {
                SuggestedMovieName =
                    "Testfilm S01E02 - Testfolge",
                UserComment =
                    "Mein eigener Kommentar.",
                Author =
                    "Joerg",
                RatingByAuthor = 4,
                EpgError = true,
                ActualContent =
                    "Tatsächlicher Inhalt",
                MissingBeginning = true,
                MissingEnding = false,
                MissingVideo = true,
                MissingAudio = false,
                OtherError = true,
                OtherErrorDescription =
                    "Technischer Hinweis."
            };

        IReadOnlyList<CutlistKeepSegment> cuts =
            Array.Empty<CutlistKeepSegment>();

        var source =
            new CutlistDocument(
                general,
                cuts,
                info);

        var result =
            CutlistServerUploadDocumentFactory.Create(
                source);

        Assert.NotSame(
            source,
            result);

        Assert.NotSame(
            source.General,
            result.General);

        Assert.NotSame(
            source.Info,
            result.Info);

        Assert.Equal(
            "Cut Assistant",
            result.General.Application);

        Assert.Equal(
            "0.26.5.6",
            result.General.Version);

        Assert.Equal(
            "joerg",
            result.Info.Author);

        Assert.Equal(
            "Mein eigener Kommentar.",
            result.Info.UserComment);

        Assert.Equal(
            source.General.FramesPerSecond,
            result.General.FramesPerSecond);

        Assert.Equal(
            source.General.DisplayAspectRatio,
            result.General.DisplayAspectRatio);

        Assert.Equal(
            source.General.IntendedCutApplicationName,
            result.General.IntendedCutApplicationName);

        Assert.Equal(
            source.General.IntendedCutApplication,
            result.General.IntendedCutApplication);

        Assert.Equal(
            source.General.IntendedCutApplicationVersion,
            result.General.IntendedCutApplicationVersion);

        Assert.Equal(
            source.General.IntendedCutApplicationOptions,
            result.General.IntendedCutApplicationOptions);

        Assert.Equal(
            source.General.NoOfCuts,
            result.General.NoOfCuts);

        Assert.Equal(
            source.General.ApplyToFile,
            result.General.ApplyToFile);

        Assert.Equal(
            source.General.OriginalFileSizeBytes,
            result.General.OriginalFileSizeBytes);

        Assert.Same(
            source.Cuts,
            result.Cuts);

        Assert.Equal(
            source.Info.SuggestedMovieName,
            result.Info.SuggestedMovieName);

        Assert.Equal(
            source.Info.RatingByAuthor,
            result.Info.RatingByAuthor);

        Assert.Equal(
            source.Info.EpgError,
            result.Info.EpgError);

        Assert.Equal(
            source.Info.ActualContent,
            result.Info.ActualContent);

        Assert.Equal(
            source.Info.MissingBeginning,
            result.Info.MissingBeginning);

        Assert.Equal(
            source.Info.MissingEnding,
            result.Info.MissingEnding);

        Assert.Equal(
            source.Info.MissingVideo,
            result.Info.MissingVideo);

        Assert.Equal(
            source.Info.MissingAudio,
            result.Info.MissingAudio);

        Assert.Equal(
            source.Info.OtherError,
            result.Info.OtherError);

        Assert.Equal(
            source.Info.OtherErrorDescription,
            result.Info.OtherErrorDescription);

        Assert.Equal(
            "Cut Assistant Next",
            source.General.Application);

        Assert.Equal(
            "1.2.3",
            source.General.Version);

        Assert.Equal(
            "Joerg",
            source.Info.Author);

        Assert.Equal(
            "Mein eigener Kommentar.",
            source.Info.UserComment);
    }
}