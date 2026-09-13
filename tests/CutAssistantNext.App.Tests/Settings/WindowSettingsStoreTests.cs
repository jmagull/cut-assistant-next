using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class WindowSettingsStoreTests
{
    [Theory]
    [InlineData(820, 640, false)]
    [InlineData(1280, 900, true)]
    public void SaveAndLoad_WithoutPlayback_PreservesWindowSize(
        double width, double height, bool maximized)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");
        try
        {
            var store = new WindowSettingsStore(path);
            Assert.True(store.Save(new WindowSettings
            {
                Width = width,
                Height = height,
                IsMaximized = maximized
            }));

            var saved = Assert.IsType<WindowSettings>(store.Load());
            Assert.Equal(width, saved.Width);
            Assert.Equal(height, saved.Height);
            Assert.Equal(maximized, saved.IsMaximized);
            Assert.Null(saved.Volume);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Save_WhenParentIsAFile_ReportsFailure()
    {
        var path = Path.GetTempFileName();
        try
        {
            var store = new WindowSettingsStore(Path.Combine(path, "settings.json"));
            Assert.False(store.Save(new WindowSettings { Width = 900, Height = 700 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

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
