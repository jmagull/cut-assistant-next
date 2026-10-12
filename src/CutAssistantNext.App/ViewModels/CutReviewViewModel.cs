using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.ViewModels;

// Owns only the review player. All commands and disposal share one gate.
internal sealed class CutReviewViewModel : INotifyPropertyChanged, IAsyncDisposable
{
    private readonly IMediaPlayerService _player;
    private readonly Func<string, bool> _fileExists;
    private readonly TimeSpan _loadTimeout;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly CancellationToken _token;
    private CutCompletionViewModel? _completion;
    private int _index;
    private string? _loadedPath;
    private bool _initialized;
    private bool _busy;
    private Task? _closeTask;

    internal CutReviewViewModel(IMediaPlayerService player, Func<string, bool>? fileExists = null,
        TimeSpan? loadTimeout = null)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _fileExists = fileExists ?? File.Exists;
        _loadTimeout = loadTimeout ?? TimeSpan.FromSeconds(20);
        _token = _lifetime.Token;
        Playback = new PlaybackViewModel(player);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public PlaybackViewModel Playback { get; }
    internal CancellationToken LifetimeToken => _token;
    internal bool IsClosing => _token.IsCancellationRequested;
    public bool IsIdle => !_busy && !IsClosing;
    public bool CanPrevious => IsIdle && _completion is not null && _index > 0;
    public bool CanNext => IsIdle && _completion is not null && _index < _completion.CheckPoints.Count - 1;
    public string FileName => _completion?.FileName ?? "Geschnittener Film";
    public string CurrentTransitionText
    {
        get
        {
            if (_completion is null) return "Prüfstelle wird geladen …";
            var point = _completion.CheckPoints[_index];
            if (point.Kind == CutCompletionCheckPointKind.End && _loadedPath == _completion.OutputFilePath &&
                _player.Duration is { } duration)
                point = point with { Position = duration };
            return point.IsTransition
                ? $"{point.Label} von {_completion.Transitions.Count} · {point.PositionText}"
                : $"{point.Label} · {point.PositionText}";
        }
    }
    public string StatusText => _busy ? "Film wird für die Kontrolle vorbereitet …" :
        (_completion?.CheckPoints[_index].Kind switch
        {
            CutCompletionCheckPointKind.Beginning => "Wiedergabe direkt ab Filmanfang.",
            CutCompletionCheckPointKind.End => "Wiedergabe ab 3 Sekunden vor dem tatsächlichen Filmende.",
            _ => "Wiedergabe ab 3 Sekunden vor dem Übergang."
        }) + " Bitte Bild, Ton und Tonsynchronität prüfen.";

