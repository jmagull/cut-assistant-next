using System.Windows;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private bool _schnittshufflePausing;

    private async void SchnittshuffleStart_Click(object sender, RoutedEventArgs e) =>
        await SeekSchnittshuffleAsync(CutEdgeSide.Start);

    private async void SchnittshuffleEnd_Click(object sender, RoutedEventArgs e) =>
        await SeekSchnittshuffleAsync(CutEdgeSide.End);

    private async Task SeekSchnittshuffleAsync(CutEdgeSide side)
    {
        if (_isClosed || _mediaLoadInProgress || _schnittshufflePausing ||
            _keyframeNavigation.IsNavigating || _playerFrameSearch.IsNavigating ||
            !_playbackViewModel.CanSeek || _playbackViewModel.IsSeeking ||
            !_cutPlanViewModel.CanShuffleSelectedSegment ||
            _cutPlanViewModel.SelectedRemoveSegment is not { } segment ||
            _cutPlanViewModel.MediaDuration is not { } mediaDuration ||
            _playbackViewModel.Duration is not { } playerDuration)
        {
            return;
        }

        var boundary = side == CutEdgeSide.Start ? segment.Start : segment.End;
        var end = mediaDuration < playerDuration ? mediaDuration : playerDuration;
        var fps = _viewModel.AnalysisResult?.VideoStreams.FirstOrDefault()?.FramesPerSecond;
        var target = GetSchnittshuffleSeekPosition(boundary, end, fps);

        await ExecutePlaybackActionAsync(async () =>
        {
            // Keep the final picture available for inspection when jumping to the file boundary.
            if (boundary >= end && _playbackViewModel.CanPause)
            {
                _schnittshufflePausing = true;
                try
                {
                    await _playbackViewModel.PauseAsync(_mediaInputCancellation.Token);
                }
                finally
                {
                    _schnittshufflePausing = false;
                }
            }

            if (_isClosed || _mediaLoadInProgress ||
                !_cutPlanViewModel.CanShuffleSelectedSegment ||
                !ReferenceEquals(segment, _cutPlanViewModel.SelectedRemoveSegment))
            {
                return;
            }

            await _playbackViewModel.SeekToAsync(target, _mediaInputCancellation.Token);
        });
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
