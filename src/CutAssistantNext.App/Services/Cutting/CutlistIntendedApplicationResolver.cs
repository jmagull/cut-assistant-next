using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Services.Cutting;

internal static class CutlistIntendedApplicationResolver
{
    internal static CutEngineKind PreferredEngine(
        string mediaPath,
        string? loadedCutlistApplicationName,
        string? lastSuccessfulCutMediaPath,
        CutEngineKind? lastSuccessfulCutEngine)
    {
        if (lastSuccessfulCutEngine is { } engine &&
            string.Equals(mediaPath, lastSuccessfulCutMediaPath, StringComparison.OrdinalIgnoreCase))
            return engine;

        return string.Equals(loadedCutlistApplicationName, "otr-can", StringComparison.OrdinalIgnoreCase)
            ? CutEngineKind.OtrCan
            : CutEngineKind.Mp4Box;
    }

    internal static async Task<CutApplicationInfo> ForOtrCanAsync(
        OtrCanSettings settings,
        IToolProbeRunner? probe = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var executablePath = settings.Normalize().ExecutablePath;
        if (string.IsNullOrWhiteSpace(executablePath))
            return new CutApplicationInfo("otr-can", string.Empty, string.Empty, string.Empty);

        var version = string.Empty;
        try
        {
            var result = await (probe ?? new ToolProbeRunner())
                .RunAsync(executablePath, ["--version"], cancellationToken);
            if (result.ExitCode == 0)
            {
                var match = Regex.Match(
                    result.StandardOutput + "\n" + result.StandardError,
                    @"\botr_can\s+(?<version>\d+\.\d+\.\d+(?:[-+][0-9A-Za-z.-]+)?)",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (match.Success)
                    version = match.Groups["version"].Value;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or
            Win32Exception or TimeoutException or InvalidOperationException or ArgumentException)
        {
            // An unavailable version must not prevent saving the selected engine.
        }

        return new CutApplicationInfo("otr-can", Path.GetFileName(executablePath), version, string.Empty);
    }
}
