using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Dialogs;

internal partial class CutReviewWindow : Window
{
    private readonly CutReviewViewModel _viewModel;
    private readonly TaskCompletionSource<nint> _handle = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private Task? _closeTask;
    private bool _allowClose;
    private bool _sliderDragging;

    internal CutReviewWindow(IMediaPlayerService player)
    {
        _viewModel = new CutReviewViewModel(player);
        InitializeComponent();
        DataContext = _viewModel;
        VideoHost.VideoWindowHandleCreated += VideoHost_HandleCreated;
        Closing += Window_Closing;
        if (VideoHost.IsVideoWindowReady) VideoHost_HandleCreated(null, EventArgs.Empty);
    }

    internal bool IsClosing => _viewModel.IsClosing;
    internal Task ReviewAsync(CutCompletionViewModel completion, int index) => RunAsync(async () =>
    {
        var handle = await _handle.Task.WaitAsync(_viewModel.LifetimeToken);
        await _viewModel.ReviewAsync(completion, index, handle);
    });

    private void VideoHost_HandleCreated(object? sender, EventArgs e) =>
        _handle.TrySetResult(VideoHost.VideoWindowHandle);

    private async Task RunAsync(Func<Task> action)
    {
        if (IsClosing) return;
        ErrorText.Text = string.Empty;
        try { await action(); }
        catch (OperationCanceledException) when (IsClosing) { }
        catch (Exception exception)
        {
            if (!IsClosing) ErrorText.Text = $"Kontrolle nicht möglich: {exception.Message}";
        }
    }

    private async void ReviewAudio_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_viewModel.IsIdle && ReviewAudio.SelectedItem is PlayerAudioTrack track && track.Id != _viewModel.SelectedAudioTrack?.Id)
            await RunAsync(() => _viewModel.SelectAudioAsync(track));
    }

    private async void PreviousButton_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(() => _viewModel.MoveAsync(-1, VideoHost.VideoWindowHandle));
    private async void NextButton_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(() => _viewModel.MoveAsync(1, VideoHost.VideoWindowHandle));
    private async void PlayButton_Click(object sender, RoutedEventArgs e) =>
        await RunAsync(_viewModel.TogglePlaybackAsync);
    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private async void PositionSlider_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsClosing || !_viewModel.IsIdle || !_viewModel.Playback.CanSeek) return;
        if (PositionSlider.Template?.FindName("PART_Track", PositionSlider) is Track track &&
            track.Thumb is not null && e.OriginalSource is DependencyObject source &&
            !ReferenceEquals(source, track.Thumb) && !track.Thumb.IsAncestorOf(source))
        {
            e.Handled = true;
            var value = track.ValueFromPoint(e.GetPosition(track));
            if (double.IsFinite(value))
                await RunAsync(() => _viewModel.SeekAsync(TimeSpan.FromSeconds(Math.Clamp(value, 0, PositionSlider.Maximum))));
            return;
        }
        _sliderDragging = true;
        _viewModel.Playback.BeginSeek();
    }

    private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_sliderDragging) _viewModel.Playback.UpdateSeekPosition(e.NewValue);
    }

    private async void PositionSlider_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_sliderDragging) return;
        var position = _viewModel.Playback.TimelinePositionSeconds;
        _sliderDragging = false;
        await RunAsync(() => _viewModel.SeekAsync(TimeSpan.FromSeconds(position)));
    }

    private async void PositionSlider_LostMouseCapture(object sender, MouseEventArgs e)
    {
        // Thumb releases capture before mouse-up. Commit once rather than lose the preview.
        if (_sliderDragging)
        {
            var position = _viewModel.Playback.TimelinePositionSeconds;
            _sliderDragging = false;
            await RunAsync(() => _viewModel.SeekAsync(TimeSpan.FromSeconds(position)));
        }
    }

    private async void PositionSlider_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        double? seconds = e.Key switch
        {
            Key.Left or Key.Down => _viewModel.Playback.PositionSeconds - 1,
            Key.Right or Key.Up => _viewModel.Playback.PositionSeconds + 1,
            Key.PageDown => _viewModel.Playback.PositionSeconds - 10,
            Key.PageUp => _viewModel.Playback.PositionSeconds + 10,
            Key.Home => 0,
            Key.End => _viewModel.Playback.DurationSeconds,
            _ => null
        };
        if (!seconds.HasValue || !_viewModel.IsIdle || !_viewModel.Playback.CanSeek) return;
        e.Handled = true;
        await RunAsync(() => _viewModel.SeekAsync(TimeSpan.FromSeconds(
            Math.Clamp(seconds.Value, 0, _viewModel.Playback.DurationSeconds))));
    }

    private async void VolumeSlider_MouseUp(object sender, MouseButtonEventArgs e) =>
        await RunAsync(() => _viewModel.SetVolumeAsync(VolumeSlider.Value));
    private async void VolumeSlider_KeyUp(object sender, KeyEventArgs e) =>
        await RunAsync(() => _viewModel.SetVolumeAsync(VolumeSlider.Value));

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) { e.Handled = true; Close(); }
    }

    private async void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (_allowClose) return;
        e.Cancel = true;
        await CloseSafelyAsync();
    }

    internal Task CloseSafelyAsync() => _closeTask ??= CloseCoreAsync();

    private async Task CloseCoreAsync()
    {
        try
        {
            var disposal = _viewModel.DisposeAsync();
            // Close may have been called from Closing; let that event return first.
            await Task.Yield();
            await disposal;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Kontrollplayer konnte nicht freigegeben werden: {exception.Message}");
        }
        finally
        {
            VideoHost.VideoWindowHandleCreated -= VideoHost_HandleCreated;
            Closing -= Window_Closing;
            _allowClose = true;
            Close();
        }
    }
}
