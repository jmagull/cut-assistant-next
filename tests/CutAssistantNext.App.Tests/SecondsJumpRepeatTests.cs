using CutAssistantNext.App.Settings;
using System.Threading.Channels;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests;

public sealed class SecondsJumpRepeatTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public async Task ShortHoldJumpsExactlyOnce(int direction)
    {
        var clock = new TestClock();
        var jumps = new List<int>();
        var repeat = new SecondsJumpRepeat(value =>
        {
            jumps.Add(value);
            return Task.CompletedTask;
        }, clock.Delay);

        var hold = repeat.HoldAsync(direction, () => true);
        var first = await clock.NextAsync();
        Assert.Equal(TimeSpan.FromMilliseconds(350), first.Interval);
        Assert.Equal([direction], jumps);
        repeat.Stop();
        await hold;
        Assert.Equal([direction], jumps);
    }

    [Fact]
    public async Task HeldInputRepeatsAfterInitialDelayAndStopsWhenPlayerIsDisabled()
    {
        var clock = new TestClock();
        var jumps = new List<int>();
        var canRepeat = true;
        var repeat = new SecondsJumpRepeat(direction =>
        {
            jumps.Add(direction);
            return Task.CompletedTask;
        }, clock.Delay);

        var hold = repeat.HoldAsync(1, () => canRepeat);
        (await clock.NextAsync()).Elapsed.SetResult();
        var interval = await clock.NextAsync();
        Assert.Equal(TimeSpan.FromMilliseconds(200), interval.Interval);
        interval.Elapsed.SetResult();
        interval = await clock.NextAsync();
        canRepeat = false;
        interval.Elapsed.SetResult();
        await hold;
        Assert.Equal([1, 1, 1], jumps);
    }

    [Fact]
    public async Task NewPressesAndElapsedIntervalsDoNotQueueBehindSlowSeek()
    {
        var clock = new TestClock();
        var seek = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var jumps = new List<int>();
        var repeat = new SecondsJumpRepeat(direction =>
        {
            jumps.Add(direction);
            return seek.Task;
        }, clock.Delay);

        var hold = repeat.HoldAsync(1, () => true);
        (await clock.NextAsync()).Elapsed.SetResult();
        await repeat.HoldAsync(-1, () => true);
        await repeat.JumpOnceAsync(-1);
        repeat.Stop();
        await repeat.HoldAsync(-1, () => true);
        await repeat.JumpOnceAsync(-1);
        Assert.Equal([1], jumps);

        seek.SetResult();
        await hold;
        Assert.Equal([1], jumps);
        await repeat.JumpOnceAsync(-1);
        Assert.Equal([1, -1], jumps);
    }

    [Theory]
    [InlineData(-3)]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CustomDelaysApplyToEveryNavigationAction(int action)
    {
        var clock = new TestClock();
        var jumps = new List<int>();
        var repeat = new SecondsJumpRepeat(code =>
        {
            jumps.Add(code);
            return Task.CompletedTask;
        }, clock.Delay);
        repeat.Configure(new() { NavigationInitialDelayMs = 500, NavigationRepeatIntervalMs = 120 });

        var hold = repeat.HoldAsync(action, () => true);
        var first = await clock.NextAsync();
        Assert.Equal(TimeSpan.FromMilliseconds(500), first.Interval);
        first.Elapsed.SetResult();

        var second = await clock.NextAsync();
        Assert.Equal(TimeSpan.FromMilliseconds(120), second.Interval);
        repeat.Stop();
        await hold;
        Assert.Equal([action, action], jumps);
    }

    private sealed class TestClock
    {
        private readonly Channel<DelayRequest> _requests = Channel.CreateUnbounded<DelayRequest>();

        internal Task Delay(TimeSpan interval, CancellationToken token)
        {
            var elapsed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _requests.Writer.TryWrite(new DelayRequest(interval, elapsed));
            return elapsed.Task.WaitAsync(token);
        }

        internal async Task<DelayRequest> NextAsync()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            return await _requests.Reader.ReadAsync(timeout.Token);
        }
    }

    private sealed record DelayRequest(TimeSpan Interval, TaskCompletionSource Elapsed);
}
