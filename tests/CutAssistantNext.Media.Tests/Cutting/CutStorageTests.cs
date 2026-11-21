using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class CutStorageTests : IDisposable
{
    private readonly DirectoryInfo _root = Directory.CreateTempSubdirectory("can-storage62-");
    public void Dispose() => _root.Delete(true);
    private string PathIn(string name) => Path.Combine(_root.FullName, name);
    private sealed class Log : IProgress<CutProgressUpdate>
    {
        public readonly List<string> Lines = [];
        public void Report(CutProgressUpdate update) => Lines.Add(update.Message);
        public override string ToString() => string.Join("\n", Lines);
    }

    private sealed class Callback(Action<CutProgressUpdate> callback) : IProgress<CutProgressUpdate>
    {
        public void Report(CutProgressUpdate update) => callback(update);
    }

    [Fact]
    public async Task CancellationAfterPartialCopyDeletesOnlyStagingAndPreservesExistingTarget()
    {
        var pending = PathIn("large.mp4"); var output = PathIn("target.mp4");
        File.WriteAllBytes(pending, new byte[3 * 1048576]); File.WriteAllText(output, "preserve");
        using var cancellation = new CancellationTokenSource();
        double? observed = null;
        var progress = new Callback(update =>
        {
            if (update.Kind != CutProgressKind.Progress) return;
            observed = update.Percentage; cancellation.Cancel();
        });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            CutOutputPublisher.PublishAsync(pending, output, true, progress, cancellation.Token));
        Assert.True(observed is > 0 and < 100); Assert.Equal("preserve", File.ReadAllText(output));
        Assert.Empty(Directory.GetFiles(_root.FullName, ".can-publish-*"));
    }

    [Fact]
    public async Task TargetModifiedAfterOriginalConsentIsNotOverwritten()
    {
        var pending = PathIn("pending.mp4"); var output = PathIn("target.mp4");
        File.WriteAllText(pending, "new output"); File.WriteAllText(output, "old target");
        var original = await CutOutputIdentityVerifier.ReadAsync(output);
        File.WriteAllText(output, "changed by another process");
        await Assert.ThrowsAsync<IOException>(() => CutOutputPublisher.PublishAsync(pending, output, true, null, default, original));
        Assert.Equal("changed by another process", File.ReadAllText(output));
        Assert.Empty(Directory.GetFiles(_root.FullName, ".can-publish-*"));
    }

    [Theory]
    [InlineData(true, 299, true)]
    [InlineData(true, 300, false)]
    [InlineData(false, 199, true)]
    [InlineData(false, 200, false)]
    public void SharedCapacityIncludesSegmentsConcatAndTargetCopy(bool shared, long available, bool low)
    {
        var log = new Log();
        var work = PathIn("work");
        var result = CutStoragePreflight.Check(work, PathIn("out.mp4"), new(200, 100, "segments + concat + copy"), log,
            path => new(path == work || shared ? "work-volume" : "target-volume", available));
        Assert.Equal(low, result.LowSpace);
        Assert.Equal(available, result.Working.AvailableBytes);
        Assert.Equal(available, result.Target.AvailableBytes);
        Assert.Contains(work, log.ToString());
        Assert.Contains(_root.FullName, log.ToString());
        Assert.Contains("Gemessen frei", log.ToString());
        Assert.Contains("Geschätzter zusätzlicher", log.ToString());
        Assert.Contains("ohne Platzgarantie", log.ToString());
        Assert.Equal(low, log.ToString().Contains("WARNUNG"));
    }

    [Fact]
    public void TargetCapacityIsCheckedSeparatelyAndUnknownMeasurementsAreReported()
    {
        var work = PathIn("work"); var log = new Log();
        var check = CutStoragePreflight.Check(work, PathIn("out.mp4"), new(200, 100, "test"), log,
            path => path == work ? new("W", 1000) : new("T", 99));
        Assert.True(check.LowSpace);
        check = CutStoragePreflight.Check(work, PathIn("out.mp4"), new(200, 100, "test"), log,
            _ => new("", null, "access denied 5"));
        Assert.False(check.LowSpace);
        Assert.Contains(check.Warnings, w => w.Contains("unvollständig"));
        Assert.Contains("access denied 5", log.ToString());
    }

    [Fact]
    public void ActualMeasurementAndEstimatedShortageStopBeforeProcessing()
    {
        var log = new Log();
        Assert.NotNull(CutStoragePreflight.Measure(_root.FullName).AvailableBytes);
        Assert.Throws<IOException>(() => CutStoragePreflight.EnsureSufficient(PathIn("source"), _root.FullName,
            PathIn("output.mp4"), new(long.MaxValue / 2, long.MaxValue / 2, "test estimate"), log));
        Assert.Contains("vorsorglich nicht gestartet", log.ToString());
    }

    [Fact]
    public void EstimatesIncludePcmAndAdditionalCopiesWithoutMutatingCutBoundaries()
    {
        var mp4 = CutStorageEstimate.Mp4Box(1000);
        var otr = CutStorageEstimate.OtrCan(1000, TimeSpan.FromSeconds(100), 48000 * 6 * 8);
        Assert.True(mp4.WorkingBytes > 3000);
        Assert.True(mp4.TargetBytes > 1000);
        Assert.True(otr.WorkingBytes > mp4.WorkingBytes + 100 * 48000L * 6 * 8);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WorkspaceCleansOnlyOwnedFilesOnSuccessAndUnwind(bool fail)
    {
        var keep = PathIn("unrelated.txt"); File.WriteAllText(keep, "keep");
        var log = new Log(); string? work = null;
        try
        {
            using var workspace = CutWorkspace.Create(PathIn("output.mp4"), _root.FullName + Path.DirectorySeparatorChar, log);
            work = workspace.DirectoryPath;
            File.WriteAllText(Path.Combine(work, "segment.mp4"), "partial");
            if (fail) throw new OperationCanceledException();
        }
        catch (OperationCanceledException) when (fail) { }
        Assert.NotNull(work); Assert.False(Directory.Exists(work));
        Assert.Equal("keep", File.ReadAllText(keep));
        Assert.Contains("Tatsächlicher temporärer Arbeitsordner", log.ToString());
        Assert.Contains("bereinigt", log.ToString());
    }

    [Fact]
    public void MissingOrInvalidWorkDirectoryIsRejectedWithoutCreatingIt()
    {
        var absent = PathIn("missing"); var log = new Log();
        Assert.Throws<DirectoryNotFoundException>(() => CutWorkspace.Create(PathIn("output.mp4"), absent, log));
        Assert.False(Directory.Exists(absent));
        Assert.Contains("Schreibprüfung", log.ToString());
    }

    [Fact]
    public void CleanupFailureIsLoggedAndCanBeRetriedWithoutLosingOtherFiles()
    {
        var log = new Log();
        var workspace = CutWorkspace.Create(PathIn("output.mp4"), _root.FullName, log);
        var pending = Path.Combine(workspace.DirectoryPath, "locked.mp4");
        using (var file = new FileStream(pending, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            workspace.Dispose();
            Assert.Contains("Bereinigungsfehler", log.ToString());
            Assert.Contains(workspace.DirectoryPath, log.ToString());
            Assert.Contains("locked.mp4", log.ToString());
        }
        workspace.Dispose(); Assert.False(Directory.Exists(workspace.DirectoryPath));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CopyToSeparateTargetDirectoryPreservesIdentityAndPublishesAfterCompletion(bool overwrite)
    {
        var work = Directory.CreateDirectory(PathIn("work"));
        var target = Directory.CreateDirectory(PathIn("target"));
        var pending = Path.Combine(work.FullName, "pending.mp4");
        var output = Path.Combine(target.FullName, "output.mp4");
        File.WriteAllText(pending, "complete new file");
        var identity = await CutOutputIdentityVerifier.ReadAsync(pending);
        if (overwrite) File.WriteAllText(output, "existing");
        await CutOutputPublisher.PublishAsync(pending, output, overwrite, new Log(), default);
        Assert.Equal("complete new file", File.ReadAllText(output));
        Assert.True(File.Exists(pending)); // publisher does not own engine work
        Assert.Single(target.GetFiles());
        await CutOutputIdentityVerifier.ValidateAsync(output, identity, true);
    }

    [Theory]
    [InlineData("cancel")]
    [InlineData("late-target")]
    [InlineData("locked-target")]
    public async Task FailedOrCanceledTransferPreservesTargetAndRemovesStaging(string mode)
    {
        var pending = PathIn("pending.mp4"); var output = PathIn("output.mp4");
        File.WriteAllBytes(pending, new byte[1048576]); File.WriteAllText(output, "existing");
        using var cancel = new CancellationTokenSource();
        if (mode == "cancel") cancel.Cancel();
        using var locked = mode == "locked-target" ? File.Open(output, FileMode.Open, FileAccess.Read, FileShare.Read) : null;
        var action = () => CutOutputPublisher.PublishAsync(pending, output, mode == "locked-target", new Log(), cancel.Token);
        if (mode == "cancel") await Assert.ThrowsAnyAsync<OperationCanceledException>(action);
        else
        {
            var failure = await Record.ExceptionAsync(action);
            Assert.True(failure is IOException or UnauthorizedAccessException, failure?.ToString());
        }
        Assert.Equal("existing", File.ReadAllText(output));
        Assert.Empty(Directory.GetFiles(_root.FullName, ".can-publish-*"));
    }

    [Fact]
    public void TechnicalFailureIncludesHresultDirectoriesAndLaterMeasurement()
    {
        var log = new Log();
        CutStoragePreflight.ReportFailure(new IOException("native write error 24%"), _root.FullName, PathIn("out.mp4"), log);
        Assert.Contains("HRESULT", log.ToString()); Assert.Contains("native write error 24%", log.ToString());
        Assert.Contains("Messung nach Fehler", log.ToString()); Assert.Contains(_root.FullName, log.ToString());
    }
}
