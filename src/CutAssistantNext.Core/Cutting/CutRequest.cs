namespace CutAssistantNext.Core.Cutting;

/// <summary>An immutable cut snapshot; naming and file preparation remain with the caller.</summary>
public sealed class CutRequest
{
    public CutRequest(
        string originalFilePath,
        string outputFilePath,
        IEnumerable<KeepSegment> keepSegments,
        double? framesPerSecond = null,
        bool overwriteExistingOutput = false,
        string? sourceFilePath = null)
    {
        ArgumentNullException.ThrowIfNull(keepSegments);

        OriginalFilePath = GetAbsolutePath(originalFilePath, nameof(originalFilePath));
        OutputFilePath = GetAbsolutePath(outputFilePath, nameof(outputFilePath));
        SourceFilePath = sourceFilePath is null
            ? OriginalFilePath
            : GetAbsolutePath(sourceFilePath, nameof(sourceFilePath));

        if (string.Equals(OriginalFilePath, OutputFilePath, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(SourceFilePath, OutputFilePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Die Ausgabedatei darf weder Original noch Arbeitsdatei ersetzen.", nameof(outputFilePath));
        }

        if (framesPerSecond.HasValue &&
            (!double.IsFinite(framesPerSecond.Value) || framesPerSecond.Value <= 0))
        {
            throw new ArgumentOutOfRangeException(nameof(framesPerSecond));
        }

        var segments = keepSegments.ToArray();
        if (segments.Length == 0)
        {
            throw new ArgumentException("Mindestens ein Behaltebereich ist erforderlich.", nameof(keepSegments));
        }

        KeepSegment? previous = null;
        foreach (var segment in segments)
        {
            ArgumentNullException.ThrowIfNull(segment);
            if (previous is not null && segment.Start < previous.End)
            {
                throw new ArgumentException("Behaltebereiche müssen geordnet sein und dürfen sich nicht überschneiden.", nameof(keepSegments));
            }

            previous = segment;
        }

        KeepSegments = Array.AsReadOnly(segments);
        FramesPerSecond = framesPerSecond;
        OverwriteExistingOutput = overwriteExistingOutput;
    }

    public string OriginalFilePath { get; }

    public string SourceFilePath { get; }

    public string OutputFilePath { get; }

    public IReadOnlyList<KeepSegment> KeepSegments { get; }

    public double? FramesPerSecond { get; }

    public bool OverwriteExistingOutput { get; }

    /// <summary>Select a prepared working file without losing the original identity or cut times.</summary>
    public CutRequest WithSourceFilePath(string sourceFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFilePath);

        return new CutRequest(
            OriginalFilePath,
            OutputFilePath,
            KeepSegments,
            FramesPerSecond,
            OverwriteExistingOutput,
            sourceFilePath);
    }

    private static string GetAbsolutePath(string path, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, parameterName);
        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("Ein vollständiger Dateipfad ist erforderlich.", parameterName);
        }

        return Path.GetFullPath(path);
    }
}
