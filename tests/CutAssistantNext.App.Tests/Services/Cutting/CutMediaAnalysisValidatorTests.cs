using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class CutMediaAnalysisValidatorTests
{
    [Fact]
    public void GetFramesPerSecond_ReturnsFirstVideoStreamValue()
    {
        var analysis =
            CreateAnalysis(
                new VideoStreamInfo(
                    Index: 0,
                    CodecName: "h264",
                    CodecLongName: null,
                    Width: 1920,
                    Height: 1080,
                    SampleAspectRatio: null,
                    DisplayAspectRatio: null,
                    FramesPerSecond: 25.0,
                    FieldOrder: null));

        var framesPerSecond =
            CutMediaAnalysisValidator.GetFramesPerSecond(
                analysis);

        Assert.Equal(
            25.0,
            framesPerSecond);
    }

    [Fact]
    public void GetFramesPerSecond_RejectsMissingVideoStream()
    {
        var analysis =
            CreateAnalysis();

        var exception =
            Assert.Throws<InvalidOperationException>(
                () => CutMediaAnalysisValidator.GetFramesPerSecond(
                    analysis));

        Assert.Contains(
            "Videostream",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0.0)]
    [InlineData(-25.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void GetFramesPerSecond_RejectsInvalidFrameRate(
        double? framesPerSecond)
    {
        var analysis =
            CreateAnalysis(
                new VideoStreamInfo(
                    Index: 0,
                    CodecName: "h264",
                    CodecLongName: null,
                    Width: 1920,
                    Height: 1080,
                    SampleAspectRatio: null,
                    DisplayAspectRatio: null,
                    FramesPerSecond: framesPerSecond,
                    FieldOrder: null));

        var exception =
            Assert.Throws<InvalidOperationException>(
                () => CutMediaAnalysisValidator.GetFramesPerSecond(
                    analysis));

        Assert.Contains(
            "Bildrate",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    private static MediaAnalysisResult CreateAnalysis(
        params VideoStreamInfo[] videoStreams)
    {
        return new MediaAnalysisResult(
            FormatName: null,
            FormatLongName: null,
            FileSizeBytes: null,
            Duration: null,
            VideoStreams: videoStreams,
            AudioStreams: []);
    }
}
