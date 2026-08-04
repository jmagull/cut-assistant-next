using System.Globalization;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Media.Playback.Interop;

namespace CutAssistantNext.Media.Playback;

public sealed class MpvMediaPlayerService : IMediaPlayerService
{
    private const string TimePositionProperty = "time-pos";
    private const string DurationProperty = "duration";

    private const string EstimatedFrameCountProperty =
        "estimated-frame-count";

    private const string PauseProperty = "pause";
    private const string VolumeProperty = "volume";

    private const double MinimumVolume = 0;
    private const double MaximumVolume = 100;
    private const double DefaultVolume = 100;

    private readonly ILibMpvClient _client;
    private readonly IAppLogger _logger;
    private readonly object _syncRoot = new();

    private CancellationTokenSource? _eventLoopCancellation;
    private Task? _eventLoopTask;

    private MediaPlayerState _state = MediaPlayerState.Empty;
    private TimeSpan _position = TimeSpan.Zero;
    private TimeSpan? _duration;
    private long? _frameNumber;
    private long? _estimatedFrameCount;
    private double _volume = DefaultVolume;
    private string? _errorMessage;
    private string? _currentMediaFilePath;

    private bool _initialized;
    private bool _stopRequested;
    private bool _pauseRequested = true;
    private bool _suppressNextFrameStepUnpause;
    private bool _disposed;

    public MpvMediaPlayerService()
        : this(
            new HanumanLibMpvClient(),
            NullAppLogger.Instance)
    {
    }

    public MpvMediaPlayerService(IAppLogger logger)
        : this(new HanumanLibMpvClient(), logger)
    {
    }

    internal MpvMediaPlayerService(ILibMpvClient client)
        : this(client, NullAppLogger.Instance)
    {
    }

    internal MpvMediaPlayerService(
        ILibMpvClient client,
        IAppLogger logger)
    {
        _client = client
            ?? throw new ArgumentNullException(nameof(client));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public event EventHandler? StateChanged;

    public event EventHandler? PositionChanged;

    public event EventHandler? DurationChanged;

    public event EventHandler? FrameChanged;

    public event EventHandler? VolumeChanged;

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

    public long? FrameNumber
    {
        get
        {
            lock (_syncRoot)
            {
                return _frameNumber;
            }
        }
    }

    public long? EstimatedFrameCount
    {
        get
        {
            lock (_syncRoot)
            {
                return _estimatedFrameCount;
            }
        }
    }

    public double Volume
    {
        get
        {
            lock (_syncRoot)
            {
                return _volume;
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

        _logger.Information(
            "libmpv-Initialisierung wurde gestartet.");

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
                EstimatedFrameCountProperty,
                cancellationToken);

            await _client.ObservePropertyAsync(
                PauseProperty,
                cancellationToken);

            _eventLoopCancellation =
                new CancellationTokenSource();

            _eventLoopTask = ProcessEventsAsync(
                _eventLoopCancellation.Token);

            _logger.Information(
                "libmpv wurde erfolgreich initialisiert.");
        }
        catch (Exception exception)
        {
            _logger.Error(
                "libmpv konnte nicht initialisiert werden.",
                exception);

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
            _currentMediaFilePath = fullMediaFilePath;
            _stopRequested = false;
            _pauseRequested = true;
        }

        _logger.Information(
            $"Mediendatei wird in mpv geladen: " +
            $"{fullMediaFilePath}");

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
            _logger.Error(
                $"Mediendatei konnte nicht in mpv geladen werden: " +
                $"{fullMediaFilePath}",
                exception);

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

            _logger.Information(
                "Wiedergabe wurde gestartet.");
        }
        catch (Exception exception)
        {
            _logger.Error(
                "Wiedergabe konnte nicht gestartet werden.",
                exception);

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

            _logger.Information(
                "Wiedergabe wurde pausiert.");
        }
        catch (Exception exception)
        {
            _logger.Error(
                "Wiedergabe konnte nicht pausiert werden.",
                exception);

            SetError(exception.Message);
            throw;
        }
    }

