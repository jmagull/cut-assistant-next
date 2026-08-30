using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class NamingSettingsViewModelTemplateElementTests
{
    [Fact]
    public void InsertTemplateElement_InsertsAtCursorAndUpdatesPreview()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% "
            };

        var context =
            new NameTemplateContext(
                Name: "Sliders",
                ShortYear: "26",
                OriginalName:
                    "Sliders_original.mp4");

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                context);

        var caretPosition =
            viewModel.InsertTemplateElement(
                "%YY%",
                selectionStart: 7,
                selectionLength: 0);

        Assert.Equal(
            "%Name% %YY%",
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            "Sliders 26",
            viewModel.Preview);

        Assert.Equal(
            11,
            caretPosition);
    }
    [Fact]
    public void InsertTemplateElement_ReplacesSelectedText()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% TEST"
            };

        var context =
            new NameTemplateContext(
                Name: "Sliders",
                ShortYear: "26",
                OriginalName:
                    "Sliders_original.mp4");

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                context);

        var caretPosition =
            viewModel.InsertTemplateElement(
                "%YY%",
                selectionStart: 7,
                selectionLength: 4);

        Assert.Equal(
            "%Name% %YY%",
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            "Sliders 26",
            viewModel.Preview);

        Assert.Equal(
            11,
            caretPosition);
    }
}
