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

    [Fact]
    public async Task RunAsync_WithExistingOutputAndOverwrite_ReplacesOnlyAfterSuccessfulConcat()
    {
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

            File.WriteAllText(
                outputFilePath,
                "existing");

            var runner =
                new RecordingMp4BoxRunner(
                    outputFilePath,
                    "replacement");

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
                sourceFilePath,
                outputFilePath,
                ranges,
                overwriteExistingOutput: true);

            Assert.Equal(
                2,
                runner.SplitCallCount);

            Assert.Equal(
                1,
                runner.ConcatCallCount);

            Assert.Equal(
                "existing",
                runner.OriginalOutputContentDuringConcat);

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

            Assert.False(
                File.Exists(
                    runner.ConcatOutputFilePath));
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }

    [Fact]
    public async Task RunAsync_WithExistingOutputAndOverwrite_WhenConcatFails_PreservesExistingOutput()
    {
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

            File.WriteAllText(
                outputFilePath,
                "existing");

            var runner =
                new RecordingMp4BoxRunner(
                    outputFilePath,
                    "partial replacement",
                    new IOException(
                        "Concat failed"));

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

            await Assert.ThrowsAsync<IOException>(
                () =>
                    workflow.RunAsync(
                        sourceFilePath,
                        outputFilePath,
                        ranges,
                        overwriteExistingOutput: true));

            Assert.Equal(
                2,
                runner.SplitCallCount);

            Assert.Equal(
                1,
                runner.ConcatCallCount);

            Assert.Equal(
                "existing",
                File.ReadAllText(
                    outputFilePath));

            Assert.Equal(
                "existing",
                runner.OriginalOutputContentDuringConcat);

            Assert.NotNull(
                runner.ConcatOutputFilePath);

            Assert.False(
                File.Exists(
                    runner.ConcatOutputFilePath));
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
        private readonly string? _originalOutputFilePath;
        private readonly string? _concatOutputContent;

        private readonly Exception? _concatException;

        internal RecordingMp4BoxRunner(
            string? originalOutputFilePath = null,
            string? concatOutputContent = null,
            Exception? concatException = null)
        {
            _originalOutputFilePath =
                originalOutputFilePath;

            _concatOutputContent =
                concatOutputContent;

            _concatException =
                concatException;
        }

        public int SplitCallCount { get; private set; }

        public int ConcatCallCount { get; private set; }

        public string? ConcatOutputFilePath { get; private set; }

        public string? OriginalOutputContentDuringConcat { get; private set; }

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

            ConcatOutputFilePath =
                outputFilePath;

            if (_originalOutputFilePath is not null &&
                File.Exists(
                    _originalOutputFilePath))
            {
                OriginalOutputContentDuringConcat =
                    File.ReadAllText(
                        _originalOutputFilePath);
            }

            if (_concatOutputContent is not null)
            {
                File.WriteAllText(
                    outputFilePath,
                    _concatOutputContent);
            }

            if (_concatException is not null)
            {
                throw _concatException;
            }

            return Task.CompletedTask;
        }
    }
}
