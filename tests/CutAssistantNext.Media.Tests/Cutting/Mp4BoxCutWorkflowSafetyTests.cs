using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxCutWorkflowSafetyTests
{
    [Fact]
    public async Task RunAsync_WithIdenticalSourceAndOutput_RejectsBeforeRunner()
    {
        var runner =
            new RecordingMp4BoxRunner();

        var workflow =
            new Mp4BoxCutWorkflow(
                runner);

        var filePath =
            Path.GetFullPath(
                "same-file.mp4");

        var ranges =
            new[]
            {
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20))
            };

        await Assert.ThrowsAsync<ArgumentException>(
            () =>
                workflow.RunAsync(
                    filePath,
                    filePath,
                    ranges));

        Assert.Equal(
            0,
            runner.SplitCallCount);

        Assert.Equal(
            0,
            runner.ConcatCallCount);
    }

    [Fact]
    public async Task RunAsync_WithRelativeSourceAndSameAbsoluteOutput_RejectsBeforeRunner()
    {
        var runner =
            new RecordingMp4BoxRunner();

        var workflow =
            new Mp4BoxCutWorkflow(
                runner);

        const string relativeSourceFilePath =
            "same-file.mp4";

        var absoluteOutputFilePath =
            Path.GetFullPath(
                relativeSourceFilePath);

        var ranges =
            new[]
            {
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20))
            };

        await Assert.ThrowsAsync<ArgumentException>(
            () =>
                workflow.RunAsync(
                    relativeSourceFilePath,
                    absoluteOutputFilePath,
                    ranges));

        Assert.Equal(
            0,
            runner.SplitCallCount);

        Assert.Equal(
            0,
            runner.ConcatCallCount);
    }
    [Fact]
    public async Task RunAsync_WithExistingOutput_RejectsBeforeRunner()
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
                new RecordingMp4BoxRunner();

            var workflow =
                new Mp4BoxCutWorkflow(
                    runner);

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

            var ranges =
                new[]
                {
                    new Mp4BoxSplitRange(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(20))
                };

            await Assert.ThrowsAsync<IOException>(
                () =>
                    workflow.RunAsync(
                        sourceFilePath,
                        outputFilePath,
                        ranges));

            Assert.Equal(
                0,
                runner.SplitCallCount);

            Assert.Equal(
                0,
                runner.ConcatCallCount);
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }
    private sealed class RecordingMp4BoxRunner :
        IMp4BoxRunner
    {
        public int SplitCallCount { get; private set; }

        public int ConcatCallCount { get; private set; }

        public Task RunSplitAsync(
            string sourceFilePath,
            string outputFilePath,
            Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            SplitCallCount++;

            return Task.CompletedTask;
        }

        public Task RunConcatAsync(
            IReadOnlyList<string> segmentFilePaths,
            string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            ConcatCallCount++;

            return Task.CompletedTask;
        }
    }
}
