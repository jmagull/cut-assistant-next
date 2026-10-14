namespace CutAssistantNext.Core.Media;

public sealed class VideoKeyframeIndex
{
    // Only timestamp serialization noise is tolerated, never a whole neighbouring frame.
    public static readonly TimeSpan TimestampTolerance = TimeSpan.FromMilliseconds(0.5);
    private readonly TimeSpan[] _positions;
    private VideoKeyframeIndex(TimeSpan[] positions)
    {
        _positions = positions;
        Positions = Array.AsReadOnly(positions);
    }
    public IReadOnlyList<TimeSpan> Positions { get; }

    public static VideoKeyframeIndex Create(VideoFrameWindow window, int expectedStream,
        TimeSpan duration, decimal sourceStart)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (window.StreamIndex != expectedStream || duration <= TimeSpan.Zero)
            throw new ArgumentException("Die Keyframe-Analyse gehört nicht zur geladenen Videospur.");
        var result = new List<TimeSpan>();
        decimal? previous = null;
        foreach (var frame in window.Frames)
        {
            if (frame.IsKeyFrame != true) continue;
            long? timestamp = frame.Pts;
            if (!timestamp.HasValue)
                throw new InvalidOperationException("Keyframe-Zeitstempel fehlen. Eine sichere Navigation ist nicht möglich.");
            var raw = window.TimeBase.ToSeconds(timestamp.Value);
            if (previous.HasValue && raw <= previous.Value)
                throw new InvalidOperationException("Die Keyframe-Zeiten sind mehrdeutig oder nicht aufsteigend.");
            previous = raw;
            decimal ticks = decimal.Round((raw - sourceStart) * TimeSpan.TicksPerSecond);
            if (ticks < 0 || ticks >= duration.Ticks) continue;
            result.Add(TimeSpan.FromTicks(decimal.ToInt64(ticks)));
        }
        if (result.Count == 0) throw new InvalidOperationException("Keine zuverlässig zugeordneten Keyframes gefunden.");
        return new VideoKeyframeIndex(result.ToArray());
    }

    public TimeSpan? FindAdjacent(TimeSpan position, int direction)
    {
        if (direction is not (-1 or 1)) throw new ArgumentOutOfRangeException(nameof(direction));
        // Binary search stays cheap even for long recordings.
        long target = position.Ticks;
        int low = 0, high = _positions.Length;
        while (low < high)
        {
            int middle = low + (high - low) / 2;
            bool before = direction > 0
                ? (decimal)_positions[middle].Ticks <= (decimal)target + TimestampTolerance.Ticks
                : (decimal)_positions[middle].Ticks < (decimal)target - TimestampTolerance.Ticks;
            if (before) low = middle + 1;
            else high = middle;
        }
        int index = direction > 0 ? low : low - 1;
        return index >= 0 && index < _positions.Length ? _positions[index] : null;
    }

    public bool Contains(TimeSpan position)
    {
        int index = Array.BinarySearch(_positions, position);
        if (index >= 0) return true;
        index = ~index;
        return (index < _positions.Length && Math.Abs((decimal)_positions[index].Ticks - position.Ticks) <= TimestampTolerance.Ticks) ||
            (index > 0 && Math.Abs((decimal)_positions[index - 1].Ticks - position.Ticks) <= TimestampTolerance.Ticks);
    }
}
