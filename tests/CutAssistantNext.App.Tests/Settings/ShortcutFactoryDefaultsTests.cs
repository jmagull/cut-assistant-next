using System.IO;
using System.Text.Json;
using System.Windows.Input;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class ShortcutFactoryDefaultsTests : IDisposable
{
    private readonly DirectoryInfo _root = Directory.CreateTempSubdirectory("can-shortcut-factory-");
    private string SettingsPath => Path.Combine(_root.FullName, "shortcuts.json");
    public void Dispose() => _root.Delete(true);

    [Theory]
    [InlineData("CutStart", Key.A, ModifierKeys.None)]
    [InlineData("CutEnd", Key.E, ModifierKeys.None)]
    [InlineData("VideoStart", Key.A, ModifierKeys.Shift)]
    [InlineData("VideoEnd", Key.E, ModifierKeys.Shift)]
    public void FreshProfileAndSingleResetUseFactoryBindings(string name, Key key, ModifierKeys modifiers)
    {
        var action = Enum.Parse<PlaybackShortcut>(name);
        var shortcuts = PlaybackShortcuts.Load(SettingsPath);
        Assert.Equal(new(key, modifiers), shortcuts[action]);
        Assert.Equal(action, shortcuts.Match(key, modifiers));
        Assert.Equal(shortcuts[action], PlaybackShortcuts.Definitions.Single(d => d.Action == action).Standard);
        Assert.Null(shortcuts.Assign(action, new(Key.F6)));
        Assert.Null(shortcuts.Reset(action));
        Assert.Equal(new(key, modifiers), shortcuts[action]);
        shortcuts.Save(SettingsPath);
        Assert.Equal(new(key, modifiers), PlaybackShortcuts.Load(SettingsPath)[action]);
    }

    [Theory]
    [InlineData("CutStart", Key.A, ModifierKeys.None)]
    [InlineData("CutEnd", Key.E, ModifierKeys.None)]
    [InlineData("VideoStart", Key.A, ModifierKeys.Shift)]
    [InlineData("VideoEnd", Key.E, ModifierKeys.Shift)]
    public void Build61PersonalBindingsWinOverAddedFactoryDefaults(string name, Key key, ModifierKeys modifiers)
    {
        var action = Enum.Parse<PlaybackShortcut>(name);
        var legacy = PlaybackShortcuts.Definitions.Where(d => !IsCut(d.Action)).ToDictionary(d => d.Action, d => d.Standard);
        legacy[PlaybackShortcut.FrameForward] = new(key, modifiers);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(legacy));
        var loaded = PlaybackShortcuts.Load(SettingsPath);
        Assert.Equal(PlaybackShortcut.FrameForward, loaded.Match(key, modifiers));
        Assert.Equal(Key.None, loaded[action].Key);
        foreach (var definition in PlaybackShortcuts.Definitions.Where(d => IsCut(d.Action) && d.Action != action))
            Assert.Equal(definition.Standard, loaded[definition.Action]);
        Assert.Contains("Bereits belegt", loaded.Reset(action));
        loaded.Save(SettingsPath);
        var restarted = PlaybackShortcuts.Load(SettingsPath);
        Assert.Equal(Key.None, restarted[action].Key);
        Assert.Equal(PlaybackShortcut.FrameForward, restarted.Match(key, modifiers));
    }

    [Fact]
    public void ExplicitPersonalAndDisabledBuild62ActionsSurviveUpgrade()
    {
        var personal = PlaybackShortcuts.Definitions.ToDictionary(d => d.Action, d => d.Standard);
        personal[PlaybackShortcut.CutStart] = new(Key.F8);
        personal[PlaybackShortcut.VideoStart] = new(Key.None);
        personal[PlaybackShortcut.FrameForward] = new(Key.A);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(personal));
        var loaded = PlaybackShortcuts.Load(SettingsPath);
        Assert.Equal(new(Key.F8), loaded[PlaybackShortcut.CutStart]);
        Assert.Equal(Key.None, loaded[PlaybackShortcut.VideoStart].Key);
        Assert.Equal(PlaybackShortcut.FrameForward, loaded.Match(Key.A, ModifierKeys.None));
        Assert.Equal(PlaybackShortcut.CutEnd, loaded.Match(Key.E, ModifierKeys.None));
    }

    private static bool IsCut(PlaybackShortcut action) => action is PlaybackShortcut.CutStart or PlaybackShortcut.CutEnd or PlaybackShortcut.VideoStart or PlaybackShortcut.VideoEnd;
}
