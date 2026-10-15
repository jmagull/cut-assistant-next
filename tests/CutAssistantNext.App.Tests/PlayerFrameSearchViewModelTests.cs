using System.IO;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class PlayerFrameSearchViewModelTests
{
    private static PlaybackViewModelTests.StubMediaPlayerService Player(double seconds = 40, double duration = 400) => new()
    {
        State = MediaPlayerState.Paused,
        Position = TimeSpan.FromSeconds(seconds),
        Duration = TimeSpan.FromSeconds(duration)
    };

    private static FrameLoupeViewModel Navigator(TimeSpan position, Analysis analysis, Preview? preview = null,
        double duration = 400, string? format = null) => new("video.mp4", 0, TimeSpan.FromSeconds(duration), position,
            null, analysis.Origin / 1000m, 2000, analysis, preview ?? new Preview(), format);

    private static void Configure(PlayerFrameSearchViewModel vm, Analysis? analysis = null, int step = 2000,
        Preview? preview = null, double duration = 400, string? format = null) =>
        vm.SetMedia(position => Navigator(position, analysis ?? new Analysis(), preview, duration, format), step);

    [Fact]
    public async Task FirstAndRepeatedJumpsThenDirectionChangesUseReachedFrames()
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm);
        foreach (var (direction, expected, step) in new[] { (1, 120d, 2000), (1, 200d, 2000),
            (-1, 160d, 1000), (-1, 120d, 1000), (1, 140d, 500), (-1, 130d, 250) })
        {
            await vm.SearchAsync(direction);
            Assert.False(vm.IsSearchError, vm.Status);
            Assert.Equal(TimeSpan.FromSeconds(expected), player.Position);
            Assert.Equal(step, vm.NextSearchStep);
            Assert.False(vm.IsNavigating);
            Assert.True(vm.CanSearch);
        }
        Assert.Equal(6, player.SeekPositions.Count);
        Assert.Contains("125", vm.SearchRightText);
        Assert.Contains("250", vm.SearchLeftText);
    }

    [Fact]
    public async Task AlternationUsesIntegerFloorAndNeverLessThanOneFrame()
    {
        var player = Player(200);
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm);
        int direction = -1;
        foreach (int step in new[] { 2000, 1000, 500, 250, 125, 62, 31, 15, 7, 3, 1, 1 })
        {
            var origin = player.Position;
            await vm.SearchAsync(direction);
            Assert.False(vm.IsSearchError, vm.Status);
            // PlaybackViewModel uses the existing double-valued slider path (up to one tick of roundoff).
            Assert.InRange(Math.Abs((player.Position - origin - TimeSpan.FromMilliseconds(direction * step * 40)).Ticks), 0, 1);
            Assert.Equal(step, vm.NextSearchStep);
            direction = -direction;
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ResetRestoresCustomStepAndDirectionWithoutPositionOrCutChanges(bool pendingStart)
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 125);
        var plan = new CutPlanViewModel();
        plan.Initialize(player.Duration!.Value); plan.SetStart(TimeSpan.FromSeconds(2)); plan.SetEnd(TimeSpan.FromSeconds(4));
        var segment = plan.RemoveSegments[0];
        if (pendingStart) { plan.SelectedRemoveSegment = null; plan.SetStart(TimeSpan.FromSeconds(7)); }
        else plan.SelectEdge(segment, CutEdgeSide.End);
        var segments = plan.RemoveSegments.ToArray(); var pending = plan.PendingStart;
        var selected = plan.SelectedRemoveSegment; var side = plan.SelectedEdgeSide;
        await vm.SearchAsync(1); await vm.SearchAsync(-1);
        Assert.Equal(62, vm.NextSearchStep);
        var position = player.Position; int seeks = player.SeekPositions.Count;
        for (int i = 0; i < 3; i++) vm.ResetSearch();
        Assert.Equal(125, vm.NextSearchStep);
        Assert.Equal(position, player.Position); Assert.Equal(seeks, player.SeekPositions.Count);
        await vm.SearchAsync(-1);
        Assert.InRange(Math.Abs((position - TimeSpan.FromSeconds(5) - player.Position).Ticks), 0, 1);
        Assert.Equal(segments, plan.RemoveSegments); Assert.Equal(pending, plan.PendingStart);
        Assert.Same(selected, plan.SelectedRemoveSegment); Assert.Equal(side, plan.SelectedEdgeSide);
    }

    [Theory]
    [InlineData(0, -1, 0, 2000)]
    [InlineData(0, 1, 9.96, 2000)]
    [InlineData(9.96, 1, 9.96, 2000)]
    [InlineData(9.96, -1, 0, 2000)]
    [InlineData(10, -1, 0, 2000)]
    public async Task BoundariesClampToRealFramesWithoutConsumingAnImmovableJump(double origin, int direction, double expected, int step)
    {
        var player = Player(origin, 10);
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, duration: 10);
        await vm.SearchAsync(direction);
        Assert.False(vm.IsSearchError, vm.Status);
        Assert.Equal(TimeSpan.FromSeconds(expected), player.Position);
        Assert.Equal(step, vm.NextSearchStep);
        if (origin == expected) Assert.Empty(player.SeekPositions);
        else Assert.Single(player.SeekPositions);
    }

    [Fact]
    public async Task OneFrameRecordingDoesNotMoveOrConsumeDirection()
    {
        var player = Player(0, .04);
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, duration: .04);
        await vm.SearchAsync(1); await vm.SearchAsync(-1);
        Assert.False(vm.IsSearchError, vm.Status); Assert.Empty(player.SeekPositions);
        Assert.Equal(2000, vm.NextSearchStep);
    }

    [Theory]
    [InlineData("slider")]
    [InlineData("step")]
    [InlineData("play")]
    public async Task ManualNavigationStartsNewSearchAtActualPosition(string kind)
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 10);
        await vm.SearchAsync(1); await vm.SearchAsync(-1);
        Assert.Equal(5, vm.NextSearchStep);
        if (kind == "slider") { playback.BeginSeek(); playback.UpdateSeekPosition(300); }
        else if (kind == "step") { await playback.StepForwardAsync(); player.Position = TimeSpan.FromSeconds(100); player.RaisePositionChanged(); }
        else { await playback.PlayAsync(); player.State = MediaPlayerState.Playing; player.RaiseStateChanged(); }
        Assert.Equal(10, vm.NextSearchStep);
        if (kind == "slider")
        {
            Assert.False(vm.CanSearch); await vm.SearchAsync(1); Assert.Equal(2, player.SeekPositions.Count);
            playback.CancelSeek();
        }
        var origin = player.Position;
        await vm.SearchAsync(-1);
        Assert.False(vm.IsSearchError, vm.Status);
        Assert.Equal(origin - TimeSpan.FromMilliseconds(400), player.Position);
        if (kind == "play") Assert.Equal(1, player.PauseCallCount);
    }

    [Fact]
    public async Task LateNotificationOfConfirmedPositionDoesNotResetReducedSearchStep()
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 10);
        await vm.SearchAsync(1); await vm.SearchAsync(-1);
        Assert.Equal(5, vm.NextSearchStep);
        // Native state can already contain the target while its UI notification is still queued.
        player.RaisePositionChanged();
        player.RaiseStateChanged();
        Assert.Equal(5, vm.NextSearchStep);
        await vm.SearchAsync(-1);
        Assert.False(vm.IsSearchError, vm.Status);
        Assert.Equal(5, vm.NextSearchStep);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9000)]
    public async Task VariableFrameTimesAndContainerOriginUseActualFrameTimestamps(long origin)
    {
        var analysis = new Analysis { Variable = true, Origin = origin };
        var player = Player(50, 500);
        // 1000th frame has playback time 50 s, independent of the displayed frame estimate.
        player.FrameNumber = 123; player.EstimatedFrameCount = null;
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, analysis, step: 11, duration: 500);
        await vm.SearchAsync(1);
        Assert.False(vm.IsSearchError, vm.Status);
        Assert.Equal(TimeSpan.FromMilliseconds(analysis.Timestamp(1011) - origin), player.Position);
        Assert.NotEqual(TimeSpan.FromSeconds(50.44), player.Position);
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("duplicate")]
    [InlineData("preview")]
    [InlineData("seek")]
    [InlineData("factory")]
    [InlineData("analysis")]
    public async Task UnsafeFramesAndFailuresNeverConsumeDirection(string kind)
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        var preview = new Preview(); var analysis = new Analysis();
        Configure(vm, analysis, step: 10, preview: preview);
        await vm.SearchAsync(1);
        Assert.False(vm.IsSearchError, vm.Status);
        int seeks = player.SeekPositions.Count;
        if (kind == "missing") analysis.Missing = true;
        if (kind == "duplicate") analysis.Duplicate = true;
        if (kind == "preview") preview.Fail = true;
        if (kind == "analysis") analysis.Fail = true;
        if (kind == "seek") player.SeekError = new InvalidOperationException("seek failed");
        if (kind == "factory") vm.SetMedia(_ => throw new InvalidOperationException("tool missing"), 10);
        await vm.SearchAsync(-1);
        Assert.True(vm.IsSearchError); Assert.False(vm.IsNavigating); Assert.True(vm.CanSearch);
        Assert.Equal(10, vm.NextSearchStep); Assert.Equal(seeks, player.SeekPositions.Count);
        Assert.Contains("nicht bestätigt", vm.Status);
        if (kind == "analysis") Assert.Contains("ffprobe.exe wurde nicht gefunden", vm.Status);
    }

    [Fact]
    public async Task UnavailableMediaAndDisposeDisableSearchAndCancelPendingWork()
    {
        using var playback = new PlaybackViewModel(Player());
        var vm = new PlayerFrameSearchViewModel(playback);
        Assert.False(vm.CanSearch); Assert.False(vm.CanReset);
        await vm.SearchAsync(1);
        var analysis = new Analysis { Pending = new TaskCompletionSource() };
        Configure(vm, analysis);
        var jump = vm.SearchAsync(1);
        Assert.True(vm.IsNavigating); Assert.False(vm.CanInteract);
        vm.Dispose(); vm.Dispose(); await jump;
        Assert.False(vm.IsNavigating); Assert.False(vm.CanSearch);
        Assert.Throws<ObjectDisposedException>(() => vm.SetMedia(null));
    }

    [Fact]
    public async Task VideoSwitchCancelsOldSearchAndInitializesIndependentNewState()
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 10); await vm.SearchAsync(1); await vm.SearchAsync(-1);
        Assert.Equal(5, vm.NextSearchStep);
        var analysis = new Analysis { Pending = new TaskCompletionSource() };
        Configure(vm, analysis, step: 10);
        var oldSearch = vm.SearchAsync(1);
        Assert.True(vm.IsNavigating);
        await vm.SearchAsync(-1); vm.ResetSearch();
        Assert.Equal(10, vm.NextSearchStep);
        vm.SetMedia(null); await oldSearch;
        Assert.False(vm.CanSearch); Assert.False(vm.IsNavigating);
        Configure(vm, step: 125);
        var origin = player.Position;
        await vm.SearchAsync(-1);
        Assert.False(vm.IsSearchError, vm.Status);
        Assert.Equal(origin - TimeSpan.FromSeconds(5), player.Position); Assert.Equal(125, vm.NextSearchStep);
        Assert.Equal(3, player.SeekPositions.Count);
    }

    [Fact]
    public async Task SearchWaitsForReachedPlayerPositionAndIgnoresParallelJump()
    {
        var player = Player(); var completion = new TaskCompletionSource(); player.SeekCompletion = completion.Task;
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 10);
        var search = vm.SearchAsync(1);
        Assert.True(vm.IsNavigating); Assert.Equal(TimeSpan.FromSeconds(40), player.Position);
        await vm.SearchAsync(-1); vm.ResetSearch(); Assert.Single(player.SeekPositions);
        completion.SetResult(); await search;
        Assert.False(vm.IsSearchError, vm.Status); Assert.False(vm.IsNavigating);
        Assert.Equal(TimeSpan.FromSeconds(40.4), player.Position);
    }

    [Theory]
    [InlineData(4, false)]
    [InlineData(40, true)]
    public async Task SettledPlayerClockMustRemainInsideMeasuredTargetFrame(int offsetMilliseconds, bool error)
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 10);
        await vm.SearchAsync(1);
        var jump = vm.SearchAsync(-1);
        await Task.Delay(20);
        player.Position += TimeSpan.FromMilliseconds(offsetMilliseconds);
        player.RaisePositionChanged();
        await jump;
        Assert.Equal(error, vm.IsSearchError);
        Assert.Equal(error ? 10 : 5, vm.NextSearchStep);
        Assert.Equal(player.Position, playback.Position);
    }

    [Fact]
    public async Task MainAndLoupeHaveIndependentSearchStates()
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        Configure(vm, step: 10);
        var loupe = Navigator(player.Position, new Analysis());
        await loupe.InitializeAsync(); await loupe.SearchAsync(1); await loupe.SearchAsync(-1);
        Assert.Equal(1000, loupe.NextSearchStep);
        await vm.SearchAsync(1); await vm.SearchAsync(-1); vm.ResetSearch();
        Assert.False(vm.IsSearchError, vm.Status);
        Assert.Equal(1000, loupe.NextSearchStep);
        await vm.SearchAsync(1); await vm.SearchAsync(-1);
        loupe.ResetSearch(); Assert.Equal(5, vm.NextSearchStep);
        Assert.Equal(2000, loupe.NextSearchStep);
    }

    [Fact]
    public async Task MainSearchRejectsReconstructedAviTimestampsBeforeMovingPlayer()
    {
        var player = Player();
        using var playback = new PlaybackViewModel(player);
        using var vm = new PlayerFrameSearchViewModel(playback);
        var analysis = new Analysis { Missing = true };
        Configure(vm, analysis, step: 10, format: "avi");
        await vm.SearchAsync(1);
        Assert.True(vm.IsSearchError); Assert.Contains("Original-Zeitstempel", vm.Status);
        Assert.Equal(TimeSpan.FromSeconds(40), player.Position); Assert.Empty(player.SeekPositions);
        Assert.Equal(10, vm.NextSearchStep);
    }

    private sealed class Analysis : IFrameAnalysisRunner
    {
        public long Origin { get; init; }
        public bool Variable { get; init; }
        public bool Missing { get; set; }
        public bool Fail { get; set; }
        public bool Duplicate { get; set; }
        public TaskCompletionSource? Pending { get; init; }
        public long Timestamp(int index) => Origin + index * 40L + (Variable ? index / 2 * 20L : 0);
        public async Task<VideoFrameWindow> RunAsync(string path, int stream, TimeSpan start, TimeSpan end,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (Pending is not null) await Pending.Task.WaitAsync(token);
            if (Fail) throw new FileNotFoundException("ffprobe.exe wurde nicht gefunden.");
            var frames = Enumerable.Range(0, 10000).Select(index => Timestamp(index))
                .Where(pts => pts >= (decimal)start.Ticks / 10000 && pts < (decimal)end.Ticks / 10000)
                .Select((pts, index) => new VideoFrameInfo(index, Missing ? null : pts, pts, null, null, index % 25 == 0, "I")).ToList();
            if (Duplicate && frames.Count > 1) frames[1] = frames[0] with { LocalIndex = 1 };
            return new VideoFrameWindow(stream, new VideoTimeBase(1, 1000), Origin / 1000m, Origin / 1000m, frames);
        }
    }

    private sealed class Preview : IFramePreviewRunner
    {
        public bool Fail { get; set; }
        public Task<FramePreview> RunAsync(string path, int stream, VideoTimeBase timeBase, long pts,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (Fail) throw new InvalidOperationException("preview failed");
            return Task.FromResult(new FramePreview([137, 80, 78, 71], pts, timeBase));
        }
    }
}
