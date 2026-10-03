namespace CutAssistantNext.Core.Media;

public sealed record VideoTimeBase
{
    public VideoTimeBase(int numerator, int denominator)
    {
        if (numerator <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numerator));
        }

        if (denominator <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(denominator));
        }

        Numerator = numerator;
        Denominator = denominator;
    }

    public int Numerator { get; }

    public int Denominator { get; }

    public decimal ToSeconds(long timestamp) =>
        (decimal)timestamp * Numerator / Denominator;
}

/// <summary>
/// One decoded frame. LocalIndex is zero-based within this probe result,
/// not an absolute file frame index. Timestamps retain the stream time base.
/// BestEffortTimestamp is kept separate from original PTS.
/// PacketDts is optional and must not determine presentation order.
/// </summary>
public sealed record VideoFrameInfo(
    int LocalIndex,
    long? Pts,
    long? BestEffortTimestamp,
    long? PacketDts,
    long? Duration,
    bool? IsKeyFrame,
    string? PictureType);

/// <summary>
/// Decoded frames in ffprobe output order, including seek preroll if returned.
/// Raw stream times have not been mapped to mpv playback time.
/// </summary>
public sealed record VideoFrameWindow(
    int StreamIndex,
    VideoTimeBase TimeBase,
    decimal? StreamStartTimeSeconds,
    decimal? ContainerStartTimeSeconds,
    IReadOnlyList<VideoFrameInfo> Frames);
