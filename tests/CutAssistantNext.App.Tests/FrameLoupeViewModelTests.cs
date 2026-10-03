using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Tests;

public sealed class FrameLoupeViewModelTests
{
    [Fact]
    public async Task SearchExpandsIntervalAndJumps2000RealFramesThenHalves()
    {
        var analysis = new FakeAnalysis();
        var preview = new FakePreview();
        var viewModel = Create(analysis, preview);
        await viewModel.InitializeAsync();
        var initialPts = viewModel.SelectedFrame!.Frame.Pts!.Value;
        await viewModel.SearchAsync(-1);

        Assert.Equal(initialPts - 2000 * 40, viewModel.SelectedFrame!.Frame.Pts);
        Assert.Equal(1000, viewModel.NextSearchStep);
        Assert.True(analysis.Requests.Count > 1);
        Assert.NotNull(viewModel.PreviewPngBytes);

        await viewModel.SearchAsync(1);
        Assert.Equal(initialPts - 1000 * 40, viewModel.SelectedFrame!.Frame.Pts);
        Assert.Equal(500, viewModel.NextSearchStep);
    }

    [Fact]
    public async Task NormalOneTenAndTwentyStepsDoNotConsumeSearchSequence()
    {
        var viewModel = Create(new FakeAnalysis(), new FakePreview());
        await viewModel.InitializeAsync();
        var pts = viewModel.SelectedFrame!.Frame.Pts;
        await viewModel.StepAsync(1);
        await viewModel.StepAsync(10);
        await viewModel.StepAsync(-20);
        Assert.Equal(pts - 9 * 40, viewModel.SelectedFrame!.Frame.Pts);
        Assert.Equal(2000, viewModel.NextSearchStep);
    }

    [Fact]
    public async Task ResetRestoresConfiguredStartWithoutMovingFrame()
    {
        var viewModel = Create(new FakeAnalysis(), new FakePreview(), start: 125);
        await viewModel.InitializeAsync();
        await viewModel.SearchAsync(1);
        Assert.Equal(62, viewModel.NextSearchStep);
        var selected = viewModel.SelectedFrame;
        viewModel.ResetSearch();
        Assert.Equal(125, viewModel.NextSearchStep);
        Assert.Same(selected, viewModel.SelectedFrame);
    }

    [Fact]
    public async Task FailedPreviewDoesNotHalveAndClearsStaleImage()
    {
        var preview = new FakePreview();
        var viewModel = Create(new FakeAnalysis(), preview, start: 10);
        await viewModel.InitializeAsync();
        preview.Fail = true;
        await viewModel.SearchAsync(-1);
        Assert.Equal(10, viewModel.NextSearchStep);
        Assert.Null(viewModel.PreviewPngBytes);
        Assert.Contains("preview failed", viewModel.Status);
        Assert.Contains("preview failed", viewModel.DetailsText);
    }

    [Fact]
    public async Task FileBoundaryClampsActualJumpAndNoMovementKeepsSearchWidth()
    {
        var viewModel = Create(new FakeAnalysis(), new FakePreview());
        await viewModel.InitializeAsync();
        await viewModel.SearchAsync(1);
        Assert.Equal(3999 * 40, viewModel.SelectedFrame!.Frame.Pts);
        Assert.Equal(1000, viewModel.NextSearchStep);
        await viewModel.SearchAsync(1);
        Assert.Equal(1000, viewModel.NextSearchStep);
        Assert.Contains("Dateigrenze", viewModel.Status);
    }

    [Fact]
    public async Task VariableTimestampsStillNavigateByDecodedFrameCount()
    {
        var analysis = new FakeAnalysis { VariableTimes = true };
        var viewModel = Create(analysis, new FakePreview(), start: 125);
        await viewModel.InitializeAsync();
        var before = viewModel.SelectedFrame!.Frame.Pts!.Value;
        var sourceIndex = analysis.AllFrames.FindIndex(frame => frame.Pts == before);
        await viewModel.SearchAsync(-1);
        Assert.Equal(analysis.AllFrames[sourceIndex - 125].Pts, viewModel.SelectedFrame!.Frame.Pts);
        Assert.Equal(62, viewModel.NextSearchStep);
    }

