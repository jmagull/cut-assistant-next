using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class OtrCanToolCheckerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-tool-check-{Guid.NewGuid():N}");
    public OtrCanToolCheckerTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);

    private (OtrCanSettings, FfmpegSettings) Setup()
    {
        string Tool(string name) { var path = Path.Combine(_root, name); File.WriteAllText(path, "fixture"); return path; }
        return (new() { ExecutablePath = Tool("otr_can.exe"), FfmsIndexExecutablePath = Tool("ffmsindex.exe") },
            new() { FfmpegExecutablePath = Tool("ffmpeg.exe"), FfprobeExecutablePath = Tool("ffprobe.exe") });
    }

    [Fact]
    public async Task Check_UsesOnlySafeQueriesAndIndependentExecutablePaths()
    {
        var (settings, ffmpeg) = Setup();
        var probe = new FakeProbe();
        var checker = new OtrCanToolChecker(probe, new ToolPathResolver(_root));
        var results = await checker.CheckAsync(settings, ffmpeg);
        Assert.All(results, result => Assert.True(result.IsAvailable, result.Message));
        Assert.Equal(4, probe.Calls.Count);
        Assert.Equal(["cut-can", "--help"], probe.Calls[0].Arguments);
        Assert.Empty(probe.Calls[1].Arguments);
        Assert.Equal(["-version"], probe.Calls[2].Arguments);
        Assert.Equal(["-version"], probe.Calls[3].Arguments);
        Assert.Equal(settings.ExecutablePath, probe.Calls[0].Path);
        Assert.Equal(ffmpeg.FfprobeExecutablePath, probe.Calls[3].Path);
        Assert.Equal(4, Directory.GetFiles(_root).Length);
    }

    [Fact]
    public async Task UnconfiguredTools_DoNotStartAnyProcess()
    {
        var probe = new FakeProbe();
        var results = await new OtrCanToolChecker(probe, new ToolPathResolver(_root)).CheckAsync(new(), new());
        Assert.Equal(4, results.Count);
        Assert.All(results, result => Assert.False(result.IsAvailable));
        Assert.Empty(probe.Calls);
    }

    [Fact]
    public async Task WrongInterfacesAndFailureExitCodesAreRejected()
    {
        var (settings, ffmpeg) = Setup();
        var probe = new FakeProbe { Override = new(0, "unrelated program", "") };
        var checker = new OtrCanToolChecker(probe, new ToolPathResolver(_root));
        Assert.All(await checker.CheckAsync(settings, ffmpeg), result => Assert.False(result.IsAvailable));
        probe.Override = new(2, "--input --output --index --temp-dir --cutlist ffmsindex -c -k ffmpeg version ffprobe version", "failure");
        Assert.All(await checker.CheckAsync(settings, ffmpeg), result => Assert.False(result.IsAvailable));
    }

    [Fact]
    public async Task ReferenceInterfaceWithoutExplicitToolFlagsCannotBeSelected()
    {
        var (settings, ffmpeg) = Setup();
        var results = await new OtrCanToolChecker(new FakeProbe { Override = new(0, "--input --output --index --temp-dir --cutlist", "") })
            .CheckAsync(settings, ffmpeg);
        Assert.False(results[0].IsAvailable);
    }

    [Fact]
    public async Task MissingFilesAndTimeoutAreReadableFailures()
    {
        var (settings, ffmpeg) = Setup();
        File.Delete(settings.ExecutablePath);
        var probe = new FakeProbe { Fail = new TimeoutException("Prüfung dauert zu lange.") };
        var results = await new OtrCanToolChecker(probe, new ToolPathResolver(_root)).CheckAsync(settings, ffmpeg);
        Assert.Contains("nicht gefunden", results[0].Message);
        Assert.All(results.Skip(1), result => Assert.Contains("zu lange", result.Message));
        Assert.Equal(3, probe.Calls.Count);
    }

    [Fact]
    public async Task CancellationIsPropagatedInsteadOfReportedAsMissingTool()
    {
        var (settings, ffmpeg) = Setup();
        var probe = new FakeProbe();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new OtrCanToolChecker(probe).CheckAsync(settings, ffmpeg, new CancellationToken(true)));
        Assert.Empty(probe.Calls);
    }

    [Fact]
    public async Task ExistingFfmpegFallbacksAreUsedWithoutTouchingSettings()
    {
        var resolver = new ToolPathResolver(_root);
        foreach (var kind in new[] { BundledToolKind.Ffmpeg, BundledToolKind.Ffprobe })
        {
            var path = resolver.GetBundledPath(kind);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, "fixture");
        }
        var results = await new OtrCanToolChecker(new FakeProbe(), resolver).CheckAsync(new(), new());
        Assert.False(results[0].IsAvailable);
        Assert.False(results[1].IsAvailable);
        Assert.True(results[2].IsAvailable);
        Assert.True(results[3].IsAvailable);
    }

    private sealed class FakeProbe : IToolProbeRunner
    {
        public List<(string Path, string[] Arguments)> Calls { get; } = [];
        public ToolProbeResult? Override { get; set; }
        public Exception? Fail { get; init; }
        public Task<ToolProbeResult> RunAsync(string path, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
        {
            Calls.Add((path, arguments.ToArray()));
            if (Fail is not null) return Task.FromException<ToolProbeResult>(Fail);
            var output = Path.GetFileName(path) switch
            {
                "otr_can.exe" => "--input --output --index --temp-dir --cutlist --ffmpeg --ffprobe",
                "ffmsindex.exe" => "Usage: ffmsindex -c -k inputfile outputfile",
                "ffmpeg.exe" => "ffmpeg version fixture",
                _ => "ffprobe version fixture"
            };
            return Task.FromResult(Override ?? new ToolProbeResult(0, output, ""));
        }
    }
}
