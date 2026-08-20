using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Tests.Editing;

public sealed class CutlistCutPlanBuilderTests
{
    [Fact]
    public void Build_ConvertsDocumentKeepSegmentsToRemoveSegments()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(10)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(20))
            };

        var document =
            new CutlistDocument(
                new CutlistGeneralMetadata
                {
                    NoOfCuts = cuts.Length
                },
                cuts,
                new CutlistInfoMetadata());

        var cutPlan =
            CutlistCutPlanBuilder.Build(
                document,
                TimeSpan.FromSeconds(100));

        Assert.Equal(
            TimeSpan.FromSeconds(100),
            cutPlan.MediaDuration);

        Assert.Equal(
            3,
            cutPlan.RemoveSegments.Count);

        Assert.Equal(
            TimeSpan.Zero,
            cutPlan.RemoveSegments[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            cutPlan.RemoveSegments[0].End);

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            cutPlan.RemoveSegments[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(30),
            cutPlan.RemoveSegments[1].End);

        Assert.Equal(
            TimeSpan.FromSeconds(50),
            cutPlan.RemoveSegments[2].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(100),
            cutPlan.RemoveSegments[2].End);
    }
}
