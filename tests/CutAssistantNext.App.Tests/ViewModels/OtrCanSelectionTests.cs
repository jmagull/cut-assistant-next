using System.Text.Json;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class OtrCanSelectionTests
{
    [Fact]
    public async Task SavingToolPathsDoesNotRequireNativeTools()
    {
        OtrCanSettings? saved = null;
        using var vm = new OtrCanSettingsViewModel(new(), (_, _) => throw new InvalidOperationException(),
            settings => { saved = settings; return Task.CompletedTask; });
        Assert.True(await vm.SaveAsync());
        Assert.Equal(new OtrCanSettings(), saved);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SavingNeverActivatesOrProbesATool(bool available)
    {
        OtrCanSettings? saved = null;
        var checks = 0;
        using var vm = new OtrCanSettingsViewModel(new(),
            (_, _) =>
            {
                checks++; return Task.FromResult<IReadOnlyList<OtrCanToolCheckResult>>(
                Enumerable.Range(0, 4).Select(i => new OtrCanToolCheckResult("tool" + i, available, "result")).ToArray());
            },
            settings => { saved = settings; return Task.CompletedTask; });
        Assert.True(await vm.SaveAsync());
        Assert.Equal(0, checks);
        Assert.NotNull(saved);
        Assert.DoesNotContain("UseOtrCan", JsonSerializer.Serialize(saved));
    }

    [Fact]
    public void ToolPathsSurviveAnIsolatedSettingsRoundtrip()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-native-settings-" + Guid.NewGuid().ToString("N"));
        try
        {
            var store = new OtrCanSettingsStore(Path.Combine(root, "settings.json"));
            var settings = new OtrCanSettings
            {
                ExecutablePath = Path.Combine(root, "engine.exe"),
                FfmsIndexExecutablePath = Path.Combine(root, "index.exe")
            };
            store.Save(settings);
            Assert.Equal(settings, store.Load());
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void LegacyMotorFlagIsIgnoredWithoutLosingPathsOrRewritingTheFile()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-native-legacy-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var path = Path.Combine(root, "settings.json");
            var engine = Path.Combine(root, "engine.exe");
            var index = Path.Combine(root, "index.exe");
            var json = JsonSerializer.Serialize(new { UseOtrCan = true, ExecutablePath = engine, FfmsIndexExecutablePath = index });
            File.WriteAllText(path, json);
            var settings = new OtrCanSettingsStore(path).Load();
            using var vm = new OtrCanSettingsViewModel(settings, (_, _) => throw new InvalidOperationException(), _ => Task.CompletedTask);
            Assert.Equal(engine, vm.ExecutablePath);
            Assert.Equal(index, vm.FfmsIndexExecutablePath);
            Assert.DoesNotContain("UseOtrCan", JsonSerializer.Serialize(vm.CreateSettings()));
            Assert.Equal(json, File.ReadAllText(path));
        }
        finally { Directory.Delete(root, true); }
    }
}
