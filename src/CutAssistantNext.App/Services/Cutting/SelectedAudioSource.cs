using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>Strictly checks one source track and proves the lossless MP4 stream selection.</summary>
internal sealed class SelectedAudioSource
{
    private readonly string _ffmpeg;
    private readonly ICutToolRunner _runner;
    private readonly Func<string, CancellationToken, Task<MediaAnalysisResult>> _analyze;
    private readonly Func<string, string, IReadOnlyList<(int, int)>, CancellationToken, Task> _verify;

    internal SelectedAudioSource(string ffmpeg, string ffprobe, ICutToolRunner? runner = null,
        Func<string, CancellationToken, Task<MediaAnalysisResult>>? analyze = null,
        Func<string, string, IReadOnlyList<(int, int)>, CancellationToken, Task>? verify = null)
    {
        _ffmpeg = ffmpeg;
        _runner = runner ?? new CutToolRunner();
        _analyze = analyze ?? new FfprobeRunner(ffprobe).RunAsync;
        _verify = verify ?? new FfprobeStreamCopyVerifier(ffprobe).VerifyAsync;
    }

    internal Task<string> PrepareAsync(string source, MediaAnalysisResult analysis,
        AudioStreamInfo selected, string work, string name, IProgress<CutProgressUpdate>? progress,
        CancellationToken token) => PrepareAsync(source, analysis, [selected], work, name, progress, token);

    internal async Task<string> PrepareAsync(string source, MediaAnalysisResult analysis,
        IReadOnlyList<AudioStreamInfo> selected, string work, string name, IProgress<CutProgressUpdate>? progress,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!VideoPreparation.IsMp4(analysis) || analysis.VideoStreams.Count != 1 ||
            selected.Count == 0 || selected.Select(s => s.Index).Distinct().Count() != selected.Count ||
            selected.Any(s => !analysis.AudioStreams.Contains(s) || s.CodecName is not ("aac" or "ac3")))
            throw new ArgumentException("Bitte AAC- oder AC-3-Spuren aus einer MP4-Datei mit einer Videospur wählen.");
        using var sourceLock = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (analysis.FileSizeBytes is { } size && size != sourceLock.Length)
            throw new IOException("Die geladene Datei hat sich geändert. Bitte lade sie erneut.");
        foreach (var track in selected)
        {
            progress?.Report(new(CutProgressKind.Status, $"Tonspur {track.Index} wird vollständig geprüft …"));
            var check = new StrictProgress(progress);
            await _runner.RunAsync(_ffmpeg,
                ["-hide_banner", "-nostats", "-v", "warning", "-xerror", "-threads", "1",
                 "-i", source, "-map", $"0:{track.Index}", "-vn", "-sn", "-dn", "-f", "null", "-"],
                check, token).ConfigureAwait(false);
            if (check.HasOutput)
                throw new InvalidDataException($"Tonspur {track.Index} konnte nicht ohne Warnung dekodiert werden.");
        }

        var prepared = Path.Combine(work, name);
        progress?.Report(new(CutProgressKind.Status, "Kopie mit den gewählten Tonspuren wird ohne Neukodierung erstellt …"));
        var inputVideo = analysis.VideoStreams.Single().Index;
        var arguments = new List<string>
        {
            "-hide_banner", "-nostats", "-v", "warning", "-n", "-copyts", "-i", source, "-map", $"0:{inputVideo}"
        };
        foreach (var track in selected)
            arguments.AddRange(["-map", $"0:{track.Index}"]);
        arguments.AddRange(["-c", "copy", "-map_metadata", "0", "-avoid_negative_ts", "disabled", prepared]);
        var ac3Indices = selected.Select((track, index) => (track, index))
            .Where(pair => pair.track.CodecName == "ac3").Select(pair => pair.index + 1).ToHashSet();
        var muxCheck = new StrictProgress(progress, ac3Indices);
        await _runner.RunAsync(_ffmpeg, arguments, muxCheck, token).ConfigureAwait(false);
        if (muxCheck.HasOutput) throw new InvalidDataException("Die Tonspurkopie wurde mit Warnungen erstellt.");
        var actual = await _analyze(prepared, token).ConfigureAwait(false);
        VideoPreparation.Validate(analysis with { AudioStreams = selected }, actual);
        var map = new List<(int, int)> { (inputVideo, actual.VideoStreams.Single().Index) };
        foreach (var (expected, audio) in selected.Zip(actual.AudioStreams))
        {
            if (expected.CodecName != audio.CodecName || expected.Language != audio.Language || expected.Title != audio.Title)
                throw new InvalidDataException("Codec oder Bezeichnung einer gewählten Tonspur wurden verändert.");
            map.Add((expected.Index, audio.Index));
        }
        progress?.Report(new(CutProgressKind.Status, "Alle Bild- und Tonpakete sowie Zeitstempel werden exakt verglichen …"));
        await _verify(source, prepared, map, token).ConfigureAwait(false);
        token.ThrowIfCancellationRequested();
        return prepared;
    }

    private sealed class StrictProgress(IProgress<CutProgressUpdate>? target, IReadOnlySet<int>? ac3Indices = null) : IProgress<CutProgressUpdate>
    {
        private int _output;
        internal bool HasOutput => Volatile.Read(ref _output) != 0;
        public void Report(CutProgressUpdate value)
        {
            // This muxer metadata notice is accepted only for AC-3 streamcopy, after a clean
            // full decode and subject to the exact packet/configuration comparison below.
            var match = ac3Indices is { Count: > 0 } ? System.Text.RegularExpressions.Regex.Match(
                value.Message, @"^\[mp4 @ [0-9A-Fa-f]+\] track ([0-9]+): codec frame size is not set$") : null;
            var metadataNotice = match is { Success: true } && int.TryParse(match.Groups[1].Value, out var track) &&
                ac3Indices!.Contains(track);
            if (value.Kind == CutProgressKind.Output && !string.IsNullOrWhiteSpace(value.Message) && !metadataNotice)
                Interlocked.Exchange(ref _output, 1);
            target?.Report(value);
        }
    }
}
