using System.Security.AccessControl;
using System.Security.Principal;
using CutAssistantNext.App.Services.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class MovieOutputTargetResolverTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-output-target-{Guid.NewGuid():N}");
    private readonly string _output;
    private readonly string _original;

    public MovieOutputTargetResolverTests()
    {
        Directory.CreateDirectory(_root);
        _output = Directory.CreateDirectory(Path.Combine(_root, "Ausgaben")).FullName;
        var originals = Directory.CreateDirectory(Path.Combine(_root, "Originale")).FullName;
        _original = Path.Combine(originals, "original.HQ.mp4");
        File.WriteAllText(_original, "unchanged original");
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void UsesOnlyConfiguredFolderAndExactNameWithoutLeavingProbeFiles()
    {
        var target = MovieOutputTargetResolver.Resolve(_output, "Ghosts - 4x01 - Patience", _original);
        Assert.Equal(Path.Combine(_output, "Ghosts - 4x01 - Patience.mp4"), target.FilePath);
        Assert.False(target.Exists);
        Assert.Empty(Directory.GetFileSystemEntries(_output));
        Assert.Equal("unchanged original", File.ReadAllText(_original));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("relativer-ordner")]
    public void RejectsUnconfiguredOrRelativeFolderWithoutFallback(string? directory)
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => MovieOutputTargetResolver.Resolve(directory, "Film", _original));
        Assert.Contains("Einstellungen", exception.Message);
        Assert.Empty(Directory.GetFileSystemEntries(_output));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(_original)!));
    }

    [Fact]
    public void RejectsMissingFolderWithoutCreatingIt()
    {
        var missing = Path.Combine(_root, "NichtVorhanden");
        Assert.Throws<InvalidOperationException>(() => MovieOutputTargetResolver.Resolve(missing, "Film", _original));
        Assert.False(Directory.Exists(missing));
    }

    [Fact]
    public void DetectsExistingTargetWithoutTruncatingOrDeletingIt()
    {
        var path = Path.Combine(_output, "Film.mp4");
        File.WriteAllText(path, "existing result");
        var target = MovieOutputTargetResolver.Resolve(_output, "Film", _original);
        Assert.True(target.Exists);
        Assert.Equal(path, target.FilePath);
        Assert.Equal("existing result", File.ReadAllText(path));
        Assert.Single(Directory.GetFileSystemEntries(_output));
    }

    [Fact]
    public void ProtectsOriginalEvenWhenSameFolderIsConfigured()
    {
        var directory = Path.GetDirectoryName(_original)!;
        Assert.Throws<InvalidOperationException>(() =>
            MovieOutputTargetResolver.Resolve(directory, Path.GetFileName(_original).ToUpperInvariant(), _original));
        Assert.Equal("unchanged original", File.ReadAllText(_original));
        Assert.Single(Directory.GetFileSystemEntries(directory));
    }

    [Fact]
    public void RejectsTargetThatIsADirectoryWithoutChangingIt()
    {
        var occupied = Directory.CreateDirectory(Path.Combine(_output, "Film.mp4")).FullName;
        File.WriteAllText(Path.Combine(occupied, "preserved.txt"), "preserve");
        Assert.Throws<IOException>(() => MovieOutputTargetResolver.Resolve(_output, "Film", _original));
        Assert.Equal("preserve", File.ReadAllText(Path.Combine(occupied, "preserved.txt")));
    }

    [Fact]
    public void RejectsReadOnlyTargetBeforeStartingCutAndLeavesContentIntact()
    {
        var path = Path.Combine(_output, "Film.mp4");
        File.WriteAllText(path, "preserve");
        File.SetAttributes(path, FileAttributes.ReadOnly);
        try
        {
            Assert.Throws<IOException>(() => MovieOutputTargetResolver.Resolve(_output, "Film", _original));
            Assert.Equal("preserve", File.ReadAllText(path));
            Assert.Single(Directory.GetFileSystemEntries(_output));
        }
        finally
        {
            File.SetAttributes(path, FileAttributes.Normal);
        }
    }

    [Fact]
    public void RejectsUnwritableFolderWithSettingsHintAndNoProbeLeftBehind()
    {
        var directory = new DirectoryInfo(_output);
        var permissions = directory.GetAccessControl();
        var originalDescriptor = permissions.GetSecurityDescriptorBinaryForm();
        using var identity = WindowsIdentity.GetCurrent();
        permissions.AddAccessRule(new FileSystemAccessRule(identity.User!,
            FileSystemRights.CreateFiles, AccessControlType.Deny));
        directory.SetAccessControl(permissions);
        try
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => MovieOutputTargetResolver.Resolve(_output, "Film", _original));
            Assert.Contains("Einstellungen", exception.Message);
            Assert.Equal("unchanged original", File.ReadAllText(_original));
        }
        finally
        {
            var originalPermissions = new DirectorySecurity();
            originalPermissions.SetSecurityDescriptorBinaryForm(originalDescriptor, AccessControlSections.Access);
            directory.SetAccessControl(originalPermissions);
        }

        Assert.Empty(Directory.GetFileSystemEntries(_output));
    }
}
