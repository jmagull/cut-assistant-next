using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Joining;

/// <summary>Pragmatic FFmpeg streamcopy. Warnings are information, not a sample-accurate guarantee.</summary>
public sealed class QuickCopyJoinService(string ffmpeg, string ffprobe, ICutToolRunner? runner = null,
    bool measurePhases = true, IProgress<JoinPhaseTiming>? phaseTimings = null, IQuickCopyMediaProbe? mediaProbe = null)
{
    private readonly ICutToolRunner _runner = runner ?? new BoundedJoinToolRunner();
    private readonly IQuickCopyMediaProbe _probe = mediaProbe ?? new QuickCopyMediaProbe(ffprobe);

    public async Task<CutResult> RunAsync(QuickCopyJoinRequest request, IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(request.Tracks);
        request = request with { Tracks = request.Tracks.ToArray() };
        request.ValidateSelection();
        Mp4JoinService.ValidatePaths(new(request.FirstPath, request.Tracks[0].FirstAudioIndex,
            request.SecondPath, request.Tracks[0].SecondAudioIndex, request.OutputPath));
        var work = Path.Combine(Path.GetDirectoryName(request.OutputPath)!, ".can-quick-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        using var log = new OperationLog(Path.Combine(work, "protocol.txt"), progress);
        using var notes = new TechnicalNotes(Path.Combine(work, "notes.txt"));
        var watch = Stopwatch.StartNew();
        var timing = new JoinPhaseTimer(measurePhases, value => { log.Timing(value); phaseTimings?.Report(value); });
        var completed = false;
        CutResult? result = null;
        try
        {
            using var firstLock = new FileStream(request.FirstPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var secondLock = new FileStream(request.SecondPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            log.Status("QUICK AND COPY: Filmteile und bestätigte Tonspurzuordnung werden vorbereitet …");
            log.Status("Video und ausgewählte Tonspuren werden ohne Neukodierung übernommen. Bitte den fertigen Film persönlich kontrollieren.");
            log.Status("Ressourcengrenzen für FFmpeg: 1 GiB Arbeitsspeicher, 25 % CPU, jeweils ein Verarbeitungsthread.");
            var paths = new[] { request.FirstPath, request.SecondPath };
            var parts = new QuickCopyMediaInfo[2];
            for (var i = 0; i < 2; i++)
            {
                parts[i] = await timing.RunAsync($"inspect-input-{i + 1}", () => _probe.InspectAsync(paths[i], token)).ConfigureAwait(false);
                foreach (var hint in parts[i].Hints) notes.Add($"Teil {i + 1}: {hint}");
                foreach (var pair in request.Tracks)
                {
                    var a = parts[i].Audio(i == 0 ? pair.FirstAudioIndex : pair.SecondAudioIndex);
                    log.Status($"Teil {i + 1}: gewählte Spur {a.Index}: {a.CodecName}, {a.Language}, {a.Channels} Kanäle, {a.SampleRate} Hz.");
                }
            }
            QuickCopyMediaProbe.ValidateCompatibility(parts[0], parts[1], request.Tracks);
            if (parts[0].VideoConfiguration != parts[1].VideoConfiguration)
                notes.Add("Die H.264-Decoderparameter der Filmteile unterscheiden sich. FFmpeg übernimmt die Streamcopy-Verarbeitung.");
            var direct = QuickCopyMediaProbe.CanConcatDirectly(parts[0], parts[1], request.Tracks);
            var required = checked((direct ? 1L : 2L) * checked(firstLock.Length + secondLock.Length) + 64L * 1024 * 1024);
            if (new DriveInfo(Path.GetPathRoot(request.OutputPath)!).AvailableFreeSpace < required)
                throw new IOException($"Für QUICK AND COPY werden mindestens {required / (1024d * 1024 * 1024):F1} GiB freier Platz benötigt.");
            var inputs = paths;
            var transition = parts[0].Duration;
            if (!direct)
            {
                log.Status("Gewählte Spuren werden ohne Neukodierung für eine eindeutige Zuordnung vorbereitet …");
                inputs = [Path.Combine(work, "part-1.mp4"), Path.Combine(work, "part-2.mp4")];
                var clock = parts[0].VideoTimeBase.Split('/');
                var scale = clock.Length == 2 && long.TryParse(clock[1], out var denominator) && denominator > 0 && denominator <= int.MaxValue
                    ? denominator.ToString(CultureInfo.InvariantCulture) : "90000";
                for (var i = 0; i < 2; i++)
                {
                    var args = new List<string> { "-copyts", "-i", paths[i], "-map", $"0:{parts[i].Video.Index}" };
                    foreach (var pair in request.Tracks) args.AddRange(["-map", $"0:{(i == 0 ? pair.FirstAudioIndex : pair.SecondAudioIndex)}"]);
                    args.AddRange(["-c", "copy", "-video_track_timescale", scale, "-avoid_negative_ts", "disabled", inputs[i]]);
                    await timing.RunAsync($"prepare-copy-{i + 1}", () => Run(args, log, notes, token)).ConfigureAwait(false);
                }
                var prepared = await timing.RunAsync("inspect-prepared-boundary", () => _probe.InspectAsync(inputs[0], token)).ConfigureAwait(false);
                foreach (var hint in prepared.Hints) notes.Add("Vorbereitung Teil 1: " + hint);
                transition = prepared.Duration;
            }
            var listing = Path.Combine(work, "parts.ffconcat");
            await File.WriteAllTextAsync(listing, "ffconcat version 1.0\n" + string.Concat(inputs.Select(p =>
                $"file '{Mp4JoinService.EscapeConcatPath(p)}'\n")), new UTF8Encoding(false), token).ConfigureAwait(false);
            var pending = Path.Combine(work, "result.mp4");
            var concat = new List<string> { "-f", "concat", "-safe", "0", "-i", listing, "-map", direct ? $"0:{parts[0].Video.Index}" : "0:0" };
            for (var j = 0; j < request.Tracks.Count; j++) concat.AddRange(["-map", direct ? $"0:{request.Tracks[j].FirstAudioIndex}" : $"0:{j + 1}"]);
            concat.AddRange(["-c", "copy", "-movflags", "+faststart", pending]);
            log.Status("QUICK AND COPY: Filmteile werden mit FFmpeg ohne Neukodierung zusammengefügt …");
            await timing.RunAsync("ffmpeg-concat", () => Run(concat, log, notes, token)).ConfigureAwait(false);
            if (!File.Exists(pending) || new FileInfo(pending).Length == 0)
                throw new InvalidDataException("FFmpeg hat keine lesbare Ausgabedatei erzeugt.");
            var output = await timing.RunAsync("inspect-output", () => _probe.InspectAsync(pending, token)).ConfigureAwait(false);
            foreach (var hint in output.Hints) notes.Add("Ausgabe: " + hint);
            ValidateOutput(parts[0], output, request.Tracks);
            var referenceDuration = parts[0].Duration + parts[1].Duration;
            if (output.Duration != referenceDuration)
                notes.Add($"Containerlaufzeiten: Summe der Filmteile {referenceDuration.TotalSeconds.ToString("0.000000", CultureInfo.InvariantCulture)} s; Ausgabe {output.Duration.TotalSeconds.ToString("0.000000", CultureInfo.InvariantCulture)} s. Keine samplegenaue Tonprüfung im QUICK-Modus.");
            if (transition <= TimeSpan.Zero || transition >= output.Duration)
            {
                transition = TimeSpan.FromTicks((long)(output.Duration.Ticks * (parts[0].Duration.TotalSeconds / (parts[0].Duration + parts[1].Duration).TotalSeconds)));
                notes.Add("Die Übergangsposition wurde wegen abweichender Laufzeitangaben näherungsweise berechnet.");
            }
            token.ThrowIfCancellationRequested();
            var info = new FileInfo(pending);
            string hash;
            using (var file = File.OpenRead(pending))
                hash = Convert.ToHexString(await timing.RunAsync("output-sha256", () => SHA256.HashDataAsync(file, token).AsTask()).ConfigureAwait(false));
            var identity = new CutOutputIdentity(info.Length, info.LastWriteTimeUtc.Ticks, hash);
            var timeline = new CutOutputTimeline(output.Duration, [transition], identity);
            token.ThrowIfCancellationRequested();
            timing.Run("publish-output", () => File.Move(pending, request.OutputPath, overwrite: false));
            result = new(request.OutputPath, timeline, CutReviewTimingKind.Estimated,
                "QUICK AND COPY: Filmteile zusammengefügt. Video und ausgewählte Tonspuren wurden ohne Neukodierung übernommen.\n" +
                "Gegebenenfalls vorhandene technische Hinweise stehen in der Meckerecke des Protokolls. Bitte Bild und Ton insbesondere an der Klebestelle kontrollieren.");
            completed = true;
            log.Status("QUICK AND COPY abgeschlossen. Bitte den fertigen Film kontrollieren.");
        }
        catch (Exception error)
        {
            log.Status("Abgebrochen: " + error.Message);
            log.Status("Keine automatische Umstellung auf AUDIO-ENTHUSIAST. Diagnoseprotokoll: " + log.Path);
            throw;
        }
        finally
        {
            try { timing.Run("technical-notes", () => notes.AppendTo(log)); }
            finally
            {
                notes.Dispose(); log.Dispose();
                try { timing.Run("cleanup", () => Cleanup(work, completed, progress)); }
                finally { watch.Stop(); timing.Record("total", watch.Elapsed, completed); }
            }
        }
        var total = $"Gesamtzeit QUICK AND COPY: {watch.Elapsed:hh\\:mm\\:ss\\.ff}.";
        progress?.Report(new(CutProgressKind.Status, total));
        return new(result!.OutputFilePath, result.Timeline, result.TimingKind, result.Notice + "\n" + total);
    }

    private static void ValidateOutput(QuickCopyMediaInfo first, QuickCopyMediaInfo output, IReadOnlyList<QuickCopyTrackPair> pairs)
    {
        if (output.Video.Index != 0 || output.Media.AudioStreams.Count != pairs.Count ||
            !output.Media.AudioStreams.Select(a => a.Index).SequenceEqual(Enumerable.Range(1, pairs.Count)))
            throw new InvalidDataException("Die Ausgabe enthält nicht die ausgewählten Bild- und Tonspuren.");
        var mapped = pairs.Select((p, i) => new QuickCopyTrackPair(p.FirstAudioIndex, i + 1)).ToArray();
        QuickCopyMediaProbe.ValidateCompatibility(first, output, mapped);
    }

    private async Task Run(IReadOnlyList<string> args, OperationLog log, TechnicalNotes notes, CancellationToken token)
    {
        string[] arguments = ["-hide_banner", "-nostdin", "-nostats", "-v", "warning", "-n", "-progress", "pipe:1", "-stats_period", "1", "-threads", "1", .. args];
        log.Report(new(CutProgressKind.Output, "Werkzeug: " + ffmpeg + " " + System.Text.Json.JsonSerializer.Serialize(arguments)));
        await _runner.RunAsync(ffmpeg, arguments, new CopyOutput(log, notes), token).ConfigureAwait(false);
    }

    internal static void Cleanup(string work, bool completed, IProgress<CutProgressUpdate>? progress)
    {
        var clean = true;
        foreach (var name in new[] { "decoded.pcm", "part-1.mp4", "part-2.mp4", "parts.ffconcat", "result.mp4", "notes.txt" })
            try { File.Delete(Path.Combine(work, name)); }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            { clean = false; progress?.Report(new(CutProgressKind.Output, "Arbeitsdatei erhalten: " + name + ": " + e.Message)); }
        if (!completed || !clean) return;
        try
        {
            var log = Path.Combine(work, "protocol.txt");
            if (Directory.EnumerateFileSystemEntries(work).Any(p => !p.Equals(log, StringComparison.OrdinalIgnoreCase)))
            { progress?.Report(new(CutProgressKind.Output, "Unbekannte Dateien im Arbeitsordner bleiben erhalten: " + work)); return; }
            File.Delete(log); Directory.Delete(work, recursive: false);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        { progress?.Report(new(CutProgressKind.Output, "Arbeitsordner bleibt erhalten: " + work + ": " + e.Message)); }
    }

    private sealed class CopyOutput(OperationLog log, TechnicalNotes notes) : IProgress<CutProgressUpdate>
    {
        public void Report(CutProgressUpdate value)
        {
            if (value.Kind == CutProgressKind.Output && !string.IsNullOrWhiteSpace(value.Message)) notes.Add(value.Message);
            log.Report(value);
        }
    }

    private sealed class TechnicalNotes(string path) : IDisposable
    {
        private readonly StreamWriter _writer = new(path, false, new UTF8Encoding(false)) { AutoFlush = true };
        private readonly object _gate = new();
        private long _count;
        public void Add(string message) { lock (_gate) { _writer.WriteLine(message); _count++; } }
        public void AppendTo(OperationLog log)
        {
            lock (_gate)
            {
                _writer.Dispose();
                log.Status("Meckerecke – technische Hinweise");
                if (_count == 0) log.Report(new(CutProgressKind.Output, "Keine technischen Hinweise gemeldet."));
                else foreach (var line in File.ReadLines(path)) log.Report(new(CutProgressKind.Output, line));
            }
        }
        public void Dispose() { lock (_gate) _writer.Dispose(); }
    }

    private sealed class OperationLog : IProgress<CutProgressUpdate>, IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly IProgress<CutProgressUpdate>? _target;
        private readonly object _gate = new();
        private bool _closed;
        private string _phase = "";
        internal string Path { get; }
        internal OperationLog(string path, IProgress<CutProgressUpdate>? target)
        { Path = path; _target = target; _writer = new(path, false, new UTF8Encoding(false)) { AutoFlush = true }; }
        internal void Status(string text) { _phase = text; Report(new(CutProgressKind.Status, text)); }
        public void Report(CutProgressUpdate value)
        {
            if (value.Kind == CutProgressKind.Progress) value = value with { Message = _phase + " · " + value.Message };
            lock (_gate) { _writer.WriteLine(value.Message); }
            _target?.Report(value);
        }
        internal void Timing(JoinPhaseTiming value)
        {
            lock (_gate) { if (!_closed) _writer.WriteLine(value.ToProtocolLine()); }
            _target?.Report(new(CutProgressKind.Output, value.ToProtocolLine()));
        }
        public void Dispose() { lock (_gate) { if (!_closed) { _closed = true; _writer.Dispose(); } } }
    }
}
