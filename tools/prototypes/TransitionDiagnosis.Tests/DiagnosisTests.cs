using TransitionDiagnosis;

namespace TransitionDiagnosis.Tests;

public sealed class DiagnosisTests
{
    private static VideoPacket Packet(long pts, string hash, long? dts = null, string flags = "K_") =>
        new(pts, dts ?? pts, 40, pts + 1000, hash, flags);

    private static VideoSnapshot Snapshot(VideoPacket[] packets, long denominator = 1000) =>
        new(new(new(1, denominator), 0, 30, 30, "h264", "25/1"), packets);

    private static PacketDiagnosis Fixture(long lastOld = 120, long firstNew = 80) => new(
        Snapshot([Packet(1000, "a"), Packet(1040, "b"), Packet(10000, "c"), Packet(10040, "d")]),
        Snapshot([Packet(0, "a"), Packet(lastOld, "b"), Packet(firstNew, "c"), Packet(160, "d")]),
        [new(1, 1), new(10, 1)], 0);

    [Fact]
    public void ReportsOverlapAndPlanErrorWithoutClaimingConfirmedPosition()
    {
        var d = Fixture();
        Assert.True(d.Result.AllPacketsUniquelyMapped);
        Assert.True(d.Result.KeepOrderMatchesPlan);
        var b = Assert.Single(d.Result.Boundaries);
        Assert.Equal("Overlap", b.PacketPresentation);
        Assert.Equal(.04m, b.OverlapSeconds);
        Assert.Equal(-.92m, b.CandidateMinusPlanSeconds);
        Assert.Equal(0m, b.CandidateReviewStartSeconds);
        Assert.False(b.ReviewPositionConfirmed);
    }

    [Theory]
    [InlineData(80, 80, "EqualPts")]
    [InlineData(40, 80, "SeparatedPts")]
    public void DistinguishesEqualAndSeparatedPresentation(long oldPts, long newPts, string classification) =>
        Assert.Equal(classification, Assert.Single(Fixture(oldPts, newPts).Result.Boundaries).PacketPresentation);

    [Fact]
    public void UsesIndependentRationalTimebasesAndDoesNotConfuseDtsWithPts()
    {
        var source = Snapshot([Packet(25, "a"), Packet(250, "b")], 25);
        var output = Snapshot([Packet(0, "a", -100), Packet(90_090, "b", 0)], 90_000);
        var d = new PacketDiagnosis(source, output, [new(1, 1), new(10, 1)], 0);
        Assert.Equal(1.001m, Assert.Single(d.Result.Boundaries).PacketCandidateSeconds);
    }

    [Fact]
    public void RepeatedPayloadIsAmbiguousEvenIfOneOccurrenceIsNearAPlannedKeep()
    {
        var d = new PacketDiagnosis(Snapshot([Packet(1000, "a"), Packet(9000, "a")]),
            Snapshot([Packet(0, "a")]), [new(1, 1)], 0);
        Assert.False(d.Result.AllPacketsUniquelyMapped);
        Assert.Equal("RepeatedPayloadInSource", Assert.Single(d.Result.IssueExamples).Reason);
    }

    [Fact]
    public void ResolvesRepeatedPayloadOnlyInsideAnExactSequenceWithTwoUniqueAnchors()
    {
        var source = Snapshot([Packet(1000, "a"), Packet(1040, "repeat"), Packet(1080, "repeat"), Packet(1120, "b"), Packet(9000, "repeat")]);
        var output = Snapshot([Packet(0, "a"), Packet(40, "repeat"), Packet(80, "repeat"), Packet(120, "b")]);
        var d = new PacketDiagnosis(source, output, [new(1, 1)], 0);
        Assert.True(d.Result.AllPacketsUniquelyMapped);
        Assert.Equal(2, d.Result.ContextResolvedPackets);
        Assert.Equal([0, 1, 2, 3], d.Assignments.Select(a => a.SourceIndex!.Value));
    }

