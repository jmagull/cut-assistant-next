using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxCommandLineFormatterTests
{
    [Fact]
    public void Format_WithSplitArguments_ReturnsReadableCommandLine()
    {
        var arguments =
            new[]
            {
                @"C:\Video Dateien\Sliders.mp4",
                "-splitx",
                "1762.44:2977.52",
                "-out",
                @"C:\Video Dateien\segment.mp4"
            };

        var result =
            Mp4BoxCommandLineFormatter.Format(
                @"C:\Program Files\GPAC\MP4Box.exe",
                arguments);

        Assert.Equal(
            "\"C:\\Program Files\\GPAC\\MP4Box.exe\" " +
            "\"C:\\Video Dateien\\Sliders.mp4\" " +
            "-splitx 1762.44:2977.52 -out " +
            "\"C:\\Video Dateien\\segment.mp4\"",
            result);
    }
}
