using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public interface IFrameAnalysisRunner
{
    /// <summary>
    /// Probe an interval using ffprobe's timestamp coordinates.
    /// The requested seek start may differ from the first returned frame.
    /// videoStreamIndex is the absolute stream index, not its video ordinal.
    /// </summary>
    Task<VideoFrameWindow> RunAsync(
        string mediaFilePath,
        int videoStreamIndex,
        TimeSpan intervalStart,
        TimeSpan intervalEnd,
        CancellationToken cancellationToken = default);
}
