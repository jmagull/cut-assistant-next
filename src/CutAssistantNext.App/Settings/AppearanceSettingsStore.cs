using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal enum InterfaceLayout { Standard, PowerUser }

internal sealed record AppearanceSettings(InterfaceLayout Layout = InterfaceLayout.Standard);

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
            return settings is not null && Enum.IsDefined(settings.Layout) ? settings : new();
        }
        catch { return new(); }
    }

    internal bool Save(AppearanceSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!Enum.IsDefined(settings.Layout)) throw new ArgumentOutOfRangeException(nameof(settings));
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
