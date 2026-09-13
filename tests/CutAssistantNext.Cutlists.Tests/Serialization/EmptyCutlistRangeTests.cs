using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.Tests.Serialization;

public sealed class EmptyCutlistRangeTests
{
    private static string Content(string duration = "0", string frames = "0") => $$"""
        [General]
        Application=ColdCut
        Version=1.0.8.6
        FramesPerSecond=50
        NoOfCuts=2
        ApplyToFile=sample.avi
        [Cut0]
        Start=395.52
        Duration=5356.8
        [Cut1]
        Start=5752.32
        Duration={{duration}}
        DurationFrames={{frames}}
        [Info]
        RatingByAuthor=5
        """;

    [Fact]
    public void EmptyEndMarkerIsIgnoredAndCutPlanPreserved()
    {
        var document = CutlistParser.Parse(Content());
        Assert.Single(document.Cuts);
        Assert.Equal(1, document.General.NoOfCuts);
        Assert.Equal(TimeSpan.FromSeconds(5356.8), document.Cuts[0].Duration);
        var plan = CutlistCutPlanBuilder.Build(document, TimeSpan.FromSeconds(6347.98));
        Assert.Equal(2, plan.RemoveSegments.Count);
        Assert.Equal(TimeSpan.FromSeconds(395.52), plan.RemoveSegments[0].End);
        Assert.Equal(TimeSpan.FromSeconds(5752.32), plan.RemoveSegments[1].Start);
        Assert.Single(CutlistParser.Parse(CutlistSerializer.Serialize(document)).Cuts);
    }

    [Fact]
    public void NegativeDurationIsStillRejected() =>
        Assert.ThrowsAny<ArgumentException>(() => CutlistParser.Parse(Content("-1")));

    [Fact]
    public void ContradictoryFrameDurationIsRejected() =>
        Assert.Throws<FormatException>(() => CutlistParser.Parse(Content("0", "1")));

    [Fact]
    public void OnlyEmptyRangesAreRejected() =>
        Assert.Throws<FormatException>(() => CutlistParser.Parse(Content().Replace("Duration=5356.8", "Duration=0")));
}
