using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal static class CutPlanMp4BoxRangeBuilder
{
    public static IReadOnlyList<Mp4BoxSplitRange> Build(
        CutPlan cutPlan,
        double framesPerSecond)
    {
        ArgumentNullException.ThrowIfNull(
            cutPlan);

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        var ranges =
            new List<Mp4BoxSplitRange>(
                keepSegments.Count);

        foreach (var keepSegment in keepSegments)
        {
            ranges.Add(
                Mp4BoxSplitRangeBuilder.Build(
                    keepSegment.Start,
                    keepSegment.Duration,
                    framesPerSecond));
        }

        return ranges;
    }
}
