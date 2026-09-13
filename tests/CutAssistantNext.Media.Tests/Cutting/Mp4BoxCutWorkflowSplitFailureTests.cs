using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxCutWorkflowSplitFailureTests
{
    [Fact]
    public async Task RunAsync_WhenSecondSplitFails_DeletesTemporarySegments()
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
                new FailingSecondSplitMp4BoxRunner();

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

            await Assert.ThrowsAsync<InvalidOperationException>(
                () =>
                    workflow.RunAsync(
                        "source.mp4",
                        Path.Combine(
                            directoryPath,
                            "output.mp4"),
                        ranges));

            Assert.Equal(
                2,
                runner.SegmentFilePaths.Count);

            Assert.All(
                runner.SegmentFilePaths,
                segmentFilePath =>
                    Assert.False(
                        File.Exists(
                            segmentFilePath)));

            Assert.False(
                runner.ConcatWasCalled);
        }
        finally
        {
            Directory.Delete(
                directoryPath,
                recursive: true);
        }
    }

    private sealed class FailingSecondSplitMp4BoxRunner : IMp4BoxRunner
    {
        private int _splitCallCount;

        public List<string> SegmentFilePaths { get; } = [];

        public bool ConcatWasCalled { get; private set; }

        public Task RunSplitAsync(
            string sourceFilePath,
            string outputFilePath,
            Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            _splitCallCount++;

            File.WriteAllText(
                outputFilePath,
                "segment");

            SegmentFilePaths.Add(
                outputFilePath);

            if (_splitCallCount == 2)
            {
                throw new InvalidOperationException(
                    "Simulierter Fehler beim zweiten Split.");
            }

            return Task.CompletedTask;
        }

        public Task RunConcatAsync(
            IReadOnlyList<string> segmentFilePaths,
            string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            ConcatWasCalled = true;

            return Task.CompletedTask;
        }
    }
}
