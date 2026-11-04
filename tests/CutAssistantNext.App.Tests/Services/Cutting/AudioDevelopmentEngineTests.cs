using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class AudioDevelopmentEngineTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Spotlight-Entwicklung")]
    [InlineData("RC1")]
    public void EveryOtherBuildKeepsTheConfiguredMotor(string? candidate)
    {
        var settings = new OtrCanSettings { ExecutablePath = @"C:\configured\otr_can.exe" };
        Assert.Same(settings, AudioDevelopmentEngine.Resolve(settings, Path.GetTempPath(), candidate));
    }
    [Fact]
    public void PrivateBuildUsesOnlyItsOwnMotorAndPreservesSavedChoiceAndIndexer()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-review-engine-" + Guid.NewGuid().ToString("N"));
        var path = Path.Combine(root, "runtime", "otr-can", "otr_can.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        try
        {
            File.WriteAllText(path, "review engine");
            var settings = new OtrCanSettings { ExecutablePath = @"C:\installed\otr_can.exe", FfmsIndexExecutablePath = @"C:\tools\indexer.exe" };
            var result = AudioDevelopmentEngine.Resolve(settings, root, "Audio-Diagnose-Entwicklung");
            Assert.Equal(path, result.ExecutablePath); Assert.Equal(settings.FfmsIndexExecutablePath, result.FfmsIndexExecutablePath);
            Assert.Equal(@"C:\installed\otr_can.exe", settings.ExecutablePath);
        }
        finally { Directory.Delete(root, true); }
    }
    [Fact]
    public void IncompleteReviewPackageNeverSilentlyUsesTheInstalledMotor() =>
        Assert.Throws<FileNotFoundException>(() => AudioDevelopmentEngine.Resolve(new(), Path.GetTempPath(), "Audio-Diagnose-Entwicklung"));
}
