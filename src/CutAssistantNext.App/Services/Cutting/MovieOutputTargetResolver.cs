using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed record MovieOutputTarget(string FilePath, bool Exists);

/// <summary>Resolves a movie destination using only the explicitly configured output directory.</summary>
internal static class MovieOutputTargetResolver
{
    private const string DirectoryHelp =
        "Das Standardverzeichnis für geschnittene Filme ist nicht eingerichtet oder nicht nutzbar. " +
        "Bitte überprüfe den Pfad unter Einstellungen → Standardordner.";

    internal static MovieOutputTarget Resolve(
        string? configuredDirectory,
        string movieName,
        string originalFilePath)
    {
        var fileName = OutputFileNameBuilder.BuildExactMovieFileName(movieName);
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFilePath);

        var directory = ConfiguredOutputDirectory.Require(configuredDirectory, DirectoryHelp);

        var outputPath = Path.Combine(directory, fileName);
        if (string.Equals(outputPath, Path.GetFullPath(originalFilePath), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Die Ausgabedatei darf nicht die Originaldatei überschreiben.");
        }

        bool exists;
        try
        {
            try
            {
                var attributes = File.GetAttributes(outputPath);
                if ((attributes & FileAttributes.Directory) != 0)
                {
                    throw new IOException("Unter diesem Ausgabedateinamen existiert bereits ein Ordner.");
                }

                if ((attributes & FileAttributes.ReadOnly) != 0)
                {
                    throw new IOException("Die vorhandene Ausgabedatei ist schreibgeschützt. Bitte einen anderen Namen wählen.");
                }

                exists = true;
            }
            catch (FileNotFoundException)
            {
                exists = false;
            }

            ConfiguredOutputDirectory.ProbeWriteAccess(directory);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new InvalidOperationException(DirectoryHelp, exception);
        }
        catch (IOException exception)
        {
            throw new IOException(
                $"Der Ausgabeort kann nicht verwendet werden: {exception.Message} " +
                "Bitte überprüfe den Dateinamen und den Pfad unter Einstellungen → Standardordner.",
                exception);
        }

        return new MovieOutputTarget(outputPath, exists);
    }
}
