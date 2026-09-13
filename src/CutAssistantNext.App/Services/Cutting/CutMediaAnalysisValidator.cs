using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Services.Cutting;

internal static class CutMediaAnalysisValidator
{
    public static double GetFramesPerSecond(
        MediaAnalysisResult analysis)
    {
        ArgumentNullException.ThrowIfNull(
            analysis);

        if (analysis.VideoStreams.Count == 0)
        {
            throw new InvalidOperationException(
                "Es wurde kein Videostream gefunden.");
        }

        var framesPerSecond =
            analysis.VideoStreams[0].FramesPerSecond;

        if (!framesPerSecond.HasValue ||
            framesPerSecond.Value <= 0 ||
            !double.IsFinite(framesPerSecond.Value))
        {
            throw new InvalidOperationException(
                "Für den ersten Videostream wurde keine gültige Bildrate ermittelt.");
        }

        return framesPerSecond.Value;
    }
}
