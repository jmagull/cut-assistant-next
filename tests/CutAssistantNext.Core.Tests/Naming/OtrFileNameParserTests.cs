using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.Core.Tests.Naming;

public sealed class OtrFileNameParserTests
{
    [Fact]
    public void Parse_WithTypicalOtrFileName_ReturnsNamingValues()
    {
        var context =
            OtrFileNameParser.Parse(
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.mp4");

        Assert.Equal(
            "Sliders Perfekte Piloten",
            context.Name);

        Assert.Equal(
            "26",
            context.ShortYear);

        Assert.Equal(
            "2026",
            context.Year);

        Assert.Equal(
            "08",
            context.Month);

        Assert.Equal(
            "13",
            context.Day);

        Assert.Equal(
            "17",
            context.Hour);

        Assert.Equal(
            "10",
            context.Minute);

        Assert.Equal(
            "tele5",
            context.Sender);

        Assert.Null(
            context.Season);

        Assert.Null(
            context.Episode);

        Assert.Null(
            context.Series);

        Assert.Null(
            context.EpisodeTitle);
    }

    [Fact]
    public void Parse_WithRepeatedUnderscores_NormalizesName()
    {
        const string fileName =
            "Greys_Anatomy__Die_jungen_Aerzte__Runter_damit_26.08.12_20-15_sixx_55_TVOON_DE.HQ.mp4";

        var context =
            OtrFileNameParser.Parse(
                fileName);

        Assert.Equal(
            "Greys Anatomy Die jungen Aerzte Runter damit",
            context.Name);

        Assert.Equal(
            "2026",
            context.Year);

        Assert.Equal(
            "08",
            context.Month);

        Assert.Equal(
            "12",
            context.Day);

        Assert.Equal(
            "20",
            context.Hour);

        Assert.Equal(
            "15",
            context.Minute);

        Assert.Equal(
            "sixx",
            context.Sender);

        Assert.Equal(
            fileName,
            context.OriginalName);
    }

    [Fact]
    public void TryParse_WithNonOtrFileName_ReturnsFalseAndNullContext()
    {
        var success =
            OtrFileNameParser.TryParse(
                "Greys Anatomy S22E09 [05.08.2026].mp4",
                out var context);

        Assert.False(
            success);

        Assert.Null(
            context);
    }
    [Fact]
    public void Parse_WithNonOtrFileName_ThrowsFormatException()
    {
        var exception =
            Assert.Throws<FormatException>(
                () => OtrFileNameParser.Parse(
                    "Greys Anatomy S22E09 [05.08.2026].mp4"));

        Assert.Equal(
            "Der OTR-Dateiname konnte nicht ausgewertet werden: " +
            "Greys Anatomy S22E09 [05.08.2026].mp4",
            exception.Message);
    }
}
