using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class CutlistSettingsStoreTests
{
    [Fact]
    public void SaveAndLoad_PreservesSettings()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            var store = new CutlistSettingsStore(settingsPath);

            var settings = new CutlistSettings
            {

                QuickTexts =
                [
                    "Mit Cut Assistant Next geschnitten.",
                    "Werbung vollständig entfernt."
                ]
            };

            store.Save(settings);

            var loaded = store.Load();

            Assert.NotNull(loaded);

            Assert.Equal(
                settings.QuickTexts,
                loaded.QuickTexts);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public void CreateDefault_ReturnsFiveSuggestedQuickTexts()
    {
        var settings =
            CutlistSettings.CreateDefault();

        Assert.Equal(
            [
                "Keine Werbung gefunden.",
                "Werbung vollständig entfernt.",
                "Unter Verwendung von MP4Box aus dem GPAC Paket geschnitten.",
                "Teil 1 von 2",
                "Teil 2 von 2"
            ],
            settings.QuickTexts);
        Assert.Equal(CutlistSettings.OtrCanQuickText, settings.QuickText6);
        Assert.Equal(6, settings.GetQuickTexts().Count);
    }

    [Fact]
    public void Load_WithMissingFile_ReturnsNull()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        var store = new CutlistSettingsStore(
            settingsPath);

        var loaded = store.Load();

        Assert.Null(loaded);
    }

    [Fact]
    public void Load_WithInvalidJson_ReturnsNull()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(tempDirectory);

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            File.WriteAllText(
                settingsPath,
                "{ invalid json");

            var store = new CutlistSettingsStore(
                settingsPath);

            var loaded = store.Load();

            Assert.Null(loaded);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public void SaveAndLoad_PreservesUmlautsInQuickTexts()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            var store = new CutlistSettingsStore(
                settingsPath);

            var settings = new CutlistSettings
            {

                QuickTexts =
                [
                    "Werbung vollständig entfernt.",
                    "Übergänge geprüft."
                ]
            };

            store.Save(settings);

            var loaded = store.Load();

            Assert.NotNull(loaded);
            Assert.Equal(
                settings.QuickTexts,
                loaded.QuickTexts);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public void Save_WritesUtf8WithoutBom()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            var store = new CutlistSettingsStore(
                settingsPath);

            store.Save(
                CutlistSettings.CreateDefault());

            var bytes = File.ReadAllBytes(
                settingsPath);

            var hasUtf8Bom =
                bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF;

            Assert.False(hasUtf8Bom);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public void SaveAndLoad_PreservesDefaultAuthor()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            var store = new CutlistSettingsStore(
                settingsPath);

            var settings = new CutlistSettings
            {
                DefaultAuthor = "joerg",

                QuickTexts = []
            };

            store.Save(settings);

            var loaded = store.Load();

            Assert.NotNull(loaded);
            Assert.Equal(
                "joerg",
                loaded.DefaultAuthor);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }
    [Fact]
    public void Load_WithLegacyNameTemplate_LoadsRemainingSettings()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(
            tempDirectory);

        var settingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            File.WriteAllText(
                settingsPath,
                """
                {
                  "DefaultNameTemplate": "%Name% legacy",
                  "DefaultAuthor": "joerg",
                  "QuickTexts": [
                    "Werbung vollständig entfernt."
                  ]
                }
                """);

            var store =
                new CutlistSettingsStore(
                    settingsPath);

            var loaded =
                store.Load();

            Assert.NotNull(
                loaded);

            Assert.Equal(
                "joerg",
                loaded.DefaultAuthor);

            Assert.Equal(
                ["Werbung vollständig entfernt."],
                loaded.QuickTexts);
        }
        finally
        {
            if (Directory.Exists(
                    tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }

}
