using System.Globalization;
using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed record OtrCanToolPaths(string Engine, string Indexer, string Ffmpeg, string Ffprobe);

/// <summary>CAN owns indexing, work files, validation and final publication.</summary>
internal sealed class OtrCanCutService : ICutEngine
{
    private readonly OtrCanToolPaths _tools;
    private readonly ICutToolRunner _runner;
    private readonly Func<string, CancellationToken, Task<MediaAnalysisResult>> _analyze;
    private readonly Func<CancellationToken, Task> _checkTools;

    internal OtrCanCutService(OtrCanToolPaths tools, ICutToolRunner? runner = null,
        Func<string, CancellationToken, Task<MediaAnalysisResult>>? analyze = null,
        Func<CancellationToken, Task>? checkTools = null)
    {
        _tools = tools;
        _runner = runner ?? new CutToolRunner();
        _analyze = analyze ?? new FfprobeRunner(tools.Ffprobe).RunAsync;
        _checkTools = checkTools ?? (_ => Task.CompletedTask);
    }

    public Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default) =>
        Task.Run(() => RunCoreAsync(request, progress, cancellationToken), cancellationToken);

    private async Task RunCoreAsync(CutRequest request, IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);
        token.ThrowIfCancellationRequested();
        foreach (var tool in new[] { _tools.Engine, _tools.Indexer, _tools.Ffmpeg, _tools.Ffprobe })
        {
            if (!Path.IsPathFullyQualified(tool) || !File.Exists(tool))
                throw new FileNotFoundException("Bitte die OTR-CAN- und FFmpeg-Werkzeugpfade prüfen.", tool);
        }
        if (!File.Exists(request.OriginalFilePath)) throw new FileNotFoundException("Die Originaldatei fehlt.", request.OriginalFilePath);
        if (!Path.GetExtension(request.OutputFilePath).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("OTR-CAN erzeugt in CAN eine MP4-Ausgabe.");
        var parent = Path.GetDirectoryName(request.OutputFilePath)!;
        if (!Directory.Exists(parent)) throw new DirectoryNotFoundException("Der Ausgabeordner fehlt.");
        var existed = File.Exists(request.OutputFilePath);
        if (existed && !request.OverwriteExistingOutput) throw new IOException("Die Ausgabedatei existiert bereits.");
        Report(CutProgressKind.Status, "OTR-CAN-Werkzeuge werden geprüft …");
        await _checkTools(token).ConfigureAwait(false);
        var original = await _analyze(request.OriginalFilePath, token).ConfigureAwait(false);
        if (original.VideoStreams.Count != 1 || original.VideoStreams[0].Index != 0)
            throw new InvalidOperationException("OTR-CAN unterstützt derzeit genau eine Videospur an Streamposition 0.");
        if (original.Duration is { } duration && request.KeepSegments[^1].End > duration + TimeSpan.FromMilliseconds(100))
            throw new ArgumentException("Die Behaltebereiche reichen über die Videolaufzeit hinaus.");
        var cutlist = BuildCutlist(request);
        var id = Guid.NewGuid().ToString("N");
        var work = Path.Combine(parent, $".can-otr-{id}");
        var pending = Path.Combine(parent, $".can-otr-{id}-output.mp4");
        if (Directory.Exists(work) || File.Exists(work) || File.Exists(pending))
            throw new IOException("Der private Arbeitsbereich existiert bereits.");
        // Work and result are siblings on the same volume; neither is the user's target.
        Directory.CreateDirectory(work);
        try
        {
            var index = Path.Combine(work, "source.ffindex");
            Report(CutProgressKind.Status, "FFMS2 indexiert die Originaldatei einmal …");
            await _runner.RunAsync(_tools.Indexer, ["-c", "-k", request.OriginalFilePath, index],
                new OtrCanProgressReporter(progress, request.KeepSegments.Count, indexing: true), token).ConfigureAwait(false);
            foreach (var path in new[] { index, index + "_track00.tc.txt", index + "_track00.kf.txt" })
                if (!File.Exists(path) || new FileInfo(path).Length == 0)
                    throw new InvalidOperationException("FFMS2 hat keinen vollständigen Videoindex erzeugt.");
            Report(CutProgressKind.Status, "OTR-CAN schneidet mit CPU-Encoding …");
            await _runner.RunAsync(_tools.Engine,
                ["-vv", "cut-can", "--input", request.OriginalFilePath, "--output", pending,
                    "--index", index, "--temp-dir", work, "--cutlist", cutlist,
                    "--ffmpeg", _tools.Ffmpeg, "--ffprobe", _tools.Ffprobe],
                new OtrCanProgressReporter(progress, request.KeepSegments.Count, indexing: false), token).ConfigureAwait(false);
            if (!File.Exists(pending) || new FileInfo(pending).Length == 0)
                throw new InvalidOperationException("OTR-CAN hat keine Ausgabedatei erzeugt.");
            Report(CutProgressKind.Status, "MP4-Ausgabe und Tonspuren werden geprüft …");
            ValidateOutput(original, await _analyze(pending, token).ConfigureAwait(false));
            token.ThrowIfCancellationRequested();
            File.Move(pending, request.OutputFilePath, overwrite: existed && request.OverwriteExistingOutput);
            Report(CutProgressKind.Status, "OTR-CAN-Schnitt abgeschlossen.");
        }
        finally
        {
            Clean(() => File.Delete(pending), pending);
            Clean(() =>
            {
                var resolved = Path.GetFullPath(work);
                if (!string.Equals(Path.GetDirectoryName(resolved), parent, StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileName(resolved) != $".can-otr-{id}" || new DirectoryInfo(resolved).LinkTarget is not null)
                    throw new IOException("Der Arbeitsbereich hat sich verändert; bitte manuell prüfen.");
                Directory.Delete(resolved, recursive: true);
            }, work);
        }

        void Report(CutProgressKind kind, string message) => progress?.Report(new(kind, message));
        void Clean(Action remove, string path)
        {
            try { remove(); }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                Report(CutProgressKind.Output, $"Arbeitsdateien konnten nicht vollständig gelöscht werden: {path} ({error.Message})");
            }
        }
    }

    internal static string BuildCutlist(CutRequest request)
    {
        // The native time syntax is limited to less than 24 hours. Keep duration unchanged.
        if (request.KeepSegments.Any(s => s.End >= TimeSpan.FromDays(1)))
            throw new ArgumentException("OTR-CAN unterstützt derzeit Schnittzeiten unter 24 Stunden.");
        return "times:" + string.Concat(request.KeepSegments.Select(s =>
            $"[{Format(s.Start)},{Format(s.End)}]"));
        static string Format(TimeSpan time) => time.ToString(@"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
    }

    internal static void ValidateOutput(MediaAnalysisResult original, MediaAnalysisResult output)
    {
        if (!VideoPreparation.IsMp4(output) || output.Duration is not { } duration || duration <= TimeSpan.Zero ||
            output.VideoStreams.Count != original.VideoStreams.Count || output.AudioStreams.Count != original.AudioStreams.Count)
            throw new InvalidOperationException("Die MP4-Ausgabe ist unvollständig oder ihre Ton-/Videospuren fehlen.");
        foreach (var pair in original.VideoStreams.Zip(output.VideoStreams))
            if (pair.First.CodecName != pair.Second.CodecName || pair.First.Width != pair.Second.Width || pair.First.Height != pair.Second.Height)
                throw new InvalidOperationException("Videocodec oder Auflösung der Ausgabe weichen vom Original ab.");
        foreach (var pair in original.AudioStreams.Zip(output.AudioStreams))
            if (pair.First.SampleRate != pair.Second.SampleRate || pair.First.Channels != pair.Second.Channels)
                throw new InvalidOperationException("Die Audiostruktur der Ausgabe weicht vom Original ab.");
    }
}
