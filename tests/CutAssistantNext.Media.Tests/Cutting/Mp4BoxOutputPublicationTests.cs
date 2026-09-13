using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxOutputPublicationTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-output-{Guid.NewGuid():N}");
    private static readonly Mp4BoxSplitRange[] Ranges =
        [new(TimeSpan.Zero, TimeSpan.FromSeconds(10))];

    public Mp4BoxOutputPublicationTests() => Directory.CreateDirectory(_directory);

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task FailureOrCancellation_PreservesTargetAndRemovesTemporaryFiles(bool existing, bool cancel)
    {
        var target = Path.Combine(_directory, "output.mp4");
        if (existing)
            File.WriteAllText(target, "existing");

        using var cancellation = new CancellationTokenSource();
        var workflow = new Mp4BoxCutWorkflow(new Runner(path =>
        {
            Assert.NotEqual(target, path);
            File.WriteAllText(path, "partial output");
            if (cancel)
                cancellation.Cancel();
            else
                throw new IOException("Concat failed");
        }));

        if (cancel)
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                workflow.RunAsync("source.mp4", target, Ranges, cancellation.Token, existing));
        else
            await Assert.ThrowsAsync<IOException>(() =>
                workflow.RunAsync("source.mp4", target, Ranges, cancellation.Token, existing));

        Assert.Equal(existing, File.Exists(target));
        if (existing)
            Assert.Equal("existing", File.ReadAllText(target));
        Assert.Equal(existing ? 1 : 0, Directory.GetFiles(_directory).Length);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Success_PublishesOnlyAfterConcat(bool existing)
    {
        var target = Path.Combine(_directory, "output.mp4");
        if (existing)
            File.WriteAllText(target, "existing");

        var workflow = new Mp4BoxCutWorkflow(new Runner(path =>
        {
            Assert.NotEqual(target, path);
            Assert.Equal(existing, File.Exists(target));
            if (existing)
                Assert.Equal("existing", File.ReadAllText(target));
            File.WriteAllText(path, "complete output");
        }));

        await workflow.RunAsync("source.mp4", target, Ranges, overwriteExistingOutput: existing);

        Assert.Equal("complete output", File.ReadAllText(target));
        Assert.Equal(target, Assert.Single(Directory.GetFiles(_directory)));
    }

    [Fact]
    public async Task TargetCreatedDuringConcat_IsNotOverwritten()
    {
        var target = Path.Combine(_directory, "output.mp4");
        var workflow = new Mp4BoxCutWorkflow(new Runner(path =>
        {
            File.WriteAllText(path, "complete output");
            File.WriteAllText(target, "created elsewhere");
        }));

        await Assert.ThrowsAsync<IOException>(() =>
            workflow.RunAsync("source.mp4", target, Ranges));

        Assert.Equal("created elsewhere", File.ReadAllText(target));
        Assert.Equal(target, Assert.Single(Directory.GetFiles(_directory)));
    }

    private sealed class Runner(Action<string> concat) : IMp4BoxRunner
    {
        public Task RunSplitAsync(string sourceFilePath, string outputFilePath,
            Mp4BoxSplitRange range, CancellationToken cancellationToken = default)
        {
            File.WriteAllText(outputFilePath, "segment");
            return Task.CompletedTask;
        }

        public Task RunConcatAsync(IReadOnlyList<string> segmentFilePaths,
            string outputFilePath, CancellationToken cancellationToken = default)
        {
            concat(outputFilePath);
            return Task.CompletedTask;
        }
    }
}
