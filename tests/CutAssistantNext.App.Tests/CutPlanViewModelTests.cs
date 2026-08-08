using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests;

public sealed class CutPlanViewModelTests
{
    [Fact]
    public void Initialize_SetsMediaDurationAndStartsEmpty()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        Assert.Equal(
            TimeSpan.FromMinutes(60),
            viewModel.MediaDuration);

        Assert.Null(viewModel.PendingStart);
        Assert.Empty(viewModel.RemoveSegments);
    }

    [Fact]
    public void SetStart_StoresPendingStart()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        Assert.Equal(
            TimeSpan.FromMinutes(10),
            viewModel.PendingStart);
    }

    [Fact]
    public void SetEnd_AfterStart_AddsSegmentAndClearsPendingStart()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        var segment =
            Assert.Single(viewModel.RemoveSegments);

        Assert.Equal(
            TimeSpan.FromMinutes(10),
            segment.Start);

        Assert.Equal(
            TimeSpan.FromMinutes(20),
            segment.End);

        Assert.Null(viewModel.PendingStart);
    }

    [Fact]
    public void SetEnd_WithoutStart_ThrowsInvalidOperationException()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        Assert.Throws<InvalidOperationException>(
            () => viewModel.SetEnd(
                TimeSpan.FromMinutes(20)));
    }

    [Fact]
    public void SetEnd_BeforeStart_KeepsPendingStart()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(20));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => viewModel.SetEnd(
                TimeSpan.FromMinutes(10)));

        Assert.Equal(
            TimeSpan.FromMinutes(20),
            viewModel.PendingStart);

        Assert.Empty(viewModel.RemoveSegments);
    }

    [Fact]
    public void Initialize_AfterExistingSegment_ResetsState()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        viewModel.SetStart(
            TimeSpan.FromMinutes(30));

        viewModel.Initialize(
            TimeSpan.FromMinutes(90));

        Assert.Equal(
            TimeSpan.FromMinutes(90),
            viewModel.MediaDuration);

        Assert.Null(viewModel.PendingStart);
        Assert.Empty(viewModel.RemoveSegments);
    }
    [Fact]
    public void Remove_ExistingSegment_RemovesSegment()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        var segment =
            Assert.Single(viewModel.RemoveSegments);

        var removed =
            viewModel.Remove(segment);

        Assert.True(removed);
        Assert.Empty(viewModel.RemoveSegments);
    }

    [Fact]
    public void Replace_ExistingSegment_ReplacesSegment()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        var existingSegment =
            Assert.Single(viewModel.RemoveSegments);

        viewModel.Replace(
            existingSegment,
            TimeSpan.FromMinutes(25),
            TimeSpan.FromMinutes(35));

        var replacement =
            Assert.Single(viewModel.RemoveSegments);

        Assert.Equal(
            TimeSpan.FromMinutes(25),
            replacement.Start);

        Assert.Equal(
            TimeSpan.FromMinutes(35),
            replacement.End);
    }
    [Fact]
    public void Reset_ClearsInitializedPlan()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        viewModel.SetStart(
            TimeSpan.FromMinutes(30));

        viewModel.Reset();

        Assert.Null(viewModel.MediaDuration);
        Assert.Null(viewModel.PendingStart);
        Assert.Empty(viewModel.RemoveSegments);
    }
    [Fact]
    public void MarkerState_ReflectsInitializationAndPendingStart()
    {
        var viewModel = new CutPlanViewModel();

        Assert.False(viewModel.CanSetStart);
        Assert.False(viewModel.CanSetEnd);
        Assert.Equal("–", viewModel.PendingStartText);

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        Assert.True(viewModel.CanSetStart);
        Assert.False(viewModel.CanSetEnd);

        viewModel.SetStart(
            TimeSpan.FromSeconds(600.25));

        Assert.True(viewModel.CanSetStart);
        Assert.True(viewModel.CanSetEnd);
        Assert.Equal(
            "00:10:00.250",
            viewModel.PendingStartText);

        viewModel.Reset();

        Assert.False(viewModel.CanSetStart);
        Assert.False(viewModel.CanSetEnd);
        Assert.Equal("–", viewModel.PendingStartText);
    }

    [Fact]
    public void Selection_FollowsExplicitSelectionReplaceRemoveAndReset()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        Assert.Null(viewModel.SelectedRemoveSegment);

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        var originalSegment =
            Assert.Single(viewModel.RemoveSegments);

        Assert.Null(viewModel.SelectedRemoveSegment);
        Assert.True(viewModel.CanSetStart);
        Assert.False(viewModel.CanModifySelectedSegment);

        viewModel.SelectedRemoveSegment =
            originalSegment;

        Assert.Same(
            originalSegment,
            viewModel.SelectedRemoveSegment);
        Assert.True(viewModel.CanModifySelectedSegment);
        Assert.False(viewModel.CanSetStart);
        Assert.False(viewModel.CanSetEnd);

        viewModel.Replace(
            originalSegment,
            TimeSpan.FromMinutes(12),
            TimeSpan.FromMinutes(22));

        var replacementSegment =
            Assert.Single(viewModel.RemoveSegments);

        Assert.Same(
            replacementSegment,
            viewModel.SelectedRemoveSegment);

        Assert.True(
            viewModel.Remove(replacementSegment));

        Assert.Null(viewModel.SelectedRemoveSegment);
        Assert.True(viewModel.CanSetStart);

        viewModel.SetStart(
            TimeSpan.FromMinutes(30));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(40));

        Assert.Null(viewModel.SelectedRemoveSegment);

        viewModel.Reset();

        Assert.Null(viewModel.SelectedRemoveSegment);
    }

    [Fact]
    public void CanModifySelectedSegment_FollowsSelection()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        Assert.False(viewModel.CanModifySelectedSegment);

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        var segment =
            Assert.Single(viewModel.RemoveSegments);

        Assert.False(viewModel.CanModifySelectedSegment);

        viewModel.SelectedRemoveSegment = segment;

        Assert.True(viewModel.CanModifySelectedSegment);

        Assert.True(viewModel.Remove(segment));

        Assert.False(viewModel.CanModifySelectedSegment);
    }

    [Fact]
    public void SelectingExistingSegment_ClearsPendingStartAndDisablesNewSegmentButtons()
    {
        var viewModel = new CutPlanViewModel();

        viewModel.Initialize(
            TimeSpan.FromMinutes(60));

        viewModel.SetStart(
            TimeSpan.FromMinutes(10));

        viewModel.SetEnd(
            TimeSpan.FromMinutes(20));

        var segment =
            Assert.Single(viewModel.RemoveSegments);

        viewModel.SetStart(
            TimeSpan.FromMinutes(30));

        Assert.True(viewModel.PendingStart.HasValue);
        Assert.True(viewModel.CanSetEnd);

        viewModel.SelectedRemoveSegment = segment;

        Assert.Null(viewModel.PendingStart);
        Assert.Equal("–", viewModel.PendingStartText);
        Assert.False(viewModel.CanSetStart);
        Assert.False(viewModel.CanSetEnd);
        Assert.True(viewModel.CanModifySelectedSegment);
    }
}