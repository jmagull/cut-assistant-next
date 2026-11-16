using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Settings;

internal enum InterfaceLayout { Standard = 0, PowerUser = 1, PowerUser2 = 2 }

internal sealed record AppearanceSettings(InterfaceLayout Layout = InterfaceLayout.Standard,
    CutEditingMode CutEditing = CutEditingMode.Remove)
{
    internal const int DefaultInitialDelayMs = 350;
    internal const int DefaultRepeatIntervalMs = 200;
    internal const int MinimumInitialDelayMs = 150;
    internal const int MaximumInitialDelayMs = 1500;
    internal const int MinimumRepeatIntervalMs = 100;
    internal const int MaximumRepeatIntervalMs = 1000;

    public int NavigationInitialDelayMs { get; init; } = DefaultInitialDelayMs;
    public int NavigationRepeatIntervalMs { get; init; } = DefaultRepeatIntervalMs;

    internal bool HasValidNavigationTiming =>
        NavigationInitialDelayMs is >= MinimumInitialDelayMs and <= MaximumInitialDelayMs &&
        NavigationRepeatIntervalMs is >= MinimumRepeatIntervalMs and <= MaximumRepeatIntervalMs;
}

internal sealed class AppearanceSettingsStore
{
    private readonly string _path;

    internal AppearanceSettingsStore() : this(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ApplicationInfo.ProductName, "Settings", "appearance-settings.json"))
    { }

    internal AppearanceSettingsStore(string path) => _path = Path.GetFullPath(path);

    internal AppearanceSettings Load()
    {
        try
        {
            var settings = JsonSerializer.Deserialize<AppearanceSettings>(File.ReadAllText(_path));
            return settings is not null && Enum.IsDefined(settings.Layout)
                ? settings with
                {
                    CutEditing = Enum.IsDefined(settings.CutEditing) ? settings.CutEditing : CutEditingMode.Remove,
                    NavigationInitialDelayMs = Math.Clamp(settings.NavigationInitialDelayMs,
                        AppearanceSettings.MinimumInitialDelayMs, AppearanceSettings.MaximumInitialDelayMs),
                    NavigationRepeatIntervalMs = Math.Clamp(settings.NavigationRepeatIntervalMs,
                        AppearanceSettings.MinimumRepeatIntervalMs, AppearanceSettings.MaximumRepeatIntervalMs)
                }
                : new();
        }
        catch { return new(); }
    }

    internal bool Save(AppearanceSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!Enum.IsDefined(settings.Layout)) throw new ArgumentOutOfRangeException(nameof(settings));
        if (!Enum.IsDefined(settings.CutEditing)) throw new ArgumentOutOfRangeException(nameof(settings));
        if (!settings.HasValidNavigationTiming) throw new ArgumentOutOfRangeException(nameof(settings));
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(settings,
                new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
            return true;
        }
        catch { return false; }
    }
}
