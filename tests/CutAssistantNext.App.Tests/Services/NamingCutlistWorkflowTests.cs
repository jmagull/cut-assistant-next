using CutAssistantNext.App.Naming;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;

namespace CutAssistantNext.App.Tests.Services;

public sealed class NamingCutlistWorkflowTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-naming-workflow-{Guid.NewGuid():N}");
    public NamingCutlistWorkflowTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, recursive: true);

    private const string SeMask = "%Name% %S%%Staffel%%E%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]";
    private const string XMask = "%Name% %Staffel%%x%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]";
    private const string ChicagoFile = "Chicago_Fire__Unantastbar_S13E7_26.10.12_21-00_sf2_45_TVOON_DE.HD.mp4";
    private static NameTemplateContext ChicagoContext => new(Name: "Chicago Fire", Season: "13", Episode: "07",
        EpisodeTitle: "Unantastbar", Year: "2026", Month: "10", Day: "12", OriginalName: ChicagoFile);
    private static MediaAnalysisResult ChicagoAnalysis => new("mp4", "MP4", 100, TimeSpan.FromMinutes(60),
        [new VideoStreamInfo(0, "h264", null, 320, 180, null, null, 25, null)], []);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SavedMaskChangeInSameSessionIsUsedOnlyWhenExplicitlyRegenerating(bool preserve)
    {
        var store = new NamingSettingsStore(Path.Combine(_root, "naming.json"));
        var loader = new NamingSettingsLoader(store, Path.Combine(_root, "legacy.json"));
        store.Save(new() { DefaultNameTemplate = SeMask });
        var oldState = new CutNamingState(loader.Load().DefaultNameTemplate, ChicagoContext);
        if (preserve) oldState = oldState.UseSuggestedMovieName("Fremder Namensvorschlag", true);
        store.Save(new() { DefaultNameTemplate = XMask });
        var vm = CutlistGenerationViewModelFactory.Create(new(), ChicagoFile, ChicagoAnalysis, oldState, loader.Load());

        Assert.Equal(oldState.SuggestedMovieName, vm.SuggestedMovieName);
        Assert.Equal(SeMask, vm.NameTemplate);
        Assert.Equal(SeMask, vm.CreateNamingState().NameTemplate);
        Assert.Equal(oldState.SuggestedMovieName, vm.CreateNamingState().SuggestedMovieName);
        vm.EpisodeTitle = "Unantastbar otr-can II";
        if (preserve) Assert.Equal("Fremder Namensvorschlag", vm.SuggestedMovieName);
        vm.GenerateSuggestedMovieName();

        Assert.Equal("Chicago Fire 13x07 Unantastbar otr-can II [12.10.2026]", vm.SuggestedMovieName);
        Assert.Equal(XMask, vm.NameTemplate);
        Assert.Equal(XMask, vm.CreateNamingState().NameTemplate);
        Assert.False(vm.CreateNamingState().PreserveSuggestedMovieName);
        Assert.Equal("Unantastbar otr-can II", vm.CreateNamingState().NameContext.EpisodeTitle);
        Assert.Equal(SeMask, oldState.NameTemplate);
        Assert.Equal("Unantastbar", oldState.NameContext.EpisodeTitle);
        Assert.Equal(XMask, loader.Load().DefaultNameTemplate);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NewMaskDoesNotReplaceProtectedSuggestionOnFieldChangesOrSave(bool legacy)
    {
        string oldMask = legacy ? "%Name% %Staffel:S%%Folge:E%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]" : SeMask;
        var state = new CutNamingState(oldMask, ChicagoContext).UseSuggestedMovieName("Eigener unveränderter Name", true);
        var vm = CutlistGenerationViewModelFactory.Create(new(), ChicagoFile, ChicagoAnalysis, state,
            new NamingSettings { DefaultNameTemplate = XMask });
        vm.EpisodeTitle = "Bearbeitet";
        vm.SelectedRating = 4;
        var plan = new CutPlan(TimeSpan.FromMinutes(60));
        var document = vm.CreateDocument(plan, ChicagoFile, "0.3.0", ChicagoAnalysis);
        Assert.Equal("Eigener unveränderter Name", document.Info.SuggestedMovieName);
        Assert.Equal(oldMask, vm.CreateNamingState().NameTemplate);
        Assert.True(vm.CreateNamingState().PreserveSuggestedMovieName);
        Assert.Equal("Bearbeitet", vm.EpisodeTitle);
    }

    [Fact]
    public void MultipleSavedMaskChangesUseNewestSettingWithoutRestartOrReloadingMedia()
    {
        var store = new NamingSettingsStore(Path.Combine(_root, "naming.json"));
        var loader = new NamingSettingsLoader(store, Path.Combine(_root, "legacy.json"));
        var state = new CutNamingState(SeMask, ChicagoContext);
        foreach (string mask in new[] { XMask, SeMask, XMask })
        {
            store.Save(new() { DefaultNameTemplate = mask });
            var vm = CutlistGenerationViewModelFactory.Create(new(), ChicagoFile, ChicagoAnalysis, state, loader.Load());
            Assert.Equal(state.SuggestedMovieName, vm.SuggestedMovieName);
            vm.GenerateSuggestedMovieName();
            Assert.Contains(mask == XMask ? "13x07" : "S13E07", vm.SuggestedMovieName);
            state = vm.CreateNamingState();
            Assert.Equal(mask, state.NameTemplate);
        }
    }

    [Fact]
    public void CuttingRegenerationPassesCurrentMaskToFollowingCutlistWithoutReplacingManualOutputEarly()
    {
        var oldState = new CutNamingState(SeMask, ChicagoContext);
        var movie = new CutOutputViewModel(oldState, XMask) { OutputMovieName = "Manueller Filmname" };
        Assert.Equal("Manueller Filmname", movie.OutputMovieName);
        Assert.Equal(SeMask, movie.CreateNamingState().NameTemplate);
        movie.GenerateSuggestedMovieName();
        var state = movie.CreateNamingState();
        Assert.Equal(XMask, state.NameTemplate);
        Assert.Equal("Chicago Fire 13x07 Unantastbar [12.10.2026]", movie.OutputMovieName);
        var cutlist = CutlistGenerationViewModelFactory.Create(new(), ChicagoFile, ChicagoAnalysis, state);
        cutlist.EpisodeTitle = "Unantastbar otr-can II";
        cutlist.GenerateSuggestedMovieName();
        Assert.Equal("Chicago Fire 13x07 Unantastbar otr-can II [12.10.2026]", cutlist.SuggestedMovieName);
    }

    [Fact]
    public void InvalidLatestMaskDoesNotDiscardPreviousNameOrProtectedState()
    {
        var state = new CutNamingState(SeMask, ChicagoContext).UseSuggestedMovieName("Vorlage", true);
        var vm = new CutlistGenerationViewModel(new(), state, defaultNameTemplate: "%not-a-variable%");
        Assert.Throws<ArgumentException>(() => vm.GenerateSuggestedMovieName());
        Assert.Equal("Vorlage", vm.SuggestedMovieName);
        Assert.Equal(SeMask, vm.NameTemplate);
        Assert.True(vm.CreateNamingState().PreserveSuggestedMovieName);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SavingAndReloadingCutlistPreservesCutsWithoutReplacingManualMovieName(bool legacyMask)
    {
        var mask = legacyMask
            ? "%Name% %Staffel:S%%Folge:E%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]"
            : NamingSettings.CreateDefault().DefaultNameTemplate;
        var context = NamingEditorSample.CreateContext();
        var state = new CutNamingState(mask, context);
        var movie = new CutOutputViewModel(state)
        { OutputMovieName = "Ghostbusters - 1x01 - Hexenprobleme [anderes Datum]" };
        var movieName = movie.OutputMovieName;
        var originals = Directory.CreateDirectory(Path.Combine(_root, "originals")).FullName;
        var outputs = Directory.CreateDirectory(Path.Combine(_root, "outputs")).FullName;
        var cutlists = Directory.CreateDirectory(Path.Combine(_root, "cutlists")).FullName;
        var original = Path.Combine(originals, NamingEditorSample.OriginalFileName);
        File.WriteAllText(original, "original");
        var plan = new CutPlan(TimeSpan.FromSeconds(20));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(8)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(15)));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(18), plan.MediaDuration));
        var target = MovieOutputTargetResolver.Resolve(outputs, movie.OutputMovieName, original);
        var direct = CutRequestFactory.Create(original, target.FilePath, plan, 25);
        Assert.Empty(Directory.GetFiles(cutlists)); // Direct cutting needs no saved cutlist.

        var generation = new CutlistGenerationViewModel(new CutlistSettings { DefaultAuthor = "Test" },
            movie.CreateNamingState())
        { SelectedRating = 4 };
        var analysis = new MediaAnalysisResult("mp4", "MP4", 100, plan.MediaDuration,
            [new VideoStreamInfo(0, "h264", null, 320, 180, null, null, 25, null)], []);
        var document = generation.CreateDocument(plan, Path.GetFileName(original), "0.2.1", analysis);
        var cutlistTarget = CutlistOutputTargetResolver.Resolve(cutlists, original);
        CutlistFileWriter.WriteSafely(cutlistTarget.FilePath, document);
        var loaded = CutlistFileReader.Read(cutlistTarget.FilePath);
        var reloaded = CutlistCutPlanBuilder.Build(loaded, plan.MediaDuration);
        var later = CutRequestFactory.Create(original, target.FilePath, reloaded, 25);

        Assert.Equal(direct.KeepSegments.Select(s => (s.Start, s.Duration)), later.KeepSegments.Select(s => (s.Start, s.Duration)));
        Assert.Equal(plan.RemoveSegments.Select(s => (s.Start, s.End)),
            reloaded.RemoveSegments.Select(s => (s.Start, s.End)));
        Assert.Equal(NamingEditorSample.OriginalFileName + ".cutlist", Path.GetFileName(cutlistTarget.FilePath));
        Assert.Equal(NamingEditorSample.OriginalFileName, loaded.General.ApplyToFile);
        Assert.Equal("Ghostbusters S01E01 Hexenprobleme [19.09.2026]", loaded.Info.SuggestedMovieName);
        Assert.Equal(movieName + ".mp4", Path.GetFileName(later.OutputFilePath));
        Assert.Equal(movieName, movie.OutputMovieName);
        Assert.Equal(mask, movie.CreateNamingState().NameTemplate);
        Assert.Equal(state.SuggestedMovieName, movie.CreateNamingState().SuggestedMovieName);
        Assert.Equal("original", File.ReadAllText(original));
        Assert.Single(Directory.GetFiles(cutlists));
        Assert.Empty(Directory.GetFiles(outputs));
        var completion = new CutCompletionViewModel(later.OutputFilePath, reloaded);
        Assert.Equal([5d, 9d], completion.Transitions.Select(p => p.Position.TotalSeconds));
        Assert.Equal(TimeSpan.FromSeconds(12), completion.CheckPoints[^1].Position);
    }
}
