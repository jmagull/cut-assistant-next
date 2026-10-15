using System.ComponentModel;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.ViewModels;

/// <summary>Uses the loupe's verified frame navigation, anchored anew at the reached player position.</summary>
internal sealed class PlayerFrameSearchViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly PlaybackViewModel _playback;
    private HalvingFrameSearch _search = new();
    private Func<TimeSpan, FrameLoupeViewModel>? _createNavigator;
    private CancellationTokenSource? _operation;
    private bool _disposed;

    public PlayerFrameSearchViewModel(PlaybackViewModel playback)
    {
        _playback = playback ?? throw new ArgumentNullException(nameof(playback));
        _playback.PropertyChanged += PlaybackChanged;
        _playback.NavigationRequested += NavigationRequested;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public int NextSearchStep => _search.NextStep;
    public string SearchLeftText => $"← {_search.GetStep(-1):N0} Frames";
    public string SearchRightText => $"{_search.GetStep(1):N0} Frames →";
    public bool IsNavigating => _operation is not null;
    public bool CanInteract => !IsNavigating;
    public bool CanSearch => !_disposed && _createNavigator is not null && !IsNavigating &&
        _playback.CanSeek && !_playback.IsSeeking;
    public bool CanReset => !_disposed && !IsNavigating && _createNavigator is not null;
    public string Status { get; private set; } = "Bitte zuerst ein Video laden.";

    public void SetMedia(Func<TimeSpan, FrameLoupeViewModel>? createNavigator,
        int initialStep = HalvingFrameSearch.DefaultInitialStep)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var search = new HalvingFrameSearch(initialStep);
        CancelOperation();
        _search = search;
        _createNavigator = createNavigator;
        IsSearchError = false;
        Status = createNavigator is null ? "Halbierungssuche nicht verfügbar: eindeutige Videospur und Dauer erforderlich."
            : "Gleiche Richtung: gleiche Weite. Richtungswechsel: halbe Weite. Manuelle Navigation startet eine neue Suche.";
        Publish();
    }

    public void ResetSearch()
    {
        if (!CanReset) return;
        _search.Reset();
        IsSearchError = false;
        Status = "Suche zurückgesetzt. Die aktuelle Playerposition bleibt erhalten.";
        Publish();
    }

    public async Task SearchAsync(int direction)
    {
        if (direction is not (-1 or 1)) throw new ArgumentOutOfRangeException(nameof(direction));
        if (!CanSearch) return;
        var operation = _operation = new CancellationTokenSource();
        var token = operation.Token;
        var search = _search;
        IsSearchError = false;
        Status = "Suchsprung wird anhand der Videoframes ermittelt …";
        Publish();
        try
        {
            if (_playback.CanPause) await _playback.PauseAsync(token);
            token.ThrowIfCancellationRequested();
            var origin = _playback.Position;
            // The transient inspection model shares the tested PTS/AVI/VFR and boundary handling.
            // Its private search state and preview never affect an open loupe or the cut plan.
            var navigator = _createNavigator!(origin);
            await navigator.InitializeAsync(token);
            token.ThrowIfCancellationRequested();
            RequireConfirmedFrame(navigator);
            var anchor = navigator.SelectedCutPosition;
            await navigator.StepAsync(direction * search.GetStep(direction), token);
            token.ThrowIfCancellationRequested();
            var target = RequireConfirmedFrame(navigator);
            if (target == anchor)
            {
                Status = "Dateigrenze erreicht. Die Suchweite bleibt erhalten.";
                return;
            }
            await _playback.SeekToAsync(target, token);
            await WaitForPositionAsync(target, GetPositionTolerance(navigator), token);
            token.ThrowIfCancellationRequested();
            search.CompleteJump(direction);
            Status = "Suchsprung bestätigt. Gleiche Richtung: gleiche Weite. Richtungswechsel: halbe Weite.";
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception exception)
        {
            if (ReferenceEquals(_operation, operation))
            {
                IsSearchError = true;
                Status = "Halbierungssuche nicht bestätigt: " + exception.Message;
            }
        }
        finally
        {
            if (ReferenceEquals(_operation, operation))
            {
                _operation = null;
                Publish();
            }
            operation.Dispose();
        }
    }

    public bool IsSearchError { get; private set; }

    private static TimeSpan RequireConfirmedFrame(FrameLoupeViewModel navigator)
    {
        if (navigator.PreviewPngBytes is not { Length: > 0 } || navigator.SelectedCutPosition is not TimeSpan position)
            throw new InvalidOperationException(navigator.Status);
        if (navigator.SelectedFrame?.Frame.Pts is null)
            throw new InvalidOperationException("Für die Halbierungssuche im Hauptplayer fehlen eindeutige Original-Zeitstempel.");
        return position;
    }

    private async Task WaitForPositionAsync(TimeSpan target, TimeSpan tolerance, CancellationToken token)
    {
        bool Reached() => Math.Abs((decimal)_playback.Position.Ticks - target.Ticks) <= tolerance.Ticks;
        var reached = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void Changed(object? sender, PropertyChangedEventArgs args) { if (Reached()) reached.TrySetResult(); }
        _playback.PropertyChanged += Changed;
        try
        {
            if (!Reached()) await reached.Task.WaitAsync(TimeSpan.FromSeconds(5), token);
            // A seek request can briefly publish its target before the decoded frame clock settles.
            await Task.Delay(100, token);
            if (!Reached()) throw new InvalidOperationException("Die erreichte Playerposition passt nicht zum analysierten Zielframe.");
        }
        finally { _playback.PropertyChanged -= Changed; }
    }

    private void PlaybackChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(PlaybackViewModel.Position) or nameof(PlaybackViewModel.State) or
            nameof(PlaybackViewModel.IsSeeking) or nameof(PlaybackViewModel.CanSeek)) Publish();
    }

    private void NavigationRequested(object? sender, EventArgs args)
    {
        if (IsNavigating) return;
        _search.Reset();
        Publish();
    }

    private static TimeSpan GetPositionTolerance(FrameLoupeViewModel navigator)
    {
        // Player clock and frame PTS are distinct. Permit less than half the nearest measured
        // frame interval, never a guessed average frame rate or an adjacent frame's timestamp.
        var rows = navigator.Frames;
        int index = rows.ToList().IndexOf(navigator.SelectedFrame!);
        decimal timestamp = navigator.SelectedFrame!.RawSeconds!.Value;
        var distances = new List<decimal>();
        foreach (int adjacent in new[] { index - 1, index + 1 })
        {
            if (adjacent >= 0 && adjacent < rows.Count && rows[adjacent].RawSeconds is decimal time)
                distances.Add(Math.Abs(time - timestamp));
        }
        return distances.Count == 0 ? VideoKeyframeIndex.TimestampTolerance :
            TimeSpan.FromTicks(Math.Max(1, decimal.ToInt64(decimal.Floor(distances.Min() * TimeSpan.TicksPerSecond / 2)) - 1));
    }

    private void CancelOperation()
    {
        _operation?.Cancel();
        _operation = null;
    }

    private void Publish()
    {
        foreach (string property in new[] { nameof(NextSearchStep), nameof(SearchLeftText), nameof(SearchRightText),
            nameof(IsNavigating), nameof(CanInteract), nameof(CanSearch), nameof(CanReset), nameof(Status) })
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _playback.PropertyChanged -= PlaybackChanged;
        _playback.NavigationRequested -= NavigationRequested;
        CancelOperation();
        _createNavigator = null;
        Publish();
    }
}
