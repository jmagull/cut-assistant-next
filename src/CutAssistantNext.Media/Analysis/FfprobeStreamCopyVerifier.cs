using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Analysis;

/// <summary>Verifies selected stream-copy packets including exact source clocks and AAC padding.</summary>
public sealed partial class FfprobeStreamCopyVerifier(string executable)
{
    private sealed record Clock(long N, long D);
    private sealed record StreamData(Clock Clock, string Description, long Start, long Duration);
    internal sealed record CopyPacket(long Pts, long Dts, long Duration, long Size,
        string Hash, string Flags, long Skip, long Discard, string OtherSideData);

    public async Task VerifyAsync(string source, string prepared,
        IReadOnlyList<(int SourceIndex, int PreparedIndex)> streams, CancellationToken token)
    {
        foreach (var pair in streams)
        {
            var a = await InspectAsync(source, pair.SourceIndex, token).ConfigureAwait(false);
            var b = await InspectAsync(prepared, pair.PreparedIndex, token).ConfigureAwait(false);
            if (a.Description != b.Description)
                throw new InvalidDataException("Codec oder Decoderkonfiguration wurden bei der Tonspurauswahl verändert.");
            bool SameClock(long x, long y) =>
                (BigInteger)x * a.Clock.N * b.Clock.D == (BigInteger)y * b.Clock.N * a.Clock.D;
            if (!SameClock(a.Start, b.Start) || !SameClock(a.Duration, b.Duration))
                throw new InvalidDataException(
                    "Wiedergabeanfang oder Spurlaufzeit würden sich beim Kopieren verändern. " +
                    "Die Tonspurkopie wurde verworfen; die bisherige Datei bleibt erhalten.");
            await using var left = ReadAsync(source, pair.SourceIndex, token).GetAsyncEnumerator(token);
            await using var right = ReadAsync(prepared, pair.PreparedIndex, token).GetAsyncEnumerator(token);
            long count = 0;
            while (await left.MoveNextAsync().ConfigureAwait(false))
            {
                if (!await right.MoveNextAsync().ConfigureAwait(false) ||
                    !Same(left.Current, right.Current, a.Clock.N, a.Clock.D, b.Clock.N, b.Clock.D))
                    throw new InvalidDataException($"Paketdaten oder Zeitstempel von Spur {pair.SourceIndex} wurden verändert.");
                count++;
            }
            if (count == 0 || await right.MoveNextAsync().ConfigureAwait(false))
                throw new InvalidDataException("Die Arbeitsdatei enthält eine abweichende Paketanzahl.");
        }
    }

    internal static bool Same(CopyPacket a, CopyPacket b, long an, long ad, long bn, long bd)
    {
        bool Time(long x, long y) => (BigInteger)x * an * bd == (BigInteger)y * bn * ad;
        return Time(a.Pts, b.Pts) && Time(a.Dts, b.Dts) && Time(a.Duration, b.Duration) &&
            a.Size == b.Size && a.Hash == b.Hash && a.Flags == b.Flags &&
            a.Skip == b.Skip && a.Discard == b.Discard && a.OtherSideData == b.OtherSideData;
    }

    private async Task<StreamData> InspectAsync(string path, int index, CancellationToken token)
    {
        var result = await new ToolProbeRunner().RunAsync(executable,
            ["-v", "error", "-select_streams", index.ToString(CultureInfo.InvariantCulture),
                "-show_streams", "-show_data_hash", "sha256", "-of", "json", path], token).ConfigureAwait(false);
        if (result.ExitCode != 0 || !string.IsNullOrWhiteSpace(result.StandardError))
            throw new InvalidDataException("Die Spurkonfiguration konnte nicht fehlerfrei geprüft werden.");
        using var doc = JsonDocument.Parse(result.StandardOutput);
        var streams = doc.RootElement.GetProperty("streams");
        if (streams.GetArrayLength() != 1) throw new InvalidDataException("Ausgewählte Spur fehlt.");
        var s = streams[0];
        var tb = s.GetProperty("time_base").GetString()!.Split('/');
        if (tb.Length != 2 || !long.TryParse(tb[0], out var n) || !long.TryParse(tb[1], out var d) || n <= 0 || d <= 0)
            throw new InvalidDataException("Ungültige Paketzeitbasis.");
        var fields = new[] { "codec_type", "codec_name", "profile", "width", "height",
            "sample_rate", "channels", "channel_layout", "extradata_hash" };
        var description = string.Join("|", fields.Select(k => s.TryGetProperty(k, out var v) ? v.ToString() : ""));
        // AC-3 carries its decoder configuration in the frames, covered by every packet hash.
        if (!s.TryGetProperty("extradata_hash", out _) && s.GetProperty("codec_name").GetString() != "ac3")
            throw new InvalidDataException("Decoderkonfiguration fehlt.");
        if (s.TryGetProperty("tags", out var tags))
            description += "|" + string.Join("|", new[] { "language", "title" }
                .Select(k => tags.TryGetProperty(k, out var v) ? v.ToString() : ""));
        else description += "||";
        if (!s.TryGetProperty("start_pts", out var start) || !start.TryGetInt64(out var first) ||
            !s.TryGetProperty("duration_ts", out var duration) || !duration.TryGetInt64(out var length) || length <= 0)
            throw new InvalidDataException("Der Wiedergabezeitraum der Spur konnte nicht exakt geprüft werden.");
        return new(new(n, d), description, first, length);
    }