    public Task StepForwardAsync(
        CancellationToken cancellationToken = default)
    {
        return StepFramesAsync(
            1,
            cancellationToken);
    }

    public async Task StepFramesAsync(
        int frameCount,
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        if (frameCount == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameCount),
                "Die Anzahl der Bildschritte darf nicht 0 sein.");
        }

        var isForwardStep =
            frameCount > 0;

        var frameStepMode =
            isForwardStep
                ? "mute"
                : "seek";

        if (isForwardStep)
        {
            lock (_syncRoot)
            {
                _suppressNextFrameStepUnpause = true;
            }
        }

        try
        {
            await _client.CommandAsync(
                [
                    "frame-step",
                    frameCount.ToString(
                        CultureInfo.InvariantCulture),
                    frameStepMode
                ],
                cancellationToken);
        }
        catch (Exception exception)
        {
            if (isForwardStep)
            {
                lock (_syncRoot)
                {
                    _suppressNextFrameStepUnpause = false;
                }
            }

            _logger.Error(
                $"Bildnavigation konnte nicht ausgeführt werden: " +
                $"{frameCount:+0;-0} Frames.",
                exception);

            SetError(exception.Message);
            throw;
        }
    }

    public Task StepBackwardAsync(
        CancellationToken cancellationToken = default)
    {
        return StepFramesAsync(
            -1,
            cancellationToken);
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

            _logger.Information(
                $"Wiedergabeposition wurde geändert: " +
                $"{position:hh\\:mm\\:ss\\.fff}.");
        }
        catch (Exception exception)
        {
            _logger.Error(
                $"Wiedergabeposition konnte nicht geändert werden: " +
                $"{position:hh\\:mm\\:ss\\.fff}.",
                exception);

            SetError(exception.Message);
            throw;
        }
    }

    public async Task SetVolumeAsync(
        double volume,
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        if (!double.IsFinite(volume))
        {
            throw new ArgumentOutOfRangeException(
                nameof(volume),
                "Die Lautstärke muss eine endliche Zahl sein.");
        }

        var normalizedVolume = Math.Clamp(
            volume,
            MinimumVolume,
            MaximumVolume);

        try
        {
            await _client.SetDoublePropertyAsync(
                VolumeProperty,
                normalizedVolume,
                cancellationToken);

            lock (_syncRoot)
            {
                _volume = normalizedVolume;
            }

            VolumeChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
        catch (Exception exception)
        {
            _logger.Error(
                $"Lautstärke konnte nicht geändert werden: " +
                $"{normalizedVolume:0.##}.",
                exception);

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

            _logger.Information(
                "Wiedergabe wurde gestoppt.");
        }
        catch (Exception exception)
        {
            _logger.Error(
                "Wiedergabe konnte nicht gestoppt werden.",
                exception);

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

        _logger.Information(
            "libmpv-Ressourcen werden freigegeben.");

        try
        {
            await _client.DisposeAsync().ConfigureAwait(false);

            _logger.Information(
                "libmpv-Ressourcen wurden erfolgreich freigegeben.");
        }
        catch (Exception exception)
        {
            _logger.Error(
                "libmpv-Ressourcen konnten nicht freigegeben werden.",
                exception);

            throw;
        }

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
                _logger.Error(
                    "Die libmpv-Ereignisverarbeitung wurde unerwartet beendet.",
                    exception);

                SetError(exception.Message);
            }
        }
    }

    private void ProcessEvent(LibMpvEvent mpvEvent)
    {
        switch (mpvEvent.Kind)
        {
            case LibMpvEventKind.FileLoaded:
                ProcessFileLoaded();
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
                ProcessErrorEvent(mpvEvent.ErrorMessage);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(mpvEvent),
                    mpvEvent.Kind,
                    "Unbekanntes libmpv-Ereignis.");
        }
    }

    private void ProcessErrorEvent(string? errorMessage)
    {
        var understandableMessage =
            string.IsNullOrWhiteSpace(errorMessage)
                ? "libmpv hat einen unbekannten Fehler gemeldet."
                : errorMessage;

        _logger.Error(
            $"libmpv hat einen Fehler gemeldet: " +
            $"{understandableMessage}");

        SetError(understandableMessage);
    }

    private void ProcessFileLoaded()
    {
        string? mediaFilePath;

        lock (_syncRoot)
        {
            mediaFilePath = _currentMediaFilePath;
        }

        _logger.Information(
            string.IsNullOrWhiteSpace(mediaFilePath)
                ? "Mediendatei wurde erfolgreich in mpv geladen."
                : $"Mediendatei wurde erfolgreich in mpv geladen: " +
                  $"{mediaFilePath}");

        ApplyRequestedPlaybackState();
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

            case EstimatedFrameCountProperty:
                if (TryParseFrameNumber(
                    value,
                    out var estimatedFrameCount))
                {
                    SetEstimatedFrameCount(
                        estimatedFrameCount);
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
        bool suppressPauseChange;

        lock (_syncRoot)
        {
            suppressPauseChange =
                !isPaused &&
                _suppressNextFrameStepUnpause;

            if (suppressPauseChange)
            {
                _suppressNextFrameStepUnpause = false;
            }

            if (!suppressPauseChange)
            {
                _pauseRequested = isPaused;
            }
        }

        if (suppressPauseChange)
        {
            return;
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
        bool positionChanged;
        bool frameChanged;

        lock (_syncRoot)
        {
            positionChanged = _position != position;

            if (positionChanged)
            {
                _position = position;
            }

            frameChanged =
                RecalculateFrameNumberLocked();
        }

        if (positionChanged)
        {
            PositionChanged?.Invoke(
                this,
                EventArgs.Empty);
        }

        if (frameChanged)
        {
            FrameChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }

    private void SetDuration(TimeSpan duration)
    {
        bool durationChanged;
        bool frameChanged;

        lock (_syncRoot)
        {
            durationChanged = _duration != duration;

            if (durationChanged)
            {
                _duration = duration;
            }

            frameChanged =
                RecalculateFrameNumberLocked();
        }

        if (durationChanged)
        {
            DurationChanged?.Invoke(
                this,
                EventArgs.Empty);
        }

        if (frameChanged)
        {
            FrameChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }

    private void SetEstimatedFrameCount(
        long estimatedFrameCount)
    {
        bool frameChanged;

        lock (_syncRoot)
        {
            var estimatedFrameCountChanged =
                _estimatedFrameCount !=
                estimatedFrameCount;

            if (estimatedFrameCountChanged)
            {
                _estimatedFrameCount =
                    estimatedFrameCount;
            }

            var frameNumberChanged =
                RecalculateFrameNumberLocked();

            frameChanged =
                estimatedFrameCountChanged ||
                frameNumberChanged;
        }

        if (frameChanged)
        {
            FrameChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }

    private bool RecalculateFrameNumberLocked()
    {
        var frameNumber =
            CalculateFrameNumber(
                _position,
                _duration,
                _estimatedFrameCount);

        if (_frameNumber == frameNumber)
        {
            return false;
        }

        _frameNumber = frameNumber;
        return true;
    }

    private static long? CalculateFrameNumber(
        TimeSpan position,
        TimeSpan? duration,
        long? estimatedFrameCount)
    {
        if (!duration.HasValue ||
            duration.Value <= TimeSpan.Zero ||
            !estimatedFrameCount.HasValue ||
            estimatedFrameCount.Value <= 0)
        {
            return null;
        }

        var progress = Math.Clamp(
            position.TotalSeconds /
                duration.Value.TotalSeconds,
            0,
            1);

        var frameNumber = (long)Math.Floor(
            progress *
            estimatedFrameCount.Value);

        return Math.Min(
            frameNumber,
            estimatedFrameCount.Value - 1);
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

    private static bool TryParseFrameNumber(
        string? value,
        out long result)
    {
        if (long.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out result) &&
            result >= 0)
        {
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