    internal Task ReviewAsync(CutCompletionViewModel completion, int index, nint handle)
    {
        ArgumentNullException.ThrowIfNull(completion);
        if (index < 0 || index >= completion.CheckPoints.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        return ExecuteAsync(async token =>
        {
            if (!_fileExists(completion.OutputFilePath))
                throw new FileNotFoundException("Die geschnittene Datei wurde nicht gefunden.", completion.OutputFilePath);
            if (!_initialized)
            {
                await _player.InitializeAsync(handle, token);
                _initialized = true;
            }

            Playback.CancelSeek();
            var completionChanged = !ReferenceEquals(_completion, completion);
            _completion = completion;
            _index = index;
            Notify();
            if (completionChanged || !string.Equals(_loadedPath, completion.OutputFilePath, StringComparison.OrdinalIgnoreCase) ||
                _player.State is MediaPlayerState.Error or MediaPlayerState.Empty or MediaPlayerState.Ended)
            {
                _loadedPath = null;
                await LoadReadyAsync(completion.OutputFilePath, token);
                _loadedPath = completion.OutputFilePath;
            }
            else
            {
                await _player.PauseAsync(token);
            }

            var duration = _player.Duration ?? throw new InvalidOperationException("Die Filmdauer ist nicht verfügbar.");
            // Stay before EOF even if calculated output times differ from the actual file.
            var latest = duration > TimeSpan.Zero ? duration - TimeSpan.FromTicks(1) : TimeSpan.Zero;
            var point = completion.CheckPoints[index];
            var requested = point.Kind switch
            {
                CutCompletionCheckPointKind.Beginning => TimeSpan.Zero,
                CutCompletionCheckPointKind.End => duration - TimeSpan.FromSeconds(3),
                _ => point.Position - TimeSpan.FromSeconds(3)
            };
            var position = requested < TimeSpan.Zero ? TimeSpan.Zero : requested > latest ? latest : requested;
            await _player.SeekAsync(position, token);
            token.ThrowIfCancellationRequested();
            await _player.PlayAsync(token);
        });
    }

    internal Task MoveAsync(int delta, nint handle)
    {
        if (_completion is null || delta is not (-1 or 1) ||
            _index + delta < 0 || _index + delta >= _completion.CheckPoints.Count)
            return Task.CompletedTask;
        return ReviewAsync(_completion, _index + delta, handle);
    }

    internal Task TogglePlaybackAsync() => ExecuteAsync(token => Playback.TogglePlaybackAsync(token));
    internal Task SeekAsync(TimeSpan position) => ExecuteAsync(async token =>
    {
        Playback.CancelSeek();
        if (_player.State == MediaPlayerState.Ended && _completion is not null)
        {
            if (!_fileExists(_completion.OutputFilePath))
                throw new FileNotFoundException("Die geschnittene Datei wurde nicht gefunden.", _completion.OutputFilePath);
            _loadedPath = null;
            await LoadReadyAsync(_completion.OutputFilePath, token);
            _loadedPath = _completion.OutputFilePath;
        }
        await Playback.SeekToAsync(position, token);
    });
    internal Task SetVolumeAsync(double volume) => ExecuteAsync(token => Playback.SetVolumeAsync(volume, token));

    private async Task ExecuteAsync(Func<CancellationToken, Task> action)
    {
        await _gate.WaitAsync(_token);
        try
        {
            _token.ThrowIfCancellationRequested();
            _busy = true;
            Notify();
            await action(_token);
        }
        finally
        {
            _busy = false;
            Notify();
            _gate.Release();
        }
    }

    private async Task LoadReadyAsync(string path, CancellationToken token)
    {
        var ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        void Changed(object? sender, EventArgs args)
        {
            if (_player.State == MediaPlayerState.Error)
                ready.TrySetResult(false);
            else if (_player.State is MediaPlayerState.Paused or MediaPlayerState.Playing &&
                     _player.Duration > TimeSpan.Zero)
                ready.TrySetResult(true);
        }
        _player.StateChanged += Changed;
        _player.DurationChanged += Changed;
        _player.ErrorOccurred += Changed;
        try
        {
            await _player.LoadAsync(path, token);
            Changed(null, EventArgs.Empty);
            try
            {
                if (!await ready.Task.WaitAsync(_loadTimeout, token))
                    throw new InvalidOperationException(_player.ErrorMessage ?? "Die Datei konnte nicht geladen werden.");
            }
            catch (TimeoutException exception)
            {
                throw new InvalidOperationException("Die geschnittene Datei wurde nicht rechtzeitig geladen. Bitte erneut prüfen.", exception);
            }
        }
        finally
        {
            _player.StateChanged -= Changed;
            _player.DurationChanged -= Changed;
            _player.ErrorOccurred -= Changed;
        }
    }

    public ValueTask DisposeAsync()
    {
        _closeTask ??= CloseCoreAsync();
        return new ValueTask(_closeTask);
    }

    private async Task CloseCoreAsync()
    {
        _lifetime.Cancel();
        Notify();
        await _gate.WaitAsync();
        try
        {
            Playback.CancelSeek();
            Playback.Dispose();
            await _player.DisposeAsync();
        }
        finally
        {
            _gate.Release();
            _lifetime.Dispose();
        }
    }

    private void Notify()
    {
        OnPropertyChanged(nameof(IsIdle));
        OnPropertyChanged(nameof(CanPrevious));
        OnPropertyChanged(nameof(CanNext));
        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(CurrentTransitionText));
        OnPropertyChanged(nameof(StatusText));
    }
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
