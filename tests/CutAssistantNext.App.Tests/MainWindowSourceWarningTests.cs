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
        Assert.False(viewModel.HasSourceWarning);
        Assert.Equal(string.Empty, viewModel.SourceWarningMessage);
        Assert.Contains(nameof(viewModel.HasSourceWarning), changedProperties);
        nextResult.SetResult(Analysis("mov,mp4"));
        await nextAnalysis;
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
        Assert.Equal("Analyse wurde abgebrochen.", viewModel.StatusMessage);
    }

    private static MediaAnalysisResult Analysis(string format) =>
        new(format, null, null, TimeSpan.FromMinutes(40), [], []);

    private sealed class StubRunner(
        Func<string, CancellationToken, Task<MediaAnalysisResult>> run) : IMediaAnalysisRunner
    {
        public Task<MediaAnalysisResult> RunAsync(string path, CancellationToken cancellationToken = default) =>
            run(path, cancellationToken);
    }
}
