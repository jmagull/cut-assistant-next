using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class FfprobeStreamCopyVerifierTests
{
    [Theory]
    [InlineData("skip_samples", "skip_samples")]
    [InlineData("side_datum/skip_samples:skip_samples", "skip_samples")]
    [InlineData("side_datum/skip_samples:discard_padding", "discard_padding")]
    [InlineData("side_datum/skip_samples:side_data_type", "side_data_type")]
    public void ReadsLegacyAndQualifiedPaddingFields(string key, string expected) =>
        Assert.Equal(expected, FfprobeStreamCopyVerifier.NormalizePacketKey(key));

    private static FfprobeStreamCopyVerifier.CopyPacket Packet() =>
        new(-1024, -1024, 1024, 300, "SHA256:" + new string('a', 64), "KD", 1024, 0, "");

    [Fact]
    public void ComparesRationalClocksWithoutRoundingOrOverflow()
    {
        var a = Packet() with { Pts = long.MaxValue / 2, Dts = 0, Duration = 1024 };
        var b = a with { Pts = a.Pts * 2, Duration = 2048 };
        Assert.True(FfprobeStreamCopyVerifier.Same(a, b, 1, 48000, 1, 96000));
        Assert.False(FfprobeStreamCopyVerifier.Same(a, b with { Pts = b.Pts - 1 }, 1, 48000, 1, 96000));
    }

    [Fact]
    public void RejectsChangedPayloadClockPaddingOrFlags()
    {
        var a = Packet();
        foreach (var changed in new[] {
            a with { Pts = -1023 }, a with { Dts = -1023 }, a with { Duration = 1023 },
            a with { Hash = "SHA256:" + new string('b', 64) }, a with { Skip = 0 },
            a with { Discard = 1 }, a with { Flags = "K_" }, a with { Size = 301 },
            a with { OtherSideData = "changed" } })
            Assert.False(FfprobeStreamCopyVerifier.Same(a, changed, 1, 48000, 1, 48000));
        Assert.True(FfprobeStreamCopyVerifier.Same(a, a, 1, 48000, 1, 48000));
    }
}
