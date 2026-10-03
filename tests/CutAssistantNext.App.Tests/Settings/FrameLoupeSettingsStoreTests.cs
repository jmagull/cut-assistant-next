using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class FrameLoupeSettingsStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-frame-loupe-{Guid.NewGuid():N}");

    [Fact]
    public void MissingSettings_Uses2000Frames()
    {
        var store = new FrameLoupeSettingsStore(Path.Combine(_directory, "settings.json"));
        Assert.Equal(2000, store.Load().InitialSearchFrames);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(125)]
    [InlineData(100000)]
    public void CustomStart_RoundTrips(int initialFrames)
    {
        var path = Path.Combine(_directory, "settings.json");
        var store = new FrameLoupeSettingsStore(path);
        Assert.True(store.Save(new FrameLoupeSettings(initialFrames)));
        Assert.Equal(initialFrames, store.Load().InitialSearchFrames);
        Assert.False(File.ReadAllBytes(path).Take(3).SequenceEqual(new byte[] { 239, 187, 191 }));
    }

    [Theory]
    [InlineData("{\"InitialSearchFrames\":0}")]
    [InlineData("{\"InitialSearchFrames\":-1}")]
    [InlineData("{\"InitialSearchFrames\":100001}")]
    [InlineData("{invalid")]
    [InlineData("null")]
    public void InvalidSavedValue_UsesDefault(string json)
    {
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, "settings.json");
        File.WriteAllText(path, json);
        Assert.Equal(2000, new FrameLoupeSettingsStore(path).Load().InitialSearchFrames);
    }

    [Fact]
    public void FailedSave_IsReported()
    {
        Directory.CreateDirectory(_directory);
        var blockingFile = Path.Combine(_directory, "file");
        File.WriteAllText(blockingFile, "file instead of directory");
        var store = new FrameLoupeSettingsStore(Path.Combine(blockingFile, "settings.json"));
        Assert.False(store.Save(new FrameLoupeSettings(2000)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100001)]
    public void InvalidStart_IsNotSaved(int frames)
    {
        var store = new FrameLoupeSettingsStore(Path.Combine(_directory, "settings.json"));
        Assert.Throws<ArgumentOutOfRangeException>(() => store.Save(new FrameLoupeSettings(frames)));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
