using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.Tests.Editing;

public sealed class CutlistRangeValidationTests
{
    [Theory]
    [InlineData(10, 40, 20, 10)]
    [InlineData(10, 20, 20, 30)]
    [InlineData(40, 10, 10, 10)]
    [InlineData(10, 10, 10, 10)]
    public void Parse_WithConflictingRanges_RejectsCutlist(
        int start1, int duration1, int start2, int duration2)
    {
        var exception = Assert.Throws<InvalidDataException>(() =>
            CutlistParser.Parse(Content(start1, duration1, start2, duration2)));

        Assert.Equal(
            "Diese Cutlist enthält überlappende oder falsch sortierte Schnittbereiche " +
            "und wurde nicht geladen. Bitte wähle eine andere Cutlist.",
            exception.Message);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(30)]
    public void Parse_WithAdjacentOrSeparatedRanges_AcceptsCutlist(int secondStart)
    {
        var document = CutlistParser.Parse(Content(10, 10, secondStart, 10));
        Assert.Equal(2, document.Cuts.Count);
    }

    private static string Content(int start1, int duration1, int start2, int duration2) =>
        $"""
        [General]
        NoOfCuts=2
        [Info]
        RatingByAuthor=5
        [Cut0]
        Start={start1}
        Duration={duration1}
        [Cut1]
        Start={start2}
        Duration={duration2}
        """;
}
