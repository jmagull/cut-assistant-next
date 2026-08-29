using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal sealed class NamingSettings
{
    public string DefaultNameTemplate { get; init; } =
        string.Empty;

    public static NamingSettings CreateDefault()
    {
        return new NamingSettings
        {
            DefaultNameTemplate =
                "%Name% %Staffel:S%%Folge:E%%Folgentitel: - % [%Tag%.%Monat%.%YYYY%]"
        };
    }
}

internal sealed class NamingSettingsStore
{
    private const string SettingsDirectoryName =
        "Settings";

    private const string SettingsFileName =
        "naming-settings.json";

    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    private readonly string _settingsFilePath;

    public NamingSettingsStore()
        : this(GetDefaultSettingsFilePath())
    {
    }

    internal NamingSettingsStore(
        string settingsFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            settingsFilePath);

        _settingsFilePath =
            Path.GetFullPath(settingsFilePath);
    }

    public NamingSettings? Load()
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

            return JsonSerializer.Deserialize<NamingSettings>(
                json);
        }
        catch
        {
            return null;
        }
    }

    public void Save(
        NamingSettings settings)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        try
        {
            var directoryPath =
                Path.GetDirectoryName(
                    _settingsFilePath);

            if (!string.IsNullOrWhiteSpace(
                    directoryPath))
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
