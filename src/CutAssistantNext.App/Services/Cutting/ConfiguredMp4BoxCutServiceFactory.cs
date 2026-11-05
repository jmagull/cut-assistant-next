using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Media.Tools;
using System.IO;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed class ConfiguredMp4BoxCutServiceFactory
{
    private readonly Func<CutApplicationSettings?> _settingsProvider;

    private readonly Func<
        string,
        IProgress<CutProgressUpdate>?,
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
            IProgress<CutProgressUpdate>?,
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
        IProgress<CutProgressUpdate>? progress)
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
            runner, new ConfiguredCutOutputAnalyzer());
    }

    internal async Task<Mp4BoxCutService> CreateCheckedAsync(IProgress<CutProgressUpdate>? progress,
        CancellationToken token, IToolProbeRunner? probe = null)
    {
        var path = await ResolveAvailableAsync(token, probe).ConfigureAwait(false);
        progress?.Report(new(CutProgressKind.Output, "MP4Box ist verfügbar; der bestätigte neue Schnitt wird gestartet."));
        return new Mp4BoxCutService(_runnerFactory(path, progress), new ConfiguredCutOutputAnalyzer());
    }
    internal async Task<(bool Available, string? Reason)> CheckAvailabilityAsync(CancellationToken token)
    {
        try { await ResolveAvailableAsync(token).ConfigureAwait(false); return (true, null); }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or System.ComponentModel.Win32Exception or TimeoutException)
        { return (false, e.Message); }
    }

    private async Task<string> ResolveAvailableAsync(CancellationToken token, IToolProbeRunner? probe = null)
    {
        token.ThrowIfCancellationRequested();
        var settings = _settingsProvider() ?? CutApplicationSettings.CreateDefault();
        var path = _toolPathResolver.Resolve(BundledToolKind.Mp4Box, settings.ExecutablePath);
        if (!Path.IsPathFullyQualified(path) || !Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase) ||
            !File.Exists(path))
            throw new FileNotFoundException("MP4Box ist nicht verfügbar. Bitte den MP4Box-Pfad in den Einstellungen prüfen.", path);
        var result = await (probe ?? new ToolProbeRunner()).RunAsync(path, ["-version"], token).ConfigureAwait(false);
        if (result.ExitCode != 0 || !(result.StandardOutput + result.StandardError).Contains("MP4Box", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("MP4Box konnte nicht erfolgreich gestartet und erkannt werden. Bitte Installation und Werkzeugpfad prüfen.");
        return path;
    }
}
