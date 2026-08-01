using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class PlaybackViewModelTests
{
    [Fact]
    public void Constructor_ProjectsInitialServiceState()
    {
        var service = new StubMediaPlayerService
        {
            State = MediaPlayerState.Paused,
            Position = TimeSpan.FromSeconds(65),
            Duration = TimeSpan.FromSeconds(130)
        };

        using var viewModel =
            new PlaybackViewModel(service);

        Assert.Equal(
            MediaPlayerState.Paused,
            viewModel.State);

        Assert.Equal(
            TimeSpan.FromSeconds(65),
            viewModel.Position);

        Assert.Equal(
            TimeSpan.FromSeconds(130),
            viewModel.Duration);

        Assert.Equal(65, viewModel.PositionSeconds);
        Assert.Equal(130, viewModel.DurationSeconds);

        Assert.Equal("00:01:05", viewModel.PositionText);
        Assert.Equal("00:02:10", viewModel.DurationText);

        Assert.True(viewModel.CanPlay);
        Assert.False(viewModel.CanPause);
        Assert.True(viewModel.CanSeek);
        Assert.False(viewModel.HasError);
    }

    [Theory]
    [InlineData(MediaPlayerState.Empty, false)]
    [InlineData(MediaPlayerState.Loading, false)]
    [InlineData(MediaPlayerState.Paused, true)]
    [InlineData(MediaPlayerState.Playing, false)]
    [InlineData(MediaPlayerState.Ended, false)]
    [InlineData(MediaPlayerState.Error, false)]
    public void CanStepFrame_DependsOnPausedState(
        MediaPlayerState state,
        bool expected)
    {
        var service = new StubMediaPlayerService
        {
            State = state
        };

        using var viewModel =
            new PlaybackViewModel(service);

        Assert.Equal(expected, viewModel.CanStepFrame);
    }

    [Fact]
    public void ServiceEvents_RaiseExpectedPropertyChanges()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        var changedProperties = new List<string>();

        viewModel.PropertyChanged +=
            (_, eventArgs) =>
            {
                if (eventArgs.PropertyName is not null)
                {
                    changedProperties.Add(
                        eventArgs.PropertyName);
                }
            };

        service.State = MediaPlayerState.Playing;
        service.RaiseStateChanged();

        service.Position =
            TimeSpan.FromSeconds(12);

        service.Duration =
            TimeSpan.FromSeconds(120);

        service.RaisePositionChanged();

        service.ErrorMessage = "Testfehler";
        service.RaiseErrorOccurred();

        Assert.Contains(
            nameof(PlaybackViewModel.State),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.CanPause),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.CanStepFrame),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.PositionText),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.DurationText),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.ErrorMessage),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.HasError),
            changedProperties);

        Assert.Equal("00:00:12", viewModel.PositionText);
        Assert.Equal("00:02:00", viewModel.DurationText);
        Assert.True(viewModel.CanPause);
        Assert.True(viewModel.HasError);
    }

    [Fact]
    public void DurationChanged_UpdatesDurationProperties()
    {
        var service = new StubMediaPlayerService
        {
            State = MediaPlayerState.Paused,
            Position = TimeSpan.FromSeconds(10)
        };

        using var viewModel =
            new PlaybackViewModel(service);

        var changedProperties = new List<string>();

        viewModel.PropertyChanged +=
            (_, eventArgs) =>
            {
                if (eventArgs.PropertyName is not null)
                {
                    changedProperties.Add(
                        eventArgs.PropertyName);
                }
            };

        service.Duration =
            TimeSpan.FromSeconds(120);

        service.RaiseDurationChanged();

        Assert.Equal(120, viewModel.DurationSeconds);
        Assert.Equal("00:02:00", viewModel.DurationText);
        Assert.True(viewModel.CanSeek);

        Assert.Contains(
            nameof(PlaybackViewModel.DurationSeconds),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.DurationText),
            changedProperties);

        Assert.Contains(
            nameof(PlaybackViewModel.CanSeek),
            changedProperties);
    }

    [Fact]
    public async Task PlayAsync_ForwardsToService()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        await viewModel.PlayAsync();

        Assert.Equal(1, service.PlayCallCount);
    }

    [Fact]
    public async Task PauseAsync_ForwardsToService()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        await viewModel.PauseAsync();

        Assert.Equal(1, service.PauseCallCount);
    }

    [Fact]
    public async Task StepForwardAsync_ForwardsToService()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        await viewModel.StepForwardAsync();

        Assert.Equal(1, service.StepForwardCallCount);
    }

    [Fact]
    public async Task StepBackwardAsync_ForwardsToService()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        await viewModel.StepBackwardAsync();

        Assert.Equal(1, service.StepBackwardCallCount);
    }

    [Fact]
    public async Task StepBackwardTenFramesAsync_ForwardsMinusTenToService()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        await viewModel.StepBackwardTenFramesAsync();

        Assert.Equal(
            [-10],
            service.StepFrameCounts);
    }

    [Fact]
    public async Task StepForwardTenFramesAsync_ForwardsTenToService()
    {
        var service = new StubMediaPlayerService();

        using var viewModel =
            new PlaybackViewModel(service);

        await viewModel.StepForwardTenFramesAsync();

        Assert.Equal(
            [10],
            service.StepFrameCounts);
    }

    [Fact]
    public void Seeking_KeepsTimelinePositionDuringServiceUpdates()
    {
        var service = new StubMediaPlayerService
        {
            State = MediaPlayerState.Playing,
            Position = TimeSpan.FromSeconds(10),
            Duration = TimeSpan.FromSeconds(120)
        };

        using var viewModel =
            new PlaybackViewModel(service);

        viewModel.BeginSeek();
        viewModel.UpdateSeekPosition(42.5);

        service.Position =
            TimeSpan.FromSeconds(20);

        service.RaisePositionChanged();

        Assert.True(viewModel.IsSeeking);
        Assert.Equal(20, viewModel.PositionSeconds);
        Assert.Equal(42.5, viewModel.TimelinePositionSeconds);
    }

    [Fact]
    public void BeginSeek_WithoutDuration_DoesNothing()
    {
        var service = new StubMediaPlayerService
        {
            State = MediaPlayerState.Playing,
            Position = TimeSpan.FromSeconds(10),
            Duration = null
        };

        using var viewModel =
            new PlaybackViewModel(service);

        viewModel.BeginSeek();

        Assert.False(viewModel.IsSeeking);
        Assert.Equal(10, viewModel.TimelinePositionSeconds);
    }

    [Fact]
    public void CancelSeek_RestoresCurrentServicePosition()
    {
        var service = new StubMediaPlayerService
        {
            State = MediaPlayerState.Paused,
            Position = TimeSpan.FromSeconds(10),
            Duration = TimeSpan.FromSeconds(120)
        };

        using var viewModel =
            new PlaybackViewModel(service);

        viewModel.BeginSeek();
        viewModel.UpdateSeekPosition(45);

        Assert.Equal(45, viewModel.TimelinePositionSeconds);

        viewModel.CancelSeek();

        Assert.False(viewModel.IsSeeking);
        Assert.Equal(10, viewModel.TimelinePositionSeconds);
        Assert.Empty(service.SeekPositions);
    }

    [Fact]
    public async Task CommitSeekAsync_ClampsAndForwardsPosition()
    {
        var service = new StubMediaPlayerService
        {
            State = MediaPlayerState.Paused,
            Position = TimeSpan.FromSeconds(10),
            Duration = TimeSpan.FromSeconds(120)
        };

        using var viewModel =
            new PlaybackViewModel(service);

        viewModel.BeginSeek();
        viewModel.UpdateSeekPosition(150);

        Assert.Equal(120, viewModel.TimelinePositionSeconds);

        await viewModel.CommitSeekAsync();

        Assert.False(viewModel.IsSeeking);

        Assert.Equal(
            new[] { TimeSpan.FromSeconds(120) },
            service.SeekPositions);
    }

    [Fact]
    public void Dispose_UnsubscribesFromServiceEvents()
    {
        var service = new StubMediaPlayerService();
        var viewModel = new PlaybackViewModel(service);

        var propertyChangedCount = 0;

        viewModel.PropertyChanged +=
            (_, _) => propertyChangedCount++;

        viewModel.Dispose();

        service.RaiseStateChanged();
        service.RaisePositionChanged();
        service.RaiseDurationChanged();
        service.RaiseErrorOccurred();

        Assert.Equal(0, propertyChangedCount);
    }

    private sealed class StubMediaPlayerService
        : IMediaPlayerService
    {
        public MediaPlayerState State { get; set; } =
            MediaPlayerState.Empty;

        public TimeSpan Position { get; set; } =
            TimeSpan.Zero;

        public TimeSpan? Duration { get; set; }

        public string? ErrorMessage { get; set; }

        public int PlayCallCount { get; private set; }

        public int PauseCallCount { get; private set; }

        public int StepForwardCallCount { get; private set; }

        public int StepBackwardCallCount { get; private set; }

        public List<int> StepFrameCounts { get; } = [];

        public List<TimeSpan> SeekPositions { get; } = [];

        public event EventHandler? StateChanged;

        public event EventHandler? PositionChanged;

        public event EventHandler? DurationChanged;

        public event EventHandler? ErrorOccurred;

        public Task InitializeAsync(
            nint videoWindowHandle,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(
            string mediaFilePath,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task PlayAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PlayCallCount++;

            return Task.CompletedTask;
        }

        public Task PauseAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PauseCallCount++;

            return Task.CompletedTask;
        }

        public Task StepForwardAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            StepForwardCallCount++;

            return Task.CompletedTask;
        }

        public Task StepBackwardAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            StepBackwardCallCount++;

            return Task.CompletedTask;
        }

        public Task StepFramesAsync(
            int frameCount,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            StepFrameCounts.Add(frameCount);

            return Task.CompletedTask;
        }

        public Task SeekAsync(
            TimeSpan position,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SeekPositions.Add(position);

            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public void RaiseStateChanged()
        {
            StateChanged?.Invoke(
                this,
                EventArgs.Empty);
        }

        public void RaisePositionChanged()
        {
            PositionChanged?.Invoke(
                this,
                EventArgs.Empty);
        }

        public void RaiseDurationChanged()
        {
            DurationChanged?.Invoke(
                this,
                EventArgs.Empty);
        }

        public void RaiseErrorOccurred()
        {
            ErrorOccurred?.Invoke(
                this,
                EventArgs.Empty);
        }
    }
}
