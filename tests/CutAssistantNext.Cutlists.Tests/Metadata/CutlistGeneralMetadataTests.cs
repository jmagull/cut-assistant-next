using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;

namespace CutAssistantNext.Cutlists.Tests.Metadata;

public sealed class CutlistGeneralMetadataTests
{
    [Fact]
    public void Create_WithMediaAnalysis_PreservesGeneralValues()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(3740.18),
            VideoStreams:
            [
                new VideoStreamInfo(
                    Index: 0,
                    CodecName: "h264",
                    CodecLongName: "H.264",
                    Width: 720,
                    Height: 576,
                    SampleAspectRatio: "64:45",
                    DisplayAspectRatio: "16:9",
                    FramesPerSecond: 25.0,
                    FieldOrder: "progressive")
            ],
            AudioStreams: []);

        var metadata = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis);

        Assert.Equal(
            "Cut Assistant Next",
            metadata.Application);

        Assert.Equal(
            "1.0.0",
            metadata.Version);

        Assert.Equal(
            25.0,
            metadata.FramesPerSecond);

        Assert.Equal(
            "16:9",
            metadata.DisplayAspectRatio);

        Assert.Equal(
            "Tatort.avi",
            metadata.ApplyToFile);

        Assert.Equal(
            734003200,
            metadata.OriginalFileSizeBytes);
    }

    [Fact]
    public void Create_WithoutVideoStream_LeavesVideoValuesEmpty()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(3740.18),
            VideoStreams: [],
            AudioStreams: []);

        var metadata = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis);

        Assert.Null(metadata.FramesPerSecond);
        Assert.Null(metadata.DisplayAspectRatio);
    }

    [Fact]
    public void Create_WithCutApplication_PreservesValues()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(3740.18),
            VideoStreams: [],
            AudioStreams: []);

        var cutApplication = new CutApplicationInfo(
            Name: "MP4Box",
            Executable: "mp4box.exe",
            Version: "",
            Options: "");

        var metadata = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            intendedCutApplication: cutApplication);

        Assert.Equal(
            "MP4Box",
            metadata.IntendedCutApplicationName);

        Assert.Equal(
            "mp4box.exe",
            metadata.IntendedCutApplication);

        Assert.Equal(
            string.Empty,
            metadata.IntendedCutApplicationVersion);

        Assert.Equal(
            string.Empty,
            metadata.IntendedCutApplicationOptions);
    }

    [Fact]
    public void Create_WithKeepSegments_SetsNumberOfCuts()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(100),
            VideoStreams: [],
            AudioStreams: []);

        var keepSegments = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10)),
            new CutlistKeepSegment(
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(20)),
            new CutlistKeepSegment(
                TimeSpan.FromSeconds(50),
                TimeSpan.FromSeconds(50))
        };

        var metadata = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: keepSegments);

        Assert.Equal(
            3,
            metadata.NoOfCuts);
    }
}
