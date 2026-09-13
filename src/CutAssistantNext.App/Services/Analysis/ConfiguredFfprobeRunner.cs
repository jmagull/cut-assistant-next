using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Services.Analysis;

internal sealed class ConfiguredFfprobeRunner :
    IMediaAnalysisRunner
{
    private readonly Func<FfmpegSettings?> _settingsProvider;
    private readonly Func<string, IMediaAnalysisRunner> _runnerFactory;

    internal ConfiguredFfprobeRunner(
        Func<FfmpegSettings?> settingsProvider,
        Func<string, IMediaAnalysisRunner> runnerFactory)
    {
        ArgumentNullException.ThrowIfNull(
            settingsProvider);

        ArgumentNullException.ThrowIfNull(
            runnerFactory);

        _settingsProvider =
            settingsProvider;

        _runnerFactory =
            runnerFactory;
    }

    public Task<MediaAnalysisResult> RunAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default)
    {
        var settings =
            _settingsProvider()
            ?? FfmpegSettings.CreateDefault();

        if (string.IsNullOrWhiteSpace(
            settings.FfprobeExecutablePath))
        {
            throw new InvalidOperationException(
                "ffprobe.exe ist nicht konfiguriert.");
        }

        var runner =
            _runnerFactory(
                settings.FfprobeExecutablePath);

        return runner.RunAsync(
            mediaFilePath,
            cancellationToken);
    }
}
