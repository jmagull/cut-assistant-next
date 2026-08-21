using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutOutputViewModelTests
{
    [Fact]
    public void Constructor_UsesNamingContextAndTemplate()
    {
        var nameContext =
            NameTemplateContextFactory.Create(
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi");

        var viewModel =
            new CutOutputViewModel(
                "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
                nameContext);

        Assert.Equal(
            "Sliders Perfekte Piloten [13.08.2026]",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void ChangingNamingValues_UpdatesSuggestedMovieName()
    {
        var nameContext =
            NameTemplateContextFactory.Create(
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi");

        var viewModel =
            new CutOutputViewModel(
                "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
                nameContext);

        viewModel.Season = "1";
        viewModel.Episode = "2";
        viewModel.EpisodeTitle = "Der Test";

        Assert.Equal(
            "Sliders Perfekte Piloten S1E2 - Der Test [13.08.2026]",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void ChangingNameTemplate_UpdatesSuggestedMovieName()
    {
        var nameContext =
            NameTemplateContextFactory.Create(
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi");

        var viewModel =
            new CutOutputViewModel(
                "%Name%",
                nameContext);

        viewModel.NameTemplate =
            "%Name% [%Tag%.%Monat%.%YYYY%]";

        Assert.Equal(
            "Sliders Perfekte Piloten [13.08.2026]",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void ChangingEpisodeTitle_RaisesPropertyChangedForValueAndSuggestion()
    {
        var nameContext =
            NameTemplateContextFactory.Create(
                "Sliders_Perfekte_Piloten_26.08.13_17-10_tele5_60_TVOON_DE.HQ.avi");

        var viewModel =
            new CutOutputViewModel(
                "%Name%%Folgentitel: - %",
                nameContext);

        var changedProperties =
            new List<string?>();

        viewModel.PropertyChanged +=
            (_, e) =>
                changedProperties.Add(
                    e.PropertyName);

        viewModel.EpisodeTitle =
            "Der Test";

        Assert.Contains(
            nameof(viewModel.EpisodeTitle),
            changedProperties);

        Assert.Contains(
            nameof(viewModel.SuggestedMovieName),
            changedProperties);
    }

    [Fact]
    public void CreateNamingState_UsesCompleteCurrentNamingData()
    {
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
            new CutOutputViewModel(
                "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]",
                context);

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
    public void CreateNamingState_PreservesExplicitSuggestedMovieName()
    {
        var context =
            new NameTemplateContext(
                Name: "Sliders",
                OriginalName: "Sliders_original.mp4");

        var namingState =
            new CutNamingState(
                "%Name%",
                context)
            .UseSuggestedMovieName(
                "Sliders S04E20 - Der Abgrund [19.08.2026]");

        var viewModel =
            new CutOutputViewModel(
                namingState);

        var state =
            viewModel.CreateNamingState();

        Assert.Equal(
            "Sliders S04E20 - Der Abgrund [19.08.2026]",
            state.SuggestedMovieName);

        Assert.Same(
            context,
            state.NameContext);
    }
}