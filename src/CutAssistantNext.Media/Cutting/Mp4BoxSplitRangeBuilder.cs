namespace CutAssistantNext.Media.Cutting;

public sealed record Mp4BoxSplitRange(
    TimeSpan Start,
    TimeSpan End);

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

        var end =
            start + duration - frameDuration;

        return new Mp4BoxSplitRange(
            start,
            end);
    }
}
