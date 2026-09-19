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
