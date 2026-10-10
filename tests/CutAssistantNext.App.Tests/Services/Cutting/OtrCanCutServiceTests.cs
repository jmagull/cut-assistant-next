using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class OtrCanCutServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-otr-job-{Guid.NewGuid():N}");
    private readonly OtrCanToolPaths _tools;
    public OtrCanCutServiceTests()
    {
        Directory.CreateDirectory(_root);
        string Tool(string name) { var p = Path.Combine(_root, name); File.WriteAllText(p, "tool"); return p; }
        _tools = new(Tool("engine.exe"), Tool("indexer.exe"), Tool("ffmpeg.exe"), Tool("ffprobe.exe"));
    }
    public void Dispose() => Directory.Delete(_root, true);
    private static MediaAnalysisResult Analysis(int audioCount = 2, string format = "mp4") => new(format, format, 100, TimeSpan.FromSeconds(30),
        [new(0, "h264", null, 320, 180, null, null, 25, null)],
        Enumerable.Range(1, audioCount).Select(i => new AudioStreamInfo(i, "aac", null, 48000, 2, "stereo")).ToArray());
    private CutRequest Request(bool overwrite = false)
    {
        var source = Path.Combine(_root, "original.avi");
        File.WriteAllText(source, "original");
        return new(source, Path.Combine(_root, "final.mp4"),
            [new(TimeSpan.FromSeconds(.12), TimeSpan.FromSeconds(1.16)), new(TimeSpan.FromSeconds(2.04), TimeSpan.FromSeconds(.68))],
            overwriteExistingOutput: overwrite);
    }
    private OtrCanCutService Service(FakeRunner runner, Func<string, CancellationToken, Task<MediaAnalysisResult>>? analyze = null) =>
        new(_tools, runner, analyze ?? ((_, _) => Task.FromResult(Analysis())));

    [Fact]
    public async Task IndexesExactlyOnceUsesOriginalAndExplicitToolsPublishesAndCleans()
    {
        var request = Request().WithSourceFilePath(Path.Combine(_root, "unused-working.mp4"));
        var runner = new FakeRunner();
        await Service(runner).RunAsync(request);
        Assert.Equal(2, runner.Calls.Count);
        Assert.Equal(["-c", "-k", request.OriginalFilePath], runner.Calls[0].Args.Take(3));
        var args = runner.Calls[1].Args;
        Assert.Equal(request.OriginalFilePath, Value(args, "--input"));
        Assert.Equal(runner.Calls[0].Args[3], Value(args, "--index"));
        Assert.Equal(_tools.Ffmpeg, Value(args, "--ffmpeg"));
        Assert.Equal(_tools.Ffprobe, Value(args, "--ffprobe"));
        Assert.Equal("times:[00:00:00.1200000,00:00:01.2800000][00:00:02.0400000,00:00:02.7200000]", Value(args, "--cutlist"));
        Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        Assert.Equal("original", File.ReadAllText(request.OriginalFilePath));
        Assert.Empty(Directory.GetDirectories(_root));
        Assert.Empty(Directory.GetFiles(_root, ".can-otr-*"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HonorsOverwriteConsentAndPreservesTargetUntilValidated(bool overwrite)
    {
        var request = Request(overwrite);
        File.WriteAllText(request.OutputFilePath, "existing");
        var runner = new FakeRunner { OnEngine = () => Assert.Equal("existing", File.ReadAllText(request.OutputFilePath)) };
        if (overwrite)
        {
            await Service(runner).RunAsync(request);
            Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        }
        else
        {
            await Assert.ThrowsAsync<IOException>(() => Service(runner).RunAsync(request));
            Assert.Empty(runner.Calls);
            Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
        }
    }

    [Theory]
    [InlineData("index")]
    [InlineData("engine")]
    [InlineData("cancel")]
    [InlineData("audio")]
    [InlineData("container")]
    [InlineData("late-target")]
    public async Task FailureOrCancellationPreservesOriginalAndExistingOutputAndCleans(string failure)
    {
        var request = Request(overwrite: true);
        if (failure != "late-target") File.WriteAllText(request.OutputFilePath, "existing");
        var runner = new FakeRunner
        {
            FailIndex = failure == "index",
            FailEngine = failure == "engine",
            Cancel = failure == "cancel",
            OnEngine = () => { if (failure == "late-target") File.WriteAllText(request.OutputFilePath, "existing"); }
        };
        var service = Service(runner, (path, _) => Task.FromResult(path == request.OriginalFilePath ? Analysis() :
            Analysis(failure == "audio" ? 1 : 2, failure == "container" ? "avi" : "mp4")));
        if (failure == "cancel") await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.RunAsync(request));
        else await Assert.ThrowsAnyAsync<Exception>(() => service.RunAsync(request));
        Assert.Equal("original", File.ReadAllText(request.OriginalFilePath));
        Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
        Assert.Empty(Directory.GetDirectories(_root));
        Assert.Empty(Directory.GetFiles(_root, ".can-otr-*"));
    }

    [Fact]
    public void KeepsCanTickPrecisionWithoutAddingFrameCorrections()
    {
        var request = new CutRequest(Path.Combine(_root, "in.mp4"), Path.Combine(_root, "out.mp4"),
            [new(TimeSpan.FromTicks(1), TimeSpan.FromMilliseconds(100))]);
        Assert.Equal("times:[00:00:00.0000001,00:00:00.1000001]", OtrCanCutService.BuildCutlist(request));
    }

    [Fact]
    public async Task IncompleteIndexNeverStartsEngineAndIsCleaned()
    {
        var request = Request();
        var runner = new FakeRunner { IncompleteIndex = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() => Service(runner).RunAsync(request));
        Assert.Single(runner.Calls);
        Assert.False(File.Exists(request.OutputFilePath));
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Fact]
    public async Task UnsupportedVideoLayoutIsRejectedBeforeIndexing()
    {
        var request = Request();
        var runner = new FakeRunner();
        var analysis = Analysis() with { VideoStreams = [new(1, "h264", null, 320, 180, null, null, 25, null)] };
        await Assert.ThrowsAsync<InvalidOperationException>(() => Service(runner, (_, _) => Task.FromResult(analysis)).RunAsync(request));
        Assert.Empty(runner.Calls);
    }

    [Fact]
    public async Task MissingToolsAndPreCancellationDoNotIndex()
    {
        var request = Request();
        var runner = new FakeRunner();
        File.Delete(_tools.Ffmpeg);
        await Assert.ThrowsAsync<FileNotFoundException>(() => Service(runner).RunAsync(request));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Service(runner).RunAsync(request, cancellationToken: new(true)));
        Assert.Empty(runner.Calls);
    }

    [Fact]
    public void NativeSelectionBypassesMp4PreparationAndUsesSnapshot()
    {
        var native = new OtrCanSettings();
        var engine = Service(new());
        var factory = new ConfiguredCutEngineFactory(new ConfiguredMp4BoxCutServiceFactory(
            () => throw new InvalidOperationException("MP4 settings should not be read"), (_, _) => throw new InvalidOperationException()),
            settings => { Assert.Same(native, settings); return engine; });
        Assert.Same(engine, factory.Create(CutEngineKind.OtrCan, native));
        Assert.False(ConfiguredCutEngineFactory.RequiresPreparation(CutEngineKind.OtrCan, Analysis(format: "avi")));
        Assert.True(ConfiguredCutEngineFactory.RequiresPreparation(CutEngineKind.Mp4Box, Analysis(format: "avi")));
        Assert.False(ConfiguredCutEngineFactory.RequiresPreparation(CutEngineKind.Mp4Box, Analysis()));
    }

    internal static string Value(string[] args, string flag) => args[Array.IndexOf(args, flag) + 1];
    private sealed class FakeRunner : ICutToolRunner
    {
        public List<(string Path, string[] Args)> Calls { get; } = [];
        public bool FailIndex { get; init; }
        public bool IncompleteIndex { get; init; }
        public bool FailEngine { get; init; }
        public bool Cancel { get; init; }
        public Action? OnEngine { get; init; }
        public Task RunAsync(string path, IReadOnlyList<string> arguments, IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            var args = arguments.ToArray();
            Calls.Add((path, args));
            if (Calls.Count == 1)
            {
                foreach (var suffix in new[] { "", "_track00.tc.txt", "_track00.kf.txt" }) File.WriteAllText(args[3] + suffix, "index");
                if (IncompleteIndex) File.Delete(args[3] + "_track00.kf.txt");
                if (FailIndex) throw new IOException("index failed");
            }
            else
            {
                OnEngine?.Invoke();
                File.WriteAllText(Value(args, "--output"), "complete");
                if (Cancel) throw new OperationCanceledException();
                if (FailEngine) throw new IOException("engine failed");
            }
            return Task.CompletedTask;
        }
    }
}
