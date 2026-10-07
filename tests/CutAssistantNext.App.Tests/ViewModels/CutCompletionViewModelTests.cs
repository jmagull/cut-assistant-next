using System.Globalization;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutCompletionViewModelTests
{
    [Fact]
    public void CheckPointsUseOutputPositionsInsteadOfSourceTimes()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(4000));
        plan.Add(new RemoveSegment(TimeSpan.Zero, TimeSpan.FromSeconds(582.240)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(1902.960), TimeSpan.FromSeconds(2551.320)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(3645.400), plan.MediaDuration));

        var viewModel = new CutCompletionViewModel("film.mp4", plan);

        Assert.Equal(["Anfang", "Übergang 1", "Ende"], viewModel.CheckPoints.Select(point => point.Label));
        Assert.Equal(["00:00:00.000", "00:22:00.720", "00:40:14.800"],
            viewModel.CheckPoints.Select(point => point.PositionText));
        Assert.DoesNotContain("00:42:31.320", viewModel.ClipboardText);
    }

    [Fact]
    public void MultipleTransitionsAccumulateOnlyKeptDurations()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(120));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(70)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(90), TimeSpan.FromSeconds(100)));

        var viewModel = new CutCompletionViewModel("film.mp4", plan);

        Assert.Equal([0d, 10d, 30d, 50d, 70d],
            viewModel.CheckPoints.Select(point => point.Position.TotalSeconds));
    }

    [Fact]
    public void AdjacentRemovedRangesDoNotCreatePhantomTransitions()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(100));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(60)));

        var viewModel = new CutCompletionViewModel("film.mp4", plan);

        Assert.Equal([0d, 20d, 60d], viewModel.CheckPoints.Select(point => point.Position.TotalSeconds));
    }

    [Fact]
    public void UncutFilmHasOnlyStartAndEnd()
    {
        var viewModel = new CutCompletionViewModel("film.mp4", new CutPlan(TimeSpan.FromSeconds(90)));

        Assert.Equal(["Anfang", "Ende"], viewModel.CheckPoints.Select(point => point.Label));
        Assert.Equal(TimeSpan.FromSeconds(90), viewModel.CheckPoints[^1].Position);
    }

    [Fact]
    public void LaterCorrectionsDoNotChangeCompletedCutNotes()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(100));
        var removed = new RemoveSegment(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40));
        plan.Add(removed);
        var viewModel = new CutCompletionViewModel("film.mp4", plan);
        var clipboardText = viewModel.ClipboardText;

        plan.Remove(removed);

        Assert.Equal(TimeSpan.FromSeconds(80), viewModel.CheckPoints[^1].Position);
        Assert.Equal(clipboardText, viewModel.ClipboardText);
    }

    [Fact]
    public void ClipboardContainsFileAllCheckPointsAndTimeReference()
    {
        var viewModel = new CutCompletionViewModel(@"D:\Filme\Mein Film.mp4",
            new CutPlan(TimeSpan.FromSeconds(90)));

        Assert.Equal("Mein Film.mp4", viewModel.FileName);
        Assert.Contains(viewModel.OutputFilePath, viewModel.ClipboardText);
        Assert.Contains(viewModel.TimeReferenceText, viewModel.ClipboardText);
        Assert.Contains(viewModel.CheckInstructionText, viewModel.ClipboardText);
        Assert.All(viewModel.CheckPoints,
            point => Assert.Contains($"{point.PositionText} – {point.Label}", viewModel.ClipboardText));
    }

    [Fact]
    public void TimeFormatPreservesTotalHoursAndMillisecondsAcrossCultures()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var viewModel = new CutCompletionViewModel("film.mp4",
                new CutPlan(TimeSpan.FromHours(49) + TimeSpan.FromMilliseconds(1234)));

            Assert.Equal("49:00:01.234", viewModel.CheckPoints[^1].PositionText);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Fact]
    public void FullyRemovedFilmCannotProduceCompletionNotes()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(100));
        plan.Add(new RemoveSegment(TimeSpan.Zero, plan.MediaDuration));

        Assert.Throws<ArgumentException>(() => new CutCompletionViewModel("film.mp4", plan));
    }
}
