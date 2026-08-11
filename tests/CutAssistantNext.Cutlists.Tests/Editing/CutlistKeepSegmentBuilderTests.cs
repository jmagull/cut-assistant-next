using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.Cutlists.Tests.Editing;

public sealed class CutlistKeepSegmentBuilderTests
{
    [Fact]
    public void Build_WithTwoRemoveSegments_ReturnsThreeKeepSegments()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20)));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(40),
                TimeSpan.FromSeconds(50)));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        Assert.Equal(
            3,
            keepSegments.Count);

        Assert.Equal(
            TimeSpan.FromSeconds(0),
            keepSegments[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            keepSegments[0].Duration);

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            keepSegments[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            keepSegments[1].Duration);

        Assert.Equal(
            TimeSpan.FromSeconds(50),
            keepSegments[2].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(50),
            keepSegments[2].Duration);
    }

    [Fact]
    public void Build_WithNoRemoveSegments_ReturnsWholeMovieAsKeepSegment()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        var keepSegment =
            Assert.Single(keepSegments);

        Assert.Equal(
            TimeSpan.Zero,
            keepSegment.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(100),
            keepSegment.Duration);
    }

    [Fact]
    public void Build_WithRemoveSegmentAtStart_ReturnsRemainingMovie()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10)));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        var keepSegment =
            Assert.Single(keepSegments);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            keepSegment.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(90),
            keepSegment.Duration);
    }

    [Fact]
    public void Build_WithRemoveSegmentAtEnd_ReturnsMovieBeforeRemoval()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(90),
                TimeSpan.FromSeconds(100)));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        var keepSegment =
            Assert.Single(keepSegments);

        Assert.Equal(
            TimeSpan.Zero,
            keepSegment.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(90),
            keepSegment.Duration);
    }

    [Fact]
    public void Build_WithAdjacentRemoveSegments_DoesNotCreateEmptyKeepSegment()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20)));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30)));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        Assert.Equal(
            2,
            keepSegments.Count);

        Assert.Equal(
            TimeSpan.Zero,
            keepSegments[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            keepSegments[0].Duration);

        Assert.Equal(
            TimeSpan.FromSeconds(30),
            keepSegments[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(70),
            keepSegments[1].Duration);
    }

    [Fact]
    public void Build_WithWholeMovieRemoved_ReturnsNoKeepSegments()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(100)));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        Assert.Empty(keepSegments);
    }

    [Fact]
    public void Build_ReturnedKeepSegment_HasExpectedEnd()
    {
        var cutPlan = new CutPlan(
            TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20)));

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            keepSegments[0].End);

        Assert.Equal(
            TimeSpan.FromSeconds(100),
            keepSegments[1].End);
    }
}
