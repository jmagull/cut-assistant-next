using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Core.Tests.Editing;

public sealed class HalvingFrameSearchTests
{
    [Fact]
    public void SameDirectionKeepsStepAndOppositeJumpUsesHalfImmediately()
    {
        var search = new HalvingFrameSearch();
        foreach (var (direction, step) in new[] { (1, 2000), (1, 2000), (-1, 1000), (-1, 1000), (1, 500) })
        {
            Assert.Equal(step, search.GetStep(direction));
            search.CompleteJump(direction);
            Assert.Equal(step, search.NextStep);
        }
    }

    [Fact]
    public void AlternatingDirectionsHalveWithFloorAndStayAtOne()
    {
        var search = new HalvingFrameSearch();
        int direction = -1;
        foreach (int step in new[] { 2000, 1000, 500, 250, 125, 62, 31, 15, 7, 3, 1, 1 })
        {
            Assert.Equal(step, search.GetStep(direction));
            search.CompleteJump(direction);
            direction = -direction;
        }
    }

    [Fact]
    public void PreviewingOppositeDirectionDoesNotConsumeFailedOrCancelledJump()
    {
        var search = new HalvingFrameSearch(125);
        search.CompleteJump(-1);
        Assert.Equal(62, search.GetStep(1));
        Assert.Equal(62, search.GetStep(1));
        Assert.Equal(125, search.GetStep(-1));
        Assert.Equal(125, search.NextStep);
        search.CompleteJump(1);
        Assert.Equal(62, search.GetStep(1));
    }

    [Fact]
    public void ResetRestoresCustomStartAndForgetsDirection()
    {
        var search = new HalvingFrameSearch(125);
        search.CompleteJump(1);
        search.CompleteJump(-1);
        Assert.Equal(62, search.NextStep);
        search.Reset();
        Assert.Equal(125, search.GetStep(1));
        Assert.Equal(125, search.GetStep(-1));
        search.CompleteJump(1);
        Assert.Equal(125, search.NextStep);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonPositiveStartIsRejected(int initialStep) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new HalvingFrameSearch(initialStep));

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(-2)]
    public void InvalidDirectionDoesNotChangeState(int direction)
    {
        var search = new HalvingFrameSearch();
        Assert.Throws<ArgumentOutOfRangeException>(() => search.GetStep(direction));
        Assert.Throws<ArgumentOutOfRangeException>(() => search.CompleteJump(direction));
        Assert.Equal(2000, search.NextStep);
    }
}
