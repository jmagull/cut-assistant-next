using System.Globalization;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Tests;

public class MainWindowViewModelTests
{
    [Fact]
    public async Task AnalyzeAsync_FormatsSuccessfulResult()
    {
        using var cultureScope = new CultureScope("de-DE");

        var runner = new StubMediaAnalysisRunner(
            (_, _) => Task.FromResult(CreateCompleteResult()));

        var viewModel = new MainWindowViewModel(runner);

        var mediaFilePath = Path.Combine(
            Path.GetTempPath(),
            "Beispiel.mp4");

        await viewModel.AnalyzeAsync(mediaFilePath);

        Assert.Equal("Beispiel.mp4", viewModel.FileName);
        Assert.Equal(
            Path.GetFullPath(mediaFilePath),
            viewModel.FilePath);

        Assert.Equal(
            "QuickTime / MOV (mov,mp4,m4a,3gp,3g2,mj2)",
            viewModel.ContainerFormat);

        Assert.Equal("700,00 MiB", viewModel.FileSize);
        Assert.Equal("01:02:20.180", viewModel.Duration);

        Assert.Equal(
            "H.264 / AVC (h264)",
            viewModel.VideoCodec);

        Assert.Equal("720 × 576", viewModel.Resolution);
        Assert.Equal("16:15", viewModel.SampleAspectRatio);
        Assert.Equal("4:3", viewModel.DisplayAspectRatio);
        Assert.Equal("25 fps", viewModel.FrameRate);
        Assert.Equal("progressive", viewModel.FieldOrder);

        Assert.Equal("AAC (aac)", viewModel.AudioCodec);
        Assert.Equal("48.000 Hz", viewModel.SampleRate);
        Assert.Equal("2", viewModel.ChannelCount);
        Assert.Equal("stereo", viewModel.ChannelLayout);

        Assert.Equal(
            "Analyse erfolgreich abgeschlossen.",
            viewModel.StatusMessage);

        Assert.False(viewModel.IsAnalyzing);
        Assert.True(viewModel.CanAnalyze);
        Assert.False(viewModel.HasError);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public async Task AnalyzeAsync_ShowsRunnerError()
    {
        var runner = new StubMediaAnalysisRunner(
            (_, _) => throw new InvalidOperationException(
                "Testfehler bei ffprobe."));

        var viewModel = new MainWindowViewModel(runner);

        await viewModel.AnalyzeAsync("Beispiel.mp4");

        Assert.Equal(
            "Analyse fehlgeschlagen.",
            viewModel.StatusMessage);

        Assert.Equal(
            "Testfehler bei ffprobe.",
            viewModel.ErrorMessage);

        Assert.True(viewModel.HasError);
        Assert.False(viewModel.IsAnalyzing);
        Assert.True(viewModel.CanAnalyze);
    }

    [Fact]
    public async Task AnalyzeAsync_UpdatesBusyStateWhileRunnerIsActive()
    {
        var completionSource =
            new TaskCompletionSource<MediaAnalysisResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        var runner = new StubMediaAnalysisRunner(
            (_, _) => completionSource.Task);

        var viewModel = new MainWindowViewModel(runner);

        var analysisTask =
            viewModel.AnalyzeAsync("Beispiel.mp4");

        Assert.True(viewModel.IsAnalyzing);
        Assert.False(viewModel.CanAnalyze);
        Assert.Equal(
            "Datei wird analysiert …",
            viewModel.StatusMessage);

        completionSource.SetResult(CreateCompleteResult());

        await analysisTask;

        Assert.False(viewModel.IsAnalyzing);
        Assert.True(viewModel.CanAnalyze);
        Assert.Equal(
            "Analyse erfolgreich abgeschlossen.",
            viewModel.StatusMessage);
    }

    [Fact]
    public async Task AnalyzeAsync_UsesFallbackForMissingInformation()
    {
        var result = new MediaAnalysisResult(
            null,
            null,
            null,
            null,
            [],
            []);

        var runner = new StubMediaAnalysisRunner(
            (_, _) => Task.FromResult(result));

        var viewModel = new MainWindowViewModel(runner);

        await viewModel.AnalyzeAsync("Beispiel.mp4");

        Assert.Equal(
            "Nicht verfügbar",
            viewModel.ContainerFormat);

        Assert.Equal("Nicht verfügbar", viewModel.FileSize);
        Assert.Equal("Nicht verfügbar", viewModel.Duration);
        Assert.Equal("Nicht verfügbar", viewModel.VideoCodec);
        Assert.Equal("Nicht verfügbar", viewModel.Resolution);
        Assert.Equal(
            "Nicht verfügbar",
            viewModel.SampleAspectRatio);

        Assert.Equal(
            "Nicht verfügbar",
            viewModel.DisplayAspectRatio);

        Assert.Equal("Nicht verfügbar", viewModel.FrameRate);
        Assert.Equal("Nicht verfügbar", viewModel.FieldOrder);
        Assert.Equal("Nicht verfügbar", viewModel.AudioCodec);
        Assert.Equal("Nicht verfügbar", viewModel.SampleRate);
        Assert.Equal("Nicht verfügbar", viewModel.ChannelCount);
        Assert.Equal("Nicht verfügbar", viewModel.ChannelLayout);
    }

    private static MediaAnalysisResult CreateCompleteResult()
    {
        return new MediaAnalysisResult(
            "mov,mp4,m4a,3gp,3g2,mj2",
            "QuickTime / MOV",
            734_003_200,
            TimeSpan.FromSeconds(3740.18),
            [
                new VideoStreamInfo(
                    0,
                    "h264",
                    "H.264 / AVC",
                    720,
                    576,
                    "16:15",
                    "4:3",
                    25.0,
                    "progressive")
            ],
            [
                new AudioStreamInfo(
                    1,
                    "aac",
                    "AAC",
                    48_000,
                    2,
                    "stereo")
            ]);
    }

    private sealed class StubMediaAnalysisRunner
        : IMediaAnalysisRunner
    {
        private readonly Func<
            string,
            CancellationToken,
            Task<MediaAnalysisResult>> _runAsync;

        public StubMediaAnalysisRunner(
            Func<
                string,
                CancellationToken,
                Task<MediaAnalysisResult>> runAsync)
        {
            _runAsync = runAsync;
        }

        public Task<MediaAnalysisResult> RunAsync(
            string mediaFilePath,
            CancellationToken cancellationToken = default)
        {
            return _runAsync(
                mediaFilePath,
                cancellationToken);
        }
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCulture;
        private readonly CultureInfo _originalUiCulture;

        public CultureScope(string cultureName)
        {
            _originalCulture = CultureInfo.CurrentCulture;
            _originalUiCulture = CultureInfo.CurrentUICulture;

            var culture = CultureInfo.GetCultureInfo(cultureName);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUiCulture;
        }
    }
}
