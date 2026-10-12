using System.IO;

namespace CutAssistantNext.App.Settings;

/// <summary>Strict destination validation, separate from file-picker fallback behavior.</summary>
internal static class ConfiguredOutputDirectory
{
    internal static string Require(string? configuredDirectory, string errorMessage)
    {
        string directory;
        try
        {
            if (string.IsNullOrWhiteSpace(configuredDirectory) ||
                !Path.IsPathFullyQualified(configuredDirectory.Trim()))
            {
                throw new InvalidOperationException(errorMessage);
            }

            directory = Path.GetFullPath(configuredDirectory.Trim());
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new InvalidOperationException(errorMessage, exception);
        }

        if (!Directory.Exists(directory))
        {
            throw new InvalidOperationException(errorMessage);
        }

        return directory;
    }

    internal static void ProbeWriteAccess(string directory)
    {
        // Never open a destination or original for this probe, and never create a directory.
        var probePath = Path.Combine(directory, $".can-write-check-{Guid.NewGuid():N}.tmp");
        using var probe = new FileStream(
            probePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose);
        probe.WriteByte(0);
        probe.Flush();
    }
}
