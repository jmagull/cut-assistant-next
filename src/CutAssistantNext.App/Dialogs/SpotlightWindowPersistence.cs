using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Dialogs;

internal sealed class SpotlightWindowPersistence
{
    private readonly Window _window;
    private readonly WindowSettingsStore _store;
    private WindowSettings? _closingSettings;
    private bool _wasMaximized;

    private SpotlightWindowPersistence(Window window, WindowSettingsStore store)
    {
        _window = window;
        _store = store;
        window.SourceInitialized += SourceInitialized;
        window.StateChanged += StateChanged;
        window.Closing += Closing;
        window.Closed += Closed;
    }

    internal static void Attach(Window window, WindowSettingsStore? store = null) =>
        _ = new SpotlightWindowPersistence(window, store ?? new(SpotlightWindowSettings.GetSettingsFilePath()));

    private void SourceInitialized(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(_window).Handle;
        var monitors = NativeWindowGeometry.GetMonitors();
        if (monitors.Count == 0) return; // Let Windows perform its default placement.
        var owner = _window.Owner is { } parent ? new WindowInteropHelper(parent).Handle : handle;
        var ownerBounds = NativeWindowGeometry.GetBounds(owner) ?? monitors[0].Bounds;
        var placement = SpotlightWindowSettings.Restore(_store.Load(), monitors, ownerBounds,
            _window.Width, _window.Height, _window.MinWidth, _window.MinHeight);
        _window.MinWidth = placement.MinWidth;
        _window.MinHeight = placement.MinHeight;
        _window.WindowStartupLocation = WindowStartupLocation.Manual;
        NativeWindowGeometry.Place(handle, placement.Bounds);
        _wasMaximized = placement.IsMaximized;
        if (_wasMaximized) _window.WindowState = WindowState.Maximized;
    }

    private void StateChanged(object? sender, EventArgs e)
    {
        if (_window.WindowState != WindowState.Minimized)
            _wasMaximized = _window.WindowState == WindowState.Maximized;
    }

    private void Closing(object? sender, CancelEventArgs e)
    {
        if (!_window.IsLoaded) return;
        _closingSettings = NativeWindowGeometry.Capture(new WindowInteropHelper(_window).Handle, _wasMaximized);
    }

    private void Closed(object? sender, EventArgs e)
    {
        _window.SourceInitialized -= SourceInitialized;
        _window.StateChanged -= StateChanged;
        _window.Closing -= Closing;
        _window.Closed -= Closed;
        // Spotlight may cancel Closing for asynchronous cleanup or an unsaved-work prompt.
        // Only an actual Closed event commits the last captured normal geometry.
        if (_closingSettings is { } settings && !_store.Save(settings))
            Trace.TraceWarning("Spotlight-Fensterposition konnte nicht gespeichert werden.");
    }
}

internal static class NativeWindowGeometry
{
    internal static IReadOnlyList<MonitorWorkArea> GetMonitors()
    {
        var monitors = new List<MonitorWorkArea>();
        EnumDisplayMonitors(0, 0, (nint monitor, nint dc, ref NativeRect rect, nint data) =>
        {
            var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
            if (GetMonitorInfo(monitor, ref info))
            {
                var scale = GetDpiForMonitor(monitor, 0, out var dpi, out _) == 0 ? dpi / 96d : 1;
                if (info.Work.Right > info.Work.Left && info.Work.Bottom > info.Work.Top)
                    monitors.Add(new(info.Work.ToRect(), scale > 0 ? scale : 1));
            }
            return true;
        }, 0);
        return monitors;
    }

    internal static Rect? GetBounds(nint handle) => GetWindowRect(handle, out var rect) ? rect.ToRect() : null;

    internal static void Place(nint handle, Rect bounds)
    {
        if (!SetWindowPos(handle, 0, (int)Math.Round(bounds.Left), (int)Math.Round(bounds.Top),
            (int)Math.Round(bounds.Width), (int)Math.Round(bounds.Height), 0x14)) // NOZORDER | NOACTIVATE
            Trace.TraceWarning("Spotlight-Fensterposition konnte nicht wiederhergestellt werden.");
    }

    internal static WindowSettings? Capture(nint handle, bool maximized)
    {
        var placement = new WindowPlacement { Length = Marshal.SizeOf<WindowPlacement>() };
        if (!GetWindowPlacement(handle, ref placement)) return null;
        var rect = placement.Normal.ToRect();
        var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        if (GetMonitorInfo(MonitorFromWindow(handle, 2), ref info))
            rect.Offset(info.Work.Left - info.Monitor.Left, info.Work.Top - info.Monitor.Top);
        // WINDOWPLACEMENT uses workspace coordinates; settings and SetWindowPos use screen coordinates.
        if (rect.Width <= 0 || rect.Height <= 0) return null;
        return new WindowSettings
        {
            Left = rect.Left,
            Top = rect.Top,
            Width = rect.Width,
            Height = rect.Height,
            DpiScale = Math.Max(96, GetDpiForWindow(handle)) / 96d,
            IsMaximized = maximized
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect { public int Left, Top, Right, Bottom; public readonly Rect ToRect() => new(Left, Top, Math.Max(0, Right - Left), Math.Max(0, Bottom - Top)); }
    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint { public int X, Y; }
    [StructLayout(LayoutKind.Sequential)]
    private struct WindowPlacement { public int Length, Flags, Show; public NativePoint Min, Max; public NativeRect Normal; }
    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo { public int Size; public NativeRect Monitor, Work; public int Flags; }
    private delegate bool MonitorCallback(nint monitor, nint dc, ref NativeRect rect, nint data);
    [DllImport("user32.dll")] private static extern bool EnumDisplayMonitors(nint dc, nint clip, MonitorCallback callback, nint data);
    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern bool GetMonitorInfo(nint monitor, ref MonitorInfo info);
    [DllImport("user32.dll")] private static extern nint MonitorFromWindow(nint window, int flags);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(nint window, out NativeRect rect);
    [DllImport("user32.dll")] private static extern bool GetWindowPlacement(nint window, ref WindowPlacement placement);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(nint window);
    [DllImport("shcore.dll")] private static extern int GetDpiForMonitor(nint monitor, int type, out uint x, out uint y);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(nint window, nint after, int x, int y, int width, int height, int flags);
}
