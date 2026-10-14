using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.Core.Tests.Naming;

public sealed class IndependentSeriesTokenTests
{
    [Theory]
    [InlineData("1", "1", "S1E1")]
    [InlineData("01", "01", "S01E01")]
    [InlineData("04", "01", "S04E01")]
    [InlineData("02", "13", "S02E13")]
    public void IndependentLettersAndValuesPreserveEnteredZeros(string season, string episode, string expected)
    {
        Assert.Equal(expected, NameTemplateRenderer.Render("%S%%Staffel%%E%%Folge%",
            new NameTemplateContext(Season: season, Episode: episode)));
    }

    [Theory]
    [InlineData("1", "02", "1x02")]
    [InlineData("4", "01", "4x01")]
    [InlineData("04", "01", "04x01")]
    [InlineData("01", "02", "01x02")]
    public void LowerCaseXWorksWithEnteredValues(string season, string episode, string expected)
    {
        Assert.Equal(expected, NameTemplateRenderer.Render("%Staffel%%x%%Folge%",
            new NameTemplateContext(Season: season, Episode: episode)));
    }

    [Theory]
    [InlineData(null, null, "Film")]
    [InlineData("", "", "Film")]
    [InlineData(" ", "\t", "Film")]
    [InlineData("01", null, "Film S01 [21.01.2021]")]
    [InlineData(null, "02", "Film E02 [21.01.2021]")]
    [InlineData(" ", "02", "Film E02 [21.01.2021]")]
    [InlineData("01", " ", "Film S01 [21.01.2021]")]
    public void MissingSeriesDataDoesNotLeaveLettersOrJoinTitleToEpisode(string? season, string? episode, string expected)
    {
        var context = new NameTemplateContext(Name: "Film", Season: season, Episode: episode,
            Year: "2021", Month: "01", Day: "21");
        Assert.Equal(expected, NameTemplateRenderer.Render(
            "%Name% %S%%Staffel%%E%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]", context));
    }

    [Theory]
    [InlineData(null, null, "Film")]
    [InlineData("4", null, "Film 4")]
    [InlineData(null, "01", "Film 01")]
    [InlineData(" ", "01", "Film 01")]
    [InlineData("4", " ", "Film 4")]
    public void XOmittedUnlessBothValuesExistAndRetainsBoundarySpace(string? season, string? episode, string expected)
    {
        Assert.Equal(expected, NameTemplateRenderer.Render("%Name% %Staffel%%x%%Folge%",
            new NameTemplateContext(Name: "Film", Season: season, Episode: episode)));
    }

    [Fact]
    public void MoviesKeepLiteralPrefixesButOmitAllTextAfterTheTitle()
    {
        Assert.Equal("Film", NameTemplateRenderer.Render(
            "%Name% %S%%Staffel% - %E%%Folge% | S E x", new NameTemplateContext(Name: "Film")));
        Assert.Equal("Vorspann - Film", NameTemplateRenderer.Render(
            "Vorspann - %Name% - | S E x", new NameTemplateContext(Name: "Film")));
        Assert.Equal("02-13", NameTemplateRenderer.Render("%Staffel%-%Folge%",
            new NameTemplateContext(Season: "02", Episode: "13")));
    }

    [Fact]
    public void IndependentBlocksCanBeReorderedAndUsedSeparately()
    {
        var context = new NameTemplateContext(Season: "04", Episode: "01");
        Assert.Equal("01E_04S", NameTemplateRenderer.Render("%Folge%%E%_%Staffel%%S%", context));
        Assert.Equal("S", NameTemplateRenderer.Render("%S%", context));
        Assert.Equal("x", NameTemplateRenderer.Render("%x%", context));
        Assert.Equal(string.Empty, NameTemplateRenderer.Render("%x%", new NameTemplateContext(Season: "04")));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("1", "1")]
    [InlineData("01", "01")]
    [InlineData(null, "01")]
    [InlineData("04", null)]
    public void OldPrefixedMasksKeepTheirExistingOutputs(string? season, string? episode)
    {
        var context = new NameTemplateContext(Name: "Serie", Season: season, Episode: episode);
        var expected = "Serie" + (season is null && episode is null ? "" : " ") +
            (season is null ? "" : "S" + season) + (episode is null ? "" : "E" + episode);
        Assert.Equal(expected, NameTemplateRenderer.Render("%Name% %Staffel:S%%Folge:E%", context));
    }
}
