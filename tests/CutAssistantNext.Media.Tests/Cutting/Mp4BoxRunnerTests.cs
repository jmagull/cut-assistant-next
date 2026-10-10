using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxRunnerTests
{
    [Fact]
    public void Constructor_RejectsEmptyExecutablePath()
    {
        Assert.Throws<ArgumentException>(
            () =>
                new Mp4BoxRunner(" "));
    }

    [Fact]
    public async Task RunSplitAsync_ThrowsWhenExecutableIsMissing()
    {
        var missingExecutablePath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}-mp4box.exe");

        var runner =
            new Mp4BoxRunner(
                missingExecutablePath);

        var exception =
            await Assert.ThrowsAsync<FileNotFoundException>(
                () =>
                    runner.RunSplitAsync(
                        "source.mp4",
                        "output.mp4",
                        CreateRange()));

        Assert.Equal(
            Path.GetFullPath(missingExecutablePath),
            exception.FileName);
    }

    [Fact]
    public async Task RunSplitAsync_ThrowsWhenSourceFileIsMissing()
    {
        var executablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var missingSourcePath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.mp4");

        var runner =
            new Mp4BoxRunner(
                executablePath);

        var exception =
            await Assert.ThrowsAsync<FileNotFoundException>(
                () =>
                    runner.RunSplitAsync(
                        missingSourcePath,
                        "output.mp4",
                        CreateRange()));

        Assert.Equal(
            Path.GetFullPath(missingSourcePath),
            exception.FileName);
    }

    [Fact]
    public async Task RunSplitAsync_RejectsIdenticalSourceAndOutput()
    {
        var executablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var sourcePath =
            Path.GetTempFileName();

        try
        {
            var runner =
                new Mp4BoxRunner(
                    executablePath);

            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    runner.RunSplitAsync(
                        sourcePath,
                        sourcePath,
                        CreateRange()));
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    [Fact]
    public void Constructor_WithProgress_AcceptsProgressReporter()
    {
        var executablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var progress =
            new Progress<CutProgressUpdate>();

        var runner =
            new Mp4BoxRunner(
                executablePath,
                progress: progress);

        Assert.NotNull(
            runner);
    }
    private static Mp4BoxSplitRange CreateRange()
    {
        return new Mp4BoxSplitRange(
            TimeSpan.Zero,
            TimeSpan.FromSeconds(10));
    }
}
