using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Media.Cutting;

public interface ICutOutputAnalyzer
{
    Task<CutOutputTimeline> AnalyzeAsync(string outputFilePath, IReadOnlyList<string> segmentFilePaths,
        IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken);
}

public sealed record Mp4BoxWorkflowResult(CutOutputTimeline? Timeline, string? AnalysisWarning);
