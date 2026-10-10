namespace CutAssistantNext.Core.Cutting;

public sealed class KeepSegment
{
    public KeepSegment(TimeSpan start, TimeSpan duration)
    {
        if (start < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (duration <= TimeSpan.Zero || start.Ticks > TimeSpan.MaxValue.Ticks - duration.Ticks)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        Start = start;
        Duration = duration;
    }

    public TimeSpan Start { get; }

    public TimeSpan Duration { get; }

    public TimeSpan End => Start + Duration;
}
