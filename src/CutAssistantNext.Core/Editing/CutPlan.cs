namespace CutAssistantNext.Core.Editing;

public sealed class CutPlan
{
    private readonly List<RemoveSegment> _removeSegments = [];
    private readonly IReadOnlyList<RemoveSegment> _readOnlyRemoveSegments;

    public CutPlan(
        TimeSpan mediaDuration)
    {
        if (mediaDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mediaDuration),
                "Die Laufzeit des Videos muss größer als null sein.");
        }

        MediaDuration = mediaDuration;
        _readOnlyRemoveSegments = _removeSegments.AsReadOnly();
    }

    public TimeSpan MediaDuration { get; }

    public IReadOnlyList<RemoveSegment> RemoveSegments =>
        _readOnlyRemoveSegments;

    public void Add(
        RemoveSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);

        ValidateSegment(segment);

        _removeSegments.Add(segment);
        SortSegments();
    }

    public bool Remove(
        RemoveSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);

        return _removeSegments.Remove(segment);
    }

    public void Replace(
        RemoveSegment existingSegment,
        RemoveSegment replacementSegment)
    {
        ArgumentNullException.ThrowIfNull(existingSegment);
        ArgumentNullException.ThrowIfNull(replacementSegment);

        var existingIndex =
            _removeSegments.IndexOf(existingSegment);

        if (existingIndex < 0)
        {
            throw new InvalidOperationException(
                "Der zu ersetzende Bereich ist nicht im Schnittplan enthalten.");
        }

        ValidateSegment(
            replacementSegment,
            existingSegment);

        _removeSegments[existingIndex] =
            replacementSegment;

        SortSegments();
    }

    private void ValidateSegment(
        RemoveSegment segment,
        RemoveSegment? ignoredSegment = null)
    {
        if (segment.End > MediaDuration)
        {
            throw new ArgumentOutOfRangeException(
                nameof(segment),
                "Der zu entfernende Bereich darf nicht über das Videoende hinausreichen.");
        }

        if (_removeSegments.Any(
                existingSegment =>
                    !ReferenceEquals(
                        existingSegment,
                        ignoredSegment) &&
                    existingSegment.Start < segment.End &&
                    segment.Start < existingSegment.End))
        {
            throw new InvalidOperationException(
                "Der zu entfernende Bereich überschneidet sich mit einem vorhandenen Bereich.");
        }
    }

    private void SortSegments()
    {
        _removeSegments.Sort(
            static (first, second) =>
                first.Start.CompareTo(second.Start));
    }
}