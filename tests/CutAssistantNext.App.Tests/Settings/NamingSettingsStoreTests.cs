using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class NamingSettingsStoreTests
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
            "naming-settings.json");

        try
        {
            var store =
                new NamingSettingsStore(
                    settingsPath);

            var settings =
                new NamingSettings
                {
                    DefaultNameTemplate =
                        "%Name% [%Tag%.%Monat%.%YYYY%]"
                };

            store.Save(
                settings);

            var loaded =
                store.Load();

            Assert.NotNull(
                loaded);

            Assert.Equal(
                settings.DefaultNameTemplate,
                loaded.DefaultNameTemplate);
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

    [Fact]
    public void CreateDefault_ReturnsDefaultTemplate()
    {
        var settings =
            NamingSettings.CreateDefault();

        Assert.Equal(
            "%Name% %Staffel:S%%Folge:E%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]",
            settings.DefaultNameTemplate);
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsNull()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "naming-settings.json");

        var store =
            new NamingSettingsStore(
                settingsPath);

        var loaded =
            store.Load();

        Assert.Null(
            loaded);
    }

    [Fact]
    public void Load_WhenJsonIsInvalid_ReturnsNull()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var settingsPath = Path.Combine(
            tempDirectory,
            "naming-settings.json");

        Directory.CreateDirectory(
            tempDirectory);

        try
        {
            File.WriteAllText(
                settingsPath,
                "{ invalid json");

            var store =
                new NamingSettingsStore(
                    settingsPath);

            var loaded =
                store.Load();

            Assert.Null(
                loaded);
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
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
            "naming-settings.json");

        try
        {
            var store =
                new NamingSettingsStore(
                    settingsPath);

            store.Save(
                NamingSettings.CreateDefault());

            var bytes =
                File.ReadAllBytes(
                    settingsPath);

            var hasUtf8Bom =
                bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF;

            Assert.False(
                hasUtf8Bom);
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
