using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class ConfiguredCutOutputAnalyzer : ICutOutputAnalyzer
{
    public Task<CutOutputTimeline> AnalyzeAsync(string outputFilePath, IReadOnlyList<string> segmentFilePaths,
        IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken) =>
        Task.Run(() =>
        {
            var settings = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var path = new ToolPathResolver().Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath);
            return new FfprobeCutOutputAnalyzer(path).AnalyzeAsync(outputFilePath, segmentFilePaths, progress, cancellationToken);
        }, cancellationToken);
}
