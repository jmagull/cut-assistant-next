using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed class WindowSettings
{
    public double Width { get; init; }

    public double Height { get; init; }

    public bool IsMaximized { get; init; }

    public double? Volume { get; init; }
}

internal sealed class WindowSettingsStore
{
    private const string SettingsDirectoryName =
        "Settings";

    private const string SettingsFileName =
        "window-settings.json";

    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    private readonly string _settingsFilePath;

    public WindowSettingsStore()
        : this(GetDefaultSettingsFilePath())
    {
    }

    internal WindowSettingsStore(
        string settingsFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            settingsFilePath);

        _settingsFilePath =
            Path.GetFullPath(settingsFilePath);
    }

    public WindowSettings? Load()
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

            return JsonSerializer.Deserialize<WindowSettings>(
                json);
        }
        catch
        {
            // Fehlerhafte oder nicht lesbare Einstellungen
            // dürfen den Anwendungsstart nicht verhindern.
            return null;
        }
    }

    public void Save(
        WindowSettings settings)
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