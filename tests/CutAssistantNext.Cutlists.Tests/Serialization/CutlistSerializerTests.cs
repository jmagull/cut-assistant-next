using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.Tests.Serialization;

public sealed class CutlistSerializerTests
{
    [Fact]
    public void Serialize_WritesKeepSegmentsAsCutSections()
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
                TimeSpan.FromSeconds(10.5)),
            new CutlistKeepSegment(
                TimeSpan.FromSeconds(20.25),
                TimeSpan.FromSeconds(79.75))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "NoOfCuts=2",
            result);

        Assert.Contains(
            "[Cut0]",
            result);

        Assert.Contains(
            "Start=0",
            result);

        Assert.Contains(
            "Duration=10.5",
            result);

        Assert.Contains(
            "[Cut1]",
            result);

        Assert.Contains(
            "Start=20.25",
            result);

        Assert.Contains(
            "Duration=79.75",
            result);
    }

    [Fact]
    public void Serialize_WithGermanCulture_UsesInvariantDecimalSeparator()
    {
        var originalCulture =
            System.Globalization.CultureInfo.CurrentCulture;

        try
        {
            System.Globalization.CultureInfo.CurrentCulture =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");

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
                    TimeSpan.FromSeconds(10.5))
            };

            var general = CutlistGeneralMetadata.Create(
                applyToFile: "Tatort.avi",
                applicationVersion: "1.0.0",
                analysis: analysis,
                keepSegments: cuts);

            var info = CutlistInfoMetadata.Create(
                suggestedMovieName: "Tatort [13.08.2026]",
                userComment: null,
                technicalNotices: []);

            var document = new CutlistDocument(
                general,
                cuts,
                info);

            var result =
                CutlistSerializer.Serialize(document);

            Assert.Contains(
                "Duration=10.5",
                result);

            Assert.DoesNotContain(
                "Duration=10,5",
                result);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture =
                originalCulture;
        }
    }

    [Fact]
    public void Serialize_WritesGeneralMetadata()
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
                TimeSpan.FromSeconds(100))
        };

        var cutApplication = new CutApplicationInfo(
            Name: "MP4Box",
            Executable: "mp4box.exe",
            Version: "",
            Options: "");

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            intendedCutApplication: cutApplication,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "Application=Cut Assistant Next",
            result);

        Assert.Contains(
            "Version=1.0.0",
            result);

        Assert.Contains(
            "ApplyToFile=Tatort.avi",
            result);

        Assert.Contains(
            "OriginalFileSizeBytes=734003200",
            result);

        Assert.Contains(
            "IntendedCutApplicationName=MP4Box",
            result);

        Assert.Contains(
            "IntendedCutApplication=mp4box.exe",
            result);

        Assert.Contains(
            "IntendedCutApplicationVersion=",
            result);

        Assert.Contains(
            "IntendedCutApplicationOptions=",
            result);

        Assert.Contains(
            "NoOfCuts=1",
            result);
    }

    [Fact]
    public void Serialize_WritesFrameRateAndDisplayAspectRatio()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(100),
            VideoStreams:
            [
                new VideoStreamInfo(
                    Index: 0,
                    CodecName: "h264",
                    CodecLongName: "H.264",
                    Width: 1920,
                    Height: 1080,
                    SampleAspectRatio: "1:1",
                    DisplayAspectRatio: "16:9",
                    FramesPerSecond: 29.97,
                    FieldOrder: "progressive")
            ],
            AudioStreams: []);

        var cuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(100))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "FramesPerSecond=29.97",
            result);

        Assert.Contains(
            "DisplayAspectRatio=16:9",
            result);
    }

    [Fact]
    public void Serialize_WritesInfoMetadata()
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
                TimeSpan.FromSeconds(100))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: "Mit Cut Assistant Next geschnitten.",
            technicalNotices: [],
            author: "joerg");

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "[Info]",
            result);

        Assert.Contains(
            "RatingByAuthor=5",
            result);

        Assert.Contains(
            "Author=joerg",
            result);

        Assert.Contains(
            "UserComment=Mit Cut Assistant Next geschnitten.",
            result);

        Assert.Contains(
            "EPGError=0",
            result);

        Assert.Contains(
            "ActualContent=",
            result);

        Assert.Contains(
            "MissingBeginning=0",
            result);

        Assert.Contains(
            "MissingEnding=0",
            result);

        Assert.Contains(
            "MissingVideo=0",
            result);

        Assert.Contains(
            "MissingAudio=0",
            result);

        Assert.Contains(
            "OtherError=0",
            result);

        Assert.Contains(
            "OtherErrorDescription=",
            result);

        Assert.Contains(
            "SuggestedMovieName=Tatort [13.08.2026]",
            result);
    }

    [Fact]
    public void Serialize_WithAviExtensionAndMp4Container_WritesTechnicalNotice()
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
                TimeSpan.FromSeconds(100))
        };

        var notices =
            CutAssistantNext.Core.Metadata.TechnicalNoticeDetector.Detect(
                "Tatort.avi",
                analysis);

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: notices,
            author: "joerg");

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "OtherError=1",
            result);

        Assert.Contains(
            "OtherErrorDescription=Dateiendung .avi, tats\u00E4chlich erkannter Container: MP4/ISO-BMFF.",
            result);

    }
    [Fact]
    public void Serialize_WritesClassicCompatibilityComment()
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
                TimeSpan.FromSeconds(100))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: "Tatort [13.08.2026]",
            userComment: null,
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "comment1=The following parts of the movie will be kept, the rest will be cut out.",
            result);

        Assert.Contains(
            "comment2=All values are given in seconds.",
            result);
    }

    [Fact]
    public void Serialize_PreservesTimeSpanPrecisionWithoutFloatingPointArtifacts()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(1000),
            VideoStreams: [],
            AudioStreams: []);

        var cuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.FromTicks(3597803334),
                TimeSpan.FromTicks(10812323333))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Test.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: null,
            userComment: null,
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "Start=359.7803334",
            result);

        Assert.Contains(
            "Duration=1081.2323333",
            result);
    }

    [Fact]
    public void Serialize_LongTimeSpan_DoesNotWriteFloatingPointArtifacts()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromTicks(100000000000000),
            VideoStreams: [],
            AudioStreams: []);

        var cuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.FromTicks(93110739675520),
                TimeSpan.FromSeconds(1))
        };

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Test.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName: null,
            userComment: null,
            technicalNotices: []);

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        Assert.Contains(
            "Start=9311073.967552",
            result);

        Assert.DoesNotContain(
            "Start=9311073.9675520007",
            result);
    }

    [Fact]
    public void Serialize_CompleteDocument_MatchesExpectedCutlistFormat()
    {
        var analysis = new MediaAnalysisResult(
            FormatName: "mov,mp4,m4a,3gp,3g2,mj2",
            FormatLongName: "QuickTime / MOV",
            FileSizeBytes: 734003200,
            Duration: TimeSpan.FromSeconds(100),
            VideoStreams:
            [
                new VideoStreamInfo(
                    Index: 0,
                    CodecName: "h264",
                    CodecLongName: "H.264",
                    Width: 1920,
                    Height: 1080,
                    SampleAspectRatio: "1:1",
                    DisplayAspectRatio: "16:9",
                    FramesPerSecond: 25.0,
                    FieldOrder: "progressive")
            ],
            AudioStreams: []);

        var cuts = new[]
        {
            new CutlistKeepSegment(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(42.5)),
            new CutlistKeepSegment(
                TimeSpan.FromSeconds(55.25),
                TimeSpan.FromSeconds(44.75))
        };

        var notices =
            CutAssistantNext.Core.Metadata.TechnicalNoticeDetector.Detect(
                "Tatort_Test.avi",
                analysis);

        var cutApplication = new CutApplicationInfo(
            Name: "MP4Box",
            Executable: "mp4box.exe",
            Version: "26.07",
            Options: "");

        var general = CutlistGeneralMetadata.Create(
            applyToFile: "Tatort_Test.avi",
            applicationVersion: "1.0.0",
            analysis: analysis,
            intendedCutApplication: cutApplication,
            keepSegments: cuts);

        var info = CutlistInfoMetadata.Create(
            suggestedMovieName:
                "Tatort S2026E01 - Testfolge [13.08.2026]",
            userComment:
                "Mit Cut Assistant Next geschnitten, Werbung vollst\u00E4ndig entfernt.",
            technicalNotices: notices,
            author: "joerg");

        var document = new CutlistDocument(
            general,
            cuts,
            info);

        var result =
            CutlistSerializer.Serialize(document);

        var expected = string.Join(
            Environment.NewLine,
            new[]
            {
                "[General]",
                "Application=Cut Assistant Next",
                "Version=1.0.0",
                "comment1=The following parts of the movie will be kept, the rest will be cut out.",
                "comment2=All values are given in seconds.",
                "FramesPerSecond=25",
                "DisplayAspectRatio=16:9",
                "ApplyToFile=Tatort_Test.avi",
                "OriginalFileSizeBytes=734003200",
                "IntendedCutApplicationName=MP4Box",
                "IntendedCutApplication=mp4box.exe",
                "IntendedCutApplicationVersion=26.07",
                "IntendedCutApplicationOptions=",
                "NoOfCuts=2",
                "",
                "[Cut0]",
                "Start=0",
                "Duration=42.5",
                "",
                "[Cut1]",
                "Start=55.25",
                "Duration=44.75",
                "",
                "[Info]",
                "RatingByAuthor=5",
                "Author=joerg",
                "UserComment=Mit Cut Assistant Next geschnitten, Werbung vollst\u00E4ndig entfernt.",
                "EPGError=0",
                "ActualContent=",
                "MissingBeginning=0",
                "MissingEnding=0",
                "MissingVideo=0",
                "MissingAudio=0",
                "OtherError=1",
                "OtherErrorDescription=Dateiendung .avi, tats\u00E4chlich erkannter Container: MP4/ISO-BMFF.",
                "SuggestedMovieName=Tatort S2026E01 - Testfolge [13.08.2026]",
                ""
            });

        Assert.Equal(
            expected,
            result);
    }
}
