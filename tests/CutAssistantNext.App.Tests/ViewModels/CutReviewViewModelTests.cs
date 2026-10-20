using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutReviewViewModelTests
{
    private static CutCompletionViewModel Completion(string file = "cut.mp4", double first = 10)
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(120));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(first), TimeSpan.FromSeconds(first + 20)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(80)));
        return new CutCompletionViewModel(file, plan);
    }

    [Fact]
    public async Task LoadsSeeksThenPlaysUsingOutputTimes()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 2, 123);
        Assert.Equal(["initialize:123", "load:cut.mp4", "seek:37", "play"], p.Commands);
        Assert.True(model.CanPrevious);
        Assert.True(model.CanNext);
        Assert.Equal("Übergang 2 von 2 · 00:00:40.000", model.CurrentTransitionText);
    }

    [Theory]
    [InlineData(2, 100, 0)]
    [InlineData(10, 100, 7)]
    [InlineData(10, 5, 4.9999999)]
    public async Task ReviewStartIsBoundedByZeroAndActualDuration(double point, double duration, double expected)
    {
        var p = new Player { ActualDuration = TimeSpan.FromSeconds(duration) };
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(first: point), 1, 123);
        Assert.Equal(TimeSpan.FromSeconds(expected), p.Position);
    }

    [Fact]
    public async Task WaitsForFileLoadedAndDurationBeforeSeek()
    {
        var p = new Player { DeferredLoad = true };
        await using var model = new CutReviewViewModel(p, _ => true);
        var review = model.ReviewAsync(Completion(), 1, 123);
        Assert.False(review.IsCompleted);
        Assert.False(model.IsIdle);
        Assert.DoesNotContain(p.Commands, c => c.StartsWith("seek:"));
        p.ReadyState();
        Assert.False(review.IsCompleted);
        p.ReadyDuration();
        await review;
        Assert.Equal(["initialize:123", "load:cut.mp4", "seek:7", "play"], p.Commands);
    }

    [Fact]
    public async Task DurationBeforeFileLoadedIsAlsoSupported()
    {
        var p = new Player { DeferredLoad = true };
        await using var model = new CutReviewViewModel(p, _ => true);
        var review = model.ReviewAsync(Completion(), 1, 123);
        p.ReadyDuration();
        Assert.False(review.IsCompleted);
        p.ReadyState();
        await review;
        Assert.Equal(MediaPlayerState.Playing, p.State);
    }

    [Fact]
    public async Task SameOutputReusesPlayerAndNavigationIncludesStartAndEndWithoutWrapping()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 0, 123);
        Assert.False(model.CanPrevious);
        Assert.True(model.CanNext);
        await model.MoveAsync(-1, 123);
        await model.MoveAsync(1, 123);
        await model.MoveAsync(1, 123);
        await model.MoveAsync(1, 123);
        Assert.True(model.CanPrevious);
        Assert.False(model.CanNext);
        await model.MoveAsync(1, 123);
        await model.MoveAsync(-1, 123);
        Assert.Single(p.Commands, c => c.StartsWith("initialize:"));
        Assert.Single(p.Commands, c => c.StartsWith("load:"));
        Assert.Equal(["seek:0", "seek:7", "seek:37", "seek:97", "seek:37"],
            p.Commands.Where(c => c.StartsWith("seek:")));
    }

    [Fact]
    public async Task OlderCompletionRetainsItsOwnFileAndTimesAfterNewerOutput()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        var old = Completion("old.mp4", 10);
        var newer = Completion("new.mp4", 20);
        await model.ReviewAsync(old, 1, 123);
        await model.ReviewAsync(newer, 1, 123);
        await model.ReviewAsync(old, 1, 123);
        Assert.Equal(["load:old.mp4", "load:new.mp4", "load:old.mp4"], p.Commands.Where(c => c.StartsWith("load:")));
        Assert.Equal(["seek:7", "seek:17", "seek:7"], p.Commands.Where(c => c.StartsWith("seek:")));
    }

    [Fact]
    public async Task MissingFileDoesNotInitializeLoadOrSeek()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => false);
        await Assert.ThrowsAsync<FileNotFoundException>(() => model.ReviewAsync(Completion(), 1, 123));
        Assert.Empty(p.Commands);
        Assert.True(model.IsIdle);
    }

    [Fact]
    public async Task SingleKeptSegmentCanReviewBeginningAndEnd()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        var completion = new CutCompletionViewModel("whole.mp4", new CutPlan(TimeSpan.FromSeconds(50)));
        Assert.Empty(completion.Transitions);
        Assert.All(completion.CheckPoints, point => Assert.False(point.IsTransition));
        Assert.Contains("Anfang und Ende", completion.CheckInstructionText);
        await model.ReviewAsync(completion, 0, 123);
        Assert.Equal(TimeSpan.Zero, p.Position);
        Assert.False(model.CanPrevious);
        Assert.True(model.CanNext);
        await model.MoveAsync(1, 123);
        Assert.Equal(TimeSpan.FromSeconds(97), p.Position);
        Assert.True(model.CanPrevious);
        Assert.False(model.CanNext);
        await model.MoveAsync(-1, 123);
        Assert.Equal(TimeSpan.Zero, p.Position);
    }

    [Fact]
    public void CheckPointIdentityDoesNotDependOnLabelAndDoesNotChangeWithPlan()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(50));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(20)));
        var completion = new CutCompletionViewModel("cut.mp4", plan);
        plan.Remove(plan.RemoveSegments[0]);
        Assert.Single(completion.Transitions);
        Assert.Equal(0, completion.Transitions[0].TransitionIndex);
        Assert.Null(completion.CheckPoints[0].TransitionIndex);
        Assert.Null(completion.CheckPoints[^1].TransitionIndex);
        Assert.Equal(TimeSpan.FromSeconds(5), completion.Transitions[0].Position);
    }

    [Fact]
    public async Task DecodeErrorDetachesReadyEventsAndAllowsRetry()
    {
        var p = new Player { DeferredLoad = true };
        await using var model = new CutReviewViewModel(p, _ => true);
        var before = p.ReadySubscriberCount;
        var review = model.ReviewAsync(Completion(), 1, 123);
        p.Fail("Defekte Datei");
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => review);
        Assert.Contains("Defekte", error.Message);
        Assert.Equal(before, p.ReadySubscriberCount);
        Assert.DoesNotContain("play", p.Commands);
        p.DeferredLoad = false;
        await model.ReviewAsync(Completion(), 1, 123);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
    }

    [Fact]
    public async Task TimeoutDoesNotPlayAndReleasesEventHandlers()
    {
        var p = new Player { DeferredLoad = true };
        await using var model = new CutReviewViewModel(p, _ => true, TimeSpan.FromMilliseconds(20));
        var before = p.ReadySubscriberCount;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => model.ReviewAsync(Completion(), 1, 123));
        Assert.Contains("rechtzeitig", error.Message);
        Assert.Equal(before, p.ReadySubscriberCount);
        Assert.True(model.IsIdle);
        Assert.DoesNotContain("play", p.Commands);
    }

    [Theory]
    [InlineData("initialize")]
    [InlineData("load")]
    [InlineData("seek")]
    public async Task CloseCancelsPendingCommandsBeforeDisposing(string phase)
    {
        var p = new Player { BlockPhase = phase };
        var model = new CutReviewViewModel(p, _ => true);
        var review = model.ReviewAsync(Completion(), 1, 123);
        Assert.False(review.IsCompleted);
        await model.DisposeAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => review);
        Assert.Equal(1, p.DisposeCount);
        Assert.False(model.IsIdle);
        Assert.DoesNotContain("play", p.Commands);
        Assert.False(p.DisposedWhileCommandRunning);
        Assert.Equal(0, p.ReadySubscriberCount);
        await model.DisposeAsync();
        Assert.Equal(1, p.DisposeCount);
    }

    [Fact]
    public async Task CloseDuringFileLoadedWaitAndQueuedReviewReleasesEverything()
    {
        var p = new Player { DeferredLoad = true };
        var model = new CutReviewViewModel(p, _ => true);
        var first = model.ReviewAsync(Completion(), 1, 123);
        var queued = model.ReviewAsync(Completion("other.mp4"), 1, 123);
        await model.DisposeAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => first);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => queued);
        Assert.Equal(0, p.ReadySubscriberCount);
        Assert.Single(p.Commands, c => c.StartsWith("load:"));
    }

    [Fact]
    public async Task ReviewOperationsDoNotTouchAnotherPlayerAndCanContinuePastTransition()
    {
        var original = new Player { Position = TimeSpan.FromSeconds(88), State = MediaPlayerState.Playing };
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        await model.SeekAsync(TimeSpan.FromSeconds(90));
        await model.SetVolumeAsync(35);
        await model.TogglePlaybackAsync();
        Assert.Equal(TimeSpan.FromSeconds(90), p.Position);
        Assert.Equal(35, p.Volume);
        Assert.Empty(original.Commands);
        Assert.Equal(TimeSpan.FromSeconds(88), original.Position);
        Assert.Equal(MediaPlayerState.Playing, original.State);
        Assert.DoesNotContain("stop", p.Commands);
    }

    [Fact]
    public async Task ClosedSessionCannotIssueNewCommandsEvenAfterTokenSourceDisposal()
    {
        var p = new Player();
        var model = new CutReviewViewModel(p, _ => true);
        await model.DisposeAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => model.ReviewAsync(Completion(), 1, 123));
        Assert.Empty(p.Commands);
    }

    [Fact]
    public async Task TransitionAfterNaturalEndReloadsFileBeforeSeekAndPlay()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        var completion = Completion();
        await model.ReviewAsync(completion, 2, 123);
        p.State = MediaPlayerState.Ended;
        await model.MoveAsync(-1, 123);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(["load:cut.mp4", "seek:7", "play"], p.Commands.TakeLast(3));
    }

    [Fact]
    public async Task ManualSeekAfterNaturalEndReloadsFileAndKeepsPlaybackPaused()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        p.State = MediaPlayerState.Ended;
        await model.SeekAsync(TimeSpan.FromSeconds(50));
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(TimeSpan.FromSeconds(50), p.Position);
        Assert.Equal(MediaPlayerState.Paused, p.State);
    }

    [Fact]
    public async Task NewCompletionWithSameFileNameReloadsItsOutput()
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion("cut.mp4", 10), 1, 123);
        await model.ReviewAsync(Completion("cut.mp4", 20), 1, 123);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(["load:cut.mp4", "seek:17", "play"], p.Commands.TakeLast(3));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task BeginningAndEndWorkWithOrWithoutInnerTransitions(bool multipleKeeps, bool end)
    {
        var p = new Player { ActualDuration = TimeSpan.FromSeconds(70) };
        await using var model = new CutReviewViewModel(p, _ => true);
        var completion = multipleKeeps ? Completion() :
            new CutCompletionViewModel("whole.mp4", new CutPlan(TimeSpan.FromSeconds(40)));
        var nominal = completion.CheckPoints[^1].Position;
        var pointIndex = end ? completion.CheckPoints.Count - 1 : 0;
        await model.ReviewAsync(completion, pointIndex, 123);
        Assert.Equal(TimeSpan.FromSeconds(end ? 67 : 0), p.Position);
        Assert.Equal(["initialize:123", $"load:{completion.OutputFilePath}",
            end ? "seek:67" : "seek:0", "play"], p.Commands);
        Assert.Equal(end ? "Ende · 00:01:10.000" : "Anfang · 00:00:00.000", model.CurrentTransitionText);
        Assert.Contains(end ? "tatsächlichen Filmende" : "Filmanfang", model.StatusText);
        Assert.Equal(nominal, completion.CheckPoints[^1].Position);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2.5, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 1)]
    [InlineData(9.25, 6.25)]
    public async Task EndReviewUsesActualDurationAndClampsVeryShortFilms(double seconds, double expected)
    {
        var p = new Player { ActualDuration = TimeSpan.FromSeconds(seconds) };
        await using var model = new CutReviewViewModel(p, _ => true);
        var completion = new CutCompletionViewModel("short.mp4", new CutPlan(TimeSpan.FromSeconds(200)));
        await model.ReviewAsync(completion, 1, 123);
        Assert.Equal(TimeSpan.FromSeconds(expected), p.Position);
        Assert.Equal(MediaPlayerState.Playing, p.State);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public async Task InvalidCheckPointDoesNotInitializePlayer(int index)
    {
        var p = new Player();
        await using var model = new CutReviewViewModel(p, _ => true);
        var completion = new CutCompletionViewModel("whole.mp4", new CutPlan(TimeSpan.FromSeconds(50)));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => model.ReviewAsync(completion, index, 123));
        Assert.Empty(p.Commands);
    }

    [Fact]
    public void CheckPointKindsAndIndicesIncludeStartTransitionsAndEnd()
    {
        var completion = Completion();
        Assert.Equal([0, 1, 2, 3], completion.CheckPoints.Select(p => p.CheckPointIndex));
        Assert.Equal([CutCompletionCheckPointKind.Beginning, CutCompletionCheckPointKind.Transition,
            CutCompletionCheckPointKind.Transition, CutCompletionCheckPointKind.End],
            completion.CheckPoints.Select(p => p.Kind));
        Assert.Equal([1, 2], completion.Transitions.Select(p => p.CheckPointIndex));
        Assert.Equal([0, 1], completion.Transitions.Select(p => p.TransitionIndex!.Value));
    }

    [Fact]
    public async Task MeasuredPositionIsUsedAndChangedOutputIsRejectedBeforeLoad()
    {
        var path = Path.Combine(Path.GetTempPath(), "can-measured-review-" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            await File.WriteAllTextAsync(path, "fixture");
            var identity = await CutAssistantNext.Media.Analysis.CutOutputIdentityVerifier.ReadAsync(path);
            var result = new CutAssistantNext.Core.Cutting.CutResult(path,
                new(TimeSpan.FromSeconds(100), [TimeSpan.FromSeconds(10.25)], identity),
                CutAssistantNext.Core.Cutting.CutReviewTimingKind.Measured);
            var completion = new CutCompletionViewModel(result);
            var player = new Player();
            await using (var model = new CutReviewViewModel(player))
            {
                await model.ReviewAsync(completion, 1, 123);
                Assert.Equal(TimeSpan.FromSeconds(7.25), player.Position);
                Assert.Contains("geprüften", model.StatusText);
            }
            var time = File.GetLastWriteTimeUtc(path);
            await File.WriteAllTextAsync(path, "changed");
            File.SetLastWriteTimeUtc(path, time);
            var other = new Player();
            await using var rejected = new CutReviewViewModel(other);
            await Assert.ThrowsAsync<IOException>(() => rejected.ReviewAsync(completion, 1, 123));
            Assert.DoesNotContain(other.Commands, c => c.StartsWith("load:") || c == "play");
        }
        finally { File.Delete(path); }
    }

    private sealed class Player : IMediaPlayerService
    {
        internal readonly List<string> Commands = [];
        internal bool DeferredLoad { get; set; }
        internal string? BlockPhase { get; init; }
        private bool _running;
        internal bool DisposedWhileCommandRunning { get; private set; }
        internal int DisposeCount { get; private set; }
        internal TimeSpan ActualDuration { get; init; } = TimeSpan.FromSeconds(100);
        public MediaPlayerState State { get; set; } = MediaPlayerState.Empty;
        public TimeSpan Position { get; set; }
        public TimeSpan? Duration { get; private set; }
        public long? FrameNumber => null;
        public long? EstimatedFrameCount => null;
        public double Volume { get; private set; } = 100;
        public string? ErrorMessage { get; private set; }
        public event EventHandler? StateChanged;
        public event EventHandler? PositionChanged;
        public event EventHandler? DurationChanged;
        public event EventHandler? FrameChanged { add { } remove { } }
        public event EventHandler? VolumeChanged;
        public event EventHandler? ErrorOccurred;
        internal int ReadySubscriberCount => (StateChanged?.GetInvocationList().Length ?? 0) +
            (DurationChanged?.GetInvocationList().Length ?? 0) + (ErrorOccurred?.GetInvocationList().Length ?? 0);
        private async Task Block(string phase, CancellationToken token)
        {
            if (BlockPhase != phase) return;
            _running = true;
            try { await Task.Delay(Timeout.Infinite, token); }
            finally { _running = false; }
        }
        public async Task InitializeAsync(nint h, CancellationToken t = default) { Commands.Add($"initialize:{h}"); await Block("initialize", t); }
        public async Task LoadAsync(string p, CancellationToken t = default)
        {
            Commands.Add("load:" + p); State = MediaPlayerState.Loading; Duration = null; ErrorMessage = null;
            StateChanged?.Invoke(this, EventArgs.Empty); await Block("load", t);
            if (!DeferredLoad) { ReadyDuration(); ReadyState(); }
        }
        internal void ReadyDuration() { Duration = ActualDuration; DurationChanged?.Invoke(this, EventArgs.Empty); }
        internal void ReadyState() { State = MediaPlayerState.Paused; StateChanged?.Invoke(this, EventArgs.Empty); }
        internal void Fail(string message) { State = MediaPlayerState.Error; ErrorMessage = message; ErrorOccurred?.Invoke(this, EventArgs.Empty); }
        public async Task SeekAsync(TimeSpan p, CancellationToken t = default)
        {
            Commands.Add("seek:" + p.TotalSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture));
            await Block("seek", t); Position = p; PositionChanged?.Invoke(this, EventArgs.Empty);
        }
        public Task PlayAsync(CancellationToken t = default) { Commands.Add("play"); State = MediaPlayerState.Playing; StateChanged?.Invoke(this, EventArgs.Empty); return Task.CompletedTask; }
        public Task PauseAsync(CancellationToken t = default) { Commands.Add("pause"); State = MediaPlayerState.Paused; StateChanged?.Invoke(this, EventArgs.Empty); return Task.CompletedTask; }
        public Task SetVolumeAsync(double v, CancellationToken t = default) { Volume = v; VolumeChanged?.Invoke(this, EventArgs.Empty); return Task.CompletedTask; }
        public Task StopAsync(CancellationToken t = default) { Commands.Add("stop"); return Task.CompletedTask; }
        public Task StepForwardAsync(CancellationToken t = default) => Task.CompletedTask;
        public Task StepFramesAsync(int f, CancellationToken t = default) => Task.CompletedTask;
        public Task StepBackwardAsync(CancellationToken t = default) => Task.CompletedTask;
        public ValueTask DisposeAsync() { DisposeCount++; DisposedWhileCommandRunning = _running; return ValueTask.CompletedTask; }
    }
}
