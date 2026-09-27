namespace CutAssistantNext.Media.Cutting;

public sealed class Mp4BoxCutWorkflow
{
    private readonly IMp4BoxRunner _runner;

    public Mp4BoxCutWorkflow(
        IMp4BoxRunner runner)
    {
        _runner =
            runner
            ?? throw new ArgumentNullException(
                nameof(runner));
    }

    public Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        IReadOnlyList<Mp4BoxSplitRange> ranges,
        CancellationToken cancellationToken = default,
        bool overwriteExistingOutput = false)
    {
        return RunAsync(
            sourceFilePath,
            outputFilePath,
            ranges,
            progress: null,
            cancellationToken,
            overwriteExistingOutput);
    }

    public async Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        IReadOnlyList<Mp4BoxSplitRange> ranges,
        IProgress<Mp4BoxProgressUpdate>? progress,
        CancellationToken cancellationToken = default,
        bool overwriteExistingOutput = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            sourceFilePath);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            outputFilePath);

        ArgumentNullException.ThrowIfNull(
            ranges);

        if (ranges.Count == 0)
        {
            throw new ArgumentException(
                "Mindestens ein Schnittbereich ist erforderlich.",
                nameof(ranges));
        }

        var fullSourceFilePath =
            Path.GetFullPath(
                sourceFilePath);

        var fullOutputFilePath =
            Path.GetFullPath(
                outputFilePath);

        if (string.Equals(
            fullSourceFilePath,
            fullOutputFilePath,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Quell- und Zieldatei dürfen nicht identisch sein.",
                nameof(outputFilePath));
        }

        var outputFileExists =
            File.Exists(
                fullOutputFilePath);

        if (outputFileExists &&
            !overwriteExistingOutput)
        {
            throw new IOException(
                $"Die Zieldatei existiert bereits: {fullOutputFilePath}");
        }

        var outputDirectoryPath =
            Path.GetDirectoryName(
                fullOutputFilePath)
            ?? throw new InvalidOperationException(
                "Das Zielverzeichnis konnte nicht bestimmt werden.");


        var concatOutputFilePath =
            Path.Combine(
                outputDirectoryPath,
                $".cut-assistant-next-{Guid.NewGuid():N}-output.mp4");

        var segmentFilePaths =
            new List<string>(
                ranges.Count);

        try
        {
            for (var index = 0; index < ranges.Count; index++)
            {
                progress?.Report(
                    new Mp4BoxProgressUpdate(
                        Mp4BoxProgressKind.Status,
                        $"Segment {index + 1} von {ranges.Count} wird geschnitten …"));

                var segmentFilePath =
                    Path.Combine(
                        outputDirectoryPath,
                        $".cut-assistant-next-{Guid.NewGuid():N}-{index + 1:D4}.mp4");

                segmentFilePaths.Add(
                    segmentFilePath);

                await _runner.RunSplitAsync(
                    sourceFilePath,
                    segmentFilePath,
                    ranges[index],
                    cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (segmentFilePaths.Count == 1)
            {
                File.Move(
                    segmentFilePaths[0],
                    fullOutputFilePath,
                    overwrite: outputFileExists && overwriteExistingOutput);
            }
            else
            {
                progress?.Report(
                    new Mp4BoxProgressUpdate(
                        Mp4BoxProgressKind.Status,
                        "Segmente werden zusammengefügt …"));

                await _runner.RunConcatAsync(
                    segmentFilePaths,
                    concatOutputFilePath,
                    cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                File.Move(
                    concatOutputFilePath,
                    fullOutputFilePath,
                    overwrite: outputFileExists && overwriteExistingOutput);
            }

            progress?.Report(
                new Mp4BoxProgressUpdate(
                    Mp4BoxProgressKind.Status,
                    "Fertig."));
        }
        finally
        {
            File.Delete(
                concatOutputFilePath);

            foreach (var segmentFilePath in segmentFilePaths)
            {
                File.Delete(
                    segmentFilePath);
            }
        }
    }
}
