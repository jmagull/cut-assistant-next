using CutAssistantNext.App.State;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.State;

public sealed class CutNamingStateTests
{
    [Fact]
    public void UseSuggestedMovieName_PreservesContextAndUsesExplicitSuggestion()
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
                "%Name% S%Staffel%E%Folge% %Folgentitel%",
                context);

        var updatedState =
            state.UseSuggestedMovieName(
                "Sliders S04E20 - Der Abgrund [19.08.2026]");

        Assert.Equal(
            "Sliders S04E20 - Der Abgrund [19.08.2026]",
            updatedState.SuggestedMovieName);

        Assert.Same(
            context,
            updatedState.NameContext);

        Assert.Equal(
            state.NameTemplate,
            updatedState.NameTemplate);
    }

    [Fact]
    public void UseNaming_ReplacesCompleteNamingDataAndRendersSuggestion()
    {
        var originalContext =
            new NameTemplateContext(
                Name: "Original",
                OriginalName: "original.mp4");

        var state =
            new CutNamingState(
                "%Name%",
                originalContext)
            .UseSuggestedMovieName(
                "Externer Vorschlag");

        var updatedContext =
            new NameTemplateContext(
                Name: "Sliders",
                Year: "2026",
                Month: "08",
                Day: "19",
                Season: "04",
                Episode: "20",
                OriginalName: "Sliders_original.mp4",
                Sender: "tele5",
                Series: "Sliders",
                EpisodeTitle: "Der Abgrund");

        var updatedState =
            state.UseNaming(
                "%Name% - %Folgentitel%",
                updatedContext);

        Assert.Equal(
            "%Name% - %Folgentitel%",
            updatedState.NameTemplate);

        Assert.Same(
            updatedContext,
            updatedState.NameContext);

        Assert.Equal(
            "Sliders - Der Abgrund",
            updatedState.SuggestedMovieName);
    }
}
