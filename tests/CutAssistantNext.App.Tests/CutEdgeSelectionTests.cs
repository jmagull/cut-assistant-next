using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.Tests;

public sealed class CutEdgeSelectionTests
{
    [Fact]
    public void SelectingStartThenEndOnSameRow_ChangesTargetPosition()
    {
        var viewModel = CreatePlan();
        var segment = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(segment, CutEdgeSide.Start);
        Assert.True(viewModel.CanInspectSelectedEdge);
        Assert.Equal(segment.Start, viewModel.SelectedEdgePosition);

        viewModel.SelectEdge(segment, CutEdgeSide.End);
        Assert.Same(segment, viewModel.SelectedRemoveSegment);
        Assert.Equal(CutEdgeSide.End, viewModel.SelectedEdgeSide);
        Assert.Equal(segment.End, viewModel.SelectedEdgePosition);
    }

    [Fact]
    public void SelectingDifferentRowWithoutEdge_ClearsPreviousEdge()
    {
        var viewModel = CreatePlan();
        viewModel.SelectEdge(viewModel.RemoveSegments[0], CutEdgeSide.Start);
        viewModel.SelectedRemoveSegment = viewModel.RemoveSegments[1];
        AssertNoEdge(viewModel);
    }

    [Theory]
    [InlineData("deselect")]
    [InlineData("remove")]
    [InlineData("replace")]
    [InlineData("reset")]
    [InlineData("initialize")]
    [InlineData("load")]
    public void ChangingPlanOrSelection_DiscardsStaleTarget(string operation)
    {
        var viewModel = CreatePlan();
        var segment = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(segment, CutEdgeSide.End);

        switch (operation)
        {
            case "deselect": viewModel.SelectedRemoveSegment = null; break;
            case "remove": viewModel.Remove(segment); break;
            case "replace": viewModel.Replace(segment, segment.Start, TimeSpan.FromSeconds(21)); break;
            case "reset": viewModel.Reset(); break;
            case "initialize": viewModel.Initialize(TimeSpan.FromSeconds(100)); break;
            case "load": viewModel.LoadCutPlan(viewModel.CreateCutPlanSnapshot()); break;
        }

        AssertNoEdge(viewModel);
    }

