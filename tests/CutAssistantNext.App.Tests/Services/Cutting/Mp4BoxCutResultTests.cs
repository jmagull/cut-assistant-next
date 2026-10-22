using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class Mp4BoxCutResultTests : IDisposable
{
    private readonly string _work = Path.Combine(Path.GetTempPath(), "can-cut-result-" + Guid.NewGuid().ToString("N"));
    public Mp4BoxCutResultTests() => Directory.CreateDirectory(_work);
    public void Dispose() => Directory.Delete(_work, recursive: true);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EngineHandsMeasuredOrExplicitlyEstimatedResultToCompletion(bool fail)
    {
        var output = Path.Combine(_work, "out.mp4");
        var request = new CutRequest(Path.Combine(_work, "in.mp4"), output,
            [new(TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(40)),
             new(TimeSpan.FromSeconds(70),TimeSpan.FromSeconds(30))], 25);
        ICutEngine engine = new Mp4BoxCutService(new Runner(), new Analyzer(fail));
        var result = await engine.RunWithResultAsync(request);
        Assert.Equal(fail ? CutReviewTimingKind.Estimated : CutReviewTimingKind.Measured, result.TimingKind);
        Assert.Equal(TimeSpan.FromSeconds(fail ? 40 : 37.5), Assert.Single(result.Timeline.Transitions));
        Assert.Equal("complete", File.ReadAllText(output));
        Assert.Contains("fehlende Bilder", result.Notice);
        if (fail) Assert.Contains("nur aus dem Schnittplan", result.Notice);
        else await CutOutputIdentityVerifier.ValidateAsync(output, result.Timeline.Identity!, true);
    }

    [Fact]
    public async Task EnginePreservesLocatedOutputTimesAndExplainsOverlappingPresentation()
    {
        var output = Path.Combine(_work, "out.mp4");
        ICutEngine engine = new Mp4BoxCutService(new Runner(), new Analyzer(false, overlap: true));
        var request = new CutRequest(Path.Combine(_work, "in.mp4"), output,
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(40)), new(TimeSpan.FromSeconds(70), TimeSpan.FromSeconds(30))], 25);
        var result = await engine.RunWithResultAsync(request);
        Assert.Equal(CutReviewTimingKind.Located, result.TimingKind);
        Assert.Equal(TimeSpan.FromSeconds(37.5), Assert.Single(result.Timeline.Transitions));
        Assert.Contains("überlappen", result.Notice);
        Assert.DoesNotContain("aus dem Schnittplan", result.Notice);
        await CutOutputIdentityVerifier.ValidateAsync(output, result.Timeline.Identity!, true);
    }

    private sealed class Analyzer(bool fail, bool overlap = false) : ICutOutputAnalyzer
    {
        public async Task<CutOutputTimeline> AnalyzeAsync(string path, IReadOnlyList<string> segments,
            IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            if (fail) throw new InvalidDataException("Not measured");
            return new(TimeSpan.FromSeconds(67.5), [TimeSpan.FromSeconds(37.5)],
                await CutOutputIdentityVerifier.ReadAsync(path, token), overlap);
        }
    }
    private sealed class Runner : IMp4BoxRunner
    {
        public Task RunSplitAsync(string source, string output, Mp4BoxSplitRange range, CancellationToken token = default)
        { File.WriteAllText(output, "segment"); return Task.CompletedTask; }
        public Task RunConcatAsync(IReadOnlyList<string> parts, string output, CancellationToken token = default)
        { File.WriteAllText(output, "complete"); return Task.CompletedTask; }
    }
}
