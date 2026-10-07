using System.IO;

namespace CutAssistantNext.App.Settings;

internal static class VideoFolderPathResolver
{
    internal static string Resolve(string? directory, Func<string, bool>? directoryExists = null)
    {
        var fullPath = GetFullPath(directory);
        return fullPath is not null && (directoryExists ?? Directory.Exists)(fullPath)
            ? fullPath
            : string.Empty;
    }

    internal static string ResolveCutlists(VideoFolderSettings settings, string? fallbackDirectory = null,
        Func<string, bool>? directoryExists = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var configured = Resolve(settings.OwnCutlistsDirectory, directoryExists);
        return configured.Length > 0 ? configured : Resolve(fallbackDirectory, directoryExists);
    }

    internal static string? GetValidationError(VideoFolderSettings settings,
        Func<string, bool>? directoryExists = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return Validate(settings.OriginalVideosDirectory, "Originalvideos", directoryExists)
            ?? Validate(settings.CutVideosDirectory, "Geschnittene Videos", directoryExists)
            ?? Validate(settings.OwnCutlistsDirectory, "Eigene Cutlists", directoryExists);
    }

    private static string? Validate(string? directory, string label, Func<string, bool>? directoryExists)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return null;
        }

        var fullPath = GetFullPath(directory);
        if (fullPath is null)
        {
            return $"{label}: Bitte einen vollständigen Ordnerpfad angeben oder das Feld leeren.";
        }

        return (directoryExists ?? Directory.Exists)(fullPath)
            ? null
            : $"{label}: Der Ordner ist nicht verfügbar. Bitte einen vorhandenen Ordner wählen oder das Feld leeren.";
    }

    private static string? GetFullPath(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return null;
        }

        try
        {
            var trimmed = directory.Trim();
            return Path.IsPathFullyQualified(trimmed) ? Path.GetFullPath(trimmed) : null;
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return null;
        }
    }
}
