using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Joining;

public sealed record Mp4JoinRequest(string FirstPath, int FirstAudioIndex, string SecondPath, int SecondAudioIndex, string OutputPath);

/// <summary>Two whole files, no cutlist. Only a completely proven result is published.</summary>
public sealed class Mp4JoinService(string ffmpeg, string ffprobe, ICutToolRunner? runner = null,
    bool measurePhases = true, IProgress<JoinPhaseTiming>? phaseTimings = null)
{
    private readonly ICutToolRunner _runner = runner ?? new BoundedJoinToolRunner();
    private readonly FfprobeStreamCopyVerifier _verifier = new(ffprobe);

    public async Task<CutResult> RunAsync(Mp4JoinRequest request, IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        ValidatePaths(request);
        var parent = Path.GetDirectoryName(request.OutputPath)!;
        var work = Path.Combine(parent, ".can-join-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        var log = new JoinLog(Path.Combine(work, "protocol.txt"), progress);
        var completed = false;
        var overall = Stopwatch.StartNew();
        var timing = new JoinPhaseTimer(measurePhases, value =>
        {
            log.Timing(value);
            phaseTimings?.Report(value);
        });
        try
        {
            // Locks cover inspection, decoding, packet proof and final publication.
            using var firstLock = new FileStream(request.FirstPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var secondLock = new FileStream(request.SecondPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (await Mp4BoxOriginDetector.IsMp4BoxOutputAsync(request.FirstPath, token).ConfigureAwait(false) ||
                await Mp4BoxOriginDetector.IsMp4BoxOutputAsync(request.SecondPath, token).ConfigureAwait(false))
                throw new InvalidDataException(Mp4BoxOriginDetector.EnthusiastUnavailableMessage);
            log.Status("Filmteile und ausgewählte Tonspuren werden geprüft …");
            var first = await timing.RunAsync("inspect-input-1", () => _verifier.InspectJoinPartAsync(request.FirstPath, request.FirstAudioIndex, token)).ConfigureAwait(false);
            var second = await timing.RunAsync("inspect-input-2", () => _verifier.InspectJoinPartAsync(request.SecondPath, request.SecondAudioIndex, token)).ConfigureAwait(false);
            FfprobeStreamCopyVerifier.ValidateJoinCompatibility(first, second);
            log.Status($"Teil 1: {first.Path}; Spur {first.AudioIndex}: {first.AudioCodec}, {first.Language}, {first.Layout}, {first.SampleRate} Hz.");
            log.Status($"Teil 2: {second.Path}; Spur {second.AudioIndex}: {second.AudioCodec}, {second.Language}, {second.Layout}, {second.SampleRate} Hz.");
            var samples = checked(first.AudioSamples + second.AudioSamples);
            var pcmBytes = checked(samples * first.Channels * 4);
            var required = RequiredFreeBytes(firstLock.Length, secondLock.Length, pcmBytes);
            CheckSpace(parent, required);
            log.Status("Bildformate passen. Bild bleibt unverändert; gewählter Ton wird einmal durchgehend als AAC kodiert.");
            log.Status("Ressourcengrenzen für FFmpeg: 1 GiB Arbeitsspeicher, 25 % CPU, jeweils ein Verarbeitungsthread.");
            var parts = new[] { first, second };
            var pcms = new string[2]; var videos = new string[2];
            for (var i = 0; i < 2; i++)
            {
                var part = parts[i];
                log.Status($"Teil {i + 1} von 2: Bildzeiten und Bildpakete werden geprüft …");
                await timing.RunAsync($"video-clock-{i + 1}", () => _verifier.ValidateJoinVideoAsync(part, token)).ConfigureAwait(false);
                pcms[i] = Path.Combine(work, $"audio-{i}.pcm");
                log.Status($"Teil {i + 1} von 2: Zeitstempel der gewählten Tonspur werden auf Lücken und Überlappungen geprüft …");
                await timing.RunAsync($"audio-clock-{i + 1}", () => _verifier.ValidateJoinSourceAudioAsync(part, token)).ConfigureAwait(false);
                log.Status($"Teil {i + 1} von 2: gewählte Tonspur {part.AudioIndex} wird vollständig dekodiert …");
                await timing.RunAsync($"audio-decode-{i + 1}", () => Run(["-threads", "1", "-i", part.Path, "-map", $"0:{part.AudioIndex}", "-vn", "-sn", "-dn",
                    "-c:a", "pcm_f32le", "-f", "f32le", pcms[i]], log, token)).ConfigureAwait(false);
                RequirePcm(pcms[i], part.AudioSamples, part.Channels);
                videos[i] = Path.Combine(work, $"video-{i}.mp4");
                await timing.RunAsync($"video-copy-{i + 1}", () => Run(["-copyts", "-i", part.Path, "-map", $"0:{part.VideoIndex}", "-c:v", "copy", "-an",
                    "-video_track_timescale", first.VideoTimeBase.ToString(CultureInfo.InvariantCulture),
                    "-avoid_negative_ts", "disabled", videos[i]], log, token)).ConfigureAwait(false);
                await timing.RunAsync($"video-copy-proof-{i + 1}", () => _verifier.VerifyAsync(part.Path, videos[i], [(part.VideoIndex, 0)], token)).ConfigureAwait(false);
            }
            var wave = Path.Combine(work, "joined.wav");
            log.Status("Ton wird samplegenau verbunden und vollständig mit beiden Ausgangsspuren verglichen …");
            await timing.RunAsync("pcm-rf64-and-proof", () => JoinWaveAsync(pcms, wave, first, samples, token)).ConfigureAwait(false);
            foreach (var file in pcms) File.Delete(file);
            CheckSpace(parent, checked(2 * (firstLock.Length + secondLock.Length) + pcmBytes + 1024L * 1024 * 1024));
            var audio = Path.Combine(work, "continuous.m4a");
            log.Status("Durchgehende AAC-Tonspur wird einmal kodiert …");
            await timing.RunAsync("aac-encode", () => Run(["-i", wave,
                "-c:a", "aac", "-profile:a", "aac_low", "-b:a", first.Channels > 2 ? "384k" : "192k", "-threads:a", "1",
                "-metadata:s:a:0", "language=" + (first.Language.Length == 0 ? "und" : first.Language), audio], log, token)).ConfigureAwait(false);
            var list = Path.Combine(work, "parts.ffconcat");
            await File.WriteAllTextAsync(list, "ffconcat version 1.0\n" +
                string.Concat(videos.Select((v, i) => $"file '{EscapeConcatPath(v)}'\nduration {parts[i].Duration.TotalSeconds.ToString("0.0000000", CultureInfo.InvariantCulture)}\n")),
                new UTF8Encoding(false), token).ConfigureAwait(false);
            var pending = Path.Combine(work, "result.mp4");
            log.Status("Bildteile und durchgehender Ton werden in der neuen MP4 zusammengeführt …");
            await timing.RunAsync("final-mux", () => Run(["-copyts", "-f", "concat", "-safe", "0", "-auto_convert", "0", "-i", list,
                "-i", audio, "-map", "0:v:0", "-map", "1:a:0", "-c", "copy", "-map_metadata", "1",
                "-avoid_negative_ts", "disabled", "-movflags", "+faststart", pending], log, token)).ConfigureAwait(false);
            log.Status("Alle Bildpakete, Bildzeiten, AAC-Pakete und Wiedergabezeiten werden nachgeprüft …");
            var actual = await timing.RunAsync("inspect-output", () => _verifier.InspectJoinPartAsync(pending, 1, token)).ConfigureAwait(false);
            if (actual.AudioCodec != "aac" || actual.AudioProfile != "LC") throw new InvalidDataException("Die erwartete AAC-LC-Ausgabe fehlt.");
            FfprobeStreamCopyVerifier.ValidateJoinCompatibility(first, actual);
            await timing.RunAsync("final-video-proof", () => _verifier.VerifyJoinedVideoAsync(first, second, pending, token)).ConfigureAwait(false);
            await timing.RunAsync("final-aac-proof", () => _verifier.VerifyAsync(audio, pending, [(0, 1)], token)).ConfigureAwait(false);
            await timing.RunAsync("final-audio-clock", () => _verifier.VerifyContinuousJoinAudioAsync(pending, samples, first.SampleRate, token)).ConfigureAwait(false);
            var decoded = Path.Combine(work, "verified.pcm");
            log.Status("Fertiger Ton wird vollständig dekodiert und auf exakte Samplezahl geprüft …");
            await timing.RunAsync("final-audio-decode", () => Run(["-threads", "1", "-i", pending, "-map", "0:a:0", "-c:a", "pcm_f32le", "-f", "f32le", decoded], log, token)).ConfigureAwait(false);
            RequirePcm(decoded, samples, first.Channels);
            log.Status("Die Klebestelle wird mit drei Sekunden Vor- und Nachlauf dekodiert …");
            await timing.RunAsync("transition-decode", () => Run(["-threads", "1", "-ss", Math.Max(0, first.Duration.TotalSeconds - 3).ToString("0.0000000", CultureInfo.InvariantCulture),
                "-i", pending, "-t", "6", "-map", "0:v:0", "-map", "0:a:0", "-f", "null", "-"], log, token)).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            var info = new FileInfo(pending);
            using var verifiedFile = File.OpenRead(pending);
            var hash = Convert.ToHexString(await timing.RunAsync("output-sha256", () => SHA256.HashDataAsync(verifiedFile, token).AsTask()).ConfigureAwait(false));
            var identity = new CutOutputIdentity(info.Length, info.LastWriteTimeUtc.Ticks, hash);
            verifiedFile.Close();
            token.ThrowIfCancellationRequested();
            timing.Run("publish-output", () => File.Move(pending, request.OutputPath, overwrite: false));
            log.Status("Zusammenfügen und technische Prüfung abgeschlossen.");
            log.Status($"Geprüfte Tonsamples: {samples}; Klebestelle: {first.Duration}; Gesamtdauer: {first.Duration + second.Duration}.");
            var result = new CutResult(request.OutputPath, new(first.Duration + second.Duration, [first.Duration], identity),
                CutReviewTimingKind.Measured, "Bild unverändert übernommen. Je Teil nur die ausgewählte Tonspur verwendet; übrige Spuren entfallen. " +
                "Ton einmal durchgehend als AAC neu kodiert. Bitte die Klebestelle auf Bild, Ton und Synchronität kontrollieren.");
            completed = true;
            return result;
        }
        catch (Exception error)
        {
            log.Status("Abgebrochen: " + error.Message);
            progress?.Report(new(CutProgressKind.Output, $"Diagnoseprotokoll: {log.Path}"));
            throw;
        }
        finally
        {
            log.Dispose();
            try { timing.Run("cleanup", () => CleanupWorkspace(work, completed, progress)); }
            finally { timing.Record("total", overall.Elapsed, completed); }
        }
    }

    internal static void CleanupWorkspace(string work, bool completed, IProgress<CutProgressUpdate>? progress)
    {
        // Only remove our known files; never recursively delete unexpected contents.
        var clean = true;
        foreach (var name in new[] { "audio-0.pcm", "audio-1.pcm", "joined.pcm", "joined.wav", "verified.pcm", "video-0.mp4",
            "video-1.mp4", "continuous.m4a", "parts.ffconcat", "result.mp4" })
        {
            try { File.Delete(Path.Combine(work, name)); }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                clean = false;
                progress?.Report(new(CutProgressKind.Output, $"Arbeitsdatei erhalten: {name}: {e.Message}"));
            }
        }
        if (!completed || !clean) return;
        try
        {
            var protocol = Path.Combine(work, "protocol.txt");
            if (Directory.EnumerateFileSystemEntries(work).Any(path => !path.Equals(protocol, StringComparison.OrdinalIgnoreCase)))
            {
                progress?.Report(new(CutProgressKind.Output, $"Arbeitsordner enthält weitere Dateien und bleibt erhalten: {work}"));
                return;
            }
            File.Delete(protocol);
            Directory.Delete(work, recursive: false);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            progress?.Report(new(CutProgressKind.Output, $"Arbeitsordner konnte nicht vollständig bereinigt werden: {work}: {e.Message}"));
        }
    }

    public static long RequiredFreeBytes(long firstBytes, long secondBytes, long pcmBytes) =>
        checked(4 * checked(firstBytes + secondBytes) + 3 * pcmBytes + 2L * 1024 * 1024 * 1024);

    public static void ValidatePaths(Mp4JoinRequest request)
    {
        foreach (var path in new[] { request.FirstPath, request.SecondPath, request.OutputPath })
            if (!Path.IsPathFullyQualified(path) || path.IndexOfAny(['\r', '\n', '\0']) >= 0 ||
                !Path.GetExtension(path).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Bitte vollständige MP4-Dateipfade ohne Zeilenumbrüche verwenden.");
        var paths = new[] { request.FirstPath, request.SecondPath, request.OutputPath }.Select(Path.GetFullPath).ToArray();
        if (paths.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 3)
            throw new ArgumentException("Bitte zwei unterschiedliche Filmteile und einen neuen Ausgabenamen wählen.");
        if (!File.Exists(paths[0]) || !File.Exists(paths[1])) throw new FileNotFoundException("Ein Filmteil fehlt.");
        if (File.Exists(paths[2]) || Directory.Exists(paths[2])) throw new IOException("Die Zieldatei existiert bereits. Bitte einen neuen Namen wählen.");
        if (!Directory.Exists(Path.GetDirectoryName(paths[2]))) throw new DirectoryNotFoundException("Der Ausgabeordner fehlt.");
    }

    public static string EscapeConcatPath(string path) => path.Replace('\\', '/').Replace("'", "'\\''");

    private static void CheckSpace(string parent, long needed)
    {
        var root = Path.GetPathRoot(parent)!;
        var drive = new DriveInfo(root);
        if (drive.AvailableFreeSpace < needed)
            throw new IOException($"Im Ausgabeordner werden mindestens {needed / (1024d * 1024 * 1024):F1} GiB freier Platz für Verarbeitung und Prüfung benötigt.");
    }

    private static void RequirePcm(string path, long samples, int channels)
    {
        if (new FileInfo(path).Length != checked(samples * channels * 4))
            throw new InvalidDataException("Die dekodierte Tonsamplezahl stimmt nicht mit der Bildlaufzeit überein.");
    }

    internal static async Task JoinWaveAsync(IReadOnlyList<string> inputs, string output,
        JoinPart part, long samples, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(samples);
        if (inputs.Count != 2) throw new ArgumentException("Genau zwei PCM-Filmteile werden benötigt.", nameof(inputs));
        var bytes = checked(samples * part.Channels * 4);
        var header = WaveHeader(bytes, part);
        long inputBytes = 0;
        foreach (var input in inputs)
        {
            var length = new FileInfo(input).Length;
            if (length == 0 || length % (part.Channels * 4) != 0)
                throw new InvalidDataException("Ein PCM-Filmteil enthält unvollständige Samples.");
            inputBytes = checked(inputBytes + length);
        }
        if (inputBytes != bytes) throw new InvalidDataException("Die PCM-Samplezahl stimmt nicht mit dem Schnittplan überein.");
        using var expected = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        await using (var target = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1024 * 1024, true))
        {
            await target.WriteAsync(header, token).ConfigureAwait(false);
            var buffer = new byte[1024 * 1024];
            long copied = 0;
            foreach (var path in inputs)
            {
                await using var source = File.OpenRead(path);
                int read;
                while ((read = await source.ReadAsync(buffer, token).ConfigureAwait(false)) > 0)
                {
                    copied = checked(copied + read);
                    if (copied > bytes) throw new InvalidDataException("Die PCM-Samplezahl hat sich beim Lesen verändert.");
                    expected.AppendData(buffer, 0, read);
                    await target.WriteAsync(buffer.AsMemory(0, read), token).ConfigureAwait(false);
                }
            }
            if (copied != bytes) throw new InvalidDataException("Beim PCM-Lesen fehlen Samples.");
        }
        await VerifyWaveAsync(output, part, samples, expected.GetHashAndReset(), token).ConfigureAwait(false);
    }

    internal static byte[] WaveHeader(long bytes, JoinPart part)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bytes);
        var mask = part.Layout switch
        {
            "mono" when part.Channels == 1 => 4u,
            "stereo" when part.Channels == 2 => 3u,
            "5.1" when part.Channels == 6 => 0x3fu,
            "5.1(side)" when part.Channels == 6 => 0x60fu,
            _ => throw new InvalidDataException("Unbekannte Kanalbelegung.")
        };
        if (part.SampleRate is not (44100 or 48000) || bytes % (part.Channels * 4) != 0)
            throw new InvalidDataException("Ungültiges PCM-Format oder unvollständige Samples.");
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true))
        {
            void Tag(string text) => writer.Write(Encoding.ASCII.GetBytes(text));
            Tag("RF64"); writer.Write(uint.MaxValue); Tag("WAVE");
            Tag("ds64"); writer.Write(28u); writer.Write(checked((ulong)bytes + 96)); writer.Write((ulong)bytes);
            writer.Write((ulong)(bytes / (part.Channels * 4))); writer.Write(0u);
            Tag("fmt "); writer.Write(40u); writer.Write((ushort)0xfffe); writer.Write((ushort)part.Channels);
            writer.Write((uint)part.SampleRate); writer.Write((uint)(part.SampleRate * part.Channels * 4));
            writer.Write((ushort)(part.Channels * 4)); writer.Write((ushort)32); writer.Write((ushort)22); writer.Write((ushort)32);
            writer.Write(mask);
            writer.Write(new Guid("00000003-0000-0010-8000-00aa00389b71").ToByteArray());
            Tag("data"); writer.Write(uint.MaxValue);
        }
        return stream.ToArray();
    }

    internal static async Task VerifyWaveAsync(string path, JoinPart part, long samples,
        byte[] expectedPayloadHash, CancellationToken token)
    {
        var bytes = checked(samples * part.Channels * 4);
        var header = WaveHeader(bytes, part);
        await using var source = File.OpenRead(path);
        if (source.Length != checked(bytes + header.Length))
            throw new InvalidDataException("Die RF64-Datei enthält eine abweichende Samplezahl.");
        var actualHeader = new byte[header.Length];
        await source.ReadExactlyAsync(actualHeader, token).ConfigureAwait(false);
        if (!actualHeader.AsSpan().SequenceEqual(header)) throw new InvalidDataException("Der RF64-Header wurde verändert.");
        var actual = await SHA256.HashDataAsync(source, token).ConfigureAwait(false);
        if (!actual.AsSpan().SequenceEqual(expectedPayloadHash)) throw new InvalidDataException("PCM-Vergleich fehlgeschlagen.");
    }

    private async Task Run(string[] args, JoinLog log, CancellationToken token)
    {
        var check = new WarningCheck(log);
        string[] arguments = ["-hide_banner", "-nostdin", "-nostats", "-v", "warning", "-xerror", "-n",
            "-progress", "pipe:1", "-stats_period", "1", .. args];
        log.Report(new(CutProgressKind.Output, "Werkzeug: " + ffmpeg + " " + System.Text.Json.JsonSerializer.Serialize(arguments)));
        await _runner.RunAsync(ffmpeg, arguments, check, token).ConfigureAwait(false);
        if (check.Warning) throw new InvalidDataException("FFmpeg hat eine Warnung gemeldet. Der Vorgang wurde zur Sicherheit abgebrochen; siehe Protokoll.");
    }

    private sealed class WarningCheck(JoinLog log) : IProgress<CutProgressUpdate>
    {
        private int _warning;
        public bool Warning => Volatile.Read(ref _warning) != 0;
        public void Report(CutProgressUpdate value)
        {
            if (value.Kind == CutProgressKind.Output && !string.IsNullOrWhiteSpace(value.Message)) Interlocked.Exchange(ref _warning, 1);
            log.Report(value);
        }
    }
    private sealed class JoinLog : IProgress<CutProgressUpdate>, IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly IProgress<CutProgressUpdate>? _target;
        private readonly object _gate = new();
        private string _phase = "";
        private bool _disposed;
        public string Path { get; }
        public JoinLog(string path, IProgress<CutProgressUpdate>? target)
        { Path = path; _target = target; _writer = new(path, false, new UTF8Encoding(false)) { AutoFlush = true }; }
        public void Status(string text) { _phase = text; Report(new(CutProgressKind.Status, text)); }
        public void Report(CutProgressUpdate value)
        {
            if (value.Kind == CutProgressKind.Progress) value = value with { Message = _phase + " · " + value.Message };
            lock (_gate) { _writer.WriteLine(value.Message); }
            _target?.Report(value);
        }
        public void Timing(JoinPhaseTiming value)
        {
            var text = value.ToProtocolLine();
            lock (_gate) { if (!_disposed) _writer.WriteLine(text); }
            _target?.Report(new(CutProgressKind.Output, text));
        }
        public void Dispose()
        {
            lock (_gate) { _disposed = true; _writer.Dispose(); }
        }
    }
}
