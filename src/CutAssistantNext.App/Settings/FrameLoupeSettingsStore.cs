using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Settings;

internal sealed record FrameLoupeSettings(int InitialSearchFrames = HalvingFrameSearch.DefaultInitialStep)
{
    internal const int MaximumInitialSearchFrames = 100000;

    internal bool IsValid => InitialSearchFrames is >= 1 and <= MaximumInitialSearchFrames;
}

internal sealed class FrameLoupeSettingsStore
{
    private readonly string _path;

    internal FrameLoupeSettingsStore()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationInfo.ProductName, "Settings", "frame-loupe-settings.json"))
    {
    }

    internal FrameLoupeSettingsStore(string path)
    {
        _path = Path.GetFullPath(path);
    }

    internal FrameLoupeSettings Load()
    {
        try
        {
            var settings = JsonSerializer.Deserialize<FrameLoupeSettings>(File.ReadAllText(_path));
            return settings is { IsValid: true } ? settings : new FrameLoupeSettings();
        }
        catch
        {
            return new FrameLoupeSettings();
        }
    }

    internal bool Save(FrameLoupeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!settings.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(settings));
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(settings,
                new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
