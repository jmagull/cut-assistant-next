using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxCutWorkflowFailureTests
{
    [Fact]
    public async Task RunAsync_WhenConcatFails_DeletesTemporarySegments()
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
                new FailingConcatMp4BoxRunner();

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

            Assert.All(
                runner.SegmentFilePaths,
                segmentFilePath =>
                    Assert.False(
                        File.Exists(
                            segmentFilePath)));
        }
        finally
        {
            Directory.Delete(
                directoryPath,
                recursive: true);
        }
    }

    private sealed class FailingConcatMp4BoxRunner : IMp4BoxRunner
    {
        public List<string> SegmentFilePaths { get; } = [];

        public Task RunSplitAsync(
            string sourceFilePath,
            string outputFilePath,
            Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            File.WriteAllText(
                outputFilePath,
                "segment");

            SegmentFilePaths.Add(
                outputFilePath);

            return Task.CompletedTask;
        }

        public Task RunConcatAsync(
            IReadOnlyList<string> segmentFilePaths,
            string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException(
                "Simulierter MP4Box-Fehler.");
        }
    }
}
