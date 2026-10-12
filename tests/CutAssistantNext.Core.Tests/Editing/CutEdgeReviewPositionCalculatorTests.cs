using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Core.Tests.Editing;

public sealed class CutEdgeReviewPositionCalculatorTests
{
    private static (CutPlan Plan, RemoveSegment Segment) Single(double start, double end, double duration = 100)
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(duration));
        var segment = new RemoveSegment(TimeSpan.FromSeconds(start), TimeSpan.FromSeconds(end));
        plan.Add(segment);
        return (plan, segment);
    }

    [Fact]
    public void AdvertExampleUsesExactlyThreeSecondsOnOriginalTimeAxis()
    {
        var (plan, segment) = Single(600, 720, 1800);
        Assert.Equal(TimeSpan.FromSeconds(597),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.Start));
        Assert.Equal(TimeSpan.FromSeconds(723),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End));
        Assert.Equal(TimeSpan.FromSeconds(600), segment.Start);
        Assert.Equal(TimeSpan.FromSeconds(720), segment.End);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(3.5, 0.5)]
    public void StartOffsetIsClampedToFileBeginning(double start, double expected)
    {
        var (plan, segment) = Single(start, 10);
        Assert.Equal(TimeSpan.FromSeconds(expected),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.Start));
    }

    [Theory]
    [InlineData(96, 99)]
    [InlineData(97, 100)]
    [InlineData(99, 100)]
    [InlineData(100, 100)]
    public void EndOffsetIsClampedToFileEnd(double end, double expected)
    {
        var (plan, segment) = Single(50, end);
        Assert.Equal(TimeSpan.FromSeconds(expected),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End));
    }

    [Theory]
    [InlineData(5, 7)]
    [InlineData(7, 7)]
    [InlineData(9, 9)]
    public void ShortPrecedingFilmStopsAtPreviousRemovedEnd(double previousEnd, double expected)
    {
        var (plan, segment) = Single(10, 20);
        plan.Add(new RemoveSegment(TimeSpan.Zero, TimeSpan.FromSeconds(previousEnd)));
        Assert.Equal(TimeSpan.FromSeconds(expected),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.Start));
    }

    [Theory]
    [InlineData(21)]
    [InlineData(23)]
    public void ShortFollowingFilmStopsStrictlyBeforeNextRemovedStart(double nextStart)
    {
        var (plan, segment) = Single(10, 20);
        var next = new RemoveSegment(TimeSpan.FromSeconds(nextStart), TimeSpan.FromSeconds(40));
        plan.Add(next);
        var result = CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End);
        Assert.Equal(TimeSpan.FromTicks(next.Start.Ticks - 1), result);
        Assert.True(result >= segment.End && result < next.Start);
    }

    [Fact]
    public void LongFollowingFilmKeepsExactThreeSeconds()
    {
        var (plan, segment) = Single(10, 20);
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(40)));
        Assert.Equal(TimeSpan.FromSeconds(23),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End));
    }

    [Theory]
    [InlineData(CutEdgeSide.Start)]
    [InlineData(CutEdgeSide.End)]
    public void TouchingRemovedRangesHaveNoAdjacentFilmOnThatSide(CutEdgeSide side)
    {
        var (plan, segment) = Single(10, 20);
        plan.Add(side == CutEdgeSide.Start
            ? new RemoveSegment(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            : new RemoveSegment(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40)));
        Assert.Null(CutEdgeReviewPositionCalculator.Calculate(plan, segment, side));
    }

    [Fact]
    public void SingleTickFilmIntervalIsNotLost()
    {
        var (plan, segment) = Single(10, 20);
        var previousEnd = segment.Start - TimeSpan.FromTicks(1);
        plan.Add(new RemoveSegment(TimeSpan.Zero, previousEnd));
        plan.Add(new RemoveSegment(segment.End + TimeSpan.FromTicks(1), TimeSpan.FromSeconds(40)));
        Assert.Equal(previousEnd, CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.Start));
        Assert.Equal(segment.End, CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End));
    }

    [Fact]
    public void FractionalCutTimeRetainsAllTickPrecision()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(100));
        var segment = new RemoveSegment(TimeSpan.FromTicks(123456789), TimeSpan.FromTicks(234567891));
        plan.Add(segment);
        Assert.Equal(segment.Start - TimeSpan.FromSeconds(3),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.Start));
        Assert.Equal(segment.End + TimeSpan.FromSeconds(3),
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End));
    }

    [Fact]
    public void EndCalculationCannotOverflowTimeSpan()
    {
        var plan = new CutPlan(TimeSpan.MaxValue);
        var segment = new RemoveSegment(TimeSpan.MaxValue - TimeSpan.FromSeconds(10),
            TimeSpan.MaxValue - TimeSpan.FromSeconds(1));
        plan.Add(segment);
        Assert.Equal(TimeSpan.MaxValue,
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, CutEdgeSide.End));
    }

    [Fact]
    public void ForeignSegmentAndInvalidSideAreRejectedWithoutChangingPlan()
    {
        var (plan, segment) = Single(10, 20);
        Assert.Throws<InvalidOperationException>(() => CutEdgeReviewPositionCalculator.Calculate(
            plan, new RemoveSegment(segment.Start, segment.End), CutEdgeSide.Start));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CutEdgeReviewPositionCalculator.Calculate(plan, segment, (CutEdgeSide)99));
        Assert.Single(plan.RemoveSegments);
        Assert.Same(segment, plan.RemoveSegments[0]);
    }
}
