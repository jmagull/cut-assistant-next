using System.IO;
using System.Windows.Input;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests;

public sealed class PlaybackShortcutsTests
{
    [Fact]
    public void DefaultChordTakesPrecedenceOverSingleFrame()
    {
        var shortcuts = new PlaybackShortcuts();
        Assert.Equal(PlaybackShortcut.SecondsForward, shortcuts.Match(Key.Right, ModifierKeys.None, key => key == Key.S));
        Assert.Equal(PlaybackShortcut.FrameForward, shortcuts.Match(Key.Right, ModifierKeys.None));
        Assert.Equal(PlaybackShortcut.KeyframeForward, shortcuts.Match(Key.Right, ModifierKeys.Control));
    }

    [Fact]
    public void RejectsConflictsAndReservedTextGestures()
    {
        var shortcuts = new PlaybackShortcuts();
        Assert.NotNull(shortcuts.Assign(PlaybackShortcut.FrameForward, new(Key.Left)));
        Assert.NotNull(shortcuts.Assign(PlaybackShortcut.FrameForward, new(Key.C, ModifierKeys.Control)));
        Assert.NotNull(shortcuts.Assign(PlaybackShortcut.FrameForward, new(Key.F4, ModifierKeys.Alt)));
        Assert.Equal(new ShortcutGesture(Key.Right), shortcuts[PlaybackShortcut.FrameForward]);
    }

    [Theory]
    [InlineData(Key.Q, ModifierKeys.None)]
    [InlineData(Key.S, ModifierKeys.None)]
    [InlineData(Key.Oem1, ModifierKeys.None)] // German Ü
    [InlineData(Key.Oem3, ModifierKeys.None)] // German Ö
    [InlineData(Key.Oem7, ModifierKeys.None)] // German Ä
    [InlineData(Key.F, ModifierKeys.Shift)]
    [InlineData(Key.Q, ModifierKeys.Control)]
    [InlineData(Key.Oem7, ModifierKeys.Control | ModifierKeys.Shift)]
    public void LetterBindingsSurviveReloadAndRejectDuplicates(Key key, ModifierKeys modifiers)
    {
        var directory = Directory.CreateTempSubdirectory("can-shortcuts-");
        var file = Path.Combine(directory.FullName, "shortcuts.json");
        try
        {
            var shortcuts = new PlaybackShortcuts();
            var action = key == Key.F ? PlaybackShortcut.FrameLoupe : PlaybackShortcut.FrameForward;
            var gesture = new ShortcutGesture(key, modifiers);
            Assert.Null(shortcuts.Assign(action, gesture));
            Assert.Contains("Bereits belegt", shortcuts.Assign(PlaybackShortcut.TogglePlayback, gesture));
            shortcuts.Save(file);

            var restored = PlaybackShortcuts.Load(file);
            Assert.Equal(gesture, restored[action]);
            Assert.Equal(action, restored.Match(key, modifiers, isDown: pressed => pressed == key));
            Assert.Equal(PlaybackShortcut.SecondsForward, restored.Match(Key.Right, ModifierKeys.None, isDown: key => key == Key.S));
            Assert.Equal(PlaybackShortcut.KeyframeForward, restored.Match(Key.Right, ModifierKeys.Control));
        }
        finally
        {
            File.Delete(file);
            directory.Delete();
        }
    }

    [Fact]
    public void EveryLetterFromAToZCanBeAssignedIndividually()
    {
        var shortcuts = new PlaybackShortcuts();
        Assert.Null(shortcuts.Assign(PlaybackShortcut.CutStart, new(Key.None)));
        Assert.Null(shortcuts.Assign(PlaybackShortcut.CutEnd, new(Key.None)));
        for (var key = Key.A; key <= Key.Z; key++)
        {
            Assert.Null(shortcuts.Assign(PlaybackShortcut.FrameForward, new(key)));
            Assert.Equal(PlaybackShortcut.FrameForward, shortcuts.Match(key, ModifierKeys.None, isDown: pressed => pressed == key));
        }
    }


