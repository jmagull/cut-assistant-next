using System.Globalization;

namespace TransitionDiagnosis;

public sealed record VideoTimeBase(long Numerator, long Denominator)
{
    public decimal Seconds(long timestamp) => checked((decimal)timestamp * Numerator / Denominator);
    public decimal? Seconds(long? timestamp) => timestamp is { } value ? Seconds(value) : null;

    public static VideoTimeBase Parse(string text)
    {
        var parts = text.Split('/');
        if (parts.Length != 2 || !long.TryParse(parts[0], CultureInfo.InvariantCulture, out var n) ||
            !long.TryParse(parts[1], CultureInfo.InvariantCulture, out var d) || n <= 0 || d <= 0)
            throw new InvalidDataException("Ungültige Videozeitbasis.");
        return new(n, d);
    }
}

public sealed record KeepRange(decimal Start, decimal Duration)
{
    public decimal End => Start + Duration;
}

public sealed record VideoPacket(long? Pts, long? Dts, long? Duration, long Position, string Hash, string Flags);
public sealed record VideoFrame(long? Pts, long? Dts, long? BestEffortTimestamp, long? Duration,
    long? Position, string PictureType);
public sealed record VideoMetadata(VideoTimeBase TimeBase, decimal? VideoStart, decimal? VideoDuration,
    decimal? FormatDuration, string Codec, string AverageFrameRate);
public sealed record VideoSnapshot(VideoMetadata Metadata, IReadOnlyList<VideoPacket> Packets);
public sealed record PacketAssignment(int? SourceIndex, int? Keep, string? Issue);
public sealed record MappingIssue(int OutputPacket, string Reason);
public sealed record SegmentEvidence(int Keep, int PacketCount, decimal? MinimumPtsSeconds,
    decimal? MaximumPtsSeconds, decimal? FirstSourcePtsSeconds, decimal? LastSourcePtsSeconds);
public sealed record PacketEvidence(int OutputPacket, decimal? PtsSeconds, decimal? DtsSeconds,
    decimal? DurationSeconds, long Position, string Flags, int? SourcePacket, decimal? SourcePtsSeconds, int? Keep);
public sealed record FrameEvidence(decimal? PtsSeconds, decimal? DtsSeconds, decimal? BestEffortSeconds,
    decimal? DurationSeconds, long? Position, string PictureType, int? Keep, int? SourcePacket,
    decimal? SourcePtsSeconds);

public sealed class BoundaryEvidence
{
    public required int NewKeep { get; init; }
    public required decimal PlannedSeconds { get; init; }
    public decimal? PacketCandidateSeconds { get; init; }
    public decimal? CandidateMinusPlanSeconds => PacketCandidateSeconds - PlannedSeconds;
    public decimal? CandidateReviewStartSeconds => PacketCandidateSeconds is { } value ? Math.Max(0, value - 3) : null;
    public decimal? PreviousMaximumPtsSeconds { get; init; }
    public decimal? OverlapSeconds => PreviousMaximumPtsSeconds - PacketCandidateSeconds;
    public required string PacketPresentation { get; init; }
    public required IReadOnlyList<PacketEvidence> PacketNeighborhood { get; init; }
    public string DecodeStatus { get; set; } = "NotAttempted";
    public decimal? FirstDecodedNewKeepSeconds { get; set; }
    public IReadOnlyList<FrameEvidence> Frames { get; set; } = [];
    // Decoder observations alone do not certify what libmpv presents at a timestamp.
    public bool ReviewPositionConfirmed => false;
}

public sealed record DiagnosisResult(bool AllPacketsUniquelyMapped, bool KeepOrderMatchesPlan,
    int SourcePacketCount, int OutputPacketCount, int AmbiguousOrUnmatchedPackets,
    int SourceSequenceBreaks, int DiscardOrCorruptPackets, int ContextResolvedPackets, IReadOnlyList<MappingIssue> IssueExamples,
    IReadOnlyList<SegmentEvidence> Segments, IReadOnlyList<BoundaryEvidence> Boundaries);
