using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed class FfmpegSettings
{
    public string FfprobeExecutablePath { get; init; } =
        string.Empty;

    public string FfmpegExecutablePath { get; init; } =
        string.Empty;

    public static FfmpegSettings CreateDefault()
    {
        return new FfmpegSettings();
    }
}

internal sealed class FfmpegSettingsStore
{
    private const string SettingsDirectoryName =
        "Settings";

    private const string SettingsFileName =
        "ffmpeg-settings.json";

    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    private readonly string _settingsFilePath;

    public FfmpegSettingsStore()
        : this(GetDefaultSettingsFilePath())
    {
    }

    internal FfmpegSettingsStore(
        string settingsFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            settingsFilePath);

        _settingsFilePath =
            Path.GetFullPath(
                settingsFilePath);
    }

    public FfmpegSettings? Load()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return null;
            }

            var json =
                File.ReadAllText(
                    _settingsFilePath,
                    Utf8WithoutBom);

            return JsonSerializer.Deserialize<FfmpegSettings>(
                json);
        }
        catch
        {
            return null;
        }
    }

    public void Save(
        FfmpegSettings settings)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        try
        {
            var directoryPath =
                Path.GetDirectoryName(
                    _settingsFilePath);

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(
                    directoryPath);
            }

            var json =
                JsonSerializer.Serialize(
                    settings,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            File.WriteAllText(
                _settingsFilePath,
                json,
                Utf8WithoutBom);
        }
        catch
        {
            // Einstellungsfehler dürfen die Anwendung
            // nicht beeinträchtigen.
        }
    }

    internal static string GetDefaultSettingsFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            ApplicationInfo.ProductName,
            SettingsDirectoryName,
            SettingsFileName);
    }
}
