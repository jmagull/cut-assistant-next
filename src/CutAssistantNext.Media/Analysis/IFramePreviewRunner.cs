using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public sealed record FramePreview(byte[] PngBytes, long Pts, VideoTimeBase TimeBase);

public interface IFramePreviewRunner
{
    Task<FramePreview> RunAsync(
        string mediaFilePath,
        int videoStreamIndex,
        VideoTimeBase timeBase,
        long pts,
        CancellationToken cancellationToken = default);
}
