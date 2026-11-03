using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.ViewModels;

public enum CutEditingMode { Remove, Keep }

public sealed class CutPlanViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<RemoveSegment> _removeSegments = [];

    private CutPlan? _markedPlan;
    private RemoveSegment? _selectedRemoveSegment;
    private TimeSpan? _pendingStart;

    public CutPlanViewModel()
    {
        RemoveSegments =
            new ReadOnlyObservableCollection<RemoveSegment>(
                _removeSegments);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CutEditingMode EditingMode { get; private set; }
    public bool IsKeepMode => EditingMode == CutEditingMode.Keep;
    public ReadOnlyObservableCollection<RemoveSegment> MarkedSegments => RemoveSegments;
    public string EditingDescription => IsKeepMode
        ? "Das soll bleiben: Grün markierte Bereiche werden behalten. Nur diese Bereiche werden ausgegeben; unmarkierte Teile werden weggelassen."
        : "Das kann weg: Rot markierte Bereiche werden entfernt.";
    public string MarkingHelp => EditingDescription + " Schnittanfang und Schnittende setzen; Esc verwirft einen offenen Anfang.";
    public string StartButtonText => IsKeepMode ? "Behalteanfang setzen" : "Schnittanfang setzen";
    public string EndButtonText => IsKeepMode ? "Behalteende setzen" : "Schnittende setzen";
    public string KeepResultConfirmation =>
        $"Nur die {MarkedSegments.Count} grün markierten Behaltebereiche werden verwendet. " +
        $"Die geplante Ausgabe dauert {EstimatedOutputDurationText}.\n\n" +
        "Alle unmarkierten Filmteile werden weggelassen, auch wenn sie noch nicht bearbeitet wurden. Diese Auswahl verwenden?";

    public TimeSpan? MediaDuration =>
        _markedPlan?.MediaDuration;

    public TimeSpan? EstimatedOutputDuration
    {
        get
        {
            if (_markedPlan is null)
            {
                return null;
            }

            var markedDuration =
                TimeSpan.FromTicks(
                    _markedPlan.RemoveSegments.Sum(
                        segment => segment.Duration.Ticks));

            return IsKeepMode ? markedDuration : _markedPlan.MediaDuration - markedDuration;
        }
    }

    public string EstimatedOutputDurationText =>
        EstimatedOutputDuration.HasValue
            ? FormatTime(
                EstimatedOutputDuration.Value)
            : "–";

    public bool CanSetStart =>
        _markedPlan is not null;

    public bool CanSetEnd =>
        _markedPlan is not null &&
        (
            PendingStart.HasValue ||
            SelectedRemoveSegment is not null
        );

    public bool CanModifySelectedSegment =>
        SelectedRemoveSegment is not null;

    public CutEdgeSide? SelectedEdgeSide { get; private set; }

    public TimeSpan? SelectedEdgePosition =>
        SelectedEdgeSide switch
        {
            CutEdgeSide.Start => SelectedRemoveSegment?.Start,
            CutEdgeSide.End => SelectedRemoveSegment?.End,
            _ => null
        };

    public bool CanInspectSelectedEdge =>
        SelectedRemoveSegment is not null && SelectedEdgeSide.HasValue;

    public RemoveSegment? SelectedRemoveSegment
    {
        get => _selectedRemoveSegment;
        set
        {
            if (ReferenceEquals(
                    _selectedRemoveSegment,
                    value))
            {
                return;
            }

            _selectedRemoveSegment = value;
            SelectedEdgeSide = null;

            if (value is not null)
            {
                PendingStart = null;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(CanModifySelectedSegment));
            OnPropertyChanged(nameof(CanSetStart));
            OnPropertyChanged(nameof(CanSetEnd));
            PublishEdgeSelectionChanges();
        }
    }

    public TimeSpan? GetEdgeReviewPosition(RemoveSegment segment, CutEdgeSide side)
    {
        if (!IsKeepMode) return CutEdgeReviewPositionCalculator.Calculate(GetInitializedMarkedPlan(), segment, side);
        ArgumentNullException.ThrowIfNull(segment);
        if (!Enum.IsDefined(side)) throw new ArgumentOutOfRangeException(nameof(side));
        if (!GetInitializedMarkedPlan().RemoveSegments.Contains(segment))
            throw new InvalidOperationException("Die Schnittkante gehört nicht zum aktuellen Schnittplan.");
        var offset = Math.Min(3 * TimeSpan.TicksPerSecond, segment.Duration.Ticks - 1);
        return TimeSpan.FromTicks(side == CutEdgeSide.Start ? segment.Start.Ticks + offset : segment.End.Ticks - Math.Min(3 * TimeSpan.TicksPerSecond, segment.Duration.Ticks));
    }

    public void ToggleSegmentSelection(RemoveSegment segment, CutEdgeSide? side = null)
    {
        ArgumentNullException.ThrowIfNull(segment);
        if (side.HasValue && !Enum.IsDefined(side.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(side));
        }

        if (!GetInitializedMarkedPlan().RemoveSegments.Contains(segment))
        {
            throw new InvalidOperationException("Der ausgewählte Schnittbereich gehört nicht zum aktuellen Schnittplan.");
        }

        if (ReferenceEquals(SelectedRemoveSegment, segment))
        {
            SelectedRemoveSegment = null;
        }
        else if (side is { } edgeSide)
        {
            SelectEdge(segment, edgeSide);
        }
        else
        {
            SelectedRemoveSegment = segment;
        }
    }

    public void SelectEdge(RemoveSegment segment, CutEdgeSide side)
    {
        ArgumentNullException.ThrowIfNull(segment);
        if (!Enum.IsDefined(side))
        {
            throw new ArgumentOutOfRangeException(nameof(side));
        }

        if (!GetInitializedMarkedPlan().RemoveSegments.Contains(segment))
        {
            throw new InvalidOperationException(
                "Die ausgewählte Schnittkante gehört nicht zum aktuellen Schnittplan.");
        }

        SelectedRemoveSegment = segment;
        SelectedEdgeSide = side;
        PublishEdgeSelectionChanges();
    }

    private void PublishEdgeSelectionChanges()
    {
        OnPropertyChanged(nameof(SelectedEdgeSide));
        OnPropertyChanged(nameof(SelectedEdgePosition));
        OnPropertyChanged(nameof(CanInspectSelectedEdge));
    }

    public void ApplyFrameEdge(RemoveSegment expectedSegment, CutEdgeSide side, TimeSpan position)
    {
        ArgumentNullException.ThrowIfNull(expectedSegment);
        if (!Enum.IsDefined(side))
        {
            throw new ArgumentOutOfRangeException(nameof(side));
        }

        var cutPlan = GetInitializedMarkedPlan();
        if (!ReferenceEquals(SelectedRemoveSegment, expectedSegment) || SelectedEdgeSide != side ||
            !cutPlan.RemoveSegments.Contains(expectedSegment))
        {
            throw new InvalidOperationException("Die Schnittkante hat sich inzwischen geändert. Bitte die Frame-Lupe erneut öffnen.");
        }

        ValidatePosition(position, cutPlan.MediaDuration);
        if (SelectedEdgePosition == position)
        {
            return;
        }

        // Both edges are boundaries before the chosen frame, without an implicit frame offset.
        Replace(expectedSegment,
            side == CutEdgeSide.Start ? position : expectedSegment.Start,
            side == CutEdgeSide.End ? position : expectedSegment.End);
        SelectEdge(SelectedRemoveSegment!, side);
    }

    public TimeSpan? PendingStart
    {
        get => _pendingStart;
        private set
        {
            if (_pendingStart == value)
            {
                return;
            }

            _pendingStart = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(PendingStartText));
            OnPropertyChanged(nameof(CanSetEnd));
        }
    }

    public string PendingStartText =>
        PendingStart.HasValue
            ? FormatTime(PendingStart.Value)
            : "–";

    public void CancelPendingStart() => PendingStart = null;

    // Legacy binding/API name. These are marked editor intervals; use CreateCutPlanSnapshot for actual removals.
    public ReadOnlyObservableCollection<RemoveSegment> RemoveSegments { get; }

    public void Initialize(
        TimeSpan mediaDuration, CutEditingMode editingMode = CutEditingMode.Remove)
    {
        if (!Enum.IsDefined(editingMode)) throw new ArgumentOutOfRangeException(nameof(editingMode));
        var markedPlan = new CutPlan(mediaDuration);
        EditingMode = editingMode;
        // This private plan validates marked intervals; only CreateCutPlanSnapshot exports a removal plan.
        _markedPlan = markedPlan;
        PendingStart = null;
        SelectedRemoveSegment = null;
        _removeSegments.Clear();

        OnPropertyChanged(nameof(MediaDuration));
        OnPropertyChanged(
            nameof(EstimatedOutputDuration));

        OnPropertyChanged(
            nameof(EstimatedOutputDurationText));
        OnPropertyChanged(nameof(CanSetStart));
        OnPropertyChanged(nameof(CanSetEnd));
        OnPropertyChanged(nameof(EditingMode));
        OnPropertyChanged(nameof(IsKeepMode));
        OnPropertyChanged(nameof(EditingDescription));
        OnPropertyChanged(nameof(MarkingHelp));
        OnPropertyChanged(nameof(StartButtonText));
        OnPropertyChanged(nameof(EndButtonText));
    }

    public void LoadCutPlan(
        CutPlan cutPlan)
    {
        ArgumentNullException.ThrowIfNull(
            cutPlan);

        var importedPlan =
            new CutPlan(
                cutPlan.MediaDuration);

        var markedSegments = IsKeepMode
            ? CutlistKeepSegmentBuilder.Build(cutPlan).Select(segment => new RemoveSegment(segment.Start, segment.End))
            : cutPlan.RemoveSegments;
        foreach (var segment in markedSegments)
        {
            importedPlan.Add(
                new RemoveSegment(
                    segment.Start,
                    segment.End));
        }

        _markedPlan =
            importedPlan;

        PendingStart = null;
        SelectedRemoveSegment = null;

        SynchronizeSegments();

        OnPropertyChanged(
            nameof(MediaDuration));

        OnPropertyChanged(
            nameof(CanSetStart));

        OnPropertyChanged(
            nameof(CanSetEnd));
    }
    public void Reset()
    {
        _markedPlan = null;
        PendingStart = null;
        SelectedRemoveSegment = null;
        _removeSegments.Clear();

        OnPropertyChanged(nameof(MediaDuration));
        OnPropertyChanged(
            nameof(EstimatedOutputDuration));

        OnPropertyChanged(
            nameof(EstimatedOutputDurationText));
        OnPropertyChanged(nameof(CanSetStart));
        OnPropertyChanged(nameof(CanSetEnd));
    }

    public void SetStart(
        TimeSpan position)
    {
        var cutPlan = GetInitializedMarkedPlan();

        ValidatePosition(
            position,
            cutPlan.MediaDuration);

        if (SelectedRemoveSegment is not null)
        {
            var selectedSegment =
                SelectedRemoveSegment;

            Replace(
                selectedSegment,
                position,
                selectedSegment.End);

            return;
        }

        PendingStart = position;
    }

    public void SetEnd(
        TimeSpan position)
    {
        var cutPlan = GetInitializedMarkedPlan();

        ValidatePosition(
            position,
            cutPlan.MediaDuration);

        if (SelectedRemoveSegment is not null)
        {
            var selectedSegment =
                SelectedRemoveSegment;

            Replace(
                selectedSegment,
                selectedSegment.Start,
                position);

            SelectedRemoveSegment = null;
            return;
        }

        if (!PendingStart.HasValue)
        {
            throw new InvalidOperationException(
                "Es wurde noch kein Schnittanfang gesetzt.");
        }

        var segment = new RemoveSegment(
            PendingStart.Value,
            position);

        cutPlan.Add(segment);

        SynchronizeSegments();
        PendingStart = null;
    }

    public bool Remove(
        RemoveSegment segment)
    {
        var cutPlan = GetInitializedMarkedPlan();

        var removed =
            cutPlan.Remove(segment);

        if (removed)
        {
            SynchronizeSegments();

            if (ReferenceEquals(
                    SelectedRemoveSegment,
                    segment))
            {
                SelectedRemoveSegment = null;
            }
        }

        return removed;
    }

    public void Replace(
        RemoveSegment existingSegment,
        TimeSpan start,
        TimeSpan end)
    {
        var cutPlan = GetInitializedMarkedPlan();

        ValidatePosition(
            start,
            cutPlan.MediaDuration);

        ValidatePosition(
            end,
            cutPlan.MediaDuration);

        var replacementSegment =
            new RemoveSegment(
                start,
                end);

        cutPlan.Replace(
            existingSegment,
            replacementSegment);

        SynchronizeSegments();
        SelectedRemoveSegment = replacementSegment;
    }

    public CutPlan CreateCutPlanSnapshot()
    {
        var cutPlan =
            GetInitializedMarkedPlan();

        var snapshot =
            new CutPlan(cutPlan.MediaDuration);

        var removeSegments = IsKeepMode
            ? CutlistRemoveSegmentBuilder.Build(cutPlan.MediaDuration,
                cutPlan.RemoveSegments.Select(segment => new CutlistKeepSegment(segment.Start, segment.Duration)).ToArray())
            : cutPlan.RemoveSegments;
        foreach (var segment in removeSegments)
        {
            snapshot.Add(
                new RemoveSegment(
                    segment.Start,
                    segment.End));
        }

        return snapshot;
    }

    private CutPlan GetInitializedMarkedPlan()
    {
        return _markedPlan
            ?? throw new InvalidOperationException(
                "Es wurde noch keine Mediendatei für den Schnittplan initialisiert.");
    }

    private static void ValidatePosition(
        TimeSpan position,
        TimeSpan mediaDuration)
    {
        if (position < TimeSpan.Zero ||
            position > mediaDuration)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position),
                "Die Schnittposition muss innerhalb der Mediendatei liegen.");
        }
    }

    private void SynchronizeSegments()
    {
        _removeSegments.Clear();

        foreach (var segment in GetInitializedMarkedPlan().RemoveSegments)
        {
            _removeSegments.Add(segment);
        }

        OnPropertyChanged(
            nameof(EstimatedOutputDuration));

        OnPropertyChanged(
            nameof(EstimatedOutputDurationText));
    }

    private static string FormatTime(
        TimeSpan value)
    {
        var totalHours =
            Math.Max(0, (long)value.TotalHours);

        return $"{totalHours:00}:" +
               $"{value.Minutes:00}:" +
               $"{value.Seconds:00}." +
               $"{value.Milliseconds:000}";
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
