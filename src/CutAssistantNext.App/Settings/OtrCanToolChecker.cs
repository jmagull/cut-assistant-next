using System.IO;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Settings;

internal sealed record OtrCanToolCheckResult(string Name, bool IsAvailable, string Message);

internal sealed class OtrCanToolChecker
{
    internal const string Ffms2DownloadUrl = "https://github.com/ffms/ffms2/releases";
    private readonly IToolProbeRunner _probe;
    private readonly ToolPathResolver _resolver;

    internal OtrCanToolChecker(IToolProbeRunner? probe = null, ToolPathResolver? resolver = null)
    {
        _probe = probe ?? new ToolProbeRunner();
        _resolver = resolver ?? new ToolPathResolver();
    }

    internal async Task<IReadOnlyList<OtrCanToolCheckResult>> CheckAsync(
        OtrCanSettings settings, FfmpegSettings ffmpeg, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(ffmpeg);
        var normalized = settings.Normalize();
        var results = new List<OtrCanToolCheckResult>();
        results.Add(await CheckOneAsync("OTR-CAN", normalized.ExecutablePath, ["cut-can", "--help"],
            output => new[] { "--input", "--output", "--index", "--temp-dir", "--cutlist", "--ffmpeg", "--ffprobe" }.All(output.Contains),
            "CAN-Schnittstelle erkannt.", cancellationToken));
        results.Add(await CheckOneAsync("ffmsindex.exe", normalized.FfmsIndexExecutablePath, [],
            output => output.Contains("ffmsindex", StringComparison.OrdinalIgnoreCase) &&
                output.Contains("-c", StringComparison.Ordinal) && output.Contains("-k", StringComparison.Ordinal),
            "Indexer erreichbar.", cancellationToken));
        results.Add(await CheckOneAsync("FFmpeg", ResolveFfmpeg(BundledToolKind.Ffmpeg, ffmpeg.FfmpegExecutablePath), ["-version"],
            output => output.Contains("ffmpeg version", StringComparison.OrdinalIgnoreCase),
            "Programm erreichbar.", cancellationToken));
        results.Add(await CheckOneAsync("ffprobe", ResolveFfmpeg(BundledToolKind.Ffprobe, ffmpeg.FfprobeExecutablePath), ["-version"],
            output => output.Contains("ffprobe version", StringComparison.OrdinalIgnoreCase),
            "Programm erreichbar.", cancellationToken));
        return results.AsReadOnly();
    }

    internal static void ValidateOptionalPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        if (!Path.IsPathFullyQualified(path) || !string.Equals(Path.GetExtension(path), ".exe", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Bitte einen vollständigen Pfad zu einer EXE-Datei auswählen oder das Feld leeren.");
        }
    }

    private string ResolveFfmpeg(BundledToolKind kind, string? configuredPath) =>
        _resolver.TryResolve(kind, configuredPath) ?? string.Empty;

    private async Task<OtrCanToolCheckResult> CheckOneAsync(string name, string path, IReadOnlyList<string> arguments,
        Func<string, bool> compatible, string successMessage, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            if (string.IsNullOrWhiteSpace(path)) return new(name, false, "Noch kein Programmpfad eingerichtet.");
            ValidateOptionalPath(path);
            if (!File.Exists(path)) return new(name, false, "Programmdatei nicht gefunden. Bitte Pfad und Installation prüfen.");
            var result = await _probe.RunAsync(path, arguments, cancellationToken).ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return new(name, false, "Programmstart oder Prüfung fehlgeschlagen. Bitte das vollständige Werkzeugpaket einschließlich seiner DLLs prüfen.");
            }
            if (!compatible(result.StandardOutput + "\n" + result.StandardError))
            {
                return new(name, false, "Die ausgewählte Programmdatei unterstützt die benötigte Schnittstelle nicht.");
            }
            return new(name, true, successMessage);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or
            InvalidOperationException or System.ComponentModel.Win32Exception or TimeoutException)
        {
            return new(name, false, error.Message);
        }
    }
}
