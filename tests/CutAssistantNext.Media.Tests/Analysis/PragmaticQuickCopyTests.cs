using System.Text.Json.Nodes;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Joining;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class PragmaticQuickCopyTests
{
    private const string Header = """
        {"format":{"format_name":"mov,mp4,m4a,3gp,3g2,mj2","duration":"4.04"},"streams":[
        {"index":0,"codec_type":"video","codec_name":"h264","width":640,"height":360,
         "pix_fmt":"yuv420p","time_base":"1/12800","sample_aspect_ratio":"1:1",
         "r_frame_rate":"25/1","avg_frame_rate":"24999/1000","duration":"4.00","start_time":"0.08","extradata_hash":"SHA256:first"},
        {"index":1,"codec_type":"audio","codec_name":"aac","profile":"LC","sample_rate":"48000","channels":2,
         "channel_layout":"stereo","duration":"4.04","time_base":"1/48000","extradata_hash":"SHA256:aac","tags":{"language":"deu"}},
        {"index":2,"codec_type":"audio","codec_name":"ac3","sample_rate":"48000","channels":2,
         "channel_layout":"stereo","duration":"4.032","time_base":"1/48000","tags":{"language":"deu"}}]}
        """;

    private static QuickCopyMediaInfo Info(string? change = null)
    {
        var node = JsonNode.Parse(Header)!;
        var video = node["streams"]![0]!;
        switch (change)
        {
            case "h264": video["extradata_hash"] = "SHA256:other"; break;
            case "geometry": video["width"] = 1920; break;
            case "aac-config": node["streams"]![1]!["extradata_hash"] = "SHA256:incompatible"; break;
            case "codec": node["streams"]![1]!["codec_name"] = "ac3"; break;
            case "rate": node["streams"]![1]!["sample_rate"] = "44100"; break;
            case "channels": node["streams"]![1]!["channels"] = 6; break;
            case "missing": node["streams"]!.AsArray().RemoveAt(2); break;
            case "reverse": node["streams"]![1]!["index"] = 2; node["streams"]![2]!["index"] = 1; break;
        }
        return QuickCopyMediaProbe.Parse(node.ToJsonString());
    }

    [Fact]
    public async Task HeaderWarningsAndDifferentH264ParametersDoNotRejectPragmaticMode()
    {
        var runner = new ProbeRunner(new(0, Header, "Non-monotonic DTS; changing timestamp\nunknown warning"));
        var a = await new QuickCopyMediaProbe("probe", runner).InspectAsync("source", default);
        QuickCopyMediaProbe.ValidateCompatibility(a, Info("h264"), [new(1, 1), new(2, 2)]);
        Assert.Contains(a.Hints, h => h.Contains("unknown warning"));
        Assert.Contains(a.Hints, h => h.Contains("Bildratenangaben"));
        Assert.Contains(a.Hints, h => h.Contains("Bild-/Tonlaufzeiten"));
        Assert.DoesNotContain("-show_packets", runner.Arguments);
        Assert.DoesNotContain("-show_frames", runner.Arguments);
    }

    [Theory]
    [InlineData("geometry")]
    [InlineData("codec")]
    [InlineData("rate")]
    [InlineData("channels")]
    [InlineData("aac-config")]
    [InlineData("missing")]
    public void TrulyIncompatibleOrMissingSelectedStreamsStillFail(string scenario) =>
        Assert.Throws<InvalidDataException>(() => QuickCopyMediaProbe.ValidateCompatibility(Info(), Info(scenario), [new(1, 1), new(2, 2)]));

    [Theory]
    [InlineData(1, "{}")]
    [InlineData(0, "not json")]
    [InlineData(0, "{}")]
    [InlineData(0, "{\"streams\":[],\"format\":{\"format_name\":\"mp4\",\"duration\":\"4\"}}")]
    public async Task FailedOrUnreadableProbeCannotBecomeSuccessfulOutput(int exit, string json) =>
        await Assert.ThrowsAsync<InvalidDataException>(() => new QuickCopyMediaProbe("probe", new ProbeRunner(new(exit, json, "read error"))).InspectAsync("source", default));

    [Fact]
    public async Task BothSelectedAacAndAc3SurviveWithoutEncodingOrFullDecoding()
    {
        using var f = new Fixture();
        var result = await f.Run();
        Assert.True(File.Exists(result.OutputFilePath));
        Assert.Equal(CutReviewTimingKind.Estimated, result.TimingKind);
        var call = Assert.Single(f.Runner.Calls);
        Assert.Contains("concat", call);
        Assert.Equal(new[] { "0:0", "0:1", "0:2" }, Values(call, "-map"));
        Assert.Equal("copy", Assert.Single(Values(call, "-c")));
        Assert.DoesNotContain("-xerror", call);
        Assert.DoesNotContain("pcm_f32le", call);
        Assert.Equal(3, f.Probe.Calls.Count);
        Assert.Empty(Directory.GetDirectories(f.Root, ".can-quick-*"));
        Assert.Contains(f.Messages, m => m.Message == "Meckerecke – technische Hinweise");
        Assert.Contains(f.Messages, m => m.Message.Contains("Phasenzeit:"));
    }

    [Theory]
    [InlineData("reverse")]
    [InlineData("missing")]
    public async Task DifferentTrackOrderOrMissingTrackRequiresOnlyCopyPreparation(string scenario)
    {
        using var f = new Fixture(scenario);
        var pairs = scenario == "reverse" ? new[] { new QuickCopyTrackPair(1, 2), new(2, 1) } : [new QuickCopyTrackPair(1, 1)];
        if (scenario == "missing") f.Probe.Output = Info("missing") with { Media = Info("missing").Media with { Duration = TimeSpan.FromSeconds(8.08) } };
        await f.Run(pairs);
        Assert.Equal(3, f.Runner.Calls.Count);
        Assert.Equal(scenario == "reverse" ? new[] { "0:0", "0:2", "0:1" } : ["0:0", "0:1"], Values(f.Runner.Calls[1], "-map"));
        Assert.All(f.Runner.Calls, a => Assert.Equal("copy", Assert.Single(Values(a, "-c"))));
    }

    [Fact]
    public async Task EveryWarningIsRetainedInMeckereckeWithoutAdditionalChecks()
    {
        using var f = new Fixture("h264");
        string[] warnings = ["Non-monotonic DTS; changed by muxer", "12345 additional audio samples", "An unfamiliar FFmpeg warning"];
        f.Runner.Warnings = warnings;
        var result = await f.Run();
        var section = f.Messages.FindIndex(m => m.Message == "Meckerecke – technische Hinweise");
        Assert.True(section >= 0);
        foreach (var warning in warnings) Assert.Contains(f.Messages.Skip(section + 1), m => m.Message == warning);
        Assert.Contains(f.Messages.Skip(section + 1), m => m.Message.Contains("H.264-Decoderparameter"));
        Assert.Contains("Meckerecke", result.Notice);
        Assert.Single(f.Runner.Calls);
        Assert.Equal(3, f.Probe.Calls.Count);
    }

    [Fact]
    public async Task LargeDurationDeviationIsOnlyAHintWithoutSampleLimitOrAdditionalDecoding()
    {
        using var f = new Fixture();
        f.Probe.Output = f.Probe.Output with { Media = f.Probe.Output.Media with { Duration = TimeSpan.FromSeconds(8.5) } };
        var result = await f.Run();
        Assert.Equal(TimeSpan.FromSeconds(8.5), result.Timeline.Duration);
        Assert.Contains(f.Messages, m => m.Message.Contains("Containerlaufzeiten:"));
        Assert.Single(f.Runner.Calls);
    }

    [Theory]
    [InlineData("process")]
    [InlineData("empty")]
    [InlineData("unreadable")]
    [InlineData("wrong-tracks")]
    [InlineData("cancel")]
    public async Task TrueFailuresKeepDiagnosticsAndNeverPublishOrAlterOriginals(string scenario)
    {
        using var f = new Fixture();
        f.Runner.Failure = scenario;
        f.Probe.Failure = scenario;
        var error = await Assert.ThrowsAnyAsync<Exception>(() => f.Run());
        if (scenario == "process") Assert.Equal("FFmpeg exit 1", error.Message);
        if (scenario == "unreadable") Assert.Equal("Unreadable output", error.Message);
        if (scenario == "cancel") Assert.IsType<OperationCanceledException>(error);
        Assert.False(File.Exists(f.Output));
        f.AssertOriginals();
        var dir = Assert.Single(Directory.GetDirectories(f.Root, ".can-quick-*"));
        Assert.Equal("protocol.txt", Path.GetFileName(Assert.Single(Directory.GetFiles(dir))));
        var log = await File.ReadAllTextAsync(Path.Combine(dir, "protocol.txt"));
        Assert.Contains("Abgebrochen:", log);
        Assert.Contains("Meckerecke – technische Hinweise", log);
    }

    [Theory]
    [InlineData("exists")]
    [InlineData("race")]
    [InlineData("original")]
    public async Task ExistingFilesAndOriginalsCannotBeOverwritten(string scenario)
    {
        using var f = new Fixture();
        if (scenario == "exists") await File.WriteAllTextAsync(f.Output, "KEEP");
        if (scenario == "race") f.Runner.AfterCopy = () => File.WriteAllText(f.Output, "KEEP");
        await Assert.ThrowsAnyAsync<Exception>(() => f.Run(output: scenario == "original" ? f.First : null));
        if (scenario != "original") Assert.Equal("KEEP", await File.ReadAllTextAsync(f.Output));
        f.AssertOriginals();
    }

    private static string[] Values(IReadOnlyList<string> args, string name) =>
        args.Select((a, i) => (a, i)).Where(x => x.a == name).Select(x => args[x.i + 1]).ToArray();

    private sealed class ProbeRunner(ToolProbeResult result) : IToolProbeRunner
    {
        public IReadOnlyList<string> Arguments { get; private set; } = [];
        public Task<ToolProbeResult> RunAsync(string exe, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
        { Arguments = arguments; return Task.FromResult(result); }
    }

    private sealed class Fixture : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "can-pragmatic-tests-" + Guid.NewGuid().ToString("N"));
        public string First => Path.Combine(Root, "first.mp4");
        public string Second => Path.Combine(Root, "second.mp4");
        public string Output => Path.Combine(Root, "output.mp4");
        public FakeProbe Probe { get; }
        public FakeRunner Runner { get; } = new();
        public List<CutProgressUpdate> Messages { get; } = [];
        public Fixture(string? second = null)
        {
            Directory.CreateDirectory(Root);
            File.WriteAllText(First, "FIRST"); File.WriteAllText(Second, "SECOND");
            Probe = new FakeProbe(First, Second, Info(second));
        }
        public Task<CutResult> Run(QuickCopyTrackPair[]? pairs = null, string? output = null) =>
            new QuickCopyJoinService("ffmpeg", "ffprobe", Runner, mediaProbe: Probe).RunAsync(
                new(First, Second, pairs ?? [new(1, 1), new(2, 2)], output ?? Output, true), new Reporter(Messages), default);
        public void AssertOriginals() { Assert.Equal("FIRST", File.ReadAllText(First)); Assert.Equal("SECOND", File.ReadAllText(Second)); }
        public void Dispose()
        {
            var target = Path.GetFullPath(Root);
            Assert.Equal(Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar), Path.GetDirectoryName(target), ignoreCase: true);
            Assert.StartsWith("can-pragmatic-tests-", Path.GetFileName(target));
            Directory.Delete(target, recursive: true);
        }
    }

    private sealed class Reporter(List<CutProgressUpdate> messages) : IProgress<CutProgressUpdate>
    { public void Report(CutProgressUpdate value) => messages.Add(value); }

    private sealed class FakeProbe(string first, string second, QuickCopyMediaInfo secondInfo) : IQuickCopyMediaProbe
    {
        public List<string> Calls { get; } = [];
        public string Failure { get; set; } = "";
        public QuickCopyMediaInfo Output { get; set; } = Info() with { Media = Info().Media with { Duration = TimeSpan.FromSeconds(8.08) } };
        public Task<QuickCopyMediaInfo> InspectAsync(string path, CancellationToken token)
        {
            Calls.Add(path);
            if (path == first) return Task.FromResult(Info());
            if (path == second) return Task.FromResult(secondInfo);
            if (!path.EndsWith("result.mp4", StringComparison.Ordinal)) return Task.FromResult(Info());
            if (Failure == "unreadable") throw new InvalidDataException("Unreadable output");
            return Task.FromResult(Failure == "wrong-tracks" ? Info("missing") : Output);
        }
    }

    private sealed class FakeRunner : ICutToolRunner
    {
        public List<IReadOnlyList<string>> Calls { get; } = [];
        public string[] Warnings { get; set; } = [];
        public string Failure { get; set; } = "";
        public Action? AfterCopy { get; set; }
        public Task RunAsync(string exe, IReadOnlyList<string> arguments, IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            Calls.Add(arguments);
            foreach (var w in Warnings) progress?.Report(new(CutProgressKind.Output, w));
            if (Failure == "process") throw new InvalidOperationException("FFmpeg exit 1");
            if (Failure == "cancel") throw new OperationCanceledException();
            File.WriteAllText(arguments[^1], Failure == "empty" ? "" : "OUTPUT");
            AfterCopy?.Invoke();
            return Task.CompletedTask;
        }
    }
}
