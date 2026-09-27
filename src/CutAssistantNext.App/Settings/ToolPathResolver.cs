using System.IO;

namespace CutAssistantNext.App.Settings;

internal enum BundledToolKind
{
    Ffprobe,
    Ffmpeg,
    Mp4Box
}

internal sealed class ToolPathResolver
{
    private readonly string _applicationDirectory;
    private readonly Func<string?> _findInstalledMp4Box;

    internal ToolPathResolver()
        : this(
            AppContext.BaseDirectory,
            GpacInstallationLocator.FindMp4BoxPath)
    {
    }

    internal ToolPathResolver(string applicationDirectory)
        : this(applicationDirectory, () => null)
    {
    }

    internal ToolPathResolver(
        string applicationDirectory,
        Func<string?> findInstalledMp4Box)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            applicationDirectory);

        ArgumentNullException.ThrowIfNull(
            findInstalledMp4Box);

        _applicationDirectory =
            Path.GetFullPath(applicationDirectory);

        _findInstalledMp4Box =
            findInstalledMp4Box;
    }

    internal string GetBundledPath(BundledToolKind tool)
    {
        return tool switch
        {
            BundledToolKind.Ffprobe =>
                Path.Combine(
                    _applicationDirectory,
                    "tools",
                    "ffmpeg",
                    "bin",
                    "ffprobe.exe"),

            BundledToolKind.Ffmpeg =>
                Path.Combine(
                    _applicationDirectory,
                    "tools",
                    "ffmpeg",
                    "bin",
                    "ffmpeg.exe"),

            BundledToolKind.Mp4Box =>
                Path.Combine(
                    _applicationDirectory,
                    "tools",
                    "mp4box",
                    "MP4Box.exe"),

            _ => throw new ArgumentOutOfRangeException(
                nameof(tool))
        };
    }

    internal string Resolve(
        BundledToolKind tool,
        string? configuredPath)
    {
        var resolvedPath = TryResolve(
            tool,
            configuredPath);

        if (resolvedPath is not null)
        {
            return resolvedPath;
        }

        if (tool == BundledToolKind.Mp4Box)
        {
            throw new InvalidOperationException(
                "MP4Box wurde nicht gefunden (MP4Box.exe). Bitte GPAC installieren " +
                "oder den Programmpfad unter Einstellungen " +
                "→ Schnittanwendung eintragen.");
        }

            throw new InvalidOperationException(
                "Das notwendige Werkzeug wurde nicht gefunden: " +
               (tool == BundledToolKind.Ffprobe ? "ffprobe.exe" : "ffmpeg.exe") +
               ". Bitte das Werkzeug selbst installieren und den vollständigen " +
               "Programmpfad unter Einstellungen → FFmpeg-Werkzeuge eintragen.");
    }

    internal string? TryResolve(
        BundledToolKind tool,
        string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return configuredPath;
        }

        var bundledPath = GetBundledPath(tool);

        if (File.Exists(bundledPath))
        {
            return bundledPath;
        }

        if (tool != BundledToolKind.Mp4Box)
        {
            return null;
        }

        var installedPath = _findInstalledMp4Box();

        return !string.IsNullOrWhiteSpace(installedPath) &&
               File.Exists(installedPath)
            ? installedPath
            : null;
    }
}
