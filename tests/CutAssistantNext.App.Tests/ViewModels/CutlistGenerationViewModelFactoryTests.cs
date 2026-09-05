using CutAssistantNext.App.State;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Naming;
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

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var namingSettings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% [%Tag%.%Monat%.%YYYY%]"
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
                namingSettings,
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
            "Mit Cut Assistant Next geschnitten.",
            viewModel.UserComment);
    }

    [Fact]
    public void Create_WithNonOtrFileName_UsesFileNameFallback()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var namingSettings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% | %OriginalName%"
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
                namingSettings,
                fileName,
                analysis);

        Assert.Equal(
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026] | " +
            "Greys Anatomy S22E09 Ploetzlich Onkel [05.08.2026].mp4",
            viewModel.SuggestedMovieName);

        Assert.Empty(
            viewModel.TechnicalNotices);
    }

    [Fact]
    public void Create_WithNamingState_UsesCompleteExistingNamingState()
    {
        var settings = new CutlistSettings
        {

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

        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Year: "2026",
                Month: "08",
                Day: "19",
                Season: "04",
                Episode: "20",
                OriginalName: "Sliders_original.mp4",
                Hour: "17",
                Minute: "15",
                Sender: "tele5",
                Series: "Sliders",
                EpisodeTitle: "Der Abgrund");

        var namingState =
            new CutNamingState(
                "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
                context)
            .UseSuggestedMovieName(
                "Sliders S04E20 - Der Abgrund [19.08.2026]");

        var viewModel =
            CutlistGenerationViewModelFactory.Create(
                settings,
                "Sliders_original.mp4",
                analysis,
                namingState);

        Assert.Equal(
            namingState.NameTemplate,
            viewModel.NameTemplate);

        Assert.Equal(
            namingState.SuggestedMovieName,
            viewModel.SuggestedMovieName);

        Assert.Equal(
            "Sliders",
            viewModel.Name);

        Assert.Equal(
            "04",
            viewModel.Season);

        Assert.Equal(
            "20",
            viewModel.Episode);

        Assert.Equal(
            "Der Abgrund",
            viewModel.EpisodeTitle);
    }
}
