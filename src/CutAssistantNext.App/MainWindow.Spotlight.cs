using System.IO;
using System.Windows;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using SpotlightCan.App;
using SpotlightWindow = SpotlightCan.App.MainWindow;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private bool _spotlightInProgress;
    private SpotlightWindow? _spotlightWindow;

    private async void SpotlightMenuItem_Click(object sender, RoutedEventArgs e) => await OpenSpotlightAsync();

    internal async Task OpenSpotlightAsync(Func<bool>? confirmReplacement = null, FfmpegSettings? toolSettings = null)
    {
        if (_spotlightInProgress) { _spotlightWindow?.Activate(); return; }
        if (!CanOpenMedia) return;
        if (_viewModel.AnalysisResult is not { Duration: { } duration, VideoStreams.Count: > 0 } analysis || duration <= TimeSpan.Zero)
        {
            MessageBox.Show(this, "Bitte zuerst eine Videodatei in CAN öffnen.", "Spotlight-CAN", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _spotlightInProgress = true;
        var context = new CutlistMediaContext(_viewModel.FilePath, analysis, _cutlistMediaGeneration);
        SpotlightMediaSession? session = null;
        try
        {
            var settings = toolSettings ?? await Task.Run(() => new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault(), _mediaInputCancellation.Token);
            var tools = await SpotlightMediaSession.ResolveToolsAsync(settings, token: _mediaInputCancellation.Token);
            if (!IsCurrentCutlistContext(context)) return;
            session = await SpotlightMediaSession.CaptureAsync(context, _mediaInputCancellation.Token);
            if (!IsCurrentCutlistContext(context)) return;
            var options = new SpotlightHostOptions(session.Media, tools.Ffmpeg, tools.Ffprobe, segments =>
            {
                var applied = session.TryApply(segments, _cutPlanViewModel, () => IsCurrentCutlistContext(context),
                    confirmReplacement ?? (() => MessageBox.Show(_spotlightWindow ?? (Window)this,
                        "Die aktuelle Schnittarbeit einschließlich eines offenen Anfangs durch die Spotlight-Bereiche ersetzen? Bitte bei Bedarf vorher die CAN-Cutlist speichern.",
                        "Schnittbereiche übernehmen", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes),
                    _cutNamingState is not null || _loadedCutlistAuthor is not null || _loadedIntendedCutApplicationName is not null);
                if (applied)
                {
                    _lastSavedCutlistFilePath = null; _loadedCutlistAuthor = null; _loadedIntendedCutApplicationName = null;
                    _lastSuccessfulCutMediaPath = null; _lastSuccessfulCutEngine = null; _lastCutAudioNotice = null;
                    _logger.Information($"Spotlight-CAN: {segments.Count} Entfernen-Bereiche direkt übernommen.");
                }
                return applied;
            });
            _spotlightWindow = new SpotlightWindow(options) { Owner = this };
            await _spotlightWindow.InitializeHostAsync();
            if (!_spotlightWindow.Model.IsReady) throw new InvalidOperationException(_spotlightWindow.Model.Status);
            if (IsCurrentCutlistContext(context)) _spotlightWindow.ShowDialog();
        }
        catch (OperationCanceledException) when (_mediaInputCancellation.IsCancellationRequested) { }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException)
        {
            _logger.Error("Spotlight-CAN konnte nicht gestartet werden.", error);
            if (!_isClosed) MessageBox.Show(this, error.Message, "Spotlight-CAN", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        finally
        {
            try { await CloseSpotlightSafelyAsync(); }
            finally { session?.Dispose(); _spotlightWindow = null; _spotlightInProgress = false; }
        }
    }
    private async Task CloseSpotlightSafelyAsync()
    {
        if (_spotlightWindow is not { } window) return;
        try { await window.CloseFromHostAsync(); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            _logger.Error("Spotlight-Arbeitsdateien konnten nicht vollständig bereinigt werden.", error);
            if (!_isClosed) MessageBox.Show(this, error.Message, "Spotlight-CAN bereinigen", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
