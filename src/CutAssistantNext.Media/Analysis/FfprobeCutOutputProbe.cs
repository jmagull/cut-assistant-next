using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CutAssistantNext.Media.Analysis;

internal sealed class FfprobeVideoProbe(string executable) : IVideoProbe
{
    public async Task<VideoInfo> InspectAsync(string path, CancellationToken token)
    {
        using var doc = JsonDocument.Parse(await RunJsonAsync([
            "-show_streams", "-show_format", "-show_entries",
            "stream=codec_type,time_base:format=start_time,duration", path], token));
        var videos = doc.RootElement.GetProperty("streams").EnumerateArray()
            .Where(s => s.GetProperty("codec_type").GetString() == "video").ToArray();
        if (videos.Length != 1) throw new NotSupportedException("Die Übergangsprüfung benötigt genau eine Videospur.");
        var format = doc.RootElement.GetProperty("format");
        var start = decimal.Parse(format.GetProperty("start_time").GetString()!, CultureInfo.InvariantCulture);
        var duration = decimal.Parse(format.GetProperty("duration").GetString()!, CultureInfo.InvariantCulture);
        if (start != 0 || duration <= 0)
            throw new NotSupportedException("Die Übergangsprüfung benötigt eine positive Laufzeit mit Zeitstart null.");
        return new(TimeBase.Parse(videos[0].GetProperty("time_base").GetString()!), start, duration);
    }

    public async IAsyncEnumerable<Packet> ReadPacketsAsync(string path,
        [EnumeratorCancellation] CancellationToken token)
    {
        using var process = Start(["-select_streams", "v:0", "-show_packets", "-show_data_hash", "sha256",
            "-show_entries", "packet=pts,flags,pos,data_hash", "-of", "compact=p=0:nk=0", path]);
        var errors = process.StandardError.ReadToEndAsync(token);
        using var registration = token.Register(() => Kill(process));
        try
        {
            while (await process.StandardOutput.ReadLineAsync(token) is { } line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var values = line.Split('|').Select(x => x.Split('=', 2)).Where(x => x.Length == 2)
                    .ToDictionary(x => x[0], x => x[1], StringComparer.Ordinal);
                if (!values.TryGetValue("pts", out var pts) || !long.TryParse(pts, out var timestamp) ||
                    !values.TryGetValue("data_hash", out var hash) || !hash.StartsWith("SHA256:", StringComparison.Ordinal) ||
                    hash.Length != 71 || !hash.AsSpan(7).ContainsOnlyHex() ||
                    !values.TryGetValue("pos", out var position) || !long.TryParse(position, out var pos) || pos < 0 ||
                    !values.TryGetValue("flags", out var flags))
                    throw new InvalidDataException("Videopaket ohne vollständige PTS-/Hash-/Positionsdaten.");
                yield return new(timestamp, hash, flags, position);
            }
            await process.WaitForExitAsync(token);
            EnsureSuccess(process.ExitCode, await errors);
        }
        finally
        {
            Kill(process);
            await process.WaitForExitAsync(CancellationToken.None);
            try { await errors; } catch (OperationCanceledException) { }
        }
    }

    public async Task<IReadOnlyList<DecodedFrame>> DecodeAsync(string path, decimal start, decimal end, CancellationToken token)
    {
        using var doc = JsonDocument.Parse(await RunJsonAsync(["-select_streams", "v:0", "-read_intervals",
            $"{Formatting.Seconds(Math.Max(0, start))}%{Formatting.Seconds(end)}", "-show_frames", "-show_entries",
            "frame=pts,pkt_pos", path], token));
        return doc.RootElement.GetProperty("frames").EnumerateArray().Select(f =>
            new DecodedFrame(f.GetProperty("pts").GetInt64(), f.GetProperty("pkt_pos").GetString()!)).ToArray();
    }

    private async Task<string> RunJsonAsync(IReadOnlyList<string> arguments, CancellationToken token)
    {
        using var process = Start([.. arguments.Take(arguments.Count - 1), "-of", "json", arguments[^1]]);
        using var registration = token.Register(() => Kill(process));
        var output = process.StandardOutput.ReadToEndAsync(token);
        var error = process.StandardError.ReadToEndAsync(token);
        try
        {
            await process.WaitForExitAsync(token);
            var text = await output;
            EnsureSuccess(process.ExitCode, await error);
            return text;
        }
        finally
        {
            Kill(process);
            await process.WaitForExitAsync(CancellationToken.None);
            try { await Task.WhenAll(output, error); } catch (OperationCanceledException) { }
        }
    }

    private Process Start(IReadOnlyList<string> arguments)
    {
        if (!File.Exists(executable)) throw new FileNotFoundException("ffprobe fehlt.", executable);
        var info = new ProcessStartInfo(executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        info.ArgumentList.Add("-v");
        info.ArgumentList.Add("error");
        foreach (var argument in arguments) info.ArgumentList.Add(argument);
        return Process.Start(info) ?? throw new IOException("ffprobe konnte nicht gestartet werden.");
    }

    private static void EnsureSuccess(int code, string error)
    {
        if (code != 0 || !string.IsNullOrWhiteSpace(error))
            throw new InvalidDataException($"ffprobe-Analyse fehlgeschlagen ({code}): {error.Trim()}");
    }

    private static void Kill(Process process)
    {
        try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
        catch (InvalidOperationException) { }
    }
}

internal static class HexValidation
{
    public static bool ContainsOnlyHex(this ReadOnlySpan<char> value)
    {
        foreach (var c in value) if (!Uri.IsHexDigit(c)) return false;
        return true;
    }
}
