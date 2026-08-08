namespace CutAssistantNext.Core.Editing;

public sealed class RemoveSegment
{
    public RemoveSegment(
        TimeSpan start,
        TimeSpan end)
    {
        if (start < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(start),
                "Der Anfang darf nicht vor dem Beginn des Videos liegen.");
        }

        if (end <= start)
        {
            throw new ArgumentOutOfRangeException(
                nameof(end),
                "Das Ende muss nach dem Anfang liegen.");
        }

        Start = start;
        End = end;
    }

    public TimeSpan Start { get; }

    public TimeSpan End { get; }

    public TimeSpan Duration =>
        End - Start;
}