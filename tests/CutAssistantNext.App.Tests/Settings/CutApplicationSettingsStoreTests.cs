using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class CutApplicationSettingsStoreTests
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
            "cut-application-settings.json");

        try
        {
            var store = new CutApplicationSettingsStore(
                settingsPath);

            var settings = new CutApplicationSettings
            {
                Name = "MP4Box",
                ExecutablePath =
                    @"C:\Program Files\GPAC\MP4Box.exe",
                Version = "26.07",
                Options = "-splitx"
            };

            store.Save(settings);

            var loaded = store.Load();

            Assert.NotNull(loaded);
            Assert.Equal(
                settings.Name,
                loaded.Name);
            Assert.Equal(
                settings.ExecutablePath,
                loaded.ExecutablePath);
            Assert.Equal(
                settings.Version,
                loaded.Version);
            Assert.Equal(
                settings.Options,
                loaded.Options);
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
    public void CreateDefault_ReturnsEmptySettings()
    {
        var settings =
            CutApplicationSettings.CreateDefault();

        Assert.Equal(
            string.Empty,
            settings.Name);
        Assert.Equal(
            string.Empty,
            settings.ExecutablePath);
        Assert.Equal(
            string.Empty,
            settings.Version);
        Assert.Equal(
            string.Empty,
            settings.Options);
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
            "cut-application-settings.json");

        var store = new CutApplicationSettingsStore(
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
            "cut-application-settings.json");

        try
        {
            File.WriteAllText(
                settingsPath,
                "{ invalid json");

            var store = new CutApplicationSettingsStore(
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
    public void Save_WritesUtf8WithoutBom()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "cut-application-settings.json");

        try
        {
            var store = new CutApplicationSettingsStore(
                settingsPath);

            store.Save(
                CutApplicationSettings.CreateDefault());

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
}
