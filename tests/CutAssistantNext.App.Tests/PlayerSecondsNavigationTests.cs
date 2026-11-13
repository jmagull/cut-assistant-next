using System.IO;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class PlayerSecondsNavigationTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("5", 5)]
    [InlineData("10", 10)]
    [InlineData("30", 30)]
    [InlineData("60", 60)]
    [InlineData("2,5", 2.5)]
    [InlineData("0.125", 0.125)]
    public async Task ButtonsJumpBothDirectionsFromCurrentPlayerPosition(string text, double step)
    {
        var service = Player();
        using var model = new PlaybackViewModel(service) { JumpSecondsText = text };
        await model.JumpAsync(1); Assert.Equal(TimeSpan.FromSeconds(100 + step), service.Position);
        await model.JumpAsync(-1); Assert.Equal(TimeSpan.FromSeconds(100), service.Position);
        Assert.Equal(2, service.SeekPositions.Count);
        Assert.Empty(service.StepFrameCounts);
    }

    [Theory]
    [InlineData(MediaPlayerState.Paused)]
    [InlineData(MediaPlayerState.Playing)]
    public async Task JumpPreservesPlaybackStateAndDoesNotStartOrPausePlayer(MediaPlayerState state)
    {
        var service = Player(); service.State = state;
        using var model = new PlaybackViewModel(service);
        await model.JumpAsync(1);
        Assert.Equal(state, service.State); Assert.Equal(0, service.PlayCallCount); Assert.Equal(0, service.PauseCallCount);
    }

    [Theory]
    [InlineData(0, -1, 0)]
    [InlineData(3, -1, 0)]
    [InlineData(195, 1, 200)]
    [InlineData(200, 1, 200)]
    public async Task ClampsToFileBoundaries(double start, int direction, double expected)
    {
        var service = Player(); service.Position = TimeSpan.FromSeconds(start);
        using var model = new PlaybackViewModel(service);
        await model.JumpAsync(direction); Assert.Equal(TimeSpan.FromSeconds(expected), service.Position);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-10")]
    [InlineData("NaN")]
    [InlineData("unbekannt")]
    [InlineData("0.00000001")]
    public async Task InvalidInputDoesNotSeekOrDiscardLastValidSetting(string text)
    {
        var service = Player();
        using var model = new PlaybackViewModel(service) { JumpSecondsText = "7,5" };
        model.JumpSecondsText = text;
        await Assert.ThrowsAsync<ArgumentException>(() => model.JumpAsync(1));
        Assert.Empty(service.SeekPositions); Assert.Equal("7.5", model.JumpSecondsSetting);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("0")]
    [InlineData("-3")]
    [InlineData("NaN")]
    [InlineData("ungültig")]
    public void InvalidOrMissingStoredValueFallsBackToTen(string? text)
    {
        using var model = new PlaybackViewModel(Player());
        model.RestoreJumpSeconds(text); Assert.Equal("10", model.JumpSecondsText);
        Assert.Equal(new[] { "1", "5", "10", "30", "60" }, model.JumpPresets);
    }

    [Fact]
    public void JumpIsStoredInExistingCanSettingsAlongsideVolumeAndWindowSize()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        try
        {
            using var first = new PlaybackViewModel(Player()) { JumpSecondsText = "7,5" };
            Assert.True(new WindowSettingsStore(path).Save(new WindowSettings
            { Width = 1200, Height = 800, Volume = 42, SeekStepSeconds = first.JumpSecondsSetting }));
            var saved = new WindowSettingsStore(path).Load()!;
            using var second = new PlaybackViewModel(Player()); second.RestoreJumpSeconds(saved.SeekStepSeconds);
            Assert.Equal("7.5", second.JumpSecondsText); Assert.Equal(1200, saved.Width); Assert.Equal(42, saved.Volume);
            File.WriteAllText(path, "{ \"Width\":1200, \"Height\":800, \"SeekStepSeconds\":\"kaputt\" }");
            saved = new WindowSettingsStore(path).Load()!; second.RestoreJumpSeconds(saved.SeekStepSeconds);
            Assert.Equal("10", second.JumpSecondsText); Assert.Equal(1200, saved.Width);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task SingleFramesStillUseFrameNavigation()
    {
        var service = Player(); using var model = new PlaybackViewModel(service) { JumpSecondsText = "60" };
        await model.StepBackwardAsync(); await model.StepForwardAsync();
        Assert.Equal(1, service.StepBackwardCallCount); Assert.Equal(1, service.StepForwardCallCount);
        Assert.Empty(service.StepFrameCounts); Assert.Empty(service.SeekPositions);
    }

    [Fact]
    public async Task CancellationAndFailureLeavePositionAndSeekStateRecoverable()
    {
        var service = Player(); using var model = new PlaybackViewModel(service);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => model.JumpAsync(1, new CancellationToken(true)));
        Assert.False(model.IsSeeking); Assert.Equal(TimeSpan.FromSeconds(100), model.Position);
        service.SeekError = new InvalidOperationException("Seek fehlgeschlagen");
        await Assert.ThrowsAsync<InvalidOperationException>(() => model.JumpAsync(1));
        Assert.False(model.IsSeeking); Assert.Equal(TimeSpan.FromSeconds(100), model.Position);
    }

    private static PlaybackViewModelTests.StubMediaPlayerService Player() => new()
    { State = MediaPlayerState.Paused, Duration = TimeSpan.FromSeconds(200), Position = TimeSpan.FromSeconds(100) };
}
