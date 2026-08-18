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

    public async Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        IReadOnlyList<Mp4BoxSplitRange> ranges,
        CancellationToken cancellationToken = default)
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

        if (File.Exists(fullOutputFilePath))
        {
            throw new IOException(
                $"Die Zieldatei existiert bereits: {fullOutputFilePath}");
        }

        var outputDirectoryPath =
            Path.GetDirectoryName(
                fullOutputFilePath)
            ?? throw new InvalidOperationException(
                "Das Zielverzeichnis konnte nicht bestimmt werden.");

        var segmentFilePaths =
            new List<string>(
                ranges.Count);

        try
        {
            for (var index = 0; index < ranges.Count; index++)
            {
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

            await _runner.RunConcatAsync(
                segmentFilePaths,
                fullOutputFilePath,
                cancellationToken);
        }
        finally
        {
            foreach (var segmentFilePath in segmentFilePaths)
            {
                File.Delete(
                    segmentFilePath);
            }
        }
    }
}
