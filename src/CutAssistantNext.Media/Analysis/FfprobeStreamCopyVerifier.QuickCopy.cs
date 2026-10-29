using System.Globalization;
using System.Numerics;
using System.Text.Json;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Analysis;

internal sealed record QuickDtsCorrection(int StreamIndex, long Previous, long Original, long Corrected);
internal sealed record QuickAudioSource(long Count, long Samples, int Block, FfprobeStreamCopyVerifier.CopyPacket First,
    FfprobeStreamCopyVerifier.CopyPacket Last, long EndPadding);
internal sealed record QuickAudioVerification(long Packets, long ReferenceSamples, long ExpectedDecodedSamples,
    long PresentationSamples, int SampleRate, string Codec, IReadOnlyList<QuickDtsCorrection> Corrections,
    long InnerTrimSamples, long LostEndPadding);

public sealed partial class FfprobeStreamCopyVerifier
{
    internal async Task<string> QuickConfigurationAsync(string path, int index, int rate, CancellationToken token)
    {
        var stream = await InspectAsync(path, index, token).ConfigureAwait(false);
        if (stream.Clock.N != 1 || stream.Clock.D != rate || stream.Start != 0)
            throw new InvalidDataException("QUICK AND COPY benötigt eine genaue Tonzeitbasis und Präsentationsstart null.");
        // Codec properties, excluding language/title: their explicit mapping is shown by the UI.
        return string.Join('|', stream.Description.Split('|').Take(9));
    }

    internal async Task<QuickAudioSource> AnalyzeQuickSourceAsync(JoinPart part, CancellationToken token)
    {
        var block = part.AudioCodec switch
        {
            "aac" when part.AudioProfile == "LC" => 1024,
            "ac3" => 1536,
            _ => throw new InvalidDataException("QUICK AND COPY unterstützt hier AAC-LC und AC-3.")
        };
        var clock = new JoinAudioClock(part.AudioSamples);
        CopyPacket? first = null, last = null;
        long count = 0;
        await foreach (var packet in ReadAsync(part.Path, part.AudioIndex, token).ConfigureAwait(false))
        {
            clock.Add(packet);
            if (packet.Duration > block || packet.Skip > (block == 1024 ? 1024 : 256))
                throw new InvalidDataException("Unbekanntes Codecblock- oder Vorlaufformat.");
            first ??= packet; last = packet; count++;
        }
        clock.Complete();
        var padding = checked(count * block - first!.Skip - part.AudioSamples);
        if (padding < 0 || padding >= block || last!.Discard != padding &&
            !(last.Discard == 0 && last.Duration == block - padding))
            throw new InvalidDataException("Codecblöcke und Quellsamples erklären die Randauffüllung nicht vollständig.");
        return new(count, part.AudioSamples, block, first, last, padding);
    }

    internal async Task VerifyQuickPreparedAudioAsync(JoinPart source, QuickAudioSource proof, string output,
        int outputIndex, CancellationToken token)
    {
        if (await QuickConfigurationAsync(source.Path, source.AudioIndex, source.SampleRate, token).ConfigureAwait(false) !=
            await QuickConfigurationAsync(output, outputIndex, source.SampleRate, token).ConfigureAwait(false))
            throw new InvalidDataException("Die Tonkonfiguration wurde bei der Vorbereitung verändert.");
        await using var actual = ReadAsync(output, outputIndex, token).GetAsyncEnumerator(token);
        long count = 0;
        await foreach (var p in ReadAsync(source.Path, source.AudioIndex, token).ConfigureAwait(false))
        {
            if (!await actual.MoveNextAsync().ConfigureAwait(false)) throw new InvalidDataException("Vorbereitete Tonpakete fehlen.");
            var q = actual.Current;
            // MP4 does not retain AC-3 end-discard in this muxer; the original proof remains authoritative.
            var comparable = source.AudioCodec == "ac3" && count == proof.Count - 1 && q.Discard == 0
                ? p with { Discard = 0 } : p;
            if (!Same(comparable, q, 1, source.SampleRate, 1, source.SampleRate))
                throw new InvalidDataException("Tonpakete oder Zeiten wurden bei der Vorbereitung unerwartet verändert.");
            count++;
        }
        if (await actual.MoveNextAsync().ConfigureAwait(false) || count != proof.Count)
            throw new InvalidDataException("Die vorbereitete Paketanzahl stimmt nicht.");
    }

