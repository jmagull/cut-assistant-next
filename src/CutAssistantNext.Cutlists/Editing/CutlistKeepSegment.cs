namespace CutAssistantNext.Cutlists.Editing;

public sealed class CutlistKeepSegment
{
    public CutlistKeepSegment(
        TimeSpan start,
        TimeSpan duration)
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

        Start = start;
        Duration = duration;
    }

    public TimeSpan Start { get; }

    public TimeSpan Duration { get; }

    public TimeSpan End =>
        Start + Duration;
}
