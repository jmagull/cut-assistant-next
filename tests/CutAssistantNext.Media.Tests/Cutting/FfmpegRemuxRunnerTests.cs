using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class FfmpegRemuxRunnerTests
{
    [Fact]
    public void ArgumentsPreserveAllStreamsAndNeverEncodeOrOverwrite()
    {
        var arguments = FfmpegRemuxRunner.BuildArguments("C:\\video files\\input.avi", "C:\\temp files\\output.mp4");
        Assert.Equal(new[] { "-hide_banner", "-nostdin", "-n", "-i", "C:\\video files\\input.avi",
            "-map", "0", "-c", "copy", "-f", "mp4", "C:\\temp files\\output.mp4" }, arguments);
    }

    [Fact]
    public async Task MissingExecutableFailsClearly()
    {
        var runner = new FfmpegRemuxRunner(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe"));
        await Assert.ThrowsAsync<FileNotFoundException>(() => runner.RunAsync("input.avi", "output.mp4", null, default));
    }

    [Fact]
    public async Task CancelledRequestDoesNotLaunchProcess()
    {
        var runner = new FfmpegRemuxRunner(Environment.ProcessPath!);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            runner.RunAsync("input.avi", "output.mp4", null, new CancellationToken(true)));
    }
}
