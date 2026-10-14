using CutAssistantNext.App.Naming;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.Naming;

public sealed class IndependentNamingBlocksTests
{
    private const string NewDefault = "%Name% %S%%Staffel%%E%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]";

    [Fact]
    public void ReferenceFileDateAndSupplementalValuesMatchRequestedGhostbustersExample()
    {
        var parsed = OtrFileNameParser.Parse(NamingEditorSample.OriginalFileName);
        Assert.Equal("2026", parsed.Year);
        Assert.Equal("09", parsed.Month);
        Assert.Equal("19", parsed.Day);
        Assert.Null(parsed.Season);
        Assert.Null(parsed.Episode);

        var context = NamingEditorSample.CreateContext();
        Assert.Equal("Ghostbusters", context.Name);
        Assert.Equal("01", context.Season);
        Assert.Equal("01", context.Episode);
        Assert.Equal("Hexenprobleme", context.EpisodeTitle);
        Assert.Equal(NamingEditorSample.OriginalFileName, context.OriginalName);
        Assert.Equal(NewDefault, NamingSettings.CreateDefault().DefaultNameTemplate);
        var model = new NamingSettingsViewModel(NamingSettings.CreateDefault(), context);
        Assert.Equal("Ghostbusters S01E01 Hexenprobleme [19.09.2026]", model.Preview);
        Assert.Equal("Ghostbusters S01E01 Hexenprobleme [19.09.2026].mp4",
            OutputFileNameBuilder.BuildExactMovieFileName(model.Preview));
    }

    [Fact]
    public void DefaultFilmMaskWithoutSeriesDataHasNoSE()
    {
        var context = new NameTemplateContext(Name: "Die Teufelsbrigade", Year: "2021", Month: "01", Day: "21");
        Assert.Equal("Die Teufelsbrigade", NameTemplateRenderer.Render(NewDefault, context));
    }

    [Fact]
    public void EachNewElementHasAnIndependentFriendlyBlockAndRoundTrips()
    {
        const string mask = "%S%%Staffel%%E%%Folge%%x%";
        var blocks = NameTemplateBlockParser.ParseForEditor(mask);
        Assert.Equal(["S", "Staffel", "E", "Folge", "x"], blocks.Select(block => block.DisplayText));
        Assert.All(blocks, block => Assert.Equal(NameTemplateBlockKind.Token, block.Kind));
        Assert.Equal(mask, NameTemplateBlockParser.Render(blocks));
    }

    [Theory]
    [InlineData("%Name% %Staffel:S%%Folge:E%")]
    [InlineData("%Name% %Staffel:S%%Folge:E%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]")]
    [InlineData("%Staffel:S%-%Folge:E%-%Staffel:S%")]
    [InlineData("%Staffel:Staffel_%%Folge:Episode_%%Name% frei gesetzt")]
    public void OpeningLegacyMaskPreservesExactSyntaxAndRendering(string mask)
    {
        var blocks = NameTemplateBlockParser.ParseForEditor(mask);
        Assert.Equal(mask, NameTemplateBlockParser.Render(blocks));
        foreach (var context in new[] { NamingEditorSample.CreateContext(),
                     new NameTemplateContext(Name: "Film", Season: null, Episode: null),
                     new NameTemplateContext(Name: "Film", Season: null, Episode: "01"),
                     new NameTemplateContext(Name: "Film", Season: " ", Episode: "\t") })
        {
            Assert.Equal(NameTemplateRenderer.Render(mask, context),
                NameTemplateRenderer.Render(NameTemplateBlockParser.Render(blocks), context));
        }
    }

    [Fact]
    public void KnownLegacyPairsAreDisplayedAsSeparateElementsWithoutChangingStoredMask()
    {
        const string old = "%Name% %Staffel:S%%Folge:E%";
        var settings = new NamingSettings { DefaultNameTemplate = old };
        var model = new NamingSettingsViewModel(settings, NamingEditorSample.CreateContext());
        Assert.Equal(["Name", "Leerzeichen", "S", "Staffel", "E", "Folge"],
            model.TemplateBlocks.Select(block => block.DisplayText));
        Assert.Equal(old, model.DefaultNameTemplate);
        Assert.Equal(old, model.CreateSettings().DefaultNameTemplate);
        Assert.Equal(old, settings.DefaultNameTemplate);
    }

