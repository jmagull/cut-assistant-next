using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class GpacInstallationLocatorTests
{
    [Fact]
    public void FindMp4BoxPath_UsesUninstallStringWithSpaces()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-gpac-locator-" +
            Guid.NewGuid().ToString("N"));

        var gpacDirectory = Path.Combine(
            root,
            "Program Files",
            "GPAC");

        Directory.CreateDirectory(gpacDirectory);

        try
        {
            var mp4BoxPath = Path.Combine(
                gpacDirectory,
                "MP4Box.exe");

            File.WriteAllText(
                mp4BoxPath,
                string.Empty);

            var uninstallString = Path.Combine(
                gpacDirectory,
                "uninstall.exe");

            var actual =
                GpacInstallationLocator.FindMp4BoxPath(
                    null,
                    uninstallString);

            Assert.Equal(mp4BoxPath, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void FindMp4BoxPath_PrefersInstallLocation()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-gpac-location-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var mp4BoxPath = Path.Combine(
                root,
                "MP4Box.exe");

            File.WriteAllText(mp4BoxPath, string.Empty);

            var actual =
                GpacInstallationLocator.FindMp4BoxPath(
                    root,
                    @"C:\NichtVorhanden\uninstall.exe");

            Assert.Equal(mp4BoxPath, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void FindMp4BoxPath_AcceptsQuotedUninstallString()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-gpac-quoted-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var mp4BoxPath = Path.Combine(
                root,
                "MP4Box.exe");

            File.WriteAllText(mp4BoxPath, string.Empty);

            var uninstallString =
                $"\"{Path.Combine(root, "uninstall.exe")}\" /S";

            var actual =
                GpacInstallationLocator.FindMp4BoxPath(
                    null,
                    uninstallString);

            Assert.Equal(mp4BoxPath, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void FindMp4BoxPath_ReturnsNullWhenExecutableIsMissing()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-gpac-missing-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var actual =
                GpacInstallationLocator.FindMp4BoxPath(
                    root,
                    Path.Combine(root, "uninstall.exe"));

            Assert.Null(actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}