    [Theory]
    [InlineData("jump")]
    [InlineData("changed")]
    public void DoesNotBridgeACutOrChangedSequenceToResolveDuplicates(string variant)
    {
        var source = Snapshot([Packet(1000, "a"), Packet(1040, "repeat"), Packet(1080, "gap"), Packet(1120, "b"), Packet(9000, "repeat")]);
        var output = variant == "jump" ? Snapshot([Packet(0, "a"), Packet(40, "repeat"), Packet(80, "b")]) :
            Snapshot([Packet(0, "a"), Packet(40, "changed"), Packet(80, "repeat"), Packet(120, "b")]);
        var d = new PacketDiagnosis(source, output, [new(1, 1)], 0);
        Assert.False(d.Result.AllPacketsUniquelyMapped);
        Assert.Equal(0, d.Result.ContextResolvedPackets);
    }

    [Theory]
    [InlineData("x", 1000, "PayloadNotInSource")]
    [InlineData("a", 5000, "OutsideKeepWindows")]
    public void DoesNotGuessUnmatchedPackets(string hash, long sourceTime, string reason)
    {
        var d = new PacketDiagnosis(Snapshot([Packet(sourceTime, "a")]), Snapshot([Packet(0, hash)]), [new(1, 1)], 0);
        Assert.Equal(reason, Assert.Single(d.Result.IssueExamples).Reason);
    }

    [Fact]
    public void OverlappingAttributionWindowsRemainAmbiguous()
    {
        var d = new PacketDiagnosis(Snapshot([Packet(2500, "a")]), Snapshot([Packet(0, "a")]), [new(1, 1), new(3, 1)], 1);
        Assert.Equal("OverlappingKeepWindows", Assert.Single(d.Result.IssueExamples).Reason);
    }

    [Fact]
    public void WrongKeepOrderAndMissingKeepAreNotAccepted()
    {
        var source = Snapshot([Packet(1000, "a"), Packet(10000, "b")]);
        var reversed = new PacketDiagnosis(source, Snapshot([Packet(0, "b"), Packet(40, "a")]), [new(1, 1), new(10, 1)], 0);
        Assert.True(reversed.Result.AllPacketsUniquelyMapped);
        Assert.False(reversed.Result.KeepOrderMatchesPlan);
        var missing = new PacketDiagnosis(source, Snapshot([Packet(0, "a")]), [new(1, 1), new(10, 1)], 0);
        Assert.False(missing.Result.KeepOrderMatchesPlan);
        Assert.Null(Assert.Single(missing.Result.Boundaries).PacketCandidateSeconds);
    }

    [Fact]
    public void FlagsArePreservedAndNeverSilentlyCertified()
    {
        var d = new PacketDiagnosis(Snapshot([Packet(1000, "a"), Packet(10000, "b")]),
            Snapshot([Packet(0, "a", flags: "D"), Packet(40, "b", flags: "C")]), [new(1, 1), new(10, 1)], 0);
        Assert.Equal(2, d.Result.DiscardOrCorruptPackets);
        Assert.False(Assert.Single(d.Result.Boundaries).ReviewPositionConfirmed);
    }

    [Fact]
    public void DecoderMembershipUsesPacketPositionsRatherThanEqualPts()
    {
        var d = Fixture(80, 80);
        // The two boundary packets have equal PTS, but distinct container byte positions.
        var packets = d.Output.Packets.Select((p, i) => p with { Position = 100 + i }).ToArray();
        d = new PacketDiagnosis(d.Source, new(d.Output.Metadata, packets), [new(1, 1), new(10, 1)], 0);
        var b = Assert.Single(d.Result.Boundaries);
        d.AddDecodedFrames(b, [new(80, null, 80, 40, 101, "B"), new(80, null, 80, 40, 102, "I")]);
        Assert.Equal([1, 2], b.Frames.Select(f => f.Keep!.Value));
        Assert.Equal("EqualDecodedBoundaryPts", b.DecodeStatus);
        Assert.False(b.ReviewPositionConfirmed);
    }

    [Theory]
    [InlineData("interleave", "InterleavedKeeps")]
    [InlineData("backwards", "NonMonotonicDecodedPts")]
    [InlineData("missing", "MissingFrameEvidence")]
    [InlineData("new-only", "BothSidesNotObserved")]
    [InlineData("single", "SingleObservedSwitch")]
    public void ClassifiesDecodedSequencesConservatively(string kind, string expected)
    {
        FrameEvidence F(decimal time, int? keep) => new(time, null, time, .04m, 1, "B", keep, 0, 1);
        FrameEvidence[] frames = kind switch
        {
            "interleave" => [F(0, 1), F(.04m, 2), F(.08m, 1)],
            "backwards" => [F(.08m, 1), F(.04m, 2)],
            "missing" => [F(0, null), F(.04m, 2)],
            "new-only" => [F(.04m, 2)],
            _ => [F(0, 1), F(.04m, 2)]
        };
        Assert.Equal(expected, PacketDiagnosis.ClassifyDecoded(frames, 2, true));
        Assert.Equal("PacketMappingIncomplete", PacketDiagnosis.ClassifyDecoded(frames, 2, false));
    }

