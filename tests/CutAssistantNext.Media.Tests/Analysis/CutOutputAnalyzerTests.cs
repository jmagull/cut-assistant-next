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

    private Probe SmallOverlapFixture(long step = 40, long overlap = 8)
    {
        var p = Fixture();
        p.CodecName = "h264";
        p.Data[_paths[0]] = [P(0, "a", true), P(step, "b"), P(2 * step, "c"),
            P(2 * step - overlap, "d", true), P(3 * step - overlap, "e"), P(4 * step - overlap, "h"), P(300, "f", true), P(350, "g")];
        p.Data[_paths[0]] = p.Data[_paths[0]].Select((packet, i) => packet with
        {
            Dts = (i - 2) * step,
            Duration = i == 2 ? step - overlap : i == 3 ? overlap : step
        }).ToArray();
        p.Data[_paths[2]] = [P(0, "d", true), P(100, "e"), P(200, "h")];
        return p;
    }

    [Theory]
    [InlineData(40, 8)]
    [InlineData(40, 1)]
    [InlineData(40, 39)]
    [InlineData(20, 4)]
    public async Task LocatesOnlyTheBoundedH264OverlapWithOrderedDtsAndDecodedNeighbors(long step, long overlap)
    {
        var timeline = await new FfprobeCutOutputAnalyzer(SmallOverlapFixture(step, overlap))
            .AnalyzeAsync(_paths[0], _paths[1..], null, default);
        Assert.True(timeline.HasOverlappingPresentationTimes);
        Assert.Equal(TimeSpan.FromMilliseconds(2 * step - overlap), timeline.Transitions[0]);
        Assert.Equal(TimeSpan.FromMilliseconds(300), timeline.Transitions[1]);
        Assert.NotNull(timeline.Identity);
    }

    [Theory]
    [InlineData("codec")]
    [InlineData("missing-dts")]
    [InlineData("backwards-dts")]
    [InlineData("missing-duration")]
    [InlineData("old-duration")]
    [InlineData("new-duration")]
    [InlineData("large-overlap")]
    [InlineData("spacing")]
    [InlineData("two-old-times")]
    [InlineData("old-not-last")]
    [InlineData("duplicate-old")]
    [InlineData("duplicate-new")]
    public async Task RejectsUnsupportedOverlapShapesWithoutRelaxingOldGuards(string fault)
    {
        var p = SmallOverlapFixture();
        var output = p.Data[_paths[0]];
        switch (fault)
        {
            case "codec": p.CodecName = "hevc"; break;
            case "missing-dts": output[0] = output[0] with { Dts = null }; break;
            case "backwards-dts": output[3] = output[3] with { Dts = -1 }; break;
            case "missing-duration": output[3] = output[3] with { Duration = null }; break;
            case "old-duration": output[2] = output[2] with { Duration = 31 }; break;
            case "new-duration": output[3] = output[3] with { Duration = 7 }; break;
            case "large-overlap": output[3] = output[3] with { Pts = 40 }; break;
            case "spacing": output[5] = output[5] with { Pts = 153 }; break;
            case "two-old-times": output[1] = output[1] with { Pts = 75 }; break;
            case "old-not-last":
                (output[1], output[2]) = (output[2], output[1]);
                (p.Data[_paths[1]][1], p.Data[_paths[1]][2]) = (p.Data[_paths[1]][2], p.Data[_paths[1]][1]);
                break;
            case "duplicate-old":
                p.Data[_paths[0]] = [.. output.Take(3), P(80, "c2") with { Dts = 20, Duration = 32 }, .. output.Skip(3)];
                p.Data[_paths[1]] = [.. p.Data[_paths[1]], P(0, "c2")];
                break;
            case "duplicate-new":
                p.Data[_paths[0]] = [.. output.Take(4), P(72, "d2") with { Dts = 60, Duration = 8 }, .. output.Skip(4)];
                p.Data[_paths[2]] = [p.Data[_paths[2]][0], P(0, "d2"), .. p.Data[_paths[2]].Skip(1)];
                break;
        }
        await Assert.ThrowsAsync<NotSupportedException>(() => new FfprobeCutOutputAnalyzer(p).AnalyzeAsync(_paths[0], _paths[1..], null, default));
    }

    [Theory]
    [InlineData("missing-old")]
    [InlineData("missing-new")]
    [InlineData("wrong-order")]
    [InlineData("extra-backstep")]
    [InlineData("equal-time")]
    [InlineData("duplicate-anchor")]
    public async Task RejectsAmbiguousDecodedOverlapRatherThanClaimingMeasuredPositions(string fault)
    {
        var p = SmallOverlapFixture();
        var frames = p.Data[_paths[0]].Select(packet => new DecodedFrame(packet.Pts, packet.Position)).ToList();
        switch (fault)
        {
            case "missing-old": frames.RemoveAt(2); break;
            case "missing-new": frames.RemoveAt(3); break;
            case "wrong-order": (frames[4], frames[5]) = (frames[5], frames[4]); break;
            case "extra-backstep": frames[^1] = frames[^1] with { Pts = 299 }; break;
            case "equal-time": frames[^1] = frames[^1] with { Pts = 300 }; break;
            case "duplicate-anchor": frames.Insert(4, frames[3]); break;
        }
        p.DecodedOverride = frames;
        await Assert.ThrowsAsync<InvalidDataException>(() => new FfprobeCutOutputAnalyzer(p).AnalyzeAsync(_paths[0], _paths[1..], null, default));
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
        public string? CodecName { get; set; }
        public IReadOnlyList<DecodedFrame>? DecodedOverride { get; set; }
        public Task<VideoInfo> InspectAsync(string path, CancellationToken token)
        { token.ThrowIfCancellationRequested(); return Task.FromResult(new VideoInfo(new(1, 1000), 0, .8m, CodecName)); }
        public async IAsyncEnumerable<Packet> ReadPacketsAsync(string path, [EnumeratorCancellation] CancellationToken token)
        { await Task.CompletedTask; foreach (var p in Data[path]) { token.ThrowIfCancellationRequested(); yield return p; } }
        public Task<IReadOnlyList<DecodedFrame>> DecodeAsync(string path, decimal start, decimal end, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); OnDecode?.Invoke();
            IReadOnlyList<DecodedFrame> frames = DecodedOverride ?? (OmitDecoded ? [] : Data[path].Select(p => new DecodedFrame(p.Pts + (ShiftDecoded ? 1 : 0), p.Position)).ToArray());
            return Task.FromResult(frames);
        }
    }
}
