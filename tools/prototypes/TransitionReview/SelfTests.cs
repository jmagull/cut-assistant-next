using System.Runtime.CompilerServices;

namespace TransitionReview;

internal static class SelfTests
{
    public static async Task<int> RunAsync(CancellationToken token)
    {
        var checks = new List<string>();
        var root = Path.Combine(Path.GetTempPath(), "can-transition-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        // These fixtures contain no media; the probe boundary is replaced with explicit timing evidence.
        var paths = Enumerable.Range(0, 4).Select(i => Path.Combine(root, $"fixture-{i}.bin")).ToArray();
        foreach (var p in paths) await File.WriteAllTextAsync(p, "fixture", token);
        var segments = paths[1..];
        try
        {
            FakeProbe Fixture()
            {
                var probe = new FakeProbe();
                probe.Data[paths[0]] = [P(0, "a", true), P(40, "b"), P(80, "c"),
                    P(80, "d", true), P(120, "e"), P(300, "f", true), P(350, "g")];
                probe.Data[paths[1]] = [P(0, "a", true), P(40, "b"), P(80, "c")];
                probe.Data[paths[2]] = [P(0, "d", true), P(100, "e")];
                probe.Data[paths[3]] = [P(0, "f", true), P(50, "g")];
                return probe;
            }
            async Task<AnalysisResult> Analyze(FakeProbe p) => await new TransitionAnalyzer(p).AnalyzeAsync(paths[0], segments, token);
            void Check(bool value, string label)
            {
                if (!value) throw new Exception("FAIL " + label);
                checks.Add(label);
            }
            async Task Reject(string label, Action<FakeProbe> change)
            {
                var p = Fixture(); change(p);
                try { await Analyze(p); }
                catch (Exception e) when (e is InvalidDataException or NotSupportedException or IOException)
                { checks.Add(label); return; }
                throw new Exception("Expected rejection: " + label);
            }
            var success = await Analyze(Fixture());
            Check(success.Transitions.Select(t => t.Seconds).SequenceEqual([.080m, .300m]), "Nonuniform PTS, cumulative mixed lengths, independent of durations/fps");
            Check(success.Transitions[0].DuplicatePtsAtBoundary && !success.Transitions[1].DuplicatePtsAtBoundary, "Duplicate boundary PTS remains explicit");
            Check(success.Transitions.Select(t => t.FirstPacketOrdinal).SequenceEqual([3L, 5L]), "Complete ordered packet membership determines boundaries");
            await TransitionAnalyzer.ValidateForReviewAsync(success, token);
            checks.Add("Measured result can be reopened against matching output hash");
            await Reject("Changed middle packet", p => p.Data[paths[0]][1] = P(40, "changed"));
            await Reject("Truncated output", p => p.Data[paths[0]] = p.Data[paths[0]][..^1]);
            await Reject("Extra output packets", p => p.Data[paths[0]] = [.. p.Data[paths[0]], P(390, "extra")]);
            await Reject("Empty segment", p => p.Data[paths[2]] = []);
            await Reject("Reordered segment fingerprints", p => (p.Data[paths[1]], p.Data[paths[2]]) = (p.Data[paths[2]], p.Data[paths[1]]));
            await Reject("Packet present but decoder anchor absent", p => p.OmitDecoded = true);
            await Reject("Decoder anchor has different PTS", p => p.ShiftDecoded = true);
            await Reject("Negative PTS", p => p.Data[paths[0]][0] = P(-1, "a", true));
            await Reject("Discarded anchor", p => p.Data[paths[0]][3] = P(80, "d", true) with { Flags = "KD" });
            await Reject("Corrupt packet", p => p.Data[paths[0]][4] = P(120, "e") with { Flags = "C" });
            await Reject("No independent first keyframe", p => p.Data[paths[0]][3] = P(80, "d"));
            await Reject("Leading presentation preroll", p => p.Data[paths[0]][4] = P(60, "e"));
            await Reject("Overlapping segment presentation", p => p.Data[paths[0]][2] = P(100, "c"));
            await Reject("Anchor beyond reported duration", p => p.Data[paths[0]][5] = P(900, "f", true));
            await Reject("Nonzero video origin", p => p.Data[paths[0]][0] = P(1, "a", true));
            await Reject("File changes during decoding", p => p.OnDecode = () => File.AppendAllText(paths[0], "change"));
            await File.WriteAllTextAsync(paths[0], "fixture", token);
            try
            {
                await new TransitionAnalyzer(Fixture()).AnalyzeAsync(paths[0], [], token);
                throw new Exception("Empty input accepted");
            }
            catch (ArgumentException) { checks.Add("No segments rejected"); }
            foreach (var bad in new[] { new[] { paths[0] }, new[] { paths[1], paths[1] } })
            {
                try { await new TransitionAnalyzer(Fixture()).AnalyzeAsync(paths[0], bad, token); throw new Exception("Duplicate file accepted"); }
                catch (ArgumentException) { checks.Add("Aliased input paths rejected"); }
            }
            using (var canceled = new CancellationTokenSource())
            {
                canceled.Cancel();
                try { await new TransitionAnalyzer(Fixture()).AnalyzeAsync(paths[0], segments, canceled.Token); throw new Exception("Cancellation ignored"); }
                catch (OperationCanceledException) { checks.Add("Cancellation stops analysis"); }
            }
            var single = Fixture(); single.Data[paths[0]] = single.Data[paths[1]];
            var one = await new TransitionAnalyzer(single).AnalyzeAsync(paths[0], [paths[1]], token);
            Check(one.Transitions.Count == 0, "Single segment has no phantom transition");
            single.Data[paths[1]] = [.. single.Data[paths[1]], P(800, "clipped") with { Flags = "D" }];
            single.Data[paths[0]] = single.Data[paths[1]];
            var clipped = await new TransitionAnalyzer(single).AnalyzeAsync(paths[0], [paths[1]], token);
            Check(clipped.Segments[0].PacketCount == 4 && clipped.Transitions.Count == 0 && clipped.Notices.Count == 2,
                "Discarded reordered sample at EOF is verified as bytes without becoming a visible boundary");
            foreach (var text in new[] { "1/0", "0/1", "1", "-1/25" })
            {
                try { _ = TimeBase.Parse(text); throw new Exception("Invalid timebase accepted"); }
                catch (InvalidDataException) { checks.Add("Invalid timebase " + text); }
            }
            Check(new TimeBase(1001, 30000).Seconds(30) == 1.001m, "Rational timebase preserves noninteger frame timing");
            var fresh = await Analyze(Fixture());
            var time = File.GetLastWriteTimeUtc(paths[0]);
            await File.WriteAllTextAsync(paths[0], "changed", token); // same size, timestamp restored: hash must catch it
            File.SetLastWriteTimeUtc(paths[0], time);
            try { await TransitionAnalyzer.ValidateForReviewAsync(fresh, token); throw new Exception("Stale output accepted"); }
            catch (IOException) { checks.Add("Same-size same-mtime changed output rejected by SHA-256"); }
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { Passed = true, Count = checks.Count, Checks = checks }, Program.JsonOptions));
            return 0;
        }
        finally
        {
            // No recursive removal: only files explicitly created by this test.
            foreach (var path in paths) File.Delete(path);
            Directory.Delete(root);
        }
    }

    private static Packet P(long pts, string hash, bool key = false) => new(pts, hash, key ? "K" : "_", pts + ":" + hash);

    private sealed class FakeProbe : IVideoProbe
    {
        public Dictionary<string, Packet[]> Data { get; } = [];
        public bool OmitDecoded { get; set; }
        public bool ShiftDecoded { get; set; }
        public Action? OnDecode { get; set; }
        public Task<VideoInfo> InspectAsync(string path, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(new VideoInfo(new(1, 1000), 0, .8m));
        }
        public async IAsyncEnumerable<Packet> ReadPacketsAsync(string path, [EnumeratorCancellation] CancellationToken token)
        {
            await Task.CompletedTask;
            foreach (var packet in Data[path]) { token.ThrowIfCancellationRequested(); yield return packet; }
        }
        public Task<IReadOnlyList<DecodedFrame>> DecodeAsync(string path, decimal start, decimal end, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            OnDecode?.Invoke();
            IReadOnlyList<DecodedFrame> frames = OmitDecoded ? [] : Data[path].Select(p => new DecodedFrame(p.Pts + (ShiftDecoded ? 1 : 0), p.Position)).ToArray();
            return Task.FromResult(frames);
        }
    }
}
