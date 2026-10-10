using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class Mp4BoxProgressViewModelTests
{
    [Fact]
    public void FailureReasonRemainsVisibleAfterQueuedProgress()
    {
        var viewModel = new Mp4BoxProgressViewModel();
        viewModel.MarkRunning();
        viewModel.MarkFailed("FFmpeg wurde nicht gefunden.");
        var failure = viewModel.StatusText;
        viewModel.ApplyProgress(new CutProgressUpdate(CutProgressKind.Status, "Alte Fortschrittsmeldung"));

        Assert.Contains("FFmpeg wurde nicht gefunden.", failure);
        Assert.Equal(failure, viewModel.StatusText);
        Assert.Contains(failure, viewModel.ProtocolText);
        Assert.Contains("Alte Fortschrittsmeldung", viewModel.ProtocolText);
        Assert.False(viewModel.CanCancel);
        Assert.False(viewModel.ShouldAutoClose);

        viewModel.MarkRunning();
        viewModel.ApplyProgress(new CutProgressUpdate(CutProgressKind.Status, "Neuer Versuch"));
        Assert.Equal("Neuer Versuch", viewModel.StatusText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void MissingFailureReasonShowsProtocolHint(string? reason)
    {
        var viewModel = new Mp4BoxProgressViewModel();
        viewModel.MarkFailed(reason);
        Assert.Contains("fehlgeschlagen", viewModel.StatusText);
        Assert.Contains("Protokoll", viewModel.StatusText);
    }

    [Fact]
    public void ApplyProgress_StatusAndOutput_UpdateDisplayAndProtocol()
    {
        var viewModel =
            new Mp4BoxProgressViewModel();

        viewModel.ApplyProgress(
            new CutProgressUpdate(
                CutProgressKind.Status,
                "Segment 1 von 3 wird geschnitten …"));

        viewModel.ApplyProgress(
            new CutProgressUpdate(
                CutProgressKind.Output,
                "> MP4Box.exe source.mp4 -splitx 10:20"));

        viewModel.ApplyProgress(
            new CutProgressUpdate(
                CutProgressKind.Output,
                "splitting: file 1 done"));

        Assert.Equal(
            "Segment 1 von 3 wird geschnitten …",
            viewModel.StatusText);

        Assert.Equal(
            "Segment 1 von 3 wird geschnitten …" +
            Environment.NewLine +
            "> MP4Box.exe source.mp4 -splitx 10:20" +
            Environment.NewLine +
            "splitting: file 1 done",
            viewModel.ProtocolText);
    }

    [Fact]
    public void RunningAndSucceeded_UpdateLifecycleState()
    {
        var viewModel =
            new Mp4BoxProgressViewModel();

        Assert.False(
            viewModel.CanCancel);

        Assert.False(
            viewModel.ShouldAutoClose);

        viewModel.MarkRunning();

        Assert.True(
            viewModel.CanCancel);

        Assert.False(
            viewModel.ShouldAutoClose);

        viewModel.MarkSucceeded();

        Assert.False(
            viewModel.CanCancel);

        Assert.True(
            viewModel.ShouldAutoClose);
    }

    [Fact]
    public void FailedOrCancelled_DoNotEnableAutoClose()
    {
        var failedViewModel =
            new Mp4BoxProgressViewModel();

        failedViewModel.MarkRunning();
        failedViewModel.MarkFailed();

        Assert.False(
            failedViewModel.CanCancel);

        Assert.False(
            failedViewModel.ShouldAutoClose);

        var cancelledViewModel =
            new Mp4BoxProgressViewModel();

        cancelledViewModel.MarkRunning();
        cancelledViewModel.MarkCancelled();

        Assert.False(
            cancelledViewModel.CanCancel);

        Assert.False(
            cancelledViewModel.ShouldAutoClose);
    }
    [Fact]
    public void MarkSucceeded_StartsTwentySecondCountdown()
    {
        var viewModel =
            new Mp4BoxProgressViewModel();

        viewModel.MarkRunning();
        viewModel.MarkSucceeded();

        Assert.Equal(
            20,
            viewModel.AutoCloseSecondsRemaining);

        Assert.Equal(
            "Schließen (20)",
            viewModel.CloseButtonText);

        Assert.False(
            viewModel.IsAutoCloseDue);
    }

    [Fact]
    public void TickAutoCloseCountdown_ReachesZeroAndSignalsAutoClose()
    {
        var viewModel =
            new Mp4BoxProgressViewModel();

        viewModel.MarkRunning();
        viewModel.MarkSucceeded();

        for (var second = 19; second >= 1; second--)
        {
            viewModel.TickAutoCloseCountdown();

            Assert.Equal(
                second,
                viewModel.AutoCloseSecondsRemaining);

            Assert.Equal(
                $"Schließen ({second})",
                viewModel.CloseButtonText);

            Assert.False(
                viewModel.IsAutoCloseDue);
        }

        viewModel.TickAutoCloseCountdown();

        Assert.Equal(
            0,
            viewModel.AutoCloseSecondsRemaining);

        Assert.Equal(
            "Schließen",
            viewModel.CloseButtonText);

        Assert.True(
            viewModel.IsAutoCloseDue);
    }
    [Fact]
    public void MarkCancelRequested_DisablesCancelAndUpdatesStatus()
    {
        var viewModel =
            new Mp4BoxProgressViewModel();

        viewModel.MarkRunning();

        Assert.True(
            viewModel.CanCancel);

        viewModel.MarkCancelRequested();

        Assert.False(
            viewModel.CanCancel);

        Assert.False(
            viewModel.ShouldAutoClose);

        Assert.Equal(
            "Abbruch wird angefordert …",
            viewModel.StatusText);
    }
}
