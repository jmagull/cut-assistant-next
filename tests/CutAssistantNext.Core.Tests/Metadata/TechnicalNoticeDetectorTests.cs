using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Metadata;

namespace CutAssistantNext.Core.Tests.Metadata;

public sealed class TechnicalNoticeDetectorTests
{
    [Fact]
    public void Detect_WithAviExtensionAndMp4Container_ReturnsContainerMismatchNotice()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: null,
            Duration: null,
            VideoStreams: [],
            AudioStreams: []);

        var notices = TechnicalNoticeDetector.Detect(
            "irgendein-film.avi",
            analysis);

        var notice = Assert.Single(notices);

        Assert.Equal(
            "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.",
            notice.Description);
    }

    [Fact]
    public void Detect_WithMp4ExtensionAndMp4Container_ReturnsNoNotice()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: null,
            Duration: null,
            VideoStreams: [],
            AudioStreams: []);

        var notices = TechnicalNoticeDetector.Detect(
            "irgendein-film.mp4",
            analysis);

        Assert.Empty(notices);
    }

    [Fact]
    public void Detect_WithAviExtensionAndAviContainer_ReturnsNoNotice()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "avi",
            FormatLongName: "AVI (Audio Video Interleaved)",
            FileSizeBytes: null,
            Duration: null,
            VideoStreams: [],
            AudioStreams: []);

        var notices = TechnicalNoticeDetector.Detect(
            "irgendein-film.avi",
            analysis);

        Assert.Empty(notices);
    }

    [Fact]
    public void Detect_WithUppercaseAviExtensionAndMp4Container_ReturnsNotice()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: null,
            Duration: null,
            VideoStreams: [],
            AudioStreams: []);

        var notices = TechnicalNoticeDetector.Detect(
            "IRGENDEIN-FILM.AVI",
            analysis);

        var notice = Assert.Single(notices);

        Assert.Equal(
            "Dateiendung .avi, tatsächlich erkannter Container: MP4/ISO-BMFF.",
            notice.Description);
    }
}
