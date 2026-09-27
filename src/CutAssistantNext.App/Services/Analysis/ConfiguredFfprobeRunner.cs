using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Services.Analysis;

internal sealed class ConfiguredFfprobeRunner :
    IMediaAnalysisRunner
{
    private readonly Func<FfmpegSettings?> _settingsProvider;
    private readonly Func<string, IMediaAnalysisRunner> _runnerFactory;
    private readonly ToolPathResolver _toolPathResolver;

    internal ConfiguredFfprobeRunner(
        Func<FfmpegSettings?> settingsProvider,
        Func<string, IMediaAnalysisRunner> runnerFactory,
        ToolPathResolver? toolPathResolver = null)
    {
        ArgumentNullException.ThrowIfNull(
            settingsProvider);

        ArgumentNullException.ThrowIfNull(
            runnerFactory);

        _settingsProvider =
            settingsProvider;

        _runnerFactory =
            runnerFactory;

        _toolPathResolver =
            toolPathResolver ?? new ToolPathResolver();
    }

    public Task<MediaAnalysisResult> RunAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default)
    {
        var settings =
            _settingsProvider()
            ?? FfmpegSettings.CreateDefault();

        var executablePath =
            _toolPathResolver.Resolve(
                BundledToolKind.Ffprobe,
                settings.FfprobeExecutablePath);

        var runner =
            _runnerFactory(
                executablePath);

        return runner.RunAsync(
            mediaFilePath,
            cancellationToken);
    }
}