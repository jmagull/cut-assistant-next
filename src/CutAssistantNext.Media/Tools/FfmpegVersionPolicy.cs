using System.Globalization;
using System.Text.RegularExpressions;

namespace CutAssistantNext.Media.Tools;

public sealed record FfmpegVersionAssessment(bool IsSupported, bool MeetsRecommendation, string Message);

/// <summary>Assesses the program version, never mistaking library or build dates for release versions.</summary>
public static class FfmpegVersionPolicy
{
    public static FfmpegVersionAssessment Assess(string output, string programName)
    {
        ArgumentNullException.ThrowIfNull(output);
        if (programName is not ("ffmpeg" or "ffprobe"))
        {
            throw new ArgumentOutOfRangeException(nameof(programName));
        }

        var header = Regex.Match(output,
            $@"^\s*{programName} version (?<token>\S+)",
            RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!header.Success)
        {
            return new(false, false, "Die ausgewählte Datei meldet keine passende Programmversion.");
        }

        string token = header.Groups["token"].Value;
        var release = Regex.Match(token, @"^n?(?<version>\d+\.\d+(?:\.\d+){0,2})(?=$|[-+])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (release.Success && Version.TryParse(release.Groups["version"].Value, out var version))
        {
            var normalized = new Version(version.Major, version.Minor, Math.Max(0, version.Build), Math.Max(0, version.Revision));
            if (normalized <= new Version(5, 1, 0, 0))
            {
                return new(false, false, $"Version {token} ist zu alt. Erforderlich: größer als 5.1; empfohlen: ab 9.0.2.");
            }

            bool recommended = normalized >= new Version(9, 0, 2, 0) &&
                !Regex.IsMatch(token, @"[-+](?:rc|alpha|beta|dev|git)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return new(true, recommended, recommended
                ? $"Version {token} erfüllt die Empfehlung ab 9.0.2."
                : $"Version {token} ist unterstützt. Empfohlen: ab 9.0.2.");
        }

        // Git snapshots have no comparable release number. A codec ABI from FFmpeg 6 or later
        // can establish the lower bound (> 5.1), but cannot prove the recommended patch release.
        if (Regex.IsMatch(token, @"^(?:N-\d+-g[0-9a-f]+|\d{4}-\d{2}-\d{2}-git-[0-9a-f]+)(?:-|$)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            var codec = Regex.Match(output, @"^\s*libavcodec\s+(?<major>\d+)\.\s*\d+\.\s*\d+\s*/\s*(?<runtime>\d+)\.",
                RegexOptions.Multiline | RegexOptions.CultureInvariant);
            if (codec.Success && int.TryParse(codec.Groups["major"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int major) &&
                int.TryParse(codec.Groups["runtime"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int runtime) &&
                major >= 60 && runtime >= 60)
            {
                return new(true, false,
                    $"Entwicklungsbuild {token}: Bibliotheksstand neuer als 5.1. Eine Releaseversion ab 9.0.2 ist nicht nachweisbar.");
            }
        }

        return new(false, false,
            $"Versionsstand {token} nicht eindeutig prüfbar. Bitte eine Version größer als 5.1 auswählen; empfohlen: ab 9.0.2.");
    }
}
