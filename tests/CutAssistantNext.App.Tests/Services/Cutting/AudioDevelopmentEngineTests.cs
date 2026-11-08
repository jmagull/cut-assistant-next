using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class AudioDevelopmentEngineTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(@"C:\configured\otr_can.exe")]
    [InlineData(@"C:\previous-installation\otr_can.exe")]
    [InlineData(@"C:\previous-installation\runtime\otr-can\otr_can.exe")]
    public void RegularBuildUsesBundledMotorRegardlessOfSavedPath(string? savedPath)
    {
        var root = Path.Combine(Path.GetTempPath(), "can bundled engine " + Guid.NewGuid().ToString("N"));
        var path = Path.Combine(root, "tools", "otr-can", "otr_can.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        try
        {
            File.WriteAllText(path, "test engine");
            var settings = new OtrCanSettings { ExecutablePath = savedPath ?? "" };
            Assert.Equal(path, AudioDevelopmentEngine.Resolve(settings, root).ExecutablePath);
            Assert.Equal(savedPath ?? "", settings.ExecutablePath);
        }
        finally { Directory.Delete(root, true); }
    }
    [Fact]
    public void PrivateBuildUsesOnlyItsOwnMotorAndPreservesSavedChoiceAndIndexer()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-review-engine-" + Guid.NewGuid().ToString("N"));
        var path = Path.Combine(root, "tools", "otr-can", "otr_can.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        try
        {
            File.WriteAllText(path, "review engine");
            var settings = new OtrCanSettings { ExecutablePath = @"C:\installed\otr_can.exe", FfmsIndexExecutablePath = @"C:\tools\indexer.exe" };
            var result = AudioDevelopmentEngine.Resolve(settings, root);
            Assert.Equal(path, result.ExecutablePath); Assert.Equal(settings.FfmsIndexExecutablePath, result.FfmsIndexExecutablePath);
            Assert.Equal(@"C:\installed\otr_can.exe", settings.ExecutablePath);
        }
        finally { Directory.Delete(root, true); }
    }
    [Fact]
    public void IncompletePackageNeverFallsBackToTheLegacyRuntimeMotor()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-legacy-engine-" + Guid.NewGuid().ToString("N"));
        var legacy = Path.Combine(root, "runtime", "otr-can", "otr_can.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(legacy)!);
        try
        {
            File.WriteAllText(legacy, "legacy engine");
            var settings = new OtrCanSettings { ExecutablePath = legacy };
            Assert.Throws<FileNotFoundException>(() => AudioDevelopmentEngine.Resolve(settings, root));
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void ToolsMotorWinsWhenAnOldRuntimeCopyIsPresent()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-engine-upgrade-" + Guid.NewGuid().ToString("N"));
        var current = Path.Combine(root, "tools", "otr-can", "otr_can.exe");
        var legacy = Path.Combine(root, "runtime", "otr-can", "otr_can.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(current)!);
        Directory.CreateDirectory(Path.GetDirectoryName(legacy)!);
        try
        {
            File.WriteAllText(current, "current engine");
            File.WriteAllText(legacy, "legacy engine");
            var settings = new OtrCanSettings { ExecutablePath = legacy };
            Assert.Equal(current, AudioDevelopmentEngine.Resolve(settings, root).ExecutablePath);
            Assert.Equal(legacy, settings.ExecutablePath);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void ApplicationHasOnlyTheRegularProductName()
    {
        Assert.True(string.IsNullOrEmpty(ApplicationVersion.CandidateName));
        Assert.StartsWith("Cut Assistant Next · 1.0.3 · Build ", ApplicationVersion.DisplayName);
        Assert.DoesNotContain("Entwicklung", ApplicationVersion.DisplayName);
        var metadata = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(ApplicationVersion).Assembly.Location);
        Assert.Equal("Cut Assistant Next", metadata.ProductName);
        Assert.Equal("Cut Assistant Next", metadata.FileDescription);
    }
}
