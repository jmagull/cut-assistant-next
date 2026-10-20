using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxOutputAnalysisWorkflowTests : IDisposable
{
    private readonly string _work = Path.Combine(Path.GetTempPath(), "can-output-workflow-" + Guid.NewGuid().ToString("N"));
    public Mp4BoxOutputAnalysisWorkflowTests() => Directory.CreateDirectory(_work);
    public void Dispose() => Directory.Delete(_work, recursive: true);

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public async Task MeasuresBeforePublicationAndCleanupAndKeepsFileIdentity(int count)
    {
        string? measuredPath = null;
        string[]? segments = null;
        var output = Path.Combine(_work, "output.mp4");
        var analyzer = new Analyzer(async (path, parts, token) =>
        {
            measuredPath = path; segments = parts.ToArray();
            Assert.False(File.Exists(output));
            Assert.All(parts, p => Assert.True(File.Exists(p)));
            return new CutOutputTimeline(TimeSpan.FromSeconds(100),
                Enumerable.Range(1, count - 1).Select(i => TimeSpan.FromSeconds(i * 10)),
                await CutOutputIdentityVerifier.ReadAsync(path, token));
        });
        var result = await new Mp4BoxCutWorkflow(new Runner()).RunWithAnalysisAsync("source.mp4", output, Ranges(count), null, analyzer: analyzer);
        Assert.NotNull(result.Timeline);
        Assert.Null(result.AnalysisWarning);
        Assert.True(File.Exists(output));
        Assert.False(File.Exists(measuredPath));
        Assert.All(segments!, p => Assert.False(File.Exists(p)));
        await CutOutputIdentityVerifier.ValidateAsync(output, result.Timeline.Identity!, true);
    }

    [Theory]
    [InlineData("error")]
    [InlineData("cancel")]
    [InlineData("invalid-result")]
    public async Task FailedOrCanceledSupplementaryAnalysisDoesNotLoseSuccessfulCut(string mode)
    {
        using var cancel = new CancellationTokenSource();
        var output = Path.Combine(_work, "output.mp4");
        var analyzer = new Analyzer((_, _, _) =>
        {
            if (mode == "cancel") { cancel.Cancel(); throw new OperationCanceledException(cancel.Token); }
            if (mode == "invalid-result") return Task.FromResult(new CutOutputTimeline(TimeSpan.FromSeconds(10), []));
            throw new InvalidDataException("analysis-failed");
        });
        var result = await new Mp4BoxCutWorkflow(new Runner()).RunWithAnalysisAsync("source.mp4", output, Ranges(2), null, cancel.Token, analyzer: analyzer);
        Assert.Null(result.Timeline);
        Assert.Contains("berechnet", result.AnalysisWarning);
        Assert.Equal("complete", File.ReadAllText(output));
        Assert.Single(Directory.GetFiles(_work));
    }

    private static Mp4BoxSplitRange[] Ranges(int count) => Enumerable.Range(0, count).Select(i =>
        new Mp4BoxSplitRange(TimeSpan.FromSeconds(i * 20), TimeSpan.FromSeconds(i * 20 + 10))).ToArray();
    private sealed class Analyzer(Func<string, IReadOnlyList<string>, CancellationToken, Task<CutOutputTimeline>> run) : ICutOutputAnalyzer
    {
        public Task<CutOutputTimeline> AnalyzeAsync(string path, IReadOnlyList<string> parts, IProgress<CutProgressUpdate>? progress, CancellationToken token) => run(path, parts, token);
    }
    private sealed class Runner : IMp4BoxRunner
    {
        public Task RunSplitAsync(string source, string output, Mp4BoxSplitRange range, CancellationToken token = default)
        { token.ThrowIfCancellationRequested(); File.WriteAllText(output, "segment"); return Task.CompletedTask; }
        public Task RunConcatAsync(IReadOnlyList<string> parts, string output, CancellationToken token = default)
        { token.ThrowIfCancellationRequested(); File.WriteAllText(output, "complete"); return Task.CompletedTask; }
    }
}
