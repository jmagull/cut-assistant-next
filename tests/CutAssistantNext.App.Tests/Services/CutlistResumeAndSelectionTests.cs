using CutAssistantNext.App.Services;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistResumeAndSelectionTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-resume-{Guid.NewGuid():N}");
    public CutlistResumeAndSelectionTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);
    private static Task<byte[]> NoDownload(string url, string id, CancellationToken token) => throw new InvalidOperationException("Local files must not be downloaded.");

    [Fact]
    public async Task SavedPartialWorkCanBeSelectedResumedAndEditedWithoutReadingOrMergingServerCuts()
    {
        var model = new CutPlanViewModel();
        model.Initialize(TimeSpan.FromSeconds(100));
        foreach (var (start, end) in new[] { (10, 20), (40, 50), (60, 70) })
        { model.SetStart(TimeSpan.FromSeconds(start)); model.SetEnd(TimeSpan.FromSeconds(end)); }
        var plan = CutlistSaveGuard.CreateSnapshot(model);
        var keep = CutlistKeepSegmentBuilder.Build(plan);
        var document = new CutlistDocument(new CutlistGeneralMetadata
        { NoOfCuts = keep.Count, ApplyToFile = "Aufnahme.HQ.mp4", OriginalFileSizeBytes = 1234, FramesPerSecond = 25 },
            keep, new CutlistInfoMetadata { RatingByAuthor = 4, Author = "Eigene Arbeit" });
        var movie = Path.Combine(_root, "Aufnahme.HQ.mp4");
        var path = movie + ".cutlist";
        CutlistFileWriter.WriteSafely(path, document);
        var before = await File.ReadAllBytesAsync(path);
        var results = await new LocalCutlistDiscoveryService().SearchAsync(LocalCutlistDiscoveryTests.Context(movie), _root);
        var row = Assert.Single(results.Rows);
        var imported = await CutlistSelectionReader.ReadAsync(row, NoDownload);
        Assert.Equal(path, imported.LocalFilePath);
        var resumed = new CutPlanViewModel();
        resumed.LoadCutPlan(CutlistCutPlanBuilder.Build(imported.Document, TimeSpan.FromSeconds(100)));
        Assert.Equal(plan.RemoveSegments.Select(s => (s.Start, s.End)), resumed.RemoveSegments.Select(s => (s.Start, s.End)));
        resumed.SelectedRemoveSegment = resumed.RemoveSegments[2];
        resumed.SetEnd(TimeSpan.FromSeconds(75));
        Assert.Equal(TimeSpan.FromSeconds(75), resumed.RemoveSegments[2].End);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    [Fact]
    public void OpenStartBlocksSavingWithoutDiscardingClosedCutsOrChangingSelectionState()
    {
        var model = new CutPlanViewModel();
        model.Initialize(TimeSpan.FromSeconds(100));
        model.SetStart(TimeSpan.FromSeconds(10)); model.SetEnd(TimeSpan.FromSeconds(20));
        model.SetStart(TimeSpan.FromSeconds(40));
        var path = Path.Combine(_root, "blocked.cutlist");
        var error = Assert.Throws<InvalidOperationException>(() =>
        {
            var snapshot = CutlistSaveGuard.CreateSnapshot(model);
            var keep = CutlistKeepSegmentBuilder.Build(snapshot);
            CutlistFileWriter.WriteSafely(path, new CutlistDocument(new CutlistGeneralMetadata { NoOfCuts = keep.Count }, keep, new()));
        });
        Assert.Equal(CutlistSaveGuard.Message, error.Message);
        Assert.False(File.Exists(path));
        Assert.Single(model.RemoveSegments);
        Assert.Equal(TimeSpan.FromSeconds(40), model.PendingStart);
        model.SetEnd(TimeSpan.FromSeconds(50));
        Assert.Equal(2, CutlistSaveGuard.CreateSnapshot(model).RemoveSegments.Count);
    }

    [Fact]
    public async Task ChangedLocalFileCannotBeLoadedFromStaleSearchResult()
    {
        var path = Path.Combine(_root, "Aufnahme.HQ.mp4.cutlist");
        await File.WriteAllBytesAsync(path, LocalCutlistDiscoveryTests.Bytes());
        var row = Assert.Single((await new LocalCutlistDiscoveryService().SearchAsync(
            LocalCutlistDiscoveryTests.Context(path[..^8]), _root)).Rows);
        await File.WriteAllBytesAsync(path, LocalCutlistDiscoveryTests.Bytes("andere Fassung"));
        await Assert.ThrowsAsync<IOException>(() => CutlistSelectionReader.ReadAsync(row, NoDownload));
        Assert.Equal(LocalCutlistDiscoveryTests.Bytes("andere Fassung"), await File.ReadAllBytesAsync(path));
    }

    [Fact]
    public async Task ServerSelectionHasNoLocalUploadCandidateAndNeverChangesExistingOwnFile()
    {
        var path = Path.Combine(_root, "Aufnahme.HQ.mp4.cutlist");
        await File.WriteAllBytesAsync(path, LocalCutlistDiscoveryTests.Bytes("eigene Arbeit"));
        var before = await File.ReadAllBytesAsync(path);
        var server = CutlistResultRow.FromServer(new CutlistSearchResult { Id = "321", Author = "Serverautor" }, "https://example.invalid/");
        var imported = await CutlistSelectionReader.ReadAsync(server, (url, id, _) =>
        {
            Assert.Equal("321", id);
            Assert.Equal("https://example.invalid/", url);
            return Task.FromResult(LocalCutlistDiscoveryTests.Bytes("Serverarbeit"));
        });
        Assert.Null(imported.LocalFilePath);
        Assert.Equal("Serverarbeit", imported.Document.Info.UserComment);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
        Assert.Throws<IOException>(() => CutlistFileWriter.WriteSafely(path, imported.Document));
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    [Fact]
    public void SnapshotDoesNotRequireAllAdvertisingToBeRemovedOrACompletionFlag()
    {
        var plan = new CutPlanViewModel(); plan.Initialize(TimeSpan.FromSeconds(100));
        Assert.Single(CutlistKeepSegmentBuilder.Build(CutlistSaveGuard.CreateSnapshot(plan)));
        plan.SetStart(TimeSpan.FromSeconds(10)); plan.SetEnd(TimeSpan.FromSeconds(20));
        Assert.Equal(2, CutlistKeepSegmentBuilder.Build(CutlistSaveGuard.CreateSnapshot(plan)).Count);
    }
}
