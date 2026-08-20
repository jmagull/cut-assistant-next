using System.Text;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;

namespace CutAssistantNext.Cutlists.Tests.Integration;

public sealed class CutlistImportWorkflowTests
{
    [Fact]
    public void ReadAndBuild_HistoricalCutlist_CreatesExpectedCutPlan()
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
            NoOfCuts=2
            ApplyToFile=Sliders.avi
            OriginalFileSizeBytes=359618984

            [Cut0]
            Start=10
            Duration=10

            [Cut1]
            Start=30
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
            Encoding.RegisterProvider(
                CodePagesEncodingProvider.Instance);

            var windows1252 =
                Encoding.GetEncoding(
                    1252);

            File.WriteAllText(
                filePath,
                content,
                windows1252);

            var document =
                CutlistFileReader.Read(
                    filePath);

            var cutPlan =
                CutlistCutPlanBuilder.Build(
                    document,
                    TimeSpan.FromSeconds(100));

            Assert.Equal(
                "Sliders.avi",
                document.General.ApplyToFile);

            Assert.Equal(
                "VirtualDub",
                document.General.IntendedCutApplicationName);

            Assert.Equal(
                "cut-ä-gut",
                document.Info.Author);

            Assert.Equal(
                3,
                cutPlan.RemoveSegments.Count);

            Assert.Equal(
                TimeSpan.Zero,
                cutPlan.RemoveSegments[0].Start);

            Assert.Equal(
                TimeSpan.FromSeconds(10),
                cutPlan.RemoveSegments[0].End);

            Assert.Equal(
                TimeSpan.FromSeconds(20),
                cutPlan.RemoveSegments[1].Start);

            Assert.Equal(
                TimeSpan.FromSeconds(30),
                cutPlan.RemoveSegments[1].End);

            Assert.Equal(
                TimeSpan.FromSeconds(50),
                cutPlan.RemoveSegments[2].Start);

            Assert.Equal(
                TimeSpan.FromSeconds(100),
                cutPlan.RemoveSegments[2].End);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
