using System.Text;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Tests.IO;

public sealed class CutlistFileWriterTests
{
    [Fact]
    public void Write_SavesUtf8WithoutBomAndPreservesUmlauts()
    {
        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CutAssistantNext.Tests",
            Guid.NewGuid().ToString("N"));

        var filePath = Path.Combine(
            tempDirectory,
            "Tatort.cutlist");

        try
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
                userComment: "Werbung vollst\u00E4ndig entfernt.",
                technicalNotices: [],
                author: "joerg");

            var document = new CutlistDocument(
                general,
                cuts,
                info);

            CutlistFileWriter.Write(
                filePath,
                document);

            var bytes =
                File.ReadAllBytes(filePath);

            Assert.False(
                bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF);

            var text =
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true)
                .GetString(bytes);

            Assert.Contains(
                "Werbung vollst\u00E4ndig entfernt.",
                text);

            Assert.Contains(
                "\r\n[Info]\r\n",
                text);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true);
            }
        }
    }
}
