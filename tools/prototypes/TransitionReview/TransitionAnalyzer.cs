using System.Security.Cryptography;

namespace TransitionReview;

internal sealed class TransitionAnalyzer(IVideoProbe probe)
{
    public async Task<AnalysisResult> AnalyzeAsync(string output, IReadOnlyList<string> segments, CancellationToken token)
    {
        if (segments.Count == 0) throw new ArgumentException("Mindestens ein Segment ist erforderlich.");
        output = Path.GetFullPath(output);
        var inputs = segments.Select(Path.GetFullPath).ToArray();
        if (inputs.Contains(output, StringComparer.OrdinalIgnoreCase) ||
            inputs.Distinct(StringComparer.OrdinalIgnoreCase).Count() != inputs.Length)
            throw new ArgumentException("Ausgabe und Segmentdateien müssen verschieden sein.");
        var stamps = inputs.Append(output).ToDictionary(p => p, FileStamp.Read, StringComparer.OrdinalIgnoreCase);
        var info = await probe.InspectAsync(output, token);
        var segmentEvidence = new List<SegmentEvidence>();
        var transitions = new List<OutputTransition>();
        var notices = new List<string>();
        long ordinal = 0;
        decimal? previousMaximum = null;
        await using (var actual = probe.ReadPacketsAsync(output, token).GetAsyncEnumerator(token))
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                var segmentInfo = await probe.InspectAsync(inputs[i], token);
                var count = 0;
                decimal minimum = decimal.MaxValue, maximum = decimal.MinValue;
                Packet? first = null;
                var firstOrdinal = ordinal;
                await foreach (var expected in probe.ReadPacketsAsync(inputs[i], token))
                {
                    if (!await actual.MoveNextAsync() || actual.Current.Hash != expected.Hash)
                        throw new InvalidDataException($"Paketfolge stimmt bei Segment {i + 1}, Paket {count + 1} nicht überein.");
                    var packet = actual.Current;
                    if (packet.Pts < 0 || packet.Flags.Contains('C'))
                        throw new NotSupportedException("Verworfene, beschädigte oder negative Video-PTS werden nicht als verifiziert ausgegeben.");
                    if (packet.Flags.Contains('D'))
                    {
                        if (i != inputs.Length - 1 || info.TimeBase.Seconds(packet.Pts) < info.Duration)
                            throw new NotSupportedException("Verworfene Bilder innerhalb des Films sind nicht eindeutig zuzuordnen.");
                        // MP4 edit lists can discard a final reordered sample at/after EOF.
                        // Verify its bytes, but do not treat a non-presented tail sample as visible timing.
                        notices.Add("Ein verworfenes Videopaket am/nach dem Dateiende wurde nur als Paketinhalt geprüft.");
                        count++;
                        ordinal++;
                        continue;
                    }
                    first ??= packet;
                    minimum = Math.Min(minimum, info.TimeBase.Seconds(packet.Pts));
                    maximum = Math.Max(maximum, info.TimeBase.Seconds(packet.Pts));
                    count++;
                    ordinal++;
                }
                if (count == 0 || first is null || !first.Flags.Contains('K') ||
                    info.TimeBase.Seconds(first.Pts) != minimum || minimum >= info.Duration)
                    throw new NotSupportedException($"Segment {i + 1} hat keinen eindeutig prüfbaren ersten Zugriffspunkt.");
                if (i == 0 && minimum != 0)
                    throw new NotSupportedException("Die Videopräsentation muss im Prototyp bei null beginnen.");
                if (i > 0)
                {
                    if (transitions.Count > 0 && minimum <= transitions[^1].Seconds || previousMaximum > minimum)
                        throw new NotSupportedException("Überlappende oder ungeordnete Segmentpräsentation ist nicht eindeutig zuzuordnen.");
                    var duplicate = previousMaximum == minimum;
                    if (duplicate) notices.Add($"Übergang {i}: letztes und erstes Bild haben denselben PTS; Wiedergabe gesondert prüfen.");
                    transitions.Add(new(i, minimum, firstOrdinal, first.Position, duplicate, "PacketSequenceMatched"));
                }
                segmentEvidence.Add(new(i + 1, count, segmentInfo.Duration));
                previousMaximum = maximum;
            }
            if (await actual.MoveNextAsync()) throw new InvalidDataException("Ausgabe enthält zusätzliche Videopakete.");
        }
        // Packet matching identifies membership; a local decoder confirms the anchor is actually output.
        for (var i = 0; i < transitions.Count; i++)
        {
            var item = transitions[i];
            var frames = await probe.DecodeAsync(output, item.Seconds - 1, item.Seconds + 1, token);
            if (!frames.Any(f => f.Position == item.FirstPacketPosition && info.TimeBase.Seconds(f.Pts) == item.Seconds))
                throw new InvalidDataException($"Der Anfang von Segment {i + 2} wurde nicht dekodiert bestätigt.");
            transitions[i] = item with { Evidence = "FullPacketSequenceAndDecodedAnchor" };
        }
        foreach (var (path, stamp) in stamps)
            if (FileStamp.Read(path) != stamp) throw new IOException("Eine Mediendatei wurde während der Analyse verändert.");
        var hash = await HashAsync(output, token);
        if (FileStamp.Read(output) != stamps[output]) throw new IOException("Ausgabedatei wurde beim Prüfsummenlesen verändert.");
        notices.Add("Die Ermittlung korrigiert ausschließlich Prüfpositionen. Fehlende Bilder werden nicht wiederhergestellt.");
        return new("prototype-v1", output, hash, stamps[output], info.Duration,
            segmentEvidence.AsReadOnly(), transitions.AsReadOnly(), notices.AsReadOnly());
    }

    public static async Task<string> HashAsync(string path, CancellationToken token)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true);
        return Convert.ToHexString(await SHA256.HashDataAsync(stream, token));
    }

    public static async Task ValidateForReviewAsync(AnalysisResult result, CancellationToken token)
    {
        if (result.Algorithm != "prototype-v1" || result.Duration <= 0 || result.Segments.Count == 0 ||
            result.Transitions.Count != result.Segments.Count - 1 ||
            result.Transitions.Any(t => t.Evidence != "FullPacketSequenceAndDecodedAnchor") ||
            result.Transitions.Select((t, i) => t.Index != i + 1 || t.Seconds <= 0 || t.Seconds >= result.Duration ||
                i > 0 && t.Seconds <= result.Transitions[i - 1].Seconds).Any(invalid => invalid))
            throw new InvalidDataException("Keine gültige verifizierte Prototyp-Ergebnisdatei.");
        if (FileStamp.Read(result.OutputFilePath) != result.OutputStamp ||
            await HashAsync(result.OutputFilePath, token) != result.OutputSha256)
            throw new IOException("Die Ausgabedatei passt nicht mehr zum gemessenen Ergebnis.");
    }
}
