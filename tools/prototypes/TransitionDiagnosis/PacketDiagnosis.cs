namespace TransitionDiagnosis;

public sealed class PacketDiagnosis
{
    public static void ValidateKeeps(IReadOnlyList<KeepRange> keeps, decimal margin)
    {
        if (keeps.Count == 0 || margin < 0 || margin > 60)
            throw new ArgumentException("Behaltebereiche erforderlich; Zuordnungsspielraum muss zwischen 0 und 60 Sekunden liegen.");
        for (var i = 0; i < keeps.Count; i++)
            if (keeps[i].Start < 0 || keeps[i].Duration <= 0 || i > 0 && keeps[i].Start < keeps[i - 1].End)
                throw new ArgumentException("Behaltebereiche müssen positiv, geordnet und überlappungsfrei sein.");
    }

    public PacketDiagnosis(VideoSnapshot source, VideoSnapshot output, IReadOnlyList<KeepRange> keeps,
        decimal marginSeconds, CancellationToken token = default)
    {
        ValidateKeeps(keeps, marginSeconds);
        Source = source;
        Output = output;
        // A repeated payload is deliberately not assigned to the first occurrence.
        var sourceHashes = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var i = 0; i < source.Packets.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            if (!sourceHashes.TryAdd(source.Packets[i].Hash, i)) sourceHashes[source.Packets[i].Hash] = -1;
        }

        var matches = output.Packets.Select(p => sourceHashes.TryGetValue(p.Hash, out var index) && index >= 0 ? (int?)index : null).ToArray();
        var resolved = ResolveAnchoredRuns(source, output, sourceHashes, matches, token);

