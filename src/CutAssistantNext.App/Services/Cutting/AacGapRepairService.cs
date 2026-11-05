using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;
namespace CutAssistantNext.App.Services.Cutting;

/// <summary>Versioned explicit Rust protocol. No C# reconstruction or runtime lab dependency.</summary>
internal sealed class AacGapRepairService : IAacGapRepair
{
    internal const string Notice = "Experimentelle AAC-Zeitlückenreparatur: Nachgewiesene Lücken wurden positionsgenau mit Stille belegt. Fehlende ursprüngliche Audiodaten können nicht wiederhergestellt werden. Bitte Bild, Ton und alle Schnittübergänge persönlich kontrollieren.";
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
    private readonly OtrCanToolPaths tools;
    private readonly ICutToolRunner runner;
    private readonly IToolProbeRunner probe;
    private readonly Func<string, CancellationToken, Task<MediaAnalysisResult>> analyze;
    internal AacGapRepairService(OtrCanToolPaths tools, ICutToolRunner? runner = null, IToolProbeRunner? probe = null,
     Func<string, CancellationToken, Task<MediaAnalysisResult>>? analyze = null)
    { this.tools = tools; this.runner = runner ?? new CutToolRunner(); this.probe = probe ?? new ToolProbeRunner(); this.analyze = analyze ?? new FfprobeRunner(tools.Ffprobe).RunAsync; }
    internal sealed record Identity(string Path, long Bytes, string Sha256);
    internal static long SampleTick(TimeSpan time)
    {
        var value = (decimal)time.Ticks * 48000 / TimeSpan.TicksPerSecond;
        if (value != decimal.Truncate(value) || value < 0 || value > long.MaxValue) throw new InvalidDataException("Schnittgrenze liegt nicht exakt auf einer Sampleposition.");
        return (long)value;
    }
    internal static async Task<Identity> IdentifyAsync(string path, CancellationToken token)
    {
        await using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan);
        return new(Path.GetFullPath(path), file.Length, Convert.ToHexStringLower(await SHA256.HashDataAsync(file, token).ConfigureAwait(false)));
    }
    private static async Task<JsonDocument> ReadAsync(string path, long limit, CancellationToken token)
    {
        var info = new FileInfo(path);
        if (!info.Exists || info.Length is <= 0 || info.Length > limit || info.LinkTarget is not null) throw new InvalidDataException("Der Motor hat keinen vollständigen, begrenzten Prüfnachweis erstellt.");
        await using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, FileOptions.Asynchronous);
        return await JsonDocument.ParseAsync(file, cancellationToken: token).ConfigureAwait(false);
    }
    public async Task<AacGapRepairOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
     IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        string? root = null; var ownsRoot = false; FileStream? sourceLock = null; FileStream? engineLock = null;
        void Cleanup()
        {
            sourceLock?.Dispose(); sourceLock = null; engineLock?.Dispose(); engineLock = null;
            if (!ownsRoot || root is null || !Directory.Exists(root)) return;
            var resolved = Path.GetFullPath(root); var parent = Path.GetDirectoryName(request.OutputFilePath)!;
            if (!string.Equals(Path.GetDirectoryName(resolved), parent, StringComparison.OrdinalIgnoreCase) ||
                !Path.GetFileName(resolved).StartsWith(".can-aac-", StringComparison.Ordinal) || new DirectoryInfo(resolved).LinkTarget is not null)
                throw new IOException("Der Reparaturarbeitsbereich hat sich verändert und wird nicht automatisch gelöscht.");
            Directory.Delete(resolved, recursive: true);
        }
        void CleanSafely() { try { Cleanup(); } catch (Exception e) when (e is IOException or UnauthorizedAccessException) { progress?.Report(new(CutProgressKind.Output, "Reparaturarbeitsdateien konnten nicht vollständig entfernt werden: " + e.Message)); } }
        try
        {
            token.ThrowIfCancellationRequested();
            if (!string.Equals(request.OriginalFilePath, request.SourceFilePath, StringComparison.OrdinalIgnoreCase)) return new("Experimentelle Reparatur ist für vorbereitete Arbeitsquellen derzeit nicht freigegeben.");
            if (failure.Diagnostic?.Issues.Any(issue => issue.Kind == "audio_end") == true)
                return new("Fehlender Ton am Dateiende liegt außerhalb des geprüften Verfahrens und wird nicht aufgefüllt.");
            var capability = await probe.RunAsync(tools.Engine, ["capabilities"], token).ConfigureAwait(false);
            if (capability.ExitCode != 0 || !capability.StandardOutput.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Contains("aac-gap-repair-v1"))
                return new("Der gewählte otr-can-Motor unterstützt den experimentellen Reparaturvertrag nicht.");
            sourceLock = new FileStream(request.SourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            engineLock = new FileStream(tools.Engine, FileMode.Open, FileAccess.Read, FileShare.Read);
            var source = await analyze(request.SourceFilePath, token).ConfigureAwait(false);
            if (source.AudioStreams.Count != 1 || source.AudioStreams[0].CodecName != "aac" || source.AudioStreams[0].SampleRate != 48000 || source.AudioStreams[0].Channels != 2)
            { CleanSafely(); return new("Unterstützt wird derzeit genau eine AAC-LC-Stereospur mit 48 kHz. Mehrspurdateien, andere Formate und fehlender Endton sind nicht freigegeben."); }
            var keeps = request.KeepSegments.Select(k => new { Start = SampleTick(k.Start), End = SampleTick(k.End) }).ToArray();
            if (keeps.Any(k => k.Start % 1920 != 0 || k.End % 1920 != 0)) { CleanSafely(); return new("Die Behaltebereiche liegen nicht auf dem unterstützten 25-fps-Bildraster."); }
            var parent = Path.GetDirectoryName(request.OutputFilePath)!;
            var id = Guid.NewGuid().ToString("N"); root = Path.Combine(parent, ".can-aac-" + id);
            if (Directory.Exists(root) || File.Exists(root)) throw new IOException("Der neue Reparaturarbeitsbereich existiert bereits.");
            Directory.CreateDirectory(root);
            using (new FileStream(Path.Combine(root, ".can-aac-owner"), FileMode.CreateNew, FileAccess.Write, FileShare.None)) ownsRoot = true;
            var index = Path.Combine(root, "source.ffindex");
            progress?.Report(new(CutProgressKind.Status, "Experimentelle AAC-Reparatur: Quelle und Behaltebereiche werden vollständig geprüft …"));
            await runner.RunAsync(tools.Indexer, ["-c", "-k", request.SourceFilePath, index], progress, token).ConfigureAwait(false);
            var cutlist = Path.Combine(root, "keep-snapshot.cutlist");
            var text = new StringBuilder("[General]\nNoOfCuts=" + keeps.Length + "\n");
            for (var i = 0; i < keeps.Length; i++) text.Append(CultureInfo.InvariantCulture, $"[Cut{i}]\nStart={(decimal)keeps[i].Start / 48000:0.000000000}\nDuration={(decimal)(keeps[i].End - keeps[i].Start) / 48000:0.000000000}\n");
            await File.WriteAllTextAsync(cutlist, text.ToString(), new UTF8Encoding(false), token).ConfigureAwait(false);
            var sourceIdentity = await IdentifyAsync(request.SourceFilePath, token).ConfigureAwait(false);
            var engineIdentity = await IdentifyAsync(tools.Engine, token).ConfigureAwait(false);
            var requestFile = Path.Combine(root, "request.json");
            var document = new
            {
                Schema = 1,
                Source = sourceIdentity,
                Cutlist = await IdentifyAsync(cutlist, token).ConfigureAwait(false),
                Index = await IdentifyAsync(index, token).ConfigureAwait(false),
                Timecodes = await IdentifyAsync(index + "_track00.tc.txt", token).ConfigureAwait(false),
                Keyframes = await IdentifyAsync(index + "_track00.kf.txt", token).ConfigureAwait(false),
                Stream = source.AudioStreams[0].Index,
                Keeps = keeps
            };
            await File.WriteAllTextAsync(requestFile, JsonSerializer.Serialize(document, Json), token).ConfigureAwait(false);
            var analysis = Path.Combine(root, "analysis");
            await runner.RunAsync(tools.Engine, ["aac-gap-dev", "analyze", "--request", requestFile, "--ffmpeg", tools.Ffmpeg, "--ffprobe", tools.Ffprobe, "--work", analysis], progress, token).ConfigureAwait(false);
            using var offerDoc = await ReadAsync(Path.Combine(analysis, "repair-offer.json"), 4 * 1024 * 1024, token).ConfigureAwait(false);
            using var planDoc = await ReadAsync(Path.Combine(analysis, "plan.json"), 4 * 1024 * 1024, token).ConfigureAwait(false);
            var o = offerDoc.RootElement; var p = planDoc.RootElement;
            var hash = p.GetProperty("plan_sha256").GetString();
            if (o.GetProperty("schema").GetInt32() != 1 || !o.GetProperty("available").GetBoolean() ||
                !o.GetProperty("explicit_consent_required").GetBoolean() || hash is null || hash.Length != 64 || !hash.All(Uri.IsHexDigit) ||
                o.GetProperty("plan_sha256").GetString() != hash) throw new InvalidDataException("Keine vollständig nachgewiesene, ausgewählte AAC-Zeitlücke für diesen Schnittplan.");
            var missing = o.GetProperty("missing_vectors").GetInt64(); var gaps = o.GetProperty("selected_source_gaps").GetInt32();
            var expected = keeps.Sum(k => k.End - k.Start);
            if (missing is <= 0 or > 4096 || gaps is <= 0 or > 8 || o.GetProperty("output_vectors").GetInt64() != expected) throw new InvalidDataException("Der Nachweis überschreitet die freigegebenen Reparaturgrenzen.");
            var body = p.GetProperty("plan");
            if (body.GetProperty("schema").GetInt32() != 1 || body.GetProperty("status").GetString() != "experimental-rust-aac-gap-plan-v1" ||
                !JsonElement.DeepEquals(body.GetProperty("request"), JsonSerializer.SerializeToElement(document, Json)) ||
                body.GetProperty("inserted_vectors").GetInt64() != missing || body.GetProperty("output_vectors").GetInt64() != expected)
                throw new InvalidDataException("Der geprüfte Reparaturplan gehört nicht zu diesem Auftrag.");
            var planPath = Path.Combine(analysis, "plan.json"); var ownedRoot = root;
            var planIdentity = await IdentifyAsync(planPath, token).ConfigureAwait(false);
            progress?.Report(new(CutProgressKind.Output, $"Prüfbarer Reparaturplan: {planPath}; SHA-256 {hash}; {gaps} Quelllücken, {missing} Samplevektoren. Noch keine Reparatur gestartet."));
            var description = $"Tonspur: AAC-LC, Stereo, 48 kHz\nBetroffene Stellen: {gaps}\nFehlende Samples: {missing}\nFehlende Audiodauer: {(decimal)missing / 48:0.00} ms\nReparatur: Für diese Fehlerklasse experimentell verfügbar.";
            return new(description, async (approvedRequest, updates, cancel) =>
            {
                if (!ReferenceEquals(request, approvedRequest)) throw new InvalidOperationException("Der Schnittauftrag hat sich seit der Bestätigung verändert.");
                if (await IdentifyAsync(request.SourceFilePath, cancel).ConfigureAwait(false) != sourceIdentity || await IdentifyAsync(tools.Engine, cancel).ConfigureAwait(false) != engineIdentity || await IdentifyAsync(planPath, cancel).ConfigureAwait(false) != planIdentity)
                    throw new InvalidDataException("Quelle, Reparaturplan oder Reparaturmotor haben sich seit der Bestätigung verändert.");
                var existed = File.Exists(request.OutputFilePath);
                if (existed && !request.OverwriteExistingOutput) throw new IOException("Die Ausgabedatei existiert bereits und wird geschützt.");
                var oldTarget = existed ? await IdentifyAsync(request.OutputFilePath, cancel).ConfigureAwait(false) : null;
                var pending = Path.Combine(parent, ".can-aac-" + id + "-output.mp4");
                if (File.Exists(pending)) throw new IOException("Die neue temporäre Ausgabe existiert bereits.");
                try
                {
                    var cut = Path.Combine(ownedRoot, "repair");
                    await runner.RunAsync(tools.Engine, ["aac-gap-dev", "cut", "--plan", planPath, "--confirm-plan-sha256", hash, "--work", cut, "--output", pending], updates, cancel).ConfigureAwait(false);
                    using var complete = await ReadAsync(Path.Combine(cut, "completed.json"), 65536, cancel).ConfigureAwait(false);
                    using var proof = await ReadAsync(Path.Combine(cut, "independent-final-check", "mp4-verification.json"), 64 * 1024 * 1024, cancel).ConfigureAwait(false);
                    var c = complete.RootElement; var v = proof.RootElement;
                    if (new FileInfo(pending).LinkTarget is not null) throw new InvalidDataException("Unerwartete verknüpfte Ausgabe.");
                    var outputIdentity = await IdentifyAsync(pending, cancel).ConfigureAwait(false);
                    if (c.GetProperty("schema").GetInt32() != 1 || c.GetProperty("status").GetString() != "experimental-repair-technically-verified-personal-review-pending" ||
               c.GetProperty("output").GetString() != pending || c.GetProperty("sha256").GetString() != outputIdentity.Sha256 ||
               c.GetProperty("plan_sha256").GetString() != hash || c.GetProperty("output_vectors").GetInt64() != expected || c.GetProperty("inserted_vectors").GetInt64() != missing ||
               v.GetProperty("status").GetString() != "independently-verified-experimental-mp4-personal-review-pending" || v.GetProperty("candidate_sha256").GetString() != outputIdentity.Sha256 ||
               v.GetProperty("plan_sha256").GetString() != hash || !v.GetProperty("aac_packets_unchanged").GetBoolean() || !v.GetProperty("no_unexplained_sample_deviation").GetBoolean() ||
               !v.GetProperty("no_tail_repair").GetBoolean() || !v.GetProperty("h264_parameters").GetProperty("all_profiles_compatible").GetBoolean() ||
               v.GetProperty("h264_parameters").GetProperty("lossless_transform_bypass").GetBoolean() ||
               !v.GetProperty("pcm").GetProperty("both_channels_checked").GetBoolean() ||
               v.GetProperty("pcm").GetProperty("reconstructed_zero_vectors").GetInt64() != missing ||
               v.GetProperty("audio").GetProperty("decoded_vectors").GetInt64() != expected ||
               v.GetProperty("video").GetProperty("frames").GetInt64() != expected / 1920 || outputIdentity.Bytes <= 0) throw new InvalidDataException("Der vollständige unabhängige Reparatur-/MP4-Nachweis fehlt oder passt nicht zur Datei.");
                    OtrCanCutService.ValidateOutput(source, await analyze(pending, cancel).ConfigureAwait(false));
                    if (await IdentifyAsync(request.SourceFilePath, cancel).ConfigureAwait(false) != sourceIdentity) throw new InvalidDataException("Quelle hat sich während der Reparatur verändert.");
                    if (File.Exists(request.OutputFilePath) != existed || existed && await IdentifyAsync(request.OutputFilePath, cancel).ConfigureAwait(false) != oldTarget)
                        throw new IOException("Die Zieldatei hat sich während des Auftrags verändert und wird nicht ersetzt.");
                    cancel.ThrowIfCancellationRequested();
                    File.Move(pending, request.OutputFilePath, overwrite: existed && request.OverwriteExistingOutput);
                    updates?.Report(new(CutProgressKind.Output, $"Experimentelle Reparatur unabhängig geprüft: {expected} Nutzsamples, {missing} bestätigte Stillevektoren; Ausgabe-SHA-256 {outputIdentity.Sha256}."));
                    updates?.Report(new(CutProgressKind.Output, Notice));
                    return CutResult.Planned(request.OutputFilePath, request.KeepSegments, Notice);
                }
                catch (Exception error) when (error is not OperationCanceledException)
                {
                    throw new InvalidOperationException(Explain(error), error);
                }
                finally
                {
                    try { if (File.Exists(pending)) File.Delete(pending); }
                    catch (Exception e) when (e is IOException or UnauthorizedAccessException) { updates?.Report(new(CutProgressKind.Output, "Temporäre Reparaturausgabe konnte nicht entfernt werden: " + e.Message)); }
                }
            }, () => { CleanSafely(); return ValueTask.CompletedTask; });
        }
        catch (OperationCanceledException) { CleanSafely(); throw; }
        catch (Exception error) when (error is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or KeyNotFoundException or FormatException or JsonException or OverflowException or System.ComponentModel.Win32Exception or TimeoutException)
        {
            progress?.Report(new(CutProgressKind.Output, "Experimentelle AAC-Reparatur nicht verfügbar: " + error));
            CleanSafely();
            return new(Explain(error));
        }
    }
    private static string Explain(Exception error)
    {
        if (error is InvalidDataException or IOException && !error.Message.Contains("[aac-gap", StringComparison.Ordinal)) return error.Message;
        var message = error.Message;
        if (message.Contains("plan SHA-256", StringComparison.Ordinal) || message.Contains("identity differs", StringComparison.Ordinal)) return "Quelle oder bestätigter Reparaturplan passen nicht mehr zum Auftrag.";
        if (message.Contains("tail", StringComparison.OrdinalIgnoreCase)) return "Fehlender Ton am Dateiende wird nicht aufgefüllt.";
        if (message.Contains("eight original gaps", StringComparison.Ordinal)) return "Die Auswahl überschreitet acht Lücken oder 4.096 zu ergänzende Samplevektoren.";
        if (message.Contains("packet clock", StringComparison.Ordinal) || message.Contains("gap size", StringComparison.Ordinal) || message.Contains("contradictory timestamps", StringComparison.Ordinal)) return "Die Tonzeiten lassen sich nicht vollständig durch die unterstützten Lücken von je 512 Samples erklären.";
        if (message.Contains("narrow AAC", StringComparison.Ordinal) || message.Contains("selected AAC stream", StringComparison.Ordinal)) return "Die Aufnahme entspricht nicht der geprüften AAC-LC-Stereoklasse mit 48 kHz.";
        if (message.Contains("H.264", StringComparison.Ordinal) || message.Contains("picture", StringComparison.Ordinal)) return "Die strenge Prüfung der Bildfolge oder des H.264-Profils ist fehlgeschlagen.";
        return "Die strenge Prüfung konnte keine sichere experimentelle Reparatur bestätigen. Einzelheiten stehen im Schnittprotokoll.";
    }
}
