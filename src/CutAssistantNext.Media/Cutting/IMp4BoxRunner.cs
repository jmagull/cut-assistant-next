namespace CutAssistantNext.Media.Cutting;

public interface IMp4BoxRunner
{
    Task RunSplitAsync(
        string sourceFilePath,
        string outputFilePath,
        Mp4BoxSplitRange range,
        CancellationToken cancellationToken = default);

    Task RunConcatAsync(
        IReadOnlyList<string> segmentFilePaths,
        string outputFilePath,
        CancellationToken cancellationToken = default);
}
