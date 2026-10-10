using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

internal static class VideoPreparation
{
    public static bool IsMp4(MediaAnalysisResult analysis) =>
        analysis.FormatName?.Split(',').Any(x => x.Trim().Equals("mp4", StringComparison.OrdinalIgnoreCase)) == true;

    public static string ContainerName(MediaAnalysisResult analysis) =>
        analysis.FormatName?.Split(',')[0].Trim().ToLowerInvariant() switch
        {
            "avi" => "AVI",
            "matroska" => "Matroska (MKV)",
            "mpegts" => "MPEG-TS",
            "mpeg" => "MPEG-PS",
            "asf" => "ASF/WMV",
            _ => analysis.FormatLongName ?? analysis.FormatName ?? "unbekannter Container"
        };

    public static void ValidateVideoPacketCounts(
        IReadOnlyDictionary<int, long> original,
        IReadOnlyDictionary<int, long> prepared)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(prepared);

        if (original.Count != prepared.Count)
            throw new InvalidOperationException(
                "Die Anzahl der Videostreams hat sich bei der MP4-Vorbereitung verändert.");

        foreach (var pair in original)
        {
            if (!prepared.TryGetValue(pair.Key, out var preparedCount))
                throw new InvalidOperationException(
                    $"Der Videostream {pair.Key} fehlt in der MP4-Arbeitsdatei.");

            if (pair.Value != preparedCount)
                throw new InvalidOperationException(
                    $"Die Videopaket-Anzahl von Stream {pair.Key} hat sich verändert: " +
                    $"{pair.Value} → {preparedCount}. Der Schnitt wurde gestoppt.");
        }
    }

    public static void Validate(
        MediaAnalysisResult original,
        MediaAnalysisResult prepared,
        Action<string>? reportDetail = null,
        IReadOnlyDictionary<int, long>? originalVideoPacketCounts = null,
        IReadOnlyDictionary<int, long>? preparedVideoPacketCounts = null)
    {
        if (!IsMp4(prepared) ||
            original.VideoStreams.Count != prepared.VideoStreams.Count ||
            original.AudioStreams.Count != prepared.AudioStreams.Count)
            throw new InvalidOperationException(
                "Die MP4-Vorbereitung hat die Stream-Struktur verändert. Der Schnitt wurde gestoppt.");

        if ((originalVideoPacketCounts is null) !=
            (preparedVideoPacketCounts is null))
            throw new ArgumentException(
                "Videopaket-Anzahlen müssen für Original und Arbeitsdatei gemeinsam angegeben werden.");

        var videoPacketsVerified =
            originalVideoPacketCounts is not null &&
            preparedVideoPacketCounts is not null;

        if (videoPacketsVerified)
        {
            ValidateVideoPacketCounts(
                originalVideoPacketCounts!,
                preparedVideoPacketCounts!);

            reportDetail?.Invoke(
                "Videopaket-Anzahl vollständig und exakt verifiziert.");
        }

        var fps = videoPacketsVerified
            ? CutMediaAnalysisValidator.GetFramesPerSecond(prepared)
            : CutMediaAnalysisValidator.GetFramesPerSecond(original);

        if (original.Duration is not { } duration ||
            prepared.Duration is not { } preparedDuration ||
            duration <= TimeSpan.Zero ||
            preparedDuration <= TimeSpan.Zero ||
            Math.Abs((duration - preparedDuration).TotalSeconds) >
            Math.Max(0.1, 2 / fps) + 0.000001)
            throw new InvalidOperationException(
                "Die Laufzeit der Arbeitsdatei stimmt nicht ausreichend mit dem Original überein. Der Schnitt wurde gestoppt.");

        if (duration != preparedDuration)
            reportDetail?.Invoke(
                $"Laufzeitabweichung innerhalb der Toleranz: {(preparedDuration - duration).TotalSeconds:0.######} s.");

        foreach (var pair in original.VideoStreams.Zip(prepared.VideoStreams))
        {
            ValidateStart(
                pair.First.StartTimeSeconds,
                original.StartTimeSeconds,
                pair.Second.StartTimeSeconds,
                prepared.StartTimeSeconds,
                2 / fps,
                reportDetail);

            if (!videoPacketsVerified)
            {
                if (pair.First.FrameCount is > 0 &&
                    pair.Second.FrameCount is > 0)
                {
                    var difference =
                        (decimal)pair.Second.FrameCount.Value -
                        pair.First.FrameCount.Value;

                    if (Math.Abs(difference) > 2)
                        throw new InvalidOperationException(
                            "Die Bildanzahl weicht um mehr als zwei Frames ab. Der Schnitt wurde gestoppt.");

                    if (difference != 0)
                        reportDetail?.Invoke(
                            $"Frame-Angaben: Original {pair.First.FrameCount}, MP4 {pair.Second.FrameCount}; Abweichung {difference} innerhalb der Toleranz.");
                }
                else
                {
                    reportDetail?.Invoke(
                        "Frame-Anzahl nicht vergleichbar: Containerangabe fehlt. Übrige Prüfungen werden angewendet.");
                }
            }

            if (pair.First.CodecName != pair.Second.CodecName ||
                pair.First.Width != pair.Second.Width ||
                pair.First.Height != pair.Second.Height)
                throw new InvalidOperationException(
                    "Videocodec oder Auflösung haben sich verändert. Der Schnitt wurde gestoppt.");

            if (videoPacketsVerified)
            {
                if (pair.Second.FramesPerSecond is not { } after ||
                    !double.IsFinite(after) ||
                    after <= 0)
                    throw new InvalidOperationException(
                        "Für die MP4-Arbeitsdatei wurde keine gültige Bildrate ermittelt.");

                if (pair.First.FramesPerSecond is { } before &&
                    double.IsFinite(before) &&
                    before > 0 &&
                    before != after)
                    reportDetail?.Invoke(
                        $"Container-Bildrate: Original {before:0.######}, MP4 {after:0.######}; wegen exakt verifizierter Videopakete wird die abweichende Quellangabe nicht als Bildverlust gewertet.");
            }
            else
            {
                if (pair.First.FramesPerSecond is not { } before ||
                    pair.Second.FramesPerSecond is not { } after ||
                    !double.IsFinite(before) ||
                    !double.IsFinite(after) ||
                    before <= 0 ||
                    after <= 0 ||
                    Math.Abs(before - after) * duration.TotalSeconds >
                    2.000001)
                    throw new InvalidOperationException(
                        "Videocodec, Auflösung oder Bildrate haben sich verändert. Der Schnitt wurde gestoppt.");

                if (before != after)
                    reportDetail?.Invoke(
                        $"Mittlere Bildrate: {before:0.######} → {after:0.######}; rechnerische Abweichung über die Laufzeit höchstens zwei Frames.");
            }
        }

        foreach (var pair in original.AudioStreams.Zip(prepared.AudioStreams))
        {
            ValidateStart(
                pair.First.StartTimeSeconds,
                original.StartTimeSeconds,
                pair.Second.StartTimeSeconds,
                prepared.StartTimeSeconds,
                0.1,
                reportDetail);

            if (pair.First.CodecName != pair.Second.CodecName)
                reportDetail?.Invoke(
                    $"Audio-Codec-Bezeichnung: {pair.First.CodecName} → {pair.Second.CodecName}; " +
                    "bei unveränderter Abtastrate und Kanalzahl wird dies nicht als Datenverlust gewertet.");

            if (pair.First.SampleRate != pair.Second.SampleRate ||
                pair.First.Channels != pair.Second.Channels)
                throw new InvalidOperationException(
                    "Die Audiodaten haben sich verändert. Der Schnitt wurde gestoppt.");
        }
    }

    private static void ValidateStart(double? sourceStart, double? sourceOrigin,
        double? targetStart, double? targetOrigin, double tolerance, Action<string>? reportDetail)
    {
        if (sourceStart is not { } s || targetStart is not { } t ||
            sourceOrigin is not { } so || targetOrigin is not { } to ||
            !double.IsFinite(s) || !double.IsFinite(t) || !double.IsFinite(so) || !double.IsFinite(to))
        {
            reportDetail?.Invoke("Stream-Startzeit nicht vergleichbar: Metadaten fehlen. Übrige Prüfungen werden angewendet.");
            return;
        }
        var difference = (t - to) - (s - so);
        if (Math.Abs(difference) > tolerance + 0.000001)
            throw new InvalidOperationException("Die zeitliche Zuordnung der Arbeitsdatei weicht deutlich ab. Der Schnitt wurde gestoppt.");
        if (difference != 0)
            reportDetail?.Invoke($"Stream-Startabweichung innerhalb der Toleranz: {difference:0.######} s.");
    }

    public static async Task PrepareAsync(string source, string destination, MediaAnalysisResult original,
        string ffmpeg, string ffprobe, IProgress<CutProgressUpdate> progress, CancellationToken token)
    {
        if (!File.Exists(ffprobe))
            throw new FileNotFoundException("Bitte unter FFmpeg-Werkzeuge einen gültigen Pfad zu ffprobe.exe einstellen.", ffprobe);
        var packetCounter = new FfprobePacketCounter(ffprobe);

        progress.Report(new(
            CutProgressKind.Status,
            "Videopakete der Quelldatei werden geprüft …"));

        var originalVideoPacketCounts =
            await packetCounter.RunAsync(source, token);

        await new FfmpegRemuxRunner(ffmpeg)
            .RunAsync(source, destination, progress, token);

        progress.Report(new(
            CutProgressKind.Status,
            "MP4-Arbeitsdatei wird geprüft …"));

        var prepared =
            await new FfprobeRunner(ffprobe)
                .RunAsync(destination, token);

        var preparedVideoPacketCounts =
            await packetCounter.RunAsync(destination, token);

        Validate(
            original,
            prepared,
            detail => progress.Report(
                new(CutProgressKind.Output, detail)),
            originalVideoPacketCounts,
            preparedVideoPacketCounts);

        progress.Report(new(
            CutProgressKind.Output,
            "Stream-, Videopaket-, Bildraten- und Laufzeitprüfung bestanden. Bitte Schnittstellen und Ton-Synchronität im Ergebnis kontrollieren."));
    }
}
