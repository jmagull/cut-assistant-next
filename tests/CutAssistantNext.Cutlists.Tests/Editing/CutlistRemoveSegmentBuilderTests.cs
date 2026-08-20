using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.Cutlists.Tests.Editing;

public sealed class CutlistRemoveSegmentBuilderTests
{
    [Fact]
    public void Build_WithTwoKeepSegments_ReturnsThreeRemoveSegments()
    {
        var keepSegments =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(10)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(20))
            };

        var removeSegments =
            CutlistRemoveSegmentBuilder.Build(
                TimeSpan.FromSeconds(100),
                keepSegments);

        Assert.Equal(
            3,
            removeSegments.Count);

        Assert.Equal(
            TimeSpan.Zero,
            removeSegments[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            removeSegments[0].End);

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            removeSegments[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(30),
            removeSegments[1].End);

        Assert.Equal(
            TimeSpan.FromSeconds(50),
            removeSegments[2].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(100),
            removeSegments[2].End);
    }

    [Fact]
    public void Build_WithKeepSegmentsAtBeginningAndEnd_ReturnsOnlyGap()
    {
        var keepSegments =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(70))
            };

        var removeSegments =
            CutlistRemoveSegmentBuilder.Build(
                TimeSpan.FromSeconds(100),
                keepSegments);

        var removeSegment =
            Assert.Single(
                removeSegments);

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            removeSegment.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(30),
            removeSegment.End);
    }}
