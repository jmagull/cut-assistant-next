using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxRunnerConcatTests
{
    [Fact]
    public async Task RunConcatAsync_RejectsEmptySegmentList()
    {
        var executablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var runner =
            new Mp4BoxRunner(
                executablePath);

        await Assert.ThrowsAsync<ArgumentException>(
            () =>
                runner.RunConcatAsync(
                    Array.Empty<string>(),
                    "output.mp4"));
    }

    [Fact]
    public async Task RunConcatAsync_ThrowsWhenSegmentIsMissing()
    {
        var executablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var missingSegmentPath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.mp4");

        var runner =
            new Mp4BoxRunner(
                executablePath);

        var exception =
            await Assert.ThrowsAsync<FileNotFoundException>(
                () =>
                    runner.RunConcatAsync(
                        new[] { missingSegmentPath },
                        "output.mp4"));

        Assert.Equal(
            Path.GetFullPath(missingSegmentPath),
            exception.FileName);
    }

    [Fact]
    public async Task RunConcatAsync_RejectsOutputThatMatchesSegment()
    {
        var executablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "Der Pfad des aktuellen Testprozesses ist nicht verfügbar.");

        var segmentPath =
            Path.GetTempFileName();

        try
        {
            var runner =
                new Mp4BoxRunner(
                    executablePath);

            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    runner.RunConcatAsync(
                        new[] { segmentPath },
                        segmentPath));
        }
        finally
        {
            File.Delete(segmentPath);
        }
    }
}
