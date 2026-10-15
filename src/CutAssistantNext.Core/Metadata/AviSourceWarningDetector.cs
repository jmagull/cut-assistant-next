using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Core.Metadata;

/// <summary>Describes source risks without marking a cutlist or media analysis as failed.</summary>
public static class AviSourceWarningDetector
{
    public const string FrameLoupeUnavailableMessage = "Die Frame-Lupe steht für AVI-Container derzeit nicht zur Verfügung.";

    public static bool IsAvi(MediaAnalysisResult analysis)
    {
        ArgumentNullException.ThrowIfNull(analysis);
        return (analysis.FormatName ?? string.Empty).Split(
            ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Contains("avi", StringComparer.OrdinalIgnoreCase);
    }

    public static string? Detect(string fileName, MediaAnalysisResult analysis)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(analysis);

        if (!IsAvi(analysis))
        {
            return null;
        }

        var extension = Path.GetExtension(fileName);
        var containerNotice = !string.IsNullOrEmpty(extension) &&
            !extension.Equals(".avi", StringComparison.OrdinalIgnoreCase)
                ? $"Die Datei endet auf {extension}, enthält aber einen AVI-Container. "
                : "Diese Datei verwendet den AVI-Container. ";

        return containerNotice +
            "Bei älteren AVI-Aufnahmen können Bildfehler, ungenaue Bildsprünge oder Tonversatz " +
            "bereits im Original auftreten. Bitte prüfe die Originalwiedergabe und das " +
            "Schnittergebnis. Die Bearbeitung bleibt möglich. " +
            "Die Keyframe-Anzeige und Keyframe-Navigation im Hauptfenster sind für AVI nicht verfügbar. " +
            "Die Halbierungssuche steht für AVI-Container derzeit nicht zur Verfügung. " +
            FrameLoupeUnavailableMessage;
    }
}
