using System.Text;
using CutAssistantNext.Cutlists.IO;

namespace CutAssistantNext.Cutlists.Tests.IO;

public sealed class CutlistFileReaderTests
{
    [Fact]
    public void Read_ParsesCutlistFromFile()
    {
        var filePath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.cutlist");

        const string content =
            """
            [General]
            Application=Cut_assistant.exe
            Version=0.26.5.6
            FramesPerSecond=25
            IntendedCutApplicationName=VirtualDub
            IntendedCutApplication=VirtualDub.exe
            IntendedCutApplicationVersion=1.7.8.0
            IntendedCutApplicationOptions=
            NoOfCuts=1
            ApplyToFile=Sliders.avi
            OriginalFileSizeBytes=359618984

            [Cut0]
            Start=1070.0792084
            Duration=2313.6327916

            [Info]
            RatingByAuthor=5
            Author=joerg
            UserComment=
            EPGError=0
            ActualContent=
            MissingBeginning=0
            MissingEnding=0
            MissingVideo=0
            MissingAudio=0
            OtherError=0
            OtherErrorDescription=
            SuggestedMovieName=
            """;

        try
        {
            File.WriteAllText(
                filePath,
                content,
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false));

            var document =
                CutlistFileReader.Read(
                    filePath);

            var cut =
                Assert.Single(
                    document.Cuts);

            Assert.Equal(
                TimeSpan.FromSeconds(1070.0792084),
                cut.Start);

            Assert.Equal(
                TimeSpan.FromSeconds(2313.6327916),
                cut.Duration);

            Assert.Equal(
                "VirtualDub",
                document.General.IntendedCutApplicationName);

            Assert.Equal(
                "Sliders.avi",
                document.General.ApplyToFile);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void Read_Windows1252Cutlist_PreservesUmlauts()
    {
        var filePath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.cutlist");

        const string content =
            """
            [General]
            Application=Cut_assistant.exe
            Version=0.26.5.6
            FramesPerSecond=25
            IntendedCutApplicationName=VirtualDub
            IntendedCutApplication=VirtualDub.exe
            IntendedCutApplicationVersion=1.7.8.0
            IntendedCutApplicationOptions=
            NoOfCuts=1
            ApplyToFile=Beispiel.avi
            OriginalFileSizeBytes=123456

            [Cut0]
            Start=10
            Duration=20

            [Info]
            RatingByAuthor=5
            Author=cut-ä-gut
            UserComment=Historische Grüße
            EPGError=0
            ActualContent=
            MissingBeginning=0
            MissingEnding=0
            MissingVideo=0
            MissingAudio=0
            OtherError=0
            OtherErrorDescription=
            SuggestedMovieName=
            """;

        try
        {
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);

            var windows1252 =
                System.Text.Encoding.GetEncoding(
                    1252);

            File.WriteAllText(
                filePath,
                content,
                windows1252);

            var document =
                CutlistFileReader.Read(
                    filePath);

            Assert.Equal(
                "cut-ä-gut",
                document.Info.Author);

            Assert.Equal(
                "Historische Grüße",
                document.Info.UserComment);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }}
