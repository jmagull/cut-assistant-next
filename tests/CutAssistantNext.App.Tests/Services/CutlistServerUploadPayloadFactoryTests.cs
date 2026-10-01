using System.Text;
using CutAssistantNext.App.Services;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistServerUploadPayloadFactoryTests
{
    [Fact]
    public void CreateBytes_CreatesServerCompatibleUtf8Payload()
    {
        var source =
            new CutlistDocument(
                new CutlistGeneralMetadata
                {
                    Application = "Cut Assistant Next",
                    Version = "1.2.3",
                    FramesPerSecond = 25,
                    DisplayAspectRatio = "16:9",
                    NoOfCuts = 0,
                    ApplyToFile = "Überraschung.avi",
                    OriginalFileSizeBytes = 123456789
                },
                [],
                new CutlistInfoMetadata
                {
                    SuggestedMovieName =
                        "Überraschung für Schüler",
                    UserComment =
                        "Mein eigener Kommentar.",
                    Author =
                        "Joerg",
                    RatingByAuthor = 5
                });

        var bytes =
            CutlistServerUploadPayloadFactory.CreateBytes(
                source);

        Assert.False(
            bytes.Length >= 3 &&
            bytes[0] == 0xEF &&
            bytes[1] == 0xBB &&
            bytes[2] == 0xBF);

        var text =
            new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true)
            .GetString(
                bytes);

        Assert.Contains(
            "Application=Cut Assistant Next\r\n",
            text);

        Assert.DoesNotContain(
            "Application=Cut Assistant\r\n",
            text);

        Assert.Contains(
            "Version=1.2.3\r\n",
            text);

        Assert.Contains(
            "Author=Joerg\r\n",
            text);

        Assert.Contains(
            "UserComment=Mein eigener Kommentar.\r\n",
            text);

        Assert.Contains(
            "ApplyToFile=Überraschung.avi\r\n",
            text);

        Assert.Contains(
            "SuggestedMovieName=Überraschung für Schüler\r\n",
            text);

        Assert.Contains(
            "\r\n[Info]\r\n",
            text);
    }
}