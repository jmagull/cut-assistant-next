using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class WindowSettingsStoreTests
{
    [Fact]
    public void Load_OldSettingsWithoutVolume_LeavesVolumeUnset()
    {
        var directoryPath =
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

        var settingsFilePath =
            Path.Combine(
                directoryPath,
                "window-settings.json");

        try
        {
            Directory.CreateDirectory(
                directoryPath);

            File.WriteAllText(
                settingsFilePath,
                """
                {
                  "Width": 1200,
                  "Height": 800,
                  "IsMaximized": false
                }
                """);

            var store =
                new WindowSettingsStore(
                    settingsFilePath);

            var settings =
                store.Load();

            Assert.NotNull(
                settings);

            Assert.Null(
                settings.Volume);
        }
        finally
        {
            if (Directory.Exists(
                    directoryPath))
            {
                Directory.Delete(
                    directoryPath,
                    recursive: true);
            }
        }
    }

    [Fact]
    public void SaveAndLoad_PreservesVolume()
    {
        var directoryPath =
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

        var settingsFilePath =
            Path.Combine(
                directoryPath,
                "window-settings.json");

        try
        {
            var store =
                new WindowSettingsStore(
                    settingsFilePath);

            store.Save(
                new WindowSettings
                {
                    Width = 1280,
                    Height = 720,
                    IsMaximized = true,
                    Volume = 37.5
                });

            var settings =
                store.Load();

            Assert.NotNull(
                settings);

            Assert.Equal(
                37.5,
                settings.Volume);
        }
        finally
        {
            if (Directory.Exists(
                    directoryPath))
            {
                Directory.Delete(
                    directoryPath,
                    recursive: true);
            }
        }
    }
}