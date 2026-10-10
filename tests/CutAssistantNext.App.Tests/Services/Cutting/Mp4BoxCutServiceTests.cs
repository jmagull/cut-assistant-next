using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class Mp4BoxCutServiceTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-service-{Guid.NewGuid():N}");

    public Mp4BoxCutServiceTests() => Directory.CreateDirectory(_directory);

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    [Fact]
    public async Task RunAsync_PeacemakerCutPlan_PassesExpectedRangesToWorkflow()
    {
        var runner =
            new RecordingMp4BoxRunner();

        var service =
            new Mp4BoxCutService(
                runner);

        var cutPlan =
            new CutPlan(
                TimeSpan.FromSeconds(2612.3213334));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(519.8746667)));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(977.0356667),
                TimeSpan.FromSeconds(1460.0823334)));

        var tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                $"cut-assistant-next-{Guid.NewGuid():N}");

        Directory.CreateDirectory(
            tempDirectory);

        try
        {
            var sourceFilePath =
                Path.Combine(
                    tempDirectory,
                    "source.mp4");

            var outputFilePath =
                Path.Combine(
                    tempDirectory,
                    "output.mp4");

            await service.RunAsync(
                sourceFilePath,
                outputFilePath,
                cutPlan,
                25);

            Assert.Equal(
                2,
                runner.SplitRanges.Count);

            Assert.Equal(
                TimeSpan.FromSeconds(519.8746667),
                runner.SplitRanges[0].Start);

            Assert.Equal(
                TimeSpan.FromSeconds(976.9956667),
                runner.SplitRanges[0].End);

            Assert.Equal(
                TimeSpan.FromSeconds(1460.0823334),
                runner.SplitRanges[1].Start);

            Assert.Equal(
                TimeSpan.FromSeconds(2612.2813334),
                runner.SplitRanges[1].End);

            Assert.Equal(
                1,
                runner.ConcatCallCount);

            Assert.NotEqual(
                Path.GetFullPath(outputFilePath),
                runner.ConcatOutputFilePath);
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }

    [Fact]
    public async Task RunAsync_WithProgress_ReportsWorkflowProgress()
    {
        var runner =
            new RecordingMp4BoxRunner();

        var service =
            new Mp4BoxCutService(
                runner);

        var cutPlan =
            new CutPlan(
                TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30)));

        var updates =
            new List<CutProgressUpdate>();

        var progress =
            new InlineProgress<CutProgressUpdate>(
                updates.Add);

        var outputFilePath =
            Path.Combine(
                _directory,
                $"{Guid.NewGuid():N}.mp4");

        await service.RunAsync(
            "source.mp4",
            outputFilePath,
            cutPlan,
            25,
            progress);

        Assert.Collection(
            updates,
            update =>
                Assert.Equal(
                    "Segment 1 von 2 wird geschnitten …",
                    update.Message),
            update =>
                Assert.Equal(
                    "Segment 2 von 2 wird geschnitten …",
                    update.Message),
            update =>
                Assert.Equal(
                    "Segmente werden zusammengefügt …",
                    update.Message),
            update =>
                Assert.Equal(
                    "Fertig.",
                    update.Message));
    }

    [Fact]
    public async Task RunAsync_WithExistingOutputAndOverwrite_ForwardsOverwritePermission()
    {
        var tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                $"cut-assistant-next-{Guid.NewGuid():N}");

        Directory.CreateDirectory(
            tempDirectory);

        try
        {
            var runner =
                new RecordingMp4BoxRunner
                {
                    ConcatOutputContent =
                        "replacement"
                };

            var service =
                new Mp4BoxCutService(
                    runner);

            var cutPlan =
                new CutPlan(
                    TimeSpan.FromSeconds(100));

            cutPlan.Add(
                new RemoveSegment(
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(30)));

            var sourceFilePath =
                Path.Combine(
                    tempDirectory,
                    "source.mp4");

            var outputFilePath =
                Path.Combine(
                    tempDirectory,
                    "output.mp4");

            File.WriteAllText(
                outputFilePath,
                "existing");

            await service.RunAsync(
                sourceFilePath,
                outputFilePath,
                cutPlan,
                25,
                overwriteExistingOutput: true);

            Assert.Equal(
                1,
                runner.ConcatCallCount);

            Assert.NotNull(
                runner.ConcatOutputFilePath);

            Assert.NotEqual(
                Path.GetFullPath(
                    outputFilePath),
                Path.GetFullPath(
                    runner.ConcatOutputFilePath));

            Assert.Equal(
                "replacement",
                File.ReadAllText(
                    outputFilePath));
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }

    private sealed class InlineProgress<T> :
        IProgress<T>
    {
        private readonly Action<T> _report;

        public InlineProgress(
            Action<T> report)
        {
            _report =
                report
                ?? throw new ArgumentNullException(
                    nameof(report));
        }

        public void Report(
            T value)
        {
            _report(
                value);
        }
    }
    private sealed class RecordingMp4BoxRunner :
        IMp4BoxRunner
    {
        public List<Mp4BoxSplitRange> SplitRanges { get; } =
            [];

        public int ConcatCallCount { get; private set; }

        public string? ConcatOutputFilePath { get; private set; }

        public string? ConcatOutputContent { get; init; } = "complete";

        public Task RunSplitAsync(
            string sourceFilePath,
            string outputFilePath,
            Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            SplitRanges.Add(
                range);

            return Task.CompletedTask;
        }

        public Task RunConcatAsync(
            IReadOnlyList<string> segmentFilePaths,
            string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            ConcatCallCount++;

            ConcatOutputFilePath =
                outputFilePath;

            if (ConcatOutputContent is not null)
            {
                File.WriteAllText(
                    outputFilePath,
                    ConcatOutputContent);
            }

            return Task.CompletedTask;
        }
    }
}
