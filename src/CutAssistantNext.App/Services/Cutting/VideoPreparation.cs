using System.IO;
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
            "avi" => "AVI", "matroska" => "Matroska (MKV)",
            "mpegts" => "MPEG-TS", "mpeg" => "MPEG-PS", "asf" => "ASF/WMV",
            _ => analysis.FormatLongName ?? analysis.FormatName ?? "unbekannter Container"
        };

    public static void Validate(MediaAnalysisResult original, MediaAnalysisResult prepared,
        Action<string>? reportDetail = null)
    {
        if (!IsMp4(prepared) || original.VideoStreams.Count != prepared.VideoStreams.Count ||
            original.AudioStreams.Count != prepared.AudioStreams.Count)
            throw new InvalidOperationException("Die MP4-Vorbereitung hat die Stream-Struktur verändert. Der Schnitt wurde gestoppt.");
        var fps = CutMediaAnalysisValidator.GetFramesPerSecond(original);
        if (original.Duration is not { } duration || prepared.Duration is not { } preparedDuration ||
            duration <= TimeSpan.Zero || preparedDuration <= TimeSpan.Zero ||
            Math.Abs((duration - preparedDuration).TotalSeconds) > Math.Max(0.1, 2 / fps) + 0.000001)
            throw new InvalidOperationException("Die Laufzeit der Arbeitsdatei stimmt nicht ausreichend mit dem Original überein. Der Schnitt wurde gestoppt.");
        if (duration != preparedDuration)
            reportDetail?.Invoke($"Laufzeitabweichung innerhalb der Toleranz: {(preparedDuration - duration).TotalSeconds:0.######} s.");
        foreach (var pair in original.VideoStreams.Zip(prepared.VideoStreams))
        {
            ValidateStart(pair.First.StartTimeSeconds, original.StartTimeSeconds,
                pair.Second.StartTimeSeconds, prepared.StartTimeSeconds, 2 / fps, reportDetail);
            if (pair.First.FrameCount is > 0 && pair.Second.FrameCount is > 0)
            {
                var difference = (decimal)pair.Second.FrameCount.Value - pair.First.FrameCount.Value;
                if (Math.Abs(difference) > 2)
                    throw new InvalidOperationException("Die Bildanzahl weicht um mehr als zwei Frames ab. Der Schnitt wurde gestoppt.");
                if (difference != 0)
                    reportDetail?.Invoke($"Frame-Angaben: Original {pair.First.FrameCount}, MP4 {pair.Second.FrameCount}; Abweichung {difference} innerhalb der Toleranz.");
            }
            else
                reportDetail?.Invoke("Frame-Anzahl nicht vergleichbar: Containerangabe fehlt. Übrige Prüfungen werden angewendet.");
            if (pair.First.CodecName != pair.Second.CodecName ||
                pair.First.Width != pair.Second.Width || pair.First.Height != pair.Second.Height ||
                pair.First.FramesPerSecond is not { } before || pair.Second.FramesPerSecond is not { } after ||
                !double.IsFinite(before) || !double.IsFinite(after) || before <= 0 || after <= 0 ||
                // Compare against the original timeline. The output container can include
                // a small additional stream offset (e.g. 20 ms for the Diplomatin AVI).
                Math.Abs(before - after) * duration.TotalSeconds > 2.000001)
                throw new InvalidOperationException("Videocodec, Auflösung oder Bildrate haben sich verändert. Der Schnitt wurde gestoppt.");
            if (before != after)
                reportDetail?.Invoke($"Mittlere Bildrate: {before:0.######} → {after:0.######}; rechnerische Abweichung über die Laufzeit höchstens zwei Frames.");
        }
        foreach (var pair in original.AudioStreams.Zip(prepared.AudioStreams))
        {
            ValidateStart(pair.First.StartTimeSeconds, original.StartTimeSeconds,
                pair.Second.StartTimeSeconds, prepared.StartTimeSeconds, 0.1, reportDetail);
            if (pair.First.CodecName != pair.Second.CodecName || pair.First.SampleRate != pair.Second.SampleRate ||
                pair.First.Channels != pair.Second.Channels)
                throw new InvalidOperationException("Die Audiodaten haben sich verändert. Der Schnitt wurde gestoppt.");
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
        string ffmpeg, string ffprobe, IProgress<Mp4BoxProgressUpdate> progress, CancellationToken token)
    {
        if (!File.Exists(ffprobe))
            throw new FileNotFoundException("Bitte unter FFmpeg-Werkzeuge einen gültigen Pfad zu ffprobe.exe einstellen.", ffprobe);
        await new FfmpegRemuxRunner(ffmpeg).RunAsync(source, destination, progress, token);
        progress.Report(new(Mp4BoxProgressKind.Status, "MP4-Arbeitsdatei wird geprüft …"));
        var prepared = await new FfprobeRunner(ffprobe).RunAsync(destination, token);
        Validate(original, prepared, detail => progress.Report(new(Mp4BoxProgressKind.Output, detail)));
        progress.Report(new(Mp4BoxProgressKind.Output, "Stream-, Bildraten- und Laufzeitprüfung bestanden. Bitte Schnittstellen und Ton-Synchronität im Ergebnis kontrollieren."));
    }
}
