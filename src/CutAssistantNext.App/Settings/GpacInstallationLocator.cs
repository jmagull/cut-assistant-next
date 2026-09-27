using System.IO;
using Microsoft.Win32;

namespace CutAssistantNext.App.Settings;

internal static class GpacInstallationLocator
{
    private const string UninstallKey =
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GPAC";

    internal static string? FindMp4BoxPath()
    {
        foreach (var hive in new[]
                 {
                     RegistryHive.LocalMachine,
                     RegistryHive.CurrentUser
                 })
        {
            foreach (var view in new[]
                     {
                         RegistryView.Registry64,
                         RegistryView.Registry32
                     })
            {
                try
                {
                    using var root =
                        RegistryKey.OpenBaseKey(hive, view);

                    using var key =
                        root.OpenSubKey(UninstallKey);

                    if (key is null)
                    {
                        continue;
                    }

                    var path = FindMp4BoxPath(
                        key.GetValue("InstallLocation") as string,
                        key.GetValue("UninstallString") as string);

                    if (path is not null)
                    {
                        return path;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Eine nicht lesbare Registry-Ansicht
                    // darf die Anwendung nicht blockieren.
                }
                catch (System.Security.SecurityException)
                {
                    // Weitere Registry-Ansichten prüfen.
                }
            }
        }

        return null;
    }

    internal static string? FindMp4BoxPath(
        string? installLocation,
        string? uninstallString)
    {
        if (!string.IsNullOrWhiteSpace(installLocation))
        {
            var candidate = Path.Combine(
                installLocation.Trim().Trim('"'),
                "MP4Box.exe");

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        if (string.IsNullOrWhiteSpace(uninstallString))
        {
            return null;
        }

        var command = uninstallString.Trim();

        string uninstaller;

        if (command.StartsWith('"'))
        {
            var closingQuote = command.IndexOf('"', 1);

            if (closingQuote < 2)
            {
                return null;
            }

            uninstaller = command[1..closingQuote];
        }
        else
        {
            var extension = command.IndexOf(
                ".exe",
                StringComparison.OrdinalIgnoreCase);

            if (extension < 0)
            {
                return null;
            }

            uninstaller = command[..(extension + 4)];
        }

        if (!string.Equals(
                Path.GetFileName(uninstaller),
                "uninstall.exe",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var directory = Path.GetDirectoryName(uninstaller);

        if (string.IsNullOrWhiteSpace(directory))
        {
            return null;
        }

        var mp4BoxPath = Path.Combine(
            directory,
            "MP4Box.exe");

        return File.Exists(mp4BoxPath)
            ? mp4BoxPath
            : null;
    }
}
