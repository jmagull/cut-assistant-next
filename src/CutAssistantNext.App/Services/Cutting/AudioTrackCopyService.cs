using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed record AudioTrackCopyChoice(AudioStreamInfo Stream)
{
    public string Description => $"Spur {Stream.Index}: {Stream.CodecName?.ToUpperInvariant()} · " +
        $"{Language(Stream.Language)} · {Stream.Channels} Kanäle · {Stream.SampleRate} Hz" +
        (string.IsNullOrWhiteSpace(Stream.Title) ? "" : $" · {Stream.Title}");

    private static string Language(string? code) => code?.ToLowerInvariant() switch
    {
        "deu" or "ger" or "de" => "Deutsch",
        "eng" or "en" => "Englisch",
        null or "" or "und" => "Sprache unbekannt",
        _ => code
    };

    internal static IReadOnlyList<AudioTrackCopyChoice> Choices(MediaAnalysisResult media) =>
        VideoPreparation.IsMp4(media) && media.VideoStreams.Count == 1 && media.AudioStreams.Count > 1
            ? media.AudioStreams.Where(s => s.CodecName is "aac" or "ac3")
                .Select(s => new AudioTrackCopyChoice(s)).ToArray() : [];

    internal string Notice(MediaAnalysisResult media) =>
        $"Tonspurkopie: nur {Description}. Entfallene Tonspuren: " +
        string.Join(", ", media.AudioStreams.Where(s => s.Index != Stream.Index)
            .Select(s => new AudioTrackCopyChoice(s).Description)) + ".";
}

internal sealed record AudioTrackCopyResult(string OutputFilePath, string Notice);

/// <summary>Copies the whole loaded file with one explicitly chosen track; never applies a cut plan.</summary>
internal sealed class AudioTrackCopyService(
    Func<string, MediaAnalysisResult, AudioStreamInfo, string, IProgress<CutProgressUpdate>?, CancellationToken, Task<string>> prepare)
{
    internal AudioTrackCopyService(string ffmpeg, string ffprobe)
        : this((source, media, selected, work, progress, token) =>
            new SelectedAudioSource(ffmpeg, ffprobe).PrepareAsync(
                source, media, selected, work, "selected-audio.mp4", progress, token))
    { }

    internal async Task<AudioTrackCopyResult> RunAsync(string source, string output,
        MediaAnalysisResult media, AudioTrackCopyChoice selected,
        IProgress<CutProgressUpdate>? progress = null, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        if (!Path.IsPathFullyQualified(source) || !Path.IsPathFullyQualified(output))
            throw new ArgumentException("Bitte vollständige Dateipfade verwenden.");
        source = Path.GetFullPath(source);
        output = Path.GetFullPath(output);
        if (string.Equals(source, output, StringComparison.OrdinalIgnoreCase) || File.Exists(output))
            throw new IOException("Bitte einen neuen Dateinamen wählen. Vorhandene Dateien werden nicht ersetzt.");
        if (!File.Exists(source)) throw new FileNotFoundException("Die geladene Datei fehlt.", source);
        if (!Path.GetExtension(output).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Die Tonspurkopie muss als MP4 gespeichert werden.");
        if (!AudioTrackCopyChoice.Choices(media).Contains(selected))
            throw new ArgumentException("Die gewählte Tonspur steht für diese MP4-Datei nicht zur Verfügung.");
        var parent = Path.GetDirectoryName(output)!;
        if (!Directory.Exists(parent)) throw new DirectoryNotFoundException("Der Ausgabeordner fehlt.");
        var name = ".can-audio-" + Guid.NewGuid().ToString("N");
        var work = Path.Combine(parent, name);
        Directory.CreateDirectory(work);
        try
        {
            var notice = selected.Notice(media);
            progress?.Report(new(CutProgressKind.Output, notice));
            progress?.Report(new(CutProgressKind.Output, "Die gesamte geladene Datei wird kopiert; die Schnittliste wird nicht angewendet."));
            var prepared = await prepare(source, media, selected.Stream, work, progress, token).ConfigureAwait(false);
            if (!string.Equals(Path.GetDirectoryName(Path.GetFullPath(prepared)), work, StringComparison.OrdinalIgnoreCase) ||
                new FileInfo(prepared).LinkTarget is not null || !File.Exists(prepared) || new FileInfo(prepared).Length == 0)
                throw new IOException("Die geprüfte Tonspurkopie fehlt im privaten Arbeitsbereich.");
            token.ThrowIfCancellationRequested();
            File.Move(prepared, output, overwrite: false);
            progress?.Report(new(CutProgressKind.Status, "Tonspurkopie vollständig geprüft und gespeichert."));
            progress?.Report(new(CutProgressKind.Output, output));
            return new(output, notice);
        }
        finally
        {
            try
            {
                var resolved = Path.GetFullPath(work);
                if (!string.Equals(Path.GetDirectoryName(resolved), parent, StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileName(resolved) != name || new DirectoryInfo(resolved).LinkTarget is not null)
                    throw new IOException("Der Arbeitsbereich hat sich verändert.");
                Directory.Delete(resolved, recursive: true);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                progress?.Report(new(CutProgressKind.Output, $"Arbeitsbereich konnte nicht gelöscht werden: {error.Message}"));
            }
        }
    }
}
