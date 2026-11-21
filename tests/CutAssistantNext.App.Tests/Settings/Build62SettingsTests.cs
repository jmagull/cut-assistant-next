using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using CutAssistantNext.Media.Cutting;
using System.Windows.Input;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class Build62SettingsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void WorkDirectoryModeSurvivesRestartAndViewModelRoundTrip(int modeValue)
    {
        var mode = (CutWorkingDirectoryMode)modeValue;
        var root = Directory.CreateTempSubdirectory("can-settings62-");
        try
        {
            var path = Path.Combine(root.FullName, "folders.json");
            var settings = new VideoFolderSettings
            {
                WorkingDirectoryMode = mode,
                CustomWorkingDirectory = root.FullName,
                OriginalVideosDirectory = root.FullName,
                CutVideosDirectory = root.FullName,
                OwnCutlistsDirectory = root.FullName
            };
            Assert.True(new VideoFolderSettingsStore(path).Save(settings));
            var loaded = new VideoFolderSettingsStore(path).Load();
            Assert.Equal(settings, new VideoFolderSettingsViewModel(loaded).CreateSettings());
            var output = Path.Combine(root.FullName, "output.mp4");
            Assert.Equal(mode == CutWorkingDirectoryMode.WindowsTemp ? Path.TrimEndingDirectorySeparator(Path.GetTempPath()) : root.FullName,
                loaded.ResolveWorkingDirectory(output));
            Assert.Null(VideoFolderPathResolver.GetValidationError(loaded));
            var request = new CutRequest(Path.Combine(root.FullName, "original.mp4"), output,
                [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))]).WithWorkingDirectory(loaded.ResolveWorkingDirectory(output));
            Assert.Equal(request.WorkingDirectory, request.WithSourceFilePath(Path.Combine(root.FullName, "prepared.mp4")).WorkingDirectory);
        }
        finally { root.Delete(true); }
    }

    [Fact]
    public void WorkspaceRejectsActualMissingWritePermissionAndLeavesNoProbe()
    {
        var root = Directory.CreateTempSubdirectory("can-denied62-");
        var permissions = root.GetAccessControl();
        var descriptor = permissions.GetSecurityDescriptorBinaryForm();
        using var identity = WindowsIdentity.GetCurrent();
        permissions.AddAccessRule(new FileSystemAccessRule(identity.User!, FileSystemRights.CreateFiles, AccessControlType.Deny));
        root.SetAccessControl(permissions);
        try
        {
            Assert.Throws<UnauthorizedAccessException>(() => CutWorkspace.ValidateWritable(root.FullName));
        }
        finally
        {
            var restore = new DirectorySecurity(); restore.SetSecurityDescriptorBinaryForm(descriptor, AccessControlSections.Access);
            root.SetAccessControl(restore);
            Assert.Empty(root.EnumerateFileSystemInfos()); root.Delete();
        }
    }

    [Fact]
    public void LegacyFolderConfigurationDefaultsToOutputAndCustomRequiresExistingAbsoluteDirectory()
    {
        var root = Directory.CreateTempSubdirectory("can-settings62-");
        try
        {
            var file = Path.Combine(root.FullName, "old.json"); File.WriteAllText(file, "{\"CutVideosDirectory\":\"\"}");
            Assert.Equal(CutWorkingDirectoryMode.OutputDirectory, new VideoFolderSettingsStore(file).Load().WorkingDirectoryMode);
            Assert.NotNull(VideoFolderPathResolver.GetValidationError(new() { WorkingDirectoryMode = CutWorkingDirectoryMode.Custom }));
            Assert.NotNull(VideoFolderPathResolver.GetValidationError(new() { WorkingDirectoryMode = CutWorkingDirectoryMode.Custom, CustomWorkingDirectory = "relative" }));
        }
        finally { root.Delete(true); }
    }

    [Theory]
    [InlineData("CutStart", Key.F6)]
    [InlineData("CutEnd", Key.F7)]
    [InlineData("VideoStart", Key.F8)]
    [InlineData("VideoEnd", Key.F9)]
    public void NewShortcutsUseFactoryGesturesAndExistingConflictAndPersistenceRules(string actionName, Key key)
    {
        var action = Enum.Parse<PlaybackShortcut>(actionName);
        var root = Directory.CreateTempSubdirectory("can-shortcuts62-");
        try
        {
            var shortcuts = new PlaybackShortcuts();
            Assert.Equal(PlaybackShortcuts.Definitions.Single(d => d.Action == action).Standard, shortcuts[action]);
            Assert.Null(shortcuts.Assign(action, new(key)));
            Assert.Contains("Bereits belegt", shortcuts.Assign(PlaybackShortcut.FrameForward, new(key)));
            Assert.NotNull(shortcuts.Assign(action, new(Key.C, ModifierKeys.Control)));
            var file = Path.Combine(root.FullName, "shortcuts.json"); shortcuts.Save(file);
            Assert.Equal(action, PlaybackShortcuts.Load(file).Match(key, ModifierKeys.None));
            Assert.Null(shortcuts.Reset(action)); Assert.Equal(PlaybackShortcuts.Definitions.Single(d => d.Action == action).Standard, shortcuts[action]);
        }
        finally { root.Delete(true); }
    }

    [Theory]
    [InlineData("No space left on device", "Speicherplatzproblem")]
    [InlineData("System.IO.IOException (0x80070070)", "Speicherplatzproblem")]
    [InlineData("Write Error at 24%", "Ursache ist damit noch nicht nachgewiesen")]
    [InlineData("failed to write", "Schreibfehler")]
    [InlineData("unrelated native error", null)]
    public void FailureDiagnosisDistinguishesEvidenceFromPossibleCauses(string technical, string? expected)
    {
        var vm = new Mp4BoxProgressViewModel(); vm.MarkRunning(); vm.MarkFailed(technical);
        Assert.True(vm.HasFailed); Assert.False(vm.ShouldAutoClose);
        Assert.Contains(technical, vm.ProtocolText); Assert.Equal(vm.ProtocolText, vm.DisplayProtocolText);
        if (expected is not null) Assert.Contains(expected, vm.StatusText);
        else Assert.DoesNotContain("Speicherplatzproblem", vm.StatusText);
        Assert.DoesNotContain("beschädigte Quelldatei", vm.StatusText);
    }
}
