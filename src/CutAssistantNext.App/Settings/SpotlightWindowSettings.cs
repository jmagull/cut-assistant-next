using System.IO;
using System.Windows;

namespace CutAssistantNext.App.Settings;

// Only Spotlight uses native screen coordinates; the existing dialogs keep their DIP sizes.
internal sealed record MonitorWorkArea(Rect Bounds, double DpiScale);
internal sealed record SpotlightWindowPlacement(Rect Bounds, double MinWidth, double MinHeight, bool IsMaximized);

internal static class SpotlightWindowSettings
{
    internal static string GetSettingsFilePath() => Path.Combine(
        Path.GetDirectoryName(WindowSettingsStore.GetDefaultSettingsFilePath())!, "spotlight-window-settings.json");

    internal static SpotlightWindowPlacement Restore(WindowSettings? saved, IReadOnlyList<MonitorWorkArea> monitors,
        Rect owner, double defaultWidth, double defaultHeight, double minWidth, double minHeight)
    {
        ArgumentOutOfRangeException.ThrowIfZero(monitors.Count);
        var hasPosition = saved?.Left is { } x && ValidCoordinate(x) && saved.Top is { } y && ValidCoordinate(y);
        var reference = hasPosition
            ? new Rect(saved!.Left!.Value, saved.Top!.Value, Dimension(saved.Width, defaultWidth), Dimension(saved.Height, defaultHeight))
            : owner;
        // Largest overlap preserves second/third monitors, including negative origins.
        // With no overlap, choose the nearest available work area.
        var monitor = monitors.OrderByDescending(m => Overlap(reference, m.Bounds))
            .ThenBy(m => Distance(reference, m.Bounds)).First();
        var scale = monitor.DpiScale;
        var oldScale = saved?.DpiScale is { } dpi && double.IsFinite(dpi) && dpi is >= 0.5 and <= 8 ? dpi : scale;
        var size = DialogWindowSettings.Restore(saved is null ? null : new WindowSettings
        {
            Width = Dimension(saved.Width, defaultWidth * oldScale) * scale / oldScale,
            Height = Dimension(saved.Height, defaultHeight * oldScale) * scale / oldScale,
            IsMaximized = saved.IsMaximized
        }, defaultWidth * scale, defaultHeight * scale, minWidth * scale, minHeight * scale,
            monitor.Bounds.Width, monitor.Bounds.Height);
        var left = hasPosition ? reference.Left : owner.Left + (owner.Width - size.Width) / 2;
        var top = hasPosition ? reference.Top : owner.Top + (owner.Height - size.Height) / 2;
        return new(new Rect(Math.Clamp(left, monitor.Bounds.Left, monitor.Bounds.Right - size.Width),
            Math.Clamp(top, monitor.Bounds.Top, monitor.Bounds.Bottom - size.Height), size.Width, size.Height),
            size.MinWidth / scale, size.MinHeight / scale, size.IsMaximized);
    }

    private static bool ValidCoordinate(double value) => double.IsFinite(value) && Math.Abs(value) <= int.MaxValue / 2;
    private static double Dimension(double value, double fallback) => double.IsFinite(value) && value > 0 ? Math.Min(value, 100000) : fallback;
    private static double Overlap(Rect a, Rect b) { a.Intersect(b); return a.IsEmpty ? 0 : a.Width * a.Height; }
    private static double Distance(Rect a, Rect b)
    {
        var dx = Math.Max(0, Math.Max(b.Left - a.Right, a.Left - b.Right));
        var dy = Math.Max(0, Math.Max(b.Top - a.Bottom, a.Top - b.Bottom));
        return dx * dx + dy * dy;
    }
}
