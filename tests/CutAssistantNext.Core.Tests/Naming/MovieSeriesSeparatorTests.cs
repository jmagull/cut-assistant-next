using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.Core.Tests.Naming;

public sealed class MovieSeriesSeparatorTests
{
    private const string Mask = "%Name% - %S%%Staffel%%E%%Folge% - %Folgentitel%";

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("", "", "")]
    [InlineData(" \t", "\t", " ")]
    public void EmptySeriesValuesEndAtTheMovieTitle(string? season, string? episode, string? episodeTitle)
    {
        var context = new NameTemplateContext(Name: "Ghostbusters", Season: season, Episode: episode,
            EpisodeTitle: episodeTitle);
        Assert.Equal("Ghostbusters", NameTemplateRenderer.Render(Mask, context));
    }

    [Theory]
    [InlineData("%Name% - %Staffel:S%%Folge:E% - %Folgentitel%")]
    [InlineData("%Name% - %Staffel%%x%%Folge% - %Folgentitel%")]
    [InlineData("%Name% / %Staffel% | %Folge% : %Folgentitel%")]
    [InlineData("%Name%_%Staffel%_%Folge%_%Folgentitel%")]
    [InlineData("%Name% – %S%%Staffel% — %E%%Folge% — %Folgentitel%")]
    [InlineData("%Name% - (%S%%Staffel%%E%%Folge%) - %Folgentitel%")]
    [InlineData("%Name% [%Staffel:S%-%Folge:E%] [%Folgentitel%]")]
    [InlineData("%Name% - %Folgentitel% - %Folge% - %Staffel% -")]
    public void EmptyModernLegacyAndReorderedSeriesPartsDoNotLeaveSeparators(string mask)
    {
        Assert.Equal("Ghostbusters", NameTemplateRenderer.Render(mask, new(Name: "Ghostbusters")));
    }

    [Theory]
    [InlineData("01", null, null, "Kimi - S01 -")]
    [InlineData(null, "03", null, "Kimi - E03 -")]
    [InlineData(null, null, "Der Besuch", "Kimi - - Der Besuch")]
    [InlineData("01", "03", null, "Kimi - S01E03 -")]
    [InlineData("01", null, "Der Besuch", "Kimi - S01 - Der Besuch")]
    [InlineData(null, "03", "Der Besuch", "Kimi - E03 - Der Besuch")]
    [InlineData("01", "03", "Der Besuch", "Kimi - S01E03 - Der Besuch")]
    public void PartialAndCompleteSeriesDataKeepTheirExistingOutputs(string? season, string? episode,
        string? episodeTitle, string expected)
    {
        Assert.Equal(expected, NameTemplateRenderer.Render(Mask,
            new(Name: "Kimi", Season: season, Episode: episode, EpisodeTitle: episodeTitle)));
    }

    [Theory]
    [InlineData("%Name% - %S%%Staffel%%E%%Folge% - %Folgentitel% [%Tag%.%Monat%.%YYYY%]", "Ghostbusters")]
    [InlineData("%Name% [%Tag%.%Monat%.%YYYY%] - %Staffel:S%%Folge:E% - %Folgentitel%", "Ghostbusters")]
    [InlineData("%Name% - %Staffel% | %Sender% - %Folgentitel%", "Ghostbusters")]
    [InlineData("%Name% - %S%%Staffel%%E%%Folge% - %Folgentitel% - %Sender%", "Ghostbusters")]
    [InlineData("%Name% - %Folgentitel% (%Sender%)", "Ghostbusters")]
    [InlineData("%Name% - %Folgentitel% - %OriginalName%", "Ghostbusters")]
    [InlineData("%Name% - %Folgentitel% - %Serie%", "Ghostbusters")]
    [InlineData("%Name% - %S%%Staffel% - %E%%Folge% - %Folgentitel% - Director's Cut", "Ghostbusters")]
    [InlineData("%Name% - %Folgentitel%.mp4", "Ghostbusters")]
    public void AllFieldsAndLiteralTextAfterTheMovieTitleAreOmitted(string mask, string expected)
    {
        var context = new NameTemplateContext(Name: "Ghostbusters", Year: "2026", Month: "10", Day: "14",
            Sender: "ARD", OriginalName: "original.HQ.mp4", Series: "Sammlung");
        Assert.Equal(expected, NameTemplateRenderer.Render(mask, context));
    }

    [Fact]
    public void EmptyAdditionalFieldIsInspectedBeforeLeavingATrailingSeparator()
    {
        Assert.Equal("Ghostbusters", NameTemplateRenderer.Render(Mask + " - %Sender%", new(Name: "Ghostbusters")));
    }

    [Fact]
    public void EmptyDateBracketsAreAlsoOmittedForMovies()
    {
        Assert.Equal("Ghostbusters", NameTemplateRenderer.Render(Mask + " [%Tag%.%Monat%.%YYYY%]",
            new(Name: "Ghostbusters")));
    }

    [Theory]
    [InlineData("Ghostbusters -", "%Name% - %Folgentitel%", "Ghostbusters -")]
    [InlineData("Ghostbusters", "%Name:Film - % - %Folgentitel%", "Film - Ghostbusters")]
    [InlineData("Ghostbusters", "%Sender:Kanal - % | %Name% - %Folgentitel%", "Kanal - ARD | Ghostbusters")]
    [InlineData("Ghostbusters", "%Name% - | S E x", "Ghostbusters")]
    [InlineData("Ghostbusters", "%Name% S%Staffel%E%Folge%", "Ghostbusters")]
    public void TitlePunctuationAndPrefixesAreRetainedWhileMovieSuffixesAreOmitted(string name, string mask, string expected)
    {
        Assert.Equal(expected, NameTemplateRenderer.Render(mask, new(Name: name, Sender: "ARD")));
    }

    [Theory]
    [InlineData("%Name% - %Folgentitel% - %Unbekannt%")]
    [InlineData("%Name% - %Folgentitel% - %Unbekannt: - %")]
    [InlineData("%Name% - %S:ungueltig%")]
    public void EmptySeriesDoesNotHideInvalidTokens(string mask)
    {
        Assert.Throws<ArgumentException>(() => NameTemplateRenderer.Render(mask, new(Name: "Ghostbusters")));
    }
}
