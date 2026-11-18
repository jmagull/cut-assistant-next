using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Input;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal enum PlaybackShortcut
{
    SecondsBackward = -3, KeyframeBackward = -2, FrameBackward = -1,
    FrameForward = 1, KeyframeForward = 2, SecondsForward = 3,
    TogglePlayback, FrameLoupe, CancelCut
}

internal readonly record struct ShortcutGesture(Key Key, ModifierKeys Modifiers = ModifierKeys.None, Key Prefix = Key.None)
{
    public override string ToString()
    {
        if (Key == Key.None) return "—";
        var parts = new List<string>();
        if (Modifiers.HasFlag(ModifierKeys.Control)) parts.Add("Strg");
        if (Modifiers.HasFlag(ModifierKeys.Shift)) parts.Add("Umschalt");
        if (Modifiers.HasFlag(ModifierKeys.Alt)) parts.Add("Alt");
        if (Prefix != Key.None) parts.Add(KeyLabel(Prefix));
        parts.Add(Key switch
        {
            Key.Left => "←",
            Key.Right => "→",
            Key.Up => "↑",
            Key.Down => "↓",
            Key.Space => "Leertaste",
            Key.Escape => "Esc",
            _ => KeyLabel(Key)
        });
        return string.Join(" + ", parts);
    }

    private static string KeyLabel(Key key)
    {
        if (key is not (Key.Oem1 or Key.Oem3 or Key.Oem7)) return key.ToString();

        // Use the active Windows layout for display only. Store and match the WPF key.
        const uint virtualKeyToCharacter = 2;
        var character = MapVirtualKeyEx((uint)KeyInterop.VirtualKeyFromKey(key), virtualKeyToCharacter, GetKeyboardLayout(0));
        return character is > 32 and <= char.MaxValue ? ((char)character).ToString() : key.ToString();
    }

    [DllImport("user32.dll", EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
    private static extern uint MapVirtualKeyEx(uint code, uint mapType, nint layout);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetKeyboardLayout(uint threadId);
}

internal sealed class PlaybackShortcuts
{
    private sealed record SavedShortcut(Key Key, ModifierKeys Modifiers, Key Prefix, bool WithS);

    internal sealed record Definition(PlaybackShortcut Action, string Group, string Name, ShortcutGesture Standard);
    internal static readonly IReadOnlyList<Definition> Definitions =
    [
        new(PlaybackShortcut.TogglePlayback, "Wiedergabe", "Wiedergabe starten oder pausieren", new(Key.Space)),
        new(PlaybackShortcut.SecondsBackward, "Navigation", "Sekundensprung zurück", new(Key.Left, Prefix: Key.S)),
        new(PlaybackShortcut.SecondsForward, "Navigation", "Sekundensprung vor", new(Key.Right, Prefix: Key.S)),
        new(PlaybackShortcut.FrameBackward, "Navigation", "Ein Bild zurück", new(Key.Left)),
        new(PlaybackShortcut.FrameForward, "Navigation", "Ein Bild vor", new(Key.Right)),
        new(PlaybackShortcut.KeyframeBackward, "Navigation", "Vorheriger Keyframe", new(Key.Left, ModifierKeys.Control)),
        new(PlaybackShortcut.KeyframeForward, "Navigation", "Nächster Keyframe", new(Key.Right, ModifierKeys.Control)),
        new(PlaybackShortcut.FrameLoupe, "Schnittbearbeitung", "Frame-Lupe öffnen", new(Key.F, ModifierKeys.Shift)),
        new(PlaybackShortcut.CancelCut, "Schnittbearbeitung", "Vorgemerkten Anfang verwerfen", new(Key.Escape))
    ];

    private readonly Dictionary<PlaybackShortcut, ShortcutGesture> _bindings = Definitions.ToDictionary(d => d.Action, d => d.Standard);
    internal ShortcutGesture this[PlaybackShortcut shortcut] => _bindings[shortcut];
    internal PlaybackShortcuts Clone()
    {
        var copy = new PlaybackShortcuts();
        foreach (var (action, gesture) in _bindings) copy._bindings[action] = gesture;
        return copy;
    }

    internal PlaybackShortcut? Match(Key key, ModifierKeys modifiers, Func<Key, bool>? isDown = null)
    {
        if (modifiers == ModifierKeys.None && IsArrow(key) && isDown is not null)
        {
            foreach (var (action, binding) in _bindings)
                if (binding.Key == key && binding.Prefix != Key.None && isDown(binding.Prefix)) return action;
        }
        return Find(new(key, modifiers));
    }

    internal bool IsArrowPrefix(Key key) => key != Key.None && _bindings.Values.Any(binding => binding.Prefix == key);

    internal static bool IsArrow(Key key) => key is Key.Left or Key.Right or Key.Up or Key.Down;

    internal static bool IsLetter(Key key) => key is (>= Key.A and <= Key.Z) or Key.Oem1 or Key.Oem3 or Key.Oem7;

    private PlaybackShortcut? Find(ShortcutGesture gesture)
    {
        foreach (var (action, binding) in _bindings)
            if (binding.Key != Key.None && binding == gesture) return action;
        return null;
    }

    internal string? Assign(PlaybackShortcut action, ShortcutGesture gesture)
    {
        if (!IsAllowed(action, gesture))
            return gesture.Prefix != Key.None
                ? "Buchstabentaste und Pfeiltaste lassen sich als Kombination für Sekundensprünge belegen."
                : "Diese Taste ist für Windows, die Texteingabe oder Dialoge reserviert.";
        if (Find(gesture) is { } other && other != action)
            return "Bereits belegt durch „" + Definitions.First(d => d.Action == other).Name + "“.";
        _bindings[action] = gesture;
        return null;
    }

    internal string? Reset(PlaybackShortcut action) => Assign(action, Definitions.First(d => d.Action == action).Standard);

    private static bool IsAllowed(PlaybackShortcut action, ShortcutGesture gesture)
    {
        var (key, modifiers, prefix) = gesture;
        if (prefix != Key.None)
            return (action is PlaybackShortcut.SecondsBackward or PlaybackShortcut.SecondsForward) &&
                IsLetter(prefix) && IsArrow(key) && modifiers == ModifierKeys.None;
        if (key == Key.None) return action != PlaybackShortcut.CancelCut;
        if (key == Key.Escape) return action == PlaybackShortcut.CancelCut && modifiers == ModifierKeys.None;
        if ((modifiers & (ModifierKeys.Windows | ModifierKeys.Alt)) != 0) return false;
        if (IsArrow(key) || key == Key.Space) return true;
        if (key is >= Key.F2 and <= Key.F12) return true;
        if (modifiers == ModifierKeys.Control && key is Key.A or Key.C or Key.V or Key.X or Key.Y or Key.Z) return false;
        return IsLetter(key);
    }

    internal static string DefaultPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ApplicationInfo.ProductName, "Settings", "shortcuts.json");

    internal static PlaybackShortcuts Load(string path)
    {
        try
        {
            var saved = JsonSerializer.Deserialize<Dictionary<PlaybackShortcut, SavedShortcut>>(File.ReadAllText(path));
            if (saved is null) return new();
            var shortcuts = new PlaybackShortcuts();
            foreach (var (action, entry) in saved)
            {
                if (entry is null) return new();
                var prefix = entry.WithS && entry.Prefix == Key.None ? Key.S : entry.Prefix;
                var gesture = new ShortcutGesture(entry.Key, entry.Modifiers, prefix);
                if (!shortcuts._bindings.ContainsKey(action) || !IsAllowed(action, gesture)) return new();
                shortcuts._bindings[action] = gesture;
            }
            var assigned = shortcuts._bindings.Values.Where(g => g.Key != Key.None).ToArray();
            return assigned.Distinct().Count() == assigned.Length ? shortcuts : new();
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException) { return new(); }
    }

    internal void Save(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(_bindings));
            File.Move(temporary, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }
}
