using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CutAssistantNext.Media.Playback;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Logging;

namespace CutAssistantNext.App.ViewModels;

// Owns only the review player. All commands and disposal share one gate.
internal sealed class CutReviewViewModel : INotifyPropertyChanged, IAsyncDisposable
{
    private readonly IMediaPlayerService _player;
    private readonly Func<string, bool> _fileExists;
    private readonly TimeSpan _loadTimeout;
    private readonly IAppLogger _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly CancellationToken _token;
    private CutCompletionViewModel? _completion;
    private int _index;
    private string? _loadedPath;
    private IReadOnlyList<PlayerAudioTrack> _audioTracks = [];
    private PlayerAudioTrack? _selectedAudioTrack;
    public IReadOnlyList<PlayerAudioTrack> AudioTracks => _audioTracks;
    public PlayerAudioTrack? SelectedAudioTrack => _selectedAudioTrack;
    public bool HasMultipleAudioTracks => _audioTracks.Count > 1;
    private bool _initialized;
    private bool _busy;
    private Task? _closeTask;

    internal CutReviewViewModel(IMediaPlayerService player, Func<string, bool>? fileExists = null,
        TimeSpan? loadTimeout = null, IAppLogger? logger = null)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _fileExists = fileExists ?? File.Exists;
        _loadTimeout = loadTimeout ?? TimeSpan.FromSeconds(20);
        _logger = logger ?? NullAppLogger.Instance;
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
            _ => _completion?.AreTransitionsMeasured == true
                ? "Wiedergabe ab 3 Sekunden vor dem geprüften Übergang."
                : _completion?.Result.TimingKind == CutReviewTimingKind.Located
                    ? "Wiedergabe ab 3 Sekunden vor der zugeordneten Prüfstelle; bitte den Übergang kontrollieren."
                    : "Wiedergabe ab 3 Sekunden vor der berechneten Prüfstelle; der tatsächliche Schnitt kann abweichen."
        }) + " Bitte Bild, Ton und Tonsynchronität prüfen." +
        (string.IsNullOrWhiteSpace(_completion?.ReviewNotice) ? string.Empty : " " +
            (_completion.ReviewNotice.StartsWith("QUICK AND COPY:", StringComparison.Ordinal)
                ? _completion.ReviewNotice.Split('\n')[0] : _completion.ReviewNotice));

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
            var needsReload = completionChanged || NeedsReload(completion);
            if (!needsReload && completion.Result.Timeline.Identity is { } identity)
                await CutOutputIdentityVerifier.ValidateAsync(completion.OutputFilePath, identity, false, token);
            var audioId = completionChanged ? null : _selectedAudioTrack?.Id;
            _completion = completion;
            _index = index;
            Notify();
            if (needsReload)
            {
                await ReloadAsync(completion, audioId, token);
            }
            else
            {
                await _player.PauseAsync(token);
                if (NeedsReload(completion)) await ReloadAsync(completion, audioId, token);
            }

            await _player.SeekAsync(ReviewPosition(completion, index), token);
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
        if (_completion is not null && NeedsReload(_completion))
        {
            await ReloadAsync(_completion, _selectedAudioTrack?.Id, token);
        }
        await Playback.SeekToAsync(position, token);
    });
    internal Task SelectAudioAsync(PlayerAudioTrack track) => ExecuteAsync(async token =>
    {
        ArgumentNullException.ThrowIfNull(track);
        try
        {
            if (_player is not IAudioTrackSelection selection || _completion is null)
                throw new ArgumentException("Diese Tonspur steht nicht zur Verfügung.", nameof(track));
            Playback.CancelSeek();
            var wasPlaying = _player.State == MediaPlayerState.Playing;
            var reloaded = NeedsReload(_completion);
            if (reloaded) await ReloadAsync(_completion, null, token);
            await _player.PauseAsync(token);
            await RefreshAudioAsync(token);
            // EOF can arrive while Pause or the inventory query is in flight.
            if (NeedsReload(_completion) || _audioTracks.Count == 0)
            {
                await ReloadAsync(_completion, null, token);
                reloaded = true;
            }
            if (!_audioTracks.Any(t => t.Id == track.Id))
                throw new ArgumentException("Diese Tonspur steht im geladenen Film nicht zur Verfügung.", nameof(track));
            await selection.SelectAudioTrackAsync(track.Id, token);
            await RefreshAudioAsync(token);
            if (_selectedAudioTrack?.Id != track.Id)
                throw new InvalidOperationException("Die gewählte Tonspur wurde vom Kontrollplayer nicht bestätigt.");
            // A regular track change is not checkpoint navigation. Only an EOF recovery
            // restarts the current checkpoint preview in the newly loaded media context.
            if (reloaded) await _player.SeekAsync(ReviewPosition(_completion, _index), token);
            token.ThrowIfCancellationRequested();
            if (reloaded || wasPlaying) await _player.PlayAsync(token);
        }
        catch (Exception exception)
        {
            // Even a successful set followed by a failed seek must not leave a reusable load marker.
            _loadedPath = null;
            _selectedAudioTrack = null;
            _audioTracks = [];
            if (!token.IsCancellationRequested)
            {
                try { await RefreshAudioAsync(token); }
                catch (Exception inventoryError)
                {
                    _logger.Warning($"Tonspurinventar nach Wechsel nicht lesbar: {inventoryError.Message}");
                }
                _logger.Error($"Kontrollplayer-Tonspurwechsel fehlgeschlagen: angefordert={track.Id}, " +
                    $"bestätigt={_selectedAudioTrack?.Id.ToString() ?? "unbekannt"}, " +
                    $"Spuren=[{string.Join(",", _audioTracks.Select(t => $"{t.Id}:{t.Selected}"))}], " +
                    $"Zustand={_player.State}, Prüfstelle={_index} ({CurrentTransitionText}).", exception);
            }
            if (exception is ArgumentException or OperationCanceledException) throw;
            throw new InvalidOperationException("Die Tonspur konnte nicht gewechselt werden. Bitte die Prüfstelle erneut aufrufen.", exception);
        }
    });

    private bool NeedsReload(CutCompletionViewModel completion) =>
        !string.Equals(_loadedPath, completion.OutputFilePath, StringComparison.OrdinalIgnoreCase) ||
        _player.State is MediaPlayerState.Error or MediaPlayerState.Empty or MediaPlayerState.Ended;

    private async Task ReloadAsync(CutCompletionViewModel completion, int? audioId, CancellationToken token)
    {
        _loadedPath = null;
        _audioTracks = [];
        _selectedAudioTrack = null;
        if (!_fileExists(completion.OutputFilePath))
            throw new FileNotFoundException("Die geschnittene Datei wurde nicht gefunden.", completion.OutputFilePath);
        if (completion.Result.Timeline.Identity is { } identity)
            await CutOutputIdentityVerifier.ValidateAsync(completion.OutputFilePath, identity, true, token);
        await LoadReadyAsync(completion.OutputFilePath, token);
        if (audioId is { } id && _selectedAudioTrack?.Id != id && _player is IAudioTrackSelection selection)
        {
            if (!_audioTracks.Any(t => t.Id == id))
                throw new InvalidOperationException("Die bisher gewählte Tonspur steht im geladenen Film nicht zur Verfügung.");
            await selection.SelectAudioTrackAsync(id, token);
            await RefreshAudioAsync(token);
            if (_selectedAudioTrack?.Id != id)
                throw new InvalidOperationException("Die bisher gewählte Tonspur wurde vom Kontrollplayer nicht bestätigt.");
        }
        _loadedPath = completion.OutputFilePath;
    }

    private async Task RefreshAudioAsync(CancellationToken token)
    {
        var tracks = _player is IAudioTrackSelection audio ? await audio.GetAudioTracksAsync(token) : [];
        _audioTracks = tracks;
        _selectedAudioTrack = tracks.SingleOrDefault(t => t.Selected);
    }

    private TimeSpan ReviewPosition(CutCompletionViewModel completion, int index)
    {
        var duration = _player.Duration ?? throw new InvalidOperationException("Die Filmdauer ist nicht verfügbar.");
        var latest = duration > TimeSpan.Zero ? duration - TimeSpan.FromTicks(1) : TimeSpan.Zero;
        var point = completion.CheckPoints[index];
        var requested = point.Kind switch
        {
            CutCompletionCheckPointKind.Beginning => TimeSpan.Zero,
            CutCompletionCheckPointKind.End => duration - TimeSpan.FromSeconds(3),
            _ => point.Position - TimeSpan.FromSeconds(3)
        };
        return requested < TimeSpan.Zero ? TimeSpan.Zero : requested > latest ? latest : requested;
    }

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
            await RefreshAudioAsync(token);
            Notify();
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
        OnPropertyChanged(nameof(AudioTracks));
        OnPropertyChanged(nameof(SelectedAudioTrack));
        OnPropertyChanged(nameof(HasMultipleAudioTracks));
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
