using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutOutputViewModelManualNameTests
{
    private const string Template = "%Name% %Staffel:S%%Folge:E%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]";
    private static NameTemplateContext Context() => new(
        Name: "Ghosts", Season: "04", Episode: "01", EpisodeTitle: "Patience",
        Year: "2026", Month: "09", Day: "19", OriginalName: "Ghosts_original.HQ.mp4");

    [Fact]
    public void StartsWithExistingSuggestionIncludingLoadedCutlistName()
    {
        var state = new CutNamingState(Template, Context()).UseSuggestedMovieName("Fremder Cutlist-Vorschlag");
        var model = new CutOutputViewModel(state, Template);
        Assert.Equal("Fremder Cutlist-Vorschlag", model.OutputMovieName);
    }

    [Theory]
    [InlineData("Ghosts - 4x01 - Patience")]
    [InlineData("Ghostbusters")]
    [InlineData("Ghosts [22.09.2026]")]
    public void ManualNameIsPreservedAcrossUpperFieldChangesAndNotSavedToCutlistState(string manualName)
    {
        var context = Context();
        var state = new CutNamingState(Template, context);
        var model = new CutOutputViewModel(state, Template);
        string originalSuggestion = model.SuggestedMovieName;
        model.OutputMovieName = manualName;
        model.Name = "Anderer Titel";
        model.Season = "5";
        model.Episode = "02";
        model.EpisodeTitle = "Andere Folge";
        model.NameTemplate = "%Name% LOCAL";

        Assert.Equal(manualName, model.OutputMovieName);
        Assert.Equal(originalSuggestion, model.SuggestedMovieName);
        Assert.Equal(originalSuggestion, model.CreateNamingState().SuggestedMovieName);
        Assert.Equal(Template, state.NameTemplate);
        Assert.Same(context, state.NameContext);
        Assert.Equal("19", model.CreateNamingState().NameContext.Day);
        Assert.Equal("Ghosts_original.HQ.mp4", model.CreateNamingState().NameContext.OriginalName);
    }

    [Fact]
    public void RegenerateReplacesManualNameEvenWhenAutomaticSuggestionHasNotChanged()
    {
        var model = new CutOutputViewModel(Template, Context()) { OutputMovieName = "Freier Filmname" };
        var changed = new List<string?>();
        model.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        model.GenerateSuggestedMovieName();

        Assert.Equal("Ghosts S04E01 Patience [19.09.2026]", model.OutputMovieName);
        Assert.Contains(nameof(model.OutputMovieName), changed);
    }

    [Fact]
    public void RegenerateUsesCurrentUpperFieldsAndConfiguredDefaultTemplate()
    {
        var model = new CutOutputViewModel(
            new CutNamingState("%Name% LOCAL", Context()), Template)
        {
            OutputMovieName = "Manuell",
            Name = "Neue Serie",
            Season = "05",
            Episode = "02",
            EpisodeTitle = "Neue Folge"
        };
        model.GenerateSuggestedMovieName();
        Assert.Equal("Neue Serie S05E02 Neue Folge [19.09.2026]", model.OutputMovieName);
        Assert.Equal(model.SuggestedMovieName, model.OutputMovieName);
    }

    [Fact]
    public void CanTemporarilyClearInputWithoutLosingSuggestionOrThrowingDuringTyping()
    {
        var model = new CutOutputViewModel(Template, Context());
        model.OutputMovieName = string.Empty;
        Assert.Empty(model.OutputMovieName);
        Assert.Equal("Ghosts S04E01 Patience [19.09.2026]", model.CreateNamingState().SuggestedMovieName);
    }
}
