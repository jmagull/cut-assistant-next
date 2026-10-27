using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class AacEmergencyCutServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-aac-test-" + Guid.NewGuid().ToString("N"));
    public AacEmergencyCutServiceTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);
    private static MediaAnalysisResult Media() => new("mp4", "MP4", 4, TimeSpan.FromSeconds(10),
        [new(0, "h264", null, 160, 96, null, "16:9", 25, null) { StartTimeSeconds = 0 }],
        [new(1, "aac", null, 48000, 2, "stereo") { Language = "deu", StartTimeSeconds = 0 },
         new(2, "ac3", null, 48000, 2, "stereo") { Language = "deu", StartTimeSeconds = 0 }])
    { StartTimeSeconds = 0 };
    private CutRequest Request()
    {
        var source = Path.Combine(_root, "source.mp4");
        File.WriteAllText(source, "data");
        return new(source, Path.Combine(_root, "output.mp4"), [new(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))], 25);
    }
    private static OtrCanAudioFailureException Failure() =>
        new(Media(), Media(), new InvalidOperationException("PCM assembly emitted warnings; cut rejected"));

    private sealed class Engine(Func<CutRequest, CancellationToken, Task> run) : ICutEngine
    {
        internal List<CutRequest> Requests { get; } = [];
        public Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return run(request, cancellationToken);
        }
    }
    private sealed class Preparation : IAacEmergencySource
    {
        internal int Calls;
        public Task<string> PrepareAsync(CutRequest request, AacEmergencyOffer offer, AacEmergencyChoice choice,
            string work, IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            Calls++;
            var p = Path.Combine(work, "prepared.mp4");
            File.WriteAllText(p, "aac-only");
            return Task.FromResult(p);
        }
    }

    [Fact]
    public void MultipleAacChoicesExposeLanguageTitleAndListAllOmittedTracks()
    {
        var media = Media();
        var english = media.AudioStreams[0] with { Index = 3, Language = "eng", Title = "Originalton" };
        media = media with { AudioStreams = [media.AudioStreams[0], media.AudioStreams[1], english] };
        var offer = AacEmergencyOffer.Create(media, media)!;
        Assert.Equal(2, offer.Choices.Count);
        var second = offer.Choices[1];
        Assert.Equal(3, second.Original.Index);
        Assert.Contains("eng", second.Description);
        Assert.Contains("Originalton", second.Description);
        Assert.Contains("Spur 1 (aac, deu", offer.Notice(second));
        Assert.Contains("Spur 2 (ac3, deu", offer.Notice(second));
        Assert.Equal(english, Assert.Single(OtrCanCutService.ExpectedOriginal(media, 3).AudioStreams));
    }

    [Fact]
    public async Task OnlyExplicitChoiceRetriesAndReportsEveryOmittedTrack()
    {
        var request = Request();
        var normal = new Engine((_, _) => throw Failure());
        var selected = new Engine((r, _) =>
        {
            Assert.Equal(request.OriginalFilePath, r.OriginalFilePath);
            Assert.NotEqual(request.SourceFilePath, r.SourceFilePath);
            Assert.Equal("aac-only", File.ReadAllText(r.SourceFilePath));
            File.WriteAllText(r.OutputFilePath, "complete");
            return Task.CompletedTask;
        });
        var preparation = new Preparation();
        var confirmations = 0;
        var service = new OtrCanAacEmergencyCutService(normal, index =>
        {
            Assert.Equal(1, index);
            return selected;
        }, preparation, (offer, _) =>
        {
            confirmations++;
            Assert.Equal(0, preparation.Calls);
            Assert.Contains("deu", offer.Choices.Single().Description);
            return Task.FromResult<AacEmergencyChoice?>(offer.Choices.Single());
        });
        var result = await service.RunWithResultAsync(request);
        Assert.Equal(1, confirmations);
        Assert.Equal(1, preparation.Calls);
        Assert.Single(selected.Requests);
        Assert.Contains("Notfallausgabe", result.Notice);
        Assert.Contains("ac3", result.Notice);
        Assert.Equal("data", File.ReadAllText(request.OriginalFilePath));
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Fact]
    public async Task DecliningDoesNotPrepareOrRetry()
    {
        var preparation = new Preparation();
        var normal = new Engine((_, _) => throw Failure());
        var service = new OtrCanAacEmergencyCutService(normal,
            _ => throw new Exception("must not retry"), preparation,
            (_, _) => Task.FromResult<AacEmergencyChoice?>(null));
        await Assert.ThrowsAsync<OtrCanAudioFailureException>(() => service.RunAsync(Request()));
        Assert.Equal(0, preparation.Calls);
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Fact]
    public async Task CancellationAfterQuestionDoesNotStartPreparation()
    {
        using var cancellation = new CancellationTokenSource();
        var preparation = new Preparation();
        var service = new OtrCanAacEmergencyCutService(new Engine((_, _) => throw Failure()),
            _ => throw new Exception("must not retry"), preparation, (offer, _) =>
            {
                cancellation.Cancel();
                return Task.FromResult<AacEmergencyChoice?>(offer.Choices.Single());
            });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.RunAsync(Request(), cancellationToken: cancellation.Token));
        Assert.Equal(0, preparation.Calls);
    }

    [Fact]
    public async Task UnrelatedFailuresDoNotOfferTrackLoss()
    {
        var service = new OtrCanAacEmergencyCutService(new Engine((_, _) => throw new IOException("video failed")),
            _ => throw new Exception("must not retry"), new Preparation(),
            (_, _) => throw new Exception("must not prompt"));
        await Assert.ThrowsAsync<IOException>(() => service.RunAsync(Request()));
        Assert.False(OtrCanAudioFailureException.IsAudioFailure("Video packet count differs"));
    }

    [Fact]
    public async Task HealthyJobNeverUsesFallback()
    {
        var service = new OtrCanAacEmergencyCutService(new Engine((_, _) => Task.CompletedTask),
            _ => throw new Exception("must not retry"), new Preparation(),
            (_, _) => throw new Exception("must not prompt"));
        await service.RunAsync(Request());
    }

    [Fact]
    public async Task FailedRetryCleansPreparedSourceAndPreservesExistingTarget()
    {
        var request = Request();
        File.WriteAllText(request.OutputFilePath, "existing");
        var service = new OtrCanAacEmergencyCutService(new Engine((_, _) => throw Failure()),
            _ => new Engine((_, _) => throw new IOException("retry failed")), new Preparation(),
            (offer, _) => Task.FromResult<AacEmergencyChoice?>(offer.Choices.Single()));
        await Assert.ThrowsAsync<IOException>(() => service.RunAsync(request));
        Assert.Equal("existing", File.ReadAllText(request.OutputFilePath));
        Assert.Equal("data", File.ReadAllText(request.OriginalFilePath));
        Assert.Empty(Directory.GetDirectories(_root));
    }

    [Fact]
    public void StandardValidationStillRequiresAllTracksAndExplicitSelectionRequiresAac()
    {
        var media = Media();
        var reduced = media with { AudioStreams = [media.AudioStreams[0]] };
        Assert.Throws<InvalidOperationException>(() => OtrCanCutService.ValidateOutput(media, reduced));
        OtrCanCutService.ValidateOutput(OtrCanCutService.ExpectedOriginal(media, 1), reduced);
        Assert.Throws<ArgumentException>(() => OtrCanCutService.ExpectedOriginal(media, 2));
        Assert.Throws<ArgumentException>(() => OtrCanCutService.ExpectedOriginal(media, 99));
        Assert.Null(AacEmergencyOffer.Create(reduced, reduced));
        Assert.Null(AacEmergencyOffer.Create(media with { AudioStreams = [media.AudioStreams[1], media.AudioStreams[1]] }, media));
    }

    private sealed class Runner(bool warning) : ICutToolRunner
    {
        internal List<string[]> Calls { get; } = [];
        public Task RunAsync(string executablePath, IReadOnlyList<string> arguments,
            IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken)
        {
            Calls.Add(arguments.ToArray());
            if (Calls.Count == 1 && warning) progress?.Report(new(CutProgressKind.Output, "decoder warning"));
            if (Calls.Count == 2) File.WriteAllText(arguments[^1], "copy");
            return Task.CompletedTask;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PreparationRequiresCleanAacAndPacketProofBeforeRetry(bool warning)
    {
        var request = Request();
        var offer = AacEmergencyOffer.Create(Media(), Media())!;
        var work = Path.Combine(_root, "prepare");
        Directory.CreateDirectory(work);
        var runner = new Runner(warning);
        var verified = false;
        var service = new AacEmergencySource(new("engine", "index", "ffmpeg", "ffprobe"), runner,
            (_, _) => Task.FromResult(Media() with { AudioStreams = [Media().AudioStreams[0]] }),
            (_, _, map, _) =>
            {
                verified = true;
                Assert.Equal([(0, 0), (1, 1)], map);
                return Task.CompletedTask;
            });
        if (warning)
        {
            await Assert.ThrowsAsync<InvalidDataException>(() => service.PrepareAsync(request, offer, offer.Choices[0], work, null, default));
            Assert.False(verified);
            Assert.Single(runner.Calls);
        }
        else
        {
            await service.PrepareAsync(request, offer, offer.Choices[0], work, null, default);
            Assert.True(verified);
            Assert.Contains("-xerror", runner.Calls[0]);
            Assert.Contains("copy", runner.Calls[1]);
            Assert.Contains("-n", runner.Calls[1]);
            Assert.DoesNotContain("-y", runner.Calls[1]);
        }
    }
}