    private async IAsyncEnumerable<CopyPacket> ReadAsync(string path, int index,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token)
    {
        var start = new ProcessStartInfo(executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var arg in new[] { "-v", "error", "-select_streams", index.ToString(CultureInfo.InvariantCulture),
            "-show_packets", "-show_data_hash", "sha256", "-show_entries",
            "packet=stream_index,pts,dts,duration,size,flags,data_hash:packet_side_data=side_data_type,skip_samples,discard_padding",
            "-of", "compact=p=0:nk=0", path })
            start.ArgumentList.Add(arg);
        using var process = Process.Start(start) ?? throw new IOException("ffprobe konnte nicht gestartet werden.");
        using var registration = token.Register(() => Kill(process));
        var error = process.StandardError.ReadToEndAsync(token);
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            while (await process.StandardOutput.ReadLineAsync(token).ConfigureAwait(false) is { } line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("stream_index=", StringComparison.Ordinal) && fields.Count > 0)
                {
                    yield return Parse(fields);
                    fields.Clear();
                }
                foreach (var entry in line.Split('|', StringSplitOptions.RemoveEmptyEntries))
                {
                    var value = entry.Split('=', 2);
                    if (value.Length != 2 || !fields.TryAdd(NormalizePacketKey(value[0]), value[1]))
                        throw new InvalidDataException("Mehrdeutige Paketdaten bei der Tonspurauswahl.");
                }
            }
            if (fields.Count > 0) yield return Parse(fields);
            await process.WaitForExitAsync(token).ConfigureAwait(false);
            if (process.ExitCode != 0 || !string.IsNullOrWhiteSpace(await error.ConfigureAwait(false)))
                throw new InvalidDataException("Die Paketprüfung wurde nicht fehlerfrei abgeschlossen.");
        }
        finally
        {
            Kill(process);
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            try { await error.ConfigureAwait(false); } catch (OperationCanceledException) { }
        }
    }

    private static CopyPacket Parse(IReadOnlyDictionary<string, string> fields)
    {
        long Required(string key) => fields.TryGetValue(key, out var text) &&
            long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
                ? n : throw new InvalidDataException("Paketwert fehlt: " + key);
        long Optional(string key) => fields.ContainsKey(key) ? Required(key) : 0;
        if (!fields.TryGetValue("data_hash", out var hash) || !hash.StartsWith("SHA256:", StringComparison.Ordinal) ||
            hash.Length != 71 || !hash.AsSpan(7).ContainsOnlyHex() || !fields.TryGetValue("flags", out var flags))
            throw new InvalidDataException("Paket ohne Hash oder Flags.");
        var duration = Required("duration");
        var size = Required("size");
        var skip = Optional("skip_samples");
        var discard = Optional("discard_padding");
        if (duration < 0 || size <= 0 || skip < 0 || discard < 0) throw new InvalidDataException("Ungültige Paketlänge.");
        var side = fields.GetValueOrDefault("side_data_type", "");
        if (side == "Skip Samples") side = "";
        return new(Required("pts"), Required("dts"), duration, size, hash, flags, skip, discard, side);
    }

    internal static string NormalizePacketKey(string key)
    {
        // New ffprobe compact output qualifies side-data fields, e.g.
        // side_datum/skip_samples:skip_samples. Older output uses skip_samples directly.
        var separator = key.IndexOf(':');
        return key.StartsWith("side_datum/", StringComparison.Ordinal) && separator > 0
            ? key[(separator + 1)..] : key;
    }

    private static void Kill(Process process)
    {
        try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
        catch (InvalidOperationException) { }
    }
}
