using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class DirectMovieOutputTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-direct-movie-{Guid.NewGuid():N}");

    public DirectMovieOutputTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Theory]
    [InlineData(false, "new")]
    [InlineData(true, "new")]
    [InlineData(false, "refused")]
    [InlineData(true, "refused")]
    [InlineData(false, "confirmed")]
    [InlineData(true, "confirmed")]
    [InlineData(false, "late")]
    [InlineData(true, "late")]
    public async Task BothEnginesHonorExactDestinationAndOverwriteDecision(bool useOtrCan, string scenario)
    {
        var outputDirectory = Directory.CreateDirectory(Path.Combine(_root, "Ausgaben")).FullName;
        var original = Path.Combine(_root, "original.HQ.mp4");
        File.WriteAllText(original, "original");
        const string name = "Ghosts - 4x01 - Patience [22.09.2026]";
        var outputPath = Path.Combine(outputDirectory, name + ".mp4");
        if (scenario is "refused" or "confirmed")
        {
            File.WriteAllText(outputPath, "existing");
        }

        var target = MovieOutputTargetResolver.Resolve(outputDirectory, name, original);
        var plan = new CutPlan(TimeSpan.FromSeconds(30));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20)));
        var request = CutRequestFactory.Create(original, target.FilePath, plan, 25,
            overwriteExistingOutput: target.Exists && scenario == "confirmed");
        int workCalls = 0;
        void DuringWork()
        {
            workCalls++;
            if (scenario == "late")
            {
                File.WriteAllText(outputPath, "late existing");
            }
            else if (scenario == "confirmed")
            {
                Assert.Equal("existing", File.ReadAllText(outputPath));
            }
        }

        ICutEngine engine = !useOtrCan
            ? new Mp4BoxCutService(new MovieRunner(DuringWork))
            : new OtrCanCutService(
                new OtrCanToolPaths(Tool("engine.exe"), Tool("index.exe"), Tool("ffmpeg.exe"), Tool("ffprobe.exe")),
                new NativeRunner(DuringWork),
                (_, _) => Task.FromResult(new MediaAnalysisResult("mp4", "mp4", 100, TimeSpan.FromSeconds(30),
                    [new VideoStreamInfo(0, "h264", null, 320, 180, null, null, 25, null)],
                    [new AudioStreamInfo(1, "aac", null, 48000, 2, "stereo")])));

        if (scenario is "refused" or "late")
        {
            await Assert.ThrowsAsync<IOException>(() => engine.RunAsync(request));
            Assert.Equal(scenario == "late" ? "late existing" : "existing", File.ReadAllText(outputPath));
            Assert.Equal(scenario == "refused", workCalls == 0);
        }
        else
        {
            await engine.RunAsync(request);
            Assert.Equal("complete", File.ReadAllText(outputPath));
            Assert.True(workCalls > 0);
        }

        Assert.Equal(outputPath, request.OutputFilePath);
        Assert.Equal("original", File.ReadAllText(original));
        Assert.Single(Directory.GetFileSystemEntries(outputDirectory));
        Assert.Equal(TimeSpan.Zero, request.KeepSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(20), request.KeepSegments[1].Start);
    }

    private string Tool(string name)
    {
        var path = Path.Combine(_root, name);
        File.WriteAllText(path, "tool");
        return path;
    }

    private sealed class MovieRunner(Action duringWork) : IMp4BoxRunner
    {
        public Task RunSplitAsync(string sourceFilePath, string outputFilePath, Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
            duringWork();
            File.WriteAllText(outputFilePath, "complete");
            return Task.CompletedTask;
        }

        public Task RunConcatAsync(IReadOnlyList<string> segmentFilePaths, string outputFilePath,
            CancellationToken cancellationToken = default)
        {
            File.WriteAllText(outputFilePath, "complete");
            return Task.CompletedTask;
        }
    }

    private sealed class NativeRunner(Action duringWork) : ICutToolRunner
    {
        public Task RunAsync(string path, IReadOnlyList<string> arguments,
            IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            var args = arguments.ToArray();
            if (args[0] == "-c")
            {
                foreach (var suffix in new[] { "", "_track00.tc.txt", "_track00.kf.txt" })
                {
                    File.WriteAllText(args[3] + suffix, suffix == "_track00.tc.txt" ? "# timecode format v2\n0.00\n40.00\n80.00\n" : "index");
                }
            }
            else
            {
                duringWork();
                File.WriteAllText(args[Array.IndexOf(args, "--output") + 1], "complete");
            }

            return Task.CompletedTask;
        }
    }
}
