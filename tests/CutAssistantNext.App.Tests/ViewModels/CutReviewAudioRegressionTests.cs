using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed partial class CutReviewViewModelTests
{
    private static Player TwoTracks(double duration = 100) => new()
    {
        AudioTracks = [new(7, "Deutsch AAC", true), new(11, "English AAC", false)],
        ActualDuration = TimeSpan.FromSeconds(duration)
    };

    [Theory]
    [InlineData(0, 100, 0)]
    [InlineData(1, 100, 7)]
    [InlineData(2, 100, 37)]
    [InlineData(3, 100, 97)]
    [InlineData(3, 2, 0)]
    [InlineData(1, 5, 4.9999999)]
    public async Task AudioSwitchKeepsCheckpointAndCurrentPreviewPosition(int index, double duration, double start)
    {
        var p = TwoTracks(duration);
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), index, 123);
        var point = model.CurrentTransitionText;
        await model.SelectAudioAsync(model.AudioTracks[1]);
        Assert.Equal(11, model.SelectedAudioTrack!.Id);
        Assert.Equal(TimeSpan.FromSeconds(start), p.Position);
        Assert.Equal(point, model.CurrentTransitionText);
        Assert.Equal(MediaPlayerState.Playing, p.State);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(3, true)]
    public async Task RegularAudioSwitchKeepsAdvancedPositionAndPlaybackState(int checkpoint, bool paused)
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), checkpoint, 123);
        await model.SeekAsync(TimeSpan.FromSeconds(42.25));
        p.State = paused ? MediaPlayerState.Paused : MediaPlayerState.Playing;
        var point = model.CurrentTransitionText;
        var plays = p.Commands.Count(c => c == "play");
        var seeks = p.Commands.Count(c => c.StartsWith("seek:"));
        foreach (var id in new[] { 11, 7, 11, 7 })
        {
            await model.SelectAudioAsync(model.AudioTracks.Single(t => t.Id == id));
            Assert.Equal(TimeSpan.FromSeconds(42.25), p.Position);
            Assert.Equal(paused ? MediaPlayerState.Paused : MediaPlayerState.Playing, p.State);
            Assert.Equal(point, model.CurrentTransitionText);
            Assert.Equal(id, model.SelectedAudioTrack!.Id);
        }
        Assert.Equal(plays + (paused ? 0 : 4), p.Commands.Count(c => c == "play"));
        Assert.Equal(seeks, p.Commands.Count(c => c.StartsWith("seek:")));
        Assert.Single(p.Commands, c => c.StartsWith("load:"));
    }

    [Fact]
    public async Task SingleTrackNavigationAndNaturalEndRemainUsable()
    {
        var p = new Player { AudioTracks = [new(7, "AAC", true)] };
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 0, 123);
        for (var i = 0; i < 3; i++) await model.MoveAsync(1, 123);
        p.End();
        for (var i = 0; i < 3; i++) await model.MoveAsync(-1, 123);
        Assert.False(model.CanPrevious);
        Assert.True(model.CanNext);
        Assert.False(model.HasMultipleAudioTracks);
        Assert.Equal(TimeSpan.Zero, p.Position);
    }

    [Fact]
    public async Task RepeatedRealIdSelectionSurvivesReloadDuringNavigation()
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 2, 123);
        foreach (var id in new[] { 11, 7, 11, 7, 11 })
            await model.SelectAudioAsync(model.AudioTracks.Single(t => t.Id == id));
        p.End();
        await model.MoveAsync(-1, 123);
        Assert.Equal(11, model.SelectedAudioTrack!.Id);
        Assert.Equal(11, p.AudioTracks.Single(t => t.Selected).Id);
        Assert.Equal(TimeSpan.FromSeconds(7), p.Position);
    }

    [Theory]
    [InlineData(MediaPlayerState.Ended)]
    [InlineData(MediaPlayerState.Empty)]
    [InlineData(MediaPlayerState.Error)]
    public async Task AudioSelectionReloadsInvalidMediaBeforeUsingRequestedId(MediaPlayerState state)
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 3, 123);
        var requested = model.AudioTracks[1];
        p.End();
        p.State = state;
        await model.SelectAudioAsync(requested);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(11, model.SelectedAudioTrack!.Id);
        Assert.Equal(TimeSpan.FromSeconds(97), p.Position);
    }

    [Fact]
    public async Task EndDuringPauseReloadsBeforeAudioSwitch()
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        p.OnPause = player => { player.OnPause = null; player.End(); };
        await model.SelectAudioAsync(model.AudioTracks[1]);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(11, model.SelectedAudioTrack!.Id);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public async Task EndDuringAudioSwitchAllowsFollowingNavigation(int delta)
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        var point = model.CurrentTransitionText;
        p.OnSelect = player => { player.OnSelect = null; player.End(); };
        await Assert.ThrowsAsync<ArgumentException>(() => model.SelectAudioAsync(model.AudioTracks[1]));
        Assert.Equal(point, model.CurrentTransitionText);
        Assert.True(model.IsIdle);
        Assert.Null(model.SelectedAudioTrack);
        await model.MoveAsync(delta, 123);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(TimeSpan.FromSeconds(delta < 0 ? 0 : 37), p.Position);
        Assert.Equal(MediaPlayerState.Playing, p.State);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(1, false)]
    [InlineData(-1, true)]
    [InlineData(1, true)]
    public async Task RejectedAudioSwitchPreservesActualSelectionAndNavigation(int delta, bool changedBeforeFailure)
    {
        var p = TwoTracks();
        var logger = new AudioLogger();
        await using var model = new CutReviewViewModel(p, _ => true, logger: logger);
        await model.ReviewAsync(Completion(), 1, 123);
        p.SelectException = new InvalidOperationException("Native Ablehnung");
        p.FailAfterSelection = changedBeforeFailure;
        var point = model.CurrentTransitionText;
        await Assert.ThrowsAsync<InvalidOperationException>(() => model.SelectAudioAsync(model.AudioTracks[1]));
        Assert.Equal(point, model.CurrentTransitionText);
        var actualId = changedBeforeFailure ? 11 : 7;
        Assert.Equal(actualId, model.SelectedAudioTrack!.Id);
        var log = Assert.Single(logger.Errors);
        Assert.Contains("angefordert=11", log);
        Assert.Contains($"bestätigt={actualId}", log);
        Assert.Contains("Spuren=[7:", log);
        Assert.Contains("Zustand=Paused", log);
        Assert.Contains("Prüfstelle=1", log);
        p.SelectException = null;
        await model.MoveAsync(delta, 123);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(actualId, model.SelectedAudioTrack!.Id);
        Assert.Equal(TimeSpan.FromSeconds(delta < 0 ? 0 : 37), p.Position);
    }

    [Fact]
    public async Task SfonwebLastCheckpointFailureThenPreviousReloadsInsteadOfSeekingUnloadedFile()
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 3, 123);
        var request = model.AudioTracks[1];
        p.End();
        p.SelectException = new ArgumentException("Die gewählte Tonspur steht nicht zur Verfügung.", "id");
        await Assert.ThrowsAsync<ArgumentException>(() => model.SelectAudioAsync(request));
        Assert.Contains("Ende", model.CurrentTransitionText);
        Assert.True(model.CanPrevious);
        p.SelectException = null;
        await model.MoveAsync(-1, 123);
        Assert.Equal(3, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(TimeSpan.FromSeconds(37), p.Position);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("empty")]
    [InlineData("unconfirmed")]
    [InlineData("inventory")]
    [InlineData("seek")]
    public async Task InvalidInventoryReadbackAndSeekFailuresLeaveNavigationUsable(string failure)
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        var request = model.AudioTracks[1];
        if (failure == "invalid") request = new(99, "Nicht vorhanden", false);
        if (failure == "empty") p.AudioTracks = [];
        if (failure == "unconfirmed") p.IgnoreSelection = true;
        if (failure == "inventory") p.InventoryException = new InvalidDataException("Inventar fehlt");
        if (failure == "seek") { p.End(); p.SeekException = new InvalidOperationException("Seek abgelehnt"); }
        var error = await Assert.ThrowsAnyAsync<Exception>(() => model.SelectAudioAsync(request));
        if (failure is "invalid" or "empty") Assert.IsType<ArgumentException>(error);
        else Assert.IsType<InvalidOperationException>(error);
        Assert.True(model.IsIdle);
        Assert.Contains("Übergang 1", model.CurrentTransitionText);
        if (failure == "seek") Assert.Equal(11, model.SelectedAudioTrack!.Id);
        if (failure == "inventory") Assert.Null(model.SelectedAudioTrack);
        p.AudioTracks = [new(7, "AAC", true), new(11, "AAC", false)];
        p.IgnoreSelection = false;
        p.InventoryException = null;
        p.SeekException = null;
        await model.MoveAsync(1, 123);
        Assert.Equal(TimeSpan.FromSeconds(37), p.Position);
    }

    [Fact]
    public async Task TrackMissingAfterReloadIsRejectedWithoutChoosingAReplacement()
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        var request = model.AudioTracks[1];
        p.AudioTracks = [new(7, "AAC", true)];
        p.End();
        await Assert.ThrowsAsync<ArgumentException>(() => model.SelectAudioAsync(request));
        Assert.DoesNotContain(p.Commands, command => command.StartsWith("audio:"));
        Assert.Equal(7, model.SelectedAudioTrack!.Id);
        await model.MoveAsync(1, 123);
        Assert.Equal(TimeSpan.FromSeconds(37), p.Position);
    }

    [Fact]
    public async Task EndDuringNavigationPauseStillReloadsBeforeSeek()
    {
        var p = TwoTracks();
        await using var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 3, 123);
        p.OnPause = player => { player.OnPause = null; player.End(); };
        await model.MoveAsync(-1, 123);
        Assert.Equal(2, p.Commands.Count(c => c.StartsWith("load:")));
        Assert.Equal(TimeSpan.FromSeconds(37), p.Position);
    }

    [Theory]
    [InlineData("audio")]
    [InlineData("inventory")]
    [InlineData("load")]
    [InlineData("seek")]
    public async Task CloseDuringAudioOperationCancelsQueuedNavigationBeforeNativeDisposal(string phase)
    {
        var p = TwoTracks();
        var model = new CutReviewViewModel(p, _ => true);
        await model.ReviewAsync(Completion(), 1, 123);
        var request = model.AudioTracks[1];
        p.BlockPhase = phase;
        if (phase is "load" or "seek") p.End();
        var switching = model.SelectAudioAsync(request);
        Assert.False(switching.IsCompleted);
        var navigation = model.MoveAsync(1, 123);
        await model.DisposeAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => switching);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => navigation);
        Assert.Equal(1, p.DisposeCount);
        Assert.False(p.DisposedWhileCommandRunning);
        Assert.False(model.IsIdle);
        Assert.Equal(0, p.ReadySubscriberCount);
    }

    private sealed class AudioLogger : IAppLogger
    {
        internal List<string> Errors { get; } = [];
        public void Information(string message) { }
        public void Warning(string message) { }
        public void Error(string message, Exception? exception = null) => Errors.Add(message);
    }
}
