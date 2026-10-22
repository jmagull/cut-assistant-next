using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace TransitionDiagnosis;

public sealed class FfprobeReader(string executable, int maxPackets = 2_000_000)
{
    public async Task<VideoSnapshot> ReadAsync(string path, CancellationToken token)
    {
        using var metadata = JsonDocument.Parse(await ReadJsonAsync([
            "-show_streams", "-show_format", "-show_entries",
            "stream=codec_type,codec_name,time_base,start_time,duration,avg_frame_rate:format=duration", path], token));
        var streams = metadata.RootElement.GetProperty("streams").EnumerateArray()
            .Where(s => s.GetProperty("codec_type").GetString() == "video").ToArray();
        if (streams.Length != 1) throw new NotSupportedException("Die Diagnose benötigt genau eine Videospur.");
        var stream = streams[0];
        var info = new VideoMetadata(VideoTimeBase.Parse(stream.GetProperty("time_base").GetString()!),
            Decimal(stream, "start_time"), Decimal(stream, "duration"), Decimal(metadata.RootElement.GetProperty("format"), "duration"),
            stream.GetProperty("codec_name").GetString()!, stream.GetProperty("avg_frame_rate").GetString()!);
        var packets = new List<VideoPacket>();
        await foreach (var line in LinesAsync(["-select_streams", "v:0", "-show_packets", "-show_data_hash", "sha256",
            "-show_entries", "packet=pts,dts,duration,pos,flags,data_hash", "-of", "compact=p=0:nk=0", path], token))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (packets.Count >= maxPackets) throw new NotSupportedException($"Paketgrenze {maxPackets} erreicht; Diagnose beendet.");
            packets.Add(ParsePacket(line));
        }
        if (packets.Count == 0) throw new InvalidDataException("Keine Videopakete gefunden.");
        return new(info, packets);
    }

    public async Task<IReadOnlyList<VideoFrame>> DecodeAsync(string path, decimal start, decimal end, CancellationToken token)
    {
        if (start < 0 || end <= start || end - start > 20) throw new ArgumentOutOfRangeException(nameof(end));
        using var doc = JsonDocument.Parse(await ReadJsonAsync(["-select_streams", "v:0", "-read_intervals",
            string.Create(CultureInfo.InvariantCulture, $"{start}%{end}"), "-show_frames", "-show_entries",
            "frame=pts,pkt_dts,best_effort_timestamp,duration,pkt_pos,pict_type", path], token));
        return doc.RootElement.GetProperty("frames").EnumerateArray().Select(f => new VideoFrame(
            Integer(f, "pts"), Integer(f, "pkt_dts"), Integer(f, "best_effort_timestamp"), Integer(f, "duration"),
            Integer(f, "pkt_pos"), f.TryGetProperty("pict_type", out var type) ? type.GetString()! : "?")).ToArray();
    }

    public static VideoPacket ParsePacket(string line)
    {
        var fields = line.Split('|').Select(p => p.Split('=', 2)).Where(p => p.Length == 2)
            .ToDictionary(p => p[0], p => p[1], StringComparer.Ordinal);
        if (!fields.TryGetValue("data_hash", out var hash) || !hash.StartsWith("SHA256:", StringComparison.Ordinal) ||
            hash.Length != 71 || !hash.AsSpan(7).ToArray().All(Uri.IsHexDigit) ||
            !fields.TryGetValue("pos", out var position) || !long.TryParse(position, CultureInfo.InvariantCulture, out var pos) || pos < 0 ||
            !fields.TryGetValue("flags", out var flags))
            throw new InvalidDataException("Videopaket ohne gültigen SHA-256, Position oder Flags.");
        return new(Read("pts"), Read("dts"), Read("duration"), pos, hash, flags);
        long? Read(string key) => fields.TryGetValue(key, out var text) && long.TryParse(text, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    private static decimal? Decimal(JsonElement element, string name) => element.TryGetProperty(name, out var value) &&
        decimal.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;
    private static long? Integer(JsonElement element, string name) => element.TryGetProperty(name, out var value) &&
        long.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var result) ? result : null;

    private async Task<string> ReadJsonAsync(IReadOnlyList<string> args, CancellationToken token)
    {
        var text = new System.Text.StringBuilder();
        await foreach (var line in LinesAsync([.. args.Take(args.Count - 1), "-of", "json", args[^1]], token))
        {
            if (text.Length + line.Length > 16 * 1024 * 1024) throw new InvalidDataException("Diagnoseantwort überschreitet 16 MiB.");
            text.AppendLine(line);
        }
        return text.ToString();
    }

    private async IAsyncEnumerable<string> LinesAsync(IReadOnlyList<string> args,
        [EnumeratorCancellation] CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!File.Exists(executable)) throw new FileNotFoundException("ffprobe fehlt.", executable);
        var info = new ProcessStartInfo(executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var item in new[] { "-v", "error", "-threads", "1" }.Concat(args)) info.ArgumentList.Add(item);
        using var process = Process.Start(info) ?? throw new IOException("ffprobe konnte nicht gestartet werden.");
        try { process.PriorityClass = ProcessPriorityClass.BelowNormal; }
        catch (Exception error) when (error is InvalidOperationException or System.ComponentModel.Win32Exception) { }
        using var registration = token.Register(() => Kill(process));
        // Drain stderr even when the reader rejects a packet or the operation is canceled.
        var errors = DrainErrorsAsync(process.StandardError);
        try
        {
            while (await process.StandardOutput.ReadLineAsync(token) is { } line) yield return line;
            await process.WaitForExitAsync(token);
            var error = await errors;
            if (process.ExitCode != 0 || !string.IsNullOrWhiteSpace(error))
                throw new InvalidDataException($"ffprobe fehlgeschlagen ({process.ExitCode}): {error}");
        }
        finally
        {
            Kill(process);
            await process.WaitForExitAsync(CancellationToken.None);
            await errors;
        }
    }

    private static async Task<string> DrainErrorsAsync(StreamReader reader)
    {
        var buffer = new char[4096];
        var text = new System.Text.StringBuilder();
        int read;
        while ((read = await reader.ReadAsync(buffer)) > 0)
            if (text.Length < 8192) text.Append(buffer, 0, Math.Min(read, 8192 - text.Length));
        return text.ToString();
    }

    private static void Kill(Process process)
    {
        try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
        catch (InvalidOperationException) { }
    }
}
