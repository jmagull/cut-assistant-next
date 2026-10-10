namespace CutAssistantNext.Core.Cutting;

public interface ICutEngine
{
    Task RunAsync(
        CutRequest request,
        IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default);
}