    [Fact]
    public async Task NonZeroContainerStartMapsInitialCutToRawTimestamp()
    {
        var analysis = new FakeAnalysis { Origin = 2000 };
        var viewModel = Create(analysis, new FakePreview(), origin: 2);
        await viewModel.InitializeAsync();
        Assert.Equal(102000, viewModel.SelectedFrame!.Frame.Pts);
        Assert.Equal(TimeSpan.FromSeconds(100), analysis.Requests[0].Start);
    }

    [Fact]
    public async Task DuplicateTimestampsAreRejectedWithoutPreview()
    {
        var analysis = new FakeAnalysis { DuplicateTimes = true };
        var preview = new FakePreview();
        var viewModel = Create(analysis, preview);
        await viewModel.InitializeAsync();
        Assert.Null(viewModel.PreviewPngBytes);
        Assert.Empty(preview.Requests);
        Assert.Contains("mehrdeutig", viewModel.Status);
    }

    [Fact]
    public async Task MissingOriginalPtsDoesNotClaimVerifiedBestEffortImage()
    {
        var analysis = new FakeAnalysis { MissingPts = true };
        var preview = new FakePreview();
        var viewModel = Create(analysis, preview);
        await viewModel.InitializeAsync();
        Assert.NotNull(viewModel.SelectedFrame);
        Assert.Null(viewModel.PreviewPngBytes);
        Assert.Empty(preview.Requests);
        Assert.Contains("Original-PTS fehlt", viewModel.Status);
    }

