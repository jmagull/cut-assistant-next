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

                        count++;
                        continue;
                    }
                    first ??= packet;
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
                    if (transitions.Count > 0 && minimum <= transitions[^1].Seconds || previousMaximum > minimum)
                        throw new NotSupportedException("Überlappende oder ungeordnete Segmentpräsentation ist nicht eindeutig zuzuordnen.");
                    if (previousMaximum == minimum)
                        progress?.Report(new(CutProgressKind.Output, $"Übergang {i}: gleiche Bildzeit am Abschnittswechsel; dekodierter neuer Anfang wird geprüft."));
                    transitions.Add(new(minimum, first.Position));
                }
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
        }
        foreach (var (path, stamp) in stamps)
            if (FileStamp.Read(path) != stamp) throw new IOException("Eine Mediendatei wurde während der Analyse verändert.");
        var identity = await CutOutputIdentityVerifier.ReadAsync(output, token).ConfigureAwait(false);
        if (FileStamp.Read(output) != stamps[output]) throw new IOException("Ausgabedatei wurde beim Prüfsummenlesen verändert.");
        return new(Formatting.Time(info.Duration), transitions.Select(t => Formatting.Time(t.Seconds)), identity);
    }

    private sealed record Anchor(decimal Seconds, string FirstPacketPosition);
}
