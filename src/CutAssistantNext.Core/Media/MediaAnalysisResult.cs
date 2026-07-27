namespace CutAssistantNext.Core.Media;

public sealed record MediaAnalysisResult(
    string? FormatName,
    string? FormatLongName,
    long? FileSizeBytes,
    TimeSpan? Duration,
    IReadOnlyList<VideoStreamInfo> VideoStreams,
    IReadOnlyList<AudioStreamInfo> AudioStreams);

public sealed record VideoStreamInfo(
    int Index,
    string? CodecName,
    string? CodecLongName,
    int? Width,
    int? Height,
    string? SampleAspectRatio,
    string? DisplayAspectRatio,
    double? FramesPerSecond,
    string? FieldOrder);

public sealed record AudioStreamInfo(
    int Index,
    string? CodecName,
    string? CodecLongName,
    int? SampleRate,
    int? Channels,
    string? ChannelLayout);
