using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class WorkingDirectoryInfoTests : IDisposable
{
    private readonly DirectoryInfo _root = Directory.CreateTempSubdirectory("can-folder-info-");
    private const long GiB = 1073741824;
    public void Dispose() => _root.Delete(true);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task AllModesResolveActualPathAndSeparateDiskFromFolderSize(int mode)
    {
        var reader = new FakeReader();
        using var vm = new WorkingDirectoryInfoViewModel(reader);
        var settings = new VideoFolderSettings
        {
            WorkingDirectoryMode = (CutWorkingDirectoryMode)mode,
            CutVideosDirectory = _root.FullName,
            CustomWorkingDirectory = _root.FullName + Path.DirectorySeparatorChar
        };
        await vm.RefreshAsync(settings);
        Assert.Equal(mode == 1 ? Path.TrimEndingDirectorySeparator(Path.GetTempPath()) : _root.FullName, vm.ResolvedPath);
        Assert.Equal(vm.ResolvedPath, reader.LastPath);
        Assert.Contains("100", vm.CapacityText); Assert.Contains("25", vm.FreeText); Assert.Contains("2", vm.FolderSizeText);
        Assert.EndsWith("GiB", vm.CapacityText); Assert.EndsWith("GiB", vm.FreeText); Assert.EndsWith("GiB", vm.FolderSizeText);
        Assert.False(vm.IsBusy); Assert.Empty(vm.Notice);
    }

    [Fact]
    public async Task RefreshRereadsChangedDiskAndFolderValues()
    {
        var reader = new FakeReader(); using var vm = new WorkingDirectoryInfoViewModel(reader);
        await vm.RefreshAsync(Custom(_root.FullName));
        reader.Drive = new(200 * GiB, 10 * GiB); reader.Folder = new(3 * GiB);
        await vm.RefreshAsync(Custom(_root.FullName));
        Assert.Equal(2, reader.DriveCalls); Assert.Equal(2, reader.FolderCalls);
        Assert.Contains("200", vm.CapacityText); Assert.Contains("10", vm.FreeText); Assert.Contains("3", vm.FolderSizeText);
    }

    [Fact]
    public async Task InvalidEditClearsPreviouslyResolvedPath()
    {
        var reader = new FakeReader(); using var vm = new WorkingDirectoryInfoViewModel(reader);
        await vm.RefreshAsync(Custom(_root.FullName));
        await vm.RefreshAsync(Custom("relative"));
        Assert.DoesNotContain(_root.FullName, vm.ResolvedPath);
        Assert.NotEmpty(vm.Notice); Assert.Equal(1, reader.DriveCalls);
    }

    [Fact]
    public async Task RapidPathEditsCancelDebouncedIoForOldSelections()
    {
        var reader = new FakeReader(); using var vm = new WorkingDirectoryInfoViewModel(reader);
        var first = vm.RefreshAsync(Custom(Path.Combine(_root.FullName, "one")), debounce: true);
        var second = vm.RefreshAsync(Custom(Path.Combine(_root.FullName, "two")), debounce: true);
        var last = vm.RefreshAsync(Custom(_root.FullName), debounce: true);
        await Task.WhenAll(first, second, last);
        Assert.Equal(1, reader.DriveCalls); Assert.Equal(_root.FullName, vm.ResolvedPath);
    }

    [Fact]
    public async Task SlowFolderCalculationDoesNotDelayDiskDisplay()
    {
        var completion = new TaskCompletionSource<WorkingFolderSize>(TaskCreationOptions.RunContinuationsAsynchronously);
        var reader = new FakeReader { FolderRead = (_, token) => completion.Task.WaitAsync(token) };
        using var vm = new WorkingDirectoryInfoViewModel(reader);
        var refresh = vm.RefreshAsync(Custom(_root.FullName));
        Assert.False(refresh.IsCompleted); Assert.True(vm.IsBusy);
        Assert.Contains("25", vm.FreeText); Assert.Contains("Hintergrund", vm.FolderSizeText);
        completion.SetResult(new(GiB)); await refresh;
        Assert.False(vm.IsBusy); Assert.Contains("1", vm.FolderSizeText);
    }

    [Fact]
    public async Task OutdatedResultCannotOverwriteNewSelection()
    {
        var oldResult = new TaskCompletionSource<WorkingDriveInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        var reader = new FakeReader();
        reader.DriveRead = (path, _) => path == _root.FullName ? oldResult.Task : Task.FromResult(new WorkingDriveInfo(300 * GiB, 50 * GiB));
        using var vm = new WorkingDirectoryInfoViewModel(reader);
        var first = vm.RefreshAsync(Custom(_root.FullName));
        var newerPath = Path.Combine(_root.FullName, "new-selection");
        await vm.RefreshAsync(Custom(newerPath));
        oldResult.SetResult(new(10 * GiB, GiB)); await first;
        Assert.Equal(newerPath, vm.ResolvedPath); Assert.Contains("300", vm.CapacityText); Assert.Contains("50", vm.FreeText);
    }

    [Fact]
    public async Task CloseCancelsPendingSizeAndPreventsLateNotifications()
    {
        CancellationToken observed = default;
        var never = new TaskCompletionSource<WorkingFolderSize>(TaskCreationOptions.RunContinuationsAsynchronously);
        var reader = new FakeReader { FolderRead = (_, token) => { observed = token; return never.Task.WaitAsync(token); } };
        var vm = new WorkingDirectoryInfoViewModel(reader); var notifications = 0;
        vm.PropertyChanged += (_, _) => notifications++;
        var refresh = vm.RefreshAsync(Custom(_root.FullName)); var before = notifications;
        vm.Dispose(); await refresh;
        Assert.True(observed.IsCancellationRequested); Assert.Equal(before, notifications);
        await vm.RefreshAsync(Custom(_root.FullName)); Assert.Equal(1, reader.DriveCalls);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(2, "")]
    [InlineData(2, "relative")]
    public async Task InvalidSelectionShowsHintWithoutStartingIo(int mode, string custom)
    {
        var reader = new FakeReader(); using var vm = new WorkingDirectoryInfoViewModel(reader);
        await vm.RefreshAsync(new() { WorkingDirectoryMode = (CutWorkingDirectoryMode)mode, CustomWorkingDirectory = custom });
        Assert.Equal(0, reader.DriveCalls); Assert.False(vm.IsBusy);
        Assert.NotEmpty(vm.Notice); Assert.Equal("Nicht ermittelbar", vm.FolderSizeText);
    }

    [Fact]
    public async Task PartialOrMissingMeasurementsNeverAppearAsCompleteFolderSize()
    {
        var reader = new FakeReader { Drive = new(null, null, "Datenträger nicht erreichbar"), Folder = new(null, "Zugriffsrechte fehlen") };
        using var vm = new WorkingDirectoryInfoViewModel(reader);
        await vm.RefreshAsync(Custom(_root.FullName));
        Assert.Equal("Nicht ermittelbar", vm.CapacityText); Assert.Equal("Nicht ermittelbar", vm.FreeText);
        Assert.Equal("Nicht ermittelbar", vm.FolderSizeText); Assert.Contains("Zugriffsrechte", vm.Notice);
    }

    [Fact]
    public async Task RealReaderCountsNestedHiddenFilesWithoutWritingOrDeletingThem()
    {
        var child = Directory.CreateDirectory(Path.Combine(_root.FullName, "child"));
        var first = Path.Combine(_root.FullName, "one.tmp"); var second = Path.Combine(child.FullName, "two.tmp");
        File.WriteAllBytes(first, new byte[129]); File.WriteAllBytes(second, new byte[257]);
        File.SetAttributes(second, FileAttributes.Hidden);
        var reader = new WorkingDirectoryInfoReader();
        var size = await reader.ReadFolderAsync(_root.FullName, default);
        var drive = await reader.ReadDriveAsync(_root.FullName, default);
        Assert.Equal(386, size.Bytes); Assert.Null(size.Notice);
        Assert.True(drive.CapacityBytes > 0); Assert.True(drive.FreeBytes >= 0); Assert.True(drive.FreeBytes <= drive.CapacityBytes);
        Assert.Equal(129, new FileInfo(first).Length); Assert.Equal(257, new FileInfo(second).Length);
        Assert.Equal(2, Directory.GetFiles(_root.FullName, "*", SearchOption.AllDirectories).Length);
    }

    [Fact]
    public async Task BoundedScanAndPreCancellationReturnPromptlyWithoutPartialTotal()
    {
        File.WriteAllText(Path.Combine(_root.FullName, "one.tmp"), "123"); File.WriteAllText(Path.Combine(_root.FullName, "two.tmp"), "456");
        var bounded = await new WorkingDirectoryInfoReader(entryLimit: 1).ReadFolderAsync(_root.FullName, default);
        Assert.Null(bounded.Bytes); Assert.Contains("nicht vollständig", bounded.Notice);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new WorkingDirectoryInfoReader().ReadFolderAsync(_root.FullName, new(true)));
    }

    [Fact]
    public async Task MissingReadPermissionIsReportedAndAclIsRestored()
    {
        var original = _root.GetAccessControl(); var descriptor = original.GetSecurityDescriptorBinaryForm();
        using var identity = WindowsIdentity.GetCurrent();
        original.AddAccessRule(new FileSystemAccessRule(identity.User!, FileSystemRights.ListDirectory, AccessControlType.Deny));
        _root.SetAccessControl(original);
        try
        {
            var result = await new WorkingDirectoryInfoReader().ReadFolderAsync(_root.FullName, default);
            Assert.Null(result.Bytes); Assert.Contains("Zugriffsrechte", result.Notice);
        }
        finally
        {
            var restore = new DirectorySecurity(); restore.SetSecurityDescriptorBinaryForm(descriptor, AccessControlSections.Access);
            _root.SetAccessControl(restore);
        }
    }

    private static VideoFolderSettings Custom(string path) => new() { WorkingDirectoryMode = CutWorkingDirectoryMode.Custom, CustomWorkingDirectory = path };

    private sealed class FakeReader : IWorkingDirectoryInfoReader
    {
        public WorkingDriveInfo Drive = new(100 * GiB, 25 * GiB);
        public WorkingFolderSize Folder = new(2 * GiB);
        public Func<string, CancellationToken, Task<WorkingDriveInfo>>? DriveRead;
        public Func<string, CancellationToken, Task<WorkingFolderSize>>? FolderRead;
        public int DriveCalls, FolderCalls;
        public string? LastPath;
        public Task<WorkingDriveInfo> ReadDriveAsync(string path, CancellationToken token)
        { DriveCalls++; LastPath = path; return DriveRead?.Invoke(path, token) ?? Task.FromResult(Drive); }
        public Task<WorkingFolderSize> ReadFolderAsync(string path, CancellationToken token)
        { FolderCalls++; return FolderRead?.Invoke(path, token) ?? Task.FromResult(Folder); }
    }
}
