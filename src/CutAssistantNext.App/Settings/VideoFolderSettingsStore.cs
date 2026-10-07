using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed record VideoFolderSettings
{
    public string OriginalVideosDirectory { get; init; } = string.Empty;
    public string CutVideosDirectory { get; init; } = string.Empty;

    public string OwnCutlistsDirectory { get; init; } = string.Empty;

    internal VideoFolderSettings Normalize() => this with
    {
        OriginalVideosDirectory = OriginalVideosDirectory?.Trim() ?? string.Empty,
        CutVideosDirectory = CutVideosDirectory?.Trim() ?? string.Empty,
        OwnCutlistsDirectory = OwnCutlistsDirectory?.Trim() ?? string.Empty
    };
}

internal sealed class VideoFolderSettingsStore
{
    private readonly string _path;

    internal VideoFolderSettingsStore()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationInfo.ProductName, "Settings", "video-folder-settings.json"))
    {
    }

    internal VideoFolderSettingsStore(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = Path.GetFullPath(path);
    }

    internal VideoFolderSettings Load()
    {
        try
        {
            return JsonSerializer.Deserialize<VideoFolderSettings>(File.ReadAllText(_path))?.Normalize()
                ?? new VideoFolderSettings();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            return new VideoFolderSettings();
        }
    }

    internal bool Save(VideoFolderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(settings.Normalize(),
                new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
