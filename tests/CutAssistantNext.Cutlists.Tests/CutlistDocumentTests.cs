using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Tests;

public sealed class CutlistDocumentTests
{
    [Fact]
    public void Create_WithGeneralCutsAndInfo_PreservesDocumentParts()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(100),
            VideoStreams: [],
            AudioStreams: []);

        var cuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10)),
            new CutlistKeepSegment(
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(80))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: "Mit Cut Assistant Next geschnitten.",
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        Assert.Same(
            general,
            document.General);

        Assert.Equal(
            cuts,
            document.Cuts);

        Assert.Same(
            info,
            document.Info);

        Assert.Equal(
            document.Cuts.Count,
            document.General.NoOfCuts);
    }

    [Fact]
    public void Constructor_WithMismatchingNumberOfCuts_ThrowsArgumentException()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(100),
            VideoStreams: [],
            AudioStreams: []);

        var generalCuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10))
        };

        var actualCuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10)),
            new CutlistKeepSegment(
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(80))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: generalCuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: []);

        Assert.Throws<ArgumentException>(
            () => new CutlistDocument(
                general,
                actualCuts,
                info));
    }
}
