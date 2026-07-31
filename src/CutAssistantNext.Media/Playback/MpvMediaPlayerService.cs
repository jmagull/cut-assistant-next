using System.Globalization;
using CutAssistantNext.Media.Playback.Interop;

namespace CutAssistantNext.Media.Playback;

public sealed class MpvMediaPlayerService : IMediaPlayerService
{
    private const string TimePositionProperty = "time-pos";
    private const string DurationProperty = "duration";
    private const string PauseProperty = "pause";

    private readonly ILibMpvClient _client;
    private readonly object _syncRoot = new();

    private CancellationTokenSource? _eventLoopCancellation;
    private Task? _eventLoopTask;

    private MediaPlayerState _state = MediaPlayerState.Empty;
    private TimeSpan _position = TimeSpan.Zero;
    private TimeSpan? _duration;
    private string? _errorMessage;

    private bool _initialized;
    private bool _stopRequested;
    private bool _pauseRequested = true;
    private bool _disposed;

    public MpvMediaPlayerService()
        : this(new HanumanLibMpvClient())
    {
    }

    internal MpvMediaPlayerService(ILibMpvClient client)
    {
        _client = client
            ?? throw new ArgumentNullException(nameof(client));
    }

    public event EventHandler? StateChanged;

    public event EventHandler? PositionChanged;

    public event EventHandler? DurationChanged;

    public event EventHandler? ErrorOccurred;

    public MediaPlayerState State
    {
        get
        {
            lock (_syncRoot)
            {
                return _state;
            }
        }
    }

    public TimeSpan Position
    {
        get
        {
            lock (_syncRoot)
            {
                return _position;
            }
        }
    }

    public TimeSpan? Duration
    {
        get
        {
            lock (_syncRoot)
            {
                return _duration;
            }
        }
    }

    public string? ErrorMessage
    {
        get
        {
            lock (_syncRoot)
            {
                return _errorMessage;
            }
        }
    }

    public async Task InitializeAsync(
        nint videoWindowHandle,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (videoWindowHandle == 0)
        {
            throw new ArgumentException(
                "Das Video-Fensterhandle darf nicht 0 sein.",
                nameof(videoWindowHandle));
        }

        lock (_syncRoot)
        {
            if (_initialized)
            {
                throw new InvalidOperationException(
                    "Der MediaPlayer-Service wurde bereits initialisiert.");
            }

            _initialized = true;
        }

        try
        {
            await _client.InitializeAsync(
                videoWindowHandle,
                cancellationToken);

            await _client.ObservePropertyAsync(
                TimePositionProperty,
                cancellationToken);

            await _client.ObservePropertyAsync(
                DurationProperty,
                cancellationToken);

            await _client.ObservePropertyAsync(
                PauseProperty,
                cancellationToken);

            _eventLoopCancellation =
                new CancellationTokenSource();

            _eventLoopTask = ProcessEventsAsync(
                _eventLoopCancellation.Token);
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            throw;
        }
    }

