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
    private double _seekPositionSeconds;
    private bool _disposed;

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

    public string FrameText =>
        $"Frame {FormatFrameNumber(FrameNumber)} / " +
        $"ca. {FormatFrameNumber(EstimatedFrameCount)}";

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

    public void CancelSeek()
    {
        EndSeek();
    }

    public Task PlayAsync(
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.PlayAsync(
            cancellationToken);
    }

    public Task PauseAsync(
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.PauseAsync(
            cancellationToken);
    }

    public Task TogglePlaybackAsync(
        CancellationToken cancellationToken = default)
    {
        return State switch
        {
            MediaPlayerState.Paused =>
                PlayAsync(cancellationToken),

            MediaPlayerState.Playing =>
                PauseAsync(cancellationToken),

            _ => Task.CompletedTask
        };
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
        return _mediaPlayerService.StepForwardAsync(
            cancellationToken);
    }

    public Task StepBackwardAsync(
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.StepBackwardAsync(
            cancellationToken);
    }

    public Task StepBackwardTenFramesAsync(
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.StepFramesAsync(
            -10,
            cancellationToken);
    }

    public Task StepForwardTenFramesAsync(
        CancellationToken cancellationToken = default)
    {
        return _mediaPlayerService.StepFramesAsync(
            10,
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
