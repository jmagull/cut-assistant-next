using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class ToolPathResolverTests
{
    [Theory]
    [InlineData("Ffprobe", "ffprobe.exe")]
    [InlineData("Ffmpeg", "ffmpeg.exe")]
    [InlineData("Mp4Box", "MP4Box.exe")]
    public void Resolve_UsesBundledToolWhenConfigurationIsEmpty(
        string toolName,
        string fileName)
    {
        var tool = Enum.Parse<BundledToolKind>(toolName);

        var root = CreateTempDirectory();

        try
        {
            var resolver = new ToolPathResolver(root);
            var bundledPath = resolver.GetBundledPath(tool);

            Directory.CreateDirectory(
                Path.GetDirectoryName(bundledPath)!);

            File.WriteAllText(bundledPath, "");

            Assert.Equal(
                fileName,
                Path.GetFileName(bundledPath));

            Assert.Equal(
                bundledPath,
                resolver.Resolve(tool, ""));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Resolve_PrefersCustomPath()
    {
        var resolver = new ToolPathResolver(
            @"C:\CAN");

        var customPath =
            @"D:\MeineTools\ffmpeg.exe";

        Assert.Equal(
            customPath,
            resolver.Resolve(
                BundledToolKind.Ffmpeg,
                customPath));
    }

    [Fact]
    public void Resolve_RejectsMissingBundledTool()
    {
        var root = CreateTempDirectory();

        try
        {
            var resolver = new ToolPathResolver(root);

            Assert.Throws<InvalidOperationException>(
                () => resolver.Resolve(
                    BundledToolKind.Mp4Box,
                    ""));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

[Theory]
[InlineData("Ffprobe", "ffprobe.exe")]
[InlineData("Ffmpeg", "ffmpeg.exe")]
public void Resolve_MissingFfmpegTool_ExplainsManualConfiguration(
    string toolName,
    string fileName)
{
    var root = CreateTempDirectory();

    try
    {
        var resolver = new ToolPathResolver(root);
        var tool = Enum.Parse<BundledToolKind>(toolName);

        var error = Assert.Throws<InvalidOperationException>(
            () => resolver.Resolve(tool, ""));

        Assert.Contains(fileName, error.Message);
        Assert.Contains("FFmpeg-Werkzeuge", error.Message);
        Assert.DoesNotContain(root, error.Message);
    }
    finally
    {
        Directory.Delete(root, true);
    }
}

    [Fact]
    public void Resolve_FollowsMovedApplicationDirectory()
    {
        var root = CreateTempDirectory();

        try
        {
            var originalDirectory =
                Path.Combine(root, "Original");

            var movedDirectory =
                Path.Combine(root, "tolle Software");

            Directory.CreateDirectory(originalDirectory);

            var originalResolver =
                new ToolPathResolver(originalDirectory);

            var originalTool =
                originalResolver.GetBundledPath(
                    BundledToolKind.Ffmpeg);

            Directory.CreateDirectory(
                Path.GetDirectoryName(originalTool)!);

            File.WriteAllText(originalTool, "");

            Directory.Move(
                originalDirectory,
                movedDirectory);

            var movedResolver =
                new ToolPathResolver(movedDirectory);

            var actualPath =
                movedResolver.Resolve(
                    BundledToolKind.Ffmpeg,
                    "");

            Assert.Equal(
                Path.Combine(
                    movedDirectory,
                    "tools",
                    "ffmpeg",
                    "bin",
                    "ffmpeg.exe"),
                actualPath);

            Assert.True(File.Exists(actualPath));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Resolve_UsesInstalledMp4BoxWhenBundledToolIsMissing()
    {
        var root = CreateTempDirectory();

        try
        {
            var installedDirectory = Path.Combine(
                root,
                "Installed GPAC");

            Directory.CreateDirectory(installedDirectory);

            var installedPath = Path.Combine(
                installedDirectory,
                "MP4Box.exe");

            File.WriteAllText(installedPath, string.Empty);

            var resolver = new ToolPathResolver(
                root,
                () => installedPath);

            var actual = resolver.Resolve(
                BundledToolKind.Mp4Box,
                string.Empty);

            Assert.Equal(installedPath, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Resolve_PrefersCustomMp4BoxWithoutRegistryLookup()
    {
        var root = CreateTempDirectory();

        try
        {
            var resolver = new ToolPathResolver(
                root,
                () => throw new InvalidOperationException(
                    "Die Registry darf nicht abgefragt werden."));

            var customPath = @"D:\MeineTools\MP4Box.exe";

            var actual = resolver.Resolve(
                BundledToolKind.Mp4Box,
                customPath);

            Assert.Equal(customPath, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Resolve_PrefersBundledMp4BoxWithoutRegistryLookup()
    {
        var root = CreateTempDirectory();

        try
        {
            var resolver = new ToolPathResolver(
                root,
                () => throw new InvalidOperationException(
                    "Die Registry darf nicht abgefragt werden."));

            var bundledPath = resolver.GetBundledPath(
                BundledToolKind.Mp4Box);

            Directory.CreateDirectory(
                Path.GetDirectoryName(bundledPath)!);

            File.WriteAllText(bundledPath, string.Empty);

            var actual = resolver.Resolve(
                BundledToolKind.Mp4Box,
                string.Empty);

            Assert.Equal(bundledPath, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            "can-resolver-tests-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(path);

        return path;
    }
}
