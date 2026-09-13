using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class CutlistServerSettingsStoreTests
{
    [Fact]
    public void SaveAndLoad_PreservesPersonalServerUrl()
    {
        var tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                "CutAssistantNext.Tests",
                Guid.NewGuid().ToString("N"));

        var settingsPath =
            Path.Combine(
                tempDirectory,
                "cutlist-server-settings.json");

        try
        {
            var store =
                new CutlistServerSettingsStore(
                    settingsPath);

            var settings =
                new CutlistServerSettings
                {
                    PersonalServerUrl =
                        "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/"
                };

            store.Save(
                settings);

            var loaded =
                store.Load();

            Assert.NotNull(
                loaded);

            Assert.Equal(
                settings.PersonalServerUrl,
                loaded.PersonalServerUrl);
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
    public void Load_WithMissingFile_ReturnsNull()
    {
        var tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                "CutAssistantNext.Tests",
                Guid.NewGuid().ToString("N"));

        var settingsPath =
            Path.Combine(
                tempDirectory,
                "cutlist-server-settings.json");

        var store =
            new CutlistServerSettingsStore(
                settingsPath);

        var loaded =
            store.Load();

        Assert.Null(
            loaded);
    }
}