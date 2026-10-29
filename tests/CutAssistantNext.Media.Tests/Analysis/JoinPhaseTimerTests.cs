using CutAssistantNext.Media.Joining;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class JoinPhaseTimerTests
{
    [Fact]
    public async Task ReportsMonotonicPhaseTimeAndResult()
    {
        var clock = new Clock();
        var values = new List<JoinPhaseTiming>();
        var timer = new JoinPhaseTimer(true, values.Add, clock);
        var result = await timer.RunAsync("aac-encode", () =>
        {
            clock.Advance(TimeSpan.FromMilliseconds(1250));
            return Task.FromResult(42);
        });
        Assert.Equal(42, result);
        var value = Assert.Single(values);
        Assert.Equal("aac-encode", value.Phase);
        Assert.Equal(1250, value.ElapsedMilliseconds);
        Assert.True(value.Succeeded);
    }

    [Fact]
    public async Task FailureIsMeasuredWithoutReplacingTheOriginalException()
    {
        var values = new List<JoinPhaseTiming>();
        var timer = new JoinPhaseTimer(true, values.Add);
        var error = new InvalidDataException("decoder warning");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidDataException>(() =>
            timer.RunAsync("decode", () => Task.FromException(error))));
        Assert.False(Assert.Single(values).Succeeded);
    }

    [Fact]
    public void DisabledTimingStillRunsWorkWithoutPublishingMeasurements()
    {
        var ran = false;
        var timer = new JoinPhaseTimer(false, _ => throw new InvalidOperationException("must not report"));
        timer.Run("copy", () => ran = true);
        timer.Record("total", TimeSpan.Zero, true);
        Assert.True(ran);
    }

    [Fact]
    public async Task CancellationRemainsCancellationAndIsReportedAsIncomplete()
    {
        using var cancel = new CancellationTokenSource();
        cancel.Cancel();
        var values = new List<JoinPhaseTiming>();
        var timer = new JoinPhaseTimer(true, values.Add);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            timer.RunAsync("copy", () => Task.FromCanceled(cancel.Token)));
        Assert.False(Assert.Single(values).Succeeded);
    }

    private sealed class Clock : TimeProvider
    {
        private long _ticks;
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => _ticks;
        public void Advance(TimeSpan time) => _ticks += time.Ticks;
    }
}
