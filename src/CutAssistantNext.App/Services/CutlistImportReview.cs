using System.IO;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services;

internal sealed record CutlistImportReview(CutPlan Plan, string? FrameRateNotice,
    CutlistKeepSegment? Fragment, int? MeasuredFrameCount, bool AudioEndsBeforeFragment, TimeSpan? LastFrameStart, bool FrameRateMismatch)
{
    // Decode only near-EOF candidates. The window also covers VFR tails for which average FPS is misleading.
    internal static bool NeedsTailProbe(IReadOnlyList<CutlistKeepSegment> cuts, TimeSpan duration) =>
        cuts.Count > 0 && (cuts.Count == 1 || cuts[^2].End < cuts[^1].Start) &&
        cuts[^1].Start >= duration - TimeSpan.FromSeconds(3) && cuts[^1].Start < duration;

    internal static TimeSpan GetVideoEnd(MediaAnalysisResult? media, TimeSpan mediaDuration)
    {
        var stream = media?.VideoStreams.FirstOrDefault();
        return StreamEnd(stream?.StartTimeSeconds, stream?.DurationSeconds, media?.StartTimeSeconds) ?? mediaDuration;
    }

    internal static CutlistImportReview Analyze(CutlistDocument document, TimeSpan mediaDuration,
        MediaAnalysisResult? media, VideoFrameWindow? tail = null)
    {
        var stream = media?.VideoStreams.FirstOrDefault();
        var mediaFps = stream?.FramesPerSecond;
        var cutlistFps = document.General.FramesPerSecond;
        var frameRateMismatch = CutlistEndFragmentDetector.IsKnownFrameRate(cutlistFps) &&
            CutlistEndFragmentDetector.IsKnownFrameRate(mediaFps) &&
            Math.Abs(cutlistFps.GetValueOrDefault() - mediaFps.GetValueOrDefault()) > 0.001;
        var frameRateNotice = !CutlistEndFragmentDetector.IsKnownFrameRate(cutlistFps)
            ? "Die Cutlist enthält keine gültige Bildrate. CAN prüft zeitbasierte Bereiche anhand der geladenen Aufnahme."
            : !CutlistEndFragmentDetector.IsKnownFrameRate(mediaFps)
                ? "Die Bildraten von Cutlist und Film sind nicht zuverlässig vergleichbar."
                : frameRateMismatch
                    ? $"Die Bildrate der Cutlist ({cutlistFps:0.###} fps) weicht vom Film ({mediaFps:0.###} fps) ab."
                    : null;

        var videoEnd = GetVideoEnd(media, mediaDuration);
        var measured = ReadMeasuredTail(tail, stream?.Index, videoEnd);
        if (measured is not null) videoEnd = measured.Value.End;

        if (document.Cuts.LastOrDefault() is { } last && last.End > videoEnd)
            throw new InvalidDataException(
                $"Der letzte Behaltebereich endet bei {last.End:c}. Das Videoende liegt bei {videoEnd:c}. " +
                "Bitte korrigiere die Cutlist vor dem Laden.");

        var frameStarts = measured?.Starts;
        var fragment = CutlistEndFragmentDetector.Find(document, videoEnd, mediaFps, frameStarts);
        var withinFragment = fragment is not null && frameStarts is { Length: > 0 } && frameStarts[0] <= fragment.Start
            ? frameStarts.Where(time => time >= fragment.Start && time < fragment.End).ToArray()
            : null;
        var audioEndsBeforeFragment = fragment is not null && media is { AudioStreams.Count: > 0 } &&
            media.AudioStreams.All(audio => StreamEnd(audio.StartTimeSeconds, audio.DurationSeconds,
                media.StartTimeSeconds) is { } end && end <= fragment.Start);
        return new(CutlistCutPlanBuilder.Build(document, TimeSpan.FromTicks(Math.Max(mediaDuration.Ticks, videoEnd.Ticks))),
            frameRateNotice, fragment, withinFragment?.Length, audioEndsBeforeFragment,
            withinFragment is { Length: > 0 } ? withinFragment[^1] : null, frameRateMismatch);
    }

    internal Mp4BoxSplitRange BuildMp4BoxRange(double fps)
    {
        var fragment = Fragment ?? throw new InvalidOperationException("Der letzte Behaltebereich fehlt.");
        if (MeasuredFrameCount is null)
            return Mp4BoxSplitRangeBuilder.Build(fragment.Start, fragment.Duration, fps);

        var end = MeasuredFrameCount > 1 ? LastFrameStart!.Value : fragment.End;
        return new(fragment.Start, end) { RequestedEnd = fragment.End };
    }

    private static TimeSpan? StreamEnd(double? start, double? duration, double? containerStart)
    {
        if (duration is not > 0 || !double.IsFinite(duration.Value) ||
            start is null || containerStart is null)
            return null;

        var end = start.Value - containerStart.Value + duration.Value;
        return double.IsFinite(end) && end > 0 && end < TimeSpan.MaxValue.TotalSeconds
            ? TimeSpan.FromSeconds(end)
            : null;
    }

    private static (TimeSpan[] Starts, TimeSpan End)? ReadMeasuredTail(VideoFrameWindow? tail,
        int? streamIndex, TimeSpan expectedEnd)
    {
        if (tail is null || tail.StreamIndex != streamIndex || tail.ContainerStartTimeSeconds is not { } origin ||
            tail.Frames.LastOrDefault() is not { Pts: { } lastPts, Duration: > 0 } last)
            return null;

        var starts = new TimeSpan[tail.Frames.Count];
        for (var i = 0; i < starts.Length; i++)
        {
            if (tail.Frames[i].Pts is not { } pts || (i > 0 && pts <= tail.Frames[i - 1].Pts))
                return null;
            starts[i] = TimeSpan.FromTicks((long)decimal.Round(
                (tail.TimeBase.ToSeconds(pts) - origin) * TimeSpan.TicksPerSecond));
        }

        var exclusiveEnd = TimeSpan.FromTicks((long)decimal.Round(
            (tail.TimeBase.ToSeconds(lastPts) + tail.TimeBase.ToSeconds(last.Duration!.Value) - origin) *
            TimeSpan.TicksPerSecond));
        // Only use a demonstrably complete tail. Stream metadata may be rounded.
        return (exclusiveEnd - expectedEnd).Duration() <= TimeSpan.FromMilliseconds(1)
            ? (starts, exclusiveEnd)
            : null;
    }
}
