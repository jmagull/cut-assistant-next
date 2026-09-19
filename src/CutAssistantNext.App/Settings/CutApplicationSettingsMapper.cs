using System.IO;
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

        if (!hasCustomPath &&
            !File.Exists(
                resolver.GetBundledPath(
                    BundledToolKind.Mp4Box)))
        {
            return null;
        }

        var executablePath =
            resolver.Resolve(
                BundledToolKind.Mp4Box,
                settings.ExecutablePath);

        return new CutApplicationInfo(
            hasCustomPath ? settings.Name : "MP4Box",
            executablePath,
            hasCustomPath ? settings.Version : string.Empty,
            hasCustomPath ? settings.Options : string.Empty);
    }
}
