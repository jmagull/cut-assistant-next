using System.IO;
using System.Text.Json;

namespace CutAssistantNext.App.Settings;

internal sealed class NamingSettingsLoader
{
    private readonly NamingSettingsStore _store;
    private readonly string _legacySettingsFilePath;

    public NamingSettingsLoader()
        : this(
            new NamingSettingsStore(),
            CutlistSettingsStore.GetDefaultSettingsFilePath())
    {
    }

    internal NamingSettingsLoader(
        NamingSettingsStore store,
        string legacySettingsFilePath)
    {
        ArgumentNullException.ThrowIfNull(
            store);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            legacySettingsFilePath);

        _store =
            store;

        _legacySettingsFilePath =
            Path.GetFullPath(
                legacySettingsFilePath);
    }

    public NamingSettings Load()
    {
        var currentSettings =
            _store.Load();

        if (currentSettings is not null &&
            !string.IsNullOrWhiteSpace(
                currentSettings.DefaultNameTemplate))
        {
            return currentSettings;
        }

        var legacyNameTemplate =
            LoadLegacyNameTemplate();

        if (!string.IsNullOrWhiteSpace(
                legacyNameTemplate))
        {
            var migratedSettings =
                new NamingSettings
                {
                    DefaultNameTemplate =
                        legacyNameTemplate
                };

            _store.Save(
                migratedSettings);

            return migratedSettings;
        }

        var defaultSettings =
            NamingSettings.CreateDefault();

        _store.Save(
            defaultSettings);

        return defaultSettings;
    }

    private string? LoadLegacyNameTemplate()
    {
        try
        {
            if (!File.Exists(
                    _legacySettingsFilePath))
            {
                return null;
            }

            var json =
                File.ReadAllText(
                    _legacySettingsFilePath);

            var legacySettings =
                JsonSerializer.Deserialize<LegacyCutlistSettings>(
                    json);

            return legacySettings?.DefaultNameTemplate;
        }
        catch
        {
            return null;
        }
    }

    private sealed class LegacyCutlistSettings
    {
        public string DefaultNameTemplate { get; init; } =
            string.Empty;
    }
}
