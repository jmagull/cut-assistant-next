using System.Globalization;
using System.Text.Json;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

internal static class FfprobeFrameJsonParser
{
    internal static VideoFrameWindow Parse(string json, int videoStreamIndex)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (!root.TryGetProperty("streams", out var streams) ||
            streams.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException("ffprobe hat keine Streamdaten geliefert.");
        }

        var matchingStreams = streams.EnumerateArray()
            .Where(stream => ReadInteger(stream, "index") == videoStreamIndex)
            .ToArray();

        if (matchingStreams.Length != 1 ||
            ReadText(matchingStreams[0], "codec_type") != "video")
        {
            throw new FormatException("Der ausgewählte Videostream fehlt oder ist nicht eindeutig.");
        }

        var selectedStream = matchingStreams[0];
        var timeBaseParts = ReadText(selectedStream, "time_base")?.Split('/');

        if (timeBaseParts is not { Length: 2 } ||
            !int.TryParse(timeBaseParts[0], NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var numerator) ||
            !int.TryParse(timeBaseParts[1], NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var denominator) ||
            numerator <= 0 || denominator <= 0)
        {
            throw new FormatException("ffprobe hat keine gültige Video-Zeitbasis geliefert.");
        }

        var frames = new List<VideoFrameInfo>();

        if (root.TryGetProperty("frames", out var frameArray))
        {
            if (frameArray.ValueKind != JsonValueKind.Array)
            {
                throw new FormatException("Die ffprobe-Framedaten sind ungültig.");
            }

            foreach (var frame in frameArray.EnumerateArray())
            {
                if (ReadInteger(frame, "stream_index") != videoStreamIndex ||
                    ReadText(frame, "media_type") != "video")
                {
                    throw new FormatException("ffprobe hat Frames eines unerwarteten Streams geliefert.");
                }

                var keyFrame = ReadInteger(frame, "key_frame");
                if (keyFrame.HasValue && keyFrame.Value is not (0 or 1))
                {
                    throw new FormatException("Das Keyframe-Kennzeichen ist ungültig.");
                }

                frames.Add(new VideoFrameInfo(
                    frames.Count,
                    ReadInteger(frame, "pts"),
                    ReadInteger(frame, "best_effort_timestamp"),
                    ReadInteger(frame, "pkt_dts"),
                    ReadInteger(frame, "duration") ?? ReadInteger(frame, "pkt_duration"),
                    keyFrame.HasValue ? keyFrame.Value == 1 : null,
                    ReadText(frame, "pict_type")));
            }
        }

        decimal? containerStartTime = null;
        if (root.TryGetProperty("format", out var format))
        {
            containerStartTime = ReadDecimal(format, "start_time");
        }

        return new VideoFrameWindow(
            videoStreamIndex,
            new VideoTimeBase(numerator, denominator),
            ReadDecimal(selectedStream, "start_time"),
            containerStartTime,
            frames.AsReadOnly());
    }

    private static string? ReadText(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) ||
            value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : value.GetRawText();
    }

    private static long? ReadInteger(JsonElement element, string name)
    {
        var text = ReadText(element, name);
        if (text is null or "N/A")
        {
            return null;
        }

        if (long.TryParse(text, NumberStyles.Integer,
            CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new FormatException($"Ungültiger ffprobe-Ganzzahlwert: {name}.");
    }

    private static decimal? ReadDecimal(JsonElement element, string name)
    {
        var text = ReadText(element, name);
        if (text is null or "N/A")
        {
            return null;
        }

        if (decimal.TryParse(text, NumberStyles.Float,
            CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new FormatException($"Ungültiger ffprobe-Zeitwert: {name}.");
    }
}
