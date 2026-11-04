using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed record OtrCanAudioIssue(string Kind, int Keep, long StartSample, long EndSample, long MissingSamples);

internal sealed record OtrCanAudioDiagnostic(int SchemaVersion, string Classification, int StreamIndex,
    string Codec, int SampleRate, long ExpectedSamples, long ActualSamples,
    IReadOnlyList<OtrCanAudioIssue> Issues, string Uncertainty)
{
    internal static OtrCanAudioDiagnostic? Read(string work, MediaAnalysisResult source, CutRequest request)
    {
        try
        {
            var file = new FileInfo(Path.Combine(work, "audio-diagnostic.json"));
            if (!file.Exists || file.Length is <= 0 or > 65_536 || file.LinkTarget is not null) return null;
            var result = JsonSerializer.Deserialize<OtrCanAudioDiagnostic>(File.ReadAllText(file.FullName),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
            return result is not null && result.IsValid(source, request) ? result : null;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException or OverflowException)
        {
            return null;
        }
    }

    private bool IsValid(MediaAnalysisResult source, CutRequest request)
    {
        var track = source.AudioStreams.SingleOrDefault(s => s.Index == StreamIndex);
        if (SchemaVersion != 1 || track is null || track.CodecName != Codec || track.SampleRate != SampleRate ||
            SampleRate is not (32000 or 44100 or 48000) || ExpectedSamples <= 0 || ActualSamples < 0 ||
            ActualSamples == ExpectedSamples || Issues is null || Issues.Count > 64 ||
            Uncertainty is null || Uncertainty.Length > 4096) return false;
        if (Classification == "unclassified") return Issues.Count == 0;
        if (Classification != "source_coverage_gap" || ActualSamples >= ExpectedSamples || Issues.Count == 0) return false;
        long missing = 0;
        var previousKeep = 0;
        long previousEnd = 0;
        foreach (var issue in Issues)
        {
            if (issue is null || issue.Kind is not ("internal_gap" or "audio_end") ||
                issue.Keep < 1 || issue.Keep > request.KeepSegments.Count || issue.Keep < previousKeep ||
                issue.StartSample < 0 || issue.EndSample <= issue.StartSample || issue.EndSample > (long)SampleRate * 86400 ||
                issue.EndSample - issue.StartSample != issue.MissingSamples ||
                issue.Keep == previousKeep && issue.StartSample < previousEnd) return false;
            missing = checked(missing + issue.MissingSamples);
            previousKeep = issue.Keep;
            previousEnd = issue.EndSample;
        }
        return missing == ExpectedSamples - ActualSamples;
    }

    internal string Description
    {
        get
        {
            var text = new StringBuilder($"otr-can konnte Tonspur {StreamIndex} ({Codec.ToUpperInvariant()}, {SampleRate} Hz) nicht sicher verarbeiten.");
            text.AppendLine();
            if (Classification == "source_coverage_gap")
            {
                foreach (var issue in Issues)
                {
                    var time = TimeSpan.FromTicks(checked((long)((decimal)issue.StartSample * TimeSpan.TicksPerSecond / SampleRate)));
                    text.AppendLine($"{(issue.Kind == "audio_end" ? "Fehlender Ton am Dateiende" : "Innere Zeitlücke")} ab {time.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture)}, " +
                        $"Behaltebereich {issue.Keep}: {issue.MissingSamples} fehlende Samples ({(decimal)issue.MissingSamples * 1000 / SampleRate:0.###} ms).");
                }
                text.Append("Die fehlende Zeitabdeckung ist nachgewiesen. Ob Tonmaterial fehlt oder Container-Zeitstempel falsch sind, bleibt ungeklärt. ");
            }
            else text.Append("Die Sampleabweichung lässt sich nicht eindeutig als Zeitlücke erklären. ");
            text.Append($"Erwartet: {ExpectedSamples} Samples, vorhanden: {ActualSamples}. Die strenge Prüfung hat den Schnitt abgebrochen. Es wurde kein Ton ergänzt oder verschoben.");
            return text.ToString();
        }
    }
}
