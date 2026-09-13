namespace CutAssistantNext.Cutlists.Editing;

public static class CutlistEndFragmentDetector
{
    private const double MaximumFragmentFrames = 3;

    public static CutlistKeepSegment? Find(
        Model.CutlistDocument document,
        TimeSpan mediaDuration)
    {
        ArgumentNullException.ThrowIfNull(
            document);

        if (mediaDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mediaDuration));
        }

        var framesPerSecond =
            document.General.FramesPerSecond;

        if (!framesPerSecond.HasValue ||
            framesPerSecond.Value <= 0 ||
            document.Cuts.Count < 2)
        {
            return null;
        }

        var lastSegment =
            document.Cuts[^1];

        var previousSegment =
            document.Cuts[^2];

        var maximumFragmentDuration =
            TimeSpan.FromSeconds(
                MaximumFragmentFrames /
                framesPerSecond.Value);

        var maximumEndDistance =
            TimeSpan.FromSeconds(
                2 /
                framesPerSecond.Value);

        var maximumEndOvershoot =
            TimeSpan.FromSeconds(
                1 /
                framesPerSecond.Value);

        var distanceToMediaEnd =
            mediaDuration -
            lastSegment.End;

        if (lastSegment.Duration > maximumFragmentDuration ||
            previousSegment.End >= lastSegment.Start ||
            distanceToMediaEnd < -maximumEndOvershoot ||
            distanceToMediaEnd > maximumEndDistance)
        {
            return null;
        }

        return lastSegment;
    }
}
