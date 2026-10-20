namespace CutAssistantNext.Core.Cutting;

public interface ICutEngine
{
    Task RunAsync(
        CutRequest request,
        IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default);

    async Task<CutResult> RunWithResultAsync(
        CutRequest request,
        IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await RunAsync(request, progress, cancellationToken).ConfigureAwait(false);
        return CutResult.Planned(request.OutputFilePath, request.KeepSegments);
    }
}
