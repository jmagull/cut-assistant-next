using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Tests;

public sealed class SchnittshuffleBoundaryTests
{
    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void AdjacentSegmentsHaveSeparateStationsAtTheSameTime(CutEditingMode mode)
    {
        var plan = new CutPlanViewModel();
        plan.Initialize(TimeSpan.FromSeconds(10), mode);
        Add(1, 3);
        Add(3, 5);
        var first = plan.MarkedSegments[0];
        var second = plan.MarkedSegments[1];
        var shared = TimeSpan.FromSeconds(3);

        Assert.Equal((first, CutEdgeSide.End), plan.FindShuffleBoundary(true, shared));
        Assert.Equal((second, CutEdgeSide.Start), plan.FindShuffleBoundary(false, shared));

        plan.SelectEdge(first, CutEdgeSide.End);
        Assert.Equal((second, CutEdgeSide.Start), plan.FindShuffleBoundary(true, shared));
        plan.SelectEdge(second, CutEdgeSide.Start);
        Assert.Equal((first, CutEdgeSide.End), plan.FindShuffleBoundary(false, shared));

        void Add(int start, int end)
        {
            plan.SetStart(TimeSpan.FromSeconds(start));
            plan.SetEnd(TimeSpan.FromSeconds(end));
        }
    }

    [Fact]
    public void OpenMarkAndPositionsBeyondTheOuterBoundariesHaveNoTarget()
    {
        var plan = new CutPlanViewModel();
        plan.Initialize(TimeSpan.FromSeconds(10));
        Assert.Null(plan.FindShuffleBoundary(true, TimeSpan.Zero));
        plan.SetStart(TimeSpan.FromSeconds(2));
        Assert.Null(plan.FindShuffleBoundary(true, TimeSpan.Zero));
        plan.SetEnd(TimeSpan.FromSeconds(6));
        Assert.Null(plan.FindShuffleBoundary(false, TimeSpan.Zero));
        Assert.Null(plan.FindShuffleBoundary(true, TimeSpan.FromSeconds(7)));
        plan.SetStart(TimeSpan.FromSeconds(8));
        Assert.Null(plan.FindShuffleBoundary(false, TimeSpan.FromSeconds(7)));
        Assert.Equal(TimeSpan.FromSeconds(8), plan.PendingStart);
    }
}
