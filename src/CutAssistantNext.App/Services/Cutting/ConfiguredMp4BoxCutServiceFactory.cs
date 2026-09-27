using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class ConfiguredMp4BoxCutServiceFactory
{
    private readonly Func<CutApplicationSettings?> _settingsProvider;

    private readonly Func<
        string,
        IProgress<Mp4BoxProgressUpdate>?,
        IMp4BoxRunner> _runnerFactory;

    private readonly ToolPathResolver _toolPathResolver;

    internal ConfiguredMp4BoxCutServiceFactory(
        Func<CutApplicationSettings?> settingsProvider,
        Func<string, IMp4BoxRunner> runnerFactory,
        ToolPathResolver? toolPathResolver = null)
        : this(
            settingsProvider,
            (path, _) => runnerFactory(path),
            toolPathResolver)
    {
        ArgumentNullException.ThrowIfNull(
            runnerFactory);
    }

    internal ConfiguredMp4BoxCutServiceFactory(
        Func<CutApplicationSettings?> settingsProvider,
        Func<
            string,
            IProgress<Mp4BoxProgressUpdate>?,
            IMp4BoxRunner> runnerFactory,
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

    public Mp4BoxCutService Create()
    {
        return Create(
            progress: null);
    }

    public Mp4BoxCutService Create(
        IProgress<Mp4BoxProgressUpdate>? progress)
    {
        var settings =
            _settingsProvider()
            ?? CutApplicationSettings.CreateDefault();

        var executablePath =
            _toolPathResolver.Resolve(
                BundledToolKind.Mp4Box,
                settings.ExecutablePath);

        var runner =
            _runnerFactory(
                executablePath,
                progress);

        return new Mp4BoxCutService(
            runner);
    }
}
