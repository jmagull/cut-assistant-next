using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Core.Tests.Editing;

public sealed class HalvingFrameSearchTests
{
    [Fact]
    public void DefaultSequence_HalvesWithFloorAndStaysAtOne()
    {
        var search = new HalvingFrameSearch();
        int[] expected = [2000, 1000, 500, 250, 125, 62, 31, 15, 7, 3, 1, 1];

        foreach (var step in expected)
        {
            Assert.Equal(step, search.NextStep);
            search.Advance();
        }
    }

    [Fact]
    public void Reset_RestoresCustomStart()
    {
        var search = new HalvingFrameSearch(125);
        search.Advance();
        search.Advance();
        Assert.Equal(31, search.NextStep);
        search.Reset();
        Assert.Equal(125, search.NextStep);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonPositiveStart_IsRejected(int initialStep)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new HalvingFrameSearch(initialStep));
    }
}
