using CutAssistantNext.Media.Playback;

namespace TransitionReview;

// Diagnostic decorator: mute before the first play command and record actual VM seek requests.
internal sealed class RecordingPlayer(IMediaPlayerService inner) : IMediaPlayerService
{
    public List<TimeSpan> RequestedSeeks { get; } = [];
    public MediaPlayerState State => inner.State;
    public TimeSpan Position => inner.Position;
    public TimeSpan? Duration => inner.Duration;
    public long? FrameNumber => inner.FrameNumber;
    public long? EstimatedFrameCount => inner.EstimatedFrameCount;
    public double Volume => inner.Volume;
    public string? ErrorMessage => inner.ErrorMessage;
    public event EventHandler? StateChanged { add => inner.StateChanged += value; remove => inner.StateChanged -= value; }
    public event EventHandler? PositionChanged { add => inner.PositionChanged += value; remove => inner.PositionChanged -= value; }
    public event EventHandler? DurationChanged { add => inner.DurationChanged += value; remove => inner.DurationChanged -= value; }
    public event EventHandler? FrameChanged { add => inner.FrameChanged += value; remove => inner.FrameChanged -= value; }
    public event EventHandler? VolumeChanged { add => inner.VolumeChanged += value; remove => inner.VolumeChanged -= value; }
    public event EventHandler? ErrorOccurred { add => inner.ErrorOccurred += value; remove => inner.ErrorOccurred -= value; }
    public async Task InitializeAsync(nint handle, CancellationToken token = default)
    {
        await inner.InitializeAsync(handle, token);
        if (inner is MpvMediaPlayerService native) NativeVideoCapture.EnableLog(native);
        await inner.SetVolumeAsync(0, token);
    }
    public Task LoadAsync(string path, CancellationToken token = default) => inner.LoadAsync(path, token);
    public Task PlayAsync(CancellationToken token = default) => inner.PlayAsync(token);
    public Task PauseAsync(CancellationToken token = default) => inner.PauseAsync(token);
    public Task StepForwardAsync(CancellationToken token = default) => inner.StepForwardAsync(token);
    public Task StepFramesAsync(int count, CancellationToken token = default) => inner.StepFramesAsync(count, token);
    public Task StepBackwardAsync(CancellationToken token = default) => inner.StepBackwardAsync(token);
    public Task SeekAsync(TimeSpan position, CancellationToken token = default)
    {
        RequestedSeeks.Add(position);
        return inner.SeekAsync(position, token);
    }
    public Task SetVolumeAsync(double volume, CancellationToken token = default) => inner.SetVolumeAsync(volume, token);
    public Task StopAsync(CancellationToken token = default) => inner.StopAsync(token);
    public ValueTask DisposeAsync() => inner.DisposeAsync();
}
