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
    [InlineData(2)]
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

    [Fact]
    public void CustomTimingPersistsAlongWithLayout()
    {
        var settings = new AppearanceSettings(InterfaceLayout.PowerUser2)
        {
            NavigationInitialDelayMs = 480,
            NavigationRepeatIntervalMs = 130
        };
        var store = new AppearanceSettingsStore(SettingsPath);
        Assert.True(store.Save(settings));
        Assert.Equal(settings, new AppearanceSettingsStore(SettingsPath).Load());
    }

    [Fact]
    public void OlderSettingsRetainDefaultRepeatSpeed()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsPath, "{\"Layout\":1}");
        var settings = new AppearanceSettingsStore(SettingsPath).Load();
        Assert.Equal(InterfaceLayout.PowerUser, settings.Layout);
        Assert.Equal(350, settings.NavigationInitialDelayMs);
        Assert.Equal(200, settings.NavigationRepeatIntervalMs);
    }

    [Fact]
    public void LoadBoundsUnsafeValues()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsPath, "{\"NavigationInitialDelayMs\":1,\"NavigationRepeatIntervalMs\":5000}");
        var settings = new AppearanceSettingsStore(SettingsPath).Load();
        Assert.Equal(150, settings.NavigationInitialDelayMs);
        Assert.Equal(1000, settings.NavigationRepeatIntervalMs);
    }

    [Fact]
    public void InvalidTimingIsNotSaved()
    {
        var store = new AppearanceSettingsStore(SettingsPath);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            store.Save(new() { NavigationRepeatIntervalMs = 99 }));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }
}
