namespace CutAssistantNext.Core.Editing;

/// <summary>Independent search steps; ordinary frame navigation does not advance this sequence.</summary>
public sealed class HalvingFrameSearch
{
    public const int DefaultInitialStep = 2000;

    public HalvingFrameSearch(int initialStep = DefaultInitialStep)
    {
        if (initialStep <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialStep));
        }

        InitialStep = initialStep;
        NextStep = initialStep;
    }

    public int InitialStep { get; }

    public int NextStep { get; private set; }

    /// <summary>Call only after a search jump has completed successfully.</summary>
    public void Advance()
    {
        NextStep = Math.Max(1, NextStep / 2);
    }

    public void Reset()
    {
        NextStep = InitialStep;
    }
}
