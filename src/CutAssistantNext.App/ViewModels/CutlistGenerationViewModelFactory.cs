using System.IO;
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
            OtrFileNameParser.TryParse(
                originalFileName,
                out var parsedContext)
                ? parsedContext
                : new NameTemplateContext(
                    Name: Path.GetFileNameWithoutExtension(
                        originalFileName),
                    OriginalName: originalFileName);

        var technicalNotices =
            TechnicalNoticeDetector.Detect(
                originalFileName,
                analysis);

        return new CutlistGenerationViewModel(
            settings,
            nameContext,
            technicalNotices);
    }
}
