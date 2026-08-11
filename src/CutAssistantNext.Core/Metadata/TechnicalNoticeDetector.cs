using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Core.Metadata;

public static class TechnicalNoticeDetector
{
    public static IReadOnlyList<TechnicalNotice> Detect(
        string fileName,
        MediaAnalysisResult analysis)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(analysis);

        var notices =
            new List<TechnicalNotice>();

        var extension =
            Path.GetExtension(fileName);

        var formats =
            (analysis.FormatName ?? string.Empty)
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

        if (extension.Equals(
                ".avi",
                StringComparison.OrdinalIgnoreCase) &&
            formats.Contains(
                "mp4",
                StringComparer.OrdinalIgnoreCase))
        {
            notices.Add(
                new TechnicalNotice(
                    "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF."));
        }

        return notices;
    }
}
