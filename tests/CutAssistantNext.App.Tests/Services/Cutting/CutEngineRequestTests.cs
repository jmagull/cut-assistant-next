using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class CutEngineRequestTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-engine-request-{Guid.NewGuid():N}");

    public CutEngineRequestTests() => Directory.CreateDirectory(_directory);

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    private string FilePath(string name) => Path.Combine(_directory, name);

    private CutRequest Request(bool overwrite = false, double? rate = 25) => new(
        FilePath("original.avi"), FilePath("output.mp4"),
        [new(TimeSpan.Zero, TimeSpan.FromSeconds(10)), new(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10))],
        rate, overwrite);

    [Fact]
    public async Task CommonRequest_UsesPreparedSourceAndUnchangedMp4BoxBoundaries()
    {
        var runner = new RecordingRunner();
        ICutEngine engine = new Mp4BoxCutService(runner);
        var request = Request().WithSourceFilePath(FilePath("working.mp4"));
        File.WriteAllText(request.OriginalFilePath, "original");
        File.WriteAllText(request.SourceFilePath, "working");
        using var cancellation = new CancellationTokenSource();

        await engine.RunAsync(request, cancellationToken: cancellation.Token);

        Assert.Equal(2, runner.Sources.Count);
        Assert.All(runner.Sources, source => Assert.Equal(request.SourceFilePath, source));
        Assert.Equal(TimeSpan.FromSeconds(9.96), runner.Ranges[0].End);
        Assert.Equal(TimeSpan.FromSeconds(29.96), runner.Ranges[1].End);
        Assert.Equal(cancellation.Token, runner.Token);
        Assert.Equal("original", File.ReadAllText(request.OriginalFilePath));
        Assert.Equal("working", File.ReadAllText(request.SourceFilePath));
        Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        Assert.Equal(3, Directory.GetFiles(_directory).Length);
    }

    [Fact]
    public async Task Factory_DefaultsToMp4BoxAndForwardsSharedProgress()
    {
        var updates = new List<CutProgressUpdate>();
        var progress = new InlineProgress(updates.Add);
        var settings = new CutApplicationSettings { Name = "OTR-CAN", ExecutablePath = FilePath("MP4Box.exe") };
        IProgress<CutProgressUpdate>? runnerProgress = null;
        var runner = new RecordingRunner();
        var factory = new ConfiguredCutEngineFactory(new ConfiguredMp4BoxCutServiceFactory(
            () => settings, (_, sink) => { runnerProgress = sink; runner.Progress = sink; return runner; }));

        var engine = factory.Create(progress);
        Assert.IsType<Mp4BoxCutService>(engine);
        await engine.RunAsync(Request(), progress);

        Assert.Same(progress, runnerProgress);
        Assert.Contains(updates, update => update.Kind == CutProgressKind.Output && update.Message == "runner output");
        Assert.Equal("Fertig.", updates[^1].Message);
    }

    [Fact]
    public async Task ExplicitMp4BoxCommandWorksWithEmptyNativeSettings()
    {
        var factory = new ConfiguredCutEngineFactory(new ConfiguredMp4BoxCutServiceFactory(
            () => new CutApplicationSettings { ExecutablePath = FilePath("MP4Box.exe") },
            (_, _) => new RecordingRunner()),
            _ => throw new InvalidOperationException("Native tools must not be queried."));
        var engine = factory.Create(CutEngineKind.Mp4Box, new());
        Assert.IsType<Mp4BoxCutService>(engine);
        await engine.RunAsync(Request());
        Assert.Equal("complete", File.ReadAllText(FilePath("output.mp4")));
    }

    [Fact]
    public void ExplicitNativeCommandDoesNotFallBackToMp4BoxOnConfigurationFailure()
    {
        var factory = new ConfiguredCutEngineFactory(new ConfiguredMp4BoxCutServiceFactory(
            () => throw new InvalidOperationException("MP4Box must not be selected."),
            (_, _) => new RecordingRunner()),
            _ => throw new FileNotFoundException("Native engine missing."));
        Assert.Throws<FileNotFoundException>(() => factory.Create(CutEngineKind.OtrCan, new()));
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.Create((CutEngineKind)99, new()));
    }

    [Fact]
    public async Task CancelledRequest_NeverStartsRunnerOrChangesExistingOutput()
    {
        var request = Request(overwrite: true);
        File.WriteAllText(request.OutputFilePath, "existing");
        var runner = new RecordingRunner();
        ICutEngine engine = new Mp4BoxCutService(runner);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => engine.RunAsync(request,
            cancellationToken: new CancellationToken(canceled: true)));

        Assert.Empty(runner.Sources);
        Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
    }

    [Fact]
    public async Task Mp4Box_RequiresFrameRateButCommonRequestDoesNot()
    {
        var runner = new RecordingRunner();
        ICutEngine engine = new Mp4BoxCutService(runner);
        var request = Request(rate: null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => engine.RunAsync(request));
        Assert.Empty(runner.Sources);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CommonRequest_RespectsOverwritePermission(bool overwrite)
    {
        var request = Request(overwrite);
        File.WriteAllText(request.OutputFilePath, "existing");
        var runner = new RecordingRunner();
        ICutEngine engine = new Mp4BoxCutService(runner);
        if (overwrite)
        {
            await engine.RunAsync(request);
            Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        }
        else
        {
            await Assert.ThrowsAsync<IOException>(() => engine.RunAsync(request));
            Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
            Assert.Empty(runner.Sources);
        }
    }

    private sealed class InlineProgress(Action<CutProgressUpdate> report) : IProgress<CutProgressUpdate>
    {
        public void Report(CutProgressUpdate value) => report(value);
    }

    private sealed class RecordingRunner : IMp4BoxRunner
    {
        public List<string> Sources { get; } = [];
        public List<Mp4BoxSplitRange> Ranges { get; } = [];
        public CancellationToken Token { get; private set; }
        public IProgress<CutProgressUpdate>? Progress { get; set; }

        public Task RunSplitAsync(string sourceFilePath, string outputFilePath, Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Sources.Add(sourceFilePath);
            Ranges.Add(range);
            Token = cancellationToken;
            File.WriteAllText(outputFilePath, "segment");
            Progress?.Report(new CutProgressUpdate(CutProgressKind.Output, "runner output"));
            return Task.CompletedTask;
        }

        public Task RunConcatAsync(IReadOnlyList<string> segmentFilePaths, string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            File.WriteAllText(outputFilePath, "complete");
            return Task.CompletedTask;
        }
    }
}
