using CutAssistantNext.App.Services.Analysis;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Tests.Services.Analysis;

public sealed class ConfiguredFfprobeRunnerTests
{
    [Fact]
    public async Task RunAsync_LoadsCurrentFfprobePathForEveryAnalysis()
    {
        var settings =
            new FfmpegSettings
            {
                FfprobeExecutablePath =
                    @"C:\First\ffprobe.exe"
            };

        var usedPaths =
            new List<string>();

        var runner =
            new ConfiguredFfprobeRunner(
                () => settings,
                path =>
                {
                    usedPaths.Add(
                        path);

                    return new RecordingMediaAnalysisRunner();
                });

        await runner.RunAsync(
            "first.mp4");

        settings =
            new FfmpegSettings
            {
                FfprobeExecutablePath =
                    @"D:\Second\ffprobe.exe"
            };

        await runner.RunAsync(
            "second.mp4");

        Assert.Equal(
            new[]
            {
                @"C:\First\ffprobe.exe",
                @"D:\Second\ffprobe.exe"
            },
            usedPaths);
    }

    [Fact]
    public async Task RunAsync_RejectsMissingFfprobeConfiguration()
    {
        var runnerFactoryCalled =
            false;

        var runner =
            new ConfiguredFfprobeRunner(
                () => FfmpegSettings.CreateDefault(),
                path =>
                {
                    runnerFactoryCalled =
                        true;

                    return new RecordingMediaAnalysisRunner();
                });

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => runner.RunAsync(
                    "example.mp4"));

        Assert.Contains(
            "ffprobe",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);

        Assert.False(
            runnerFactoryCalled);
    }
    private sealed class RecordingMediaAnalysisRunner :
        IMediaAnalysisRunner
    {
        public Task<MediaAnalysisResult> RunAsync(
            string mediaFilePath,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new MediaAnalysisResult(
                    FormatName: null,
                    FormatLongName: null,
                    FileSizeBytes: null,
                    Duration: null,
                    VideoStreams: [],
                    AudioStreams: []));
        }
    }
}
