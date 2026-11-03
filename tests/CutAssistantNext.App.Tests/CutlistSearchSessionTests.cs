using CutAssistantNext.App.Services;
using CutAssistantNext.App.Tests.Services;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests;

public sealed class CutlistSearchSessionTests
{
    private static CutlistResultRow Server(string id = "10") => CutlistResultRow.FromServer(
        new CutlistSearchResult { Id = id, CutlistFileName = "Aufnahme.HQ.mp4.cutlist", Rating = "4.5" }, "https://example.invalid/");
    private static CutlistResultRow Local() => CutlistResultRow.FromLocal(
        new LocalCutlistCandidate(Path.Combine(Path.GetTempPath(), "Aufnahme.HQ.mp4.cutlist"), LocalCutlistDiscoveryTests.Bytes()),
        LocalCutlistDiscoveryTests.Context(Path.Combine(Path.GetTempPath(), "Aufnahme.HQ.mp4")));

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EitherSourceCanArriveFirstWithoutSelectionOrSuppression(bool serverFirst)
    {
        var slow = new TaskCompletionSource<CutlistSourceResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = serverFirst ? Server() : Local();
        var second = serverFirst ? Local() : Server();
        var model = new CutlistSearchResultsViewModel("Aufnahme.HQ.mp4");
        var work = model.SearchAsync(_ => serverFirst ? Task.FromResult(new CutlistSourceResult([first], "1 Treffer")) : slow.Task,
            _ => serverFirst ? slow.Task : Task.FromResult(new CutlistSourceResult([first], "1 Treffer")), CancellationToken.None);
        Assert.True(await model.Ready);
        Assert.Same(first, Assert.Single(model.Results));
        Assert.Null(model.SelectedResult);
        model.SelectedResult = first;
        Assert.False(model.CanLoad);
        slow.SetResult(new([second], "1 Treffer"));
        await work;
        Assert.Equal(2, model.Results.Count);
        Assert.Same(first, model.SelectedResult);
        Assert.True(model.CanLoad);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OneFailureDoesNotSuppressTheOtherSource(bool serverFails)
    {
        var model = new CutlistSearchResultsViewModel("Film.mp4");
        Task<CutlistSourceResult> Failure() => Task.FromException<CutlistSourceResult>(new IOException("failure"));
        await model.SearchAsync(_ => serverFails ? Failure() : Task.FromResult(new CutlistSourceResult([Server()], "1 Treffer")),
            _ => serverFails ? Task.FromResult(new CutlistSourceResult([Local()], "1 Treffer")) : Failure(), CancellationToken.None);
        Assert.Single(model.Results);
        Assert.Contains("fehlgeschlagen", serverFails ? model.ServerStatus : model.LocalStatus);
        Assert.Null(model.SelectedResult);
    }

    [Fact]
    public async Task EmptySearchDoesNotDemandAnAutomaticEmptyWindow()
    {
        var model = new CutlistSearchResultsViewModel("Film.mp4");
        await model.SearchAsync(_ => Task.FromResult(new CutlistSourceResult([], "Nicht eingerichtet")),
            _ => Task.FromResult(new CutlistSourceResult([], "0 Treffer")), CancellationToken.None);
        Assert.False(await model.Ready);
        Assert.False(model.CanLoad);
    }

    [Fact]
    public async Task LateIgnoringSourceCannotPublishAfterCancellationAndInvalidation()
    {
        var late = new TaskCompletionSource<CutlistSourceResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var model = new CutlistSearchResultsViewModel("Film.mp4");
        using var cancel = new CancellationTokenSource();
        var search = model.SearchAsync(_ => late.Task, _ => Task.FromResult(new CutlistSourceResult([], "0 Treffer")), cancel.Token);
        model.Invalidate();
        cancel.Cancel();
        await search;
        late.SetResult(new([Server()], "1 Treffer"));
        Assert.Empty(model.Results);
        Assert.Null(model.SelectedResult);
    }

    [Fact]
    public void FilmContextRejectsAnotherGenerationPathOrAnalysisEvenForSameFilename()
    {
        var context = LocalCutlistDiscoveryTests.Context(Path.Combine(Path.GetTempPath(), "Film.mp4"));
        Assert.True(context.IsCurrent(context.FilePath, context.Analysis, 1));
        Assert.False(context.IsCurrent(context.FilePath, context.Analysis, 2));
        Assert.False(context.IsCurrent(context.FilePath, context.Analysis with { }, 1));
        Assert.False(context.IsCurrent(Path.Combine(Path.GetTempPath(), "Other", "Film.mp4"), context.Analysis, 1));
    }
}
