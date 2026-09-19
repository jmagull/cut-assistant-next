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

    internal ToolPathResolver()
        : this(AppContext.BaseDirectory)
    {
    }

    internal ToolPathResolver(string applicationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            applicationDirectory);

        _applicationDirectory =
            Path.GetFullPath(applicationDirectory);
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
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return configuredPath;
        }

        var bundledPath = GetBundledPath(tool);

        if (!File.Exists(bundledPath))
        {
            throw new InvalidOperationException(
                "Das mitgelieferte Werkzeug wurde nicht gefunden: " +
                bundledPath);
        }

        return bundledPath;
    }
}
