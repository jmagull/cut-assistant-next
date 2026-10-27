using System.IO;
using System.Windows;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using Microsoft.Win32;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private bool _audioTrackCopyInProgress;

    private void AudioTrackCopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (!CanOpenMedia || _audioTrackCopyInProgress) return;
        var media = _viewModel.AnalysisResult;
        var source = _viewModel.FilePath;
        if (media is null || string.IsNullOrWhiteSpace(source))
        {
            MessageBox.Show(this, "Bitte lade zuerst den bereits geschnittenen Filmteil.",
                "Tonspur für Filmteile", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        if (AudioTrackCopyChoice.Choices(media).Count == 0)
        {
            MessageBox.Show(this,
                "Dafür wird eine MP4-Datei mit einer Videospur und mindestens zwei Tonspuren benötigt. " +
                "AAC- und AC-3-Spuren können ausgewählt werden.",
                "Tonspur für Filmteile", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _audioTrackCopyInProgress = true;
        try
        {
            var selection = new AudioTrackCopyDialog(source, media) { Owner = this };
            if (selection.ShowDialog() != true || selection.SelectedChoice is not { } choice) return;
            var save = new SaveFileDialog
            {
                Title = "Zusätzliche Tonspurkopie speichern",
                Filter = "MP4-Datei (*.mp4)|*.mp4",
                DefaultExt = ".mp4",
                AddExtension = true,
                InitialDirectory = Path.GetDirectoryName(source),
                FileName = Path.GetFileNameWithoutExtension(source) + " - " + choice.Stream.CodecName!.ToUpperInvariant() + ".mp4",
                OverwritePrompt = false,
                CheckPathExists = true
            };
            save.FileOk += (_, args) =>
            {
                if (!File.Exists(save.FileName)) return;
                args.Cancel = true;
                MessageBox.Show(this, "Bitte wähle einen neuen Dateinamen. Vorhandene Dateien bleiben erhalten.",
                    "Zusätzliche Tonspurkopie", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            if (save.ShowDialog(this) != true) return;
            var settings = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var resolver = new ToolPathResolver();
            var service = new AudioTrackCopyService(
                resolver.Resolve(BundledToolKind.Ffmpeg, settings.FfmpegExecutablePath),
                resolver.Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath));
            var viewModel = new Mp4BoxProgressViewModel();
            using var cancellation = new CancellationTokenSource();
            var progress = new BufferedCutProgressReporter(viewModel.ApplyProgress);
            var dialog = new Mp4BoxProgressDialog(viewModel, progress, operationTitle: "Tonspurkopie erstellen") { Owner = this };
            dialog.CancelRequested += (_, _) => cancellation.Cancel();
            var started = false;
            AudioTrackCopyResult? result = null;
            dialog.Loaded += async (_, _) =>
            {
                if (started) return;
                started = true;
                viewModel.MarkRunning();
                try
                {
                    result = await service.RunAsync(source, save.FileName, media, choice, progress, cancellation.Token);
                    progress.Flush();
                    viewModel.MarkSucceeded();
                }
                catch (OperationCanceledException)
                {
                    progress.Flush();
                    viewModel.ApplyProgress(new(CutProgressKind.Status, "Abgebrochen. Die bisherige Datei bleibt erhalten."));
                    viewModel.MarkCancelled();
                }
                catch (Exception error)
                {
                    progress.Flush();
                    viewModel.MarkFailed(error.Message);
                    _logger.Error("Tonspurkopie konnte nicht erstellt werden.", error);
                }
                finally { dialog.MarkOperationCompleted(startAutoClose: false); }
            };
            dialog.ShowDialog();
            if (result is not null)
                MessageBox.Show(this, $"Zusätzliche MP4-Datei gespeichert:\n{result.OutputFilePath}\n\n{result.Notice}\n\n" +
                    "Bitte Bild, Ton, Anfang und Ende der neuen Datei kontrollieren.",
                    "Tonspurkopie erstellt", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception error)
        {
            _logger.Error("Tonspurauswahl konnte nicht geöffnet werden.", error);
            MessageBox.Show(this, error.Message, "Tonspur für Filmteile", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { _audioTrackCopyInProgress = false; }
    }
}
