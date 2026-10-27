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

    internal async Task<string> PrepareAsync(string source, MediaAnalysisResult analysis,
        AudioStreamInfo selected, string work, string name, IProgress<CutProgressUpdate>? progress,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!VideoPreparation.IsMp4(analysis) || analysis.VideoStreams.Count != 1 ||
            !analysis.AudioStreams.Contains(selected) || selected.CodecName is not ("aac" or "ac3"))
            throw new ArgumentException("Bitte eine AAC- oder AC-3-Spur aus einer MP4-Datei mit einer Videospur wählen.");
        using var sourceLock = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (analysis.FileSizeBytes is { } size && size != sourceLock.Length)
            throw new IOException("Die geladene Datei hat sich geändert. Bitte lade sie erneut.");
        progress?.Report(new(CutProgressKind.Status, "Gewählte Tonspur wird vollständig geprüft …"));
        var check = new StrictProgress(progress);
        await _runner.RunAsync(_ffmpeg,
            ["-hide_banner", "-nostats", "-v", "warning", "-xerror", "-threads", "1",
             "-i", source, "-map", $"0:{selected.Index}", "-vn", "-sn", "-dn", "-f", "null", "-"],
            check, token).ConfigureAwait(false);
        if (check.HasOutput)
            throw new InvalidDataException("Die gewählte Tonspur konnte nicht ohne Warnung dekodiert werden.");
        var prepared = Path.Combine(work, name);
        progress?.Report(new(CutProgressKind.Status, "Kopie mit der gewählten Tonspur wird ohne Neukodierung erstellt …"));
        check = new StrictProgress(progress, allowAc3FrameSizeNotice: selected.CodecName == "ac3");
        await _runner.RunAsync(_ffmpeg,
            ["-hide_banner", "-nostats", "-v", "warning", "-n", "-copyts", "-i", source,
             "-map", $"0:{analysis.VideoStreams.Single().Index}", "-map", $"0:{selected.Index}",
             "-c", "copy", "-map_metadata", "0", "-avoid_negative_ts", "disabled", prepared],
            check, token).ConfigureAwait(false);
        if (check.HasOutput) throw new InvalidDataException("Die Tonspurkopie wurde mit Warnungen erstellt.");
        var actual = await _analyze(prepared, token).ConfigureAwait(false);
        VideoPreparation.Validate(analysis with { AudioStreams = [selected] }, actual);
        var audio = actual.AudioStreams.Single();
        if (selected.CodecName != audio.CodecName || selected.Language != audio.Language || selected.Title != audio.Title)
            throw new InvalidDataException("Codec oder Bezeichnung der gewählten Tonspur wurden verändert.");
        progress?.Report(new(CutProgressKind.Status, "Alle Bild- und Tonpakete sowie Zeitstempel werden exakt verglichen …"));
        await _verify(source, prepared,
            [(analysis.VideoStreams.Single().Index, actual.VideoStreams.Single().Index),
             (selected.Index, audio.Index)], token).ConfigureAwait(false);
        token.ThrowIfCancellationRequested();
        return prepared;
    }

    private sealed class StrictProgress(IProgress<CutProgressUpdate>? target, bool allowAc3FrameSizeNotice = false) : IProgress<CutProgressUpdate>
    {
        private int _output;
        internal bool HasOutput => Volatile.Read(ref _output) != 0;
        public void Report(CutProgressUpdate value)
        {
            // This muxer metadata notice is accepted only for AC-3 streamcopy, after a clean
            // full decode and subject to the exact packet/configuration comparison below.
            var metadataNotice = allowAc3FrameSizeNotice && System.Text.RegularExpressions.Regex.IsMatch(
                value.Message, @"^\[mp4 @ [0-9A-Fa-f]+\] track 1: codec frame size is not set$");
            if (value.Kind == CutProgressKind.Output && !string.IsNullOrWhiteSpace(value.Message) && !metadataNotice)
                Interlocked.Exchange(ref _output, 1);
            target?.Report(value);
        }
    }
}
