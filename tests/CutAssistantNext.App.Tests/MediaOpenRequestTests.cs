using System.IO;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests;

public sealed class MediaOpenRequestTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-open-{Guid.NewGuid():N}");

    public MediaOpenRequestTests() => Directory.CreateDirectory(_directory);

    [Fact]
    public async Task NoArgumentsDoesNotRequestAFile() =>
        Assert.Null(await MediaOpenRequest.FromStartupAsync([], _directory));

    [Theory]
    [InlineData("Film mit Leerzeichen.mp4")]
    [InlineData("ÄÖÜ 日本語 & Sonderzeichen #1.avi.mp4")]
    [InlineData("ohne-bekannte-endung.bin")]
    public async Task StartupResolvesRelativeAndAbsolutePathsWithoutChangingTheName(string name)
    {
        var path = Path.Combine(_directory, name);
        await File.WriteAllTextAsync(path, "fixture");
        Assert.Equal(path, await MediaOpenRequest.FromStartupAsync([name], _directory));
        Assert.Equal(path, await MediaOpenRequest.FromStartupAsync([path], Path.GetTempPath()));
    }

    [Fact]
    public async Task RelativeParentDirectoryIsResolved()
    {
        var child = Directory.CreateDirectory(Path.Combine(_directory, "child"));
        var path = Path.Combine(_directory, "Film.mp4");
        await File.WriteAllTextAsync(path, "fixture");
        Assert.Equal(path, await MediaOpenRequest.FromStartupAsync(["../Film.mp4"], child.FullName));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task MultipleArgumentsAreRejected(int count) =>
        await Assert.ThrowsAsync<ArgumentException>(() => MediaOpenRequest.FromStartupAsync(
            Enumerable.Repeat("Film.mp4", count).ToArray(), _directory));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\0.mp4")]
    public async Task InvalidPathsAreRejected(string path) =>
        await Assert.ThrowsAnyAsync<ArgumentException>(() => MediaOpenRequest.FromStartupAsync([path], _directory));

    [Fact]
    public async Task MissingFileIsReported() =>
        await Assert.ThrowsAsync<FileNotFoundException>(() => MediaOpenRequest.FromStartupAsync(["missing.mp4"], _directory));

    [Fact]
    public async Task DirectoryIsRejectedEvenWithVideoExtension()
    {
        var directory = Directory.CreateDirectory(Path.Combine(_directory, "folder.mp4"));
        await Assert.ThrowsAsync<FileNotFoundException>(() => MediaOpenRequest.ResolveAsync(
            directory.FullName, _directory, requireVideoExtension: true));
    }

    [Fact]
    public async Task DropRejectsUnsupportedExtensionButFileDialogCanStillOpenIt()
    {
        var path = Path.Combine(_directory, "file.txt");
        await File.WriteAllTextAsync(path, "fixture");
        await Assert.ThrowsAsync<ArgumentException>(() => MediaOpenRequest.ResolveAsync(path, _directory, true));
        Assert.Equal(path, await MediaOpenRequest.ResolveAsync(path, _directory));
    }

    [Fact]
    public async Task CancelledValidationDoesNotOpenAFile()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => MediaOpenRequest.FromStartupAsync(
            ["Film.mp4"], _directory, cancellation.Token));
    }

    [Theory]
    [InlineData("film.MP4", true)]
    [InlineData("film.avi.mp4", true)]
    [InlineData("film.webm", true)]
    [InlineData("film.txt", false)]
    [InlineData("", false)]
    public void DropCandidateUsesOneKnownExtension(string path, bool expected) =>
        Assert.Equal(expected, MediaOpenRequest.IsDropCandidate([path]));

    [Fact]
    public void EmptyAndMultipleDropsAreRejected()
    {
        Assert.False(MediaOpenRequest.IsDropCandidate([]));
        Assert.False(MediaOpenRequest.IsDropCandidate(["one.mp4", "two.mp4"]));
    }

    [Theory]
    [InlineData(0, false, false, false)]
    [InlineData(1, false, false, true)]
    [InlineData(0, true, false, true)]
    [InlineData(0, false, true, true)]
    [InlineData(3, true, true, true)]
    public void ExistingWorkRequiresConfirmation(int segments, bool pending, bool cutlist, bool expected) =>
        Assert.Equal(expected, MediaOpenRequest.NeedsReplacementConfirmation(segments, pending, cutlist));

    [Fact]
    public void UncPathIsPreservedWithoutAccessingANetworkShare()
    {
        const string path = @"\\server\Freigabe mit Leerzeichen\Filme\Ärger.mp4";
        Assert.Equal(path, MediaOpenRequest.NormalizePath(path, _directory));
    }

    public void Dispose() => Directory.Delete(_directory, recursive: true);
}
