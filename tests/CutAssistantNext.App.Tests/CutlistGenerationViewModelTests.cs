using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Core.Metadata;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Metadata;

namespace CutAssistantNext.App.Tests;

public sealed class CutlistGenerationViewModelTests
{
    [Fact]
    public void Constructor_UsesSettingsAndRendersSuggestedMovieName()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts =
            [
                "Mit Cut Assistant Next geschnitten.",
                "Werbung vollständig entfernt."
            ]
        };

        var context = new NameTemplateContext(
            Name: "Tatort",
            Year: "2026",
            Month: "08",
            Day: "13",
            Season: "2026",
            Episode: "01",
            EpisodeTitle: "Borowski und das Haupt der Medusa");

        var viewModel =
            CreateViewModel(
                settings,
                context,
                nameTemplate:
                    "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]");

        Assert.Equal(
            "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
            viewModel.NameTemplate);

        Assert.Equal(
            "joerg",
            viewModel.Author);

        Assert.Equal(
            settings.QuickTexts,
            viewModel.QuickTexts);

        Assert.Equal(
            "Tatort S2026E01 - Borowski und das Haupt der Medusa [13.08.2026]",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void ChangingNameTemplate_UpdatesSuggestedMovieName()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort",
            Year: "2026",
            Month: "08",
            Day: "13");

        var viewModel =
            CreateViewModel(
                settings,
                context,
                nameTemplate:
                    "%Name% [%Tag%.%Monat%.%YYYY%]");

        viewModel.NameTemplate =
            "%Name% - %YYYY%";

        Assert.Equal(
            "Tatort - 2026",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void ChangingNameTemplate_ToInvalidTemplate_KeepsPreviousState()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort",
            Year: "2026",
            Month: "08",
            Day: "13");

        var viewModel =
            CreateViewModel(
                settings,
                context,
                nameTemplate:
                    "%Name% [%Tag%.%Monat%.%YYYY%]");

        var originalTemplate =
            viewModel.NameTemplate;

        var originalSuggestedMovieName =
            viewModel.SuggestedMovieName;

        Assert.Throws<ArgumentException>(
            () => viewModel.NameTemplate =
                "%Name% %Stafel:S%");

        Assert.Equal(
            originalTemplate,
            viewModel.NameTemplate);

        Assert.Equal(
            originalSuggestedMovieName,
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void ChangingNameTemplate_RaisesPropertyChangedForTemplateAndSuggestedMovieName()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort",
            Year: "2026",
            Month: "08",
            Day: "13");

        var viewModel =
            CreateViewModel(
                settings,
                context,
                nameTemplate:
                    "%Name% [%Tag%.%Monat%.%YYYY%]");

        var changedProperties =
            new List<string?>();

        viewModel.PropertyChanged +=
            (_, eventArgs) =>
                changedProperties.Add(
                    eventArgs.PropertyName);

        viewModel.NameTemplate =
            "%Name% - %YYYY%";

        Assert.Contains(
            nameof(viewModel.NameTemplate),
            changedProperties);

        Assert.Contains(
            nameof(viewModel.SuggestedMovieName),
            changedProperties);
    }

    [Fact]
    public void ChangingNamingValues_UpdatesSuggestedMovieName()
    {
        var settings = new CutlistSettings
        {
            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort",
            Year: "2026",
            Month: "08",
            Day: "13",
            Season: "2026",
            Episode: "01",
            EpisodeTitle: "Alter Titel");

        var viewModel =
            CreateViewModel(
                settings,
                context,
                nameTemplate:
                    "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]");

        viewModel.Name = "Frieren";
        viewModel.Season = "02";
        viewModel.Episode = "06";
        viewModel.EpisodeTitle = "Der Held des Dorfes";

        Assert.Equal(
            "Frieren S02E06 - Der Held des Dorfes [13.08.2026]",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void CreateNamingState_UsesCompleteCurrentNamingData()
    {
        var settings = new CutlistSettings
        {
            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context =
            new NameTemplateContext(
                Name: "Alter Name",
                Year: "2026",
                Month: "08",
                Day: "19",
                Season: "01",
                Episode: "01",
                OriginalName: "Sliders_original.mp4",
                Hour: "17",
                Minute: "15",
                Sender: "tele5",
                Series: "Sliders",
                EpisodeTitle: "Alter Titel");

        var viewModel =
            CreateViewModel(
                settings,
                context,
                nameTemplate:
                    "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]");

        viewModel.Name = "Sliders";
        viewModel.Season = "04";
        viewModel.Episode = "20";
        viewModel.EpisodeTitle = "Der Abgrund";

        var state =
            viewModel.CreateNamingState();

        Assert.Equal(
            viewModel.NameTemplate,
            state.NameTemplate);

        Assert.Equal(
            viewModel.SuggestedMovieName,
            state.SuggestedMovieName);

        Assert.Equal(
            "Sliders",
            state.NameContext.Name);

        Assert.Equal(
            "04",
            state.NameContext.Season);

        Assert.Equal(
            "20",
            state.NameContext.Episode);

        Assert.Equal(
            "Der Abgrund",
            state.NameContext.EpisodeTitle);

        Assert.Equal(
            "2026",
            state.NameContext.Year);

        Assert.Equal(
            "08",
            state.NameContext.Month);

        Assert.Equal(
            "19",
            state.NameContext.Day);

        Assert.Equal(
            "Sliders_original.mp4",
            state.NameContext.OriginalName);

        Assert.Equal(
            "17",
            state.NameContext.Hour);

        Assert.Equal(
            "15",
            state.NameContext.Minute);

        Assert.Equal(
            "tele5",
            state.NameContext.Sender);

        Assert.Equal(
            "Sliders",
            state.NameContext.Series);
    }

    [Fact]
    public void ChangingAuthorAndUserComment_RaisesPropertyChanged()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        var changedProperties =
            new List<string?>();

        viewModel.PropertyChanged +=
            (_, eventArgs) =>
                changedProperties.Add(
                    eventArgs.PropertyName);

        viewModel.Author = "Neuer Autor";
        viewModel.UserComment =
            "Werbung vollständig entfernt.";

        Assert.Equal(
            "Neuer Autor",
            viewModel.Author);

        Assert.Equal(
            "Werbung vollständig entfernt.",
            viewModel.UserComment);

        Assert.Contains(
            nameof(viewModel.Author),
            changedProperties);

        Assert.Contains(
            nameof(viewModel.UserComment),
            changedProperties);
    }

    [Fact]
    public void ApplyQuickText_AppendsTextsToUserCommentOnSingleLine()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts =
            [
                "Keine Werbung gefunden.",
                "Werbung vollständig entfernt."
            ]
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        viewModel.ApplyQuickText(
            settings.QuickTexts[0]);

        Assert.Equal(
            "Mit Cut Assistant Next geschnitten. Keine Werbung gefunden.",
            viewModel.UserComment);

        viewModel.ApplyQuickText(
            settings.QuickTexts[1]);

        Assert.Equal(
            "Mit Cut Assistant Next geschnitten. Keine Werbung gefunden. Werbung vollständig entfernt.",
            viewModel.UserComment);

        Assert.DoesNotContain(
            "\r",
            viewModel.UserComment);

        Assert.DoesNotContain(
            "\n",
            viewModel.UserComment);
    }

    [Fact]
    public void Constructor_ProvidesTechnicalNoticesSeparatelyFromUserComment()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        TechnicalNotice[] technicalNotices =
        [
            new TechnicalNotice(
                "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.")
        ];

        var viewModel =
            CreateViewModel(
                settings,
                context,
                technicalNotices);

        var notice =
            Assert.Single(
                viewModel.TechnicalNotices);

        Assert.Equal(
            technicalNotices[0].Description,
            notice.Description);

        Assert.Equal(
            "Mit Cut Assistant Next geschnitten.",
            viewModel.UserComment);
    }

    [Fact]
    public void CreateDocument_BuildsCutlistDocumentFromCurrentValues()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        TechnicalNotice[] technicalNotices =
        [
            new TechnicalNotice(
                "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.")
        ];

        var viewModel =
            CreateViewModel(
                settings,
                context,
                technicalNotices);

        viewModel.UserComment =
            "Mit Cut Assistant Next geschnitten.";

        viewModel.SelectedRating = 3;
        viewModel.EpgError = true;

        viewModel.ActualContent =
            "Andere Sendung als angekündigt";

        viewModel.MissingBeginning = true;
        viewModel.MissingEnding = true;
        viewModel.MissingVideo = true;
        viewModel.MissingAudio = true;
        viewModel.OtherError = true;

        viewModel.OtherErrorDescription =
            "Bildstörung während der Aufnahme.";

        var cutPlan =
            new CutPlan(
                TimeSpan.FromSeconds(100));

        cutPlan.Add(
            new RemoveSegment(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20)));

        var analysis =
            new MediaAnalysisResult(
                FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
                FormatLongName: "QuickTime / MOV",
                FileSizeBytes: 734003200,
                Duration: TimeSpan.FromSeconds(100),
                VideoStreams: [],
                AudioStreams: []);

        var intendedCutApplication =
            new CutApplicationInfo(
                Name: "MP4Box",
                Executable: "MP4Box.exe",
                Version: "2.6.0",
                Options: "-split");

        var document =
            viewModel.CreateDocument(
                cutPlan,
                applyToFile: "Tatort.avi",
                applicationVersion: "1.0.0",
                analysis: analysis,
                intendedCutApplication: intendedCutApplication);

        Assert.Equal(
            "Tatort.avi",
            document.General.ApplyToFile);

        Assert.Equal(
            "1.0.0",
            document.General.Version);

        Assert.Equal(
            734003200,
            document.General.OriginalFileSizeBytes);

        Assert.Equal(
            "MP4Box",
            document.General.IntendedCutApplicationName);

        Assert.Equal(
            2,
            document.General.NoOfCuts);

        Assert.Equal(
            2,
            document.Cuts.Count);

        Assert.Equal(
            TimeSpan.Zero,
            document.Cuts[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            document.Cuts[0].Duration);

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            document.Cuts[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(80),
            document.Cuts[1].Duration);

        Assert.Equal(
            "Tatort",
            document.Info.SuggestedMovieName);

        Assert.Equal(
            "joerg",
            document.Info.Author);

        Assert.Equal(
            "Mit Cut Assistant Next geschnitten.",
            document.Info.UserComment);

        Assert.Equal(
            3,
            document.Info.RatingByAuthor);

        Assert.True(
            document.Info.OtherError);

        Assert.Equal(
            "Bildstörung während der Aufnahme. " +
            technicalNotices[0].Description,
            document.Info.OtherErrorDescription);

        Assert.True(
            document.Info.EpgError);

        Assert.Equal(
            "Andere Sendung als angekündigt",
            document.Info.ActualContent);

        Assert.True(
            document.Info.MissingBeginning);

        Assert.True(
            document.Info.MissingEnding);

        Assert.True(
            document.Info.MissingVideo);

        Assert.True(
            document.Info.MissingAudio);
    }

    [Fact]
    public void SelectedRating_DefaultsToNullAndCanBeSelectedExplicitly()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        Assert.Null(
            viewModel.SelectedRating);

        viewModel.SelectedRating = 5;

        Assert.Equal(
            5,
            viewModel.SelectedRating);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void SelectedRating_OutsideValidRange_ThrowsArgumentOutOfRangeException(
        int rating)
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => viewModel.SelectedRating = rating);
    }

    [Fact]
    public void CreateDocument_WithoutSelectedRating_ThrowsInvalidOperationException()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        var cutPlan =
            new CutPlan(
                TimeSpan.FromSeconds(100));

        var analysis =
            new MediaAnalysisResult(
                FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
                FormatLongName: "QuickTime / MOV",
                FileSizeBytes: 734003200,
                Duration: TimeSpan.FromSeconds(100),
                VideoStreams: [],
                AudioStreams: []);

        var exception =
            Assert.Throws<InvalidOperationException>(
                () => viewModel.CreateDocument(
                    cutPlan,
                    applyToFile: "Tatort.avi",
                    applicationVersion: "1.0.0",
                    analysis: analysis));

        Assert.Equal(
            "Vor dem Erzeugen der Cutlist muss eine Bewertung ausgewählt werden.",
            exception.Message);
    }

    [Fact]
    public void EpgError_DefaultsToFalseAndActualContentCanBeEdited()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        Assert.False(
            viewModel.EpgError);

        Assert.Null(
            viewModel.ActualContent);

        viewModel.EpgError = true;
        viewModel.ActualContent =
            "Tatsächlicher Inhalt der Aufnahme";

        Assert.True(
            viewModel.EpgError);

        Assert.Equal(
            "Tatsächlicher Inhalt der Aufnahme",
            viewModel.ActualContent);
    }

    [Fact]
    public void MissingBeginningAndEnding_DefaultToFalseAndCanBeSelected()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        Assert.False(
            viewModel.MissingBeginning);

        Assert.False(
            viewModel.MissingEnding);

        viewModel.MissingBeginning = true;
        viewModel.MissingEnding = true;

        Assert.True(
            viewModel.MissingBeginning);

        Assert.True(
            viewModel.MissingEnding);
    }

    [Fact]
    public void MissingVideoAndAudio_DefaultToFalseAndCanBeSelected()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        Assert.False(
            viewModel.MissingVideo);

        Assert.False(
            viewModel.MissingAudio);

        viewModel.MissingVideo = true;
        viewModel.MissingAudio = true;

        Assert.True(
            viewModel.MissingVideo);

        Assert.True(
            viewModel.MissingAudio);
    }

    [Fact]
    public void OtherError_DefaultsToFalseAndDescriptionCanBeEdited()
    {
        var settings = new CutlistSettings
        {

            DefaultAuthor = "joerg",
            QuickTexts = []
        };

        var context = new NameTemplateContext(
            Name: "Tatort");

        var viewModel =
            CreateViewModel(
                settings,
                context);

        Assert.False(
            viewModel.OtherError);

        Assert.Null(
            viewModel.OtherErrorDescription);

        viewModel.OtherError = true;
        viewModel.OtherErrorDescription =
            "Bildstörung während der Aufnahme";

        Assert.True(
            viewModel.OtherError);

        Assert.Equal(
            "Bildstörung während der Aufnahme",
            viewModel.OtherErrorDescription);
    }

    private static CutlistGenerationViewModel CreateViewModel(
        CutlistSettings settings,
        NameTemplateContext context,
        IReadOnlyCollection<TechnicalNotice>? technicalNotices = null,
        string nameTemplate = "%Name%")
    {
        var namingSettings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    nameTemplate
            };

        return new CutlistGenerationViewModel(
            settings,
            namingSettings,
            context,
            technicalNotices);
    }

}
