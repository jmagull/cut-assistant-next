using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Core.Tests.Editing;

public sealed class CutPlanTests
{
    [Fact]
    public void Constructor_SetsMediaDuration()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        Assert.Equal(
            TimeSpan.FromMinutes(60),
            plan.MediaDuration);

        Assert.Empty(plan.RemoveSegments);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveDuration_ThrowsArgumentOutOfRangeException(
        double durationSeconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CutPlan(
                TimeSpan.FromSeconds(durationSeconds)));
    }

    [Fact]
    public void Add_SortsSegmentsChronologically()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var laterSegment = new RemoveSegment(
            TimeSpan.FromMinutes(20),
            TimeSpan.FromMinutes(30));

        var earlierSegment = new RemoveSegment(
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(10));

        plan.Add(laterSegment);
        plan.Add(earlierSegment);

        Assert.Equal(
            [earlierSegment, laterSegment],
            plan.RemoveSegments);
    }

    [Fact]
    public void Add_WithEndAfterMediaDuration_ThrowsArgumentOutOfRangeException()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var segment = new RemoveSegment(
            TimeSpan.FromMinutes(55),
            TimeSpan.FromMinutes(65));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => plan.Add(segment));
    }

    [Fact]
    public void Add_WithOverlappingSegment_ThrowsInvalidOperationException()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        plan.Add(
            new RemoveSegment(
                TimeSpan.FromMinutes(10),
                TimeSpan.FromMinutes(20)));

        var overlappingSegment = new RemoveSegment(
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(25));

        Assert.Throws<InvalidOperationException>(
            () => plan.Add(overlappingSegment));
    }

    [Fact]
    public void Add_WithAdjacentSegment_Succeeds()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var firstSegment = new RemoveSegment(
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(20));

        var adjacentSegment = new RemoveSegment(
            TimeSpan.FromMinutes(20),
            TimeSpan.FromMinutes(30));

        plan.Add(firstSegment);
        plan.Add(adjacentSegment);

        Assert.Equal(
            [firstSegment, adjacentSegment],
            plan.RemoveSegments);
    }
    [Fact]
    public void Remove_ExistingSegment_RemovesSegment()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var segment = new RemoveSegment(
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(20));

        plan.Add(segment);

        var removed = plan.Remove(segment);

        Assert.True(removed);
        Assert.Empty(plan.RemoveSegments);
    }

    [Fact]
    public void Remove_UnknownSegment_ReturnsFalse()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var segment = new RemoveSegment(
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(20));

        var removed = plan.Remove(segment);

        Assert.False(removed);
    }

    [Fact]
    public void Replace_ExistingSegment_ReplacesAndSortsSegment()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var firstSegment = new RemoveSegment(
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(10));

        var oldSegment = new RemoveSegment(
            TimeSpan.FromMinutes(20),
            TimeSpan.FromMinutes(30));

        var replacement = new RemoveSegment(
            TimeSpan.FromMinutes(12),
            TimeSpan.FromMinutes(18));

        plan.Add(firstSegment);
        plan.Add(oldSegment);

        plan.Replace(
            oldSegment,
            replacement);

        Assert.Equal(
            [firstSegment, replacement],
            plan.RemoveSegments);
    }

    [Fact]
    public void Replace_WithOverlappingSegment_ThrowsInvalidOperationException()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var firstSegment = new RemoveSegment(
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(20));

        var secondSegment = new RemoveSegment(
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(40));

        var overlappingReplacement = new RemoveSegment(
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(35));

        plan.Add(firstSegment);
        plan.Add(secondSegment);

        Assert.Throws<InvalidOperationException>(
            () => plan.Replace(
                secondSegment,
                overlappingReplacement));

        Assert.Equal(
            [firstSegment, secondSegment],
            plan.RemoveSegments);
    }

    [Fact]
    public void Replace_UnknownSegment_ThrowsInvalidOperationException()
    {
        var plan = new CutPlan(
            TimeSpan.FromMinutes(60));

        var unknownSegment = new RemoveSegment(
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(20));

        var replacement = new RemoveSegment(
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(40));

        Assert.Throws<InvalidOperationException>(
            () => plan.Replace(
                unknownSegment,
                replacement));
    }
}