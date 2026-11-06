using System.Globalization;
using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Services.Cutting;

internal interface IAudioTrackRecovery
{
    Task<AudioTrackRecoveryOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
        IProgress<CutProgressUpdate>? progress, CancellationToken token);
}

internal sealed record AudioTrackRecoveryOffer(string Description,
    Func<CancellationToken, Task<CutResult>>? Restart = null, IDisposable? SourceLease = null) : IDisposable
{
    internal bool IsAvailable => Restart is not null;
    public void Dispose() => SourceLease?.Dispose();
}

/// <summary>Proves one faulty original stream and every retained stream before offering a new cut.</summary>
internal sealed class OtrCanAudioTrackRecovery(
    OtrCanToolPaths tools, Func<IReadOnlyList<int>, ICutEngine> createEngine,
    ICutToolRunner? runner = null, Func<string, CancellationToken, Task<MediaAnalysisResult>>? analyze = null) : IAudioTrackRecovery
{
    private readonly ICutToolRunner _runner = runner ?? new CutToolRunner();
    private readonly Func<string, CancellationToken, Task<MediaAnalysisResult>> _analyze =
        analyze ?? new FfprobeRunner(tools.Ffprobe).RunAsync;
    private readonly SelectedAudioSource _preparation = new(tools.Ffmpeg, tools.Ffprobe);

    public async Task<AudioTrackRecoveryOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
        IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (failure.SourceAudioTrackCount < 2)
            return new("Die Aufnahme enthält nur eine Tonspur.");
        if (!string.Equals(request.OriginalFilePath, request.SourceFilePath, StringComparison.OrdinalIgnoreCase))
            return new("Die Spur-Abwahl benötigt die unveränderte MP4-Originalquelle.");

        FileStream? sourceLease = null;
        try
        {
            sourceLease = new FileStream(request.SourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var media = await _analyze(request.SourceFilePath, token).ConfigureAwait(false);
            if (!VideoPreparation.IsMp4(media) || media.VideoStreams.Count != 1 || media.VideoStreams[0].Index != 0 ||
                media.StartTimeSeconds != 0 || media.VideoStreams[0].StartTimeSeconds != 0 ||
                media.AudioStreams.Count != failure.SourceAudioTrackCount ||
                media.AudioStreams.Select(s => s.Index).Distinct().Count() != media.AudioStreams.Count)
                return new("Die Tonspuren konnten der MP4-Originalquelle nicht eindeutig zugeordnet werden.");

            // The normal motor already verified CFR. A 40 ms boundary is exact on both supported grids.
            if (request.KeepSegments.Any(k => k.Start.Ticks % 400000 != 0 || k.End.Ticks % 400000 != 0))
                return new("Die Behaltebereiche liegen nicht auf dem für die Spur-Abwahl geprüften Zeitraster von 40 ms.");

            var checks = new List<(AudioStreamInfo Track, AudioTrackCheck Check)>();
            foreach (var track in media.AudioStreams)
            {
                progress?.Report(new(CutProgressKind.Status, $"Tonspur {track.Index} wird auf Decoderfehler und Zeitabdeckung geprüft …"));
                var check = await CheckAsync(request, track, token).ConfigureAwait(false);
                checks.Add((track, check));
                progress?.Report(new(CutProgressKind.Output, $"Tonspur {track.Index}: {check.Description}"));
            }
            var damaged = checks.Where(item => item.Check.State == AudioTrackState.Damaged).ToArray();
            var unproven = checks.Where(item => item.Check.State == AudioTrackState.Unproven).ToArray();
            if (damaged.Length == 0)
                return new("Der Fehler konnte keiner Tonspur eindeutig zugeordnet werden.");
            if (damaged.Length != 1 || unproven.Length != 0)
                return new("Für den Ausschluss genau einer beschädigten Spur müssen alle übrigen Tonspuren verwendbar sein. " +
                    string.Join(" ", checks.Select(item => $"Spur {item.Track.Index}: {item.Check.Description}")));

            var omitted = damaged[0].Track;
            var retained = checks.Where(item => item.Check.State == AudioTrackState.Usable).Select(item => item.Track).ToArray();
            var retainedIndices = retained.Select(track => track.Index).ToArray();
            var notice = $"Ausgeschlossen: {Describe(omitted)} ({damaged[0].Check.Description}).{Environment.NewLine}" +
                $"Erhalten: {string.Join("; ", retained.Select(Describe))}.";
            var offer = new AudioTrackRecoveryOffer(notice + Environment.NewLine +
                "CAN startet den vollständigen otr-can-Schnitt mit den geprüften Resttonspuren neu.", restartToken =>
                OtrCanAacEmergencyCutService.RetryWithPreparedSourceAsync(request, notice,
                    work => _preparation.PrepareAsync(request.SourceFilePath, media, retained, work, "selected-audio.mp4",
                        progress, restartToken),
                    () => createEngine(retainedIndices), progress, restartToken), sourceLease);
            sourceLease = null;
            return offer;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            return new("Die Vorprüfung der Tonspuren ist fehlgeschlagen. " + error.Message);
        }
        finally { sourceLease?.Dispose(); }
    }

    private async Task<AudioTrackCheck> CheckAsync(CutRequest request, AudioStreamInfo track, CancellationToken token)
    {
        if (track.SampleRate is not (32000 or 44100 or 48000) || track.Channels is not (1 or 2) ||
            track.ChannelLayout is not ("mono" or "stereo") || track.CodecName is not ("aac" or "ac3") ||
            track.Index <= 0)
            return new(AudioTrackState.Unproven, "Tonformat liegt außerhalb der geprüften AAC-/AC-3-Spurauswahl");

        var rate = track.SampleRate.Value;
        var ranges = request.KeepSegments.Select(k => (Start: ToSamples(k.Start), End: ToSamples(k.End))).ToArray();
        long ToSamples(TimeSpan time) => (long)((decimal)time.Ticks * rate / TimeSpan.TicksPerSecond);
        var coverage = new AudioTrackCoverageProbe(track, ranges);
        try
        {
            await _runner.RunAsync(tools.Ffprobe,
                ["-v", "level+warning", "-select_streams", track.Index.ToString(CultureInfo.InvariantCulture),
                 "-show_frames", "-show_entries",
                 "frame=stream_index,pts,nb_samples,channels,channel_layout:stream=index,codec_name,profile,sample_rate,channels,channel_layout,time_base,bit_rate,start_pts,duration_ts",
                 "-of", "compact", request.SourceFilePath], coverage, token).ConfigureAwait(false);
            return coverage.Result();
        }
        catch (InvalidOperationException error)
        {
            return new(AudioTrackState.Unproven, $"ffprobe konnte die Spur nicht vollständig prüfen: {error.Message}");
        }
    }

    private static string Describe(AudioStreamInfo track) =>
        $"Spur {track.Index} ({track.CodecName?.ToUpperInvariant()}, {track.Language ?? "Sprache unbekannt"}, " +
        $"{track.Channels} Kanäle, {track.SampleRate} Hz" +
        (string.IsNullOrWhiteSpace(track.Title) ? ")" : $", {track.Title})");
}
