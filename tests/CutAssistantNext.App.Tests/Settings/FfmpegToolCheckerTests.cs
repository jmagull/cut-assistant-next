using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class FfmpegToolCheckerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-ffmpeg-check-{Guid.NewGuid():N}");
    public FfmpegToolCheckerTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);
    private FfmpegSettings Setup()
    {
        string Tool(string name) { var path = Path.Combine(_root, name); File.WriteAllText(path, "fixture"); return path; }
        return new() { FfmpegExecutablePath = Tool("ffmpeg.exe"), FfprobeExecutablePath = Tool("ffprobe.exe") };
    }

    [Fact]
    public async Task CheckUsesConfiguredPathsAndOnlyVersionQueries()
    {
        var settings = Setup();
        var probe = new FakeProbe();
        var results = await new FfmpegToolChecker(probe, new ToolPathResolver(_root)).CheckAsync(settings);
        Assert.All(results, result => Assert.True(result.IsAvailable, result.Message));
        Assert.Equal(settings.FfmpegExecutablePath, probe.Calls[0].Path);
        Assert.Equal(settings.FfprobeExecutablePath, probe.Calls[1].Path);
        Assert.All(probe.Calls, call => Assert.Equal(["-version"], call.Arguments));
        Assert.Equal(2, Directory.GetFiles(_root).Length);
    }

    [Theory]
    [InlineData("5.1", "9.0.2", false, true)]
    [InlineData("9.0.2", "5.1", true, false)]
    [InlineData("5.1.1", "8.1.3", true, true)]
    public async Task EachProgramVersionIsCheckedIndependently(string ffmpeg, string ffprobe, bool first, bool second)
    {
        var probe = new FakeProbe { Ffmpeg = ffmpeg, Ffprobe = ffprobe };
        var results = await new FfmpegToolChecker(probe).CheckAsync(Setup());
        Assert.Equal(first, results[0].IsAvailable);
        Assert.Equal(second, results[1].IsAvailable);
        Assert.All(results, result => Assert.Contains("9.0.2", result.Message));
    }

    [Fact]
    public async Task MissingAndUnconfiguredProgramsDoNotStartProcesses()
    {
        var probe = new FakeProbe();
        var checker = new FfmpegToolChecker(probe, new ToolPathResolver(_root));
        var unconfigured = await checker.CheckAsync(new());
        Assert.All(unconfigured, result => Assert.False(result.IsAvailable));
        Assert.Empty(probe.Calls);
        var settings = Setup();
        File.Delete(settings.FfmpegExecutablePath);
        File.Delete(settings.FfprobeExecutablePath);
        var missing = await checker.CheckAsync(settings);
        Assert.All(missing, result => Assert.Contains("nicht gefunden", result.Message));
        Assert.Empty(probe.Calls);
    }

    [Fact]
    public async Task BundledFallbacksWorkWithoutChangingEmptySettings()
    {
        var resolver = new ToolPathResolver(_root);
        foreach (var kind in new[] { BundledToolKind.Ffmpeg, BundledToolKind.Ffprobe })
        {
            var path = resolver.GetBundledPath(kind);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, "fixture");
        }
        var settings = new FfmpegSettings();
        var results = await new FfmpegToolChecker(new FakeProbe(), resolver).CheckAsync(settings);
        Assert.All(results, result => Assert.True(result.IsAvailable));
        Assert.Empty(settings.FfmpegExecutablePath);
        Assert.Empty(settings.FfprobeExecutablePath);
    }

    [Theory]
    [InlineData("exit")]
    [InlineData("timeout")]
    [InlineData("wrong")]
    [InlineData("unknown")]
    public async Task FailureIsReportedWithoutAcceptingUnknownVersion(string failure)
    {
        var probe = new FakeProbe { Failure = failure };
        var results = await new FfmpegToolChecker(probe).CheckAsync(Setup());
        Assert.All(results, result => Assert.False(result.IsAvailable));
        Assert.Equal(2, probe.Calls.Count);
    }

    [Fact]
    public async Task CancellationPropagatesAndStopsStartingFurtherTools()
    {
        using var cancellation = new CancellationTokenSource();
        var probe = new FakeProbe { Cancel = cancellation };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new FfmpegToolChecker(probe).CheckAsync(Setup(), cancellation.Token));
        Assert.Single(probe.Calls);
    }

    private sealed class FakeProbe : IToolProbeRunner
    {
        public string Ffmpeg { get; init; } = "9.0.2";
        public string Ffprobe { get; init; } = "9.0.2";
        public string? Failure { get; init; }
        public CancellationTokenSource? Cancel { get; init; }
        public List<(string Path, string[] Arguments)> Calls { get; } = [];
        public Task<ToolProbeResult> RunAsync(string path, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
        {
            Calls.Add((path, arguments.ToArray()));
            Cancel?.Cancel();
            cancellationToken.ThrowIfCancellationRequested();
            if (Failure == "timeout") throw new TimeoutException("Prüfung dauert zu lange.");
            string name = Path.GetFileNameWithoutExtension(path);
            string version = name == "ffmpeg" ? Ffmpeg : Ffprobe;
            string output = Failure == "wrong" ? "unrelated program" : $"{name} version {(Failure == "unknown" ? "fixture" : version)}";
            return Task.FromResult(new ToolProbeResult(Failure == "exit" ? 2 : 0, "", output));
        }
    }
}
