using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed class CutlistSettings
{

    public string DefaultAuthor { get; init; } =
        string.Empty;

    public List<string> QuickTexts { get; init; } =
        [];

    public static CutlistSettings CreateDefault()
    {
        return new CutlistSettings
        {

            QuickTexts =
            [
                "Keine Werbung gefunden.",
                "Werbung vollständig entfernt.",
                "Unter Verwendung von MP4Box aus dem GPAC Paket geschnitten.",
                "Teil 1 von 2",
                "Teil 2 von 2"
            ]
        };
    }
}

internal sealed class CutlistSettingsStore
{
    private const string SettingsDirectoryName =
        "Settings";

    private const string SettingsFileName =
        "cutlist-settings.json";

    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    private readonly string _settingsFilePath;

    public CutlistSettingsStore()
        : this(GetDefaultSettingsFilePath())
    {
    }

    internal CutlistSettingsStore(
        string settingsFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            settingsFilePath);

        _settingsFilePath =
            Path.GetFullPath(settingsFilePath);
    }

    public CutlistSettings? Load()
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

            return JsonSerializer.Deserialize<CutlistSettings>(
                json);
        }
        catch
        {
            return null;
        }
    }

    public void Save(
        CutlistSettings settings)
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
