using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.Tests.Serialization;

public sealed class CutlistParserTests
{
    [Fact]
    public void Parse_SerializedDocument_RestoresKeepSegmentsAndMetadata()
    {
        var cuts =
            new[]
            {
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(519.84),
                    TimeSpan.FromSeconds(457.20)),

                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(1459.48),
                    TimeSpan.FromSeconds(2448.76))
            };

        var general =
            new CutlistGeneralMetadata
            {
                Application = "Cut Assistant Next",
                Version = "1.0",
                FramesPerSecond = 25.0,
                DisplayAspectRatio = "16:9",
                IntendedCutApplicationName = "MP4Box",
                IntendedCutApplication = "mp4box.exe",
                IntendedCutApplicationVersion = "26.07",
                IntendedCutApplicationOptions = string.Empty,
                NoOfCuts = cuts.Length,
                ApplyToFile =
                    "Peacemaker Die Kuh muss weg.mp4",
                OriginalFileSizeBytes = 123456789
            };

        var info =
            new CutlistInfoMetadata
            {
                RatingByAuthor = 5,
                Author = "Testautor",
                UserComment = "Referenzschnitt",
                SuggestedMovieName =
                    "Peacemaker Die Kuh muss weg"
            };

        var document =
            new CutlistDocument(
                general,
                cuts,
                info);

        var serialized =
            CutlistSerializer.Serialize(
                document);

        var parsed =
            CutlistParser.Parse(
                serialized);

        Assert.Equal(
            2,
            parsed.Cuts.Count);

        Assert.Equal(
            TimeSpan.FromSeconds(519.84),
            parsed.Cuts[0].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(457.20),
            parsed.Cuts[0].Duration);

        Assert.Equal(
            TimeSpan.FromSeconds(1459.48),
            parsed.Cuts[1].Start);

        Assert.Equal(
            TimeSpan.FromSeconds(2448.76),
            parsed.Cuts[1].Duration);

        Assert.Equal(
            "Peacemaker Die Kuh muss weg.mp4",
            parsed.General.ApplyToFile);

        Assert.Equal(
            "MP4Box",
            parsed.General.IntendedCutApplicationName);

        Assert.Equal(
            "Peacemaker Die Kuh muss weg",
            parsed.Info.SuggestedMovieName);
    }

    [Fact]
    public void Parse_HistoricalVirtualDubCutlist_RestoresRelevantData()
    {
        var content =
            """
            [General]
            Application=Cut_assistant.exe
            Version=0.26.5.6
            comment1=Die folgenden Teile des Films bleiben erhalten, der Rest wird ausgeschnitten. Alle Werte in Sekunden.
            ApplyToFile=Sliders__Fast_ein_Mensch_26.08.08_03-40_tele5_40_TVOON_DE.mpg.HQ.avi
            OriginalFileSizeBytes=359618984
            FramesPerSecond=25
            IntendedCutApplicationName=VirtualDub
            IntendedCutApplication=VirtualDub.exe
            IntendedCutApplicationVersion=1.7.8.0
            VDUseSmartRendering=1
            VDSmartRenderingCodecFourCC=0x34363258
            VDSmartRenderingCodecVersion=0x00000000
            NoOfCuts=1

            [Info]
            RatingByAuthor=5
            Author=joerg
            EPGError=0
            ActualContent=
            MissingBeginning=0
            MissingEnding=0
            MissingVideo=0
            MissingAudio=0
            OtherError=0
            OtherErrorDescription=
            SuggestedMovieName=
            UserComment=

            [Cut0]
            Start=1070.0792084
            Duration=2313.6327916
            """;

        var parsed =
            CutlistParser.Parse(
                content);

        var cut =
            Assert.Single(
                parsed.Cuts);

        Assert.Equal(
            TimeSpan.FromSeconds(1070.0792084),
            cut.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(2313.6327916),
            cut.Duration);

        Assert.Equal(
            "Cut_assistant.exe",
            parsed.General.Application);

        Assert.Equal(
            "0.26.5.6",
            parsed.General.Version);

        Assert.Equal(
            25.0,
            parsed.General.FramesPerSecond);

        Assert.Equal(
            "VirtualDub",
            parsed.General.IntendedCutApplicationName);

        Assert.Equal(
            "VirtualDub.exe",
            parsed.General.IntendedCutApplication);

        Assert.Equal(
            "1.7.8.0",
            parsed.General.IntendedCutApplicationVersion);

        Assert.Equal(
            "Sliders__Fast_ein_Mensch_26.08.08_03-40_tele5_40_TVOON_DE.mpg.HQ.avi",
            parsed.General.ApplyToFile);

        Assert.Equal(
            359618984,
            parsed.General.OriginalFileSizeBytes);

        Assert.Equal(
            "joerg",
            parsed.Info.Author);
    }}
