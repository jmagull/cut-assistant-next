using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.ViewModels;

public sealed class CutPlanViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<RemoveSegment> _removeSegments = [];

    private CutPlan? _cutPlan;
    private RemoveSegment? _selectedRemoveSegment;
    private TimeSpan? _pendingStart;

    public CutPlanViewModel()
    {
        RemoveSegments =
            new ReadOnlyObservableCollection<RemoveSegment>(
                _removeSegments);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public TimeSpan? MediaDuration =>
        _cutPlan?.MediaDuration;

    public bool CanSetStart =>
        _cutPlan is not null;

    public bool CanSetEnd =>
        _cutPlan is not null &&
        (
            PendingStart.HasValue ||
            SelectedRemoveSegment is not null
        );

    public bool CanModifySelectedSegment =>
        SelectedRemoveSegment is not null;

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

            if (value is not null)
            {
                PendingStart = null;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(CanModifySelectedSegment));
            OnPropertyChanged(nameof(CanSetStart));
            OnPropertyChanged(nameof(CanSetEnd));
        }
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

    public ReadOnlyObservableCollection<RemoveSegment> RemoveSegments { get; }

    public void Initialize(
        TimeSpan mediaDuration)
    {
        _cutPlan = new CutPlan(mediaDuration);
        PendingStart = null;
        SelectedRemoveSegment = null;
        _removeSegments.Clear();

        OnPropertyChanged(nameof(MediaDuration));
        OnPropertyChanged(nameof(CanSetStart));
        OnPropertyChanged(nameof(CanSetEnd));
    }

    public void LoadCutPlan(
        CutPlan cutPlan)
    {
        ArgumentNullException.ThrowIfNull(
            cutPlan);

        var importedPlan =
            new CutPlan(
                cutPlan.MediaDuration);

        foreach (var segment in cutPlan.RemoveSegments)
        {
            importedPlan.Add(
                new RemoveSegment(
                    segment.Start,
                    segment.End));
        }

        _cutPlan =
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
        _cutPlan = null;
        PendingStart = null;
        SelectedRemoveSegment = null;
        _removeSegments.Clear();

        OnPropertyChanged(nameof(MediaDuration));
        OnPropertyChanged(nameof(CanSetStart));
        OnPropertyChanged(nameof(CanSetEnd));
    }

    public void SetStart(
        TimeSpan position)
    {
        var cutPlan = GetInitializedCutPlan();

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
        var cutPlan = GetInitializedCutPlan();

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
        var cutPlan = GetInitializedCutPlan();

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
        var cutPlan = GetInitializedCutPlan();

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
            GetInitializedCutPlan();

        var snapshot =
            new CutPlan(cutPlan.MediaDuration);

        foreach (var segment in cutPlan.RemoveSegments)
        {
            snapshot.Add(
                new RemoveSegment(
                    segment.Start,
                    segment.End));
        }

        return snapshot;
    }

    private CutPlan GetInitializedCutPlan()
    {
        return _cutPlan
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

        foreach (var segment in GetInitializedCutPlan().RemoveSegments)
        {
            _removeSegments.Add(segment);
        }
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