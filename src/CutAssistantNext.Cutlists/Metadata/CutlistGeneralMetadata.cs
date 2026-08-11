using CutAssistantNext.Core;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.Cutlists.Metadata;

public sealed class CutlistGeneralMetadata
{
    public string Application { get; init; } =
        string.Empty;

    public string Version { get; init; } =
        string.Empty;

    public double? FramesPerSecond { get; init; }

    public string? DisplayAspectRatio { get; init; }

    public string IntendedCutApplicationName { get; init; } =
        string.Empty;

    public string IntendedCutApplication { get; init; } =
        string.Empty;

    public string IntendedCutApplicationVersion { get; init; } =
        string.Empty;

    public string IntendedCutApplicationOptions { get; init; } =
        string.Empty;

    public int NoOfCuts { get; init; }

    public string ApplyToFile { get; init; } =
        string.Empty;

    public long? OriginalFileSizeBytes { get; init; }

    public static CutlistGeneralMetadata Create(
        string applyToFile,
        string applicationVersion,
        MediaAnalysisResult analysis,
        CutApplicationInfo? intendedCutApplication = null,
        IReadOnlyCollection<CutlistKeepSegment>? keepSegments = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            applyToFile);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            applicationVersion);

        ArgumentNullException.ThrowIfNull(analysis);

        var videoStream =
            analysis.VideoStreams.FirstOrDefault();

        return new CutlistGeneralMetadata
        {
            Application = ApplicationInfo.ProductName,
            Version = applicationVersion,
            FramesPerSecond =
                videoStream?.FramesPerSecond,
            DisplayAspectRatio =
                videoStream?.DisplayAspectRatio,
            IntendedCutApplicationName =
                intendedCutApplication?.Name ?? string.Empty,
            IntendedCutApplication =
                intendedCutApplication?.Executable ?? string.Empty,
            IntendedCutApplicationVersion =
                intendedCutApplication?.Version ?? string.Empty,
            IntendedCutApplicationOptions =
                intendedCutApplication?.Options ?? string.Empty,
            NoOfCuts =
                keepSegments?.Count ?? 0,
            ApplyToFile = applyToFile,
            OriginalFileSizeBytes =
                analysis.FileSizeBytes
        };
    }
}
