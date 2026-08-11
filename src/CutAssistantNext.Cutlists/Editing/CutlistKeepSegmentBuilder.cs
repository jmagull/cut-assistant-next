using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Cutlists.Editing;

public static class CutlistKeepSegmentBuilder
{
    public static IReadOnlyList<CutlistKeepSegment> Build(
        CutPlan cutPlan)
    {
        ArgumentNullException.ThrowIfNull(cutPlan);

        var keepSegments =
            new List<CutlistKeepSegment>();

        var currentPosition =
            TimeSpan.Zero;

        foreach (var removeSegment in cutPlan.RemoveSegments)
        {
            if (removeSegment.Start > currentPosition)
            {
                keepSegments.Add(
                    new CutlistKeepSegment(
                        currentPosition,
                        removeSegment.Start - currentPosition));
            }

            currentPosition =
                removeSegment.End;
        }

        if (currentPosition < cutPlan.MediaDuration)
        {
            keepSegments.Add(
                new CutlistKeepSegment(
                    currentPosition,
                    cutPlan.MediaDuration - currentPosition));
        }

        return keepSegments;
    }
}
