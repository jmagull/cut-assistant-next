using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class AppearanceSettingsStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-appearance-{Guid.NewGuid():N}");
    private string SettingsPath => Path.Combine(_directory, "appearance.json");

    [Fact]
    public void MissingSettingsUseStandardLayout() =>
        Assert.Equal(InterfaceLayout.Standard, new AppearanceSettingsStore(SettingsPath).Load().Layout);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void LayoutRoundTrips(int value)
    {
        var layout = (InterfaceLayout)value;
        var store = new AppearanceSettingsStore(SettingsPath);
        Assert.True(store.Save(new(layout)));
        Assert.Equal(layout, new AppearanceSettingsStore(SettingsPath).Load().Layout);
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("null")]
    [InlineData("{\"Layout\":999}")]
    [InlineData("{}")]
    public void InvalidOrOlderSettingsUseStandardLayout(string json)
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsPath, json);
        Assert.Equal(InterfaceLayout.Standard, new AppearanceSettingsStore(SettingsPath).Load().Layout);
    }

    [Fact]
    public void SaveFailureIsReported()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsPath, "blocks directory creation");
        Assert.False(new AppearanceSettingsStore(Path.Combine(SettingsPath, "settings.json")).Save(new()));
    }

    [Fact]
    public void InvalidLayoutIsNotSaved() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new AppearanceSettingsStore(SettingsPath).Save(new((InterfaceLayout)999)));

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }
}
