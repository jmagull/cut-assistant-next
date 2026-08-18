using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed class CutApplicationSettings
{
    public string Name { get; init; } =
        string.Empty;

    public string ExecutablePath { get; init; } =
        string.Empty;

    public string Version { get; init; } =
        string.Empty;

    public string Options { get; init; } =
        string.Empty;

    public static CutApplicationSettings CreateDefault()
    {
        return new CutApplicationSettings();
    }
}

internal sealed class CutApplicationSettingsStore
{
    private const string SettingsDirectoryName =
        "Settings";

    private const string SettingsFileName =
        "cut-application-settings.json";

    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    private readonly string _settingsFilePath;

    public CutApplicationSettingsStore()
        : this(GetDefaultSettingsFilePath())
    {
    }

    internal CutApplicationSettingsStore(
        string settingsFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            settingsFilePath);

        _settingsFilePath =
            Path.GetFullPath(settingsFilePath);
    }

    public CutApplicationSettings? Load()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return null;
            }

            var json = File.ReadAllText(
                _settingsFilePath,
                Utf8WithoutBom);

            return JsonSerializer.Deserialize<CutApplicationSettings>(
                json);
        }
        catch
        {
            return null;
        }
    }

    public void Save(
        CutApplicationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            var directoryPath =
                Path.GetDirectoryName(_settingsFilePath);

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var json = JsonSerializer.Serialize(
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
