using System.Globalization;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxSplitRangeFormatterTests
{
    [Fact]
    public void Format_PeacemakerFirstKeepSegment_MatchesClassicCutAssistant()
    {
        var range =
            Mp4BoxSplitRangeBuilder.Build(
                TimeSpan.FromSeconds(519.8746667),
                TimeSpan.FromSeconds(457.161),
                25);

        var result =
            Mp4BoxSplitRangeFormatter.Format(
                range);

        Assert.Equal(
            "519.8746667:976.9956667",
            result);
    }

    [Fact]
    public void Format_PeacemakerSecondKeepSegment_MatchesClassicCutAssistant()
    {
        var range =
            Mp4BoxSplitRangeBuilder.Build(
                TimeSpan.FromSeconds(1460.0823334),
                TimeSpan.FromSeconds(1152.239),
                25);

        var result =
            Mp4BoxSplitRangeFormatter.Format(
                range);

        Assert.Equal(
            "1460.0823334:2612.2813334",
            result);
    }

    [Fact]
    public void Format_WithGermanCulture_UsesInvariantDecimalPoint()
    {
        var originalCulture =
            CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture =
                CultureInfo.GetCultureInfo("de-DE");

            var range =
                new Mp4BoxSplitRange(
                    TimeSpan.FromSeconds(1.5),
                    TimeSpan.FromSeconds(2.5));

            var result =
                Mp4BoxSplitRangeFormatter.Format(
                    range);

            Assert.Equal(
                "1.5:2.5",
                result);
        }
        finally
        {
            CultureInfo.CurrentCulture =
                originalCulture;
        }
    }

    [Fact]
    public void Format_WithNullRange_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () =>
                Mp4BoxSplitRangeFormatter.Format(
                    null!));
    }

    [Fact]
    public void Format_SlidersSecondKeepSegment_MatchesClassicCutAssistant()
    {
        var range =
            Mp4BoxSplitRangeBuilder.Build(
                TimeSpan.FromSeconds(1762.44),
                TimeSpan.FromSeconds(1215.12),
                25);

        var result =
            Mp4BoxSplitRangeFormatter.Format(
                range);

        Assert.Equal(
            "1762.44:2977.52",
            result);
    }

    [Fact]
    public void Format_SlidersThirdKeepSegment_MatchesClassicCutAssistant()
    {
        var range =
            Mp4BoxSplitRangeBuilder.Build(
                TimeSpan.FromSeconds(3460.76),
                TimeSpan.FromSeconds(607.68),
                25);

        var result =
            Mp4BoxSplitRangeFormatter.Format(
                range);

        Assert.Equal(
            "3460.76:4068.4",
            result);
    }}
