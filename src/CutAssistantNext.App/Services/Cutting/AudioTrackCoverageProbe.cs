using System.Globalization;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Services.Cutting;

internal enum AudioTrackState { Unproven, Damaged, Usable }
internal readonly record struct AudioTrackCheck(AudioTrackState State, string Description);

/// <summary>Consumes ffprobe's selected decoded frames without buffering a movie-length probe.</summary>
internal sealed class AudioTrackCoverageProbe(AudioStreamInfo track, (long Start, long End)[] ranges)
    : IProgress<CutProgressUpdate>
{
    private readonly object _gate = new();
    private long? _firstStart;
    private long _previousStart;
    private long _lastEnd;
    private long _lastSampleCount;
    private long _declaredStart;
    private long _declaredEnd;
    private int _streamCount;
    private bool _verifiedMetadata;
    private bool _badFrames;
    private bool _missingOrOverlappingSamples;
    private bool _unexpectedMessage;
    private bool _decoderError;

    public void Report(CutProgressUpdate update)
    {
        if (update.Kind != CutProgressKind.Output || string.IsNullOrWhiteSpace(update.Message)) return;
        var line = update.Message;
        lock (_gate)
        {
            if (line.StartsWith("frame|", StringComparison.Ordinal))
                ReadFrame(line);
            else if (line.StartsWith("stream|", StringComparison.Ordinal))
                ReadStream(line);
            else if (line.StartsWith($"[{track.CodecName} @ ", StringComparison.Ordinal) &&
                line.AsSpan(line.IndexOf(']') + 1).StartsWith(" [error] ", StringComparison.Ordinal))
                _decoderError = true;
            else
                _unexpectedMessage = true;
        }
    }

    internal AudioTrackCheck Result()
    {
        if (_streamCount != 1 || !_verifiedMetadata)
            return new(AudioTrackState.Unproven, "Codec, Sampletakt oder Spuridentität sind nicht nachgewiesen");
        if (_decoderError)
            return new(AudioTrackState.Damaged, "Decoder meldet einen Tonfehler");
        if (_badFrames || _unexpectedMessage || _firstStart is not { } first)
            return new(AudioTrackState.Unproven, "Dekodierung oder Zeitstempel konnten nicht eindeutig geprüft werden");
        if (_missingOrOverlappingSamples || first > ranges[0].Start || _lastEnd < ranges[^1].End)
            return new(AudioTrackState.Damaged, "Die dekodierte Zeitachse deckt die Behaltebereiche nicht exakt ab");
        // The motor verifies exact decoded AC-3 length. Only whole output codec blocks
        // have been established for this recovery path.
        if (track.CodecName == "ac3" && (first != _declaredStart || _lastEnd != _declaredEnd || _lastSampleCount != 1536))
            return new(AudioTrackState.Unproven, "Die AC-3-Spurlaufzeit endet in einem teilweise genutzten Block");
        if (track.CodecName == "ac3" && ranges.Sum(k => k.End - k.Start) % 1536 != 0)
            return new(AudioTrackState.Unproven, "Die AC-3-Ausgabelänge liegt nicht auf dem geprüften 1536-Sample-Raster");
        return new(AudioTrackState.Usable, "Sauber dekodiert und alle Behaltebereiche vollständig abgedeckt");
    }

    private void ReadFrame(string line)
    {
        if (!Number(line, "|stream_index=", out var stream) || stream != track.Index ||
            !Number(line, "|pts=", out var start) ||
            !Number(line, "|nb_samples=", out var count) || count <= 0 || start > long.MaxValue - count ||
            !Number(line, "|channels=", out var channels) || channels != track.Channels ||
            !Field(line, "|channel_layout=").SequenceEqual(track.ChannelLayout))
        {
            _badFrames = true;
            return;
        }
        var end = start + count;
        if (_firstStart is not null)
        {
            if (start <= _previousStart || end <= _lastEnd) _badFrames = true;
            if (start != _lastEnd &&
                ranges.Any(k => Math.Min(start, _lastEnd) < k.End && k.Start < Math.Max(start, _lastEnd)))
                _missingOrOverlappingSamples = true;
        }
        _firstStart ??= start;
        _previousStart = start;
        _lastEnd = end;
        _lastSampleCount = count;
    }

    private void ReadStream(string line)
    {
        _streamCount++;
        _verifiedMetadata = Number(line, "|index=", out var index) && index == track.Index &&
            Field(line, "|codec_name=").SequenceEqual(track.CodecName) &&
            (track.CodecName != "aac" || Field(line, "|profile=").SequenceEqual("LC")) &&
            Number(line, "|sample_rate=", out var rate) && rate == track.SampleRate &&
            Field(line, "|time_base=").SequenceEqual(FormattableString.Invariant($"1/{rate}")) &&
            Number(line, "|channels=", out var channels) && channels == track.Channels &&
            Field(line, "|channel_layout=").SequenceEqual(track.ChannelLayout) &&
            Number(line, "|bit_rate=", out var bitrate) && bitrate > 0;
        if (track.CodecName == "ac3")
        {
            if (Number(line, "|start_pts=", out _declaredStart) &&
                Number(line, "|duration_ts=", out var duration) && duration > 0 && _declaredStart <= long.MaxValue - duration)
                _declaredEnd = _declaredStart + duration;
            else _verifiedMetadata = false;
        }
    }

    private static bool Number(string line, string key, out long value) =>
        long.TryParse(Field(line, key), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);

    private static ReadOnlySpan<char> Field(string line, string key)
    {
        var start = line.IndexOf(key, StringComparison.Ordinal);
        if (start < 0) return [];
        var tail = line.AsSpan(start + key.Length);
        var separator = tail.IndexOf('|');
        return separator < 0 ? tail : tail[..separator];
    }
}
