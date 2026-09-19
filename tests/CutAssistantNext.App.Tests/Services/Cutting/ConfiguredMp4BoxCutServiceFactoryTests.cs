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
    public void Create_RejectsMissingBundledMp4Box()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-missing-mp4box-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var resolver = new ToolPathResolver(root);

            var runnerFactoryCalled = false;

            var factory =
                new ConfiguredMp4BoxCutServiceFactory(
                    () => CutApplicationSettings.CreateDefault(),
                    path =>
                    {
                        runnerFactoryCalled = true;

                        return new RecordingMp4BoxRunner();
                    },
                    resolver);

            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => factory.Create());

            Assert.Contains(
                "MP4Box.exe",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            Assert.False(runnerFactoryCalled);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Create_UsesBundledMp4BoxWhenConfigurationIsEmpty()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-bundled-mp4box-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var resolver = new ToolPathResolver(root);

            var bundledPath = resolver.GetBundledPath(
                BundledToolKind.Mp4Box);

            Directory.CreateDirectory(
                Path.GetDirectoryName(bundledPath)!);

            File.WriteAllText(
                bundledPath,
                string.Empty);

            string? usedPath = null;

            var factory =
                new ConfiguredMp4BoxCutServiceFactory(
                    () => CutApplicationSettings.CreateDefault(),
                    path =>
                    {
                        usedPath = path;

                        return new RecordingMp4BoxRunner();
                    },
                    resolver);

            _ = factory.Create();

            Assert.Equal(
                bundledPath,
                usedPath);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Create_WithProgress_PassesProgressToRunnerFactory()
    {
        var settings =
            new CutApplicationSettings
            {
                ExecutablePath =
                    @"C:\Tools\MP4Box.exe"
            };

        IProgress<Mp4BoxProgressUpdate>? receivedProgress =
            null;

        var factory =
            new ConfiguredMp4BoxCutServiceFactory(
                () => settings,
                (path, progress) =>
                {
                    receivedProgress =
                        progress;

                    return new RecordingMp4BoxRunner();
                });

        var expectedProgress =
            new Progress<Mp4BoxProgressUpdate>();

        _ =
            factory.Create(
                expectedProgress);

        Assert.Same(
            expectedProgress,
            receivedProgress);
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
