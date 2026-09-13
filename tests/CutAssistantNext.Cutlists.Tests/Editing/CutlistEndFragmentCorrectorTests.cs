using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Tests.Editing;

public sealed class CutlistEndFragmentCorrectorTests
{
    [Fact]
    public void BuildCutPlan_RemovesDetectedEndFragment()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(50),
                    TimeSpan.FromSeconds(20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(99.92),
                    TimeSpan.FromSeconds(0.08))
            };

        var document =
            new CutlistDocument(
                new CutlistGeneralMetadata
                {
                    FramesPerSecond = 25,
                    NoOfCuts = cuts.Length
                },
                cuts,
                new CutlistInfoMetadata());

        var fragment =
            CutlistEndFragmentDetector.Find(
                document,
                TimeSpan.FromSeconds(100));

        Assert.NotNull(
            fragment);

        var cutPlan =
            CutlistEndFragmentCorrector.BuildCutPlan(
                document,
                TimeSpan.FromSeconds(100),
                fragment);

        Assert.Equal(
            3,
            cutPlan.RemoveSegments.Count);

        Assert.Equal(
            TimeSpan.FromSeconds(70),
            cutPlan.RemoveSegments[2].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(100),
            cutPlan.RemoveSegments[2].End);
    }
}
