using System.Runtime.CompilerServices;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class CutOutputAnalyzerTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "can-output-analysis-" + Guid.NewGuid().ToString("N"));
    private readonly string[] _paths;
    public CutOutputAnalyzerTests()
    {
        Directory.CreateDirectory(_directory);
        _paths = Enumerable.Range(0, 4).Select(i => Path.Combine(_directory, $"{i}.bin")).ToArray();
        foreach (var p in _paths) File.WriteAllText(p, "fixture");
    }
    public void Dispose() => Directory.Delete(_directory, recursive: true);

    private Probe Fixture()
    {
        var p = new Probe();
        p.Data[_paths[0]] = [P(0, "a", true), P(40, "b"), P(80, "c"), P(80, "d", true), P(120, "e"), P(300, "f", true), P(350, "g")];
        p.Data[_paths[1]] = [P(0, "a", true), P(40, "b"), P(80, "c")];
        p.Data[_paths[2]] = [P(0, "d", true), P(100, "e")];
        p.Data[_paths[3]] = [P(0, "f", true), P(50, "g")];
        return p;
    }

    [Fact]
    public async Task UsesActualRationalPtsAcrossDifferentDurationsAndDuplicateBoundary()
    {
        var timeline = await new FfprobeCutOutputAnalyzer(Fixture()).AnalyzeAsync(_paths[0], _paths[1..], null, default);
        Assert.Equal([TimeSpan.FromMilliseconds(80), TimeSpan.FromMilliseconds(300)], timeline.Transitions);
        Assert.Equal(TimeSpan.FromMilliseconds(800), timeline.Duration);
        Assert.NotNull(timeline.Identity);
        await CutOutputIdentityVerifier.ValidateAsync(_paths[0], timeline.Identity, true);
    }

    public static TheoryData<string, Type> Faults => new()
    {
        { "changed", typeof(InvalidDataException) }, { "truncated", typeof(InvalidDataException) },
        { "extra", typeof(InvalidDataException) }, { "empty", typeof(NotSupportedException) },
        { "reordered", typeof(InvalidDataException) }, { "no-decode", typeof(InvalidDataException) },
        { "decode-pts", typeof(InvalidDataException) }, { "negative", typeof(NotSupportedException) },
        { "discard", typeof(NotSupportedException) }, { "corrupt", typeof(NotSupportedException) },
        { "no-key", typeof(NotSupportedException) }, { "preroll", typeof(NotSupportedException) },
        { "overlap", typeof(NotSupportedException) }, { "past-end", typeof(NotSupportedException) },
        { "origin", typeof(NotSupportedException) }, { "changed-file", typeof(IOException) }
    };

    [Theory]
    [MemberData(nameof(Faults))]
    public async Task RejectsUnconfirmedOrChangedTimelines(string fault, Type exceptionType)
    {
        var p = Fixture();
        switch (fault)
        {
            case "changed": p.Data[_paths[0]][1] = P(40, "changed"); break;
            case "truncated": p.Data[_paths[0]] = p.Data[_paths[0]][..^1]; break;
            case "extra": p.Data[_paths[0]] = [.. p.Data[_paths[0]], P(390, "extra")]; break;
            case "empty": p.Data[_paths[2]] = []; break;
            case "reordered": (p.Data[_paths[1]], p.Data[_paths[2]]) = (p.Data[_paths[2]], p.Data[_paths[1]]); break;
            case "no-decode": p.OmitDecoded = true; break;
            case "decode-pts": p.ShiftDecoded = true; break;
            case "negative": p.Data[_paths[0]][0] = P(-1, "a", true); break;
            case "discard": p.Data[_paths[0]][3] = P(80, "d", true) with { Flags = "KD" }; break;
            case "corrupt": p.Data[_paths[0]][4] = P(120, "e") with { Flags = "C" }; break;
            case "no-key": p.Data[_paths[0]][3] = P(80, "d"); break;
            case "preroll": p.Data[_paths[0]][4] = P(60, "e"); break;
            case "overlap": p.Data[_paths[0]][2] = P(100, "c"); break;
            case "past-end": p.Data[_paths[0]][5] = P(900, "f", true); break;
            case "origin": p.Data[_paths[0]][0] = P(1, "a", true); break;
            case "changed-file": p.OnDecode = () => File.AppendAllText(_paths[0], "changed"); break;
        }
        await Assert.ThrowsAsync(exceptionType, async () => { await new FfprobeCutOutputAnalyzer(p).AnalyzeAsync(_paths[0], _paths[1..], null, default); });
    }

    [Fact]
    public async Task SingleSegmentCanBeItsOwnPendingOutputIncludingClippedTail()
    {
        var p = Fixture();
        p.Data[_paths[1]] = [.. p.Data[_paths[1]], P(800, "tail") with { Flags = "D" }];
        var result = await new FfprobeCutOutputAnalyzer(p).AnalyzeAsync(_paths[1], [_paths[1]], null, default);
        Assert.Empty(result.Transitions);
        Assert.NotNull(result.Identity);
    }

    [Fact]
    public async Task CancelsAndRejectsAliasedOrEmptyInputs()
    {
        var analyzer = new FfprobeCutOutputAnalyzer(Fixture());
        using var canceled = new CancellationTokenSource();
        canceled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => analyzer.AnalyzeAsync(_paths[0], _paths[1..], null, canceled.Token));
        await Assert.ThrowsAsync<ArgumentException>(() => analyzer.AnalyzeAsync(_paths[0], [], null, default));
        await Assert.ThrowsAsync<ArgumentException>(() => analyzer.AnalyzeAsync(_paths[0], [_paths[1], _paths[1]], null, default));
    }

    [Fact]
    public async Task IdentityDetectsSameSizeAndRestoredTimestampChange()
    {
        var identity = await CutOutputIdentityVerifier.ReadAsync(_paths[0]);
        var time = File.GetLastWriteTimeUtc(_paths[0]);
        File.WriteAllText(_paths[0], "changed");
        File.SetLastWriteTimeUtc(_paths[0], time);
        await Assert.ThrowsAsync<IOException>(() => CutOutputIdentityVerifier.ValidateAsync(_paths[0], identity, true));
    }

    [Theory]
    [InlineData("1/0")]
    [InlineData("0/1")]
    [InlineData("1")]
    [InlineData("-1/25")]
    public void RejectsInvalidRationalTimebase(string text) => Assert.Throws<InvalidDataException>(() => TimeBase.Parse(text));

    [Fact]
    public void PreservesFractionalTiming() => Assert.Equal(1.001m, new TimeBase(1001, 30000).Seconds(30));

    private static Packet P(long pts, string hash, bool key = false) => new(pts, hash, key ? "K" : "_", pts + ":" + hash);
    private sealed class Probe : IVideoProbe
    {
        public Dictionary<string, Packet[]> Data { get; } = [];
        public bool OmitDecoded { get; set; }
        public bool ShiftDecoded { get; set; }
        public Action? OnDecode { get; set; }
        public Task<VideoInfo> InspectAsync(string path, CancellationToken token)
        { token.ThrowIfCancellationRequested(); return Task.FromResult(new VideoInfo(new(1, 1000), 0, .8m)); }
        public async IAsyncEnumerable<Packet> ReadPacketsAsync(string path, [EnumeratorCancellation] CancellationToken token)
        { await Task.CompletedTask; foreach (var p in Data[path]) { token.ThrowIfCancellationRequested(); yield return p; } }
        public Task<IReadOnlyList<DecodedFrame>> DecodeAsync(string path, decimal start, decimal end, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); OnDecode?.Invoke();
            IReadOnlyList<DecodedFrame> frames = OmitDecoded ? [] : Data[path].Select(p => new DecodedFrame(p.Pts + (ShiftDecoded ? 1 : 0), p.Position)).ToArray();
            return Task.FromResult(frames);
        }
    }
}