    [Fact]
    public async Task CancelledAnalysisReleasesBusyState()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var viewModel = Create(new FakeAnalysis(), new FakePreview());
        await viewModel.InitializeAsync(cancellation.Token);
        Assert.False(viewModel.IsBusy);
        Assert.Null(viewModel.PreviewPngBytes);
        Assert.Equal("Vorgang abgebrochen.", viewModel.Status);
    }

    [Fact]
    public async Task PreviewForWrongFrameIsRejected()
    {
        var viewModel = Create(new FakeAnalysis(), new FakePreview { WrongPts = true });
        await viewModel.InitializeAsync();
        Assert.Null(viewModel.PreviewPngBytes);
        Assert.Contains("nicht zum gewählten", viewModel.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(-2)]
    public async Task SelectionUsesVerifiedOriginalPtsAndNormalizesContainerStart(int origin)
    {
        var viewModel = Create(new FakeAnalysis { Origin = origin * 1000 }, new FakePreview(), origin: origin);
        Assert.False(viewModel.CanApply);
        Assert.Throws<InvalidOperationException>(() => viewModel.CreateSelection());
        await viewModel.InitializeAsync();
        await viewModel.StepAsync(1);
        var selection = viewModel.CreateSelection();

        Assert.True(viewModel.CanApply);
        Assert.Equal(TimeSpan.FromSeconds(100.04), selection.Position);
        Assert.Equal(100040 + origin * 1000, selection.Pts);
        Assert.Equal(new VideoTimeBase(1, 1000), selection.TimeBase);
        Assert.Equal(1, selection.StreamIndex);
        Assert.False(viewModel.SelectedFrame!.Frame.IsKeyFrame);
        Assert.Contains("00:01:40.040", viewModel.SelectedCutPositionText);
    }

    [Fact]
    public async Task SelectionUsesVariableTimestampWithoutRoundingToFps()
    {
        var viewModel = Create(new FakeAnalysis { VariableTimes = true }, new FakePreview());
        await viewModel.InitializeAsync();
        await viewModel.StepAsync(1);
        var selection = viewModel.CreateSelection();
        Assert.Equal(TimeSpan.FromMilliseconds(selection.Pts), selection.Position);
    }

    [Theory]
    [InlineData("failed")]
    [InlineData("wrong")]
    [InlineData("missing")]
    [InlineData("empty")]
    [InlineData("image-error")]
    public async Task UnconfirmedPreviewCannotBeApplied(string reason)
    {
        var viewModel = Create(new FakeAnalysis { MissingPts = reason == "missing" },
            new FakePreview { Fail = reason == "failed", WrongPts = reason == "wrong", Empty = reason == "empty" });
        await viewModel.InitializeAsync();
        if (reason == "image-error")
        {
            Assert.True(viewModel.CanApply);
            viewModel.ReportImageError(new InvalidOperationException("invalid image"));
        }

        Assert.False(viewModel.CanApply);
        Assert.Throws<InvalidOperationException>(() => viewModel.CreateSelection());
    }

    [Fact]
    public async Task LoadingAnotherFrameDisablesApplyAndNotifiesTheButton()
    {
        var preview = new FakePreview();
        var viewModel = Create(new FakeAnalysis(), preview);
        await viewModel.InitializeAsync();
        var changes = new List<string?>();
        viewModel.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
        preview.Pending = new TaskCompletionSource<FramePreview>();
        var move = viewModel.StepAsync(1);
        Assert.True(viewModel.IsBusy);
        Assert.False(viewModel.CanApply);
        Assert.Throws<InvalidOperationException>(() => viewModel.CreateSelection());
        preview.Pending.SetResult(new FramePreview([137, 80, 78, 71], viewModel.SelectedFrame!.Frame.Pts!.Value,
            new VideoTimeBase(1, 1000)));
        await move;
        Assert.True(viewModel.CanApply);
        Assert.Contains(nameof(FrameLoupeViewModel.CanApply), changes);
        Assert.Equal(TimeSpan.FromSeconds(100.04), viewModel.CreateSelection().Position);
    }

    private static FrameLoupeViewModel Create(FakeAnalysis analysis, FakePreview preview, int start = 2000, decimal origin = 0) =>
        new("video.mp4", 1, TimeSpan.FromSeconds(160), TimeSpan.FromSeconds(100),
            CutEdgeSide.End, origin, start, analysis, preview);

    private sealed class FakeAnalysis : IFrameAnalysisRunner
    {
        internal List<(TimeSpan Start, TimeSpan End)> Requests { get; } = [];
        internal bool VariableTimes { get; init; }
        internal bool DuplicateTimes { get; init; }
        internal bool MissingPts { get; init; }
        internal long Origin { get; init; }
        internal List<VideoFrameInfo> AllFrames => Enumerable.Range(0, 4000)
            .Select(index => MakeFrame(index, Origin + index * 40L + (VariableTimes ? index / 3 * 20L : 0))).ToList();

        public Task<VideoFrameWindow> RunAsync(string path, int streamIndex, TimeSpan start,
            TimeSpan end, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add((start, end));
            var frames = AllFrames.Where(frame => frame.BestEffortTimestamp >= (long)start.TotalMilliseconds &&
                frame.BestEffortTimestamp < (long)end.TotalMilliseconds)
                .Select((frame, index) => frame with { LocalIndex = index }).ToList();
            if (DuplicateTimes && frames.Count > 1)
            {
                frames[1] = frames[0] with { LocalIndex = 1 };
            }

            return Task.FromResult(new VideoFrameWindow(streamIndex, new VideoTimeBase(1, 1000),
                Origin / 1000m, Origin / 1000m, frames));
        }

        private VideoFrameInfo MakeFrame(int index, long timestamp) => new(index,
            MissingPts ? null : timestamp, timestamp, null, 40, index % 25 == 0, index % 25 == 0 ? "I" : "B");
    }

    private sealed class FakePreview : IFramePreviewRunner
    {
        internal bool Fail { get; set; }
        internal bool WrongPts { get; init; }
        internal bool Empty { get; init; }
        internal TaskCompletionSource<FramePreview>? Pending { get; set; }
        internal List<long> Requests { get; } = [];

        public Task<FramePreview> RunAsync(string path, int stream, VideoTimeBase timeBase,
            long pts, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add(pts);
            if (Fail)
            {
                throw new InvalidOperationException("preview failed");
            }

            return Pending?.Task ?? Task.FromResult(new FramePreview(Empty ? [] : [137, 80, 78, 71], WrongPts ? pts + 1 : pts, timeBase));
        }
    }
}
