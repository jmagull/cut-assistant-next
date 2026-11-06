using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class OtrCanAudioTrackRecoveryTests
{
    private static readonly AudioStreamInfo Aac = new(1, "aac", null, 48000, 2, "stereo") { Language = "deu" };
    private static readonly AudioStreamInfo Ac3 = new(2, "ac3", null, 48000, 2, "stereo") { Language = "eng" };
    private static MediaAnalysisResult Media(params AudioStreamInfo[] tracks) => new(
        "mp4", "MP4", null, TimeSpan.FromSeconds(2),
        [new(0, "h264", null, 160, 96, null, null, 25, null) { StartTimeSeconds = 0 }], tracks)
    { StartTimeSeconds = 0 };

    [Theory]
    [InlineData("damaged", "healthy", 2, 1, 0.32)]
    [InlineData("healthy", "damaged", 1, 2, 0.32)]
    [InlineData("damaged", "damaged", 0, 0, 0.32)]
    [InlineData("healthy", "healthy", 0, 0, 0.32)]
    [InlineData("gapped", "damaged", 0, 0, 0.32)]
    [InlineData("short", "damaged", 0, 0, 0.32)]
    [InlineData("damaged", "healthy", 0, 0, 0.28)]
    [InlineData("damaged", "partial-block", 0, 0, 0.32)]
    [InlineData("unknown-warning", "healthy", 0, 0, 0.32)]
    [InlineData("healthy", "codec-warning", 0, 0, 0.32)]
    [InlineData("wrong-index", "damaged", 0, 0, 0.32)]
    public async Task RequiresIdentifiedDamageAndFullVerifiedCoverage(string aac, string ac3,
        int retained, int omitted, double end)
    {
        var source = Path.GetTempFileName();
        try
        {
            var request = new CutRequest(source, source + ".mp4", [new(TimeSpan.Zero, TimeSpan.FromSeconds(end))], 25);
            var media = Media(Aac, Ac3);
            var failure = new OtrCanAudioFailureException(media, media,
                new InvalidOperationException("PCM assembly emitted warnings; cut rejected"));
            var runner = new FakeProbe(aac, ac3);
            var service = new OtrCanAudioTrackRecovery(new("engine", "index", "ffmpeg", "ffprobe"),
                _ => throw new InvalidOperationException("No engine may run during preflight."), runner,
                (_, _) => Task.FromResult(media));
            using var offer = await service.AssessAsync(request, failure, null, default);
            Assert.Equal([1, 2], runner.Probed);
            Assert.Equal(retained != 0, offer.IsAvailable);
            if (offer.IsAvailable)
            {
                Assert.Contains($"Ausgeschlossen: Spur {omitted}", offer.Description);
                Assert.Contains($"Erhalten: Spur {retained}", offer.Description);
                Assert.Throws<IOException>(() => File.Open(source, FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
            }
            Assert.False(File.Exists(request.OutputFilePath));
        }
        finally { File.Delete(source); }
    }

    [Fact]
    public async Task SingleTrackNeverStartsAnExclusionProbe()
    {
        var media = Media(Aac);
        var request = new CutRequest(Path.GetFullPath("one.mp4"), Path.GetFullPath("out.mp4"),
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))], 25);
        var probe = new FakeProbe("damaged", "healthy");
        var service = new OtrCanAudioTrackRecovery(new("engine", "index", "ffmpeg", "ffprobe"),
            _ => throw new InvalidOperationException(), probe);
        using var offer = await service.AssessAsync(request,
            new(media, media, new InvalidOperationException("PCM assembly failed: damaged")), null, default);
        Assert.False(offer.IsAvailable);
        Assert.Empty(probe.Probed);
    }

    [Fact]
    public void OnlyExplicitCompatibleRetainedStreamsChangeTheExpectedOutput()
    {
        var media = Media(Aac, Ac3);
        Assert.Same(media, OtrCanCutService.ExpectedOriginal(media, null));
        Assert.Equal(Ac3, Assert.Single(OtrCanCutService.ExpectedVerifiedTracks(media, [2]).AudioStreams));
        Assert.Throws<ArgumentException>(() => OtrCanCutService.ExpectedVerifiedTracks(media, []));
        Assert.Throws<ArgumentException>(() => OtrCanCutService.ExpectedVerifiedTracks(media, [1, 2]));
        Assert.Throws<ArgumentException>(() => OtrCanCutService.ExpectedVerifiedTracks(media, [99]));
    }

    private sealed class FakeProbe(string aac, string ac3) : ICutToolRunner
    {
        internal List<int> Probed { get; } = [];
        public Task RunAsync(string executablePath, IReadOnlyList<string> arguments,
            IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken)
        {
            Assert.Equal("ffprobe", executablePath);
            Assert.Equal("-select_streams", arguments[2]);
            var track = arguments[3] == "1" ? Aac : Ac3;
            Probed.Add(track.Index);
            var variant = track.Index == 1 ? aac : ac3;
            var codec = track.CodecName;
            var size = codec == "aac" ? 1024 : 1536;
            if (variant == "damaged") Report($"[{codec} @ 012ABC] [error] decoder error");
            if (variant == "codec-warning") Report($"[{codec} @ 012ABC] [warning] decoder warning");
            if (variant == "unknown-warning") Report("Unknown warning");
            for (var pts = 0; pts < (variant == "short" ? 10240 : 16384); pts += size)
            {
                if (variant == "gapped" && pts == 4096) continue;
                Report($"frame|stream_index={(variant == "wrong-index" ? 99 : track.Index)}|pts={pts}|nb_samples={(variant == "partial-block" && pts == 15360 ? 1140 : size)}|channels=2|channel_layout=stereo");
            }
            Report($"stream|index={track.Index}|codec_name={codec}|profile=LC|sample_rate=48000|channels=2|channel_layout=stereo|time_base=1/48000|bit_rate=384000|start_pts=0|duration_ts={(variant == "partial-block" ? 16500 : 16896)}");
            return Task.CompletedTask;
            void Report(string message) => progress!.Report(new(CutProgressKind.Output, message));
        }
    }
}
