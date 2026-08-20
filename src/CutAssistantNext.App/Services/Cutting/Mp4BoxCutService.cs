using CutAssistantNext.Core.Editing;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class Mp4BoxCutService
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
        string sourceFilePath,
        string outputFilePath,
        CutPlan cutPlan,
        double framesPerSecond,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(
            sourceFilePath,
            outputFilePath,
            cutPlan,
            framesPerSecond,
            progress: null,
            cancellationToken);
    }

    public Task RunAsync(
        string sourceFilePath,
        string outputFilePath,
        CutPlan cutPlan,
        double framesPerSecond,
        IProgress<Mp4BoxProgressUpdate>? progress,
        CancellationToken cancellationToken = default)
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
            cancellationToken);
    }
}
