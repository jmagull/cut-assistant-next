using System.Globalization;
using System.IO;
using CutAssistantNext.Cutlists.Compatibility;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Services;

internal sealed class CutlistResultRow
{
    internal CutlistSearchResult? Server { get; private init; }
    internal string? ServerUrl { get; private init; }
    internal LocalCutlistCandidate? Local { get; private init; }
    internal CutlistDocument? Document { get; private init; }
    public string Source => Local is null ? "Server" : "Lokal";
    public int SourceOrder => Local is null ? 0 : 1;
    public long ServerOrder => long.TryParse(Server?.Id, out var id) ? id : long.MinValue;
    public string CutlistFileName { get; private init; } = string.Empty;
    public string DisplayFormat => new CutlistSearchResult { CutlistFileName = CutlistFileName }.DisplayFormat;
    public string Author => Server?.Author ?? Document?.Info.Author ?? "–";
    public string Rating => Server?.Rating ?? "–";
    public string RatingCount => Server?.RatingCount ?? "–";
    public string RatingByAuthor => Server?.RatingByAuthor ?? Document?.Info.RatingByAuthor.ToString(CultureInfo.CurrentCulture) ?? "–";
    public string Cuts => Server?.Cuts ?? Document?.Cuts.Count.ToString(CultureInfo.CurrentCulture) ?? "–";
    public string Duration
    {
        get
        {
            if (Server is not null)
            {
                if (!decimal.TryParse(Server.Duration, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) ||
                    seconds < 0 || seconds > TimeSpan.MaxValue.Ticks / (decimal)TimeSpan.TicksPerSecond)
                    return "–";
                return FormatDuration(seconds * 1000);
            }

            return Document is null ? "–" : FormatDuration(
                Document.Cuts.Sum(cut => cut.Duration.Ticks) / (decimal)TimeSpan.TicksPerMillisecond);
        }
    }

    private static string FormatDuration(decimal milliseconds)
    {
        // Round only the display: persisted cut times and the imported document remain untouched.
        var totalMilliseconds = (long)decimal.Round(milliseconds, 0, MidpointRounding.AwayFromZero);
        var hours = totalMilliseconds / 3_600_000;
        var minutes = totalMilliseconds / 60_000 % 60;
        var seconds = totalMilliseconds / 1000 % 60;
        return FormattableString.Invariant($"{hours:00}:{minutes:00}:{seconds:00}.{totalMilliseconds % 1000:000}");
    }
    public string DownloadCount => Server?.DownloadCount ?? "–";
    public string SuggestedMovieName => Server?.SuggestedMovieName ?? Document?.Info.SuggestedMovieName ?? string.Empty;
    public string UserComment => Server?.UserComment ?? Document?.Info.UserComment ?? string.Empty;
    public string Finding { get; private init; } = "Noch nicht geprüft";
    public string Details => Local is null
        ? "Server · Prüfung der Cutlist nach dem Herunterladen. " + Server?.Errors
        : string.Join(Environment.NewLine, Local.Paths) + Environment.NewLine + _reason;
    private string _reason = string.Empty;
    public bool CanLoad { get; private init; } = true;

    internal static CutlistResultRow FromServer(CutlistSearchResult result, string serverUrl) => new()
    { Server = result, ServerUrl = serverUrl, CutlistFileName = result.CutlistFileName };

    internal static CutlistResultRow FromLocal(LocalCutlistCandidate candidate, CutlistMediaContext context)
    {
        CutlistDocument? document = null;
        try
        {
            document = candidate.ReadDocument();
            var duration = context.Analysis.Duration ?? throw new InvalidDataException("Mediendauer fehlt.");
            // Retain the existing end-fragment exception instead of rejecting a correctable document.
            var fragment = CutlistEndFragmentDetector.Find(document, duration);
            if (fragment is null) _ = CutlistCutPlanBuilder.Build(document, duration);
            else _ = CutlistEndFragmentCorrector.BuildCutPlan(document, duration, fragment);
            var reasons = new List<string>();
            var applicable = true;
            var original = Path.GetFileName(context.FilePath);
            if (string.IsNullOrWhiteSpace(document.General.ApplyToFile)) reasons.Add("Originaldateiname fehlt.");
            else if (!string.Equals(document.General.ApplyToFile, original, StringComparison.OrdinalIgnoreCase))
            { reasons.Add("ApplyToFile nennt einen anderen Originaldateinamen."); applicable = false; }
            var expected = document.General.OriginalFileSizeBytes;
            var actual = context.Analysis.FileSizeBytes;
            if (expected is null or <= 0 || actual is null or <= 0) reasons.Add("Originaldateigröße nicht bekannt.");
            else if (expected != actual)
                reasons.Add(CutlistFileSizeCompatibilityDetector.FindMismatch(expected, actual) is null
                    ? "Dateigröße weicht innerhalb der bisherigen 5-%-Toleranz ab."
                    : "Dateigröße weicht um mehr als 5 % ab; vorhandene Rückfrage beim Laden beachten.");
            var fps = document.General.FramesPerSecond;
            var mediaFps = context.Analysis.VideoStreams.FirstOrDefault()?.FramesPerSecond;
            if (fps is null || mediaFps is null) reasons.Add("Framerate nicht zuverlässig vergleichbar.");
            else if (!double.IsFinite(fps.Value) || fps <= 0 || !double.IsFinite(mediaFps.Value) || mediaFps <= 0)
            { reasons.Add("Ungültige Framerate."); applicable = false; }
            else if (Math.Abs(fps.Value - mediaFps.Value) > 0.001)
            { reasons.Add("Framerate passt nicht zum Film."); applicable = false; }
            if (fragment is not null) reasons.Add("Winziges Endfragment: bestehende Korrekturrückfrage erforderlich.");
            if (document.Cuts.Count == 0) reasons.Add("Die geplante Ausgabe ist leer (gesamten Film entfernen).");
            return new CutlistResultRow
            {
                Local = candidate,
                Document = document,
                CutlistFileName = Path.GetFileName(candidate.Paths[0]),
                Finding = !applicable ? "Widerspruch" : reasons.Count == 0 ? "Plausibel passend" : "Unklar",
                CanLoad = applicable,
                _reason = (reasons.Count == 0 ? "Originalname, Dateigröße und Framerate stimmen überein." : string.Join(" ", reasons)) +
                    " Bearbeitungsstand nicht hinterlegt. Dauer = Summe der Behaltebereiche."
            };
        }
        catch (Exception error) when (error is ArgumentException or IOException or InvalidDataException or FormatException or OverflowException)
        {
            var durationMismatch = document is not null && error is InvalidDataException;
            return new CutlistResultRow
            {
                Local = candidate,
                Document = durationMismatch ? document : null,
                CutlistFileName = Path.GetFileName(candidate.Paths[0]),
                Finding = durationMismatch ? "Widerspruch" : "Nicht verarbeitbar",
                CanLoad = false,
                _reason = error.Message
            };
        }
    }
}
