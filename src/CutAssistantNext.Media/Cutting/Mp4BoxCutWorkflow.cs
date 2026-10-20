using CutAssistantNext.Core.Cutting;
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

    public Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        IReadOnlyList<Mp4BoxSplitRange> ranges,
        IProgress<CutProgressUpdate>? progress,
        CancellationToken cancellationToken = default,
        bool overwriteExistingOutput = false) =>
        RunWithAnalysisAsync(sourceFilePath, outputFilePath, ranges, progress, cancellationToken, overwriteExistingOutput);

    public async Task<Mp4BoxWorkflowResult> RunWithAnalysisAsync(
        string sourceFilePath,
        string outputFilePath,
        IReadOnlyList<Mp4BoxSplitRange> ranges,
        IProgress<CutProgressUpdate>? progress,
        CancellationToken cancellationToken = default,
        bool overwriteExistingOutput = false,
        ICutOutputAnalyzer? analyzer = null)
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
                    new CutProgressUpdate(
                        CutProgressKind.Status,
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

            var pendingOutput = segmentFilePaths.Count == 1 ? segmentFilePaths[0] : concatOutputFilePath;
            if (segmentFilePaths.Count > 1)
            {
                progress?.Report(new(CutProgressKind.Status, "Segmente werden zusammengefügt …"));
                await _runner.RunConcatAsync(segmentFilePaths, pendingOutput, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }

            CutOutputTimeline? timeline = null;
            string? warning = null;
            if (analyzer is not null)
            {
                progress?.Report(new(CutProgressKind.Status, "Schnittstellen im fertigen Film werden geprüft …"));
                using var analysisCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                analysisCancellation.CancelAfter(TimeSpan.FromMinutes(10));
                try
                {
                    timeline = await analyzer.AnalyzeAsync(pendingOutput, segmentFilePaths.AsReadOnly(),
                        progress, analysisCancellation.Token).ConfigureAwait(false);
                    if (timeline.Transitions.Count != ranges.Count - 1 || timeline.Identity is null)
                        throw new InvalidDataException("Die Ausgabeanalyse hat keine vollständigen bestätigten Übergänge geliefert.");
                }
                catch (Exception error) when (error is not OutOfMemoryException)
                {
                    // Cutting is complete. Failed, timed-out or canceled review analysis must not discard the film.
                    timeline = null;
                    warning = error is OperationCanceledException
                        ? "Die Übergangsprüfung wurde abgebrochen. Die Prüfstellen sind nur aus dem Schnittplan berechnet."
                        : "Die tatsächlichen Übergänge konnten nicht bestätigt werden. Die Prüfstellen sind nur aus dem Schnittplan berechnet.";
                    progress?.Report(new(CutProgressKind.Output, warning));
                    progress?.Report(new(CutProgressKind.Output, "Grund der Übergangsprüfung: " + error.Message));
                }
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            File.Move(pendingOutput, fullOutputFilePath, overwrite: outputFileExists && overwriteExistingOutput);
            progress?.Report(new(CutProgressKind.Status, "Fertig."));
            return new(timeline, warning);
        }
        finally
        {
            foreach (var path in segmentFilePaths.Prepend(concatOutputFilePath))
            {
                try { File.Delete(path); }
                catch (Exception error) when (error is IOException or UnauthorizedAccessException)
                {
                    progress?.Report(new(CutProgressKind.Output,
                        $"Arbeitsdatei konnte nicht gelöscht werden: {path} ({error.Message})"));
                }
            }
        }
    }
}
