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
        Assert.Equal(PlaybackShortcut.SecondsForward, shortcuts.Match(Key.Right, ModifierKeys.None, true));
        Assert.Equal(PlaybackShortcut.FrameForward, shortcuts.Match(Key.Right, ModifierKeys.None, false));
        Assert.Equal(PlaybackShortcut.KeyframeForward, shortcuts.Match(Key.Right, ModifierKeys.Control, false));
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
            Assert.Equal(PlaybackShortcut.FrameForward, loaded.Match(Key.F8, ModifierKeys.None, false));
            Assert.Null(loaded.Match(Key.Right, ModifierKeys.Control, false));
            Assert.Null(loaded.Reset(PlaybackShortcut.KeyframeForward));
            Assert.Equal(PlaybackShortcut.KeyframeForward, loaded.Match(Key.Right, ModifierKeys.Control, false));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
            if (Directory.Exists(Path.GetDirectoryName(path))) Directory.Delete(Path.GetDirectoryName(path)!);
        }
    }
}
