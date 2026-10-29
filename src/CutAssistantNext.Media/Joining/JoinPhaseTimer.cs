using System.Text.Json;

namespace CutAssistantNext.Media.Joining;

public sealed record JoinPhaseTiming(string Phase, TimeSpan Elapsed, bool Succeeded)
{
    public double ElapsedMilliseconds => Elapsed.TotalMilliseconds;
    public string ToProtocolLine() => "Phasenzeit: " + JsonSerializer.Serialize(this);
}

internal sealed class JoinPhaseTimer(bool enabled, Action<JoinPhaseTiming> report, TimeProvider? clock = null)
{
    private readonly TimeProvider _clock = clock ?? TimeProvider.System;

    public void Record(string phase, TimeSpan elapsed, bool succeeded)
    {
        if (enabled) report(new(phase, elapsed, succeeded));
    }

    public async Task<T> RunAsync<T>(string phase, Func<Task<T>> action)
    {
        if (!enabled) return await action().ConfigureAwait(false);
        var start = _clock.GetTimestamp();
        var succeeded = false;
        try
        {
            var value = await action().ConfigureAwait(false);
            succeeded = true;
            return value;
        }
        finally { Record(phase, _clock.GetElapsedTime(start), succeeded); }
    }

    public Task RunAsync(string phase, Func<Task> action) => RunAsync(phase, async () =>
    {
        await action().ConfigureAwait(false);
        return true;
    });

    public void Run(string phase, Action action)
    {
        if (!enabled) { action(); return; }
        var start = _clock.GetTimestamp();
        var succeeded = false;
        try { action(); succeeded = true; }
        finally { Record(phase, _clock.GetElapsedTime(start), succeeded); }
    }
}
