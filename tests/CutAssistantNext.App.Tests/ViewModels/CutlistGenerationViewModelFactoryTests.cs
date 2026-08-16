using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutlistGenerationViewModelFactoryTests
{
    [Fact]
    public void Create_WithOtrFileName_UsesParsedNamingValuesAndTechnicalNotices()
    {
        var settings = new CutlistSettings
        {
            DefaultNameTemplate =
                "%Name% [%Tag%.%Monat%.%YYYY%]",
            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var analysis =
            new MediaAnalysisResult(
                FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
                FormatLongName: "QuickTime / MOV",
                FileSizeBytes: 734003200,
                Duration: TimeSpan.FromSeconds(100),
                VideoStreams: [],
                AudioStreams: []);

        var viewModel =
            CutlistGenerationViewModelFactory.Create(
                settings,
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi",
                analysis);

        Assert.Equal(
            "Sliders Perfekte Piloten [13.08.2026]",
            viewModel.SuggestedMovieName);

        var notice =
            Assert.Single(
                viewModel.TechnicalNotices);

        Assert.Equal(
            "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.",
            notice.Description);

        Assert.Equal(
            string.Empty,
            viewModel.UserComment);
    }

    [Fact]
    public void Create_WithNonOtrFileName_UsesFileNameFallback()
    {
        var settings = new CutlistSettings
        {
            DefaultNameTemplate =
                "%Name% | %OriginalName%",
            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var analysis =
            new MediaAnalysisResult(
                FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
                FormatLongName: "QuickTime / MOV",
                FileSizeBytes: 734003200,
                Duration: TimeSpan.FromSeconds(100),
                VideoStreams: [],
                AudioStreams: []);

        const string fileName =
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026].mp4";

        var viewModel =
            CutlistGenerationViewModelFactory.Create(
                settings,
                fileName,
                analysis);

        Assert.Equal(
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026] | " +
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026].mp4",
            viewModel.SuggestedMovieName);

        Assert.Empty(
            viewModel.TechnicalNotices);
    }
}
