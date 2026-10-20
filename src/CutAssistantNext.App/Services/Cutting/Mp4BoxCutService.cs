using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class Mp4BoxCutService : ICutEngine
{
    private readonly Mp4BoxCutWorkflow _workflow;
    private readonly ICutOutputAnalyzer? _outputAnalyzer;
    private const string ImageNotice = "MP4Box kann Schnittgrenzen verschieben. Geprüfte Prüfpositionen stellen fehlende Bilder nicht wieder her.";

    public Mp4BoxCutService(
        IMp4BoxRunner runner, ICutOutputAnalyzer? outputAnalyzer = null)
    {
        _outputAnalyzer = outputAnalyzer;
        ArgumentNullException.ThrowIfNull(
            runner);

        _workflow =
            new Mp4BoxCutWorkflow(
                runner);
    }

    public Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default) => RunWithResultAsync(request, progress, cancellationToken);

    public async Task<CutResult> RunWithResultAsync(CutRequest request,
        IProgress<CutProgressUpdate>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        var framesPerSecond = request.FramesPerSecond
            ?? throw new InvalidOperationException("Für den MP4Box-Schnitt ist eine Bildrate erforderlich.");
        var planned = CutResult.Planned(request.OutputFilePath, request.KeepSegments);
        var ranges = CutPlanMp4BoxRangeBuilder.Build(request.KeepSegments, framesPerSecond);
        var result = await _workflow.RunWithAnalysisAsync(request.SourceFilePath, request.OutputFilePath, ranges,
            progress, cancellationToken, request.OverwriteExistingOutput, _outputAnalyzer).ConfigureAwait(false);
        return result.Timeline is { } timeline
            ? new(request.OutputFilePath, timeline, CutReviewTimingKind.Measured, ImageNotice)
            : new(request.OutputFilePath, planned.Timeline, CutReviewTimingKind.Estimated,
                (result.AnalysisWarning ?? "Die tatsächlichen Übergänge wurden nicht geprüft. Die Prüfstellen sind nur aus dem Schnittplan berechnet.")
                + " " + ImageNotice);
    }

    public Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        CutPlan cutPlan,
        double framesPerSecond,
        CancellationToken cancellationToken = default,
        bool overwriteExistingOutput = false)
    {
        return RunAsync(
            sourceFilePath,
            outputFilePath,
            cutPlan,
            framesPerSecond,
            progress: null,
            cancellationToken,
            overwriteExistingOutput);
    }

    public Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        CutPlan cutPlan,
        double framesPerSecond,
        IProgress<CutProgressUpdate>? progress,
        CancellationToken cancellationToken = default,
        bool overwriteExistingOutput = false)
    {
        ArgumentNullException.ThrowIfNull(
            cutPlan);

        return RunAsync(CutRequestFactory.Create(sourceFilePath, outputFilePath, cutPlan, framesPerSecond,
            overwriteExistingOutput), progress, cancellationToken);
    }
}
