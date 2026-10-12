namespace CutAssistantNext.Core.Editing;

/// <summary>Finds a review position in the film adjoining a removed range, without changing cuts.</summary>
public static class CutEdgeReviewPositionCalculator
{
    private const long ReviewOffsetTicks = 3 * TimeSpan.TicksPerSecond;

    public static TimeSpan? Calculate(CutPlan plan, RemoveSegment segment, CutEdgeSide side)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(segment);
        if (!Enum.IsDefined(side))
        {
            throw new ArgumentOutOfRangeException(nameof(side));
        }

        var index = -1;
        for (var candidate = 0; candidate < plan.RemoveSegments.Count; candidate++)
        {
            if (ReferenceEquals(plan.RemoveSegments[candidate], segment))
            {
                index = candidate;
                break;
            }
        }

        if (index < 0)
        {
            throw new InvalidOperationException("Die Schnittkante gehört nicht zum aktuellen Schnittplan.");
        }

        if (side == CutEdgeSide.Start)
        {
            if (segment.Start == TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }

            var previousEnd = index > 0 ? plan.RemoveSegments[index - 1].End.Ticks : 0;
            if (previousEnd >= segment.Start.Ticks)
            {
                return null;
            }

            var preferred = Math.Max(0, segment.Start.Ticks - ReviewOffsetTicks);
            return TimeSpan.FromTicks(Math.Max(previousEnd, preferred));
        }

        if (segment.End == plan.MediaDuration)
        {
            return plan.MediaDuration;
        }

        var nextStart = index + 1 < plan.RemoveSegments.Count
            ? plan.RemoveSegments[index + 1].Start.Ticks
            : plan.MediaDuration.Ticks;
        if (nextStart <= segment.End.Ticks)
        {
            return null;
        }

        var preferredAfter = segment.End.Ticks +
            Math.Min(ReviewOffsetTicks, plan.MediaDuration.Ticks - segment.End.Ticks);
        // A removed range includes its start, so stop strictly before the next removed range.
        var lastAllowed = index + 1 < plan.RemoveSegments.Count ? nextStart - 1 : nextStart;
        return TimeSpan.FromTicks(Math.Min(preferredAfter, lastAllowed));
    }
}
