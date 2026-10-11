using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Dialogs;

/// <summary>Persists each cut dialog separately; only a completed close writes preferences.</summary>
internal sealed class DialogWindowPersistence
{
    private readonly Window _window;
    private readonly WindowSettingsStore _store;
    private WindowSettings? _closingSettings;
    private bool _wasMaximized;

    private DialogWindowPersistence(Window window, CutDialogKind kind, WindowSettingsStore? store)
    {
        _window = window;
        _store = store ?? new WindowSettingsStore(DialogWindowSettings.GetSettingsFilePath(kind));
        var size = DialogWindowSettings.Restore(_store.Load(), window.Width, window.Height,
            window.MinWidth, window.MinHeight, SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
        window.MinWidth = size.MinWidth;
        window.MinHeight = size.MinHeight;
        window.Width = size.Width;
        window.Height = size.Height;
        _wasMaximized = size.IsMaximized;
        if (size.IsMaximized) window.WindowState = WindowState.Maximized;

        window.StateChanged += StateChanged;
        window.Closing += Closing;
        window.Closed += Closed;
    }

    internal static void Attach(Window window, CutDialogKind kind, WindowSettingsStore? store = null)
    {
        ArgumentNullException.ThrowIfNull(window);
        _ = new DialogWindowPersistence(window, kind, store);
    }

    private void StateChanged(object? sender, EventArgs e)
    {
        if (_window.WindowState != WindowState.Minimized)
            _wasMaximized = _window.WindowState == WindowState.Maximized;
    }

    private void Closing(object? sender, CancelEventArgs e)
    {
        if (!_window.IsLoaded) return;
        // Capture before the HWND is destroyed; maximized/minimized bounds must stay normal.
        var bounds = _window.WindowState == WindowState.Normal
            ? new Rect(0, 0, _window.ActualWidth, _window.ActualHeight)
            : _window.RestoreBounds;
        _closingSettings = DialogWindowSettings.Capture(bounds.Width, bounds.Height, _wasMaximized);
    }

    private void Closed(object? sender, EventArgs e)
    {
        _window.StateChanged -= StateChanged;
        _window.Closing -= Closing;
        _window.Closed -= Closed;
        if (_closingSettings is { } settings && !_store.Save(settings))
            Trace.TraceWarning("Fenstergröße konnte nicht gespeichert werden: {0}", _window.GetType().Name);
    }
}
