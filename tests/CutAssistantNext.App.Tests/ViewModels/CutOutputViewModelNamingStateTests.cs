using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutOutputViewModelNamingStateTests
{
    [Fact]
    public void Constructor_WithNamingState_UsesExplicitSuggestedMovieName()
    {
        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Season: "04",
                Episode: "20",
                OriginalName: "Sliders_original.mp4",
                EpisodeTitle: "Der Abgrund");

        var state =
            new CutNamingState(
                "%Name% S%Staffel%E%Folge% %Folgentitel%",
                context)
            .UseSuggestedMovieName(
                "Sliders S04E20 - Der Abgrund [19.08.2026]");

        var viewModel =
            new CutOutputViewModel(
                state);

        Assert.Equal(
            "Sliders S04E20 - Der Abgrund [19.08.2026]",
            viewModel.SuggestedMovieName);

        Assert.Equal(
            "%Name% S%Staffel%E%Folge% %Folgentitel%",
            viewModel.NameTemplate);

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
    [Fact]
    public void ChangingNamingValues_PreservesExplicitSuggestedMovieName()
    {
        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Season: "04",
                Episode: "20",
                OriginalName: "Sliders_original.mp4",
                EpisodeTitle: "Der Abgrund");

        var state =
            new CutNamingState(
                "%Name% S%Staffel%E%Folge% %Folgentitel%",
                context)
            .UseSuggestedMovieName(
                "Sliders S04E20 - Der Abgrund [19.08.2026]");

        var viewModel =
            new CutOutputViewModel(
                state);

        viewModel.Name =
            "Sliders Neu";

        viewModel.Season =
            "05";

        viewModel.Episode =
            "01";

        viewModel.EpisodeTitle =
            "Neuer Titel";

        Assert.Equal(
            "Sliders S04E20 - Der Abgrund [19.08.2026]",
            viewModel.SuggestedMovieName);
    }

    [Fact]
    public void GenerateSuggestedMovieName_UsesDefaultTemplateInsteadOfNamingStateTemplate()
    {
        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Year: "2026",
                Month: "08",
                Day: "19",
                Season: "04",
                Episode: "20",
                OriginalName: "Sliders_original.mp4",
                EpisodeTitle: "Der Abgrund");

        var state =
            new CutNamingState(
                "%Name% LOCAL",
                context)
            .UseSuggestedMovieName(
                "Fremder Vorschlag");

        var viewModel =
            new CutOutputViewModel(
                state,
                "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]");

        Assert.Equal(
            "Fremder Vorschlag",
            viewModel.SuggestedMovieName);

        viewModel.GenerateSuggestedMovieName();

        Assert.Equal(
            "Sliders S04E20 - Der Abgrund [19.08.2026]",
            viewModel.SuggestedMovieName);
    }
}
