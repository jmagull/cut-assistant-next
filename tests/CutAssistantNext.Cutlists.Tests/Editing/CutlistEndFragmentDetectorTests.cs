using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Tests.Editing;

public sealed class CutlistEndFragmentDetectorTests
{
    [Fact]
    public void Find_WithTwoFrameFragmentAtMediaEnd_ReturnsFragment()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
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

        Assert.Same(
            cuts[1],
            fragment);
    }

    [Fact]
    public void Find_WithNormalKeepSegmentAtMediaEnd_ReturnsNull()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(95),
                    TimeSpan.FromSeconds(5))
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

        Assert.Null(
            fragment);
    }

    [Fact]
    public void Find_WithTwoFrameFragmentAtFiftyFramesPerSecond_ReturnsFragment()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(99.96),
                    TimeSpan.FromSeconds(0.04))
            };

        var document =
            new CutlistDocument(
                new CutlistGeneralMetadata
                {
                    FramesPerSecond = 50,
                    NoOfCuts = cuts.Length
                },
                cuts,
                new CutlistInfoMetadata());

        var fragment =
            CutlistEndFragmentDetector.Find(
                document,
                TimeSpan.FromSeconds(100));

        Assert.Same(
            cuts[1],
            fragment);
    }

    [Fact]
    public void Find_WithTwoFrameFragmentEndingOneFrameBeyondMediaEnd_ReturnsFragment()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(99.96),
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

        Assert.Same(
            cuts[1],
            fragment);
    }
}
