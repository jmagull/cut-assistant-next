namespace CutAssistantNext.Core.Editing;

/// <summary>Search steps halve only on successful direction changes; ordinary navigation is independent.</summary>
public sealed class HalvingFrameSearch
{
    private int? _lastDirection;
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

    /// <summary>Previews the step without consuming a failed or cancelled direction change.</summary>
    public int GetStep(int direction)
    {
        if (direction is not (-1 or 1))
        {
            throw new ArgumentOutOfRangeException(nameof(direction));
        }

        return _lastDirection.HasValue && direction != _lastDirection.Value
            ? Math.Max(1, NextStep / 2)
            : NextStep;
    }

    /// <summary>Call only after actual search movement and its preview have succeeded.</summary>
    public void CompleteJump(int direction)
    {
        NextStep = GetStep(direction);
        _lastDirection = direction;
    }

    public void Reset()
    {
        NextStep = InitialStep;
        _lastDirection = null;
    }
}
