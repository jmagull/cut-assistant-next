using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxCutWorkflowTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-workflow-{Guid.NewGuid():N}");

    public Mp4BoxCutWorkflowTests() => Directory.CreateDirectory(_directory);

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    [Fact]
    public async Task RunAsync_WithTwoRanges_SplitsAndConcatenatesInOrder()
    {
        var runner =
            new RecordingMp4BoxRunner();

        var workflow =
            new Mp4BoxCutWorkflow(
                runner);

        var ranges =
            new[]
            {
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)),
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(40))
            };

        await workflow.RunAsync(
            "source.mp4",
            Path.Combine(_directory, "output.mp4"),
            ranges);

        Assert.Equal(
            2,
            runner.SplitCalls.Count);

        Assert.Equal(
            ranges[0],
            runner.SplitCalls[0].Range);

        Assert.Equal(
            ranges[1],
            runner.SplitCalls[1].Range);

        Assert.Single(
            runner.ConcatCalls);

        Assert.Equal(
            runner.SplitCalls.Select(
                call => call.OutputFilePath),
            runner.ConcatCalls[0].SegmentFilePaths);

        Assert.NotEqual(
            Path.Combine(_directory, "output.mp4"),
            runner.ConcatCalls[0].OutputFilePath);
    }

    [Fact]
    public async Task RunAsync_WithOneRange_PublishesSplitResultWithoutConcat()
    {
        var runner =
            new RecordingMp4BoxRunner
            {
                CreateSplitFiles = true
            };

        var workflow =
            new Mp4BoxCutWorkflow(
                runner);

        var outputFilePath =
            Path.Combine(
                _directory,
                "output.mp4");

        var ranges =
            new[]
            {
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20))
            };

        await workflow.RunAsync(
            "source.mp4",
            outputFilePath,
            ranges);

        Assert.Single(
            runner.SplitCalls);

        Assert.Empty(
            runner.ConcatCalls);

        Assert.True(
            File.Exists(
                outputFilePath));

        Assert.Equal(
            "segment",
            File.ReadAllText(
                outputFilePath));
    }


    [Fact]
    public async Task RunAsync_AfterSuccessfulConcat_DeletesTemporarySegments()
    {
        var directoryPath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}-mp4box-workflow");

        Directory.CreateDirectory(
            directoryPath);

        try
        {
            var runner =
                new RecordingMp4BoxRunner
                {
                    CreateSplitFiles = true
                };

            var workflow =
                new Mp4BoxCutWorkflow(
                    runner);

            var ranges =
                new[]
                {
                    new Mp4BoxSplitRange(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(20)),
                    new Mp4BoxSplitRange(
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(40))
                };

            await workflow.RunAsync(
                "source.mp4",
                Path.Combine(
                    directoryPath,
                    "output.mp4"),
                ranges);

            Assert.All(
                runner.SplitCalls,
                call =>
                    Assert.False(
                        File.Exists(
                            call.OutputFilePath)));
        }
        finally
        {
            Directory.Delete(
                directoryPath,
                recursive: true);
        }
    }
    [Fact]
    public async Task RunAsync_WithTwoRanges_ReportsWorkflowProgressInOrder()
    {
        var runner =
            new RecordingMp4BoxRunner();

        var workflow =
            new Mp4BoxCutWorkflow(
                runner);

        var updates =
            new List<Mp4BoxProgressUpdate>();

        var progress =
            new InlineProgress<Mp4BoxProgressUpdate>(
                updates.Add);

        var ranges =
            new[]
            {
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)),

                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(40))
            };

        var outputFilePath =
            Path.Combine(
                _directory,
                $"{Guid.NewGuid():N}.mp4");

        await workflow.RunAsync(
            "source.mp4",
            outputFilePath,
            ranges,
            progress);

        Assert.Collection(
            updates,
            update =>
            {
                Assert.Equal(
                    Mp4BoxProgressKind.Status,
                    update.Kind);

                Assert.Equal(
                    "Segment 1 von 2 wird geschnitten …",
                    update.Message);
            },
            update =>
            {
                Assert.Equal(
                    Mp4BoxProgressKind.Status,
                    update.Kind);

                Assert.Equal(
                    "Segment 2 von 2 wird geschnitten …",
                    update.Message);
            },
            update =>
            {
                Assert.Equal(
                    Mp4BoxProgressKind.Status,
                    update.Kind);

                Assert.Equal(
                    "Segmente werden zusammengefügt …",
                    update.Message);
            },
            update =>
            {
                Assert.Equal(
                    Mp4BoxProgressKind.Status,
                    update.Kind);

                Assert.Equal(
                    "Fertig.",
                    update.Message);
            });
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
    private sealed class RecordingMp4BoxRunner : IMp4BoxRunner
    {
        public List<SplitCall> SplitCalls { get; } = [];

        public List<ConcatCall> ConcatCalls { get; } = [];
        public bool CreateSplitFiles { get; init; }

        public Task RunSplitAsync(
            string sourceFilePath,
            string outputFilePath,
            Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            if (CreateSplitFiles)
            {
                File.WriteAllText(
                    outputFilePath,
                    "segment");
            }

            SplitCalls.Add(
                new SplitCall(
                    sourceFilePath,
                    outputFilePath,
                    range));

            return Task.CompletedTask;
        }

        public Task RunConcatAsync(
            IReadOnlyList<string> segmentFilePaths,
            string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            File.WriteAllText(outputFilePath, "complete");

            ConcatCalls.Add(
                new ConcatCall(
                    segmentFilePaths.ToArray(),
                    outputFilePath));

            return Task.CompletedTask;
        }
    }

    private sealed record SplitCall(
        string SourceFilePath,
        string OutputFilePath,
        Mp4BoxSplitRange Range);

    private sealed record ConcatCall(
        IReadOnlyList<string> SegmentFilePaths,
        string OutputFilePath);
}
