using CutAssistantNext.Cutlists.Metadata;

namespace CutAssistantNext.App.Settings;

internal static class CutApplicationSettingsMapper
{
    public static CutApplicationInfo? ToCutApplicationInfo(
        CutApplicationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(
                settings.ExecutablePath))
        {
            return null;
        }

        return new CutApplicationInfo(
            settings.Name,
            settings.ExecutablePath,
            settings.Version,
            settings.Options);
    }
}
