using System.Globalization;
using System.Numerics;
using System.Text.Json;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Analysis;

public sealed record JoinPart(string Path, int AudioIndex, int VideoIndex, long VideoTimeBase,
    long VideoDuration, int FrameRate, int SampleRate, int Channels, string Layout,
    string Language, string VideoConfiguration, long AudioSamples)
{
    public TimeSpan Duration => TimeSpan.FromTicks(checked(VideoDuration * TimeSpan.TicksPerSecond / VideoTimeBase));
    public string AudioCodec { get; init; } = "";
    public string AudioProfile { get; init; } = "";
}

public sealed partial class FfprobeStreamCopyVerifier
{
    /// <summary>Conservative V1: zero-based CFR H.264 MP4, full-length AAC/AC-3, no resampling.</summary>
    public async Task<JoinPart> InspectJoinPartAsync(string path, int audioIndex, CancellationToken token)
    {
        var probe = await new ToolProbeRunner().RunAsync(executable,
            ["-v", "error", "-show_streams", "-show_format", "-show_data_hash", "sha256", "-of", "json", path], token).ConfigureAwait(false);
        if (probe.ExitCode != 0 || !string.IsNullOrWhiteSpace(probe.StandardError))
            throw new InvalidDataException("Die Filmdatei konnte nicht fehlerfrei analysiert werden.");
        return ParseJoinPart(path, audioIndex, probe.StandardOutput);
    }

    internal static JoinPart ParseJoinPart(string path, int audioIndex, string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var format = root.GetProperty("format").GetProperty("format_name").GetString() ?? "";
        var streams = root.GetProperty("streams").EnumerateArray().ToArray();
        var videos = streams.Where(s => Text(s, "codec_type") == "video").ToArray();
        var audios = streams.Where(s => Text(s, "codec_type") == "audio" && Number(s, "index") == audioIndex).ToArray();
        if (!format.Split(',').Contains("mp4") || videos.Length != 1 || audios.Length != 1)
            throw new InvalidDataException("Bitte eine MP4-Datei mit genau einer Videospur und einer vorhandenen Tonspur wählen.");
        var v = videos[0]; var a = audios[0];
        var rate = Text(v, "r_frame_rate");
        var fps = rate switch { "25/1" => 25, "50/1" => 50, _ => 0 };
        var clock = Text(v, "time_base").Split('/');
        if (Text(v, "codec_name") != "h264" || fps == 0 || Text(v, "avg_frame_rate") != rate ||
            Text(v, "field_order") != "progressive" || Text(v, "extradata_hash").Length == 0 ||
            v.TryGetProperty("side_data_list", out var videoSideData) && videoSideData.GetArrayLength() != 0 ||
            clock.Length != 2 || clock[0] != "1" || !long.TryParse(clock[1], out var denominator) ||
            denominator <= 0 || denominator % fps != 0 || Number(v, "start_pts") != 0)
            throw new InvalidDataException("Diese erste Fassung benötigt progressive H.264-MP4-Dateien mit durchgehenden 25 oder 50 Bildern/s ab Zeit null.");
        var duration = Number(v, "duration_ts");
        var sampleRate = checked((int)Number(a, "sample_rate"));
        var channels = checked((int)Number(a, "channels"));
        var layout = Text(a, "channel_layout");
        if (duration <= 0 || duration % (denominator / fps) != 0 ||
            Text(a, "codec_name") is not ("aac" or "ac3") || sampleRate is not (44100 or 48000) ||
            !((channels == 1 && layout == "mono") || (channels == 2 && layout == "stereo") ||
              (channels == 6 && layout is "5.1" or "5.1(side)")) || Number(a, "start_pts") != 0 ||
            Text(a, "time_base") != $"1/{sampleRate}" ||
            (BigInteger)duration * sampleRate != (BigInteger)Number(a, "duration_ts") * denominator ||
            (BigInteger)duration * TimeSpan.TicksPerSecond % denominator != 0)
            throw new InvalidDataException("Bild und gewählte Tonspur müssen denselben vollständigen Zeitraum ab null haben; eine automatische Zeitkorrektur erfolgt nicht.");
        var config = string.Join('|', new[] { "codec_name", "profile", "level", "width", "height", "pix_fmt",
            "sample_aspect_ratio", "field_order", "color_range", "color_space", "color_transfer", "color_primaries",
            "chroma_location", "extradata_hash" }.Select(k => Text(v, k)));
        var language = a.TryGetProperty("tags", out var tags) ? Text(tags, "language") : "";
        return new(path, audioIndex, checked((int)Number(v, "index")), denominator, duration, fps,
            sampleRate, channels, layout, language, config, Number(a, "duration_ts"))
        { AudioCodec = Text(a, "codec_name"), AudioProfile = Text(a, "profile") };
    }

