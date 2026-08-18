using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class FfmpegSettingsStoreTests
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
            "ffmpeg-settings.json");

        try
        {
            var store = new FfmpegSettingsStore(
                settingsPath);

            var settings = new FfmpegSettings
            {
                FfprobeExecutablePath =
                    @"C:\Tools\ffmpeg\bin\ffprobe.exe",
                FfmpegExecutablePath =
                    @"C:\Tools\ffmpeg\bin\ffmpeg.exe"
            };

            store.Save(settings);

            var loaded = store.Load();

            Assert.NotNull(loaded);

            Assert.Equal(
                settings.FfprobeExecutablePath,
                loaded.FfprobeExecutablePath);

            Assert.Equal(
                settings.FfmpegExecutablePath,
                loaded.FfmpegExecutablePath);
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
            FfmpegSettings.CreateDefault();

        Assert.Equal(
            string.Empty,
            settings.FfprobeExecutablePath);

        Assert.Equal(
            string.Empty,
            settings.FfmpegExecutablePath);
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
            "ffmpeg-settings.json");

        var store = new FfmpegSettingsStore(
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
            "ffmpeg-settings.json");

        try
        {
            File.WriteAllText(
                settingsPath,
                "{ invalid json");

            var store = new FfmpegSettingsStore(
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
            "ffmpeg-settings.json");

        try
        {
            var store = new FfmpegSettingsStore(
                settingsPath);

            store.Save(
                FfmpegSettings.CreateDefault());

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
