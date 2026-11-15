using System.IO;
using System.Windows;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private int _recommendationRevision;

    private void InitializeCutEngineRecommendation()
    {
        ((System.Collections.Specialized.INotifyCollectionChanged)_cutPlanViewModel.MarkedSegments).CollectionChanged += (_, _) => RefreshCutEngineRecommendation();
        _cutPlanViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(CutPlanViewModel.PendingStart) or nameof(CutPlanViewModel.MediaDuration))
                RefreshCutEngineRecommendation();
        };
        _keyframeNavigation.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(KeyframeNavigationViewModel.Index)) RefreshCutEngineRecommendation();
        };
        Activated += (_, _) => RefreshCutEngineRecommendation();
    }

    private async void RefreshCutEngineRecommendation()
    {
        var revision = ++_recommendationRevision;
        CutEngineRecommendationText.Visibility = Visibility.Collapsed;
        if (_isClosed || _shutdownStarted || _viewModel.AnalysisResult is not { } media ||
            _cutPlanViewModel.MediaDuration is null || _cutPlanViewModel.MarkedSegments.Count == 0 ||
            _cutPlanViewModel.PendingStart is not null) return;
        try
        {
            await Task.Delay(130);
            if (revision != _recommendationRevision || _isClosed || _shutdownStarted) return;
            var plan = _cutPlanViewModel.CreateCutPlanSnapshot();
            var keyframes = _keyframeNavigation.Index;
            var (mp4Box, otrCan) = await Task.Run(async () =>
            {
                var resolver = new ToolPathResolver();
                var cut = new CutApplicationSettingsStore().Load();
                var ffmpeg = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
                var native = AudioDevelopmentEngine.Resolve(new OtrCanSettingsStore().Load(), AppContext.BaseDirectory);
                var box = resolver.TryResolve(BundledToolKind.Mp4Box, cut?.ExecutablePath);
                var ff = resolver.TryResolve(BundledToolKind.Ffmpeg, ffmpeg.FfmpegExecutablePath);
                var probe = resolver.TryResolve(BundledToolKind.Ffprobe, ffmpeg.FfprobeExecutablePath);
                var checks = new ToolFunctionChecks();
                var token = _mediaInputCancellation.Token;
                bool canMp4Box = box is not null && File.Exists(box) && (probe is null ||
                    await checks.GetKnownResultAsync(ToolFunction.Mp4BoxSplitConcat, [box, probe], token) is not { Compatible: false });
                bool canOtr = ff is not null && probe is not null && File.Exists(ff) && File.Exists(probe) &&
                    File.Exists(native.ExecutablePath) && File.Exists(native.FfmsIndexExecutablePath) &&
                    await checks.GetKnownResultAsync(ToolFunction.FfmpegStreamcopy, [ff, probe], token) is not { Compatible: false };
                return (canMp4Box, canOtr);
            });
            if (revision != _recommendationRevision || _isClosed || _shutdownStarted ||
                !ReferenceEquals(media, _viewModel.AnalysisResult)) return;
            CutEngineRecommendationText.Text = CutEngineRecommendation.Evaluate(media, plan, keyframes, mp4Box, otrCan);
            CutEngineRecommendationText.Visibility = Visibility.Visible;
        }
        catch (OperationCanceledException) when (_isClosed || _shutdownStarted) { }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException)
        {
            _logger.Warning($"Schnittmotor-Empfehlung konnte nicht ermittelt werden: {error.Message}");
        }
    }
}
