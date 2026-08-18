using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class ConfiguredMp4BoxCutServiceFactoryTests
{
    [Fact]
    public void Create_UsesCurrentExecutablePathForEveryCut()
    {
        var settings =
            new CutApplicationSettings
            {
                ExecutablePath =
                    @"C:\First\mp4box.exe"
            };

        var usedPaths =
            new List<string>();

        var factory =
            new ConfiguredMp4BoxCutServiceFactory(
                () => settings,
                path =>
                {
                    usedPaths.Add(
                        path);

                    return new RecordingMp4BoxRunner();
                });

        _ = factory.Create();

        settings =
            new CutApplicationSettings
            {
                ExecutablePath =
                    @"D:\Second\mp4box.exe"
            };

        _ = factory.Create();

        Assert.Equal(
            new[]
            {
                @"C:\First\mp4box.exe",
                @"D:\Second\mp4box.exe"
            },
            usedPaths);
    }

    [Fact]
    public void Create_RejectsMissingCutApplicationConfiguration()
    {
        var runnerFactoryCalled =
            false;

        var factory =
            new ConfiguredMp4BoxCutServiceFactory(
                () => CutApplicationSettings.CreateDefault(),
                path =>
                {
                    runnerFactoryCalled =
                        true;

                    return new RecordingMp4BoxRunner();
                });

        var exception =
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    _ = factory.Create();
                });

        Assert.Contains(
            "Schnittanwendung",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);

        Assert.False(
            runnerFactoryCalled);
    }

    private sealed class RecordingMp4BoxRunner :
        IMp4BoxRunner
    {
        public Task RunSplitAsync(
            string sourceFilePath,
            string outputFilePath,
            Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RunConcatAsync(
            IReadOnlyList<string> segmentFilePaths,
            string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
