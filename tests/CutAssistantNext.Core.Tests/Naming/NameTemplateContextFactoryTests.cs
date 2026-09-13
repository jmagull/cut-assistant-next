using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.Core.Tests.Naming;

public sealed class NameTemplateContextFactoryTests
{
    [Fact]
    public void Create_WithOtrFileName_UsesParsedNamingValues()
    {
        var context =
            NameTemplateContextFactory.Create(
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi");

        Assert.Equal(
            "Sliders Perfekte Piloten",
            context.Name);

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
            "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi",
            context.OriginalName);
    }

    [Fact]
    public void Create_WithNonOtrFileName_UsesFileNameFallback()
    {
        const string fileName =
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026].mp4";

        var context =
            NameTemplateContextFactory.Create(
                fileName);

        Assert.Equal(
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026]",
            context.Name);

        Assert.Equal(
            fileName,
            context.OriginalName);
    }
}
