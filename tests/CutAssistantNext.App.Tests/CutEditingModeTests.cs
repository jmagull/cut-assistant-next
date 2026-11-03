using CutAssistantNext.App.Services;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Tests.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Tests;

public sealed class CutEditingModeTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-modes-{Guid.NewGuid():N}");
    private string SettingsPath => Path.Combine(_directory, "appearance.json");
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private static TimeSpan T(int seconds) => TimeSpan.FromSeconds(seconds);
    private static void Mark(CutPlanViewModel vm, int start, int end) { vm.SetStart(T(start)); vm.SetEnd(T(end)); }
    private static (TimeSpan, TimeSpan)[] Ranges(CutPlan plan) => plan.RemoveSegments.Select(s => (s.Start, s.End)).ToArray();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task LocalAndServerSelectionLoadTheSameKeepMarksWithoutChangingOwnFile(bool local)
    {
        Directory.CreateDirectory(_directory);
        var movie = Path.Combine(_directory, "Aufnahme.HQ.mp4");
        var path = movie + ".cutlist";
        var bytes = LocalCutlistDiscoveryTests.Bytes();
        await File.WriteAllBytesAsync(path, bytes);
        var row = local
            ? Assert.Single((await new LocalCutlistDiscoveryService().SearchAsync(LocalCutlistDiscoveryTests.Context(movie), _directory)).Rows)
            : CutlistResultRow.FromServer(new CutlistSearchResult { Id = "1" }, "https://example.invalid/");
        var imported = await CutlistSelectionReader.ReadAsync(row, (_, _, _) =>
        {
            Assert.False(local); return Task.FromResult(bytes);
        });
        var model = new CutPlanViewModel(); model.Initialize(T(100), CutEditingMode.Keep);
        model.LoadCutPlan(CutlistCutPlanBuilder.Build(imported.Document, T(100)));
        Assert.Equal(new[] { (T(0), T(10)), (T(20), T(40)), (T(50), T(100)) }, model.MarkedSegments.Select(s => (s.Start, s.End)));
        Assert.Equal(bytes, await File.ReadAllBytesAsync(path));
    }

    [Fact]
    public void RedAndGreenEditingProduceTheSameCanonicalCutAndSavedCutlist()
    {
        var red = new CutPlanViewModel(); red.Initialize(T(100));
        Mark(red, 0, 10); Mark(red, 20, 40); Mark(red, 50, 100);
        var green = new CutPlanViewModel(); green.Initialize(T(100), CutEditingMode.Keep);
        Mark(green, 10, 20); Mark(green, 40, 50);
        Assert.Equal(Ranges(red.CreateCutPlanSnapshot()), Ranges(green.CreateCutPlanSnapshot()));
        var redRequest = CutRequestFactory.Create("source.mp4", "output.mp4", red.CreateCutPlanSnapshot(), 50);
        var greenRequest = CutRequestFactory.Create("source.mp4", "output.mp4", green.CreateCutPlanSnapshot(), 50);
        Assert.Equal(redRequest.KeepSegments.Select(s => (s.Start, s.Duration)), greenRequest.KeepSegments.Select(s => (s.Start, s.Duration)));
        Assert.Equal(T(20), green.EstimatedOutputDuration);
        var keep = CutlistKeepSegmentBuilder.Build(CutlistSaveGuard.CreateSnapshot(green));
        Assert.Equal(new[] { (T(10), T(20)), (T(40), T(50)) }, keep.Select(s => (s.Start, s.End)));
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, "work.cutlist");
        CutlistFileWriter.WriteSafely(path, new CutlistDocument(new CutlistGeneralMetadata { NoOfCuts = keep.Count }, keep, new CutlistInfoMetadata()));
        var resumed = new CutPlanViewModel(); resumed.Initialize(T(100), CutEditingMode.Keep);
        resumed.LoadCutPlan(CutlistCutPlanBuilder.Build(CutlistFileReader.Read(path), T(100)));
        Assert.Equal(green.MarkedSegments.Select(s => (s.Start, s.End)), resumed.MarkedSegments.Select(s => (s.Start, s.End)));
        resumed.SelectedRemoveSegment = resumed.MarkedSegments[0]; resumed.SetEnd(T(25));
        Assert.Equal(T(25), resumed.MarkedSegments[0].End);
        Assert.Equal(T(20), green.MarkedSegments[0].End);
    }

    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void IncompleteRangeCannotBeSavedAndCancelPreservesCompletedWork(CutEditingMode mode)
    {
        var vm = new CutPlanViewModel(); vm.Initialize(T(100), mode); Mark(vm, 10, 20);
        var original = vm.MarkedSegments.Single(); vm.SetStart(T(40));
        Assert.Throws<InvalidOperationException>(() => CutlistSaveGuard.CreateSnapshot(vm));
        Assert.Throws<ArgumentOutOfRangeException>(() => vm.SetEnd(T(30)));
        Assert.Equal(T(40), vm.PendingStart);
        vm.CancelPendingStart();
        Assert.Null(vm.PendingStart); Assert.Same(original, vm.MarkedSegments.Single());
        vm.SetStart(T(40)); vm.SetEnd(T(50));
        Assert.Equal(2, vm.MarkedSegments.Count);
        Assert.NotNull(CutlistSaveGuard.CreateSnapshot(vm));
    }

    [Fact]
    public void PersistedPreferenceIsAppliedOnlyToNextFilmAndSurvivesNewStoreInstance()
    {
        var store = new AppearanceSettingsStore(SettingsPath);
        Assert.Equal(CutEditingMode.Remove, store.Load().CutEditing);
        var loaded = new CutPlanViewModel(); loaded.Initialize(T(100), store.Load().CutEditing); Mark(loaded, 10, 20);
        var before = Ranges(loaded.CreateCutPlanSnapshot());
        Assert.True(store.Save(new(InterfaceLayout.PowerUser, CutEditingMode.Keep)));
        Assert.Equal(CutEditingMode.Remove, loaded.EditingMode);
        Assert.Equal(before, Ranges(loaded.CreateCutPlanSnapshot()));
        var next = new CutPlanViewModel(); next.Initialize(T(100), new AppearanceSettingsStore(SettingsPath).Load().CutEditing);
        Assert.Equal(CutEditingMode.Keep, next.EditingMode);
        Assert.Empty(next.MarkedSegments);
        Assert.Equal(TimeSpan.Zero, next.EstimatedOutputDuration);
        Assert.Contains("unmarkierten", next.KeepResultConfirmation);
    }

    [Theory]
    [InlineData("{\"Layout\":1}")]
    [InlineData("{\"Layout\":1,\"CutEditing\":999}")]
    public void OlderOrInvalidModePreservesValidLayoutAndDefaultsToRemove(string json)
    {
        Directory.CreateDirectory(_directory); File.WriteAllText(SettingsPath, json);
        var loaded = new AppearanceSettingsStore(SettingsPath).Load();
        Assert.Equal(InterfaceLayout.PowerUser, loaded.Layout);
        Assert.Equal(CutEditingMode.Remove, loaded.CutEditing);
    }

    [Fact]
    public void InvalidModeCannotBeSavedOrReinterpretAnExistingPlan()
    {
        var vm = new CutPlanViewModel(); vm.Initialize(T(100)); Mark(vm, 10, 20);
        Assert.Throws<ArgumentOutOfRangeException>(() => vm.Initialize(T(100), (CutEditingMode)99));
        Assert.Equal(CutEditingMode.Remove, vm.EditingMode); Assert.Single(vm.MarkedSegments);
        Assert.Throws<ArgumentOutOfRangeException>(() => new AppearanceSettingsStore(SettingsPath).Save(new(CutEditing: (CutEditingMode)99)));
        Assert.False(File.Exists(SettingsPath));
    }

    [Fact]
    public void SnapshotDoesNotTurnUnmarkedPartsIntoEditorMarks()
    {
        var vm = new CutPlanViewModel(); vm.Initialize(T(100), CutEditingMode.Keep); Mark(vm, 20, 30);
        var selected = vm.MarkedSegments.Single();
        var actual = vm.CreateCutPlanSnapshot();
        Assert.Equal(new[] { (T(0), T(20)), (T(30), T(100)) }, Ranges(actual));
        Assert.Same(selected, vm.MarkedSegments.Single());
        actual.Remove(actual.RemoveSegments[0]);
        Assert.Same(selected, vm.MarkedSegments.Single());
        Assert.Contains("unmarkierte", vm.EditingDescription);
    }

    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void LoadingExistingCanonicalCutsPreservesResultInEitherEditingMode(CutEditingMode mode)
    {
        var original = new CutPlan(T(100)); original.Add(new RemoveSegment(T(10), T(20)));
        var vm = new CutPlanViewModel(); vm.Initialize(T(100), mode); vm.LoadCutPlan(original);
        Assert.Equal(mode, vm.EditingMode);
        Assert.Equal(Ranges(original), Ranges(vm.CreateCutPlanSnapshot()));
        Assert.Equal(mode == CutEditingMode.Keep ? 2 : 1, vm.MarkedSegments.Count);
        Assert.Null(vm.PendingStart);
    }

    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void FrameEdgeEditingAndDeletionOperateOnTheVisibleMarkedRange(CutEditingMode mode)
    {
        var vm = new CutPlanViewModel(); vm.Initialize(T(100), mode); Mark(vm, 10, 20);
        vm.SelectEdge(vm.MarkedSegments[0], CutEdgeSide.Start);
        vm.ApplyFrameEdge(vm.MarkedSegments[0], CutEdgeSide.Start, T(12));
        Assert.Equal(T(12), vm.MarkedSegments[0].Start);
        Assert.Equal(mode == CutEditingMode.Keep ? T(15) : T(9), vm.GetEdgeReviewPosition(vm.MarkedSegments[0], CutEdgeSide.Start));
        Assert.Equal(mode == CutEditingMode.Keep ? T(17) : T(23), vm.GetEdgeReviewPosition(vm.MarkedSegments[0], CutEdgeSide.End));
        Assert.True(vm.Remove(vm.MarkedSegments[0])); Assert.Empty(vm.MarkedSegments);
    }

    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void HalfOpenFilmBoundariesAndOneTickRangesRemainExact(CutEditingMode mode)
    {
        var vm = new CutPlanViewModel(); vm.Initialize(T(1), mode);
        vm.SetStart(TimeSpan.Zero); vm.SetEnd(TimeSpan.FromTicks(1));
        vm.SetStart(T(1) - TimeSpan.FromTicks(1)); vm.SetEnd(T(1));
        Assert.Equal(2, vm.MarkedSegments.Count);
        Assert.Equal(mode == CutEditingMode.Keep ? TimeSpan.FromTicks(2) : T(1) - TimeSpan.FromTicks(2), vm.EstimatedOutputDuration);
        Assert.Throws<ArgumentOutOfRangeException>(() => vm.SetStart(T(2)));
        Assert.Equal(2, vm.MarkedSegments.Count);
        var output = CutlistKeepSegmentBuilder.Build(vm.CreateCutPlanSnapshot());
        Assert.Equal(mode == CutEditingMode.Keep ? 2 : 1, output.Count);
    }
}