    internal async Task ValidateQuickStreamCountAsync(string path, int audioCount, CancellationToken token)
    {
        var probe = await new ToolProbeRunner().RunAsync(executable,
            ["-v", "error", "-show_streams", "-of", "json", path], token).ConfigureAwait(false);
        if (probe.ExitCode != 0 || !string.IsNullOrWhiteSpace(probe.StandardError))
            throw new InvalidDataException("Die Ausgabespuren konnten nicht geprüft werden.");
        using var doc = JsonDocument.Parse(probe.StandardOutput);
        var streams = doc.RootElement.GetProperty("streams").EnumerateArray().ToArray();
        if (streams.Length != audioCount + 1 || Text(streams[0], "codec_type") != "video" || Number(streams[0], "index") != 0 ||
            streams.Skip(1).Where((s, i) => Text(s, "codec_type") != "audio" || Number(s, "index") != i + 1).Any())
            throw new InvalidDataException("Die erwartete Bild-/Tonspurzuordnung stimmt nicht mit der Ausgabe überein.");
    }

    internal async Task<QuickAudioVerification> VerifyQuickJoinedAudioAsync(JoinPart first, JoinPart second,
        QuickAudioSource a, QuickAudioSource b, string output, int outputIndex, CancellationToken token)
    {
        var config = await QuickConfigurationAsync(first.Path, first.AudioIndex, first.SampleRate, token).ConfigureAwait(false);
        if (config != await QuickConfigurationAsync(second.Path, second.AudioIndex, second.SampleRate, token).ConfigureAwait(false) ||
            config != await QuickConfigurationAsync(output, outputIndex, first.SampleRate, token).ConfigureAwait(false))
            throw new InvalidDataException("Die kopierten Tonkonfigurationen passen nicht zusammen.");
        await using var actual = ReadAsync(output, outputIndex, token).GetAsyncEnumerator(token);
        var rules = new QuickAudioPacketRules(a, b, outputIndex, first.SampleRate, first.AudioCodec);
        foreach (var (part, proof, offset) in new[] { (first, a, 0L), (second, b, first.AudioSamples) })
        {
            long index = 0;
            await foreach (var source in ReadAsync(part.Path, part.AudioIndex, token).ConfigureAwait(false))
            {
                if (!await actual.MoveNextAsync().ConfigureAwait(false)) throw new InvalidDataException("In der Ausgabe fehlen gewählte Tonpakete.");
                rules.Add(source, actual.Current, offset, index++, proof.Count);
            }
            if (index != proof.Count) throw new InvalidDataException("Die Quellpaketanzahl hat sich verändert.");
        }
        if (await actual.MoveNextAsync().ConfigureAwait(false)) throw new InvalidDataException("Zusätzliche Tonpakete in der Ausgabe.");
        var result = rules.Complete();
        var stream = await InspectAsync(output, outputIndex, token).ConfigureAwait(false);
        if (stream.Duration != result.PresentationSamples)
            throw new InvalidDataException("Die Tonspurdauer stimmt nicht mit den nachgewiesenen Paketzeiten überein.");
        return result;
    }

