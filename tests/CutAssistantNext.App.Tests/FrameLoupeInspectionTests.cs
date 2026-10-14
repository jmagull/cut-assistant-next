using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class FrameLoupeInspectionTests
{
    [Theory]
    [InlineData("empty")]
    [InlineData("unselected")]
    [InlineData("row")]
    [InlineData("pending")]
    public async Task InspectionUsesReachedPositionAndNeverChangesTheCutPlan(string kind)
    {
        var plan = MakePlan();
        if (kind is "unselected" or "row")
        {
            plan.SetStart(TimeSpan.FromSeconds(2));
            plan.SetEnd(TimeSpan.FromSeconds(4));
            plan.SelectedRemoveSegment = kind == "row" ? plan.RemoveSegments[0] : null;
        }
        if (kind == "pending") plan.SetStart(TimeSpan.FromSeconds(3));
        var segments = plan.RemoveSegments.ToArray();
        var selected = plan.SelectedRemoveSegment;
        var pending = plan.PendingStart;
        using var playback = new PlaybackViewModel(new Player { Position = TimeSpan.FromSeconds(6) });
        playback.BeginSeek();
        playback.UpdateSeekPosition(10);
        Assert.Equal(10, playback.TimelinePositionSeconds);

        for (int opening = 0; opening < 3; opening++)
        {
            var target = FrameLoupeTarget.Capture(plan, playback);
            Assert.Equal(TimeSpan.FromSeconds(6), target.Position);
            Assert.Null(target.Side);
            Assert.Null(target.Segment);
            var vm = Make(target, new Analysis(), new Preview());
            Assert.True(vm.IsInspectionMode);
            await vm.InitializeAsync();
            Assert.Equal(TimeSpan.FromSeconds(6), vm.SelectedCutPosition);
            Assert.NotNull(vm.PreviewPngBytes);
            await vm.StepAsync(1);
            await vm.SearchAsync(-1);
            await vm.SearchAsync(1);
            vm.ResetSearch();
            await vm.SelectFrameAsync(vm.Frames[0]);
            Assert.False(vm.CanApply);
            Assert.Throws<InvalidOperationException>(() => vm.CreateSelection());
            Assert.Contains("Untersuchungsmodus", vm.Title);
            Assert.Contains("Playerposition", vm.CutPositionText);
            Assert.Contains("Untersuchter Frame", vm.SelectedCutPositionText);
            Assert.Contains("Keine Kantenübernahme", vm.Status);
            Assert.Equal(segments, plan.RemoveSegments);
            Assert.Same(selected, plan.SelectedRemoveSegment);
            Assert.Equal(pending, plan.PendingStart);
            Assert.Null(plan.SelectedEdgeSide);
            Assert.Equal(TimeSpan.FromSeconds(12), plan.MediaDuration);
        }
    }

    [Theory]
    [InlineData(CutEdgeSide.Start, 2, 2.04, 4)]
    [InlineData(CutEdgeSide.End, 4, 2, 4.04)]
    public async Task SelectedEdgeRetainsItsPositionAndVerifiedApply(CutEdgeSide side, double position, double start, double end)
    {
        var plan = MakePlan();
        plan.SetStart(TimeSpan.FromSeconds(2));
        plan.SetEnd(TimeSpan.FromSeconds(4));
        var segment = plan.RemoveSegments[0];
        plan.SelectEdge(segment, side);
        using var playback = new PlaybackViewModel(new Player { Position = TimeSpan.FromSeconds(8) });
        var target = FrameLoupeTarget.Capture(plan, playback);
        Assert.Equal(TimeSpan.FromSeconds(position), target.Position);
        Assert.Equal(side, target.Side);
        Assert.Same(segment, target.Segment);
        var vm = Make(target, new Analysis(), new Preview());
        await vm.InitializeAsync();
        await vm.StepAsync(1);
        Assert.False(vm.IsInspectionMode);
        Assert.True(vm.CanApply);
        plan.ApplyFrameEdge(target.Segment!, target.Side!.Value, vm.CreateSelection().Position);
        Assert.Equal(TimeSpan.FromSeconds(start), plan.RemoveSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(end), plan.RemoveSegments[0].End);
    }

    [Theory]
    [InlineData(0, 12, 0)]
    [InlineData(11.99, 12, 11.96)]
    [InlineData(12, 12, 11.96)]
    [InlineData(0, 0.12, 0)]
    [InlineData(0.12, 0.12, 0.08)]
    [InlineData(1.021, 12, 1.04)]
    public async Task InspectionClampsAnalysisAtFileBoundariesAndFindsNearestFrame(double position, double duration, double expected)
    {
        var plan = MakePlan(duration);
        using var playback = new PlaybackViewModel(new Player { Position = TimeSpan.FromSeconds(position), Duration = TimeSpan.FromSeconds(duration) });
        var analysis = new Analysis { Duration = duration };
        var vm = Make(FrameLoupeTarget.Capture(plan, playback), analysis, new Preview());
        await vm.InitializeAsync();
        Assert.Equal(TimeSpan.FromSeconds(expected), vm.SelectedCutPosition);
        Assert.NotNull(vm.PreviewPngBytes);
        Assert.False(vm.CanApply);
        Assert.All(analysis.Requests, request =>
        {
            Assert.InRange(request.Start, TimeSpan.Zero, TimeSpan.FromSeconds(duration));
            Assert.InRange(request.End, request.Start, TimeSpan.FromSeconds(duration));
        });
        await vm.StepAsync(position == 0 ? -1 : 1);
        Assert.NotNull(vm.PreviewPngBytes);
        Assert.False(vm.CanApply);
        Assert.Empty(plan.RemoveSegments);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(12.001)]
    public void InvalidPlayerPositionIsRejectedWithoutCreatingACut(double position)
    {
        var plan = MakePlan();
        using var playback = new PlaybackViewModel(new Player { Position = TimeSpan.FromSeconds(position) });
        Assert.Throws<InvalidOperationException>(() => FrameLoupeTarget.Capture(plan, playback));
        Assert.Empty(plan.RemoveSegments);
    }

    [Theory]
    [InlineData(MediaPlayerState.Empty)]
    [InlineData(MediaPlayerState.Loading)]
    [InlineData(MediaPlayerState.Error)]
    [InlineData((MediaPlayerState)99)]
    public void UnavailablePlayerDoesNotOpenInspection(MediaPlayerState state)
    {
        using var playback = new PlaybackViewModel(new Player { State = state });
        Assert.Throws<InvalidOperationException>(() => FrameLoupeTarget.Capture(MakePlan(), playback));
    }

    [Fact]
    public void MissingMediaDurationDoesNotOpenInspection()
    {
        using var playback = new PlaybackViewModel(new Player());
        Assert.Throws<InvalidOperationException>(() => FrameLoupeTarget.Capture(new CutPlanViewModel(), playback));
    }

    [Theory]
    [InlineData("avi", 0)]
    [InlineData("avi", 2)]
    [InlineData("avi", -2)]
    [InlineData("mp4", 0)]
    public async Task InspectionKeepsTimestampProvenanceAndContainerStart(string format, int origin)
    {
        var preview = new Preview();
        var vm = new FrameLoupeViewModel("video.avi", 0, TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(6),
            null, origin, 10, new Analysis { MissingPts = true, Origin = origin }, preview, format);
        await vm.InitializeAsync();
        Assert.False(vm.CanApply);
        Assert.Throws<InvalidOperationException>(() => vm.CreateSelection());
        if (format == "avi")
        {
            Assert.Equal(TimeSpan.FromSeconds(6), vm.SelectedCutPosition);
            Assert.Equal((6 + origin) * 1000, Assert.Single(preview.Requests));
            Assert.Contains("rekonstruierter", vm.Status);
            Assert.Contains("keine Original-PTS", vm.DetailsText);
            await vm.StepAsync(1);
            Assert.Equal(TimeSpan.FromSeconds(6.04), vm.SelectedCutPosition);
        }
        else
        {
            Assert.Empty(preview.Requests);
            Assert.Null(vm.PreviewPngBytes);
            Assert.Contains("Original-PTS fehlt", vm.Status);
        }
    }

    [Theory]
    [InlineData("analysis")]
    [InlineData("empty")]
    [InlineData("stream")]
    [InlineData("duplicate")]
    [InlineData("missing")]
    [InlineData("preview")]
    [InlineData("wrong-pts")]
    [InlineData("wrong-timebase")]
    [InlineData("empty-image")]
    public async Task FailedOrUnsafeInspectionNeverClaimsConfirmedImage(string failure)
    {
        var vm = Make(new FrameLoupeTarget(TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(6), null, null),
            new Analysis { Failure = failure }, new Preview { Failure = failure });
        await vm.InitializeAsync();
        Assert.False(vm.IsBusy);
        Assert.False(vm.CanApply);
        Assert.Null(vm.PreviewPngBytes);
        Assert.DoesNotContain("stimmen überein", vm.Status);
        Assert.DoesNotContain("Untersuchter Frame:", vm.SelectedCutPositionText);
        Assert.Throws<InvalidOperationException>(() => vm.CreateSelection());
    }

    [Fact]
    public async Task CancelledInspectionClearsBusyStateAndCannotApply()
    {
        using var lifetime = new CancellationTokenSource();
        lifetime.Cancel();
        var vm = Make(new FrameLoupeTarget(TimeSpan.FromSeconds(12), TimeSpan.Zero, null, null), new Analysis(), new Preview());
        await vm.InitializeAsync(lifetime.Token);
        Assert.False(vm.IsBusy);
        Assert.Null(vm.PreviewPngBytes);
        Assert.False(vm.CanApply);
        Assert.Equal("Vorgang abgebrochen.", vm.Status);
    }

    [Fact]
    public async Task InspectionCannotApplyWhileOrAfterLoadingAnotherPreview()
    {
        var preview = new Preview();
        var vm = Make(new FrameLoupeTarget(TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(6), null, null), new Analysis(), preview);
        await vm.InitializeAsync();
        preview.Pending = new TaskCompletionSource<FramePreview>();
        var move = vm.StepAsync(1);
        Assert.True(vm.IsBusy);
        Assert.False(vm.CanApply);
        preview.Pending.SetResult(new FramePreview([137, 80, 78, 71], 6040, new VideoTimeBase(1, 1000)));
        await move;
        Assert.False(vm.IsBusy);
        Assert.NotNull(vm.PreviewPngBytes);
        Assert.False(vm.CanApply);
        vm.ReportImageError(new InvalidOperationException("invalid png"));
        Assert.Null(vm.PreviewPngBytes);
        Assert.False(vm.CanApply);
    }

    private static CutPlanViewModel MakePlan(double duration = 12)
    {
        var plan = new CutPlanViewModel();
        plan.Initialize(TimeSpan.FromSeconds(duration));
        return plan;
    }

    private static FrameLoupeViewModel Make(FrameLoupeTarget target, Analysis analysis, Preview preview) =>
        new("video.mp4", 0, target.MediaDuration, target.Position, target.Side, 0, 10, analysis, preview);

    private sealed class Analysis : IFrameAnalysisRunner
    {
        internal double Duration { get; init; } = 12;
        internal int Origin { get; init; }
        internal bool MissingPts { get; init; }
        internal string? Failure { get; init; }
        internal List<(TimeSpan Start, TimeSpan End)> Requests { get; } = [];
        public Task<VideoFrameWindow> RunAsync(string path, int streamIndex, TimeSpan start, TimeSpan end, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add((start, end));
            if (Failure == "analysis") throw new InvalidOperationException("analysis failed");
            var frames = Enumerable.Range(0, (int)Math.Round(Duration * 25))
                .Select(index => Origin * 1000L + index * 40L)
                .Where(pts => pts >= start.TotalMilliseconds && pts < end.TotalMilliseconds)
                .Select((pts, index) => new VideoFrameInfo(index, MissingPts ? null : pts, pts, null, 40, index % 25 == 0, "I"))
                .ToList();
            if (Failure == "empty") frames.Clear();
            if (Failure == "duplicate" && frames.Count > 1) frames[1] = frames[0] with { LocalIndex = 1 };
            if (Failure == "missing") frames = frames.Select(frame => frame with { Pts = null, BestEffortTimestamp = null }).ToList();
            return Task.FromResult(new VideoFrameWindow(Failure == "stream" ? streamIndex + 1 : streamIndex,
                new VideoTimeBase(1, 1000), Origin, Origin, frames));
        }
    }

    private sealed class Preview : IFramePreviewRunner
    {
        internal string? Failure { get; init; }
        internal List<long> Requests { get; } = [];
        internal TaskCompletionSource<FramePreview>? Pending { get; set; }
        public Task<FramePreview> RunAsync(string path, int streamIndex, VideoTimeBase timeBase, long pts, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add(pts);
            if (Failure == "preview") throw new InvalidOperationException("preview failed");
            return Pending?.Task ?? Task.FromResult(new FramePreview(Failure == "empty-image" ? [] : [137, 80, 78, 71],
                Failure == "wrong-pts" ? pts + 1 : pts, Failure == "wrong-timebase" ? new VideoTimeBase(1, 90000) : timeBase));
        }
    }

    private sealed class Player : IMediaPlayerService
    {
        public MediaPlayerState State { get; set; } = MediaPlayerState.Paused;
        public TimeSpan Position { get; set; }
        public TimeSpan? Duration { get; set; } = TimeSpan.FromSeconds(12);
        public long? FrameNumber => null;
        public long? EstimatedFrameCount => null;
        public double Volume => 0;
        public string? ErrorMessage => null;
        public event EventHandler? StateChanged { add { } remove { } }
        public event EventHandler? PositionChanged { add { } remove { } }
        public event EventHandler? DurationChanged { add { } remove { } }
        public event EventHandler? FrameChanged { add { } remove { } }
        public event EventHandler? VolumeChanged { add { } remove { } }
        public event EventHandler? ErrorOccurred { add { } remove { } }
        public Task InitializeAsync(nint handle, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(string path, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PlayAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PauseAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StepForwardAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StepFramesAsync(int count, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StepBackwardAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SetVolumeAsync(double volume, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