    [Fact]
    public void ParsesNegativeDtsAndRetainsMissingPtsWithoutInventingTiming()
    {
        var packet = FfprobeReader.ParsePacket("pts=N/A|dts=-80|duration=40|pos=12|flags=K_|data_hash=SHA256:" + new string('A', 64));
        Assert.Null(packet.Pts);
        Assert.Equal(-80, packet.Dts);
        var d = new PacketDiagnosis(Snapshot([packet]), Snapshot([packet]), [new(0, 1)], 0);
        Assert.Equal("SourcePtsMissing", Assert.Single(d.Result.IssueExamples).Reason);
    }

    [Theory]
    [InlineData("pts=0|pos=1|flags=K_|data_hash=SHA256:bad")]
    [InlineData("pts=0|pos=-1|flags=K_")]
    public void RejectsIncompletePacketEvidence(string line) => Assert.Throws<InvalidDataException>(() => FfprobeReader.ParsePacket(line));

    [Theory]
    [InlineData("1/0")]
    [InlineData("-1/25")]
    [InlineData("1")]
    public void RejectsInvalidTimebase(string value) => Assert.Throws<InvalidDataException>(() => VideoTimeBase.Parse(value));

    [Fact]
    public void RejectsInvalidKeepsAndHonorsCancellation()
    {
        Assert.Throws<ArgumentException>(() => PacketDiagnosis.ValidateKeeps([new(1, 2), new(2, 1)], 0));
        Assert.Throws<ArgumentException>(() => PacketDiagnosis.ValidateKeeps([new(1, 0)], 0));
        using var canceled = new CancellationTokenSource();
        canceled.Cancel();
        Assert.Throws<OperationCanceledException>(() => new PacketDiagnosis(Fixture().Source, Fixture().Output, [new(1, 1)], 0, canceled.Token));
    }

    [Fact]
    public async Task MissingToolAndPreCanceledProbeFailWithoutAChildProcess()
    {
        var reader = new FfprobeReader(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe"));
        await Assert.ThrowsAsync<FileNotFoundException>(() => reader.ReadAsync("input.mp4", default));
        using var canceled = new CancellationTokenSource();
        canceled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadAsync("input.mp4", canceled.Token));
    }

    [Fact]
    public async Task ReportCannotOverwriteAnInputAndCanceledReportLeavesNoPartialFile()
    {
        var path = Path.Combine(Path.GetTempPath(), "transition-report-" + Guid.NewGuid() + ".json");
        try
        {
            await File.WriteAllTextAsync(path, "original");
            await Assert.ThrowsAsync<IOException>(() => DiagnosticOptions.WriteReportAsync(path, new { Value = 1 }, default));
            Assert.Equal("original", await File.ReadAllTextAsync(path));
            File.Delete(path);
            using var canceled = new CancellationTokenSource();
            canceled.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => DiagnosticOptions.WriteReportAsync(path, new { Value = 1 }, canceled.Token));
            Assert.False(File.Exists(path));
            await DiagnosticOptions.WriteReportAsync(path, new { Value = 2 }, default);
            Assert.Contains("2", await File.ReadAllTextAsync(path));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void CommandLineRejectsMissingDuplicateAndAliasedInputs()
    {
        Assert.Throws<ArgumentException>(() => DiagnosticOptions.Parse([]));
        Assert.Throws<ArgumentException>(() => DiagnosticOptions.Parse(["--unknown", "x"]));
        Assert.Throws<ArgumentException>(() => DiagnosticOptions.Parse(["--source", "a", "--source", "b"]));
        Assert.Throws<ArgumentException>(() => DiagnosticOptions.Parse([
            "--ffprobe", "tool.exe", "--source", "original.mp4", "--output", "original.mp4",
            "--cutlist", "plan.cutlist", "--report", "diagnosis.json"]));
    }
}