    [Fact]
    public void RemovingSeasonValueDoesNotSilentlyRemoveIndependentS()
    {
        var model = new NamingSettingsViewModel(new NamingSettings { DefaultNameTemplate = "%S%%Staffel%%E%%Folge%" },
            NamingEditorSample.CreateContext());
        Assert.True(model.RemoveTemplateBlockAt(1));
        Assert.Equal("%S%%E%%Folge%", model.DefaultNameTemplate);
        Assert.Equal(["S", "E", "Folge"], model.TemplateBlocks.Select(block => block.DisplayText));
    }

    [Fact]
    public void NewBlocksInsertMoveAndDeleteIndependently()
    {
        var model = new NamingSettingsViewModel(new NamingSettings { DefaultNameTemplate = "%Name% " },
            NamingEditorSample.CreateContext());
        foreach (var element in new[] { "%Staffel%", "%x%", "%Folge%" })
        {
            Assert.True(model.InsertTemplateBlock(element, model.TemplateBlocks.Count) >= 0);
        }

        Assert.Equal("Ghostbusters 01x01", model.Preview);
        Assert.Equal(4, model.MoveTemplateBlock(2, 5));
        Assert.Equal("%Name% %x%%Folge%%Staffel%", model.DefaultNameTemplate);
        Assert.True(model.RemoveTemplateBlockAt(2));
        Assert.Equal("%Name% %Folge%%Staffel%", model.DefaultNameTemplate);
    }

    [Fact]
    public void StandardResetAndDiscardKeepTheirSeparateMeanings()
    {
        const string custom = "%Name% CUSTOM [%YY%]";
        var model = new NamingSettingsViewModel(new NamingSettings { DefaultNameTemplate = custom },
            NamingEditorSample.CreateContext());
        model.RestoreCanDefaultNameTemplate();
        Assert.Equal(NewDefault, model.DefaultNameTemplate);
        Assert.Equal("Ghostbusters S01E01 Hexenprobleme [19.09.2026]", model.Preview);
        model.ResetDefaultNameTemplate();
        Assert.Equal(custom, model.DefaultNameTemplate);
    }

    [Theory]
    [InlineData("%Name% %Staffel:S%%Folge:E%%Folgentitel: - %")]
    [InlineData("%Name% %Staffel:Staffel_%%Folge:Episode_%")]
    [InlineData(NewDefault)]
    [InlineData("%Name% %Staffel%%x%%Folge%")]
    public void IsolatedSaveReloadPreservesCustomOldAndNewMasksWithoutAutomaticReplacement(string mask)
    {
        var root = Path.Combine(Path.GetTempPath(), $"can-mask-roundtrip-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var path = Path.Combine(root, "naming.json");
            var store = new NamingSettingsStore(path);
            store.Save(new NamingSettings { DefaultNameTemplate = mask });
            var originalBytes = File.ReadAllBytes(path);
            var loaded = new NamingSettingsLoader(store, Path.Combine(root, "absent-legacy.json")).Load();
            var model = new NamingSettingsViewModel(loaded, NamingEditorSample.CreateContext());
            Assert.Equal(mask, model.DefaultNameTemplate);
            Assert.Equal(originalBytes, File.ReadAllBytes(path));
            store.Save(model.CreateSettings());
            Assert.Equal(mask, store.Load()!.DefaultNameTemplate);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void EditingAnotherBlockDoesNotNormalizeUnchangedLegacyPairs()
    {
        const string original = "%Name% %Staffel:S%%Folge:E%";
        var model = new NamingSettingsViewModel(new NamingSettings { DefaultNameTemplate = original },
            new NameTemplateContext(Name: "Film", Season: " ", Episode: null, Sender: "Sender"));
        model.InsertTemplateBlock("%Sender%", model.TemplateBlocks.Count);
        Assert.Equal(original + "%Sender%", model.DefaultNameTemplate);
        Assert.Equal(NameTemplateRenderer.Render(original + "%Sender%", new NameTemplateContext(
            Name: "Film", Season: " ", Episode: null, Sender: "Sender")), model.Preview);
    }
}
