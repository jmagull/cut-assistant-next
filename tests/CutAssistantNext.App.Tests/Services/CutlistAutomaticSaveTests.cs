using System.Security.AccessControl;
using System.Security.Principal;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistAutomaticSaveTests : IDisposable
{
    private const string OriginalName = "Ghostbusters__Hexenprobleme_26.09.19_10-50_pro7maxx_25_TVOON_DE.HQ.mp4";
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-cutlist-save-{Guid.NewGuid():N}");
    private readonly string _cutlists;
    private readonly string _original;
    private string Target => Path.Combine(_cutlists, OriginalName + ".cutlist");

    public CutlistAutomaticSaveTests()
    {
        Directory.CreateDirectory(_root);
        _cutlists = Directory.CreateDirectory(Path.Combine(_root, "Eigene Cutlists")).FullName;
        var originals = Directory.CreateDirectory(Path.Combine(_root, "Originale")).FullName;
        _original = Path.Combine(originals, OriginalName);
        File.WriteAllText(_original, "unchanged original");
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private static CutlistDocument Document()
    {
        var context = NameTemplateContextFactory.Create(OriginalName);
        var naming = NamingSettings.CreateDefault();
        var output = new CutOutputViewModel(new CutNamingState(naming.DefaultNameTemplate, context))
        {
            OutputMovieName = "Manuell geänderter Filmname [01.01.2000]"
        };
        var generation = new CutlistGenerationViewModel(new CutlistSettings { DefaultAuthor = "Tester" },
            output.CreateNamingState())
        { SelectedRating = 4 };
        var plan = new CutPlan(TimeSpan.FromSeconds(60));
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20)));
        var analysis = new MediaAnalysisResult("mp4", "mp4", 123456, TimeSpan.FromSeconds(60),
            [new VideoStreamInfo(0, "h264", null, 720, 576, null, null, 25, null)], []);
        return generation.CreateDocument(plan, OriginalName, "0.2.1", analysis);
    }

    [Fact]
    public void SavesWithoutCutEngineUsingOriginalNameAndUnchangedDocument()
    {
        var document = Document();
        var target = CutlistOutputTargetResolver.Resolve(_cutlists, _original);
        Assert.Equal(Target, target.FilePath);
        Assert.False(target.Exists);
        Assert.Empty(Directory.GetFileSystemEntries(_cutlists));
        CutlistFileWriter.WriteSafely(target.FilePath, document);
        Assert.Equal(CutlistFileWriter.CreateBytes(document), File.ReadAllBytes(Target));
        Assert.Equal(OriginalName, document.General.ApplyToFile);
        Assert.DoesNotContain("Manuell", document.Info.SuggestedMovieName);
        Assert.Equal("unchanged original", File.ReadAllText(_original));
        Assert.Single(Directory.GetFileSystemEntries(_cutlists));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("relative")]
    public void RejectsMissingConfigurationAndDoesNotFallBackToOriginalFolder(string? configured)
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => CutlistOutputTargetResolver.Resolve(configured, _original));
        Assert.Equal(CutlistOutputTargetResolver.DirectoryHelp, exception.Message);
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(_original)!));
        Assert.Empty(Directory.GetFileSystemEntries(_cutlists));
    }

    [Fact]
    public void RejectsMissingFolderWithoutCreatingIt()
    {
        var missing = Path.Combine(_root, "not created");
        Assert.Throws<InvalidOperationException>(() => CutlistOutputTargetResolver.Resolve(missing, OriginalName));
        Assert.False(Directory.Exists(missing));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(_original)!));
    }

    [Fact]
    public void PreservesExistingCutlistWhenConsentIsRefused()
    {
        File.WriteAllText(Target, "existing");
        var target = CutlistOutputTargetResolver.Resolve(_cutlists, OriginalName);
        Assert.True(target.Exists);
        Assert.Throws<IOException>(() => CutlistFileWriter.WriteSafely(target.FilePath, Document()));
        Assert.Equal("existing", File.ReadAllText(Target));
        Assert.Single(Directory.GetFileSystemEntries(_cutlists));
    }

    [Fact]
    public void LateConflictAfterDestinationCheckCannotBeOverwritten()
    {
        var target = CutlistOutputTargetResolver.Resolve(_cutlists, OriginalName);
        Assert.False(target.Exists);
        File.WriteAllText(Target, "created after check");
        Assert.Throws<IOException>(() => CutlistFileWriter.WriteSafely(target.FilePath, Document()));
        Assert.Equal("created after check", File.ReadAllText(Target));
        Assert.Single(Directory.GetFileSystemEntries(_cutlists));
    }

    [Fact]
    public void ConfirmedReplacementPreservesOriginalAndLeavesNoWorkFiles()
    {
        File.WriteAllText(Target, "existing");
        var target = CutlistOutputTargetResolver.Resolve(_cutlists, OriginalName);
        Assert.True(target.Exists);
        var document = Document();
        CutlistFileWriter.WriteSafely(target.FilePath, document, overwriteExisting: true);
        Assert.Equal(CutlistFileWriter.CreateBytes(document), File.ReadAllBytes(Target));
        Assert.Equal("unchanged original", File.ReadAllText(_original));
        Assert.Single(Directory.GetFileSystemEntries(_cutlists));
    }

    [Fact]
    public void RejectsReadOnlyCutlistWithoutChangingIt()
    {
        File.WriteAllText(Target, "read-only existing");
        File.SetAttributes(Target, FileAttributes.ReadOnly);
        try
        {
            Assert.Throws<IOException>(() => CutlistOutputTargetResolver.Resolve(_cutlists, OriginalName));
            Assert.Equal("read-only existing", File.ReadAllText(Target));
            Assert.Single(Directory.GetFileSystemEntries(_cutlists));
        }
        finally
        {
            File.SetAttributes(Target, FileAttributes.Normal);
        }
    }

    [Fact]
    public void LockedExistingFileSurvivesPublicationFailureAndTemporaryFileIsRemoved()
    {
        File.WriteAllText(Target, "locked existing");
        using (var locked = new FileStream(Target, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var exception = Record.Exception(() =>
                CutlistFileWriter.WriteSafely(Target, Document(), overwriteExisting: true));
            Assert.True(exception is IOException or UnauthorizedAccessException,
                $"Expected a filesystem refusal, got: {exception}");
        }

        Assert.Equal("locked existing", File.ReadAllText(Target));
        Assert.Single(Directory.GetFileSystemEntries(_cutlists));
    }

    [Fact]
    public void RejectsUnwritableFolderAndRestoresOnlyItsChangedAccessPermissions()
    {
        var directory = new DirectoryInfo(_cutlists);
        var permissions = directory.GetAccessControl();
        var originalDescriptor = permissions.GetSecurityDescriptorBinaryForm();
        using var identity = WindowsIdentity.GetCurrent();
        permissions.AddAccessRule(new FileSystemAccessRule(identity.User!,
            FileSystemRights.CreateFiles, AccessControlType.Deny));
        directory.SetAccessControl(permissions);
        try
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => CutlistOutputTargetResolver.Resolve(_cutlists, OriginalName));
            Assert.Equal(CutlistOutputTargetResolver.DirectoryHelp, exception.Message);
            Assert.Equal("unchanged original", File.ReadAllText(_original));
            Assert.Empty(Directory.GetFileSystemEntries(_cutlists));
        }
        finally
        {
            var originalPermissions = new DirectorySecurity();
            originalPermissions.SetSecurityDescriptorBinaryForm(originalDescriptor, AccessControlSections.Access);
            directory.SetAccessControl(originalPermissions);
        }
    }

    [Fact]
    public void RejectsOverlongDerivedNameInsteadOfTruncatingOrRenamingIt()
    {
        Assert.Throws<ArgumentException>(() => CutlistOutputTargetResolver.Resolve(_cutlists, new string('a', 250)));
        Assert.Empty(Directory.GetFileSystemEntries(_cutlists));
    }

    [Fact]
    public void ExistingLoadDialogFallbackIsUnaffected()
    {
        var settings = new VideoFolderSettings { OwnCutlistsDirectory = string.Empty };
        Assert.Equal(Path.GetDirectoryName(_original),
            VideoFolderPathResolver.ResolveCutlists(settings, Path.GetDirectoryName(_original)));
        Assert.Throws<InvalidOperationException>(() =>
            CutlistOutputTargetResolver.Resolve(settings.OwnCutlistsDirectory, OriginalName));
    }
}
