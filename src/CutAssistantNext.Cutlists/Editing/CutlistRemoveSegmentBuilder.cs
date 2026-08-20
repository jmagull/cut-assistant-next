using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Cutlists.Editing;

public static class CutlistRemoveSegmentBuilder
{
    public static IReadOnlyList<RemoveSegment> Build(
        TimeSpan mediaDuration,
        IReadOnlyList<CutlistKeepSegment> keepSegments)
    {
        if (mediaDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mediaDuration));
        }

        ArgumentNullException.ThrowIfNull(
            keepSegments);

        var removeSegments =
            new List<RemoveSegment>();

        var currentPosition =
            TimeSpan.Zero;

        foreach (var keepSegment in keepSegments)
        {
            if (keepSegment.Start > currentPosition)
            {
                removeSegments.Add(
                    new RemoveSegment(
                        currentPosition,
                        keepSegment.Start));
            }

            currentPosition =
                keepSegment.End;
        }

        if (currentPosition < mediaDuration)
        {
            removeSegments.Add(
                new RemoveSegment(
                    currentPosition,
                    mediaDuration));
        }

        return removeSegments;
    }
}
