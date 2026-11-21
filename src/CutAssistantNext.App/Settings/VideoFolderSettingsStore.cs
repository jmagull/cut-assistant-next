using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal enum CutWorkingDirectoryMode { OutputDirectory, WindowsTemp, Custom }

internal sealed record VideoFolderSettings
{
    public CutWorkingDirectoryMode WorkingDirectoryMode { get; init; }
    public string CustomWorkingDirectory { get; init; } = string.Empty;

    internal string ResolveWorkingDirectory(string outputPath) => WorkingDirectoryMode switch
    {
        CutWorkingDirectoryMode.OutputDirectory => Path.GetDirectoryName(Path.GetFullPath(outputPath))!,
        CutWorkingDirectoryMode.WindowsTemp => Path.TrimEndingDirectorySeparator(Path.GetTempPath()),
        CutWorkingDirectoryMode.Custom when Path.IsPathFullyQualified(CustomWorkingDirectory) => Path.GetFullPath(CustomWorkingDirectory),
        _ => throw new IOException("Bitte in den Standardordner-Einstellungen einen vollständigen temporären Arbeitsordner auswählen.")
    };

    public string OriginalVideosDirectory { get; init; } = string.Empty;
    public string CutVideosDirectory { get; init; } = string.Empty;

    public string OwnCutlistsDirectory { get; init; } = string.Empty;

    internal VideoFolderSettings Normalize() => this with
    {
        WorkingDirectoryMode = Enum.IsDefined(WorkingDirectoryMode) ? WorkingDirectoryMode : CutWorkingDirectoryMode.OutputDirectory,
        CustomWorkingDirectory = CustomWorkingDirectory?.Trim() ?? string.Empty,
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
