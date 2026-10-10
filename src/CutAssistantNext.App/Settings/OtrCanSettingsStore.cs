using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed record OtrCanSettings
{
    public string ExecutablePath { get; init; } = string.Empty;
    public string FfmsIndexExecutablePath { get; init; } = string.Empty;

    internal OtrCanSettings Normalize() => this with
    {
        ExecutablePath = ExecutablePath?.Trim().Trim('"').Trim() ?? string.Empty,
        FfmsIndexExecutablePath = FfmsIndexExecutablePath?.Trim().Trim('"').Trim() ?? string.Empty
    };
}

internal sealed class OtrCanSettingsStore
{
    private readonly string _path;

    internal OtrCanSettingsStore() : this(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ApplicationInfo.ProductName, "Settings", "otr-can-settings.json"))
    {
    }

    internal OtrCanSettingsStore(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = Path.GetFullPath(path);
    }

    internal OtrCanSettings Load()
    {
        try
        {
            return JsonSerializer.Deserialize<OtrCanSettings>(File.ReadAllText(_path))?.Normalize() ?? new();
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException)
        {
            return new();
        }
    }

    internal void Save(OtrCanSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var normalized = settings.Normalize();
        OtrCanToolChecker.ValidateOptionalPath(normalized.ExecutablePath);
        OtrCanToolChecker.ValidateOptionalPath(normalized.FfmsIndexExecutablePath);
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var pending = _path + $".{Guid.NewGuid():N}.tmp";
        try
        {
            File.WriteAllText(pending, JsonSerializer.Serialize(normalized,
                new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
            File.Move(pending, _path, overwrite: true);
        }
        finally
        {
            if (File.Exists(pending)) File.Delete(pending);
        }
    }
}
