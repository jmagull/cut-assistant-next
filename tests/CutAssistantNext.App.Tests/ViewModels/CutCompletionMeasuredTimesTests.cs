using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutCompletionMeasuredTimesTests
{
    [Fact]
    public void VeraCompletionAndClipboardUseMeasuredOutputNotPlannedKeepLengths()
    {
        var identity = new CutOutputIdentity(1, 1, new string('A', 64));
        var result = new CutResult("vera.mp4", new(TimeSpan.FromSeconds(5292.8),
            [TimeSpan.FromSeconds(1020.72), TimeSpan.FromSeconds(2120.84), TimeSpan.FromSeconds(3913.88)], identity),
            CutReviewTimingKind.Measured, "Fehlende Bilder werden nicht wiederhergestellt.");
        var completion = new CutCompletionViewModel(result);
        Assert.True(completion.AreTransitionsMeasured);
        Assert.Equal(["00:17:00.720", "00:35:20.840", "01:05:13.880"], completion.Transitions.Select(p => p.PositionText));
        Assert.DoesNotContain("00:17:04.800", completion.ClipboardText);
        Assert.Contains("geprüft", completion.TimeReferenceText);
        Assert.Contains("nicht wiederhergestellt", completion.ClipboardText);
        Assert.Equal(result.Timeline.Duration, completion.CheckPoints[^1].Position);
    }

    [Fact]
    public void FallbackIsExplicitInCompletionAndClipboard()
    {
        var result = CutResult.Planned("film.mp4",
            [new KeepSegment(TimeSpan.Zero, TimeSpan.FromSeconds(10)), new KeepSegment(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10))],
            "Die Übergangsprüfung wurde abgebrochen.", analysisFailed: true);
        var completion = new CutCompletionViewModel(result);
        Assert.False(completion.AreTransitionsMeasured);
        Assert.Contains("nicht bestätigt", completion.TimeReferenceText);
        Assert.Contains("abgebrochen", completion.ClipboardText);
        Assert.Equal(TimeSpan.FromSeconds(10), completion.Transitions[0].Position);
    }

    [Fact]
    public void LocatedOutputTimesAreNotPresentedAsMeasuredOrPlanTimes()
    {
        var identity = new CutOutputIdentity(1, 1, new string('A', 64));
        var result = new CutResult("out.mp4", new(TimeSpan.FromSeconds(20), [TimeSpan.FromSeconds(7)], identity, true),
            CutReviewTimingKind.Located, "Bitte die Übergänge kontrollieren.");
        var completion = new CutCompletionViewModel(result);
        Assert.False(completion.AreTransitionsMeasured);
        Assert.Contains("zugeordnet", completion.TimeReferenceText);
        Assert.Contains("überlappen", completion.ClipboardText);
        Assert.DoesNotContain("aus dem Schnittplan", completion.ClipboardText);
        Assert.Equal("00:00:07.000", Assert.Single(completion.Transitions).PositionText);
    }
}
