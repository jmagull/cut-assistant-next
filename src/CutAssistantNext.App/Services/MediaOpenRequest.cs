using System.IO;

namespace CutAssistantNext.App.Services;

internal static class MediaOpenRequest
{
    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".avi", ".mkv", ".mov", ".ts", ".m2ts", ".mpg", ".mpeg", ".wmv", ".webm"
    };

    internal static Task<string?> FromStartupAsync(string[] arguments, string baseDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (arguments.Length == 0) return Task.FromResult<string?>(null);
        if (arguments.Length != 1)
            throw new ArgumentException("Bitte genau eine Videodatei als Startparameter übergeben.");
        return ResolveStartupAsync(arguments[0], baseDirectory, cancellationToken);
    }

    private static async Task<string?> ResolveStartupAsync(string path, string baseDirectory,
        CancellationToken cancellationToken) =>
        await ResolveAsync(path, baseDirectory, cancellationToken: cancellationToken);

    internal static Task<string> ResolveAsync(string path, string baseDirectory,
        bool requireVideoExtension = false, CancellationToken cancellationToken = default) => Task.Run(() =>
    {
        var fullPath = NormalizePath(path, baseDirectory);
        if (requireVideoExtension && !VideoExtensions.Contains(Path.GetExtension(fullPath)))
            throw new ArgumentException("Bitte eine unterstützte Videodatei in den Player ziehen.");
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Die Videodatei wurde nicht gefunden oder ist nicht zugänglich.", fullPath);
        cancellationToken.ThrowIfCancellationRequested();
        return fullPath;
    }, cancellationToken);

    internal static string NormalizePath(string path, string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Der Pfad zur Videodatei ist leer.");
        return Path.GetFullPath(path, baseDirectory);
    }

    internal static bool IsDropCandidate(string[] files) => files.Length == 1 &&
        !string.IsNullOrWhiteSpace(files[0]) && VideoExtensions.Contains(Path.GetExtension(files[0]));

    internal static bool NeedsReplacementConfirmation(int removeSegmentCount, bool hasPendingStart,
        bool hasLoadedCutlist) => removeSegmentCount > 0 || hasPendingStart || hasLoadedCutlist;
}
