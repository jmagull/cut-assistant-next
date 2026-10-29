using System.Globalization;
using System.Text.Json;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Joining;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Analysis;

public sealed record QuickCopyMediaInfo(MediaAnalysisResult Media, string PixelFormat,
    string VideoTimeBase, string VideoConfiguration, string StreamLayout,
    IReadOnlyDictionary<int, string> AudioProfiles, IReadOnlyDictionary<int, string> AudioConfigurations,
    IReadOnlyList<string> Hints)
{
    public TimeSpan Duration => Media.Duration ?? throw new InvalidDataException("Die Filmlaufzeit fehlt.");
    public VideoStreamInfo Video => Media.VideoStreams.Single();
    public AudioStreamInfo Audio(int index) => Media.AudioStreams.SingleOrDefault(a => a.Index == index)
        ?? throw new InvalidDataException($"Die gewählte Tonspur {index} fehlt.");
}

public interface IQuickCopyMediaProbe
{
    Task<QuickCopyMediaInfo> InspectAsync(string path, CancellationToken token);
}

/// <summary>Header-only checks for the consciously selected, pragmatic stream-copy method.</summary>
public sealed class QuickCopyMediaProbe(string ffprobe, IToolProbeRunner? runner = null) : IQuickCopyMediaProbe
{
    private readonly IToolProbeRunner _runner = runner ?? new ToolProbeRunner();

    public async Task<QuickCopyMediaInfo> InspectAsync(string path, CancellationToken token)
    {
        var result = await _runner.RunAsync(ffprobe,
            ["-v", "warning", "-show_streams", "-show_format", "-show_data_hash", "sha256", "-of", "json", path], token).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidDataException("Die Filmdatei konnte nicht gelesen werden. " + result.StandardError);
        try
        {
            var info = Parse(result.StandardOutput);
            return info with { Hints = [.. info.Hints, .. result.StandardError.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)] };
        }
        catch (JsonException error) { throw new InvalidDataException("ffprobe liefert keine lesbaren Medienangaben.", error); }
    }

    internal static QuickCopyMediaInfo Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("format", out var format) || !root.TryGetProperty("streams", out var streams) || streams.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("Die Medienangaben sind unvollständig.");
        var name = Text(format, "format_name");
        var all = streams.EnumerateArray().ToArray();
        var videos = all.Where(s => Text(s, "codec_type") == "video").ToArray();
        var duration = Seconds(Text(format, "duration"));
        if (!name.Split(',').Contains("mp4") || videos.Length != 1 || duration is null || duration <= TimeSpan.Zero)
            throw new InvalidDataException("QUICK AND COPY benötigt eine lesbare MP4 mit einer Videospur und bekannter Laufzeit.");
        var v = videos[0];
        if (Text(v, "codec_name") != "h264" || Number(v, "width") <= 0 || Number(v, "height") <= 0)
            throw new InvalidDataException("QUICK AND COPY benötigt ein lesbares H.264-Bildformat.");
        var video = new VideoStreamInfo(Number(v, "index"), "h264", Text(v, "codec_long_name"), Number(v, "width"), Number(v, "height"),
            Text(v, "sample_aspect_ratio"), Text(v, "display_aspect_ratio"), null, Text(v, "field_order"));
        var audios = all.Where(s => Text(s, "codec_type") == "audio").ToArray();
        var tracks = audios.Select(a => new AudioStreamInfo(Number(a, "index"), Text(a, "codec_name"), Text(a, "codec_long_name"),
            Number(a, "sample_rate"), Number(a, "channels"), Text(a, "channel_layout"))
        {
            Language = a.TryGetProperty("tags", out var tags) ? Text(tags, "language") : null
        }).ToArray();
        var hints = new List<string>();
        var rate = Text(v, "r_frame_rate"); var average = Text(v, "avg_frame_rate");
        if (rate != average) hints.Add($"Bildratenangaben: nominal {rate}, im Mittel {average}. QUICK AND COPY prüft kein vollständiges Bildzeitraster.");
        var videoDuration = Seconds(Text(v, "duration"));
        foreach (var audio in audios)
            if (videoDuration is { } vd && Seconds(Text(audio, "duration")) is { } ad && vd != ad)
                hints.Add($"Bild-/Tonlaufzeiten unterscheiden sich: Bild {vd.TotalSeconds.ToString("0.000000", CultureInfo.InvariantCulture)} s, Tonspur {Number(audio, "index")} {ad.TotalSeconds.ToString("0.000000", CultureInfo.InvariantCulture)} s.");
        return new(new(name, Text(format, "format_long_name"), null, duration, [video], tracks), Text(v, "pix_fmt"),
            Text(v, "time_base"), Text(v, "extradata_hash"),
            string.Join('|', all.Select(s => $"{Text(s, "index")}:{Text(s, "codec_type")}:{Text(s, "codec_name")}:{Text(s, "time_base")}")),
            audios.ToDictionary(a => Number(a, "index"), a => Text(a, "profile")),
            audios.ToDictionary(a => Number(a, "index"), a => Text(a, "extradata_hash")), hints);
    }

    public static void ValidateCompatibility(QuickCopyMediaInfo first, QuickCopyMediaInfo second, IReadOnlyList<QuickCopyTrackPair> pairs)
    {
        if (first.Video.CodecName != second.Video.CodecName || first.Video.Width != second.Video.Width || first.Video.Height != second.Video.Height ||
            KnownDifferent(first.PixelFormat, second.PixelFormat) || KnownDifferent(first.Video.SampleAspectRatio, second.Video.SampleAspectRatio))
            throw new InvalidDataException("Die Bildformate passen nicht zur unveränderten Übernahme zusammen. QUICK AND COPY kodiert das Bild nicht neu.");
        foreach (var pair in pairs)
        {
            var a = first.Audio(pair.FirstAudioIndex); var b = second.Audio(pair.SecondAudioIndex);
            if (a.CodecName is not ("aac" or "ac3") || a.CodecName != b.CodecName || a.SampleRate is null or <= 0 || a.SampleRate != b.SampleRate ||
                a.Channels is null or <= 0 || a.Channels != b.Channels || KnownDifferent(a.ChannelLayout, b.ChannelLayout) ||
                KnownDifferent(first.AudioProfiles.GetValueOrDefault(a.Index), second.AudioProfiles.GetValueOrDefault(b.Index)) ||
                a.CodecName == "aac" && KnownDifferent(first.AudioConfigurations.GetValueOrDefault(a.Index), second.AudioConfigurations.GetValueOrDefault(b.Index)))
                throw new InvalidDataException("Die gewählten Tonspuren passen nicht zur unveränderten Übernahme zusammen. Bitte die Zuordnung prüfen.");
        }
    }

    internal static bool CanConcatDirectly(QuickCopyMediaInfo first, QuickCopyMediaInfo second, IReadOnlyList<QuickCopyTrackPair> pairs) =>
        first.StreamLayout == second.StreamLayout && first.Video.Index == second.Video.Index &&
        pairs.All(p => p.FirstAudioIndex == p.SecondAudioIndex);

    private static bool KnownDifferent(string? a, string? b) => !string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(b) && a != b;
    private static string Text(JsonElement value, string name) => value.TryGetProperty(name, out var p) ? p.ToString() : "";
    private static int Number(JsonElement value, string name) => int.TryParse(Text(value, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;
    private static TimeSpan? Seconds(string text) => double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var n) &&
        double.IsFinite(n) && n > 0 && n < TimeSpan.MaxValue.TotalSeconds ? TimeSpan.FromSeconds(n) : null;
}
