using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class NamingSettingsLoaderTests
{
    [Fact]
    public void Load_WhenNamingSettingsExist_UsesNamingSettings()
    {
        var tempDirectory = CreateTempDirectory();

        var namingSettingsPath = Path.Combine(
            tempDirectory,
            "naming-settings.json");

        var legacySettingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            var store =
                new NamingSettingsStore(
                    namingSettingsPath);

            store.Save(
                new NamingSettings
                {
                    DefaultNameTemplate =
                        "%Name% modern"
                });

            File.WriteAllText(
                legacySettingsPath,
                "{\"DefaultNameTemplate\":\"%Name% legacy\"}");

            var loader =
                new NamingSettingsLoader(
                    store,
                    legacySettingsPath);

            var settings =
                loader.Load();

            Assert.Equal(
                "%Name% modern",
                settings.DefaultNameTemplate);
        }
        finally
        {
            DeleteTempDirectory(
                tempDirectory);
        }
    }

    [Fact]
    public void Load_WhenNamingSettingsAreMissing_MigratesLegacyTemplate()
    {
        var tempDirectory = CreateTempDirectory();

        var namingSettingsPath = Path.Combine(
            tempDirectory,
            "naming-settings.json");

        var legacySettingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            File.WriteAllText(
                legacySettingsPath,
                "{\"DefaultNameTemplate\":\"%Name% legacy\"}");

            var store =
                new NamingSettingsStore(
                    namingSettingsPath);

            var loader =
                new NamingSettingsLoader(
                    store,
                    legacySettingsPath);

            var settings =
                loader.Load();

            Assert.Equal(
                "%Name% legacy",
                settings.DefaultNameTemplate);

            var persisted =
                store.Load();

            Assert.NotNull(
                persisted);

            Assert.Equal(
                "%Name% legacy",
                persisted.DefaultNameTemplate);
        }
        finally
        {
            DeleteTempDirectory(
                tempDirectory);
        }
    }

    [Fact]
    public void Load_WhenNoSettingsExist_UsesAndPersistsDefault()
    {
        var tempDirectory = CreateTempDirectory();

        var namingSettingsPath = Path.Combine(
            tempDirectory,
            "naming-settings.json");

        var legacySettingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            var store =
                new NamingSettingsStore(
                    namingSettingsPath);

            var loader =
                new NamingSettingsLoader(
                    store,
                    legacySettingsPath);

            var settings =
                loader.Load();

            Assert.Equal(
                NamingSettings.CreateDefault().DefaultNameTemplate,
                settings.DefaultNameTemplate);

            var persisted =
                store.Load();

            Assert.NotNull(
                persisted);

            Assert.Equal(
                settings.DefaultNameTemplate,
                persisted.DefaultNameTemplate);
        }
        finally
        {
            DeleteTempDirectory(
                tempDirectory);
        }
    }

    [Fact]
    public void Load_WhenLegacyJsonIsInvalid_UsesAndPersistsDefault()
    {
        var tempDirectory = CreateTempDirectory();

        var namingSettingsPath = Path.Combine(
            tempDirectory,
            "naming-settings.json");

        var legacySettingsPath = Path.Combine(
            tempDirectory,
            "cutlist-settings.json");

        try
        {
            File.WriteAllText(
                legacySettingsPath,
                "{ invalid json");

            var store =
                new NamingSettingsStore(
                    namingSettingsPath);

            var loader =
                new NamingSettingsLoader(
                    store,
                    legacySettingsPath);

            var settings =
                loader.Load();

            Assert.Equal(
                NamingSettings.CreateDefault().DefaultNameTemplate,
                settings.DefaultNameTemplate);

            var persisted =
                store.Load();

            Assert.NotNull(
                persisted);

            Assert.Equal(
                settings.DefaultNameTemplate,
                persisted.DefaultNameTemplate);
        }
        finally
        {
            DeleteTempDirectory(
                tempDirectory);
        }
    }

    private static string CreateTempDirectory()
    {
        var tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                "CutAssistantNext.Tests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(
            tempDirectory);

        return tempDirectory;
    }

    private static void DeleteTempDirectory(
        string tempDirectory)
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
