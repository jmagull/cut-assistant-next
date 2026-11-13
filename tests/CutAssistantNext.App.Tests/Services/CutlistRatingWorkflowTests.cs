using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistRatingWorkflowTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "can-rating-workflow-" + Guid.NewGuid().ToString("N"));
    private ICutlistRatingStore Store => new CutlistRatingSettingsStore(Path.Combine(_directory, "ratings.json"));
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true); }

    [Fact]
    public async Task SearchAndDownloadDoNotActivateProvenanceUntilActualUse()
    {
        var workflow = new CutlistRatingWorkflow(Store);
        var row = CutlistResultRow.FromServer(new CutlistSearchResult
        { Id = "12345", Author = "Community", CutlistFileName = "Aufnahme.mp4.cutlist" }, CutlistRatingTests.Url);
        Assert.Null(workflow.Current);
        var downloadCount = 0;
        var imported = await CutlistSelectionReader.ReadAsync(row, (url, id, token) =>
        {
            downloadCount++;
            Assert.Equal(CutlistRatingTests.Url, url); Assert.Equal("12345", id);
            return Task.FromResult(LocalCutlistDiscoveryTests.Bytes());
        });
        Assert.Equal(1, downloadCount); Assert.NotNull(imported.ServerSource);
        Assert.Null(workflow.Current);
        Assert.True(await workflow.CanLeaveAsync(_ => throw new Exception("No download prompt"), _ => throw new Exception("No send")));
        var plan = new CutPlanViewModel();
        plan.LoadCutPlan(CutlistCutPlanBuilder.Build(imported.Document, TimeSpan.FromSeconds(100)));
        workflow.Use(imported.ServerSource, "Aufnahme.mp4", "Ich", imported.Document.Info.Author);
        Assert.Equal("Aufnahme.mp4", workflow.Current!.MediaPath);
        Assert.Equal(12345, workflow.Current.Source.CutlistId);
        Assert.Same(imported.ServerSource, workflow.Current.Source);
    }

    [Theory]
    [InlineData("Community", "community", "Community")]
    [InlineData("Community", "  COMMUNITY  ", "Community")]
    [InlineData("Anderer Name", "Ich", "Ich")]
    [InlineData("", "Ich", "")]
    public async Task OwnAuthorsAndUnknownAuthorsAreExcluded(string serverAuthor, string ownAuthor, string documentAuthor)
    {
        var workflow = new CutlistRatingWorkflow(Store);
        workflow.Use(CutlistRatingTests.Source(author: serverAuthor), "Film.mp4", ownAuthor, documentAuthor);
        Assert.Null(workflow.Current);
        Assert.True(await workflow.CanLeaveAsync(_ => throw new Exception("No prompt"), _ => throw new Exception("No rate")));
    }

    [Fact]
    public async Task PurelyLocalImportNeverGainsServerOriginFromFilename()
    {
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, "Aufnahme.HQ.mp4.cutlist");
        await File.WriteAllBytesAsync(path, LocalCutlistDiscoveryTests.Bytes());
        var row = CutlistResultRow.FromLocal(new LocalCutlistCandidate(path, await File.ReadAllBytesAsync(path)),
            LocalCutlistDiscoveryTests.Context(path[..^8]));
        var imported = await CutlistSelectionReader.ReadAsync(row, (_, _, _) => throw new Exception("No download"));
        Assert.Null(imported.ServerSource);
        var workflow = new CutlistRatingWorkflow(Store);
        workflow.Use(imported.ServerSource, "Film.mp4", "Ich", "Community");
        Assert.Null(workflow.Current);
    }

    [Theory]
    [InlineData("Skip", true, false)]
    [InlineData("Cancel", false, false)]
    [InlineData("Rate", true, true)]
    public async Task DepartureUsesExplicitDecisionWithoutDiscardingContext(string decision, bool canLeave, bool shouldRate)
    {
        var workflow = Used();
        var original = workflow.Current;
        var rateCount = 0;
        var questionCount = 0;
        var result = await workflow.CanLeaveAsync(context =>
        {
            questionCount++; Assert.Same(original, context);
            return Task.FromResult(Enum.Parse<CutlistRatingDecision>(decision));
        }, context =>
        {
            rateCount++; Assert.Same(original, context); return Task.FromResult(true);
        });
        Assert.Equal(canLeave, result); Assert.Equal(1, questionCount);
        Assert.Equal(shouldRate ? 1 : 0, rateCount);
        Assert.Same(original, workflow.Current); // The caller commits replacement only after all confirmations.
    }

    [Fact]
    public async Task CancelingOrFailingRatingKeepsCurrentFilmAndOrigin()
    {
        var workflow = Used();
        var original = workflow.Current;
        Assert.False(await workflow.CanLeaveAsync(_ => Task.FromResult(CutlistRatingDecision.Rate), _ => Task.FromResult(false)));
        Assert.Same(original, workflow.Current);
        Assert.Equal("Original.mp4", workflow.Current!.MediaPath);
    }

    [Fact]
    public async Task SameServerAccountAndIdDoesNotPromptButOtherIdDoes()
    {
        var workflow = Used();
        Assert.True(await workflow.CanLeaveAsync(_ => throw new Exception("No prompt"), _ => throw new Exception("No rate"), CutlistRatingTests.Source()));
        var calls = 0;
        Assert.True(await workflow.CanLeaveAsync(_ => { calls++; return Task.FromResult(CutlistRatingDecision.Skip); },
            _ => throw new Exception("No rate"), CutlistRatingTests.Source("999")));
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData("Accepted")]
    [InlineData("Attempted")]
    [InlineData("Unknown")]
    [InlineData("Rejected")]
    public async Task PersistedAttemptsNeverAutomaticallyOfferOrResendAfterRestart(string state)
    {
        var source = CutlistRatingTests.Source();
        Assert.True(Store.TryBegin(source.Key));
        Store.Complete(source.Key, Enum.Parse<CutlistRatingState>(state));
        var restarted = Used();
        Assert.True(await restarted.CanLeaveAsync(_ => throw new Exception("No question"), _ => throw new Exception("No send")));
    }

    [Fact]
    public async Task OriginalAccountIsKeptWhenSettingsChange()
    {
        var workflow = Used();
        var newAccount = CutlistRatingTests.Source(url: CutlistRatingTests.Url.Replace(CutlistRatingTests.Fred, new string('b', 64)));
        Assert.True(await workflow.CanLeaveAsync(_ => Task.FromResult(CutlistRatingDecision.Rate), context =>
        {
            Assert.Equal(CutlistRatingTests.Fred, context.Source.Fred);
            Assert.NotEqual(context.Source.Key, newAccount.Key);
            return Task.FromResult(true);
        }, newAccount));
    }

    [Fact]
    public void ClearAndLocalReplacementRemoveActiveCandidate()
    {
        var workflow = Used();
        workflow.Clear(); Assert.Null(workflow.Current);
        workflow.Use(CutlistRatingTests.Source(), "Original.mp4", "Ich", "Community");
        workflow.Use(null, "Original.mp4", "Ich", "Community");
        Assert.Null(workflow.Current);
    }

    private CutlistRatingWorkflow Used()
    {
        var workflow = new CutlistRatingWorkflow(Store);
        workflow.Use(CutlistRatingTests.Source(), "Original.mp4", "Ich", "Community");
        return workflow;
    }
}
