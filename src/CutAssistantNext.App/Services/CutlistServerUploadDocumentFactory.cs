using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Services;

internal static class CutlistServerUploadDocumentFactory
{
    internal static CutlistDocument Create(
        CutlistDocument source)
    {
        ArgumentNullException.ThrowIfNull(
            source);

        var general =
            new CutlistGeneralMetadata
            {
                Application =
                    "Cut Assistant",
                Version =
                    "0.26.5.6",
                FramesPerSecond =
                    source.General.FramesPerSecond,
                DisplayAspectRatio =
                    source.General.DisplayAspectRatio,
                IntendedCutApplicationName =
                    source.General.IntendedCutApplicationName,
                IntendedCutApplication =
                    source.General.IntendedCutApplication,
                IntendedCutApplicationVersion =
                    source.General.IntendedCutApplicationVersion,
                IntendedCutApplicationOptions =
                    source.General.IntendedCutApplicationOptions,
                NoOfCuts =
                    source.General.NoOfCuts,
                ApplyToFile =
                    source.General.ApplyToFile,
                OriginalFileSizeBytes =
                    source.General.OriginalFileSizeBytes
            };

        var info =
            new CutlistInfoMetadata
            {
                SuggestedMovieName =
                    source.Info.SuggestedMovieName,
                UserComment =
                    source.Info.UserComment,
                Author =
                    "joerg",
                RatingByAuthor =
                    source.Info.RatingByAuthor,
                EpgError =
                    source.Info.EpgError,
                ActualContent =
                    source.Info.ActualContent,
                MissingBeginning =
                    source.Info.MissingBeginning,
                MissingEnding =
                    source.Info.MissingEnding,
                MissingVideo =
                    source.Info.MissingVideo,
                MissingAudio =
                    source.Info.MissingAudio,
                OtherError =
                    source.Info.OtherError,
                OtherErrorDescription =
                    source.Info.OtherErrorDescription
            };

        return new CutlistDocument(
            general,
            source.Cuts,
            info);
    }
}