using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Tests;

public sealed class CutSegmentToggleTests
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
    [InlineData(null)]
    [InlineData(CutEdgeSide.Start)]
    [InlineData(CutEdgeSide.End)]
    public void FirstClickSelectsAndSecondClickClearsRowAndEdge(CutEdgeSide? side)
    {
        var model = Model();
        var segment = model.RemoveSegments[0];
        var before = model.CreateCutPlanSnapshot().RemoveSegments.Select(s => (s.Start, s.End)).ToArray();
        model.ToggleSegmentSelection(segment, side);
        Assert.Same(segment, model.SelectedRemoveSegment);
        Assert.Equal(side, model.SelectedEdgeSide);
        Assert.True(model.CanModifySelectedSegment);
        Assert.Equal(side.HasValue, model.CanInspectSelectedEdge);
        model.ToggleSegmentSelection(segment, side);
        Assert.Null(model.SelectedRemoveSegment);
        Assert.Null(model.SelectedEdgeSide);
        Assert.Null(model.SelectedEdgePosition);
        Assert.False(model.CanModifySelectedSegment);
        Assert.False(model.CanInspectSelectedEdge);
        Assert.Equal(before, model.CreateCutPlanSnapshot().RemoveSegments.Select(s => (s.Start, s.End)).ToArray());
    }

    [Theory]
    [InlineData(CutEdgeSide.Start, CutEdgeSide.End)]
    [InlineData(CutEdgeSide.End, CutEdgeSide.Start)]
    public void ClickingOtherEdgeOnSameRowStillDeselects(CutEdgeSide first, CutEdgeSide second)
    {
        var model = Model();
        var segment = model.RemoveSegments[0];
        model.ToggleSegmentSelection(segment, first);
        model.ToggleSegmentSelection(segment, second);
        Assert.Null(model.SelectedRemoveSegment);
        Assert.Null(model.SelectedEdgeSide);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(CutEdgeSide.Start)]
    [InlineData(CutEdgeSide.End)]
    public void DifferentRowSwitchesSelectionAndEdge(CutEdgeSide? side)
    {
        var model = Model();
        model.ToggleSegmentSelection(model.RemoveSegments[0], CutEdgeSide.Start);
        var second = model.RemoveSegments[1];
        model.ToggleSegmentSelection(second, side);
        Assert.Same(second, model.SelectedRemoveSegment);
        Assert.Equal(side, model.SelectedEdgeSide);
        Assert.Equal(side switch { CutEdgeSide.Start => second.Start, CutEdgeSide.End => second.End, _ => (TimeSpan?)null },
            model.SelectedEdgePosition);
    }

    [Fact]
    public void SelectionAfterPendingStartKeepsExistingMarkerRules()
    {
        var model = Model();
        model.SetStart(TimeSpan.FromSeconds(50));
        Assert.NotNull(model.PendingStart);
        model.ToggleSegmentSelection(model.RemoveSegments[0], CutEdgeSide.Start);
        Assert.Null(model.PendingStart);
        model.ToggleSegmentSelection(model.RemoveSegments[0], CutEdgeSide.Start);
        Assert.False(model.CanSetEnd);
        Assert.True(model.CanSetStart);
    }

    [Fact]
    public void StaleSegmentsCannotChangeCurrentSelection()
    {
        var model = Model();
        var first = model.RemoveSegments[0];
        model.ToggleSegmentSelection(first, CutEdgeSide.Start);
        var foreign = new RemoveSegment(first.Start, first.End);
        Assert.Throws<InvalidOperationException>(() => model.ToggleSegmentSelection(foreign, CutEdgeSide.End));
        Assert.Same(first, model.SelectedRemoveSegment);
        Assert.Equal(CutEdgeSide.Start, model.SelectedEdgeSide);
    }

    [Fact]
    public void InvalidSideAndMissingPlanDoNotIntroduceSelection()
    {
        var model = Model();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            model.ToggleSegmentSelection(model.RemoveSegments[0], (CutEdgeSide)99));
        Assert.Null(model.SelectedRemoveSegment);
        var empty = new CutPlanViewModel();
        Assert.Throws<InvalidOperationException>(() => empty.ToggleSegmentSelection(model.RemoveSegments[0]));
        Assert.Null(empty.SelectedRemoveSegment);
    }

    [Fact]
    public void ReselectingEdgeSupportsFrameCorrectionAndRejectsOldTargets()
    {
        var model = Model();
        var first = model.RemoveSegments[0];
        model.ToggleSegmentSelection(first, CutEdgeSide.Start);
        model.ToggleSegmentSelection(first, CutEdgeSide.End);
        Assert.Throws<InvalidOperationException>(() =>
            model.ApplyFrameEdge(first, CutEdgeSide.Start, TimeSpan.FromSeconds(11)));

        model.ToggleSegmentSelection(first, CutEdgeSide.End);
        model.ApplyFrameEdge(first, CutEdgeSide.End, TimeSpan.FromSeconds(21));
        Assert.Equal(TimeSpan.FromSeconds(21), model.SelectedEdgePosition);
        Assert.Equal(TimeSpan.FromSeconds(10), model.RemoveSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(21), model.RemoveSegments[0].End);
        Assert.Equal(CutEdgeSide.End, model.SelectedEdgeSide);
        Assert.True(model.CanInspectSelectedEdge);
    }

    [Fact]
    public void TimeEditingAndRemovingSelectedSegmentRemainAvailable()
    {
        var model = Model();
        model.ToggleSegmentSelection(model.RemoveSegments[0], CutEdgeSide.Start);
        model.SetStart(TimeSpan.FromSeconds(11));
        Assert.Equal(TimeSpan.FromSeconds(11), model.RemoveSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(20), model.RemoveSegments[0].End);
        Assert.NotNull(model.SelectedRemoveSegment);
        model.Remove(model.SelectedRemoveSegment!);
        Assert.Single(model.RemoveSegments);
        Assert.Null(model.SelectedRemoveSegment);
        Assert.Null(model.SelectedEdgeSide);
    }

    [Fact]
    public void LoadingDifferentPlanClearsSelectionEvenWhenCutsHaveSameTimes()
    {
        var model = Model();
        var old = model.RemoveSegments[0];
        model.ToggleSegmentSelection(old, CutEdgeSide.Start);
        model.LoadCutPlan(model.CreateCutPlanSnapshot());
        Assert.Null(model.SelectedRemoveSegment);
        Assert.Throws<InvalidOperationException>(() => model.ToggleSegmentSelection(old, CutEdgeSide.End));
    }
}
