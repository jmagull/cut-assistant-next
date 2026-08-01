namespace CutAssistantNext.Media.Playback;

public interface IMediaPlayerService : IAsyncDisposable
{
    MediaPlayerState State { get; }

    TimeSpan Position { get; }

    TimeSpan? Duration { get; }

    long? FrameNumber { get; }

    long? EstimatedFrameCount { get; }

    string? ErrorMessage { get; }

    event EventHandler? StateChanged;

    event EventHandler? PositionChanged;

    event EventHandler? DurationChanged;

    event EventHandler? FrameChanged;

    event EventHandler? ErrorOccurred;

    Task InitializeAsync(
        nint videoWindowHandle,
        CancellationToken cancellationToken = default);

    Task LoadAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default);

    Task PlayAsync(
        CancellationToken cancellationToken = default);

    Task PauseAsync(
        CancellationToken cancellationToken = default);

    Task StepForwardAsync(
        CancellationToken cancellationToken = default);

    Task StepFramesAsync(
        int frameCount,
        CancellationToken cancellationToken = default);

    Task StepBackwardAsync(
        CancellationToken cancellationToken = default);

    Task SeekAsync(
        TimeSpan position,
        CancellationToken cancellationToken = default);

    Task StopAsync(
        CancellationToken cancellationToken = default);
}
