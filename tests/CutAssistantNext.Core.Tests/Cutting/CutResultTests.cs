using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Core.Tests.Cutting;

public sealed class CutResultTests
{
    [Fact]
    public void PlannedTimesAccumulateAllKeepsWithoutChangingThem()
    {
        var keeps = new[] { new KeepSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5)),
            new KeepSegment(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(7)),
            new KeepSegment(TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(2)) };
        var result = CutResult.Planned("out.mp4", keeps);
        Assert.Equal([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(12)], result.Timeline.Transitions);
        Assert.Equal(TimeSpan.FromSeconds(14), result.Timeline.Duration);
        Assert.Equal(CutReviewTimingKind.Planned, result.TimingKind);
        Assert.Equal(TimeSpan.FromSeconds(30), keeps[1].Start);
    }

    [Fact]
    public void TimelineCopiesInputAndRequiresOrderedInternalTransitions()
    {
        var values = new[] { TimeSpan.FromSeconds(2) };
        var timeline = new CutOutputTimeline(TimeSpan.FromSeconds(10), values);
        values[0] = TimeSpan.FromSeconds(8);
        Assert.Equal(TimeSpan.FromSeconds(2), timeline.Transitions[0]);
        Assert.Throws<ArgumentException>(() => new CutOutputTimeline(TimeSpan.FromSeconds(10), [TimeSpan.Zero]));
        Assert.Throws<ArgumentException>(() => new CutOutputTimeline(TimeSpan.FromSeconds(10), [TimeSpan.FromSeconds(10)]));
        Assert.Throws<ArgumentException>(() => new CutOutputTimeline(TimeSpan.FromSeconds(10), [TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(4)]));
    }

    [Fact]
    public void MeasuredResultRequiresOutputIdentityAndFallbackIsExplicit()
    {
        var timeline = new CutOutputTimeline(TimeSpan.FromSeconds(10), []);
        Assert.Throws<ArgumentException>(() => new CutResult("out.mp4", timeline, CutReviewTimingKind.Measured));
        var fallback = CutResult.Planned("out.mp4", [new KeepSegment(TimeSpan.Zero, TimeSpan.FromSeconds(10))],
            "Prüfung fehlgeschlagen", analysisFailed: true);
        Assert.Equal(CutReviewTimingKind.Estimated, fallback.TimingKind);
        Assert.Contains("fehlgeschlagen", fallback.Notice);
    }
}
