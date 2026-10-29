using System.Windows;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Joining;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private bool _joinInProgress;

    private void JoinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (!CanOpenMedia) return;
        _joinInProgress = true;
        try
        {
            var settings = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var resolver = new ToolPathResolver();
            var ffmpeg = resolver.Resolve(BundledToolKind.Ffmpeg, settings.FfmpegExecutablePath);
            var ffprobe = resolver.Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath);
            var selection = new JoinDialog(ffprobe) { Owner = this };
            if (selection.ShowDialog() != true || selection.Request is null && selection.QuickRequest is null) return;
            var request = selection.Request;
            var quickRequest = selection.QuickRequest;
            var viewModel = new Mp4BoxProgressViewModel();
            using var cancel = new CancellationTokenSource();
            var progress = new BufferedCutProgressReporter(viewModel.ApplyProgress);
            var dialog = new Mp4BoxProgressDialog(viewModel, progress, operationTitle: "Filmteile zusammenfügen") { Owner = this };
            dialog.CancelRequested += (_, _) => cancel.Cancel();
            CutResult? result = null;
            var started = false;
            dialog.Loaded += async (_, _) =>
            {
                if (started) return;
                started = true; viewModel.MarkRunning();
                try
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    if (quickRequest is not null)
                        result = await Task.Run(() => new QuickCopyJoinService(ffmpeg, ffprobe).RunAsync(quickRequest, progress, cancel.Token));
                    else
                    {
                        result = await Task.Run(() => new Mp4JoinService(ffmpeg, ffprobe).RunAsync(request!, progress, cancel.Token));
                        watch.Stop();
                        var total = $"Gesamtzeit AUDIO-ENTHUSIAST: {watch.Elapsed:hh\\:mm\\:ss\\.ff}.";
                        progress.Report(new(CutProgressKind.Status, total));
                        result = new(result.OutputFilePath, result.Timeline, result.TimingKind,
                            "AUDIO-ENTHUSIAST – NEUKODIERUNG. " + result.Notice + "\n" + total);
                    }
                    progress.Flush(); viewModel.MarkSucceeded();
                }
                catch (OperationCanceledException)
                { progress.Flush(); viewModel.MarkCancelled(); }
                catch (Exception error)
                { progress.Flush(); viewModel.MarkFailed(error.Message); _logger.Error("Zusammenfügen fehlgeschlagen.", error); }
                finally { dialog.MarkOperationCompleted(startAutoClose: false); }
            };
            dialog.ShowDialog();
            if (result is not null)
            {
                var completion = new CutCompletionViewModel(result, "Zusammenfügen abgeschlossen");
                new CutCompletionDialog(completion, viewModel.ProtocolText, reviewAsync: _cutReviewController.ReviewAsync) { Owner = this }.ShowDialog();
            }
        }
        catch (Exception error)
        {
            _logger.Error("Klebecenter konnte nicht gestartet werden.", error);
            MessageBox.Show(this, error.Message, "Klebecenter", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { _joinInProgress = false; }
    }
}
