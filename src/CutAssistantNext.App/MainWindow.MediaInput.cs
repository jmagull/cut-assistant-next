using System.IO;
using System.Windows;
using System.Windows.Media;
using CutAssistantNext.App.Controls;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private VideoFileDropTarget? _videoDropTarget;
    private readonly CancellationTokenSource _mediaInputCancellation = new();

    private bool CanOpenMedia => !_isClosed && !_shutdownStarted && !_mediaLoadInProgress &&
        !_cutlistSearchInProgress && !_cutlistUploadInProgress && !_frameLoupeOpening &&
        !_cutOutputOpening && !_playerFrameSearch.IsNavigating && !_keyframeNavigation.IsNavigating && IsEnabled;

    private void InitializeMediaInput(string[] arguments, string startupDirectory)
    {
        _videoDropTarget = new VideoFileDropTarget(() => CanOpenMedia,
            async path => await OpenMediaFileAsync(path),
            valid => VideoDropBorder.BorderBrush = valid ? Brushes.DodgerBlue : new SolidColorBrush(Color.FromRgb(55, 65, 81)));
        VideoHost.VideoWindowHandleDestroying += (_, _) => _videoDropTarget?.Dispose();
        Loaded += async (_, _) =>
        {
            RegisterVideoDropTarget();
            try
            {
                var path = await MediaOpenRequest.FromStartupAsync(arguments, startupDirectory, _mediaInputCancellation.Token);
                if (path is not null && !_isClosed) await OpenMediaFileAsync(path);
            }
            catch (OperationCanceledException) { }
            catch (Exception error) { ShowMediaInputError(error); }
        };
    }

    private void RegisterVideoDropTarget()
    {
        if (!VideoHost.IsVideoWindowReady || _videoDropTarget is null) return;
        try { _videoDropTarget.Register(VideoHost.VideoWindowHandle); }
        catch (Exception error)
        {
            _logger.Error("Drag & Drop im nativen Videofenster konnte nicht aktiviert werden.", error);
            ShowMediaInputError(new InvalidOperationException("Drag & Drop im Videobild konnte nicht aktiviert werden. Bitte die Dateiauswahl verwenden.", error));
        }
    }

    private void VideoArea_DragOver(object sender, DragEventArgs e)
    {
        _videoDropTarget?.SetFiles(e.Data.GetDataPresent(DataFormats.FileDrop)
            ? e.Data.GetData(DataFormats.FileDrop) as string[] ?? [] : []);
        e.Effects = _videoDropTarget?.CanDrop == true && (e.AllowedEffects & DragDropEffects.Copy) != 0
            ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void VideoArea_DragLeave(object sender, DragEventArgs e) => _videoDropTarget?.Leave();

    private void VideoArea_Drop(object sender, DragEventArgs e)
    {
        VideoArea_DragOver(sender, e);
        e.Effects = e.Effects == DragDropEffects.Copy && _videoDropTarget?.AcceptDrop() == true
            ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    internal async Task OpenMediaFileAsync(string path)
    {
        if (!CanOpenMedia) return;
        _mediaLoadInProgress = true;
        try
        {
            var fullPath = await MediaOpenRequest.ResolveAsync(path, Environment.CurrentDirectory,
                cancellationToken: _mediaInputCancellation.Token);
            if (_isClosed || !ConfirmMediaReplacement()) return;
            await LoadMediaFileCoreAsync(fullPath);
        }
        catch (OperationCanceledException) when (_mediaInputCancellation.IsCancellationRequested) { }
        catch (Exception error) { ShowMediaInputError(error); }
        finally { _mediaLoadInProgress = false; }
    }

    private bool ConfirmMediaReplacement() => !MediaOpenRequest.NeedsReplacementConfirmation(
        _cutPlanViewModel.RemoveSegments.Count, _cutPlanViewModel.PendingStart.HasValue, _cutNamingState is not null) ||
        MessageBox.Show(this,
            "Eine andere Videodatei laden und die aktuelle Schnittarbeit verwerfen? Bitte bei Bedarf vorher die Cutlist speichern.",
            "Videodatei wechseln", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;

    private void ShowMediaInputError(Exception error)
    {
        if (!_isClosed) MessageBox.Show(this, error.Message, "Videodatei konnte nicht geöffnet werden",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