    /// <summary>Exact exceptions at one known audio splice, never a general timestamp tolerance.</summary>
    internal sealed class QuickAudioPacketRules(QuickAudioSource first, QuickAudioSource second,
        int outputIndex, int sampleRate, string codec)
    {
        private long _count;
        private CopyPacket? _previous;
        private CopyPacket? _previousSource;
        private CopyPacket? _first;
        private readonly List<QuickDtsCorrection> _corrections = [];

        internal void Add(CopyPacket p, CopyPacket q, long offset, long partIndex, long partCount)
        {
            var secondFirst = _count == first.Count;
            var expected = checked(p.Pts + offset);
            if (codec == "aac" && secondFirst && _previous is not null && expected <= _previous.Dts)
            {
                var corrected = checked(_previous.Dts + 1);
                if (corrected - expected > 1025) throw new InvalidDataException("Zu große AAC-DTS-Korrektur.");
                _corrections.Add(new(outputIndex, _previous.Dts, expected, corrected)); expected = corrected;
            }
            var final = offset != 0 && partIndex == partCount - 1;
            var skip = _count == 0 ? first.First.Skip : 0;
            var discardAllowed = final
                ? q.Discard == second.EndPadding || q.Discard == 0 && (codec == "ac3" || q.Duration == second.Block - second.EndPadding)
                : q.Discard == 0;
            var flags = secondFirst && p.Skip > 0 ? p.Flags.Replace('D', '_') : p.Flags;
            if (p.Hash != q.Hash || p.Size != q.Size || q.Flags != flags || p.OtherSideData.Length != 0 || q.OtherSideData.Length != 0 ||
                q.Pts != expected || q.Dts != expected || q.Skip != skip || !discardAllowed ||
                _previous is not null && q.Dts <= _previous.Dts)
                throw new InvalidDataException($"QUICK AND COPY: Paketdaten oder nicht freigegebene Tonzeitänderung bei Paket {_count}.");
            if (_count == 0 && checked(q.Pts + q.Skip) != 0)
                throw new InvalidDataException("Der kopierte Ton startet nicht passend zum Bild.");
            if (_previous is not null)
            {
                var spliceDuration = codec == "aac" && (_count == first.Count || _count == first.Count + 1);
                var duration = spliceDuration ? checked(q.Dts - _previous.Dts) : _previousSource!.Duration;
                if (duration <= 0 || duration > first.Block || _previous.Duration != duration)
                    throw new InvalidDataException("Nicht freigegebene Audiopaketdaueränderung.");
                if (secondFirst && codec == "ac3")
                {
                    var overlap = checked(_previous.Pts + _previous.Duration - q.Pts);
                    var explained = checked(first.Last.Pts + first.Last.Duration - first.Samples + second.First.Skip);
                    if (overlap != explained || overlap < 0 || overlap >= first.Block)
                        throw new InvalidDataException("Die AC-3-Nahtüberlappung ist nicht durch die Quellränder erklärt.");
                }
            }
            _first ??= q; _previous = q; _previousSource = p; _count++;
        }

        internal QuickAudioVerification Complete()
        {
            if (_count != checked(first.Count + second.Count) || _previous is null || _first is null)
                throw new InvalidDataException("Unvollständige Tonpaketfolge.");
            var tail = codec == "aac" ? second.EndPadding : _previous.Discard;
            if (codec == "aac" && !(_previous.Duration == second.Block && _previous.Discard == tail ||
                _previous.Duration == second.Block - tail && _previous.Discard == 0) ||
                codec == "ac3" && _previous.Duration != second.Block)
                throw new InvalidDataException("Die äußere Tonauffüllung ist nicht erklärt.");
            var expectedSamples = checked(_count * first.Block - _first.Skip - tail);
            var reference = checked(first.Samples + second.Samples);
            ValidateQuickSampleDifference(expectedSamples - reference, sampleRate, codec);
            var end = checked(_previous.Pts + _previous.Duration - _previous.Discard);
            // Some muxers include AC-3 padding in the stream duration; AAC must end exactly on the reference clock.
            if (end != reference + (codec == "ac3" ? second.EndPadding - tail : 0))
                throw new InvalidDataException("Ungeklärte Audiolaufzeit oder schleichende Zeitverschiebung.");
            return new(_count, reference, expectedSamples, end, sampleRate, codec, _corrections.ToArray(),
                checked(first.EndPadding + second.First.Skip), second.EndPadding - tail);
        }
    }

    internal static void ValidateQuickSampleDifference(long difference, int rate, string codec)
    {
        var maximum = codec switch { "aac" => 3072, "ac3" => 3072, _ => throw new InvalidDataException("Unbekannter Kopiercodec.") };
        if (rate is not (44100 or 48000) || difference < 0 || difference > maximum || (BigInteger)difference * 1000 > (BigInteger)rate * 80)
            throw new InvalidDataException("Die Tonabweichung überschreitet die freigegebene Grenze (maximal 80 ms und begrenzte Codecblöcke).");
    }
}
