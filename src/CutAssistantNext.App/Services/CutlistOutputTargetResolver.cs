using System.IO;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Cutlists.IO;

namespace CutAssistantNext.App.Services;

internal sealed record CutlistOutputTarget(string FilePath, bool Exists);

internal static class CutlistOutputTargetResolver
{
    internal const string DirectoryHelp =
        "Das Standardverzeichnis für eigene Cutlists ist nicht eingerichtet oder nicht erreichbar. " +
        "Bitte überprüfe den Pfad unter Einstellungen.";

    internal static CutlistOutputTarget Resolve(string? configuredDirectory, string originalFileName)
    {
        var directory = ConfiguredOutputDirectory.Require(configuredDirectory, DirectoryHelp);
        var fileName = CutlistFileName.FromOriginal(originalFileName);

        var outputPath = Path.Combine(directory, fileName);
        bool exists;
        try
        {
            try
            {
                var attributes = File.GetAttributes(outputPath);
                if ((attributes & FileAttributes.Directory) != 0)
                {
                    throw new IOException("Am vorgesehenen Cutlist-Pfad existiert bereits ein Ordner.");
                }

                if ((attributes & FileAttributes.ReadOnly) != 0)
                {
                    throw new IOException("Die vorhandene Cutlist ist schreibgeschützt und wurde nicht verändert.");
                }

                exists = true;
            }
            catch (FileNotFoundException)
            {
                exists = false;
            }
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new InvalidOperationException(DirectoryHelp, exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw new InvalidOperationException(DirectoryHelp, exception);
        }

        try
        {
            ConfiguredOutputDirectory.ProbeWriteAccess(directory);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new InvalidOperationException(DirectoryHelp, exception);
        }

        return new CutlistOutputTarget(outputPath, exists);
    }
}
