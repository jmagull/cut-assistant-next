using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Editing;

public static class CutlistCutPlanBuilder
{
    public static CutPlan Build(
        CutlistDocument document,
        TimeSpan mediaDuration)
    {
        ArgumentNullException.ThrowIfNull(
            document);

        var removeSegments =
            CutlistRemoveSegmentBuilder.Build(
                mediaDuration,
                document.Cuts);

        var cutPlan =
            new CutPlan(
                mediaDuration);

        foreach (var removeSegment in removeSegments)
        {
            cutPlan.Add(
                removeSegment);
        }

        return cutPlan;
    }
}
