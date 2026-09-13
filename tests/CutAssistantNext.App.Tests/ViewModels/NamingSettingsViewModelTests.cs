using CutAssistantNext.App.Naming;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class NamingSettingsViewModelTests
{
    [Fact]
    public void Constructor_UsesSettingsAndContextForPreview()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]"
            };

        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Year: "2026",
                Month: "08",
                Day: "28",
                Season: "05",
                Episode: "05",
                OriginalName:
                    "Sliders__Das_Wasser_des_Lebens_26.08.28_17-10_tele5_60_TVOON_DE.HQ.mp4",
                EpisodeTitle:
                    "Das Wasser des Lebens");

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                context);

        Assert.Equal(
            settings.DefaultNameTemplate,
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            context.OriginalName,
            viewModel.OriginalFileName);

        Assert.Equal(
            "Sliders S05E05 - Das Wasser des Lebens [28.08.2026]",
            viewModel.Preview);
    }
    [Fact]
    public void ChangingDefaultNameTemplate_UpdatesPreview()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name%"
            };

        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Year: "2026",
                Month: "08",
                Day: "28",
                Season: "05",
                Episode: "05",
                OriginalName: "Sliders_original.mp4",
                EpisodeTitle: "Das Wasser des Lebens");

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                context);

        viewModel.DefaultNameTemplate =
            "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]";

        Assert.Equal(
            "Sliders S05E05 - Das Wasser des Lebens [28.08.2026]",
            viewModel.Preview);
    }
    [Fact]
    public void ChangingDefaultNameTemplate_RaisesPropertyChangedForTemplateAndPreview()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name%"
            };

        var context =
            new NameTemplateContext(
                Name: "Sliders",
                Year: "2026",
                Month: "08",
                Day: "28",
                OriginalName: "Sliders_original.mp4");

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                context);

        var changedProperties =
            new List<string?>();

        viewModel.PropertyChanged +=
            (_, e) =>
            {
                changedProperties.Add(
                    e.PropertyName);
            };

        viewModel.DefaultNameTemplate =
            "%Name% [%Tag%.%Monat%.%YYYY%]";

        Assert.Contains(
            nameof(viewModel.DefaultNameTemplate),
            changedProperties);

        Assert.Contains(
            nameof(viewModel.Preview),
            changedProperties);
    }
    [Fact]
    public void CreateSettings_UsesCurrentDefaultNameTemplate()
    {
        var viewModel =
            new NamingSettingsViewModel(
                NamingSettings.CreateDefault(),
                new NameTemplateContext(
                    Name: "Sliders",
                    OriginalName: "Sliders_original.mp4"));

        viewModel.DefaultNameTemplate =
            "%Name% [%Tag%.%Monat%.%YYYY%]";

        var settings =
            viewModel.CreateSettings();

        Assert.Equal(
            viewModel.DefaultNameTemplate,
            settings.DefaultNameTemplate);
    }
    [Fact]
    public void TemplateBlocks_ReflectCurrentDefaultNameTemplate()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% [%Tag%.%Monat%.%YY%]"
            };

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                new NameTemplateContext(
                    Name: "Sliders",
                    OriginalName: "Sliders_original.mp4"));

        Assert.Equal(
            [
                "%Name%",
                " ",
                "[",
                "%Tag%",
                ".",
                "%Monat%",
                ".",
                "%YY%",
                "]"
            ],
            viewModel.TemplateBlocks.Select(
                block => block.Value));
    }
    [Fact]
    public void RemoveTemplateBlockAt_RemovesWholeBlockAndUpdatesPreview()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% %Staffel:S%%Folge:E%"
            };

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                new NameTemplateContext(
                    Name: "Sliders",
                    Season: "05",
                    Episode: "05",
                    OriginalName: "Sliders_original.mp4"));

        var removed =
            viewModel.RemoveTemplateBlockAt(
                2);

        Assert.True(
            removed);

        Assert.Equal(
            "%Name% %Folge:E%",
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            [
                "%Name%",
                " ",
                "%Folge:E%"
            ],
            viewModel.TemplateBlocks.Select(
                block => block.Value));

        Assert.Equal(
            "Sliders E05",
            viewModel.Preview);
    }
    [Fact]
    public void MoveTemplateBlock_MovesWholeBlockToEnd()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% %Staffel:S%%Folge:E%"
            };

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                new NameTemplateContext(
                    Name: "Sliders",
                    Season: "05",
                    Episode: "05",
                    OriginalName: "Sliders_original.mp4"));

        var newIndex =
            viewModel.MoveTemplateBlock(
                sourceIndex: 1,
                insertionIndex: 4);

        Assert.Equal(
            3,
            newIndex);

        Assert.Equal(
            "%Name%%Staffel:S%%Folge:E% ",
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            [
                "%Name%",
                "%Staffel:S%",
                "%Folge:E%",
                " "
            ],
            viewModel.TemplateBlocks.Select(
                block => block.Value));

        Assert.Equal(
            "SlidersS05E05",
            viewModel.Preview);
    }
    [Fact]
    public void InsertTemplateBlock_InsertsWholeBlockAtGap()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name%%Folge:E%"
            };

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                new NameTemplateContext(
                    Name: "Sliders",
                    Episode: "05",
                    Sender: "tele5",
                    OriginalName: "Sliders_original.mp4"));

        var insertedIndex =
            viewModel.InsertTemplateBlock(
                "%Sender%",
                insertionIndex: 1);

        Assert.Equal(
            1,
            insertedIndex);

        Assert.Equal(
            "%Name%%Sender%%Folge:E%",
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            [
                "%Name%",
                "%Sender%",
                "%Folge:E%"
            ],
            viewModel.TemplateBlocks.Select(
                block => block.Value));

        Assert.Equal(
            "Sliderstele5E05",
            viewModel.Preview);
    }
    [Fact]
    public void InsertTemplateBlock_AllowsSpaceLiteral()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name%%Folge:E%"
            };

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                new NameTemplateContext(
                    Name: "Sliders",
                    Episode: "05",
                    OriginalName: "Sliders_original.mp4"));

        var insertedIndex =
            viewModel.InsertTemplateBlock(
                " ",
                insertionIndex: 1);

        Assert.Equal(
            1,
            insertedIndex);

        Assert.Equal(
            "%Name% %Folge:E%",
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            [
                "%Name%",
                " ",
                "%Folge:E%"
            ],
            viewModel.TemplateBlocks.Select(
                block => block.Value));

        Assert.Equal(
            "Sliders E05",
            viewModel.Preview);
    }
    [Fact]
    public void ResetDefaultNameTemplate_RestoresTemplateFromDialogStart()
    {
        var settings =
            new NamingSettings
            {
                DefaultNameTemplate =
                    "%Name% [%Tag%.%Monat%.%YYYY%]"
            };

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                new NameTemplateContext(
                    Name: "Sliders",
                    Year: "2026",
                    Month: "08",
                    Day: "28",
                    OriginalName: "Sliders_original.mp4"));

        viewModel.DefaultNameTemplate =
            "%Sender%%Name%---%Stunde%";

        viewModel.ResetDefaultNameTemplate();

        Assert.Equal(
            settings.DefaultNameTemplate,
            viewModel.DefaultNameTemplate);

        Assert.Equal(
            [
                "%Name%",
                " ",
                "[",
                "%Tag%",
                ".",
                "%Monat%",
                ".",
                "%YYYY%",
                "]"
            ],
            viewModel.TemplateBlocks.Select(
                block => block.Value));

        Assert.Equal(
            "Sliders [28.08.2026]",
            viewModel.Preview);
    }
}
