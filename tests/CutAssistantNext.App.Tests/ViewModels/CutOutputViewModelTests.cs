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
    }}
