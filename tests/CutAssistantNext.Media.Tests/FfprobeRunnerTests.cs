using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests;

public class FfprobeRunnerTests
{
    [Fact]
    public async Task RunAsync_RejectsEmptyMediaFilePath()
    {
        var runner = new FfprobeRunner(
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}-ffprobe.exe"));

        await Assert.ThrowsAsync<ArgumentException>(
            () => runner.RunAsync(" "));
    }

    [Fact]
    public async Task RunAsync_ThrowsWhenFfprobeExecutableIsMissing()
    {
        var missingFfprobePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}-ffprobe.exe");

        var runner = new FfprobeRunner(missingFfprobePath);

        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => runner.RunAsync("example.mp4"));

        Assert.Equal(missingFfprobePath, exception.FileName);
    }

    [Fact]
    public async Task RunAsync_ThrowsWhenMediaFileIsMissing()
    {
        var existingExecutablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var missingMediaPath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}.mp4");

        var runner = new FfprobeRunner(existingExecutablePath);

        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => runner.RunAsync(missingMediaPath));

        Assert.Equal(
            Path.GetFullPath(missingMediaPath),
            exception.FileName);
    }
    [Fact]
    public void Constructor_RejectsEmptyFfprobePath()
    {
        Assert.Throws<ArgumentException>(
            () => new FfprobeRunner(" "));
    }
}
