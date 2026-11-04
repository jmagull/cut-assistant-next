using System.IO;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using SpotlightCan.Core;
using CanPlan = CutAssistantNext.Core.Editing.CutPlan;
using CanSegment = CutAssistantNext.Core.Editing.RemoveSegment;
using SpotlightSegment = SpotlightCan.Core.RemoveSegment;

namespace CutAssistantNext.App.Services;

internal sealed class SpotlightMediaSession : IDisposable
{
    private readonly FileStream _sourceLease;
    private bool _disposed;
    internal CutlistMediaContext Context { get; }
    internal MediaInfo Media { get; }
    private SpotlightMediaSession(CutlistMediaContext context, FileStream sourceLease, MediaInfo media)
    { Context = context; _sourceLease = sourceLease; Media = media; }

    internal static Task<SpotlightMediaSession> CaptureAsync(CutlistMediaContext context, CancellationToken token = default) => Task.Run(() =>
    {
        var analysis = context.Analysis;
        if (analysis.Duration is not { } duration || duration <= TimeSpan.Zero || analysis.VideoStreams.FirstOrDefault() is not { Width: > 0, Height: > 0 } video)
            throw new InvalidOperationException("Bitte zuerst eine Videodatei in CAN öffnen und erfolgreich analysieren.");
        token.ThrowIfCancellationRequested();
        var lease = new FileStream(context.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            if (analysis.FileSizeBytes is { } expected && expected != lease.Length)
                throw new IOException("Die Videodatei wurde seit der CAN-Analyse verändert. Bitte den Film erneut in CAN öffnen.");
            var fps = video.FramesPerSecond.GetValueOrDefault();
            var rate = double.IsFinite(fps) && fps > 0 && fps < 10000 ? new FrameRate((long)Math.Round(fps * 1_000_000), 1_000_000) : default;
            var media = new MediaInfo(Path.GetFullPath(context.FilePath), duration, rate, video.CodecName ?? "unbekannt", video.Width.Value, video.Height.Value, lease.Length, video.Index);
            token.ThrowIfCancellationRequested();
            return new SpotlightMediaSession(context, lease, media);
        }
        catch { lease.Dispose(); throw; }
    }, token);

    internal static async Task<(string Ffmpeg, string Ffprobe)> ResolveToolsAsync(FfmpegSettings settings, FfmpegToolChecker? checker = null, ToolPathResolver? resolver = null, CancellationToken token = default)
    {
        var results = await (checker ?? new FfmpegToolChecker()).CheckAsync(settings, token);
        var unavailable = results.Where(result => !result.IsAvailable).ToArray();
        if (unavailable.Length != 0)
            throw new InvalidOperationException("Bitte Einstellungen → FFmpeg-Werkzeuge prüfen.\n" + string.Join("\n", unavailable.Select(result => result.Name + ": " + result.Message)));
        resolver ??= new ToolPathResolver();
        return (resolver.Resolve(BundledToolKind.Ffmpeg, settings.FfmpegExecutablePath), resolver.Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath));
    }

    internal bool TryApply(IReadOnlyList<SpotlightSegment> segments, CutPlanViewModel editor, Func<bool> isCurrent, Func<bool> confirmReplacement, bool hasProtectedMetadata = false)
    {
        void RequireCurrent()
        {
            if (_disposed || !isCurrent() || editor.MediaDuration != Media.Duration)
                throw new InvalidOperationException("Der in CAN geladene Film hat sich geändert. Spotlight-CAN bitte erneut öffnen.");
        }
        RequireCurrent();
        if (segments.Count == 0) throw new InvalidOperationException("Keine Schnittbereiche zur Übernahme vorhanden.");
        var replacement = new CanPlan(Media.Duration);
        foreach (var segment in segments.OrderBy(segment => segment.Start)) replacement.Add(new CanSegment(segment.Start, segment.End));
        var before = editor.MarkedSegments.Select(segment => (segment.Start, segment.End)).ToArray();
        var pending = editor.PendingStart;
        var mode = editor.EditingMode;
        if ((before.Length > 0 || pending.HasValue || hasProtectedMetadata) && !confirmReplacement()) return false;
        RequireCurrent();
        if (pending != editor.PendingStart || mode != editor.EditingMode || !before.SequenceEqual(editor.MarkedSegments.Select(segment => (segment.Start, segment.End))))
            throw new InvalidOperationException("Die Schnittarbeit wurde während der Bestätigung geändert. Bitte die Übernahme erneut prüfen.");
        // LoadCutPlan validates/converts the complete replacement before mutating the existing editor.
        editor.LoadCutPlan(replacement);
        return true;
    }
    public void Dispose() { _disposed = true; _sourceLease.Dispose(); }
}