    public async Task LoadAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);

        EnsureInitialized();

        var fullMediaFilePath =
            Path.GetFullPath(mediaFilePath);

        if (!File.Exists(fullMediaFilePath))
        {
            throw new FileNotFoundException(
                "Die wiederzugebende Mediendatei wurde nicht gefunden.",
                fullMediaFilePath);
        }

        lock (_syncRoot)
        {
            _position = TimeSpan.Zero;
            _duration = null;
            _errorMessage = null;
            _stopRequested = false;
            _pauseRequested = true;
        }

        SetState(MediaPlayerState.Loading);

        PositionChanged?.Invoke(this, EventArgs.Empty);
        DurationChanged?.Invoke(this, EventArgs.Empty);

        try
        {
            await _client.CommandAsync(
                [
                    "loadfile",
                    fullMediaFilePath,
                    "replace"
                ],
                cancellationToken);
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            throw;
        }
    }

    public async Task PlayAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        try
        {
            await _client.SetBooleanPropertyAsync(
                PauseProperty,
                false,
                cancellationToken);

            lock (_syncRoot)
            {
                _pauseRequested = false;
            }

            SetState(MediaPlayerState.Playing);
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            throw;
        }
    }

    public async Task PauseAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        try
        {
            await _client.SetBooleanPropertyAsync(
                PauseProperty,
                true,
                cancellationToken);

            lock (_syncRoot)
            {
                _pauseRequested = true;
            }

            SetState(MediaPlayerState.Paused);
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            throw;
        }
    }

    public async Task SeekAsync(
        TimeSpan position,
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        if (position < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position),
                "Die Wiedergabeposition darf nicht negativ sein.");
        }

        try
        {
            await _client.SetDoublePropertyAsync(
                TimePositionProperty,
                position.TotalSeconds,
                cancellationToken);
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            throw;
        }
    }

    public async Task StopAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        lock (_syncRoot)
        {
            _stopRequested = true;
            _pauseRequested = true;
            _position = TimeSpan.Zero;
            _duration = null;
        }

        try
        {
            await _client.CommandAsync(
                ["stop"],
                cancellationToken);

            SetState(MediaPlayerState.Empty);
            PositionChanged?.Invoke(this, EventArgs.Empty);
            DurationChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        var cancellation = _eventLoopCancellation;
        var eventLoopTask = _eventLoopTask;

        cancellation?.Cancel();

        if (eventLoopTask is not null)
        {
            try
            {
                await eventLoopTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Erwartetes Ende der Ereignisverarbeitung.
            }
        }

        await _client.DisposeAsync().ConfigureAwait(false);

        cancellation?.Dispose();

        _eventLoopCancellation = null;
        _eventLoopTask = null;
    }

    private async Task ProcessEventsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (
                var mpvEvent in _client.ReadEventsAsync(
                    cancellationToken))
            {
                ProcessEvent(mpvEvent);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Erwartetes Ende der Ereignisverarbeitung.
        }
        catch (Exception exception)
        {
            if (!_disposed)
            {
                SetError(exception.Message);
            }
        }
    }

    private void ProcessEvent(LibMpvEvent mpvEvent)
    {
        switch (mpvEvent.Kind)
        {
            case LibMpvEventKind.FileLoaded:
                ApplyRequestedPlaybackState();
                break;

            case LibMpvEventKind.PlaybackRestarted:
                ProcessPlaybackRestarted();
                break;

            case LibMpvEventKind.EndFile:
                ProcessEndFile();
                break;

            case LibMpvEventKind.PropertyChanged:
                ProcessPropertyChanged(mpvEvent);
                break;

            case LibMpvEventKind.Error:
                SetError(
                    string.IsNullOrWhiteSpace(mpvEvent.ErrorMessage)
                        ? "libmpv hat einen unbekannten Fehler gemeldet."
                        : mpvEvent.ErrorMessage);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(mpvEvent),
                    mpvEvent.Kind,
                    "Unbekanntes libmpv-Ereignis.");
        }
    }

    private void ProcessPlaybackRestarted()
    {
        ApplyRequestedPlaybackState();
    }

    private void ApplyRequestedPlaybackState()
    {
        bool pauseRequested;

        lock (_syncRoot)
        {
            pauseRequested = _pauseRequested;
        }

        SetState(
            pauseRequested
                ? MediaPlayerState.Paused
                : MediaPlayerState.Playing);
    }

    private void ProcessEndFile()
    {
        bool stopRequested;

        lock (_syncRoot)
        {
            stopRequested = _stopRequested;
            _stopRequested = false;
        }

        SetState(
            stopRequested
                ? MediaPlayerState.Empty
                : MediaPlayerState.Ended);
    }

    private void ProcessPropertyChanged(
        LibMpvEvent mpvEvent)
    {
        if (string.IsNullOrWhiteSpace(mpvEvent.PropertyName))
        {
            return;
        }

        var value = Convert.ToString(
            mpvEvent.Value,
            CultureInfo.InvariantCulture);

        switch (mpvEvent.PropertyName)
        {
            case TimePositionProperty:
                if (TryParseSeconds(value, out var position))
                {
                    SetPosition(position);
                }

                break;

            case DurationProperty:
                if (TryParseSeconds(value, out var duration))
                {
                    SetDuration(duration);
                }

                break;

            case PauseProperty:
                if (TryParseBoolean(value, out var isPaused))
                {
                    ProcessPauseChanged(isPaused);
                }

                break;
        }
    }

    private void ProcessPauseChanged(bool isPaused)
    {
        lock (_syncRoot)
        {
            _pauseRequested = isPaused;
        }

        var currentState = State;

        if (currentState is
            MediaPlayerState.Empty or
            MediaPlayerState.Loading or
            MediaPlayerState.Ended or
            MediaPlayerState.Error)
        {
            return;
        }

        SetState(
            isPaused
                ? MediaPlayerState.Paused
                : MediaPlayerState.Playing);
    }

    private void SetState(MediaPlayerState state)
    {
        lock (_syncRoot)
        {
            if (_state == state)
            {
                return;
            }

            _state = state;
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetPosition(TimeSpan position)
    {
        lock (_syncRoot)
        {
            if (_position == position)
            {
                return;
            }

            _position = position;
        }

        PositionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetDuration(TimeSpan duration)
    {
        lock (_syncRoot)
        {
            if (_duration == duration)
            {
                return;
            }

            _duration = duration;
        }

        DurationChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetError(string errorMessage)
    {
        lock (_syncRoot)
        {
            _errorMessage = errorMessage;
        }

        SetState(MediaPlayerState.Error);
        ErrorOccurred?.Invoke(this, EventArgs.Empty);
    }

    private void EnsureInitialized()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException(
                    "Der MediaPlayer-Service wurde noch nicht initialisiert.");
            }
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    private static bool TryParseSeconds(
        string? value,
        out TimeSpan result)
    {
        if (double.TryParse(
            value,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var seconds) &&
            double.IsFinite(seconds) &&
            seconds >= 0)
        {
            result = TimeSpan.FromSeconds(seconds);
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryParseBoolean(
        string? value,
        out bool result)
    {
        if (bool.TryParse(value, out result))
        {
            return true;
        }

        switch (value?.Trim().ToLowerInvariant())
        {
            case "yes":
            case "1":
                result = true;
                return true;

            case "no":
            case "0":
                result = false;
                return true;

            default:
                result = false;
                return false;
        }
    }
}
