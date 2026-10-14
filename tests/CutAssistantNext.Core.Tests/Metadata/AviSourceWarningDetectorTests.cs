using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Metadata;

namespace CutAssistantNext.Core.Tests.Metadata;

public sealed class AviSourceWarningDetectorTests
{
    [Theory]
    [InlineData("Film.avi")]
    [InlineData("Film.AVI")]
    [InlineData("Film.avi.mp4")]
    [InlineData("Film.mp4")]
    [InlineData("Film")]
    public void ActualAviContainerWarnsRegardlessOfFileName(string fileName)
    {
        var warning = AviSourceWarningDetector.Detect(fileName, Analysis(" AVI , avi "));

        Assert.NotNull(warning);
        Assert.Contains("bereits im Original", warning);
        Assert.Contains("Bildfehler", warning);
        Assert.Contains("Tonversatz", warning);
        Assert.Contains("Die Bearbeitung bleibt möglich.", warning);
        Assert.Contains("Keyframe-Anzeige und Keyframe-Navigation im Hauptfenster sind für AVI nicht verfügbar", warning);
        Assert.Contains("Die Frame-Lupe bleibt nutzbar.", warning);
    }

    [Theory]
    [InlineData("Film.avi.mp4", ".mp4")]
    [InlineData("Film.AVI.MP4", ".MP4")]
    [InlineData("Film.mkv", ".mkv")]
    public void MisleadingExtensionIsExplained(string fileName, string extension)
    {
        var warning = AviSourceWarningDetector.Detect(fileName, Analysis("avi"));

        Assert.StartsWith($"Die Datei endet auf {extension}, enthält aber einen AVI-Container.", warning);
    }

    [Theory]
    [InlineData("Film.avi.mp4", "mov,mp4,m4a,3gp,3g2,mj2")]
    [InlineData("Film.avi", "mov,mp4")]
    [InlineData("Film.mp4", "matroska,webm")]
    [InlineData("Film.mp4", null)]
    [InlineData("Film.mp4", "avif")]
    [InlineData("Film.mp4", "wav")]
    public void OtherOrUnknownContainersDoNotReceiveAviWarning(string fileName, string? format)
    {
        Assert.Null(AviSourceWarningDetector.Detect(fileName, Analysis(format)));
    }

    private static MediaAnalysisResult Analysis(string? format) =>
        new(format, null, null, TimeSpan.FromMinutes(40), [], []);
}
