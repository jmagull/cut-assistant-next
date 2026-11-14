namespace CutAssistantNext.Media.Cutting;

public sealed record Mp4BoxSplitRange(
    TimeSpan Start,
    TimeSpan End)
{
    public TimeSpan? RequestedEnd { get; init; }
}

public static class Mp4BoxSplitRangeBuilder
{
    public static Mp4BoxSplitRange Build(
        TimeSpan start,
        TimeSpan duration,
        double framesPerSecond)
    {
        if (start < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(start));
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration));
        }

        if (framesPerSecond <= 0 ||
            double.IsNaN(framesPerSecond) ||
            double.IsInfinity(framesPerSecond))
        {
            throw new ArgumentOutOfRangeException(
                nameof(framesPerSecond));
        }

        var frameDuration =
            TimeSpan.FromSeconds(
                1d / framesPerSecond);

        var requestedEnd = start + duration;
        var end = requestedEnd - frameDuration;
        // One frame is a valid keep interval. Pass its exclusive end when the usual adjustment collapses it.
        if (!Mp4BoxSplitRangeFormatter.HasIncreasingBounds(new(start, end)))
            end = requestedEnd;

        return new Mp4BoxSplitRange(
            start,
            end) { RequestedEnd = requestedEnd };
    }
}
