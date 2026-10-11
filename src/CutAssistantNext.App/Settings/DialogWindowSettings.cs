using System.IO;

namespace CutAssistantNext.App.Settings;

internal enum CutDialogKind
{
    Output,
    Progress,
    Completion,
    CutlistGeneration
}

internal sealed record DialogWindowSize(
    double Width, double Height, double MinWidth, double MinHeight, bool IsMaximized);

internal static class DialogWindowSettings
{
    internal static string GetSettingsFilePath(CutDialogKind kind)
    {
        var fileName = kind switch
        {
            CutDialogKind.Output => "cut-output-window-settings.json",
            CutDialogKind.Progress => "cut-progress-window-settings.json",
            CutDialogKind.Completion => "cut-completion-window-settings.json",
            CutDialogKind.CutlistGeneration => "cutlist-window-settings.json",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        return Path.Combine(Path.GetDirectoryName(WindowSettingsStore.GetDefaultSettingsFilePath())!, fileName);
    }

    internal static DialogWindowSize Restore(
        WindowSettings? saved, double defaultWidth, double defaultHeight,
        double minWidth, double minHeight, double availableWidth, double availableHeight)
    {
        availableWidth = Available(availableWidth, defaultWidth, minWidth);
        availableHeight = Available(availableHeight, defaultHeight, minHeight);
        minWidth = Math.Min(minWidth, availableWidth);
        minHeight = Math.Min(minHeight, availableHeight);
        return new(
            Dimension(saved?.Width, defaultWidth, minWidth, availableWidth),
            Dimension(saved?.Height, defaultHeight, minHeight, availableHeight),
            minWidth, minHeight, saved?.IsMaximized ?? false);
    }

    internal static WindowSettings? Capture(double width, double height, bool isMaximized) =>
        double.IsFinite(width) && double.IsFinite(height) && width > 0 && height > 0
            ? new WindowSettings { Width = width, Height = height, IsMaximized = isMaximized }
            : null;

    private static double Available(double available, double fallback, double minimum) =>
        double.IsFinite(available) && available > 0 ? available : Math.Max(fallback, minimum);

    private static double Dimension(double? saved, double fallback, double minimum, double maximum) =>
        Math.Clamp(saved is { } value && double.IsFinite(value) && value > 0 ? value : fallback, minimum, maximum);
}
