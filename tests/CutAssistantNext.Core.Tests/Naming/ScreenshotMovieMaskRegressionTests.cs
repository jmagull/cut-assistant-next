using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.Core.Tests.Naming;

public sealed class ScreenshotMovieMaskRegressionTests
{
    private const string ScreenshotMask = "%Name% %S%%Staffel%%E%%Folge%-%Folgentitel: %- [%Tag%.%Monat%.%YYYY%]";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void EmptySeriesInTheReportedMaskReturnsOnlyTheMovieTitle(string? empty)
    {
        var context = new NameTemplateContext(Name: "Die Hart 2", Season: empty, Episode: empty,
            EpisodeTitle: empty, Year: "2026", Month: "10", Day: "12");
        Assert.Equal("Die Hart 2", NameTemplateRenderer.Render(ScreenshotMask, context));
    }

    [Fact]
    public void ReportedMaskStillRendersTheEntireSuffixForASeries()
    {
        var context = new NameTemplateContext(Name: "Kimi", Season: "01", Episode: "03",
            EpisodeTitle: "Der Besuch", Year: "2026", Month: "10", Day: "12");
        Assert.Equal("Kimi S01E03- Der Besuch- [12.10.2026]", NameTemplateRenderer.Render(ScreenshotMask, context));
    }
    [Fact]
    public void LiteralColonBetweenTokensIsNotMistakenForAPrefixedVariable()
    {
        const string mask = "%Name% / %Staffel% | %Folge% : %Folgentitel%";
        var context = new NameTemplateContext(Name: "Kimi", Season: "01", Episode: "03", EpisodeTitle: "Der Besuch");
        Assert.Equal("Kimi / 01 | 03 : Der Besuch", NameTemplateRenderer.Render(mask, context));
        Assert.Equal("Kimi", NameTemplateRenderer.Render(mask, new(Name: "Kimi")));
    }
}
