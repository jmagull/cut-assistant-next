using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Tests;

public sealed class CutEdgeReviewSelectionTests
{
    private static CutPlanViewModel Model()
    {
        var model = new CutPlanViewModel();
        var plan = new CutPlan(TimeSpan.FromSeconds(100));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(40)));
        model.LoadCutPlan(plan);
        return model;
    }

    [Theory]
    [InlineData(CutEdgeSide.Start)]
    [InlineData(CutEdgeSide.End)]
    public void ReviewOtherRowPreservesSelectedSegmentAndLoupeTarget(CutEdgeSide side)
    {
        var model = Model();
        var selected = model.RemoveSegments[0];
        var reviewed = model.RemoveSegments[1];
        model.SelectEdge(selected, CutEdgeSide.Start);
        var before = model.CreateCutPlanSnapshot().RemoveSegments.Select(s => (s.Start, s.End)).ToArray();
        Assert.Equal(side == CutEdgeSide.Start ? TimeSpan.FromSeconds(27) : TimeSpan.FromSeconds(43),
            model.GetEdgeReviewPosition(reviewed, side));
        Assert.Same(selected, model.SelectedRemoveSegment);
        Assert.Equal(CutEdgeSide.Start, model.SelectedEdgeSide);
        Assert.Equal(selected.Start, model.SelectedEdgePosition);
        Assert.True(model.CanInspectSelectedEdge);
        Assert.Equal(before, model.CreateCutPlanSnapshot().RemoveSegments.Select(s => (s.Start, s.End)).ToArray());
    }

    [Fact]
    public void ReviewWithoutSelectionDoesNotSelectAndKeepsPendingStart()
    {
        var model = Model();
        model.SetStart(TimeSpan.FromSeconds(50));
        Assert.Equal(TimeSpan.FromSeconds(7),
            model.GetEdgeReviewPosition(model.RemoveSegments[0], CutEdgeSide.Start));
        Assert.Null(model.SelectedRemoveSegment);
        Assert.Null(model.SelectedEdgeSide);
        Assert.Equal(TimeSpan.FromSeconds(50), model.PendingStart);
    }

    [Fact]
    public void ReviewInvalidOrStaleRangeDoesNotModifyCurrentState()
    {
        var model = Model();
        var old = model.RemoveSegments[0];
        model.SelectEdge(old, CutEdgeSide.End);
        var copy = new RemoveSegment(old.Start, old.End);
        Assert.Throws<InvalidOperationException>(() => model.GetEdgeReviewPosition(copy, CutEdgeSide.Start));
        Assert.Same(old, model.SelectedRemoveSegment);
        Assert.Equal(CutEdgeSide.End, model.SelectedEdgeSide);
        model.LoadCutPlan(model.CreateCutPlanSnapshot());
        Assert.Throws<InvalidOperationException>(() => model.GetEdgeReviewPosition(old, CutEdgeSide.End));
    }
}
