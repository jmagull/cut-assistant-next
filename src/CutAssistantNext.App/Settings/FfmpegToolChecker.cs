using System.IO;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Settings;

internal sealed record FfmpegToolCheckResult(string Name, bool IsAvailable, string Message);

internal sealed class FfmpegToolChecker
{
    private readonly IToolProbeRunner _probe;
    private readonly ToolPathResolver _resolver;

    internal FfmpegToolChecker(IToolProbeRunner? probe = null, ToolPathResolver? resolver = null)
    {
        _probe = probe ?? new ToolProbeRunner();
        _resolver = resolver ?? new ToolPathResolver();
    }

    internal async Task<IReadOnlyList<FfmpegToolCheckResult>> CheckAsync(
        FfmpegSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return [
            await CheckOneAsync(BundledToolKind.Ffmpeg, settings.FfmpegExecutablePath, cancellationToken).ConfigureAwait(false),
            await CheckOneAsync(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath, cancellationToken).ConfigureAwait(false)
        ];
    }

    private async Task<FfmpegToolCheckResult> CheckOneAsync(
        BundledToolKind kind, string configuredPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string program = kind == BundledToolKind.Ffmpeg ? "ffmpeg" : "ffprobe";
        string name = kind == BundledToolKind.Ffmpeg ? "FFmpeg" : "ffprobe";
        try
        {
            string? path = _resolver.TryResolve(kind, configuredPath);
            if (string.IsNullOrWhiteSpace(path)) return new(name, false, "Noch kein Programmpfad eingerichtet.");
            if (!Path.IsPathFullyQualified(path) || !Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                return new(name, false, "Bitte einen vollständigen Pfad zu einer EXE-Datei auswählen.");
            if (!File.Exists(path)) return new(name, false, "Programmdatei nicht gefunden. Bitte Pfad und Installation prüfen.");
            var result = await _probe.RunAsync(path, ["-version"], cancellationToken).ConfigureAwait(false);
            if (result.ExitCode != 0)
                return new(name, false, "Programmstart oder Versionsprüfung fehlgeschlagen. Bitte das vollständige Werkzeugpaket einschließlich seiner DLLs prüfen.");
            var assessment = FfmpegVersionPolicy.Assess(result.StandardOutput + "\n" + result.StandardError, program);
            return new(name, assessment.IsSupported, assessment.Message);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or
            InvalidOperationException or System.ComponentModel.Win32Exception or TimeoutException)
        {
            return new(name, false, error.Message);
        }
    }
}
