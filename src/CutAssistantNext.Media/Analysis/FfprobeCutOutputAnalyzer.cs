using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Analysis;

/// <summary>Matches all video packets to their segment, then decodes each output anchor.</summary>
public sealed class FfprobeCutOutputAnalyzer : ICutOutputAnalyzer
{
    private readonly IVideoProbe probe;
    public FfprobeCutOutputAnalyzer(string ffprobePath) : this(new FfprobeVideoProbe(ffprobePath)) { }
    internal FfprobeCutOutputAnalyzer(IVideoProbe probe) => this.probe = probe ?? throw new ArgumentNullException(nameof(probe));

    public Task<CutOutputTimeline> AnalyzeAsync(string outputFilePath, IReadOnlyList<string> segmentFilePaths,
        IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken) =>
        Task.Run(() => AnalyzeCoreAsync(outputFilePath, segmentFilePaths, progress, cancellationToken), cancellationToken);

    private async Task<CutOutputTimeline> AnalyzeCoreAsync(string output, IReadOnlyList<string> segments,
        IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        if (segments.Count == 0) throw new ArgumentException("Mindestens ein Segment ist erforderlich.");
        output = Path.GetFullPath(output);
        var inputs = segments.Select(Path.GetFullPath).ToArray();
        if (inputs.Distinct(StringComparer.OrdinalIgnoreCase).Count() != inputs.Length ||
            inputs.Length > 1 && inputs.Contains(output, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Ausgabe und Segmentdateien müssen verschieden sein.");
        var stamps = inputs.Append(output).Distinct(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(p => p, FileStamp.Read, StringComparer.OrdinalIgnoreCase);
        var info = await probe.InspectAsync(output, token);
        var transitions = new List<Anchor>();

        decimal? previousMaximum = null;
        PresentationBounds? previousBounds = null;
        long? previousDts = null;
        var orderedDts = true;
        var hasOverlap = false;
        await using (var actual = probe.ReadPacketsAsync(output, token).GetAsyncEnumerator(token))
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                progress?.Report(new(CutProgressKind.Status, $"Schnittstellen werden geprüft: Abschnitt {i + 1} von {inputs.Length} …"));
                await probe.InspectAsync(inputs[i], token);
                var count = 0;
                decimal minimum = decimal.MaxValue, maximum = decimal.MinValue;
                Packet? first = null;
                var bounds = new PresentationBounds();
                await foreach (var expected in probe.ReadPacketsAsync(inputs[i], token))
                {
                    if (!await actual.MoveNextAsync() || actual.Current.Hash != expected.Hash)
                        throw new InvalidDataException($"Paketfolge stimmt bei Segment {i + 1}, Paket {count + 1} nicht überein.");
                    var packet = actual.Current;
                    if (packet.Dts is not { } dts || previousDts is { } lastDts && dts <= lastDts) orderedDts = false;
                    previousDts = packet.Dts;
                    if (packet.Pts < 0 || packet.Flags.Contains('C'))
                        throw new NotSupportedException("Verworfene, beschädigte oder negative Video-PTS werden nicht als verifiziert ausgegeben.");
                    if (packet.Flags.Contains('D'))
                    {
                        if (i != inputs.Length - 1 || info.TimeBase.Seconds(packet.Pts) < info.Duration)
                            throw new NotSupportedException("Verworfene Bilder innerhalb des Films sind nicht eindeutig zuzuordnen.");
                        // MP4 edit lists can discard a final reordered sample at/after EOF.
                        // Verify its bytes, but do not treat a non-presented tail sample as visible timing.

                        count++;
                        continue;
                    }
                    first ??= packet;
                    bounds.Add(packet);
                    minimum = Math.Min(minimum, info.TimeBase.Seconds(packet.Pts));
                    maximum = Math.Max(maximum, info.TimeBase.Seconds(packet.Pts));
                    count++;
                }
                if (count == 0 || first is null || !first.Flags.Contains('K') ||
                    info.TimeBase.Seconds(first.Pts) != minimum || minimum >= info.Duration)
                    throw new NotSupportedException($"Segment {i + 1} hat keinen eindeutig prüfbaren ersten Zugriffspunkt.");
                if (i == 0 && minimum != 0)
                    throw new NotSupportedException("Die Videopräsentation muss für die Übergangsprüfung bei null beginnen.");
                if (i > 0)
                {
                    if (transitions.Count > 0 && minimum <= transitions[^1].Seconds)
                        throw new NotSupportedException("Überlappende oder ungeordnete Segmentpräsentation ist nicht eindeutig zuzuordnen.");
                    OverlapBoundary? overlap = null;
                    if (previousMaximum > minimum)
                    {
                        overlap = LocateSmallOverlap(info, previousBounds!, bounds);
                        hasOverlap = true;
                        progress?.Report(new(CutProgressKind.Output,
                            $"Übergang {i}: geringfügig überlappende Bildzeiten; Paketfolge und dekodierter Abschnittswechsel werden zugeordnet."));
                    }
                    if (previousMaximum == minimum)
                        progress?.Report(new(CutProgressKind.Output, $"Übergang {i}: gleiche Bildzeit am Abschnittswechsel; dekodierter neuer Anfang wird geprüft."));
                    transitions.Add(new(minimum, first.Position, overlap));
                }
                previousMaximum = maximum;
                previousBounds = bounds;
            }
            if (await actual.MoveNextAsync()) throw new InvalidDataException("Ausgabe enthält zusätzliche Videopakete.");
        }
        if (hasOverlap && !orderedDts)
            throw new NotSupportedException("Überlappende Bildzeiten benötigen vollständige, streng aufsteigende Video-DTS.");
        // Packet matching identifies membership; a local decoder confirms the anchor is actually output.
        for (var i = 0; i < transitions.Count; i++)
        {
            var item = transitions[i];
            var frames = await probe.DecodeAsync(output, item.Seconds - 1, item.Seconds + 1, token);
            if (!frames.Any(f => f.Position == item.FirstPacketPosition && info.TimeBase.Seconds(f.Pts) == item.Seconds))
                throw new InvalidDataException($"Der Anfang von Segment {i + 2} wurde nicht dekodiert bestätigt.");
            if (item.Overlap is { } overlap) ValidateOverlapFrames(frames, overlap);
        }
        foreach (var (path, stamp) in stamps)
            if (FileStamp.Read(path) != stamp) throw new IOException("Eine Mediendatei wurde während der Analyse verändert.");
        var identity = await CutOutputIdentityVerifier.ReadAsync(output, token).ConfigureAwait(false);
        if (FileStamp.Read(output) != stamps[output]) throw new IOException("Ausgabedatei wurde beim Prüfsummenlesen verändert.");
        return new(Formatting.Time(info.Duration), transitions.Select(t => Formatting.Time(t.Seconds)), identity, hasOverlap);
    }

    private static OverlapBoundary LocateSmallOverlap(VideoInfo info, PresentationBounds before, PresentationBounds after)
    {
        if (!string.Equals(info.CodecName, "h264", StringComparison.OrdinalIgnoreCase) ||
            before.Greatest.Count < 2 || after.Smallest.Count < 3 ||
            before.Greatest[0].Count != 1 || after.Smallest.Any(p => p.Count != 1))
            throw UnsupportedOverlap();
        var old = before.Greatest[0].Packet;
        var previous = before.Greatest[1].Packet;
        var first = after.Smallest[0].Packet;
        var next = after.Smallest[1].Packet;
        var third = after.Smallest[2].Packet;
        var step = checked(next.Pts - first.Pts);
        var overlap = checked(old.Pts - first.Pts);
        // The supported MP4Box shape splits one 25/50-fps presentation interval across
        // the final old packet and the first new packet. Do not equate packet duration
        // with frame spacing, or infer this from nominal/average frame rate.
        if (info.TimeBase.Seconds(step) is not (.02m or .04m) || third.Pts - next.Pts != step ||
            overlap <= 0 || overlap >= step || previous.Pts >= first.Pts ||
            before.Last?.Position != old.Position || first.Duration != overlap || old.Duration != step - overlap)
            throw UnsupportedOverlap();
        return new(previous, old, first, next, third);
    }

    private static void ValidateOverlapFrames(IReadOnlyList<DecodedFrame> frames, OverlapBoundary boundary)
    {
        var expected = new[] { boundary.Previous, boundary.Old, boundary.First, boundary.Next, boundary.Third };
        if (expected.Select(p => p.Position).Distinct(StringComparer.Ordinal).Count() != expected.Length)
            throw new InvalidDataException("Überlappende Bildanker benötigen verschiedene Paketpositionen.");
        var indices = frames.Select((frame, index) => (frame, index))
            .Where(p => p.frame.Position == boundary.First.Position && p.frame.Pts == boundary.First.Pts)
            .Select(p => p.index).ToArray();
        if (indices.Length != 1 || indices[0] < 2 || indices[0] + 2 >= frames.Count)
            throw new InvalidDataException("Der überlappende Abschnittswechsel ist nicht eindeutig dekodiert zugeordnet.");
        var first = indices[0];
        for (var i = 0; i < expected.Length; i++)
        {
            var frame = frames[first - 2 + i];
            if (frame.Position != expected[i].Position || frame.Pts != expected[i].Pts)
                throw new InvalidDataException("Die dekodierten Nachbarbilder passen nicht zum überlappenden Abschnittswechsel.");
        }
        for (var i = 1; i < frames.Count; i++)
            if (i != first && frames[i].Pts <= frames[i - 1].Pts)
                throw new InvalidDataException("Weitere gleiche oder rückläufige Decoder-Bildzeiten verhindern die eindeutige Zuordnung.");
    }

    private static NotSupportedException UnsupportedOverlap() => new(
        "Überlappende oder ungeordnete Segmentpräsentation ist nicht eindeutig zuzuordnen; die begrenzte H.264-Zuordnung passt nicht.");

    private sealed record Anchor(decimal Seconds, string FirstPacketPosition, OverlapBoundary? Overlap = null);
    private sealed record OverlapBoundary(Packet Previous, Packet Old, Packet First, Packet Next, Packet Third);

    private sealed class PresentationBounds
    {
        public List<Sample> Smallest { get; } = [];
        public List<Sample> Greatest { get; } = [];
        public Packet? Last { get; private set; }

        public void Add(Packet packet)
        {
            Add(Smallest, packet, 3, descending: false);
            Add(Greatest, packet, 2, descending: true);
            Last = packet;
        }

        private static void Add(List<Sample> samples, Packet packet, int limit, bool descending)
        {
            var same = samples.Find(p => p.Packet.Pts == packet.Pts);
            if (same is not null) { same.Count++; return; }
            if (samples.Count == limit && (descending ? packet.Pts < samples[^1].Packet.Pts : packet.Pts > samples[^1].Packet.Pts)) return;
            samples.Add(new(packet));
            samples.Sort((a, b) => descending ? b.Packet.Pts.CompareTo(a.Packet.Pts) : a.Packet.Pts.CompareTo(b.Packet.Pts));
            if (samples.Count > limit) samples.RemoveAt(limit);
        }
    }

    private sealed class Sample(Packet packet)
    {
        public Packet Packet { get; } = packet;
        public int Count { get; set; } = 1;
    }
}
