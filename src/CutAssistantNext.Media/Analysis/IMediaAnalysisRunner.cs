using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public interface IMediaAnalysisRunner
{
    Task<MediaAnalysisResult> RunAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default);
}
