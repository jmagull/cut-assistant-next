using System.Text.Json;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class AudioMp4BoxFallbackTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-audio-fallback-" + Guid.NewGuid().ToString("N"));
    public AudioMp4BoxFallbackTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);
    private static MediaAnalysisResult Media(string format = "mp4") => new(format, format, 4, TimeSpan.FromSeconds(10),
        [new(0, "h264", null, 160, 96, null, null, 25, null)], [new(1, "aac", null, 48000, 2, "stereo")]);
    private CutRequest Request(bool overwrite = false)
    {
        var source = Path.Combine(_root, "source.mp4"); File.WriteAllText(source, "original");
        return new(source, Path.Combine(_root, "out.mp4"), [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))], 25, overwrite);
    }
    private static OtrCanAudioFailureException Failure() => new(Media(), Media(),
        new InvalidOperationException("[audio-diagnostic-v1] PCM count differs for audio stream 1: expected 48000, got 47488"));
    private sealed class Engine(Func<CutRequest, CancellationToken, Task> run) : ICutEngine
    {
        internal List<CutRequest> Requests { get; } = [];
        public Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null, CancellationToken cancellationToken = default)
        { Requests.Add(request); return run(request, cancellationToken); }
    }
    private sealed class Updates : IProgress<CutProgressUpdate>
    {
        internal List<CutProgressUpdate> Items { get; } = [];
        public void Report(CutProgressUpdate update) => Items.Add(update);
    }

    [Fact]
    public async Task HealthyCutNeverAsksOrReadsAlternativeConfiguration()
    {
        var request = Request(); var primary = new Engine((_, _) => Task.CompletedTask);
        var engine = new OtrCanMp4BoxFallbackCutService(primary, (_, _) => throw new Exception("no alternative"),
            (_, _) => throw new Exception("no question"));
        await engine.RunWithResultAsync(request);
        Assert.False(engine.UsedMp4Box); Assert.Same(request, Assert.Single(primary.Requests));
    }
    [Fact]
    public async Task OnlyExplicitConsentRestartsOnceWithSameImmutableRequestAndWarnsAtCompletion()
    {
        var request = Request(true); File.WriteAllText(request.OutputFilePath, "existing");
        var primary = new Engine((_, _) => throw Failure());
        var alternative = new Engine((r, _) => { Assert.Same(request, r); File.WriteAllText(r.OutputFilePath, "complete"); return Task.CompletedTask; });
        var confirmed = false; var updates = new Updates();
        var engine = new OtrCanMp4BoxFallbackCutService(primary, (_, _) =>
        { Assert.True(confirmed); return Task.FromResult<ICutEngine>(alternative); }, (_, _) =>
        { Assert.Equal("existing", File.ReadAllText(request.OutputFilePath)); confirmed = true; return Task.FromResult(true); });
        var result = await engine.RunWithResultAsync(request, updates);
        Assert.True(engine.UsedMp4Box); Assert.Single(primary.Requests); Assert.Single(alternative.Requests);
        Assert.Contains("keine nachgewiesene Audioreparatur", result.Notice);
        Assert.Contains("persönlich kontrollieren", result.Notice);
        Assert.Contains(updates.Items, x => x.Message.Contains("Ausdrücklich bestätigt"));
        Assert.Equal("original", File.ReadAllText(request.SourceFilePath)); Assert.Single(request.KeepSegments);
    }
    [Fact]
    public async Task DeclinePreservesOriginalFailureTargetAndCutTimes()
    {
        var request = Request(true); File.WriteAllText(request.OutputFilePath, "existing"); var failure = Failure();
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine((_, _) => throw failure),
            (_, _) => throw new Exception("must not inspect settings"), (_, _) => Task.FromResult(false));
        Assert.Same(failure, await Assert.ThrowsAsync<OtrCanAudioFailureException>(() => engine.RunAsync(request)));
        Assert.Equal("existing", File.ReadAllText(request.OutputFilePath)); Assert.Equal(TimeSpan.Zero, request.KeepSegments[0].Start);
        Assert.False(engine.UsedMp4Box);
    }
    [Theory]
    [InlineData("confirm")]
    [InlineData("check")]
    public async Task CancellationNeverStartsAlternative(string stage)
    {
        var request = Request(); using var cts = new CancellationTokenSource();
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine((_, _) => throw Failure()), (_, _) =>
        { cts.Cancel(); return Task.FromResult<ICutEngine>(new Engine((_, _) => throw new Exception("must not run"))); }, (_, _) =>
        { if (stage == "confirm") cts.Cancel(); return Task.FromResult(true); });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => engine.RunAsync(request, cancellationToken: cts.Token));
        Assert.False(File.Exists(request.OutputFilePath)); Assert.False(engine.UsedMp4Box);
    }
    [Fact]
    public async Task UnrelatedFailureDoesNotOfferFallback()
    {
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine((_, _) => throw new IOException("video failed")),
            (_, _) => throw new Exception("no alternative"), (_, _) => throw new Exception("no question"));
        await Assert.ThrowsAsync<IOException>(() => engine.RunAsync(Request()));
    }
    [Fact]
    public async Task AvailabilityFailureDoesNotRetryOrTouchTarget()
    {
        var request = Request(true); File.WriteAllText(request.OutputFilePath, "existing"); var confirmations = 0;
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine((_, _) => throw Failure()),
            (_, _) => throw new FileNotFoundException("MP4Box missing"), (_, _) => { confirmations++; return Task.FromResult(true); });
        await Assert.ThrowsAsync<FileNotFoundException>(() => engine.RunAsync(request));
        Assert.Equal(1, confirmations); Assert.False(engine.UsedMp4Box); Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
    }
    [Fact]
    public async Task FailedAlternativeDoesNotLoopOrReportSuccess()
    {
        var count = 0;
        var alternative = new Engine((_, _) => throw new IOException("MP4Box failed"));
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine((_, _) => throw Failure()),
            (_, _) => Task.FromResult<ICutEngine>(alternative), (_, _) => { count++; return Task.FromResult(true); });
        await Assert.ThrowsAsync<IOException>(() => engine.RunAsync(Request()));
        Assert.Equal(1, count); Assert.Single(alternative.Requests); Assert.False(engine.UsedMp4Box);
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OtrCanCleansPartialOutputBeforeQuestionAndPreservesExistingTarget(bool selectedAac)
    {
        var request = Request(true); File.WriteAllText(request.OutputFilePath, "existing");
        string Tool(string name) { var path = Path.Combine(_root, name); File.WriteAllText(path, "tool"); return path; }
        var tools = new OtrCanToolPaths(Tool("engine.exe"), Tool("index.exe"), Tool("ffmpeg.exe"), Tool("ffprobe.exe"));
        var source = selectedAac ? Media() with { AudioStreams = [new(1, "aac", null, 48000, 2, "stereo"), new(2, "ac3", null, 48000, 2, "stereo")] } : Media();
        var native = new OtrCanCutService(tools, new FailingRunner(), (_, _) => Task.FromResult(source), retainedAudioStreamIndex: null);
        var questions = 0;
        var engine = new OtrCanMp4BoxFallbackCutService(native, (_, _) => throw new Exception("declined"), (failure, _) =>
        {
            questions++; Assert.Equal("source_coverage_gap", failure.Diagnostic!.Classification);
            Assert.Null(failure.Offer); // Packet gaps must not expand the existing AAC emergency policy.
            Assert.Contains("Innere Zeitlücke", failure.Message);
            Assert.Empty(Directory.GetFiles(_root, ".can-otr-*")); Assert.Empty(Directory.GetDirectories(_root));
            Assert.Equal("existing", File.ReadAllText(request.OutputFilePath)); return Task.FromResult(false);
        });
        await Assert.ThrowsAsync<OtrCanAudioFailureException>(() => engine.RunAsync(request)); Assert.Equal(1, questions);
    }
    private sealed class FailingRunner : ICutToolRunner
    {
        public Task RunAsync(string path, IReadOnlyList<string> args, IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            if (args[0] == "-c")
            {
                foreach (var suffix in new[] { "", "_track00.tc.txt", "_track00.kf.txt" })
                    File.WriteAllText(args[3] + suffix, suffix == "_track00.tc.txt" ? "# timecode format v2\n0\n40\n80\n" : "index");
                return Task.CompletedTask;
            }
            var a = args.ToArray(); string Value(string key) => a[Array.IndexOf(a, key) + 1];
            File.WriteAllText(Value("--output"), "partial");
            File.WriteAllText(Path.Combine(Value("--temp-dir"), "audio-diagnostic.json"), Json());
            throw new InvalidOperationException("[audio-diagnostic-v1] PCM count differs for audio stream 1: expected 48000, got 47488");
        }
    }
    internal static string Json() => """
        {"schema_version":1,"classification":"source_coverage_gap","stream_index":1,"codec":"aac","sample_rate":48000,
         "expected_samples":48000,"actual_samples":47488,"issues":[{"kind":"internal_gap","keep":1,"start_sample":2048,"end_sample":2560,"missing_samples":512}],"uncertainty":"historical cause unknown"}
        """;

    [Theory]
    [InlineData("mp4")]
    [InlineData("matroska")]
    public async Task ExistingMp4PreparationKeepsOriginalAndIntervalsAndCleansItsOwnCopy(string format)
    {
        var request = Request(); string? copy = null; var prepared = 0;
        var target = new Engine((r, _) =>
        {
            Assert.Equal(request.OriginalFilePath, r.OriginalFilePath); Assert.Equal(request.KeepSegments, r.KeepSegments);
            if (format == "mp4") Assert.Same(request, r); else Assert.Equal("copy", File.ReadAllText(r.SourceFilePath));
            return Task.CompletedTask;
        });
        var engine = new Mp4BoxFallbackPreparation(target, (_, _) => Task.FromResult(Media(format)), (input, output, _, _, _) =>
        { Assert.Equal(request.SourceFilePath, input); prepared++; copy = output; File.WriteAllText(output, "copy"); return Task.CompletedTask; });
        await engine.RunAsync(request);
        Assert.Equal(format == "mp4" ? 0 : 1, prepared); if (copy is not null) Assert.False(File.Exists(copy));
        Assert.Empty(Directory.GetDirectories(_root)); Assert.Equal("original", File.ReadAllText(request.SourceFilePath));
    }
    [Fact]
    public async Task FailedPreparationNeverStartsMp4BoxAndCleansPartialCopy()
    {
        var engine = new Mp4BoxFallbackPreparation(new Engine((_, _) => throw new Exception("must not run")),
            (_, _) => Task.FromResult(Media("matroska")), (_, output, _, _, _) =>
            { File.WriteAllText(output, "partial"); throw new InvalidDataException("bad copy"); });
        await Assert.ThrowsAsync<InvalidDataException>(() => engine.RunAsync(Request())); Assert.Empty(Directory.GetDirectories(_root));
    }
    private sealed class Probe(int exit, string output) : IToolProbeRunner
    {
        public Task<ToolProbeResult> RunAsync(string executablePath, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
        { Assert.Equal(["-version"], arguments); return Task.FromResult(new ToolProbeResult(exit, output, "")); }
    }
    [Theory]
    [InlineData(0, "MP4Box - GPAC version 26.07", true)]
    [InlineData(1, "MP4Box", false)]
    [InlineData(0, "unrelated executable", false)]
    public async Task ConfiguredMp4BoxMustStartAndIdentifyItselfBeforeRunnerCreation(int exit, string output, bool valid)
    {
        var path = Path.Combine(_root, "MP4Box.exe"); File.WriteAllText(path, "tool"); var created = false;
        var factory = new ConfiguredMp4BoxCutServiceFactory(() => new CutApplicationSettings { ExecutablePath = path },
            _ => { created = true; return new CopyRunner(); });
        if (valid) { Assert.IsType<Mp4BoxCutService>(await factory.CreateCheckedAsync(null, default, new Probe(exit, output))); Assert.True(created); }
        else { await Assert.ThrowsAsync<InvalidOperationException>(() => factory.CreateCheckedAsync(null, default, new Probe(exit, output))); Assert.False(created); }
    }
    [Fact]
    public async Task MissingMp4BoxNeverConstructsRunner()
    {
        var factory = new ConfiguredMp4BoxCutServiceFactory(() => new CutApplicationSettings { ExecutablePath = Path.Combine(_root, "missing.exe") },
            _ => throw new Exception("must not construct"));
        await Assert.ThrowsAsync<FileNotFoundException>(() => factory.CreateCheckedAsync(null, default));
    }
    private sealed class CopyRunner(bool fail = false) : IMp4BoxRunner
    {
        public Task RunSplitAsync(string sourceFilePath, string outputFilePath, Mp4BoxSplitRange range, CancellationToken cancellationToken = default)
        { File.WriteAllText(outputFilePath, "part"); if (fail) throw new IOException("MP4Box split failed"); return Task.CompletedTask; }
        public Task RunConcatAsync(IReadOnlyList<string> paths, string output, CancellationToken token = default) => throw new NotSupportedException();
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExistingMp4BoxWorkflowProtectsTargetAndCleansPartialFiles(bool lateTarget)
    {
        var request = Request(overwrite: !lateTarget); var cutlist = OtrCanCutService.BuildCutlist(request);
        if (!lateTarget) File.WriteAllText(request.OutputFilePath, "existing");
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine((_, _) => throw Failure()),
            (_, _) => Task.FromResult<ICutEngine>(new Mp4BoxCutService(new CopyRunner(fail: true))), (_, _) =>
            { if (lateTarget) File.WriteAllText(request.OutputFilePath, "existing"); return Task.FromResult(true); });
        await Assert.ThrowsAsync<IOException>(() => engine.RunAsync(request));
        Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
        Assert.Empty(Directory.GetFiles(_root, ".cut-assistant-next-*"));
        Assert.Equal(cutlist, OtrCanCutService.BuildCutlist(request)); Assert.False(engine.UsedMp4Box);
    }
    [Fact]
    public async Task LegacyAacOfferMayBeDeclinedBeforeSeparateMp4BoxConsent()
    {
        var media = Media() with { AudioStreams = [Media().AudioStreams[0], new(2, "ac3", null, 48000, 2, "stereo")] };
        var legacy = new OtrCanAudioFailureException(media, media, new InvalidOperationException("PCM assembly failed: AC3 decoder error"));
        var asked = new List<string>();
        var existing = new OtrCanAacEmergencyCutService(new Engine((_, _) => throw legacy),
            _ => throw new Exception("declined"), new UnusedPreparation(), (_, _) =>
            { asked.Add("AAC"); return Task.FromResult<AacEmergencyChoice?>(null); });
        var engine = new OtrCanMp4BoxFallbackCutService(existing,
            (_, _) => Task.FromResult<ICutEngine>(new Engine((_, _) => Task.CompletedTask)), (_, _) =>
            { asked.Add("MP4Box"); return Task.FromResult(true); });
        await engine.RunAsync(Request()); Assert.Equal(["AAC", "MP4Box"], asked); Assert.True(engine.UsedMp4Box);
    }
    private sealed class UnusedPreparation : IAacEmergencySource
    {
        public Task<string> PrepareAsync(CutRequest request, AacEmergencyOffer offer, AacEmergencyChoice choice,
            string work, IProgress<CutProgressUpdate>? progress, CancellationToken token) => throw new Exception("declined");
    }
    [Fact]
    public async Task FailedAacPreparationStillOffersMp4BoxAfterItsOwnCleanup()
    {
        var media = Media() with { AudioStreams = [Media().AudioStreams[0], new(2, "ac3", null, 48000, 2, "stereo")] };
        var existing = new OtrCanAacEmergencyCutService(new Engine((_, _) => throw new OtrCanAudioFailureException(
            media, media, new InvalidOperationException("PCM assembly failed: decoder error"))),
            _ => throw new Exception("preparation fails"), new FailedAacPreparation(),
            (offer, _) => Task.FromResult<AacEmergencyChoice?>(offer.Choices.Single()));
        var engine = new OtrCanMp4BoxFallbackCutService(existing,
            (_, _) => Task.FromResult<ICutEngine>(new Engine((_, _) => Task.CompletedTask)), (failure, _) =>
            { Assert.Null(failure.Offer); Assert.Empty(Directory.GetDirectories(_root)); return Task.FromResult(true); });
        await engine.RunAsync(Request()); Assert.True(engine.UsedMp4Box);
    }
    private sealed class FailedAacPreparation : IAacEmergencySource
    {
        public Task<string> PrepareAsync(CutRequest request, AacEmergencyOffer offer, AacEmergencyChoice choice,
            string work, IProgress<CutProgressUpdate>? progress, CancellationToken token)
        { File.WriteAllText(Path.Combine(work, "partial.mp4"), "partial"); throw new InvalidDataException("AAC cannot be decoded"); }
    }
}