    public static void ValidateJoinCompatibility(JoinPart first, JoinPart second)
    {
        if (first.VideoConfiguration != second.VideoConfiguration || first.FrameRate != second.FrameRate)
            throw new InvalidDataException("Die Bildformate oder Decoderkonfigurationen passen nicht zusammen. Das Bild wird nicht neu kodiert.");
        if (first.SampleRate != second.SampleRate || first.Channels != second.Channels || first.Layout != second.Layout)
            throw new InvalidDataException("Bitte Tonspuren mit gleicher Kanalbelegung und Abtastrate wählen.");
    }

    public async Task ValidateJoinVideoAsync(JoinPart part, CancellationToken token)
    {
        var step = part.VideoTimeBase / part.FrameRate;
        var seen = new HashSet<long>();
        long? previous = null;
        await foreach (var p in ReadAsync(part.Path, part.VideoIndex, token).ConfigureAwait(false))
        {
            if (seen.Count == 0 && !p.Flags.Contains('K') || p.Pts < 0 || p.Pts >= part.VideoDuration ||
                p.Pts % step != 0 || p.Duration != step || !seen.Add(p.Pts) ||
                previous is { } dts && p.Dts != dts + step || p.Skip != 0 || p.Discard != 0 || p.OtherSideData.Length != 0)
                throw new InvalidDataException("Die Bildpakete haben keine sicher verbindbare Zeitstruktur oder keinen unabhängigen Anfang.");
            previous = p.Dts;
        }
        if (seen.Count == 0 || seen.Count != part.VideoDuration / step)
            throw new InvalidDataException("Im Filmteil fehlen Bildzeiten oder Bildpakete.");
    }

    public async Task ValidateJoinSourceAudioAsync(JoinPart part, CancellationToken token)
    {
        var clock = new JoinAudioClock(part.AudioSamples);
        await foreach (var packet in ReadAsync(part.Path, part.AudioIndex, token).ConfigureAwait(false))
            clock.Add(packet);
        clock.Complete();
    }

    internal sealed class JoinAudioClock(long samples)
    {
        private long? _next;
        private long _discard;
        private int _count;
        internal void Add(CopyPacket p)
        {
            // Source time_base is already proven to be exactly 1/sample_rate.
            if (p.Pts != p.Dts || p.Duration <= 0 || p.Skip > p.Duration ||
                p.OtherSideData.Length != 0 || _count == 0 && checked(p.Pts + p.Skip) != 0 ||
                _count > 0 && (p.Pts != _next || p.Skip != 0 || _discard != 0))
                throw new InvalidDataException("Die gewählte Quelltonspur enthält eine Lücke, Überlappung oder unerwartete Auffüllung.");
            _next = checked(p.Pts + p.Duration); _discard = p.Discard; _count++;
        }
        internal void Complete()
        {
            if (_count == 0 || _next != samples && _next - _discard != samples)
                throw new InvalidDataException("Das Zeitstempelende der gewählten Quelltonspur stimmt nicht mit dem Film überein.");
        }
    }

