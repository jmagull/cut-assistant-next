using System.Text.Json;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class OtrCanQuickTextTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-quicktext-" + Guid.NewGuid().ToString("N"));
    public OtrCanQuickTextTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void OldFiveCustomizedTextsAndAuthorRemainUnchangedWhenSixthSuggestionIsAdded()
    {
        var path = Path.Combine(_root, "settings.json");
        var texts = new[] { "Eigen 1", "Eigen 2", "MP4Box mit eigener Versionsangabe", "Teil 1", "Teil 2" };
        var json = JsonSerializer.Serialize(new { DefaultAuthor = "Testautor", QuickTexts = texts });
        File.WriteAllText(path, json);
        var loaded = new CutlistSettingsStore(path).Load();
        Assert.NotNull(loaded);
        Assert.Equal(texts, loaded.QuickTexts);
        Assert.Equal("Testautor", loaded.DefaultAuthor);
        var vm = new CutlistSettingsViewModel(loaded);
        Assert.Equal(texts, new[] { vm.QuickText1, vm.QuickText2, vm.QuickText3, vm.QuickText4, vm.QuickText5 });
        Assert.Equal(CutlistSettings.OtrCanQuickText, vm.QuickText6);
        Assert.Equal(json, File.ReadAllText(path));
    }

    [Theory]
    [InlineData("Mit eigenem otr-can-Kommentar geschnitten.")]
    [InlineData("")]
    public void CustomizedOrClearedSixthTextSurvivesSavingAndReloading(string text)
    {
        var store = new CutlistSettingsStore(Path.Combine(_root, "settings.json"));
        var original = CutlistSettings.CreateDefault();
        var vm = new CutlistSettingsViewModel(original) { QuickText6 = text };
        store.Save(vm.CreateSettings());
        var loaded = store.Load();
        Assert.NotNull(loaded);
        Assert.Equal(original.QuickTexts, loaded.QuickTexts);
        Assert.Equal(text, new CutlistSettingsViewModel(loaded).QuickText6);
        Assert.Equal(string.IsNullOrEmpty(text) ? 5 : 6, loaded.GetQuickTexts().Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SixthTextIsOfferedAndOnlyAppendedToTheCommentOnUserSelection(bool useNamingState)
    {
        var settings = CutlistSettings.CreateDefault();
        var context = new NameTemplateContext(Name: "Testfilm");
        var naming = NamingSettings.CreateDefault();
        var vm = useNamingState
            ? new CutlistGenerationViewModel(settings, new CutNamingState(naming.DefaultNameTemplate, context))
            : new CutlistGenerationViewModel(settings, naming, context);
        Assert.Equal(6, vm.QuickTexts.Count);
        Assert.Equal(CutlistSettings.OtrCanQuickText, vm.QuickTexts[5]);
        Assert.DoesNotContain("otr-can", vm.UserComment);
        vm.ApplyQuickText(vm.QuickTexts[5]);
        Assert.Equal("Mit Cut Assistant Next geschnitten. Mit otr-can framegenau geschnitten.", vm.UserComment);
    }
}
