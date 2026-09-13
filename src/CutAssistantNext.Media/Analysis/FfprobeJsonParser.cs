using System.Globalization;
using System.Text.Json;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public static class FfprobeJsonParser
{
    public static MediaAnalysisResult Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var videoStreams = new List<VideoStreamInfo>();
        var audioStreams = new List<AudioStreamInfo>();

        if (root.TryGetProperty("streams", out var streams) &&
            streams.ValueKind == JsonValueKind.Array)
        {
            foreach (var stream in streams.EnumerateArray())
            {
                var streamType = GetString(stream, "codec_type");
                var index = GetInt32(stream, "index") ?? -1;

                if (streamType == "video")
                {
                    videoStreams.Add(new VideoStreamInfo(
                        index,
                        GetString(stream, "codec_name"),
                        GetString(stream, "codec_long_name"),
                        GetInt32(stream, "width"),
                        GetInt32(stream, "height"),
                        GetString(stream, "sample_aspect_ratio"),
                        GetString(stream, "display_aspect_ratio"),
                        ParseFrameRate(GetString(stream, "avg_frame_rate")),
                        GetString(stream, "field_order"))
                    {
                        StartTimeSeconds = GetDouble(stream, "start_time"),
                        FrameCount = GetInt64(stream, "nb_frames")
                    });
                }
                else if (streamType == "audio")
                {
                    audioStreams.Add(new AudioStreamInfo(
                        index,
                        GetString(stream, "codec_name"),
                        GetString(stream, "codec_long_name"),
                        GetInt32(stream, "sample_rate"),
                        GetInt32(stream, "channels"),
                        GetString(stream, "channel_layout"))
                    {
                        StartTimeSeconds = GetDouble(stream, "start_time")
                    });
                }
            }
        }

        string? formatName = null;
        string? formatLongName = null;
        long? fileSizeBytes = null;
        TimeSpan? duration = null;

        if (root.TryGetProperty("format", out var format) &&
            format.ValueKind == JsonValueKind.Object)
        {
            formatName = GetString(format, "format_name");
            formatLongName = GetString(format, "format_long_name");
            fileSizeBytes = GetInt64(format, "size");

            var durationSeconds = GetDouble(format, "duration");

            if (durationSeconds.HasValue && durationSeconds.Value >= 0)
            {
                duration = TimeSpan.FromSeconds(durationSeconds.Value);
            }
        }

        return new MediaAnalysisResult(
            formatName,
            formatLongName,
            fileSizeBytes,
            duration,
            videoStreams,
            audioStreams)
        {
            StartTimeSeconds = root.TryGetProperty("format", out var formatInfo)
                ? GetDouble(formatInfo, "start_time") : null
        };
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static int? GetInt32(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number &&
            property.TryGetInt32(out var number))
        {
            return number;
        }

        return property.ValueKind == JsonValueKind.String &&
               int.TryParse(
                   property.GetString(),
                   NumberStyles.Integer,
                   CultureInfo.InvariantCulture,
                   out number)
            ? number
            : null;
    }

    private static long? GetInt64(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number &&
            property.TryGetInt64(out var number))
        {
            return number;
        }

        return property.ValueKind == JsonValueKind.String &&
               long.TryParse(
                   property.GetString(),
                   NumberStyles.Integer,
                   CultureInfo.InvariantCulture,
                   out number)
            ? number
            : null;
    }

    private static double? GetDouble(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number &&
            property.TryGetDouble(out var number))
        {
            return number;
        }

        return property.ValueKind == JsonValueKind.String &&
               double.TryParse(
                   property.GetString(),
                   NumberStyles.Float,
                   CultureInfo.InvariantCulture,
                   out number)
            ? number
            : null;
    }

    private static double? ParseFrameRate(string? frameRate)
    {
        if (string.IsNullOrWhiteSpace(frameRate))
        {
            return null;
        }

        var parts = frameRate.Split('/');

        if (parts.Length != 2 ||
            !double.TryParse(
                parts[0],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var numerator) ||
            !double.TryParse(
                parts[1],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var denominator) ||
            denominator == 0)
        {
            return null;
        }

        return numerator / denominator;
    }
}
