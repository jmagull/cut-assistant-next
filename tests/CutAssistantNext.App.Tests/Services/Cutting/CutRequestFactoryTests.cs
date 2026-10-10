using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class CutRequestFactoryTests
{
    [Fact]
    public void Create_PreservesKeepTimesWithoutMp4BoxFrameAdjustment()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(2612.3213334));
        plan.Add(new RemoveSegment(TimeSpan.Zero, TimeSpan.FromSeconds(519.8746667)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(977.0356667), TimeSpan.FromSeconds(1460.0823334)));

        var request = CutRequestFactory.Create("source.mp4", "output.mp4", plan, 25);

        Assert.Equal(TimeSpan.FromSeconds(519.8746667), request.KeepSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(977.0356667), request.KeepSegments[0].End);
        Assert.Equal(TimeSpan.FromSeconds(1460.0823334), request.KeepSegments[1].Start);
        Assert.Equal(TimeSpan.FromSeconds(2612.3213334), request.KeepSegments[1].End);
        Assert.Equal(Path.GetFullPath("source.mp4"), request.OriginalFilePath);
    }

    [Fact]
    public void Create_KeepsSnapshotWhenPlanChangesLater()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(30));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20)));
        var request = CutRequestFactory.Create("source.mp4", "output.mp4", plan);
        plan.Add(new RemoveSegment(TimeSpan.Zero, TimeSpan.FromSeconds(5)));

        Assert.Equal(TimeSpan.Zero, request.KeepSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(10), request.KeepSegments[0].End);
        Assert.Equal(2, request.KeepSegments.Count);
    }

    [Fact]
    public void Create_WholeMovieRemoved_RejectsEmptyCutWithoutToolCalls()
    {
        var plan = new CutPlan(TimeSpan.FromSeconds(30));
        plan.Add(new RemoveSegment(TimeSpan.Zero, plan.MediaDuration));
        Assert.Throws<ArgumentException>(() => CutRequestFactory.Create("source.mp4", "output.mp4", plan));
    }
}
