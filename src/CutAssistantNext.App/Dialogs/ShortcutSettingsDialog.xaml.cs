using System.IO;
using System.Windows;
using System.Windows.Input;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Dialogs;

internal partial class ShortcutSettingsDialog : Window
{
    private sealed record Row(PlaybackShortcut Action, string Group, string Name, string Standard, string Binding);
    private PlaybackShortcut? _capturing;
    private Key _pendingLetter;
    internal PlaybackShortcuts Shortcuts { get; private set; }

    internal ShortcutSettingsDialog(PlaybackShortcuts shortcuts)
    {
        Shortcuts = shortcuts.Clone();
        InitializeComponent();
        Deactivated += (_, _) => _pendingLetter = Key.None;
        Refresh();
    }

    private void Refresh()
    {
        var index = Math.Max(0, Rows.SelectedIndex);
        Rows.ItemsSource = PlaybackShortcuts.Definitions.Select(d =>
            new Row(d.Action, d.Group, d.Name, d.Standard.ToString(), Shortcuts[d.Action].ToString())).ToArray();
        Rows.SelectedIndex = index;
    }

    private void Capture_Click(object sender, RoutedEventArgs e)
    {
        if (Rows.SelectedItem is not Row row) return;
        _capturing = row.Action;
        _pendingLetter = Key.None;
        Status.Text = row.Action is PlaybackShortcut.SecondsBackward or PlaybackShortcut.SecondsForward
            ? "Taste drücken. Für einen Sekundensprung mit zwei Tasten den Buchstaben halten und die Pfeiltaste drücken. Esc bricht die Zuweisung ab."
            : "Neue Kombination drücken. Esc bricht die Zuweisung ab.";
    }

    private void CaptureKey(object sender, KeyEventArgs e)
    {
        if (_capturing is not { } action) return;
        e.Handled = true;
        if (e.IsRepeat) return;
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        if (key is Key.LeftCtrl or Key.RightCtrl or Key.LeftShift or Key.RightShift or Key.LeftAlt or Key.RightAlt or Key.LWin or Key.RWin) return;
        var modifiers = Keyboard.Modifiers;
        if (modifiers == ModifierKeys.None && PlaybackShortcuts.IsLetter(key))
        {
            _pendingLetter = key;
            return;
        }
        if (key == Key.Escape)
        {
            _pendingLetter = Key.None;
            _capturing = null;
            Status.Text = "";
            return;
        }
        var prefix = modifiers == ModifierKeys.None && PlaybackShortcuts.IsArrow(key) ? _pendingLetter : Key.None;
        _pendingLetter = Key.None;
        AssignCaptured(action, new(key, modifiers, prefix));
    }

    private void CaptureKeyUp(object sender, KeyEventArgs e)
    {
        if (_capturing is not { } action || e.Key != _pendingLetter) return;
        e.Handled = true;
        _pendingLetter = Key.None;
        if (Keyboard.Modifiers == ModifierKeys.None) AssignCaptured(action, new(e.Key));
    }

    private void AssignCaptured(PlaybackShortcut action, ShortcutGesture gesture)
    {
        var error = Shortcuts.Assign(action, gesture);
        Status.Text = error ?? "";
        if (error is null) { _capturing = null; Refresh(); }
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        _capturing = null;
        if (Rows.SelectedItem is not Row row) return;
        Status.Text = Shortcuts.Assign(row.Action, default) ?? "";
        Refresh();
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _capturing = null;
        if (Rows.SelectedItem is not Row row) return;
        Status.Text = Shortcuts.Reset(row.Action) ?? "";
        Refresh();
    }

    private void ResetAll_Click(object sender, RoutedEventArgs e)
    {
        _capturing = null;
        Shortcuts = new();
        Status.Text = "";
        Refresh();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _capturing = null;
        try
        {
            Shortcuts.Save(PlaybackShortcuts.DefaultPath);
            DialogResult = true;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            Status.Text = "Tastenkürzel konnten nicht gespeichert werden. " + error.Message;
        }
    }
}
