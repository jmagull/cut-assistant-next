using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Services;

internal sealed record CutlistMediaContext(string FilePath, MediaAnalysisResult Analysis, int Generation)
{
    internal bool IsCurrent(string filePath, MediaAnalysisResult? analysis, int generation) =>
        Generation == generation && ReferenceEquals(Analysis, analysis) &&
        string.Equals(FilePath, filePath, StringComparison.OrdinalIgnoreCase);
}
