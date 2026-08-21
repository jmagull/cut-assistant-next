using System.IO;
using CutAssistantNext.App.State;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Metadata;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.ViewModels;

internal static class CutlistGenerationViewModelFactory
{
    public static CutlistGenerationViewModel Create(
        CutlistSettings settings,
        string fileName,
        MediaAnalysisResult analysis)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            fileName);

        ArgumentNullException.ThrowIfNull(
            analysis);

        var originalFileName =
            Path.GetFileName(fileName);

        var nameContext =
            NameTemplateContextFactory.Create(
                originalFileName);

        var technicalNotices =
            TechnicalNoticeDetector.Detect(
                originalFileName,
                analysis);

        return new CutlistGenerationViewModel(
            settings,
            nameContext,
            technicalNotices);
    }
    public static CutlistGenerationViewModel Create(
        CutlistSettings settings,
        string fileName,
        MediaAnalysisResult analysis,
        CutNamingState namingState)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            fileName);

        ArgumentNullException.ThrowIfNull(
            analysis);

        ArgumentNullException.ThrowIfNull(
            namingState);

        var originalFileName =
            Path.GetFileName(
                fileName);

        var technicalNotices =
            TechnicalNoticeDetector.Detect(
                originalFileName,
                analysis);

        return new CutlistGenerationViewModel(
            settings,
            namingState,
            technicalNotices);
    }
}
