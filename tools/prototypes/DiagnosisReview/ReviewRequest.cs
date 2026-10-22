using System.Security.Cryptography;
using System.Text.Json;
using CutAssistantNext.Core.Cutting;

namespace DiagnosisReview;

public sealed record ReviewFile(string Path, long Length, DateTime LastWriteUtc, string Sha256)
{
    public async Task ValidateAsync(CancellationToken token)
    {
        await using var file = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (file.Length != Length || File.GetLastWriteTimeUtc(Path).Ticks != LastWriteUtc.ToUniversalTime().Ticks ||
            !Convert.ToHexString(await SHA256.HashDataAsync(file, token)).Equals(Sha256, StringComparison.OrdinalIgnoreCase))
            throw new IOException("Die Mediendatei stimmt nicht mehr mit dem Diagnosebericht überein: " + Path);
    }
}

public sealed record ReferenceFrame(decimal OutputSeconds, decimal SourceSeconds, bool IsFirstNewFrame);
public sealed record ReviewCandidate(int Index, decimal Seconds, IReadOnlyList<ReferenceFrame> References);
public sealed record ReviewRequest(ReviewFile Source, ReviewFile Output, decimal Duration, IReadOnlyList<ReviewCandidate> Candidates)
{
    public const string Notice = "DIAGNOSE · Kandidaten aus der Paketzuordnung, noch keine bestätigten CAN-Prüfstellen. Bitte den Drei-Sekunden-Vorlauf prüfen.";
    public static TimeSpan Time(decimal seconds) => TimeSpan.FromTicks(checked((long)decimal.Round(seconds * TimeSpan.TicksPerSecond)));

    public CutResult CreateCutResult() => new(Output.Path,
        new CutOutputTimeline(Time(Duration), Candidates.Select(c => Time(c.Seconds)),
            new(Output.Length, Output.LastWriteUtc.ToUniversalTime().Ticks, Output.Sha256)),
        CutReviewTimingKind.Estimated, Notice);

    public static ReviewRequest Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.GetProperty("SchemaVersion").GetInt32() != 1) throw new InvalidDataException("Unbekannte Diagnoseversion.");
        ReviewFile FileFor(string key)
        {
            var file = root.GetProperty(key).Deserialize<ReviewFile>() ?? throw new InvalidDataException("Dateiidentität fehlt.");
            if (!System.IO.Path.IsPathFullyQualified(file.Path) || file.Length <= 0 || file.LastWriteUtc.Ticks <= 0 ||
                file.Sha256 is null || file.Sha256.Length != 64 || !file.Sha256.All(Uri.IsHexDigit))
                throw new InvalidDataException("Ungültige Dateiidentität.");
            return file;
        }
        var diagnosis = root.GetProperty("Diagnosis");
        if (!diagnosis.GetProperty("AllPacketsUniquelyMapped").GetBoolean() || !diagnosis.GetProperty("KeepOrderMatchesPlan").GetBoolean() ||
            diagnosis.GetProperty("DiscardOrCorruptPackets").GetInt32() != 0)
            throw new InvalidDataException("Die Diagnose enthält keine vollständige, geordnete Paketzuordnung ohne verworfene/beschädigte Pakete.");
        var metadata = root.GetProperty("OutputMetadata");
        var duration = metadata.GetProperty("VideoDuration").GetDecimal();
        if (duration <= 0) throw new InvalidDataException("Positive Videodauer erforderlich.");
        var candidates = new List<ReviewCandidate>();
        foreach (var boundary in diagnosis.GetProperty("Boundaries").EnumerateArray())
        {
            var keep = boundary.GetProperty("NewKeep").GetInt32();
            var seconds = boundary.GetProperty("PacketCandidateSeconds").GetDecimal();
            if (keep != candidates.Count + 2 || seconds <= 0 || seconds >= duration ||
                candidates.Count > 0 && seconds <= candidates[^1].Seconds)
                throw new InvalidDataException("Diagnosekandidaten müssen vollständig und aufsteigend im Film liegen.");
            var frames = boundary.GetProperty("Frames").EnumerateArray().ToArray();
            var first = Array.FindIndex(frames, f => f.GetProperty("Keep").ValueKind == JsonValueKind.Number &&
                f.GetProperty("Keep").GetInt32() == keep);
            if (first <= 0 || frames[first].GetProperty("PtsSeconds").GetDecimal() != seconds)
                throw new InvalidDataException("Erster neuer Decoderanker fehlt oder weicht vom Paketkandidaten ab.");
            if (!frames.Take(first).Any(f => IsKeep(f, keep - 1)))
                throw new InvalidDataException("Decoderreferenz der vorherigen Seite fehlt.");
            var references = frames.Take(first).Where(f => IsKeep(f, keep - 1)).TakeLast(1)
                .Concat(frames.Skip(first).Where(f => IsKeep(f, keep)).Take(3))
                .Select(f => new ReferenceFrame(f.GetProperty("PtsSeconds").GetDecimal(),
                    f.GetProperty("SourcePtsSeconds").GetDecimal(), f.Equals(frames[first]))).ToArray();
            if (references.Length < 2 || !references.Any(f => f.IsFirstNewFrame) || references.Any(f => f.SourceSeconds < 0))
                throw new InvalidDataException("Originalbildreferenzen für beide Seiten erforderlich.");
            candidates.Add(new(keep - 1, seconds, references));
        }
        if (candidates.Count == 0 || diagnosis.GetProperty("Segments").GetArrayLength() != candidates.Count + 1)
            throw new InvalidDataException("Anzahl der Abschnitte und Übergänge passt nicht.");
        return new(FileFor("Source"), FileFor("Output"), duration, candidates);
    }

    private static bool IsKeep(JsonElement frame, int keep) => frame.GetProperty("Keep").ValueKind == JsonValueKind.Number &&
        frame.GetProperty("Keep").GetInt32() == keep;
}
