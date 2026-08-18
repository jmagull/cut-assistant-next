using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class ConfiguredMp4BoxCutServiceFactory
{
    private readonly Func<CutApplicationSettings?> _settingsProvider;
    private readonly Func<string, IMp4BoxRunner> _runnerFactory;

    internal ConfiguredMp4BoxCutServiceFactory(
        Func<CutApplicationSettings?> settingsProvider,
        Func<string, IMp4BoxRunner> runnerFactory)
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

    public Mp4BoxCutService Create()
    {
        var settings =
            _settingsProvider()
            ?? CutApplicationSettings.CreateDefault();

        if (string.IsNullOrWhiteSpace(
            settings.ExecutablePath))
        {
            throw new InvalidOperationException(
                "Es ist keine Schnittanwendung konfiguriert.");
        }

        var runner =
            _runnerFactory(
                settings.ExecutablePath);

        return new Mp4BoxCutService(
            runner);
    }
}
