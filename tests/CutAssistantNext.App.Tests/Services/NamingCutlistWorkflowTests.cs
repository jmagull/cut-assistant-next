using CutAssistantNext.App.Naming;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;

namespace CutAssistantNext.App.Tests.Services;

public sealed class NamingCutlistWorkflowTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-naming-workflow-{Guid.NewGuid():N}");
    public NamingCutlistWorkflowTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, recursive: true);

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