        var assignments = new PacketAssignment[output.Packets.Count];
        var issues = new List<MappingIssue>();
        var bad = 0;
        var breaks = 0;
        var flags = 0;
        var blocks = new List<int>();
        for (var i = 0; i < output.Packets.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var packet = output.Packets[i];
            string? issue = null;
            int? sourceIndex = null, keep = null;
            if (!sourceHashes.ContainsKey(packet.Hash)) issue = "PayloadNotInSource";
            else if (matches[i] is not { } found) issue = "RepeatedPayloadInSource";
            else
            {
                sourceIndex = found;
                var time = source.Metadata.TimeBase.Seconds(source.Packets[found].Pts);
                if (time is null) issue = "SourcePtsMissing";
                else
                {
                    var candidates = Enumerable.Range(0, keeps.Count).Where(k =>
                        time >= keeps[k].Start - marginSeconds && time < keeps[k].End + marginSeconds).ToArray();
                    if (candidates.Length == 1) keep = candidates[0] + 1;
                    else issue = candidates.Length == 0 ? "OutsideKeepWindows" : "OverlappingKeepWindows";
                }
            }
            assignments[i] = new(sourceIndex, keep, issue);
            if (issue is not null)
            {
                bad++;
                if (issues.Count < 20) issues.Add(new(i, issue));
            }
            if (packet.Flags.Contains('D') || packet.Flags.Contains('C')) flags++;
            if (keep is { } k && (blocks.Count == 0 || blocks[^1] != k)) blocks.Add(k);
            if (i > 0 && sourceIndex is { } current && assignments[i - 1].SourceIndex is { } previous && current != previous + 1)
                breaks++;
        }
        Assignments = assignments;
        var segments = Enumerable.Range(1, keeps.Count).Select(k =>
        {
            var indices = Enumerable.Range(0, assignments.Length).Where(i => assignments[i].Keep == k).ToArray();
            var pts = indices.Select(i => output.Metadata.TimeBase.Seconds(output.Packets[i].Pts)).Where(t => t.HasValue).Select(t => t!.Value).ToArray();
            return new SegmentEvidence(k, indices.Length, pts.Length > 0 ? pts.Min() : null, pts.Length > 0 ? pts.Max() : null,
                indices.Length > 0 ? SourceTime(indices[0]) : null, indices.Length > 0 ? SourceTime(indices[^1]) : null);
        }).ToArray();
        var boundaries = new List<BoundaryEvidence>();
        decimal planned = 0;
        for (var k = 1; k < keeps.Count; k++)
        {
            planned += keeps[k - 1].Duration;
            var before = segments[k - 1].MaximumPtsSeconds;
            var next = segments[k].MinimumPtsSeconds;
            var first = Array.FindIndex(assignments, a => a.Keep == k + 1);
            boundaries.Add(new()
            {
                NewKeep = k + 1,
                PlannedSeconds = planned,
                PacketCandidateSeconds = next,
                PreviousMaximumPtsSeconds = before,
                PacketPresentation = before is null || next is null ? "Unknown" :
                    before > next ? "Overlap" : before == next ? "EqualPts" : "SeparatedPts",
                PacketNeighborhood = first < 0 ? [] : Enumerable.Range(Math.Max(0, first - 4),
                    Math.Min(assignments.Length, first + 5) - Math.Max(0, first - 4)).Select(DescribePacket).ToArray()
            });
        }
        Result = new(bad == 0 && output.Packets.Count > 0, blocks.SequenceEqual(Enumerable.Range(1, keeps.Count)) && bad == 0,
            source.Packets.Count, output.Packets.Count, bad, breaks, flags, resolved, issues, segments, boundaries);
    }

    private static int ResolveAnchoredRuns(VideoSnapshot source, VideoSnapshot output,
        IReadOnlyDictionary<string, int> hashes, int?[] matches, CancellationToken token)
    {
        var resolved = 0;
        for (var i = 0; i < matches.Length; i++)
        {
            token.ThrowIfCancellationRequested();
            if (matches[i] is not null) continue;
            var start = i;
            while (i < matches.Length && matches[i] is null) i++;
            if (start == 0 || i == matches.Length || matches[start - 1] is not { } left || matches[i] is not { } right) continue;
            // Both unique anchors must bound exactly one contiguous original sequence of the same length.
            // A source jump at a cut, a changed payload, or a missing neighbor never qualifies.
            if (right - left != i - start + 1) continue;
            var identical = true;
            for (var n = start; n < i; n++)
            {
                token.ThrowIfCancellationRequested();
                if (!hashes.TryGetValue(output.Packets[n].Hash, out var index) || index >= 0 ||
                    output.Packets[n].Hash != source.Packets[left + n - start + 1].Hash)
                { identical = false; break; }
            }
            if (!identical) continue;
            for (var n = start; n < i; n++) matches[n] = left + n - start + 1;
            resolved += i - start;
        }
        return resolved;
    }

    public VideoSnapshot Source { get; }
    public VideoSnapshot Output { get; }
    public IReadOnlyList<PacketAssignment> Assignments { get; }
    public DiagnosisResult Result { get; }

    private decimal? SourceTime(int outputIndex) => Assignments[outputIndex].SourceIndex is { } index
        ? Source.Metadata.TimeBase.Seconds(Source.Packets[index].Pts) : null;

    private PacketEvidence DescribePacket(int index)
    {
        var p = Output.Packets[index];
        var basis = Output.Metadata.TimeBase;
        return new(index, basis.Seconds(p.Pts), basis.Seconds(p.Dts), basis.Seconds(p.Duration), p.Position,
            p.Flags, Assignments[index].SourceIndex, SourceTime(index), Assignments[index].Keep);
    }

    public void AddDecodedFrames(BoundaryEvidence boundary, IReadOnlyList<VideoFrame> frames)
    {
        // Duplicate packet positions cannot act as a unique decoder anchor.
        var positions = Output.Packets.Select((p, i) => (p.Position, Index: i)).GroupBy(p => p.Position)
            .Where(g => g.Count() == 1).ToDictionary(g => g.Key, g => g.Single().Index);
        var basis = Output.Metadata.TimeBase;
        boundary.Frames = frames.Select(f =>
        {
            int? index = f.Position is { } position && positions.TryGetValue(position, out var i) ? i : null;
            return new FrameEvidence(basis.Seconds(f.Pts), basis.Seconds(f.Dts), basis.Seconds(f.BestEffortTimestamp),
                basis.Seconds(f.Duration), f.Position, f.PictureType, index is { } n ? Assignments[n].Keep : null,
                index is { } m ? Assignments[m].SourceIndex : null, index is { } s ? SourceTime(s) : null);
        }).ToArray();
        var firstNew = boundary.Frames.ToList().FindIndex(f => f.Keep == boundary.NewKeep);
        boundary.FirstDecodedNewKeepSeconds = firstNew >= 0 ? boundary.Frames[firstNew].PtsSeconds : null;
        boundary.DecodeStatus = ClassifyDecoded(boundary.Frames, boundary.NewKeep, Result.AllPacketsUniquelyMapped && Result.KeepOrderMatchesPlan);
    }

    public static string ClassifyDecoded(IReadOnlyList<FrameEvidence> frames, int newKeep, bool mappingComplete)
    {
        if (!mappingComplete) return "PacketMappingIncomplete";
        if (frames.Count == 0 || frames.Any(f => f.Keep is null || f.PtsSeconds is null)) return "MissingFrameEvidence";
        if (frames.Any(f => f.Keep != newKeep && f.Keep != newKeep - 1)) return "OtherKeepInWindow";
        var first = frames.ToList().FindIndex(f => f.Keep == newKeep);
        if (first <= 0) return "BothSidesNotObserved";
        if (frames.Skip(first).Any(f => f.Keep == newKeep - 1)) return "InterleavedKeeps";
        for (var i = 1; i < frames.Count; i++)
            if (frames[i].PtsSeconds < frames[i - 1].PtsSeconds) return "NonMonotonicDecodedPts";
        return frames[first - 1].PtsSeconds == frames[first].PtsSeconds ? "EqualDecodedBoundaryPts" : "SingleObservedSwitch";
    }
}
