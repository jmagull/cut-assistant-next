using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class FfprobeKeyframeArgumentsTests
{
    [Fact]
    public void KeyframeAnalysisScansCompleteFileWithOriginalTimestamps()
    {
        var arguments = FfprobeFrameRunner.BuildKeyframeArguments("file with spaces.mp4", 2).ToArray();
        Assert.Equal("nokey", arguments[Array.IndexOf(arguments, "-skip_frame") + 1]);
        Assert.Contains("-show_frames", arguments); Assert.DoesNotContain("-read_intervals", arguments);
        Assert.Contains("2", arguments); Assert.Equal("file with spaces.mp4", arguments.Last());
        Assert.Contains(arguments, argument => argument.Contains("key_frame"));
    }
    [Fact]
    public async Task MissingToolProducesUnderstandableError()
    {
        var runner = new FfprobeFrameRunner(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe"));
        await Assert.ThrowsAsync<FileNotFoundException>(() => runner.RunKeyframesAsync("video.mp4", 0));
    }
}
