using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class AudioTrackCopyServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-track-copy-" + Guid.NewGuid().ToString("N"));
    public AudioTrackCopyServiceTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);

    private static MediaAnalysisResult Media() => new("mp4", "MP4", 4, TimeSpan.FromSeconds(10),
        [new(0, "h264", null, 160, 96, null, "16:9", 25, null) { StartTimeSeconds = 0 }],
        [new(1, "aac", null, 48000, 2, "stereo") { Language = "deu", StartTimeSeconds = 0 },
         new(2, "ac3", null, 48000, 2, "stereo") { Language = "eng", Title = "Originalton", StartTimeSeconds = 0 }])
    { StartTimeSeconds = 0 };
    private (string Source, string Target) Paths()
    {
        var source = Path.Combine(_root, "finished-part1.mp4");
        File.WriteAllText(source, "data");
        return (source, Path.Combine(_root, "part1-selected.mp4"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task SavesWholeVerifiedCopyOfExplicitAacOrAc3AndPreservesOriginal(int index)
    {
        var (source, output) = Paths();
        var media = Media();
        var choice = AudioTrackCopyChoice.Choices(media).Single(s => s.Stream.Index == index);
        var service = new AudioTrackCopyService((s, original, selected, work, _, _) =>
        {
            Assert.Equal(source, s);
            Assert.Equal(media, original);
            Assert.Equal(index, selected.Index);
            var path = Path.Combine(work, "copy.mp4");
            File.WriteAllText(path, "verified all packets");
            return Task.FromResult(path);
        });
        var result = await service.RunAsync(source, output, media, choice);
        Assert.Equal(output, result.OutputFilePath);
        Assert.Contains("Entfallene Tonspuren:", result.Notice);
        Assert.Contains(index == 1 ? "AC3" : "AAC", result.Notice);
        Assert.Equal("data", File.ReadAllText(source));
        Assert.Equal("verified all packets", File.ReadAllText(output));
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Theory]
    [InlineData("same")]
    [InlineData("existing")]
    [InlineData("extension")]
    [InlineData("selection")]
    public async Task InvalidTargetsOrUnofferedTrackNeverStartPreparation(string scenario)
    {
        var (source, output) = Paths();
        var media = Media();
        var choice = AudioTrackCopyChoice.Choices(media)[0];
        if (scenario == "same") output = source;
        if (scenario == "existing") File.WriteAllText(output, "previous");
        if (scenario == "extension") output += ".avi";
        if (scenario == "selection") choice = choice with { Stream = choice.Stream with { Index = 99 } };
        var called = false;
        var service = new AudioTrackCopyService((_, _, _, _, _, _) =>
        {
            called = true;
            throw new Exception("Must not prepare");
        });
        await Assert.ThrowsAnyAsync<Exception>(() => service.RunAsync(source, output, media, choice));
        Assert.Equal("data", File.ReadAllText(source));
        Assert.False(called);
        if (scenario == "existing") Assert.Equal("previous", File.ReadAllText(output));
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Theory]
    [InlineData("failure")]
    [InlineData("cancellation")]
    [InlineData("target-race")]
    public async Task FailureCancellationAndNewTargetRaceCleanWorkAndPreserveFiles(string scenario)
    {
        var (source, output) = Paths();
        var media = Media();
        using var cancellation = new CancellationTokenSource();
        var service = new AudioTrackCopyService((_, _, _, work, _, _) =>
        {
            var path = Path.Combine(work, "copy.mp4");
            File.WriteAllText(path, "pending");
            if (scenario == "failure") throw new InvalidDataException("Packet verification failed");
            if (scenario == "cancellation") cancellation.Cancel();
            if (scenario == "target-race") File.WriteAllText(output, "created by someone else");
            return Task.FromResult(path);
        });
        await Assert.ThrowsAnyAsync<Exception>(() =>
            service.RunAsync(source, output, media, AudioTrackCopyChoice.Choices(media)[0], token: cancellation.Token));
        Assert.Equal("data", File.ReadAllText(source));
        if (scenario == "target-race") Assert.Equal("created by someone else", File.ReadAllText(output));
        else Assert.False(File.Exists(output));
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Fact]
    public void ChoicesExposeLanguageTitleAndRejectUnsupportedStructures()
    {
        var media = Media();
        var choices = AudioTrackCopyChoice.Choices(media);
        Assert.Equal(2, choices.Count);
        Assert.Contains("Deutsch", choices[0].Description);
        Assert.Contains("Englisch", choices[1].Description);
        Assert.Contains("Originalton", choices[1].Description);
        Assert.Empty(AudioTrackCopyChoice.Choices(media with { FormatName = "avi" }));
        Assert.Empty(AudioTrackCopyChoice.Choices(media with { VideoStreams = [media.VideoStreams[0], media.VideoStreams[0]] }));
        Assert.Empty(AudioTrackCopyChoice.Choices(media with { AudioStreams = [media.AudioStreams[0]] }));
        Assert.Empty(AudioTrackCopyChoice.Choices(media with
        {
            AudioStreams = media.AudioStreams.Select(s => s with { CodecName = "mp3" }).ToArray()
        }));
    }

    private sealed class Runner(int warningStage) : ICutToolRunner
    {
        internal List<string[]> Calls { get; } = [];
        public Task RunAsync(string path, IReadOnlyList<string> arguments,
            IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            Calls.Add(arguments.ToArray());
            if (Calls.Count == warningStage) progress?.Report(new(CutProgressKind.Output, "decoder warning"));
            if ((warningStage == 5 && Calls.Count == 2) || (warningStage == 6 && Calls.Count == 1))
                progress?.Report(new(CutProgressKind.Output, "[mp4 @ 0000015abc] track 1: codec frame size is not set"));
            if (Calls.Count == 2) File.WriteAllText(arguments[^1], "copy");
            return Task.CompletedTask;
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task SelectedSourceRequiresCleanDecodeCopyMetadataAndPacketProof(int failureStage)
    {
        var (source, output) = Paths();
        var media = Media();
        var selected = media.AudioStreams[1];
        var runner = new Runner(failureStage is 1 or 2 or 5 or 6 ? failureStage : -1);
        var verified = false;
        var helper = new SelectedAudioSource("ffmpeg", "ffprobe", runner,
            (_, _) => Task.FromResult(media with
            {
                AudioStreams = [selected with { Index = 1, Language = failureStage == 3 ? "deu" : "eng" }]
            }),
            (_, _, map, _) =>
            {
                verified = true;
                Assert.Equal([(0, 0), (2, 1)], map);
                return failureStage == 4 ? Task.FromException(new InvalidDataException("Changed packet")) : Task.CompletedTask;
            });
        var service = new AudioTrackCopyService((s, m, a, work, progress, token) =>
            helper.PrepareAsync(s, m, a, work, "copy.mp4", progress, token));
        var task = service.RunAsync(source, output, media, new(selected));
        if (failureStage is 0 or 5)
        {
            await task;
            Assert.True(verified);
            Assert.Equal("copy", File.ReadAllText(output));
            Assert.Contains("-xerror", runner.Calls[0]);
            Assert.Contains("0:2", runner.Calls[0]);
            Assert.Contains("copy", runner.Calls[1]);
            Assert.Contains("-copyts", runner.Calls[1]);
            Assert.Contains("-n", runner.Calls[1]);
        }
        else
        {
            await Assert.ThrowsAnyAsync<Exception>(() => task);
            Assert.Equal(failureStage == 4, verified);
            Assert.False(File.Exists(output));
        }
        Assert.Empty(Directory.GetDirectories(_root));
        Assert.Equal("data", File.ReadAllText(source));
    }
}
