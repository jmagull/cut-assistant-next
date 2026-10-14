using CutAssistantNext.App.Services;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services;

public sealed class MovieSeriesNamingWorkflowTests : IDisposable
{
    private const string Mask = "%Name% - %S%%Staffel%%E%%Folge% - %Folgentitel%";
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-movie-series-{Guid.NewGuid():N}");

    public MovieSeriesNamingWorkflowTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, false)]
    [InlineData(false, false, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public async Task SavedCutlistAndBothEnginesUseTheSameCleanAutomaticName(bool useOtrCan, bool series, bool date)
    {
        string mask = Mask + (date ? " [%Tag%.%Monat%.%YYYY%]" : "");
        var context = new NameTemplateContext(Name: series ? "Kimi" : "Ghostbusters", Season: series ? "01" : null,
            Episode: series ? "03" : null, EpisodeTitle: series ? "Der Besuch" : null,
            Year: "2026", Month: "10", Day: "14");
        string expected = (series ? "Kimi - S01E03 - Der Besuch" : "Ghostbusters") + (series && date ? " [14.10.2026]" : "");
        var store = new NamingSettingsStore(Path.Combine(_root, "naming.json"));
        store.Save(new() { DefaultNameTemplate = mask });
        var storedMask = File.ReadAllBytes(Path.Combine(_root, "naming.json"));
        var state = new CutNamingState(store.Load()!.DefaultNameTemplate, context);
        var movie = new CutOutputViewModel(state);
        var generation = new CutlistGenerationViewModel(new() { DefaultAuthor = "Test" }, state) { SelectedRating = 4 };
        var (original, plan, outputDirectory, cutlistDirectory) = PrepareFiles();
        var target = MovieOutputTargetResolver.Resolve(outputDirectory, movie.OutputMovieName, original);
        var request = CutRequestFactory.Create(original, target.FilePath, plan, 25);
        var document = generation.CreateDocument(plan, Path.GetFileName(original), "0.3.0", Analysis());
        var cutlistTarget = CutlistOutputTargetResolver.Resolve(cutlistDirectory, original);
        CutlistFileWriter.WriteSafely(cutlistTarget.FilePath, document);
        var loaded = CutlistFileReader.Read(cutlistTarget.FilePath);

        await CreateEngine(useOtrCan).RunAsync(request);

        Assert.Equal(expected, movie.OutputMovieName);
        Assert.Equal(expected, loaded.Info.SuggestedMovieName);
        Assert.Equal(expected + ".mp4", Path.GetFileName(request.OutputFilePath));
        Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        Assert.Equal(Path.GetFileName(original) + ".cutlist", Path.GetFileName(cutlistTarget.FilePath));
        Assert.Equal(Path.GetFileName(original), loaded.General.ApplyToFile);
        var reloadedPlan = CutlistCutPlanBuilder.Build(loaded, plan.MediaDuration);
        Assert.Equal(plan.RemoveSegments.Select(s => (s.Start, s.End)), reloadedPlan.RemoveSegments.Select(s => (s.Start, s.End)));
        Assert.Equal(mask, movie.CreateNamingState().NameTemplate);
        Assert.Equal(mask, generation.CreateNamingState().NameTemplate);
        Assert.Equal(storedMask, File.ReadAllBytes(Path.Combine(_root, "naming.json")));
        Assert.Equal("original", File.ReadAllText(original));
        Assert.Single(Directory.GetFiles(outputDirectory));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task ProtectedCutlistAndManualMovieNamesArePreservedByBothEngines(bool useOtrCan, bool legacy)
    {
        string mask = legacy ? "%Name% - %Staffel:S%%Folge:E% - %Folgentitel%" : Mask;
        var state = new CutNamingState(mask, new(Name: "Ghostbusters"))
            .UseSuggestedMovieName("Fremder Filmname - -", preserveSuggestedMovieName: true);
        var movie = new CutOutputViewModel(state) { OutputMovieName = "Mein Filmname - -.mp4" };
        movie.Name = "Bearbeiteter Titel";
        movie.Season = "";
        var generation = new CutlistGenerationViewModel(new() { DefaultAuthor = "Test" }, state) { SelectedRating = 4 };
        generation.Name = "Bearbeiteter Titel";
        var (original, plan, outputDirectory, cutlistDirectory) = PrepareFiles();
        var target = MovieOutputTargetResolver.Resolve(outputDirectory, movie.OutputMovieName, original);
        var request = CutRequestFactory.Create(original, target.FilePath, plan, 25);
        var cutlistTarget = CutlistOutputTargetResolver.Resolve(cutlistDirectory, original);
        CutlistFileWriter.WriteSafely(cutlistTarget.FilePath,
            generation.CreateDocument(plan, Path.GetFileName(original), "0.3.0", Analysis()));

        await CreateEngine(useOtrCan).RunAsync(request);

        Assert.Equal("Mein Filmname - -.mp4", Path.GetFileName(request.OutputFilePath));
        Assert.Equal("Mein Filmname - -.mp4", movie.OutputMovieName);
        Assert.Equal("Fremder Filmname - -", CutlistFileReader.Read(cutlistTarget.FilePath).Info.SuggestedMovieName);
        Assert.True(generation.CreateNamingState().PreserveSuggestedMovieName);
        Assert.Equal(mask, generation.CreateNamingState().NameTemplate);
        Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        Assert.Equal("original", File.ReadAllText(original));
    }

    [Fact]
    public void ClearingAllSeriesInputsUpdatesCutlistButKeepsManualOutputUntilExplicitRegeneration()
    {
        var state = new CutNamingState(Mask, new(Name: "Kimi", Season: "01", Episode: "03", EpisodeTitle: "Der Besuch"));
        var generation = new CutlistGenerationViewModel(new(), state);
        var movie = new CutOutputViewModel(state) { OutputMovieName = "Manuell - -" };
        generation.Season = movie.Season = null;
        generation.Episode = movie.Episode = null;
        generation.EpisodeTitle = movie.EpisodeTitle = null;
        Assert.Equal("Kimi", generation.SuggestedMovieName);
        Assert.Equal("Manuell - -", movie.OutputMovieName);
        movie.GenerateSuggestedMovieName();
        Assert.Equal("Kimi", movie.OutputMovieName);
        Assert.Equal(Mask, movie.NameTemplate);
    }

    [Fact]
    public void ProtectedOldMovieSuggestionIsCleanedOnlyByExplicitRegeneration()
    {
        var state = new CutNamingState(Mask, new(Name: "Ghostbusters"))
            .UseSuggestedMovieName("Ghostbusters - -", preserveSuggestedMovieName: true);
        var generation = new CutlistGenerationViewModel(new(), state);
        Assert.Equal("Ghostbusters - -", generation.SuggestedMovieName);
        generation.GenerateSuggestedMovieName();
        Assert.Equal("Ghostbusters", generation.SuggestedMovieName);
        Assert.False(generation.CreateNamingState().PreserveSuggestedMovieName);
        Assert.Equal(Mask, generation.NameTemplate);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SavedScreenshotMaskAndExplicitRegenerationUseOnlyTheMovieTitleForBothEngines(bool useOtrCan)
    {
        const string mask = "%Name% %S%%Staffel%%E%%Folge%-%Folgentitel: %- [%Tag%.%Monat%.%YYYY%]";
        var (original, plan, outputs, cutlists) = PrepareFiles("Die_Hart_2_26.10.12_20-15_ard_120_TVOON_DE.HQ.mp4");
        var store = new NamingSettingsStore(Path.Combine(_root, "screenshot-mask.json"));
        store.Save(new() { DefaultNameTemplate = mask });
        var savedSettings = store.Load()!;
        var savedBytes = File.ReadAllBytes(Path.Combine(_root, "screenshot-mask.json"));
        var context = NameTemplateContextFactory.Create(Path.GetFileName(original));
        var oldState = new CutNamingState(mask, context).UseSuggestedMovieName("Die Hart 2- [12.10.2026]");
        var generation = CutlistGenerationViewModelFactory.Create(new() { DefaultAuthor = "Test" },
            original, Analysis(), oldState, savedSettings);
        Assert.Equal("Die Hart 2- [12.10.2026]", generation.SuggestedMovieName);
        generation.GenerateSuggestedMovieName();
        generation.SelectedRating = 4;
        Assert.Equal("Die Hart 2", generation.SuggestedMovieName);
        var fresh = CutlistGenerationViewModelFactory.Create(new(), savedSettings, original, Analysis());
        Assert.Equal("Die Hart 2", fresh.SuggestedMovieName);
        var movie = new CutOutputViewModel(generation.CreateNamingState(), savedSettings.DefaultNameTemplate);
        var target = MovieOutputTargetResolver.Resolve(outputs, movie.OutputMovieName, original);
        var request = CutRequestFactory.Create(original, target.FilePath, plan, 25);
        var cutlistTarget = CutlistOutputTargetResolver.Resolve(cutlists, original);
        CutlistFileWriter.WriteSafely(cutlistTarget.FilePath,
            generation.CreateDocument(plan, Path.GetFileName(original), "0.3.0", Analysis()));

        await CreateEngine(useOtrCan).RunAsync(request);

        Assert.Equal("Die Hart 2.mp4", Path.GetFileName(request.OutputFilePath));
        Assert.Equal("Die Hart 2", CutlistFileReader.Read(cutlistTarget.FilePath).Info.SuggestedMovieName);
        Assert.Equal(mask, generation.NameTemplate);
        Assert.Equal(savedBytes, File.ReadAllBytes(Path.Combine(_root, "screenshot-mask.json")));
        Assert.Equal("complete", File.ReadAllText(request.OutputFilePath));
        Assert.Equal("original", File.ReadAllText(original));
    }

    private (string Original, CutPlan Plan, string OutputDirectory, string CutlistDirectory) PrepareFiles(string originalFileName = "Ghostbusters_26.10.14_20-15_ard_120_TVOON_DE.HQ.mp4")
    {
        var original = Path.Combine(_root, originalFileName);
        File.WriteAllText(original, "original");
        var output = Directory.CreateDirectory(Path.Combine(_root, "output")).FullName;
        var cutlists = Directory.CreateDirectory(Path.Combine(_root, "cutlists")).FullName;
        var plan = new CutPlan(TimeSpan.FromSeconds(30));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20)));
        return (original, plan, output, cutlists);
    }

    private static MediaAnalysisResult Analysis() => new("mp4", "MP4", 100, TimeSpan.FromSeconds(30),
        [new VideoStreamInfo(0, "h264", null, 320, 180, null, null, 25, null)],
        [new AudioStreamInfo(1, "aac", null, 48000, 2, "stereo")]);

    private ICutEngine CreateEngine(bool useOtrCan) => useOtrCan
        ? new OtrCanCutService(new(Tool("engine.exe"), Tool("index.exe"), Tool("ffmpeg.exe"), Tool("ffprobe.exe")),
            new NativeRunner(), (_, _) => Task.FromResult(Analysis()))
        : new Mp4BoxCutService(new MovieRunner());

    private string Tool(string name)
    {
        var path = Path.Combine(_root, name);
        File.WriteAllText(path, "tool");
        return path;
    }

    private sealed class MovieRunner : IMp4BoxRunner
    {
        public Task RunSplitAsync(string sourceFilePath, string outputFilePath, Mp4BoxSplitRange range,
            CancellationToken cancellationToken = default)
        {
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

    private sealed class NativeRunner : ICutToolRunner
    {
        public Task RunAsync(string path, IReadOnlyList<string> arguments, IProgress<CutProgressUpdate>? progress,
            CancellationToken token)
        {
            var args = arguments.ToArray();
            if (args[0] == "-c")
                foreach (var suffix in new[] { "", "_track00.tc.txt", "_track00.kf.txt" })
                    File.WriteAllText(args[3] + suffix, "index");
            else File.WriteAllText(args[Array.IndexOf(args, "--output") + 1], "complete");
            return Task.CompletedTask;
        }
    }
}
