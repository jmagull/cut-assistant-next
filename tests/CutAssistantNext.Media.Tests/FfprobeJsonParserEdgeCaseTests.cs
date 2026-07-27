using System.Text.Json;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests;

public class FfprobeJsonParserEdgeCaseTests
{
    [Fact]
    public void Parse_AllowsMissingOptionalInformation()
    {
        const string json = """
        {
          "streams": [],
          "format": {}
        }
        """;

        var result = FfprobeJsonParser.Parse(json);

        Assert.Null(result.FormatName);
        Assert.Null(result.FormatLongName);
        Assert.Null(result.FileSizeBytes);
        Assert.Null(result.Duration);
        Assert.Empty(result.VideoStreams);
        Assert.Empty(result.AudioStreams);
    }

    [Fact]
    public void Parse_ThrowsJsonExceptionForInvalidJson()
    {
        const string json = "{";

        Assert.ThrowsAny<JsonException>(
            () => FfprobeJsonParser.Parse(json));
    }
}
