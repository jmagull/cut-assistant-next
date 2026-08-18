using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxSplitRangeBuilderTests
{
    [Fact]
    public void Build_PeacemakerFirstKeepSegment_SubtractsExactlyOneFrame()
    {
        var range =
            Mp4BoxSplitRangeBuilder.Build(
                TimeSpan.FromSeconds(519.8746667),
                TimeSpan.FromSeconds(457.161),
                25);

        Assert.Equal(
            TimeSpan.FromSeconds(519.8746667),
            range.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(976.9956667),
            range.End);
    }

    [Fact]
    public void Build_PeacemakerSecondKeepSegment_SubtractsExactlyOneFrame()
    {
        var range =
            Mp4BoxSplitRangeBuilder.Build(
                TimeSpan.FromSeconds(1460.0823334),
                TimeSpan.FromSeconds(1152.239),
                25);

        Assert.Equal(
            TimeSpan.FromSeconds(1460.0823334),
            range.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(2612.2813334),
            range.End);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-25)]
    public void Build_WithNonPositiveFrameRate_Throws(
        double framesPerSecond)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () =>
                Mp4BoxSplitRangeBuilder.Build(
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(10),
                    framesPerSecond));
    }
}
