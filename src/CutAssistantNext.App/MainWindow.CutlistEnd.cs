using System.IO;
using System.Windows;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private async Task<VideoFrameWindow?> ReadCutlistTailAsync(string filePath, MediaAnalysisResult? analysis, TimeSpan duration)
    {
        if (analysis?.VideoStreams.FirstOrDefault() is not { } video ||
            analysis.StartTimeSeconds is not { } origin || !double.IsFinite(origin))
            return null;

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(_mediaInputCancellation.Token);
        timeout.CancelAfter(TimeSpan.FromSeconds(5));
        try
        {
            var start = TimeSpan.FromSeconds(Math.Max(0, duration.TotalSeconds + origin - 3));
            var settings = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var ffprobe = new ToolPathResolver().Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath);
            return await new FfprobeFrameRunner(ffprobe).RunTailAsync(filePath, video.Index, start, timeout.Token);
        }
        catch (Exception error) when (!_mediaInputCancellation.IsCancellationRequested &&
            error is System.IO.IOException or UnauthorizedAccessException or InvalidOperationException or
            ArgumentException or OverflowException or System.ComponentModel.Win32Exception or
            System.Text.Json.JsonException or FormatException or OperationCanceledException)
        {
            _logger.Warning($"Bildzeiten am Videoende konnten nicht geprüft werden: {error.Message}");
            return null;
        }
    }

    private async Task<(CutPlan? Plan, Mp4BoxSplitRange? Range)> ConfirmMp4BoxTailAsync(Window owner, string filePath,
        MediaAnalysisResult analysis, CutPlan plan, double fps)
    {
        var keeps = CutlistKeepSegmentBuilder.Build(plan);
        if (keeps.Count == 0) return (plan, null);

        var videoEnd = CutlistImportReview.GetVideoEnd(analysis, plan.MediaDuration);
        if (!CutlistImportReview.NeedsTailProbe(keeps, videoEnd)) return (plan, null);

        var metadata = new CutlistGeneralMetadata { NoOfCuts = keeps.Count };
        var document = new CutlistDocument(metadata, keeps, new CutlistInfoMetadata());
        var tail = await ReadCutlistTailAsync(filePath, analysis, videoEnd);
        if (!owner.IsVisible) return (null, null);

        var review = CutlistImportReview.Analyze(document, plan.MediaDuration, analysis, tail);
        if (review.Fragment is not { } fragment) return (plan, null);

        var range = review.BuildMp4BoxRange(fps);
        var tools = new CutApplicationSettingsStore().Load() ?? CutApplicationSettings.CreateDefault();
        var executable = new ToolPathResolver().Resolve(BundledToolKind.Mp4Box, tools.ExecutablePath);
        var temporary = Path.Combine(Path.GetTempPath(), $"can-{Guid.NewGuid():N}.mp4");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(_mediaInputCancellation.Token);
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        EventHandler onClosed = (_, _) => timeout.Cancel();
        owner.Closed += onClosed;
        try
        {
            await new Mp4BoxRunner(executable, _logger).RunSplitAsync(filePath, temporary, range, timeout.Token);
            return owner.IsVisible ? (plan, range) : (null, null);
        }
        catch (Mp4BoxSegmentException error)
        {
            if (!owner.IsVisible) return (null, null);
            var message = $"{DescribeFragment(review)}\n\nMP4Box-Segment {keeps.Count} von {keeps.Count}: {error.Message}";
            if (keeps.Count == 1)
                throw new InvalidOperationException(message + "\n\nBitte passe den einzigen Behaltebereich vor dem Schneiden an.", error);

            var question = new CutlistEndFragmentDialog(message + "\n\n" +
                "Beim Entfernen bleibt das Ende des davorliegenden Bereichs erhalten.",
                allowUnchangedImport: false) { Owner = owner };
            if (question.ShowDialog() != true) return (null, null);

            var corrected = CutlistEndFragmentCorrector.BuildCutPlan(document, plan.MediaDuration, fragment);
            _cutPlanViewModel.LoadCutPlan(corrected);
            _lastSavedCutlistFilePath = null;
            return (corrected, null);
        }
        catch (OperationCanceledException) when (!owner.IsVisible || _mediaInputCancellation.IsCancellationRequested)
        {
            return (null, null);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Die MP4Box-Vorprüfung des letzten Behaltebereichs hat zu lange gedauert. " +
                "Bitte prüfe die Schnittgrenzen und starte den Auftrag erneut.");
        }
        finally
        {
            owner.Closed -= onClosed;
            try { File.Delete(temporary); }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                _logger.Warning($"Temporäre MP4Box-Prüfdatei konnte nicht gelöscht werden: {error.Message}");
            }
        }
    }

    private static string DescribeFragment(CutlistImportReview review)
    {
        var fragment = review.Fragment!;
        var frames = review.MeasuredFrameCount switch
        {
            1 => "Im Bereich liegt genau ein Bildanfang.",
            int count => $"Im Bereich liegen {count} Bildanfänge.",
            _ => "Der Bereich ist nach der verfügbaren Bildrate ungewöhnlich kurz."
        };
        var audio = review.AudioEndsBeforeFragment ? "\nAlle erkannten Tonspuren enden bereits davor." : "";
        return $"Letzter Behaltebereich: {fragment.Start:c} bis {fragment.End:c} " +
            $"({fragment.Duration.TotalSeconds:0.###} Sekunden).\n{frames}{audio}";
    }
}
