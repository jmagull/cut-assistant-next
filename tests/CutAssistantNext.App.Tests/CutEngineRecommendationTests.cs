using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests;

public sealed class CutEngineRecommendationTests
{
    [Theory]
    [InlineData(1.0, true, true, "MP4Box")]
    [InlineData(1.04, true, true, "otr-can")]
    [InlineData(1.0, false, true, "Keine eindeutige")]
    [InlineData(1.04, true, false, "Keine eindeutige")]
    public void RecommendsOnlyAvailableMotorSuitedToBoundaries(double start, bool mp4, bool otr, string expected)
    {
        var duration = TimeSpan.FromSeconds(3);
        var plan = new CutPlan(duration);
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(start), TimeSpan.FromSeconds(2)));
        var media = new MediaAnalysisResult("mov,mp4", null, null, duration,
            [new(0, "h264", null, null, null, null, null, 25, null)],
            [new(1, "aac", null, 48000, 2, null), new(2, "ac3", null, 48000, 2, null)]);
        var window = new VideoFrameWindow(0, new(1, 25), 0, 0,
            [new(0, 0, null, null, null, true, "I"), new(0, 25, null, null, null, true, "I"), new(0, 50, null, null, null, true, "I")]);
        var keyframes = VideoKeyframeIndex.Create(window, 0, duration, 0);

        Assert.Contains(expected, CutEngineRecommendation.Evaluate(media, plan, keyframes, mp4, otr));
        Assert.StartsWith("Keine eindeutige", CutEngineRecommendation.Evaluate(media, plan, null, mp4, otr));
        Assert.StartsWith("Keine eindeutige", CutEngineRecommendation.Evaluate(media with { FormatName = "avi" }, plan, keyframes, mp4, otr));
    }
}
