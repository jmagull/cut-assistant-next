using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Tests;

public sealed class MainWindowSourceWarningTests
{
    [Fact]
    public async Task AviWarningKeepsAnalysisAvailableAndNotifiesTheView()
    {
        var result = Analysis("avi");
        var viewModel = new MainWindowViewModel(new StubRunner((_, _) => Task.FromResult(result)));
        var changedProperties = new List<string?>();
        viewModel.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName);

        await viewModel.AnalyzeAsync("Film.avi.mp4");

        Assert.True(viewModel.HasSourceWarning);
        Assert.Contains("endet auf .mp4", viewModel.SourceWarningMessage);
        Assert.Contains(nameof(viewModel.SourceWarningMessage), changedProperties);
        Assert.Contains(nameof(viewModel.HasSourceWarning), changedProperties);
        Assert.Same(result, viewModel.AnalysisResult);
        Assert.Equal(result.Duration, viewModel.MediaDuration);
        Assert.True(viewModel.CanAnalyze);
        Assert.False(viewModel.CanOpenFrameLoupe);
        Assert.Contains(nameof(viewModel.CanOpenFrameLoupe), changedProperties);
        Assert.Contains(nameof(viewModel.IsPlayerFrameSearchVisible), changedProperties);
        Assert.Equal(viewModel.FrameLoupeToolTip, viewModel.SourceWarningMessage.Substring(viewModel.SourceWarningMessage.LastIndexOf("Die Frame-Lupe", StringComparison.Ordinal)));
        Assert.False(viewModel.HasError);
        Assert.Equal("Analyse erfolgreich abgeschlossen, geladen:", viewModel.StatusMessage);
    }

    [Fact]
    public async Task SwitchingToMp4ClearsWarningBeforeNewAnalysisCompletes()
    {
        var nextResult = new TaskCompletionSource<MediaAnalysisResult>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var callCount = 0;
        var viewModel = new MainWindowViewModel(new StubRunner((_, _) =>
            ++callCount == 1 ? Task.FromResult(Analysis("avi")) : nextResult.Task));
        await viewModel.AnalyzeAsync("Erste.avi.mp4");
        Assert.True(viewModel.HasSourceWarning);
        var changedProperties = new List<string?>();
        viewModel.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName);

        var nextAnalysis = viewModel.AnalyzeAsync("Zweite.mp4");

        Assert.True(viewModel.IsAnalyzing);
        Assert.False(viewModel.CanOpenFrameLoupe);
        Assert.False(viewModel.HasSourceWarning);
        Assert.Equal(string.Empty, viewModel.SourceWarningMessage);
        Assert.Contains(nameof(viewModel.HasSourceWarning), changedProperties);
        nextResult.SetResult(Analysis("mov,mp4"));
        await nextAnalysis;
        Assert.True(viewModel.CanOpenFrameLoupe);
        Assert.Equal("Frame-Lupe öffnen (Shift+F)", viewModel.FrameLoupeToolTip);
        Assert.Contains(nameof(viewModel.CanOpenFrameLoupe), changedProperties);
        Assert.Contains(nameof(viewModel.IsPlayerFrameSearchVisible), changedProperties);
        Assert.Contains(nameof(viewModel.FrameLoupeToolTip), changedProperties);
        Assert.True(viewModel.IsPlayerFrameSearchVisible);
        Assert.Contains(nameof(viewModel.IsPlayerFrameSearchVisible), changedProperties);
        Assert.False(viewModel.HasSourceWarning);
        Assert.False(viewModel.HasError);
    }

    [Fact]
    public async Task FailedAnalysisDoesNotKeepPreviousAviWarning()
    {
        var callCount = 0;
        var viewModel = new MainWindowViewModel(new StubRunner((_, _) =>
            ++callCount == 1 ? Task.FromResult(Analysis("avi")) :
            Task.FromException<MediaAnalysisResult>(new IOException("Analysefehler"))));
        await viewModel.AnalyzeAsync("Erste.avi");
        Assert.True(viewModel.HasSourceWarning);

        await viewModel.AnalyzeAsync("Zweite.mp4");

        Assert.False(viewModel.HasSourceWarning);
        Assert.Equal(string.Empty, viewModel.SourceWarningMessage);
        Assert.True(viewModel.HasError);
        Assert.Equal("Analysefehler", viewModel.ErrorMessage);
        Assert.Null(viewModel.AnalysisResult);
        Assert.False(viewModel.CanOpenFrameLoupe);
    }

    [Fact]
    public async Task CancelledAnalysisDoesNotKeepPreviousAviWarning()
    {
        var callCount = 0;
        var viewModel = new MainWindowViewModel(new StubRunner((_, token) =>
            ++callCount == 1 ? Task.FromResult(Analysis("avi")) :
            Task.FromCanceled<MediaAnalysisResult>(token)));
        await viewModel.AnalyzeAsync("Erste.avi");
        Assert.True(viewModel.HasSourceWarning);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await viewModel.AnalyzeAsync("Zweite.mp4", cancellation.Token);

        Assert.False(viewModel.HasSourceWarning);
        Assert.Equal(string.Empty, viewModel.SourceWarningMessage);
        Assert.False(viewModel.HasError);
        Assert.Null(viewModel.AnalysisResult);
        Assert.False(viewModel.CanOpenFrameLoupe);
        Assert.Equal("Analyse wurde abgebrochen.", viewModel.StatusMessage);
    }

    [Theory]
    [InlineData("Film.avi", "avi", false)]
    [InlineData("Film.mp4", " AVI , avi ", false)]
    [InlineData("Film.avi.mp4", "avi", false)]
    [InlineData("Film.avi", "mov,mp4", true)]
    [InlineData("Film.mp4", "mov,mp4,m4a,3gp,3g2,mj2", true)]
    [InlineData("Film.mkv", "matroska,webm", true)]
    [InlineData("Film.avif", "avif", true)]
    public async Task LoupeAvailabilityUsesDetectedContainer(string fileName, string format, bool expected)
    {
        var viewModel = new MainWindowViewModel(new StubRunner((_, _) => Task.FromResult(Analysis(format))));

        await viewModel.AnalyzeAsync(fileName);

        Assert.Equal(expected, viewModel.CanOpenFrameLoupe);
        Assert.Equal(expected, viewModel.IsPlayerFrameSearchVisible);
        Assert.Equal(!expected, viewModel.HasSourceWarning);
        if (!expected)
            Assert.Contains("für AVI-Container derzeit nicht zur Verfügung", viewModel.FrameLoupeToolTip);
    }

    [Fact]
    public async Task LoupeIsUnavailableWithoutAnalyzedVideo()
    {
        var audioOnly = Analysis("mov,mp4") with { VideoStreams = [] };
        var viewModel = new MainWindowViewModel(new StubRunner((_, _) => Task.FromResult(audioOnly)));
        Assert.False(viewModel.CanOpenFrameLoupe);

        await viewModel.AnalyzeAsync("Ton.m4a");

        Assert.False(viewModel.CanOpenFrameLoupe);
        Assert.False(viewModel.HasSourceWarning);
    }

    [Fact]
    public async Task LoadingAviImmediatelyRevokesPreviousMp4LoupeAvailability()
    {
        var pending = new TaskCompletionSource<MediaAnalysisResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        var viewModel = new MainWindowViewModel(new StubRunner((_, _) =>
            ++calls == 1 ? Task.FromResult(Analysis("mov,mp4")) : pending.Task));
        await viewModel.AnalyzeAsync("Film.mp4");
        Assert.True(viewModel.CanOpenFrameLoupe);

        var loading = viewModel.AnalyzeAsync("Film.avi");
        Assert.False(viewModel.CanOpenFrameLoupe);
        pending.SetResult(Analysis("avi"));
        await loading;

        Assert.False(viewModel.CanOpenFrameLoupe);
        Assert.True(viewModel.HasSourceWarning);
    }
    private static MediaAnalysisResult Analysis(string format) =>
        new(format, null, null, TimeSpan.FromMinutes(40), [new(0, "h264", null, 720, 576, null, null, 25, "progressive")], []);

    private sealed class StubRunner(
        Func<string, CancellationToken, Task<MediaAnalysisResult>> run) : IMediaAnalysisRunner
    {
        public Task<MediaAnalysisResult> RunAsync(string path, CancellationToken cancellationToken = default) =>
            run(path, cancellationToken);
    }
}
