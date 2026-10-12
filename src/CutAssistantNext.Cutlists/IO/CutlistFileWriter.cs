using System.Text;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.IO;

public static class CutlistFileWriter
{
    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    public static void Write(
        string filePath,
        CutlistDocument document)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            filePath);

        ArgumentNullException.ThrowIfNull(document);

        var fullPath =
            Path.GetFullPath(filePath);

        var directoryPath =
            Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(
                directoryPath);
        }

        var bytes =
            CreateBytes(
                document);

        File.WriteAllBytes(
            fullPath,
            bytes);
    }

    /// <summary>Publishes a complete cutlist in an existing directory, with explicit overwrite consent.</summary>
    public static void WriteSafely(
        string filePath,
        CutlistDocument document,
        bool overwriteExisting = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(document);
        if (!Path.IsPathFullyQualified(filePath))
        {
            throw new ArgumentException("Ein vollständiger Cutlist-Dateipfad ist erforderlich.", nameof(filePath));
        }

        var fullPath = Path.GetFullPath(filePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException("Das Cutlist-Verzeichnis ist nicht erreichbar.");
        }

        var bytes = CreateBytes(document);
        var existed = File.Exists(fullPath);
        if (existed && !overwriteExisting)
        {
            throw new IOException("Die Cutlist existiert bereits und wurde nicht überschrieben.");
        }

        var temporaryPath = Path.Combine(directory, $".can-cutlist-{Guid.NewGuid():N}.tmp");
        var temporaryCreated = false;
        try
        {
            using (var stream = new FileStream(
                       temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                temporaryCreated = true;
                stream.Write(bytes);
                stream.Flush(flushToDisk: true);
            }

            // Same-directory move publishes completed bytes; an unapproved late target cannot be replaced.
            File.Move(temporaryPath, fullPath, overwrite: existed && overwriteExisting);
            temporaryCreated = false;
        }
        finally
        {
            if (temporaryCreated)
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public static byte[] CreateBytes(
        CutlistDocument document)
    {
        ArgumentNullException.ThrowIfNull(
            document);

        var content =
            CutlistSerializer.Serialize(
                document);

        content =
            NormalizeToCrLf(
                content);

        return Utf8WithoutBom.GetBytes(
            content);
    }

    private static string NormalizeToCrLf(
        string value)
    {
        return value
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\n", "\r\n");
    }
}
