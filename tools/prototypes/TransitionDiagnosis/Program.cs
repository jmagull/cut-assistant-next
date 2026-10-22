using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using CutAssistantNext.Cutlists.Serialization;
using TransitionDiagnosis;

if (args.Length == 1 && args[0] is "--help" or "-h")
{
    Console.WriteLine("TransitionDiagnosis --ffprobe <exe> --source <original> --output <fertige MP4> --cutlist <datei> --report <neue JSON-Datei> [--margin-seconds 15] [--timeout-seconds 600]");
    return 0;
}

try
{
    var options = DiagnosticOptions.Parse(args);
    using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(options.TimeoutSeconds));
    ConsoleCancelEventHandler cancel = (_, e) => { e.Cancel = true; deadline.Cancel(); };
    Console.CancelKeyPress += cancel;
    try
    {
        var token = deadline.Token;
        // Keep inputs open without write/delete sharing throughout all probes and hashing.
        using var sourceLock = File.Open(options.Source, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var outputLock = File.Open(options.Output, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var cutlistLock = File.Open(options.Cutlist, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var text = new StreamReader(cutlistLock, leaveOpen: true);
        var cutlist = CutlistParser.Parse(await text.ReadToEndAsync(token));
        if (cutlist.General.OriginalFileSizeBytes is { } expectedBytes && expectedBytes != sourceLock.Length)
            throw new InvalidDataException("Originalgröße passt nicht zur Cutlist; bitte die Eingaben prüfen.");
        var keeps = cutlist.Cuts.Select(k => new KeepRange((decimal)k.Start.Ticks / TimeSpan.TicksPerSecond,
            (decimal)k.Duration.Ticks / TimeSpan.TicksPerSecond)).ToArray();
        PacketDiagnosis.ValidateKeeps(keeps, options.MarginSeconds);
        var reader = new FfprobeReader(options.Ffprobe);
        Console.Error.WriteLine("Original: Videopakete lesen und hashen …");
        var source = await reader.ReadAsync(options.Source, token);
        Console.Error.WriteLine("Fertige Ausgabe: Videopakete lesen und hashen …");
        var output = await reader.ReadAsync(options.Output, token);
        var diagnosis = new PacketDiagnosis(source, output, keeps, options.MarginSeconds, token);
        foreach (var boundary in diagnosis.Result.Boundaries)
        {
            token.ThrowIfCancellationRequested();
            if (boundary.PacketCandidateSeconds is not { } candidate) continue;
            Console.Error.WriteLine($"Übergang zu Behaltebereich {boundary.NewKeep}: lokales Dekodierfenster …");
            // Include both packet-time sides of a small overlap; larger overlaps remain unresolved.
            var end = Math.Max(candidate, boundary.PreviousMaximumPtsSeconds ?? candidate) + 2;
            var start = Math.Max(0, candidate - 2);
            if (end - start > 20) { boundary.DecodeStatus = "OverlapExceedsLocalWindow"; continue; }
            try
            {
                var frames = await reader.DecodeAsync(options.Output, start, end, token);
                // A demuxer may seek to an earlier keyframe. Retain only the requested time window.
                diagnosis.AddDecodedFrames(boundary, frames.Where(f => output.Metadata.TimeBase.Seconds(f.Pts ?? f.BestEffortTimestamp)
                    is { } time && time >= start && time <= end).ToArray());
            }
            catch (Exception error) when (error is InvalidDataException or JsonException or NotSupportedException)
            {
                boundary.DecodeStatus = "DecodeFailed: " + error.Message;
            }
        }
        Console.Error.WriteLine("Dateiidentitäten sichern …");
        var report = new
        {
            SchemaVersion = 1,
            CreatedUtc = DateTimeOffset.UtcNow,
            Purpose = "Read-only MP4Box transition diagnosis; no certified review positions, content quality or audio verdict.",
            Source = await InputIdentity.ReadAsync(options.Source, token),
            Output = await InputIdentity.ReadAsync(options.Output, token),
            Cutlist = await InputIdentity.ReadAsync(options.Cutlist, token),
            options.MarginSeconds,
            SourceMetadata = source.Metadata,
            OutputMetadata = output.Metadata,
            Keeps = keeps,
            Diagnosis = diagnosis.Result
        };
        await DiagnosticOptions.WriteReportAsync(options.Report, report, token);
        Console.WriteLine($"Diagnose gespeichert: {options.Report}");
        Console.WriteLine($"Paketzuordnung vollständig: {diagnosis.Result.AllPacketsUniquelyMapped}; Reihenfolge passend: {diagnosis.Result.KeepOrderMatchesPlan}");
        return 0;
    }
    finally { Console.CancelKeyPress -= cancel; }
}
catch (OperationCanceledException)
{
    Console.Error.WriteLine("Diagnose abgebrochen oder Zeitlimit erreicht. Keine vollständige Diagnose erstellt.");
    return 2;
}
catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or
    FormatException or InvalidOperationException or NotSupportedException or JsonException or System.ComponentModel.Win32Exception)
{
    Console.Error.WriteLine("Diagnose fehlgeschlagen: " + error.Message);
    return 1;
}

public sealed record InputIdentity(string Path, long Length, DateTime LastWriteUtc, string Sha256)
{
    public static async Task<InputIdentity> ReadAsync(string path, CancellationToken token)
    {
        await using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new(path, stream.Length, File.GetLastWriteTimeUtc(path), Convert.ToHexString(await SHA256.HashDataAsync(stream, token)));
    }
}

public sealed record DiagnosticOptions(string Ffprobe, string Source, string Output, string Cutlist, string Report,
    decimal MarginSeconds, int TimeoutSeconds)
{
    public static DiagnosticOptions Parse(string[] args)
    {
        var allowed = new[] { "--ffprobe", "--source", "--output", "--cutlist", "--report", "--margin-seconds", "--timeout-seconds" };
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 0; i < args.Length; i += 2)
            if (i + 1 >= args.Length || !allowed.Contains(args[i]) || !values.TryAdd(args[i], args[i + 1]))
                throw new ArgumentException("Unbekannte, doppelte oder unvollständige Option. Siehe --help.");
        string PathFor(string name) => Path.GetFullPath(values.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value : throw new ArgumentException("Fehlende Option: " + name));
        var result = new DiagnosticOptions(PathFor("--ffprobe"), PathFor("--source"), PathFor("--output"), PathFor("--cutlist"), PathFor("--report"),
            values.TryGetValue("--margin-seconds", out var margin) ? decimal.Parse(margin, CultureInfo.InvariantCulture) : 15,
            values.TryGetValue("--timeout-seconds", out var timeout) ? int.Parse(timeout, CultureInfo.InvariantCulture) : 600);
        var paths = new[] { result.Ffprobe, result.Source, result.Output, result.Cutlist, result.Report };
        if (paths.Distinct(StringComparer.OrdinalIgnoreCase).Count() != paths.Length)
            throw new ArgumentException("Werkzeug, Original, Ausgabe, Cutlist und Bericht müssen unterschiedliche Pfade haben.");
        foreach (var path in paths[..^1]) if (!File.Exists(path)) throw new FileNotFoundException("Eingabedatei oder Werkzeug fehlt.", path);
        if (File.Exists(result.Report) || Directory.Exists(result.Report)) throw new IOException("Berichtsdatei existiert bereits.");
        if (result.MarginSeconds < 0 || result.MarginSeconds > 60 || result.TimeoutSeconds < 1 || result.TimeoutSeconds > 3600)
            throw new ArgumentException("Spielraum 0–60 Sekunden, Zeitlimit 1–3600 Sekunden erforderlich.");
        return result;
    }

    public static async Task WriteReportAsync(string path, object report, CancellationToken token)
    {
        // Serialize first. Never replace an existing file, even if it appeared during analysis.
        var bytes = JsonSerializer.SerializeToUtf8Bytes(report, new JsonSerializerOptions { WriteIndented = true });
        token.ThrowIfCancellationRequested();
        var pending = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path))!, $".transition-diagnosis-{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(pending, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await stream.WriteAsync(bytes, token);
                await stream.FlushAsync(token);
            }
            token.ThrowIfCancellationRequested();
            File.Move(pending, path, overwrite: false);
        }
        finally { File.Delete(pending); }
    }
}
