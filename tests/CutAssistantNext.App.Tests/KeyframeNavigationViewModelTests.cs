using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class KeyframeNavigationViewModelTests
{
    private static MediaAnalysisResult Media(string format = "mp4", int streams = 1) => new(format, null, null, TimeSpan.FromSeconds(10),
        Enumerable.Range(0, streams).Select(index => new VideoStreamInfo(index, "h264", null, 320, 180, null, null, 25, null)).ToArray(), []);
    private static VideoFrameWindow Window(long last = 8000) => new(0, new VideoTimeBase(1, 1000), 0, 0,
        new[] { 0L, 2000, 4000, last }.Select((pts, i) => new VideoFrameInfo(i, pts, null, null, null, true, "I")).ToArray());
    private static PlaybackViewModelTests.StubMediaPlayerService Player() => new()
    {
        State = MediaPlayerState.Paused,
        Duration = TimeSpan.FromSeconds(10),
        Position = TimeSpan.FromSeconds(3)
    };

    [Theory]
    [InlineData(-1, 2)]
    [InlineData(1, 4)]
    public async Task JumpsFromReachedPositionWithoutChangingCutPlan(int direction, int expected)
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        var plan = new CutPlanViewModel(); plan.Initialize(TimeSpan.FromSeconds(10)); plan.SetStart(TimeSpan.FromSeconds(1));
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        await vm.JumpAsync(direction);
        Assert.Equal(TimeSpan.FromSeconds(expected), Assert.Single(player.SeekPositions));
        Assert.Equal(" (K)", vm.Indicator);
        Assert.Empty(plan.RemoveSegments);
        Assert.Equal(TimeSpan.FromSeconds(1), plan.PendingStart);
        await vm.JumpAsync(direction);
        Assert.Equal(2, player.SeekPositions.Count);
        Assert.NotEqual(player.SeekPositions[0], player.SeekPositions[1]);
    }
    [Fact]
    public async Task PlayingJumpPausesAndOnlyPausedFramesShowK()
    {
        var player = Player(); player.State = MediaPlayerState.Playing; player.Position = TimeSpan.FromSeconds(2);
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        Assert.Empty(vm.Indicator);
        await vm.JumpAsync(1);
        Assert.Equal(1, player.PauseCallCount);
        Assert.Equal(TimeSpan.FromSeconds(4), Assert.Single(player.SeekPositions));
        player.State = MediaPlayerState.Paused; player.RaiseStateChanged();
        Assert.Equal(" (K)", vm.Indicator);
        player.Position = TimeSpan.FromSeconds(4.04); player.RaisePositionChanged();
        Assert.Empty(vm.Indicator);
    }
    [Fact]
    public async Task SeekingHidesKAndDisablesKeyframeJumps()
    {
        var player = Player(); player.Position = TimeSpan.FromSeconds(2);
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        Assert.Equal(" (K)", vm.Indicator);
        playback.BeginSeek(); playback.UpdateSeekPosition(8);
        Assert.False(vm.CanNext); Assert.False(vm.CanPrevious); Assert.Empty(vm.Indicator);
        await vm.JumpAsync(1); Assert.Empty(player.SeekPositions);
        playback.CancelSeek(); Assert.True(vm.CanNext); Assert.Equal(" (K)", vm.Indicator);
    }
    [Theory]
    [InlineData(0, false, true)]
    [InlineData(8, true, false)]
    public async Task BoundariesDisableOnlyUnavailableDirection(int position, bool previous, bool next)
    {
        var player = Player(); player.Position = TimeSpan.FromSeconds(position);
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        Assert.Equal(previous, vm.CanPrevious); Assert.Equal(next, vm.CanNext);
        await vm.JumpAsync(previous ? 1 : -1); Assert.Empty(player.SeekPositions);
    }
    [Fact]
    public async Task LoadingDoesNotBlockPlaybackAndRejectsStaleResultAfterMediaSwitch()
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        var runner = new Runner { Pending = new TaskCompletionSource<VideoFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously) };
        var first = vm.LoadAsync("old.mp4", Media(), runner);
        Assert.True(vm.IsLoading); Assert.True(playback.CanSeek); Assert.False(vm.CanNext);
        await vm.LoadAsync("new.mp4", Media("mp4"), new Runner { Window = Window(9000) });
        runner.Pending.SetResult(Window());
        await first;
        Assert.Equal(TimeSpan.FromSeconds(9), vm.Positions.Last());
        Assert.False(vm.IsLoading);
    }
    [Fact]
    public async Task ResetCancelsAnalysisAndClearsOldMarkersAndIndicator()
    {
        var player = Player(); player.Position = TimeSpan.Zero;
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        Assert.NotEmpty(vm.Positions); Assert.Equal(" (K)", vm.Indicator);
        vm.Reset(); Assert.Empty(vm.Positions); Assert.Empty(vm.Indicator); Assert.False(vm.CanNext);
        await vm.JumpAsync(1); Assert.Empty(player.SeekPositions);
    }
    [Theory]
    [InlineData("analysis")]
    [InlineData("empty")]
    [InlineData("stream")]
    public async Task UnsafeAnalysisLeavesNormalPlaybackAvailable(string reason)
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(streams: reason == "stream" ? 2 : 1),
            new Runner { Failure = reason == "analysis", Window = reason == "empty" ? Window() with { Frames = [] } : Window() });
        Assert.False(vm.IsLoading); Assert.Empty(vm.Positions); Assert.Empty(vm.Indicator);
        Assert.False(vm.CanNext); Assert.True(playback.CanSeek); Assert.Contains("nicht verfügbar", vm.Status);
    }
    [Fact]
    public async Task FailedSeekIsReportedAndReleasesBusyState()
    {
        var player = Player(); player.SeekError = new InvalidOperationException("seek failed");
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        await vm.JumpAsync(1);
        Assert.False(vm.IsNavigating); Assert.Empty(vm.Indicator); Assert.Contains("seek failed", vm.Status);
    }
    [Fact]
    public async Task ParallelJumpIsIgnoredAndResetCancelsWaitingSeek()
    {
        var player = Player(); var pending = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); player.SeekCompletion = pending.Task;
        using var playback = new PlaybackViewModel(player);
        using var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        var jump = vm.JumpAsync(1);
        Assert.True(vm.IsNavigating); Assert.Empty(vm.Indicator); Assert.False(vm.CanNext);
        await vm.JumpAsync(-1); Assert.Single(player.SeekPositions);
        vm.Reset(); await jump;
        Assert.Empty(vm.Positions); Assert.False(vm.IsNavigating);
    }
    [Theory]
    [InlineData("avi", true)]
    [InlineData("AVI", true)]
    [InlineData("other, avi", true)]
    [InlineData("mov,mp4", false)]
    public async Task UsesDetectedContainerInsteadOfFileExtension(string format, bool expected)
    {
        using var playback = new PlaybackViewModel(Player());
        using var vm = new KeyframeNavigationViewModel(playback);
        var runner = new Runner();
        await vm.LoadAsync("wrong-extension.mp4", Media(format), runner);
        Assert.Equal(!expected, vm.IsSupported);
        Assert.Equal(expected ? 0 : 1, runner.Calls);
        if (expected)
        {
            Assert.Empty(vm.Positions); Assert.Empty(vm.Indicator);
            Assert.False(vm.CanPrevious); Assert.False(vm.CanNext);
            Assert.Contains("für AVI nicht verfügbar", vm.Status);
            Assert.Contains("Die Frame-Lupe steht für AVI-Container derzeit nicht zur Verfügung.", vm.Status);
        }
    }
    [Fact]
    public async Task DisposalClearsMarkersAndRejectsFurtherLoading()
    {
        using var playback = new PlaybackViewModel(Player());
        var vm = new KeyframeNavigationViewModel(playback);
        await vm.LoadAsync("video.mp4", Media(), new Runner());
        vm.Dispose(); vm.Dispose();
        Assert.Empty(vm.Positions); Assert.False(vm.CanNext); Assert.Empty(vm.Indicator);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => vm.LoadAsync("video.mp4", Media(), new Runner()));
    }
    private sealed class Runner : IKeyframeAnalysisRunner
    {
        internal VideoFrameWindow Window { get; init; } = KeyframeNavigationViewModelTests.Window();
        internal TaskCompletionSource<VideoFrameWindow>? Pending { get; init; }
        internal bool Failure { get; init; }
        internal int Calls { get; private set; }
        public Task<VideoFrameWindow> RunKeyframesAsync(string path, int stream, CancellationToken token = default)
        {
            Calls++;
            if (Failure) throw new InvalidOperationException("analysis failed");
            return Pending?.Task ?? Task.FromResult(Window);
        }
    }
}
