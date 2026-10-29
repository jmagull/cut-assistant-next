using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Joining;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class QuickCopyJoinTests
{
    private static AudioStreamInfo Track(int index, string codec = "aac", string language = "deu") =>
        new(index, codec, null, 48000, 2, "stereo") { Language = language };

    [Fact]
    public void FuriosaAutomaticallyProposesBothAacAndAc3EvenWithDifferentStreamOrder()
    {
        var pairs = QuickCopyAudioPairing.Suggest([Track(1), Track(2, "ac3")], [Track(7, "ac3"), Track(4)]);
        Assert.Equal(new[] { new QuickCopyTrackPair(1, 4), new QuickCopyTrackPair(2, 7) }, pairs);
    }

    [Fact]
    public void JohnWickKeepsOnlyCommonAacWithoutInventingMissingAc3()
    {
        var pairs = QuickCopyAudioPairing.Suggest([Track(1), Track(2, "ac3")], [Track(5)]);
        Assert.Equal(new QuickCopyTrackPair(1, 5), Assert.Single(pairs));
    }

    [Theory]
    [InlineData("ambiguous-first")]
    [InlineData("ambiguous-second")]
    [InlineData("language")]
    [InlineData("rate")]
    [InlineData("layout")]
    [InlineData("codec")]
    public void AmbiguousOrIncompatibleTracksRequireExplicitMapping(string scenario)
    {
        AudioStreamInfo[] a = [Track(1)], b = [Track(2)];
        if (scenario == "ambiguous-first") a = [Track(1), Track(3)];
        if (scenario == "ambiguous-second") b = [Track(2), Track(3)];
        if (scenario == "language") b[0] = b[0] with { Language = "eng" };
        if (scenario == "rate") b[0] = b[0] with { SampleRate = 44100 };
        if (scenario == "layout") b[0] = b[0] with { ChannelLayout = "mono" };
        if (scenario == "codec") b[0] = b[0] with { CodecName = "mp3" };
        Assert.Empty(QuickCopyAudioPairing.Suggest(a, b));
    }

    [Fact]
    public void EquivalentLanguageCodesCanBeProposedButUnknownLanguageIsNotGerman()
    {
        Assert.Single(QuickCopyAudioPairing.Suggest([Track(1, language: "de")], [Track(2, language: "ger")]));
        Assert.Empty(QuickCopyAudioPairing.Suggest([Track(1)], [Track(2, language: "und")]));
    }

    [Theory]
    [InlineData("not-confirmed")]
    [InlineData("empty")]
    [InlineData("duplicate-first")]
    [InlineData("duplicate-second")]
    [InlineData("negative")]
    public void RequestRejectsMissingConsentOrAmbiguousSelectedPairs(string scenario)
    {
        QuickCopyTrackPair[] pairs = scenario switch
        {
            "empty" => [],
            "duplicate-first" => [new(1, 1), new(1, 2)],
            "duplicate-second" => [new(1, 1), new(2, 1)],
            "negative" => [new(-1, 1)],
            _ => [new(1, 1)]
        };
        Assert.Throws<ArgumentException>(() => new QuickCopyJoinRequest("a", "b", pairs, "c", scenario != "not-confirmed").ValidateSelection());
    }

    private static FfprobeStreamCopyVerifier.CopyPacket Packet(long pts, int block, long skip = 0, long discard = 0, string flags = "K__") =>
        new(pts, pts, block, 100, "SHA256:" + new string('a', 64), flags, skip, discard, "");

    private static (QuickAudioSource First, QuickAudioSource Second,
        FfprobeStreamCopyVerifier.CopyPacket[] Original, FfprobeStreamCopyVerifier.CopyPacket[] Output) Aac()
    {
        var a = new[] { Packet(-1024, 1024, 1024, flags: "KD_"), Packet(0, 1024), Packet(1024, 1024, discard: 512) };
        var b = new[] { Packet(-1024, 1024, 1024, flags: "KD_"), Packet(0, 1024), Packet(1024, 1024, discard: 256) };
        var output = new[] { a[0], a[1], a[2] with { Duration = 1, Discard = 0 },
            b[0] with { Pts = 1025, Dts = 1025, Duration = 511, Skip = 0, Flags = "K__" },
            b[1] with { Pts = 1536, Dts = 1536 }, b[2] with { Pts = 2560, Dts = 2560 } };
        return (new(3, 1536, 1024, a[0], a[2], 512), new(3, 1792, 1024, b[0], b[2], 256), [.. a, .. b], output);
    }

    [Fact]
    public void AcceptsOnlyTheExactlyExplainedJohnWickAacCorrectionAndReportsExtraSamples()
    {
        var x = Aac(); var rules = new FfprobeStreamCopyVerifier.QuickAudioPacketRules(x.First, x.Second, 1, 48000, "aac");
        for (var i = 0; i < 6; i++) rules.Add(x.Original[i], x.Output[i], i < 3 ? 0 : 1536, i % 3, 3);
        var proof = rules.Complete();
        Assert.Equal(3328, proof.ReferenceSamples); Assert.Equal(4864, proof.ExpectedDecodedSamples);
        Assert.Equal(new QuickDtsCorrection(1, 1024, 512, 1025), Assert.Single(proof.Corrections));
        Assert.Equal(1536, proof.InnerTrimSamples); Assert.Equal(3328, proof.PresentationSamples);
    }

    [Theory]
    [InlineData("payload")]
    [InlineData("size")]
    [InlineData("outside-pts")]
    [InlineData("correction")]
    [InlineData("duration")]
    [InlineData("flags")]
    [InlineData("unknown-side")]
    [InlineData("outer-trim")]
    [InlineData("missing")]
    public void PacketProofRejectsEveryUnexplainedChangeDespiteAllowedWarningText(string scenario)
    {
        var x = Aac();
        switch (scenario)
        {
            case "payload": x.Output[3] = x.Output[3] with { Hash = "SHA256:" + new string('b', 64) }; break;
            case "size": x.Output[3] = x.Output[3] with { Size = 101 }; break;
            case "outside-pts": x.Output[4] = x.Output[4] with { Pts = 1537, Dts = 1537 }; break;
            case "correction": x.Output[3] = x.Output[3] with { Pts = 1026, Dts = 1026 }; break;
            case "duration": x.Output[1] = x.Output[1] with { Duration = 1000 }; break;
            case "flags": x.Output[4] = x.Output[4] with { Flags = "KD_" }; break;
            case "unknown-side": x.Output[4] = x.Output[4] with { OtherSideData = "new configuration" }; break;
            case "outer-trim": x.Output[5] = x.Output[5] with { Discard = 0 }; break;
        }
        var rules = new FfprobeStreamCopyVerifier.QuickAudioPacketRules(x.First, x.Second, 1, 48000, "aac");
        Assert.Throws<InvalidDataException>(() =>
        {
            for (var i = 0; i < (scenario == "missing" ? 5 : 6); i++) rules.Add(x.Original[i], x.Output[i], i < 3 ? 0 : 1536, i % 3, 3);
            rules.Complete();
        });
    }

    [Fact]
    public void FuriosaAc3AllowsOnlySourceExplainedSpliceOverlapAndLostFinalPadding()
    {
        var a = new[] { Packet(-256, 1536, 256), Packet(1280, 1536), Packet(2816, 1536, discard: 896) };
        var b = new[] { Packet(-256, 1536, 256), Packet(1280, 1536), Packet(2816, 1536, discard: 512) };
        var rules = new FfprobeStreamCopyVerifier.QuickAudioPacketRules(new(3, 3456, 1536, a[0], a[2], 896),
            new(3, 3840, 1536, b[0], b[2], 512), 2, 48000, "ac3");
        for (var i = 0; i < 6; i++)
        {
            var p = i < 3 ? a[i] : b[i - 3]; var offset = i < 3 ? 0 : 3456;
            rules.Add(p, p with { Pts = p.Pts + offset, Dts = p.Dts + offset, Skip = i == 0 ? 256 : 0, Discard = 0 }, offset, i % 3, 3);
        }
        var proof = rules.Complete();
        Assert.Empty(proof.Corrections); Assert.Equal(1664, proof.ExpectedDecodedSamples - proof.ReferenceSamples);
        Assert.Equal(512, proof.LostEndPadding);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3073)]
    [InlineData(long.MaxValue)]
    public void QuickSampleLimitIsFiniteAndRejectsNegativeOrExcessiveDifferences(long delta)
    {
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ValidateQuickSampleDifference(delta, 48000, "aac"));
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ValidateQuickSampleDifference(delta, 44100, "ac3"));
    }

    private const string Correction = "[aost#0:1/copy @ 000002cfb33250c0] Non-monotonic DTS; previous: 1024, current: 512; changing to 1025. This may result in incorrect timestamps in the output file.";

    [Fact]
    public void WarningIsAcceptedOnlyAfterIndependentIdenticalPacketCorrectionProof()
    {
        var policy = new QuickCopyWarnings(["aac", "ac3"], true);
        policy.Add(Correction);
        policy.Add("[mp4 @ 000001c727794f80] track 2: codec frame size is not set");
        policy.Verify([new(1, 1024, 512, 1025)]);
        Assert.Throws<InvalidDataException>(() => policy.Verify([new(1, 1024, 512, 1026)]));
        Assert.Throws<InvalidDataException>(() => policy.Verify([]));
    }

    [Fact]
    public void EqualDtsOneTickCorrectionMayBeUnloggedButStillRequiresExactPacketProof()
    {
        var policy = new QuickCopyWarnings(["aac"], true);
        policy.Verify([new(1, 1024, 1024, 1025)]);
        Assert.Throws<InvalidDataException>(() => policy.Verify([new(1, 1024, 1023, 1025)]));
        Assert.Throws<InvalidDataException>(() => policy.Verify([new(1, 1024, 1024, 1026)]));
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("decoder")]
    [InlineData("wrong-ac3-index")]
    [InlineData("dts-on-ac3")]
    [InlineData("extra-dts")]
    [InlineData("unexpected-dts-during-preparation")]
    public void UnknownDuplicateWrongStreamOrDecoderWarningsAreAlwaysFatal(string scenario)
    {
        var policy = new QuickCopyWarnings(["aac", "ac3"], scenario != "unexpected-dts-during-preparation");
        policy.Add(scenario switch
        {
            "unknown" => "Other warning",
            "decoder" => "[aac @ abc] Error decoding packet",
            "wrong-ac3-index" => "[mp4 @ abc] track 1: codec frame size is not set",
            "dts-on-ac3" => Correction.Replace("#0:1/", "#0:2/"),
            _ => Correction
        });
        if (scenario == "extra-dts") { policy.Add(Correction); policy.Add(Correction); }
        Assert.Throws<InvalidDataException>(() => policy.Verify([]));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void QuickCleanupDeletesOnlyKnownFilesAndKeepsFailureDiagnostics(bool success)
    {
        var work = Path.Combine(Path.GetTempPath(), "quick-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        try
        {
            File.WriteAllText(Path.Combine(work, "decoded.pcm"), "temporary");
            File.WriteAllText(Path.Combine(work, "protocol.txt"), "diagnosis");
            QuickCopyJoinService.Cleanup(work, success, null);
            Assert.Equal(!success, Directory.Exists(work));
            if (!success) Assert.Equal("diagnosis", File.ReadAllText(Path.Combine(work, "protocol.txt")));
        }
        finally { if (Directory.Exists(work)) Directory.Delete(work, true); }
    }

    [Fact]
    public void QuickCleanupPreservesUnexpectedContentsAndProtocol()
    {
        var work = Path.Combine(Path.GetTempPath(), "quick-test-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(work);
        try
        {
            File.WriteAllText(Path.Combine(work, "mine.txt"), "keep"); File.WriteAllText(Path.Combine(work, "protocol.txt"), "diagnosis");
            QuickCopyJoinService.Cleanup(work, true, null);
            Assert.Equal("keep", File.ReadAllText(Path.Combine(work, "mine.txt")));
            Assert.True(File.Exists(Path.Combine(work, "protocol.txt")));
        }
        finally { Directory.Delete(work, true); }
    }
}
