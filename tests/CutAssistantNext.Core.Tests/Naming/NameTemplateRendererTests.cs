using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.Core.Tests.Naming;

public sealed class NameTemplateRendererTests
{
    [Fact]
    public void Render_WithNameAndDateTokens_ReturnsRenderedName()
    {
        var context = new NameTemplateContext(
            Name: "Tatort",
            Year: "2026",
            Month: "08",
            Day: "13");

        var result = NameTemplateRenderer.Render(
            "%Name% [%Tag%.%Monat%.%YYYY%]",
            context);

        Assert.Equal(
            "Tatort [13.08.2026]",
            result);
    }

    [Fact]
    public void Render_WithKnownEmptyToken_ReplacesTokenWithEmptyText()
    {
        var context = new NameTemplateContext(
            Name: "Frieren",
            Season: null,
            Episode: "06");

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel% %Folge%",
            context);

        Assert.Equal(
            "Frieren 06",
            result);
    }

    [Fact]
    public void Render_WithUnknownToken_ThrowsArgumentException()
    {
        var context = new NameTemplateContext(
            Name: "Tatort");

        Assert.Throws<ArgumentException>(
            () => NameTemplateRenderer.Render(
                "%Name% %Stafel%",
                context));
    }

    [Fact]
    public void Render_WithTextSeasonAndEpisode_KeepsValuesUnchanged()
    {
        var context = new NameTemplateContext(
            Name: "Tatort",
            Season: "2026",
            Episode: "01");

        var result = NameTemplateRenderer.Render(
            "%Name% S%Staffel%E%Folge%",
            context);

        Assert.Equal(
            "Tatort S2026E01",
            result);
    }

    [Fact]
    public void Render_WithOptionalPrefixAndMissingSeason_OmitsSeasonExpression()
    {
        var context = new NameTemplateContext(
            Name: "Frieren",
            Season: null,
            Episode: "06");

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel:S%%Folge:E%",
            context);

        Assert.Equal(
            "Frieren E06",
            result);
    }

    [Fact]
    public void Render_WithOptionalPrefixesAndValues_AddsPrefixes()
    {
        var context = new NameTemplateContext(
            Name: "Tatort",
            Season: "2026",
            Episode: "01");

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel:S%%Folge:E%",
            context);

        Assert.Equal(
            "Tatort S2026E01",
            result);
    }

    [Fact]
    public void Render_WithRemainingStandardTokens_ReturnsRenderedValues()
    {
        var context = new NameTemplateContext(
            OriginalName: "Tatort_Das_Verbrechen",
            ShortYear: "26",
            Hour: "20",
            Minute: "15",
            Sender: "ARD",
            Series: "Tatort",
            EpisodeTitle: "Das Verbrechen");

        var result = NameTemplateRenderer.Render(
            "%OriginalName% | %YY% | %Stunde%:%Minute% | %Sender% | %Serie% | %Folgentitel%",
            context);

        Assert.Equal(
            "Tatort_Das_Verbrechen | 26 | 20:15 | ARD | Tatort | Das Verbrechen",
            result);
    }

    [Fact]
    public void Render_WithMissingOptionalSeasonAndEpisode_OmitsBothExpressions()
    {
        var context = new NameTemplateContext(
            Name: "Frieren",
            Season: null,
            Episode: null);

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel:S%%Folge:E%",
            context);

        Assert.Equal(
            "Frieren",
            result);
    }

    [Fact]
    public void Render_WithUnknownPrefixedToken_ThrowsArgumentException()
    {
        var context = new NameTemplateContext(
            Name: "Tatort");

        Assert.Throws<ArgumentException>(
            () => NameTemplateRenderer.Render(
                "%Name% %Stafel:S%",
                context));
    }

    [Fact]
    public void Render_WithMissingValueAndOptionalSeparator_OmitsSeparator()
    {
        var context = new NameTemplateContext(
            Name: "Tatort",
            EpisodeTitle: null);

        var result = NameTemplateRenderer.Render(
            "%Name%%Folgentitel: - %",
            context);

        Assert.Equal(
            "Tatort",
            result);
    }

    [Fact]
    public void Render_WithCompleteSeriesTemplate_ReturnsExpectedName()
    {
        var context = new NameTemplateContext(
            Name: "Tatort",
            Season: "2026",
            Episode: "01",
            EpisodeTitle: "Borowski und das Haupt der Medusa",
            Year: "2026",
            Month: "08",
            Day: "13");

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
            context);

        Assert.Equal(
            "Tatort S2026E01 - Borowski und das Haupt der Medusa [13.08.2026]",
            result);
    }

    [Fact]
    public void Render_WithMovieTemplateAndMissingSeriesValues_ReturnsMovieName()
    {
        var context = new NameTemplateContext(
            Name: "Der Pate",
            Season: null,
            Episode: null,
            EpisodeTitle: null,
            Year: "2026",
            Month: "08",
            Day: "13");

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
            context);

        Assert.Equal(
            "Der Pate [13.08.2026]",
            result);
    }

    [Fact]
    public void Render_WithEpisodeButMissingSeason_KeepsSpaceBeforeEpisode()
    {
        var context = new NameTemplateContext(
            Name: "Frieren",
            Season: null,
            Episode: "06",
            EpisodeTitle: "Der Held des Dorfes",
            Year: "2026",
            Month: "08",
            Day: "13");

        var result = NameTemplateRenderer.Render(
            "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
            context);

        Assert.Equal(
            "Frieren E06 - Der Held des Dorfes [13.08.2026]",
            result);
    }
}
