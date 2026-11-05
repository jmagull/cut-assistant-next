using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class AudioRecoveryDecisionTests
{
    private static CutRequest Request() => new(Path.GetFullPath("unchanged-source.mp4"), Path.GetFullPath("protected-output.mp4"), [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))], 25);
    private static OtrCanAudioFailureException Failure()
    {
        var media = new MediaAnalysisResult("mp4", "MP4", 10, TimeSpan.FromSeconds(2), [], [new(1, "aac", null, 48000, 2, "stereo")]);
        return new(media, media, new InvalidOperationException("[audio-diagnostic-v1] strict gap"));
    }
    private sealed class Engine(Func<CutRequest, Task> run) : ICutEngine
    {
        public Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null, CancellationToken cancellationToken = default) => run(request);
    }
    private sealed class Repair(Func<CutRequest, AacGapRepairOffer> offer) : IAacGapRepair
    {
        public Task<AacGapRepairOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure, IProgress<CutProgressUpdate>? progress, CancellationToken token) => Task.FromResult(offer(request));
    }
    private static Task<ICutEngine> NoFallback(IProgress<CutProgressUpdate>? _, CancellationToken __) => throw new Exception("MP4Box was not chosen");
    private static Task<(bool, string?)> Available(CancellationToken _) => Task.FromResult<(bool, string?)>((true, null));
    [Fact]
    public async Task HealthyNormalCutDoesNotInspectRepairOrFallback()
    {
        var r = Request(); var engine = new OtrCanMp4BoxFallbackCutService(new Engine(x => { Assert.Same(r, x); return Task.CompletedTask; }), NoFallback,
         (_, _) => throw new Exception("decision unexpectedly requested"), new Repair(_ => throw new Exception("analyzed healthy cut")), _ => throw new Exception("checked MP4Box"));
        await engine.RunAsync(r);
        Assert.False(engine.UsedMp4Box); Assert.False(engine.UsedAacRepair);
    }
    [Fact]
    public async Task ExplicitRepairRunsOnceAfterNormalCleanupAndDisposesOffer()
    {
        var r = Request(); var phases = new List<string>();
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine(_ => { try { throw Failure(); } finally { phases.Add("normal-ended"); } }), NoFallback,
         (o, _) => { Assert.True(o.Repair.IsAvailable); phases.Add("confirmed"); return Task.FromResult(AudioRecoveryChoice.RepairAacGaps); },
         new Repair(_ => { phases.Add("analyzed"); return new("512", (x, _, _) => { Assert.Same(r, x); phases.Add("repair"); return Task.FromResult(CutResult.Planned(x.OutputFilePath, x.KeepSegments, AacGapRepairService.Notice)); }, () => { phases.Add("disposed"); return ValueTask.CompletedTask; }); }), Available);
        var result = await engine.RunWithResultAsync(r);
        Assert.Equal(["normal-ended", "analyzed", "confirmed", "repair", "disposed"], phases);
        Assert.True(engine.UsedAacRepair); Assert.False(engine.UsedMp4Box); Assert.Equal(AacGapRepairService.Notice, result.Notice);
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DefaultCancelNeverStartsAnotherEngine(bool cancelToken)
    {
        var r = Request(); var disposed = false; using var cts = new CancellationTokenSource();
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine(_ => throw Failure()), NoFallback,
         (_, _) => { if (cancelToken) cts.Cancel(); return Task.FromResult(cancelToken ? AudioRecoveryChoice.RepairAacGaps : AudioRecoveryChoice.Cancel); },
         new Repair(_ => new("512", (_, _, _) => throw new Exception("unconfirmed repair"), () => { disposed = true; return ValueTask.CompletedTask; })), Available);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => engine.RunAsync(r, cancellationToken: cts.Token));
        Assert.True(disposed); Assert.False(engine.UsedAacRepair); Assert.False(engine.UsedMp4Box);
    }
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FailedRepairCanOfferOnlyExplicitMp4BoxOrCancel(bool confirmMp4Box)
    {
        var r = Request(); var decisions = 0; var repairs = 0; var fallbacks = 0;
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine(_ => throw Failure()),
         (_, _) => { Assert.Equal(2, decisions); return Task.FromResult<ICutEngine>(new Engine(x => { Assert.Same(r, x); fallbacks++; return Task.CompletedTask; })); },
         (o, _) =>
         {
             if (++decisions == 1) { Assert.True(o.Repair.IsAvailable); return Task.FromResult(AudioRecoveryChoice.RepairAacGaps); }
             Assert.False(o.Repair.IsAvailable); Assert.Contains("independent PCM rejected", o.Diagnosis); Assert.Equal(0, fallbacks);
             return Task.FromResult(confirmMp4Box ? AudioRecoveryChoice.Mp4Box : AudioRecoveryChoice.Cancel);
         }, new Repair(_ => new("512", (_, _, _) => { repairs++; throw new InvalidDataException("independent PCM rejected"); })), Available);
        if (confirmMp4Box) Assert.Contains("MP4Box-Fallback:", (await engine.RunWithResultAsync(r)).Notice);
        else await Assert.ThrowsAsync<OperationCanceledException>(() => engine.RunAsync(r));
        Assert.Equal(1, repairs); Assert.Equal(2, decisions); Assert.Equal(confirmMp4Box ? 1 : 0, fallbacks);
        Assert.Equal(confirmMp4Box, engine.UsedMp4Box); Assert.False(engine.UsedAacRepair);
    }
    [Fact]
    public async Task CancelledRepairDoesNotOfferAutomaticFallback()
    {
        var decisions = 0; var engine = new OtrCanMp4BoxFallbackCutService(new Engine(_ => throw Failure()), NoFallback,
         (_, _) => { decisions++; return Task.FromResult(AudioRecoveryChoice.RepairAacGaps); },
         new Repair(_ => new("512", (_, _, _) => throw new OperationCanceledException())), Available);
        await Assert.ThrowsAsync<OperationCanceledException>(() => engine.RunAsync(Request()));
        Assert.Equal(1, decisions);
    }
    [Theory]
    [InlineData(2)]
    [InlineData(1)]
    public async Task AChoiceNotActuallyAvailableCannotStartAProcess(int choice)
    {
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine(_ => throw Failure()), NoFallback,
         (o, _) => { Assert.False(o.Repair.IsAvailable); Assert.False(o.Mp4BoxAvailable); return Task.FromResult((AudioRecoveryChoice)choice); },
         new Repair(_ => new("unsupported tail")), _ => Task.FromResult<(bool, string?)>((false, "missing tool")));
        await Assert.ThrowsAsync<InvalidOperationException>(() => engine.RunAsync(Request()));
    }
}
