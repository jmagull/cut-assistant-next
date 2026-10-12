using System.Globalization;
using System.Text.RegularExpressions;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>Preserves raw output and translates the existing CAN-CLI/FFMS2 messages for the UI.</summary>
internal sealed class OtrCanProgressReporter(IProgress<CutProgressUpdate>? target, int segmentCount, bool indexing)
    : IProgress<CutProgressUpdate>
{
    private readonly object _gate = new();
    private int _segment;
    private static readonly Regex IndexPercentage = new(@"^Indexing, please wait\.\.\.\s*(\d{1,3})%\s*$", RegexOptions.CultureInvariant);
    private static readonly Regex SegmentNumber = new(@"^TRACE: Processing interval no (\d+) \.\.\.$", RegexOptions.CultureInvariant);
    private static readonly Regex Range = new(@"^TRACE: (Copying|Re-encoding) interval \[([0-9]+\.[0-9]+), ([0-9]+\.[0-9]+)\] \.\.\.$", RegexOptions.CultureInvariant);

    public void Report(CutProgressUpdate update)
    {
        if (target is null) return;
        lock (_gate)
        {
            target.Report(update);
            if (update.Kind != CutProgressKind.Output) return;
            var line = update.Message.Trim();
            string? status = null;
            double? percentage = null;
            if (indexing)
            {
                var match = IndexPercentage.Match(line);
                if (match.Success && int.TryParse(match.Groups[1].Value, out var value) && value <= 100)
                {
                    status = "FFMS2: Schnittquelle indexieren";
                    percentage = value;
                }
                else if (line == "Writing timecodes... done.") status = "FFMS2: Zeitstempel gespeichert";
                else if (line == "Writing keyframes... done.") status = "FFMS2: Keyframes gespeichert";
                else if (line == "Writing index... done.") { status = "FFMS2: Index gespeichert"; percentage = 100; }
            }
            else
            {
                var match = SegmentNumber.Match(line);
                if (match.Success && int.TryParse(match.Groups[1].Value, out var number) && number > 0 && number <= segmentCount)
                {
                    _segment = number;
                    status = $"Behaltebereich {_segment} von {segmentCount} vorbereiten";
                }
                else
                {
                    var range = Range.Match(line);
                    if (range.Success && _segment > 0 && TryTime(range.Groups[2].Value, out var from) && TryTime(range.Groups[3].Value, out var to) && to >= from)
                    {
                        var operation = range.Groups[1].Value == "Copying" ? "Abschnitt kopieren" : "Schnittkante auf CPU neu kodieren";
                        status = $"Behaltebereich {_segment} von {segmentCount} – {operation}\nOriginal: {Format(from)} bis {Format(to)}";
                    }
                    else if (line == "TRACE: Retrieving metadata ...") status = "OTR-CAN: Mediendaten und vorhandenen Index prüfen";
                    else if (line == "TRACE: Copied interval") status = $"Behaltebereich {_segment} von {segmentCount} – Abschnitt kopiert";
                    else if (line == "TRACE: Re-encoded interval") status = $"Behaltebereich {_segment} von {segmentCount} – Schnittkante kodiert";
                    else if (line == "TRACE: Extracted interval") status = $"Behaltebereich {_segment} von {segmentCount} fertig";
                    else if (line == "TRACE: Concatenating interval files ...") status = "OTR-CAN: Behaltebereiche zusammenfügen";
                    else if (line == "INFO: Stage 1/3: corrected video cutting") status = "OTR-CAN: Video framegenau schneiden";
                    else if (line == "INFO: Stage 2/3: continuous CPU audio encoding") status = "OTR-CAN: Tonspuren vorbereiten und neu kodieren";
                    else if (line == "INFO: Decoding original audio once and assembling exact kept PCM samples") status = "OTR-CAN: Originalton lesen und Behaltebereiche vorbereiten";
                    else if ((line == "INFO: Encoding each complete audio track once with original codec/layout/rate/target bitrate" || line == "INFO: Encoding each complete audio track once; MP3 becomes AAC-LC, existing AAC/AC-3 settings are preserved")) status = "OTR-CAN: Alle Tonspuren auf CPU neu kodieren";
                    else if (line == "INFO: Stage 3/3: assembling final MP4 and verifying unchanged stream packets") status = "OTR-CAN: Bild und Ton zusammenführen und prüfen";
                    else if (line == "INFO: Final MP4 verified: video and all audio packets, PTS/DTS, padding and decoder configurations preserved") status = "OTR-CAN: Bild, Ton und Zeitstempel geprüft";
                    else if (line.StartsWith("INFO: CAN cut completed:", StringComparison.Ordinal)) status = "OTR-CAN: Schnittergebnis erstellt";
                }
            }
            if (status is not null) target.Report(new(CutProgressKind.Progress, status) { Percentage = percentage });
        }
    }

    private static bool TryTime(string text, out TimeSpan time)
    {
        time = default;
        if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value) || value < 0 || value >= 86400) return false;
        time = TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerSecond));
        return true;
    }

    private static string Format(TimeSpan time) => time.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
}
