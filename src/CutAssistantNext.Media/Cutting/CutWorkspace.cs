using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Media.Cutting;

/// <summary>Owns only a newly created private job directory; never the selected root.</summary>
public sealed class CutWorkspace : IDisposable
{
    private readonly string _root;
    private readonly IProgress<CutProgressUpdate>? _progress;
    public string DirectoryPath { get; }

    private CutWorkspace(string root, string directory, IProgress<CutProgressUpdate>? progress)
    {
        _root = root;
        DirectoryPath = directory;
        _progress = progress;
    }

    public static CutWorkspace Create(string outputPath, string? workingDirectory,
        IProgress<CutProgressUpdate>? progress)
    {
        var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(workingDirectory ?? Path.GetDirectoryName(Path.GetFullPath(outputPath))!));
        progress?.Report(new(CutProgressKind.Output, $"Gewählter Arbeitsordner: {root}\nZielverzeichnis: {Path.GetDirectoryName(outputPath)}"));
        ValidateWritable(root, progress);
        ValidateWritable(Path.GetDirectoryName(Path.GetFullPath(outputPath))!, progress);
        var directory = Path.Combine(root, ".can-work-" + Guid.NewGuid().ToString("N"));
        if (Path.Exists(directory)) throw new IOException("Der private Arbeitsbereich existiert bereits.");
        Directory.CreateDirectory(directory);
        progress?.Report(new(CutProgressKind.Output, "Tatsächlicher temporärer Arbeitsordner: " + directory));
        return new(root, directory, progress);
    }

    public static void ValidateWritable(string directory, IProgress<CutProgressUpdate>? progress = null)
    {
        var probe = Path.Combine(directory, ".can-write-check-" + Guid.NewGuid().ToString("N"));
        try
        {
            if (!Path.IsPathFullyQualified(directory) || !Directory.Exists(directory))
                throw new DirectoryNotFoundException("Der Arbeits- oder Zielordner ist nicht erreichbar: " + directory);
            using var file = new FileStream(probe, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                1, FileOptions.DeleteOnClose);
            file.WriteByte(0);
            file.Flush(flushToDisk: true);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            progress?.Report(new(CutProgressKind.Output, $"Festgestellter Fehler bei der Schreibprüfung: {directory}\n{error}"));
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            if (!Directory.Exists(DirectoryPath)) return;
            if (!string.Equals(Path.GetDirectoryName(DirectoryPath), _root, StringComparison.OrdinalIgnoreCase) ||
                new DirectoryInfo(DirectoryPath).LinkTarget is not null ||
                Directory.EnumerateFileSystemEntries(DirectoryPath, "*", new EnumerationOptions
                { RecurseSubdirectories = true, AttributesToSkip = 0, IgnoreInaccessible = false })
                    .Any(path => (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0))
                throw new IOException("Der private Arbeitsbereich enthält eine Verknüpfung oder hat sich verändert.");
            Directory.Delete(DirectoryPath, recursive: true);
            _progress?.Report(new(CutProgressKind.Output, "Temporärer Arbeitsordner bereinigt: " + DirectoryPath));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            _progress?.Report(new(CutProgressKind.Output, $"Bereinigungsfehler; bitte diesen Arbeitsordner prüfen: {DirectoryPath}\n{error}"));
        }
    }
}
