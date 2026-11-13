using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.ViewModels;

public sealed class PlaybackViewModel
    : INotifyPropertyChanged, IDisposable
{
    private readonly IMediaPlayerService _mediaPlayerService;
    private readonly SynchronizationContext? _synchronizationContext;

    private bool _isSeeking;
    private bool _isTogglingPlayback;
    private double _seekPositionSeconds;
    private bool _disposed;
    private string _jumpSecondsText = "10";
    private decimal _lastValidJumpSeconds = 10;

    public IReadOnlyList<string> JumpPresets { get; } = ["1", "5", "10", "30", "60"];

    public string JumpSecondsText
    {
        get => _jumpSecondsText;
        set
        {
            if (_jumpSecondsText == value) return;
            _jumpSecondsText = value;
            if (TryParseJumpSeconds(value, out var seconds)) _lastValidJumpSeconds = seconds;
            OnPropertyChanged(nameof(JumpSecondsText));
        }
    }

    internal string JumpSecondsSetting => _lastValidJumpSeconds.ToString(CultureInfo.InvariantCulture);

    internal void RestoreJumpSeconds(string? value)
    {
        JumpSecondsText = (TryParseJumpSeconds(value, out var seconds) ? seconds : 10)
            .ToString(CultureInfo.InvariantCulture);
    }

    private static bool TryParseJumpSeconds(string? text, out decimal seconds) =>
        decimal.TryParse(text?.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out seconds) && seconds >= 0.0000001m &&
        seconds <= TimeSpan.MaxValue.Ticks / (decimal)TimeSpan.TicksPerSecond;

    public Task JumpAsync(int direction, CancellationToken cancellationToken = default)
    {
        if (direction is not (-1 or 1)) throw new ArgumentOutOfRangeException(nameof(direction));
        if (!TryParseJumpSeconds(JumpSecondsText, out var seconds))
            throw new ArgumentException("Sprungweite bitte als positive Sekundenanzahl eingeben.");
        if (!CanSeek || IsSeeking) return Task.CompletedTask;
        var offset = decimal.Round(seconds * TimeSpan.TicksPerSecond, 0, MidpointRounding.AwayFromZero);
        var target = (long)Math.Clamp(Position.Ticks + direction * offset, 0, Duration!.Value.Ticks);
        return SeekToAsync(TimeSpan.FromTicks(target), cancellationToken);
    }

    public PlaybackViewModel(
        IMediaPlayerService mediaPlayerService)
    {
        _mediaPlayerService = mediaPlayerService
            ?? throw new ArgumentNullException(
                nameof(mediaPlayerService));

        _synchronizationContext =
            SynchronizationContext.Current;

        _mediaPlayerService.StateChanged +=
            MediaPlayerService_StateChanged;

        _mediaPlayerService.PositionChanged +=
            MediaPlayerService_PositionChanged;

        _mediaPlayerService.DurationChanged +=
            MediaPlayerService_DurationChanged;

        _mediaPlayerService.FrameChanged +=
            MediaPlayerService_FrameChanged;

        _mediaPlayerService.VolumeChanged +=
            MediaPlayerService_VolumeChanged;

        _mediaPlayerService.ErrorOccurred +=
            MediaPlayerService_ErrorOccurred;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal event EventHandler? NavigationRequested;

    public MediaPlayerState State =>
        _mediaPlayerService.State;

    public TimeSpan Position =>
        _mediaPlayerService.Position;

    public TimeSpan? Duration =>
        _mediaPlayerService.Duration;

    public double PositionSeconds =>
        Math.Max(0, Position.TotalSeconds);

    public double DurationSeconds =>
        Math.Max(0, Duration?.TotalSeconds ?? 0);

    public string PositionText =>
        FormatTime(Position);

    public string DurationText =>
        Duration.HasValue
            ? FormatTime(Duration.Value)
            : "--:--:--";

    public long? FrameNumber =>
        _mediaPlayerService.FrameNumber;

    public long? EstimatedFrameCount =>
        _mediaPlayerService.EstimatedFrameCount;

    public string CurrentFrameText =>
        $"Frame {FormatFrameNumber(FrameNumber)}";

    public string TotalFrameText =>
        $" / ca. {FormatFrameNumber(EstimatedFrameCount)}";

    public string FrameText =>
        CurrentFrameText + TotalFrameText;

    public double Volume =>
        _mediaPlayerService.Volume;

    public string VolumeText =>
        $"{Volume:0} %";

    public bool CanSetVolume =>
        State is
            MediaPlayerState.Loading or
            MediaPlayerState.Paused or
            MediaPlayerState.Playing or
            MediaPlayerState.Ended;

    public string? ErrorMessage =>
        _mediaPlayerService.ErrorMessage;

    public bool HasError =>
        !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool CanPlay =>
        State == MediaPlayerState.Paused;

    public bool CanPause =>
        State == MediaPlayerState.Playing;

    public bool CanTogglePlayback =>
        State is
            MediaPlayerState.Paused or
            MediaPlayerState.Playing;


    public bool CanStepFrame =>
        State == MediaPlayerState.Paused;

    public bool CanSeek =>
        DurationSeconds > 0 &&
        State is
            MediaPlayerState.Paused or
            MediaPlayerState.Playing or
            MediaPlayerState.Ended;

    public bool IsSeeking =>
        _isSeeking;

    public double TimelinePositionSeconds =>
        IsSeeking
            ? _seekPositionSeconds
            : PositionSeconds;

    public void BeginSeek()
    {
        if (!CanSeek || IsSeeking)
        {
            return;
        }

        NavigationRequested?.Invoke(this, EventArgs.Empty);
        _seekPositionSeconds = PositionSeconds;
        _isSeeking = true;

        OnPropertyChanged(nameof(IsSeeking));
        OnPropertyChanged(nameof(TimelinePositionSeconds));
    }

    public void UpdateSeekPosition(
        double positionSeconds)
    {
        if (!IsSeeking)
        {
            return;
        }

        var normalizedPosition =
            NormalizeSeekPosition(positionSeconds);

        if (_seekPositionSeconds == normalizedPosition)
        {
            return;
        }

        _seekPositionSeconds = normalizedPosition;

        OnPropertyChanged(nameof(TimelinePositionSeconds));
    }

    public async Task CommitSeekAsync(
        CancellationToken cancellationToken = default)
    {
        if (!IsSeeking)
        {
            return;
        }

        var position =
            TimeSpan.FromSeconds(_seekPositionSeconds);

        try
        {
            await _mediaPlayerService.SeekAsync(
                position,
                cancellationToken);
        }
        finally
        {
            EndSeek();
        }
    }

    public Task SeekToAsync(
        TimeSpan position,
        CancellationToken cancellationToken = default)
    {
        if (!CanSeek || IsSeeking)
        {
            return Task.CompletedTask;
        }

        BeginSeek();
        UpdateSeekPosition(position.TotalSeconds);
        return CommitSeekAsync(cancellationToken);
    }

    public void CancelSeek()
    {
        EndSeek();
    }

    public Task PlayAsync(
        CancellationToken cancellationToken = default)
    {
        NavigationRequested?.Invoke(this, EventArgs.Empty);
        return _mediaPlayerService.PlayAsync(
            cancellationToken);
    }

    public Task PauseAsync(
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.PauseAsync(
            cancellationToken);
    }

    public async Task TogglePlaybackAsync(CancellationToken cancellationToken = default)
    {
        if (_isTogglingPlayback) return;

        _isTogglingPlayback = true;
        try
        {
            if (CanPlay) await PlayAsync(cancellationToken);
            else if (CanPause) await PauseAsync(cancellationToken);
        }
        finally
        {
            _isTogglingPlayback = false;
        }
    }

    public Task SetVolumeAsync(
        double volume,
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.SetVolumeAsync(
            volume,
            cancellationToken);
    }

    public Task StepForwardAsync(
        CancellationToken cancellationToken = default)
    {
        NavigationRequested?.Invoke(this, EventArgs.Empty);
        return _mediaPlayerService.StepForwardAsync(
            cancellationToken);
    }

    public Task StepBackwardAsync(
        CancellationToken cancellationToken = default)
    {
        NavigationRequested?.Invoke(this, EventArgs.Empty);
        return _mediaPlayerService.StepBackwardAsync(
            cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _mediaPlayerService.StateChanged -=
            MediaPlayerService_StateChanged;

        _mediaPlayerService.PositionChanged -=
            MediaPlayerService_PositionChanged;

        _mediaPlayerService.DurationChanged -=
            MediaPlayerService_DurationChanged;

        _mediaPlayerService.FrameChanged -=
            MediaPlayerService_FrameChanged;

        _mediaPlayerService.VolumeChanged -=
            MediaPlayerService_VolumeChanged;

        _mediaPlayerService.ErrorOccurred -=
            MediaPlayerService_ErrorOccurred;
    }

    private void MediaPlayerService_StateChanged(
        object? sender,
        EventArgs e)
    {
        PublishPropertyChanges(
            nameof(State),
            nameof(CanPlay),
            nameof(CanPause),
            nameof(CanTogglePlayback),
            nameof(CanStepFrame),
            nameof(CanSeek),
            nameof(CanSetVolume));
    }

    private void MediaPlayerService_PositionChanged(
        object? sender,
        EventArgs e)
    {
        if (IsSeeking)
        {
            PublishPropertyChanges(
                nameof(Position),
                nameof(PositionSeconds),
                nameof(PositionText),
                nameof(Duration),
                nameof(DurationSeconds),
                nameof(DurationText),
                nameof(CanSeek));

            return;
        }

        PublishPropertyChanges(
            nameof(Position),
            nameof(PositionSeconds),
            nameof(PositionText),
            nameof(Duration),
            nameof(DurationSeconds),
            nameof(DurationText),
            nameof(CanSeek),
            nameof(TimelinePositionSeconds));
    }

    private void MediaPlayerService_DurationChanged(
        object? sender,
        EventArgs e)
    {
        PublishPropertyChanges(
            nameof(Duration),
            nameof(DurationSeconds),
            nameof(DurationText),
            nameof(CanSeek),
            nameof(TimelinePositionSeconds));
    }

    private void MediaPlayerService_FrameChanged(
        object? sender,
        EventArgs e)
    {
        PublishPropertyChanges(
            nameof(FrameNumber),
            nameof(EstimatedFrameCount),
            nameof(CurrentFrameText),
            nameof(TotalFrameText),
            nameof(FrameText));
    }

    private void MediaPlayerService_VolumeChanged(
        object? sender,
        EventArgs e)
    {
        PublishPropertyChanges(
            nameof(Volume),
            nameof(VolumeText));
    }

    private void MediaPlayerService_ErrorOccurred(
        object? sender,
        EventArgs e)
    {
        PublishPropertyChanges(
            nameof(ErrorMessage),
            nameof(HasError));
    }

    private void PublishPropertyChanges(
        params string[] propertyNames)
    {
        void Publish()
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        if (_synchronizationContext is null ||
            SynchronizationContext.Current ==
                _synchronizationContext)
        {
            Publish();
            return;
        }

        _synchronizationContext.Post(
            _ => Publish(),
            null);
    }

    private void EndSeek()
    {
        if (!IsSeeking)
        {
            return;
        }

        _isSeeking = false;

        OnPropertyChanged(nameof(IsSeeking));
        OnPropertyChanged(nameof(TimelinePositionSeconds));
    }

    private double NormalizeSeekPosition(
        double positionSeconds)
    {
        if (!double.IsFinite(positionSeconds))
        {
            throw new ArgumentOutOfRangeException(
                nameof(positionSeconds),
                "Die gewünschte Position muss eine endliche Zahl sein.");
        }

        return Math.Clamp(
            positionSeconds,
            0,
            DurationSeconds);
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private static string FormatFrameNumber(long? value)
    {
        return value.HasValue
            ? value.Value.ToString(
                "N0",
                CultureInfo.GetCultureInfo("de-DE"))
            : "\u2013";
    }

    private static string FormatTime(TimeSpan value)
    {
        var totalHours =
            Math.Max(0, (long)value.TotalHours);

        return $"{totalHours:00}:" +
               $"{value.Minutes:00}:" +
               $"{value.Seconds:00}";
    }
}
