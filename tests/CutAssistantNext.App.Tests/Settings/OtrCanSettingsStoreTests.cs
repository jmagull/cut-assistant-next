using System.Text;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class OtrCanSettingsStoreTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-otr-settings-{Guid.NewGuid():N}");
    public OtrCanSettingsStoreTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);
    private string SettingsPath => Path.Combine(_root, "otr-can-settings.json");

    [Fact]
    public void MissingConfiguration_IsOptionalAndEmpty()
    {
        var settings = new OtrCanSettingsStore(SettingsPath).Load();
        Assert.Empty(settings.ExecutablePath);
        Assert.Empty(settings.FfmsIndexExecutablePath);
        Assert.False(File.Exists(SettingsPath));
    }

    [Fact]
    public void SaveAndLoad_PreservesSeparatePathsAndUtf8()
    {
        var store = new OtrCanSettingsStore(SettingsPath);
        var settings = new OtrCanSettings
        {
            ExecutablePath = $"  \"{Path.Combine(_root, "Schnittmotor ü", "otr_can.exe")}\"  ",
            FfmsIndexExecutablePath = Path.Combine(_root, "anderer Ordner", "ffmsindex.exe")
        };
        store.Save(settings);
        Assert.Equal(settings.Normalize(), store.Load());
        Assert.False(File.ReadAllBytes(SettingsPath).Take(3).SequenceEqual(Encoding.UTF8.GetPreamble()));
        Assert.Single(Directory.GetFiles(_root));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"ExecutablePath\":null,\"FfmsIndexExecutablePath\":null}")]
    [InlineData("{invalid}")]
    public void Load_HandlesEmptyNullOrInvalidData(string json)
    {
        File.WriteAllText(SettingsPath, json);
        Assert.Equal(new OtrCanSettings(), new OtrCanSettingsStore(SettingsPath).Load());
    }

    [Fact]
    public void Load_OlderPartialConfiguration_PreservesExistingPath()
    {
        File.WriteAllText(SettingsPath, "{\"ExecutablePath\":\"C:\\\\Tools\\\\otr_can.exe\"}");
        var settings = new OtrCanSettingsStore(SettingsPath).Load();
        Assert.Equal(@"C:\Tools\otr_can.exe", settings.ExecutablePath);
        Assert.Empty(settings.FfmsIndexExecutablePath);
    }

    [Theory]
    [InlineData("relative.exe")]
    [InlineData("C:\\Tools\\ffmsindex.dll")]
    public void Save_RejectsInvalidPathWithoutChangingExistingConfiguration(string invalid)
    {
        var store = new OtrCanSettingsStore(SettingsPath);
        store.Save(new());
        var before = File.ReadAllBytes(SettingsPath);
        Assert.Throws<ArgumentException>(() => store.Save(new() { FfmsIndexExecutablePath = invalid }));
        Assert.Equal(before, File.ReadAllBytes(SettingsPath));
        Assert.Single(Directory.GetFiles(_root));
    }

    [Fact]
    public void Save_FailureIsReportedAndOtherSettingsStayUnchanged()
    {
        var ffmpeg = Path.Combine(_root, "ffmpeg-settings.json");
        File.WriteAllText(ffmpeg, "existing FFmpeg settings");
        Directory.CreateDirectory(SettingsPath);
        var store = new OtrCanSettingsStore(SettingsPath);
        Assert.Throws<UnauthorizedAccessException>(() => store.Save(new()));
        Assert.Equal("existing FFmpeg settings", File.ReadAllText(ffmpeg));
        Assert.Empty(Directory.GetFiles(_root, "*.tmp"));
    }

    [Fact]
    public void Save_AllowsPartialOrEmptySetupWithoutInstalledTools()
    {
        var store = new OtrCanSettingsStore(SettingsPath);
        store.Save(new() { FfmsIndexExecutablePath = Path.Combine(_root, "not installed yet", "ffmsindex.exe") });
        Assert.NotEmpty(store.Load().FfmsIndexExecutablePath);
        store.Save(new());
        Assert.Equal(new OtrCanSettings(), store.Load());
    }
}