    [Fact]
    public void SelectingForeignSegmentOrInvalidSide_DoesNotChangeTarget()
    {
        var viewModel = CreatePlan();
        var segment = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(segment, CutEdgeSide.Start);

        Assert.Throws<InvalidOperationException>(() => viewModel.SelectEdge(
            new RemoveSegment(segment.Start, segment.End), CutEdgeSide.End));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewModel.SelectEdge(segment, (CutEdgeSide)99));
        Assert.Equal(CutEdgeSide.Start, viewModel.SelectedEdgeSide);
        Assert.Equal(segment.Start, viewModel.SelectedEdgePosition);
    }

    [Theory]
    [InlineData(CutEdgeSide.Start, 11.04, 11.04, 20)]
    [InlineData(CutEdgeSide.End, 20.04, 10, 20.04)]
    public void ApplyingFrameChangesOnlyChosenBoundaryAndKeepsEdgeSelection(
        CutEdgeSide side, double position, double start, double end)
    {
        var viewModel = CreatePlan();
        var original = viewModel.RemoveSegments[0];
        var untouched = viewModel.RemoveSegments[1];
        viewModel.SelectEdge(original, side);
        viewModel.ApplyFrameEdge(original, side, TimeSpan.FromSeconds(position));

        Assert.Equal(TimeSpan.FromSeconds(start), viewModel.RemoveSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(end), viewModel.RemoveSegments[0].End);
        Assert.Same(untouched, viewModel.RemoveSegments[1]);
        Assert.Same(viewModel.RemoveSegments[0], viewModel.SelectedRemoveSegment);
        Assert.Equal(side, viewModel.SelectedEdgeSide);
        Assert.Equal(TimeSpan.FromSeconds(position), viewModel.SelectedEdgePosition);
    }

    [Theory]
    [InlineData(CutEdgeSide.Start, 20)]
    [InlineData(CutEdgeSide.Start, -0.04)]
    [InlineData(CutEdgeSide.End, 10)]
    [InlineData(CutEdgeSide.End, 30.04)]
    [InlineData(CutEdgeSide.End, 100.04)]
    public void InvalidFrameBoundaryLeavesPlanAndSelectionUnchanged(CutEdgeSide side, double position)
    {
        var viewModel = CreatePlan();
        var original = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(original, side);
        var duration = viewModel.EstimatedOutputDuration;

        Assert.ThrowsAny<Exception>(() => viewModel.ApplyFrameEdge(original, side, TimeSpan.FromSeconds(position)));
        Assert.Same(original, viewModel.RemoveSegments[0]);
        Assert.Same(original, viewModel.SelectedRemoveSegment);
        Assert.Equal(side, viewModel.SelectedEdgeSide);
        Assert.Equal(duration, viewModel.EstimatedOutputDuration);
    }

    [Fact]
    public void ApplyingUnchangedFrameKeepsOriginalSegment()
    {
        var viewModel = CreatePlan();
        var original = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(original, CutEdgeSide.End);
        viewModel.ApplyFrameEdge(original, CutEdgeSide.End, original.End);
        Assert.Same(original, viewModel.RemoveSegments[0]);
        Assert.Equal(CutEdgeSide.End, viewModel.SelectedEdgeSide);
    }

    [Fact]
    public void StaleFrameResultCannotModifyAReplacedPlan()
    {
        var viewModel = CreatePlan();
        var original = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(original, CutEdgeSide.End);
        viewModel.LoadCutPlan(viewModel.CreateCutPlanSnapshot());
        var current = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(current, CutEdgeSide.End);

        Assert.Throws<InvalidOperationException>(() => viewModel.ApplyFrameEdge(original,
            CutEdgeSide.End, TimeSpan.FromSeconds(20.04)));
        Assert.Same(current, viewModel.RemoveSegments[0]);
        Assert.Equal(TimeSpan.FromSeconds(20), current.End);
    }

    [Fact]
    public void WrongSideCannotModifyTheCapturedEdge()
    {
        var viewModel = CreatePlan();
        var original = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(original, CutEdgeSide.End);
        Assert.Throws<InvalidOperationException>(() => viewModel.ApplyFrameEdge(original,
            CutEdgeSide.Start, TimeSpan.FromSeconds(11)));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewModel.ApplyFrameEdge(original,
            (CutEdgeSide)99, TimeSpan.FromSeconds(11)));
        Assert.Same(original, viewModel.RemoveSegments[0]);
    }

    [Fact]
    public void ExportSnapshotUsesCorrectedKeepBoundariesWithoutFrameOffset()
    {
        var viewModel = new CutPlanViewModel();
        viewModel.Initialize(TimeSpan.FromSeconds(4497.76));
        viewModel.SetStart(TimeSpan.Zero);
        viewModel.SetEnd(TimeSpan.FromSeconds(582.16));
        viewModel.SetStart(TimeSpan.FromSeconds(1902.96));
        viewModel.SetEnd(TimeSpan.FromSeconds(2551.32));
        viewModel.SetStart(TimeSpan.FromSeconds(3645.4));
        viewModel.SetEnd(TimeSpan.FromSeconds(4497.76));
        var first = viewModel.RemoveSegments[0];
        viewModel.SelectEdge(first, CutEdgeSide.End);
        viewModel.ApplyFrameEdge(first, CutEdgeSide.End, TimeSpan.FromSeconds(582.2));
        var ad = viewModel.RemoveSegments[1];
        viewModel.SelectEdge(ad, CutEdgeSide.Start);
        viewModel.ApplyFrameEdge(ad, CutEdgeSide.Start, TimeSpan.FromSeconds(1902.88));

        var keep = CutlistKeepSegmentBuilder.Build(viewModel.CreateCutPlanSnapshot());
        Assert.Equal(2, keep.Count);
        Assert.Equal(TimeSpan.FromSeconds(582.2), keep[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(1320.68), keep[0].Duration);
        Assert.Equal(TimeSpan.FromSeconds(2551.32), keep[1].Start);
        Assert.Equal(TimeSpan.FromSeconds(1094.08), keep[1].Duration);
    }

    private static CutPlanViewModel CreatePlan()
    {
        var viewModel = new CutPlanViewModel();
        viewModel.Initialize(TimeSpan.FromSeconds(100));
        viewModel.SetStart(TimeSpan.FromSeconds(10));
        viewModel.SetEnd(TimeSpan.FromSeconds(20));
        viewModel.SetStart(TimeSpan.FromSeconds(30));
        viewModel.SetEnd(TimeSpan.FromSeconds(40));
        return viewModel;
    }

    private static void AssertNoEdge(CutPlanViewModel viewModel)
    {
        Assert.Null(viewModel.SelectedEdgeSide);
        Assert.Null(viewModel.SelectedEdgePosition);
        Assert.False(viewModel.CanInspectSelectedEdge);
    }
}