    /// <summary>Streaming comparison; PTS may reorder with B-frames, DTS must advance in packet order.</summary>
    public async Task VerifyJoinedVideoAsync(JoinPart first, JoinPart second, string output, CancellationToken token)
    {
        var actual = await InspectAsync(output, 0, token).ConfigureAwait(false);
        if (actual.Start != 0 || (BigInteger)actual.Duration * actual.Clock.N * first.VideoTimeBase * second.VideoTimeBase !=
            ((BigInteger)first.VideoDuration * second.VideoTimeBase + (BigInteger)second.VideoDuration * first.VideoTimeBase) * actual.Clock.D)
            throw new InvalidDataException("Die Bildlaufzeit des Ergebnisses stimmt nicht.");
        await using var target = ReadAsync(output, 0, token).GetAsyncEnumerator(token);
        BigInteger offsetN = 0; BigInteger offsetD = 1;
        long? lastDts = null;
        foreach (var part in new[] { first, second })
        {
            var original = await InspectAsync(part.Path, part.VideoIndex, token).ConfigureAwait(false);
            if (original.Description != actual.Description)
                throw new InvalidDataException("Die Bildkonfiguration wurde beim Zusammenfügen verändert.");
            await foreach (var p in ReadAsync(part.Path, part.VideoIndex, token).ConfigureAwait(false))
            {
                if (!await target.MoveNextAsync().ConfigureAwait(false)) throw new InvalidDataException("Im Ergebnis fehlen Bildpakete.");
                var q = target.Current;
                bool Time(long x, long y) => ((BigInteger)x * offsetD + offsetN * part.VideoTimeBase) * actual.Clock.D ==
                    (BigInteger)y * actual.Clock.N * part.VideoTimeBase * offsetD;
                if (!Time(p.Pts, q.Pts) || !Time(p.Dts, q.Dts) ||
                    (BigInteger)p.Duration * actual.Clock.D != (BigInteger)q.Duration * actual.Clock.N * part.VideoTimeBase ||
                    p.Hash != q.Hash || p.Size != q.Size || p.Flags != q.Flags || p.Skip != q.Skip ||
                    p.Discard != q.Discard || p.OtherSideData != q.OtherSideData || lastDts is { } last && q.Dts <= last)
                    throw new InvalidDataException("Bildpakete oder Bildzeiten wurden am Übergang verändert. Das Ergebnis wird verworfen.");
                lastDts = q.Dts;
            }
            offsetN = first.VideoDuration; offsetD = first.VideoTimeBase;
        }
        if (await target.MoveNextAsync().ConfigureAwait(false)) throw new InvalidDataException("Das Ergebnis enthält zusätzliche Bildpakete.");
    }

    public async Task VerifyContinuousJoinAudioAsync(string output, long samples, int sampleRate, CancellationToken token)
    {
        var stream = await InspectAsync(output, 1, token).ConfigureAwait(false);
        if (stream.Start != 0 || (BigInteger)stream.Duration * stream.Clock.N * sampleRate != (BigInteger)samples * stream.Clock.D)
            throw new InvalidDataException("Die fertige Tonspur hat eine abweichende Wiedergabedauer.");
        long? next = null; var count = 0; var ended = false; long lastDiscard = 0;
        await foreach (var p in ReadAsync(output, 1, token).ConfigureAwait(false))
        {
            var sampleDuration = (BigInteger)p.Duration * stream.Clock.N * sampleRate;
            if (ended || p.Pts != p.Dts || p.Duration <= 0 || next is { } expected && p.Pts != expected ||
                count > 0 && p.Skip != 0 || p.Skip > 1024 || sampleDuration > (BigInteger)1024 * stream.Clock.D ||
                p.Discard >= 1024 ||
                p.OtherSideData.Length != 0)
                throw new InvalidDataException($"Die AAC-Zeitachse ist nicht durchgehend oder enthält unerwartete Auffüllung. Paket {count}: PTS={p.Pts}, DTS={p.Dts}, Dauer={p.Duration}, Skip={p.Skip}, Discard={p.Discard}, Zusatz={p.OtherSideData}, erwartet={next}.");
            if (count == 0 && (BigInteger)(p.Pts + p.Skip) * stream.Clock.N * sampleRate != 0)
                throw new InvalidDataException("Der AAC-Anfang stimmt nicht mit dem Film überein.");
            next = checked(p.Pts + p.Duration); count++;
            ended = p.Discard > 0 || sampleDuration < (BigInteger)1024 * stream.Clock.D;
            lastDiscard = p.Discard;
        }
        // ffprobe variants report the final duration either including or excluding discard padding.
        // The stream presentation duration and the independent complete decode must also match.
        var end = (BigInteger)(next ?? 0) * stream.Clock.N * sampleRate;
        var expectedEnd = (BigInteger)samples * stream.Clock.D;
        if (count == 0 || end != expectedEnd && end - (BigInteger)lastDiscard * stream.Clock.D != expectedEnd)
            throw new InvalidDataException("Das AAC-Ende stimmt nicht mit dem Film überein.");
    }

    private static string Text(JsonElement element, string key) => element.TryGetProperty(key, out var v) ? v.ToString() : "";
    private static long Number(JsonElement element, string key) => long.TryParse(Text(element, key), NumberStyles.Integer,
        CultureInfo.InvariantCulture, out var value) ? value : throw new InvalidDataException($"Exakte Medienangabe fehlt: {key}.");
}
