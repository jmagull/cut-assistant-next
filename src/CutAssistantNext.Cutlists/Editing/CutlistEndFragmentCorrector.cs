using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Editing;

public static class CutlistEndFragmentCorrector
{
    public static CutPlan BuildCutPlan(
        CutlistDocument document,
        TimeSpan mediaDuration,
        CutlistKeepSegment fragment)
    {
        ArgumentNullException.ThrowIfNull(
            document);

        ArgumentNullException.ThrowIfNull(
            fragment);

        if (document.Cuts.Count == 0 ||
            !ReferenceEquals(
                document.Cuts[^1],
                fragment))
        {
            throw new ArgumentException(
                "Der zu korrigierende Bereich muss der letzte Cutlist-Bereich sein.",
                nameof(fragment));
        }

        var correctedKeepSegments =
            document.Cuts
                .Take(
                    document.Cuts.Count - 1)
                .ToArray();

        var removeSegments =
            CutlistRemoveSegmentBuilder.Build(
                mediaDuration,
                correctedKeepSegments);

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