    [Fact]
    public void LetterAndArrowBindingsSurviveReloadWithExactConflicts()
    {
        var directory = Directory.CreateTempSubdirectory("can-shortcuts-");
        var file = Path.Combine(directory.FullName, "shortcuts.json");
        try
        {
            var shortcuts = new PlaybackShortcuts();
            var right = new ShortcutGesture(Key.Right, Prefix: Key.U);
            var left = new ShortcutGesture(Key.Left, Prefix: Key.U);
            Assert.Null(shortcuts.Assign(PlaybackShortcut.SecondsForward, right));
            Assert.Contains("Bereits belegt", shortcuts.Assign(PlaybackShortcut.SecondsBackward, right));
            Assert.Null(shortcuts.Assign(PlaybackShortcut.SecondsBackward, left));
            Assert.Null(shortcuts.Assign(PlaybackShortcut.FrameForward, new(Key.U)));
            shortcuts.Save(file);

            var restored = PlaybackShortcuts.Load(file);
            Assert.Equal("U + →", restored[PlaybackShortcut.SecondsForward].ToString());
            Assert.Equal("U + ←", restored[PlaybackShortcut.SecondsBackward].ToString());
            Assert.Equal(PlaybackShortcut.SecondsForward, restored.Match(Key.Right, ModifierKeys.None, key => key == Key.U));
            Assert.Equal(PlaybackShortcut.SecondsBackward, restored.Match(Key.Left, ModifierKeys.None, key => key == Key.U));
            Assert.Equal(PlaybackShortcut.FrameBackward, restored.Match(Key.Left, ModifierKeys.None, key => key == Key.S));
            Assert.Equal(PlaybackShortcut.FrameForward, restored.Match(Key.U, ModifierKeys.None));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void LoadsLegacyWithSSettingsAndOtherSavedBindings()
    {
        var directory = Directory.CreateTempSubdirectory("can-shortcuts-");
        var file = Path.Combine(directory.FullName, "shortcuts.json");
        try
        {
            var saved = new
            {
                SecondsForward = new { Key = Key.Up, Modifiers = ModifierKeys.None, WithS = true },
                FrameForward = new { Key = Key.Q, Modifiers = ModifierKeys.None, WithS = false }
            };
            File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(saved));

            var restored = PlaybackShortcuts.Load(file);
            Assert.Equal(new ShortcutGesture(Key.Up, Prefix: Key.S), restored[PlaybackShortcut.SecondsForward]);
            Assert.Equal(PlaybackShortcut.SecondsForward, restored.Match(Key.Up, ModifierKeys.None, key => key == Key.S));
            Assert.Equal(new ShortcutGesture(Key.Q), restored[PlaybackShortcut.FrameForward]);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void PersonalBindingsAndRemovedGesturesSurviveReload()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "shortcuts.json");
        try
        {
            var shortcuts = new PlaybackShortcuts();
            Assert.Null(shortcuts.Assign(PlaybackShortcut.FrameForward, new(Key.F8)));
            Assert.Null(shortcuts.Assign(PlaybackShortcut.KeyframeForward, default));
            shortcuts.Save(path);

            var loaded = PlaybackShortcuts.Load(path);
            Assert.Equal(PlaybackShortcut.FrameForward, loaded.Match(Key.F8, ModifierKeys.None));
            Assert.Null(loaded.Match(Key.Right, ModifierKeys.Control));
            Assert.Null(loaded.Reset(PlaybackShortcut.KeyframeForward));
            Assert.Equal(PlaybackShortcut.KeyframeForward, loaded.Match(Key.Right, ModifierKeys.Control));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
            if (Directory.Exists(Path.GetDirectoryName(path))) Directory.Delete(Path.GetDirectoryName(path)!);
        }
    }
}
