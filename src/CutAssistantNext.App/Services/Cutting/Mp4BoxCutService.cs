using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class Mp4BoxCutService : ICutEngine
{
    private readonly Mp4BoxCutWorkflow _workflow;

    public Mp4BoxCutService(
        IMp4BoxRunner runner)
    {
        ArgumentNullException.ThrowIfNull(
            runner);

        _workflow =
            new Mp4BoxCutWorkflow(
                runner);
    }

    public Task RunAsync(
        CutRequest request,
        IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var framesPerSecond = request.FramesPerSecond
            ?? throw new InvalidOperationException("Für den MP4Box-Schnitt ist eine Bildrate erforderlich.");

        var ranges = CutPlanMp4BoxRangeBuilder.Build(request.KeepSegments, framesPerSecond);

        return _workflow.RunAsync(
            request.SourceFilePath,
            request.OutputFilePath,
            ranges,
            progress,
            cancellationToken,
            request.OverwriteExistingOutput);
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

        var ranges =
            CutPlanMp4BoxRangeBuilder.Build(
                cutPlan,
                framesPerSecond);

        return _workflow.RunAsync(
            sourceFilePath,
            outputFilePath,
            ranges,
            progress,
            cancellationToken,
            overwriteExistingOutput);
    }
}
