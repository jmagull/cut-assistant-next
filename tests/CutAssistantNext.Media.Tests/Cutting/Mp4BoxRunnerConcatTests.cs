using CutAssistantNext.Core.Cutting;
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

    [Fact]
    public async Task RunConcatAsync_ImportsFirstSegmentThenAppendsInOrderToNewOutput()
    {
        var executable = Environment.ProcessPath ?? throw new InvalidOperationException("Testprozess fehlt.");
        var directory = Directory.CreateTempSubdirectory("can-concat-command-");
        var segments = new[] { "first part.mp4", "second part.mp4", "third part.mp4" }
            .Select(name => Path.Combine(directory.FullName, name)).ToArray();
        var output = Path.Combine(directory.FullName, "new output.mp4");

        try
        {
            foreach (var segment in segments)
                File.WriteAllBytes(segment, []);

            var runner = new Mp4BoxRunner(executable, progress: new CaptureCommand());
            var command = await Assert.ThrowsAsync<CommandCaptured>(
                () => runner.RunConcatAsync(segments, output));

            Assert.Equal(Mp4BoxCommandLineFormatter.Format(executable,
                ["-add", segments[0], "-cat", segments[1], "-cat", segments[2], "-new", output]),
                command.Message);
        }
        finally
        {
            foreach (var segment in segments)
                File.Delete(segment);
            directory.Delete();
        }
    }

    private sealed class CaptureCommand : IProgress<CutProgressUpdate>
    {
        public void Report(CutProgressUpdate value) => throw new CommandCaptured(value.Message[2..]);
    }

    private sealed class CommandCaptured(string command) : Exception(command);
}
