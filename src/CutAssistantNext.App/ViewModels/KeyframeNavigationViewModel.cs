using System.ComponentModel;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.ViewModels;

internal sealed class KeyframeNavigationViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly PlaybackViewModel _playback;
    private CancellationTokenSource? _lifetime;
    private VideoKeyframeIndex? _index;
    private bool _disposed;
    public KeyframeNavigationViewModel(PlaybackViewModel playback)
    {
        _playback = playback ?? throw new ArgumentNullException(nameof(playback));
        _playback.PropertyChanged += PlaybackChanged;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public IReadOnlyList<TimeSpan> Positions => _index?.Positions ?? [];
    public string Status { get; private set; } = "Keyframe-Informationen noch nicht verfügbar.";
    public bool IsSupported { get; private set; }
    public bool IsLoading { get; private set; }
    public bool IsNavigating { get; private set; }
    private bool CanJump => !_disposed && _index is not null && !IsNavigating && _playback.CanSeek && !_playback.IsSeeking;
    public bool CanPrevious => CanJump && _index!.FindAdjacent(_playback.Position, -1).HasValue;
    public bool CanNext => CanJump && _index!.FindAdjacent(_playback.Position, 1).HasValue;
    public string Indicator => !_disposed && !IsNavigating && !_playback.IsSeeking &&
        _playback.State == MediaPlayerState.Paused && _index?.Contains(_playback.Position) == true ? " (K)" : "";

    public void Reset()
    {
        _lifetime?.Cancel();
        _lifetime?.Dispose();
        _lifetime = null;
        _index = null;
        IsSupported = false;
        IsLoading = false;
        IsNavigating = false;
        Status = "Keyframe-Informationen noch nicht verfügbar.";
        Publish();
    }

    public async Task LoadAsync(string path, MediaAnalysisResult media, IKeyframeAnalysisRunner runner)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Reset();
        if (media.VideoStreams.Count != 1 || media.Duration is not TimeSpan duration || duration <= TimeSpan.Zero)
        {
            Status = "Keyframes nicht verfügbar: eindeutige Videospur und Dauer erforderlich.";
            Publish();
            return;
        }
        var lifetime = _lifetime = new CancellationTokenSource();
        var token = lifetime.Token;
        bool avi = (media.FormatName ?? "").Split(',', StringSplitOptions.TrimEntries).Contains("avi", StringComparer.OrdinalIgnoreCase);
        if (avi)
        {
            Status = "Keyframe-Anzeige und Keyframe-Navigation sind für AVI nicht verfügbar. Die Frame-Lupe bleibt nutzbar.";
            Publish();
            return;
        }
        IsSupported = true;
        IsLoading = true;
        Status = "Keyframes werden analysiert …";
        Publish();
        try
        {
            var index = await Task.Run(async () => VideoKeyframeIndex.Create(
                await runner.RunKeyframesAsync(path, media.VideoStreams[0].Index, token),
                media.VideoStreams[0].Index, duration, (decimal)(media.StartTimeSeconds ?? 0)), token);
            if (_disposed || !ReferenceEquals(_lifetime, lifetime) || token.IsCancellationRequested) return;
            _index = index;
            Status = $"{index.Positions.Count:N0} Keyframes · Original-Zeitstempel. Blaue Bereiche: Keyframes; rote Flächen: Entfernbereiche.";
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception exception)
        {
            if (ReferenceEquals(_lifetime, lifetime) && !_disposed)
                Status = "Keyframes nicht verfügbar: " + exception.Message;
        }
        finally
        {
            if (ReferenceEquals(_lifetime, lifetime) && !_disposed) { IsLoading = false; Publish(); }
        }
    }

    public async Task JumpAsync(int direction)
    {
        if (direction is not (-1 or 1)) throw new ArgumentOutOfRangeException(nameof(direction));
        if (!CanJump || _lifetime is null) return;
        var lifetime = _lifetime;
        var token = lifetime.Token;
        IsNavigating = true;
        Publish();
        try
        {
            if (_playback.CanPause) await _playback.PauseAsync(token);
            token.ThrowIfCancellationRequested();
            var target = _index!.FindAdjacent(_playback.Position, direction);
            if (!target.HasValue) return;
            await _playback.SeekToAsync(target.Value, token);
            await WaitForPositionAsync(target.Value, token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception exception)
        {
            if (ReferenceEquals(_lifetime, lifetime) && !_disposed) Status = "Keyframe-Sprung nicht bestätigt: " + exception.Message;
        }
        finally
        {
            if (ReferenceEquals(_lifetime, lifetime) && !_disposed) { IsNavigating = false; Publish(); }
        }
    }

    private async Task WaitForPositionAsync(TimeSpan target, CancellationToken token)
    {
        bool Reached() => Math.Abs((decimal)(_playback.Position.Ticks) - target.Ticks) <= VideoKeyframeIndex.TimestampTolerance.Ticks;
        var reached = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void Changed(object? sender, PropertyChangedEventArgs args) { if (Reached()) reached.TrySetResult(); }
        _playback.PropertyChanged += Changed;
        try
        {
            if (Reached()) return;
            await reached.Task.WaitAsync(TimeSpan.FromSeconds(5), token);
        }
        finally { _playback.PropertyChanged -= Changed; }
    }

    private void PlaybackChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(PlaybackViewModel.Position) or nameof(PlaybackViewModel.State) or
            nameof(PlaybackViewModel.IsSeeking) or nameof(PlaybackViewModel.CanSeek)) Publish();
    }
    private void Publish()
    {
        foreach (string property in new[] { nameof(Positions), nameof(Status), nameof(IsSupported), nameof(IsLoading), nameof(IsNavigating),
            nameof(CanPrevious), nameof(CanNext), nameof(Indicator) })
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _playback.PropertyChanged -= PlaybackChanged;
        Reset();
    }
}
