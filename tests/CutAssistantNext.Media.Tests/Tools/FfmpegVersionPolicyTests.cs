using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Tests.Tools;

public sealed class FfmpegVersionPolicyTests
{
    [Theory]
    [InlineData("4.4.8", false, false)]
    [InlineData("5.0.9", false, false)]
    [InlineData("5.1", false, false)]
    [InlineData("5.1.0", false, false)]
    [InlineData("5.1.1", true, false)]
    [InlineData("5.1.10", true, false)]
    [InlineData("6.0", true, false)]
    [InlineData("8.1.3-full_build", true, false)]
    [InlineData("9.0", true, false)]
    [InlineData("9.0.1", true, false)]
    [InlineData("9.0.2", true, true)]
    [InlineData("9.0.10", true, true)]
    [InlineData("n9.0.2-gyan", true, true)]
    [InlineData("9.0.2-rc1", true, false)]
    [InlineData("10.0", true, true)]
    public void ReleaseVersionsUseNumericBoundaries(string version, bool supported, bool recommended)
    {
        foreach (string name in new[] { "ffmpeg", "ffprobe" })
        {
            var result = FfmpegVersionPolicy.Assess($"{name} version {version} Copyright", name);
            Assert.Equal(supported, result.IsSupported);
            Assert.Equal(recommended, result.MeetsRecommendation);
            Assert.Contains(version, result.Message);
            Assert.Contains("9.0.2", result.Message);
        }
    }

    [Theory]
    [InlineData("2026-10-04-git-a35c879992-full_build", 63, 63, true)]
    [InlineData("N-123456-gabcdef-full_build", 60, 60, true)]
    [InlineData("N-123456-gabcdef", 59, 59, false)]
    [InlineData("N-123456-gabcdef", 63, 59, false)]
    public void GitBuildRequiresProvenModernLibraryAndCannotClaimRecommendedPatch(string token, int compiled, int runtime, bool supported)
    {
        var result = FfmpegVersionPolicy.Assess($"ffmpeg version {token}\nlibavcodec {compiled}. 1.102 / {runtime}. 1.102", "ffmpeg");
        Assert.Equal(supported, result.IsSupported);
        Assert.False(result.MeetsRecommendation);
        Assert.Contains("9.0.2", result.Message);
    }

    [Theory]
    [InlineData("ffmpeg version fixture\nlibavcodec 63. 1.102 / 63. 1.102")]
    [InlineData("ffmpeg version 2026-10-04\nlibavcodec 63. 1.102 / 63. 1.102")]
    [InlineData("ffmpeg version N-123-gabcdef")]
    [InlineData("ffmpeg version 9.0.2.1.5")]
    [InlineData("ffprobe version 9.0.2")]
    [InlineData("some tool version 9.0.2\nlibavcodec 63. 1.102 / 63. 1.102")]
    [InlineData("ffmpeg version 9999999999999.0.2")]
    public void UnknownOrWrongProgramVersionsAreNotAccepted(string output)
    {
        var result = FfmpegVersionPolicy.Assess(output, "ffmpeg");
        Assert.False(result.IsSupported);
        Assert.False(result.MeetsRecommendation);
    }
}
