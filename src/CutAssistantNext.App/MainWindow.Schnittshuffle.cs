using System.Windows;
using System.Windows.Input;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private bool _schnittshuffleSeeking;

    private void Schnittshuffle_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (Mouse.LeftButton == MouseButtonState.Pressed && ReferenceEquals(e.NewFocus, sender) &&
            e.OldFocus is DependencyObject previous && ReferenceEquals(Window.GetWindow(previous), this) &&
            !IsPlaybackTextInput(previous))
            e.Handled = true;
    }

    private async void SchnittshuffleStart_Click(object sender, RoutedEventArgs e) =>
        await SeekSchnittshuffleAsync(forward: false);

    private async void SchnittshuffleEnd_Click(object sender, RoutedEventArgs e) =>
        await SeekSchnittshuffleAsync(forward: true);

    private async Task SeekSchnittshuffleAsync(bool forward)
    {
        if (_isClosed || _mediaLoadInProgress || _schnittshuffleSeeking ||
            _keyframeNavigation.IsNavigating || _playerFrameSearch.IsNavigating ||
            !_playbackViewModel.CanSeek || _playbackViewModel.IsSeeking ||
            _cutPlanViewModel.MediaDuration is not { } mediaDuration ||
            _playbackViewModel.Duration is not { } playerDuration ||
            _cutPlanViewModel.FindShuffleBoundary(forward, _playbackViewModel.Position) is not { } next)
            return;

        var previousSegment = _cutPlanViewModel.SelectedRemoveSegment;
        var previousEdge = _cutPlanViewModel.SelectedEdgeSide;
        var boundary = next.Side == CutEdgeSide.Start ? next.Segment.Start : next.Segment.End;
        var end = mediaDuration < playerDuration ? mediaDuration : playerDuration;
        var fps = _viewModel.AnalysisResult?.VideoStreams.FirstOrDefault()?.FramesPerSecond;
        var target = GetSchnittshuffleSeekPosition(boundary, end, fps);

        _schnittshuffleSeeking = true;
        try
        {
            await ExecutePlaybackActionAsync(async () =>
            {
                if (_playbackViewModel.CanPause)
                    await _playbackViewModel.PauseAsync(_mediaInputCancellation.Token);

                if (!CanCompleteJump()) return;
                await _playbackViewModel.SeekToAsync(target, _mediaInputCancellation.Token);
                if (CanCompleteJump()) _cutPlanViewModel.SelectEdge(next.Segment, next.Side);
            });
        }
        finally
        {
            _schnittshuffleSeeking = false;
        }

        bool CanCompleteJump() => !_isClosed && !_mediaLoadInProgress && _cutPlanViewModel.CanShuffleBoundaries &&
            ReferenceEquals(previousSegment, _cutPlanViewModel.SelectedRemoveSegment) &&
            previousEdge == _cutPlanViewModel.SelectedEdgeSide &&
            _cutPlanViewModel.MarkedSegments.Any(segment => ReferenceEquals(segment, next.Segment));
    }

    internal static TimeSpan GetSchnittshuffleSeekPosition(TimeSpan boundary, TimeSpan end, double? framesPerSecond)
    {
        if (boundary < end) return boundary;

        // The stored end is exclusive. Seek one nominal frame earlier using the existing player seek.
        var fps = framesPerSecond is > 0 and < double.PositiveInfinity ? framesPerSecond.Value : 25;
        var frameTicks = Math.Clamp(Math.Ceiling(TimeSpan.TicksPerSecond / fps), 1, end.Ticks);
        return TimeSpan.FromTicks(end.Ticks - (long)frameTicks);
    }
}
