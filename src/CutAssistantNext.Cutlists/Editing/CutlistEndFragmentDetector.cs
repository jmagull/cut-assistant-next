using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Editing;

public static class CutlistEndFragmentDetector
{
    public static bool IsKnownFrameRate(double? value) => value is > 0 && double.IsFinite(value.Value);

    public static CutlistKeepSegment? Find(CutlistDocument document, TimeSpan mediaDuration,
        double? mediaFramesPerSecond = null, IReadOnlyList<TimeSpan>? measuredFrameStarts = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(mediaDuration, TimeSpan.Zero);
        if (document.Cuts.Count == 0)
            return null;

        var last = document.Cuts[^1];
        if ((document.Cuts.Count >= 2 && document.Cuts[^2].End >= last.Start) || last.End > mediaDuration)
            return null;

        if (measuredFrameStarts is { Count: > 0 } && measuredFrameStarts[0] <= last.Start)
        {
            var frames = measuredFrameStarts.Count(time => time >= last.Start && time < last.End);
            var after = measuredFrameStarts.Count(time => time >= last.End);
            return frames <= 3 && after <= 2 ? last : null;
        }

        var fps = IsKnownFrameRate(mediaFramesPerSecond) ? mediaFramesPerSecond : document.General.FramesPerSecond;
        if (!IsKnownFrameRate(fps))
            return null;

        return last.Duration.TotalSeconds * fps.GetValueOrDefault() <= 3 &&
               (mediaDuration - last.End).TotalSeconds * fps.GetValueOrDefault() <= 2 ? last : null;
    }
}
