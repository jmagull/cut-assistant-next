using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class CutPlanMp4BoxRangeBuilderTests
{
    [Fact]
    public void Build_PeacemakerCutPlan_CreatesClassicMp4BoxRanges()
    {
        var cutPlan =
            new CutPlan(
                TimeSpan.FromSeconds(2612.3213334));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(519.8746667)));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(977.0356667),
                TimeSpan.FromSeconds(1460.0823334)));

        var ranges =
            CutPlanMp4BoxRangeBuilder.Build(
                cutPlan,
                25);

        Assert.Equal(
            2,
            ranges.Count);

        Assert.Equal(
            TimeSpan.FromSeconds(519.8746667),
            ranges[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(976.9956667),
            ranges[0].End);

        Assert.Equal(
            TimeSpan.FromSeconds(1460.0823334),
            ranges[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(2612.2813334),
            ranges[1].End);
    }
}
