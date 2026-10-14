using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public interface IKeyframeAnalysisRunner
{
    Task<VideoFrameWindow> RunKeyframesAsync(string mediaFilePath, int videoStreamIndex,
        CancellationToken cancellationToken = default);
}
