using CutAssistantNext.Cutlists.Metadata;

namespace CutAssistantNext.App.Settings;

internal static class CutApplicationSettingsMapper
{
    public static CutApplicationInfo? ToCutApplicationInfo(
        CutApplicationSettings settings,
        ToolPathResolver? resolver = null)
    {
        ArgumentNullException.ThrowIfNull(settings);

        resolver ??= new ToolPathResolver();

        var hasCustomPath =
            !string.IsNullOrWhiteSpace(
                settings.ExecutablePath);

        var executablePath =
            resolver.TryResolve(
                BundledToolKind.Mp4Box,
                settings.ExecutablePath);

        if (executablePath is null)
        {
            return null;
        }

        return new CutApplicationInfo(
            hasCustomPath ? settings.Name : "MP4Box",
            executablePath,
            hasCustomPath ? settings.Version : string.Empty,
            hasCustomPath ? settings.Options : string.Empty);
    }
}
