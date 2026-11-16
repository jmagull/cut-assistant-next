using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services;

// Used on the UI thread. Releasing the input cancels the delay, while an
// already dispatched seek finishes before another input can start a seek.
internal sealed class SecondsJumpRepeat(
    Func<int, Task> jump,
    Func<TimeSpan, CancellationToken, Task>? delay = null)
{
    internal static readonly TimeSpan InitialDelay = TimeSpan.FromMilliseconds(AppearanceSettings.DefaultInitialDelayMs);
    internal static readonly TimeSpan RepeatInterval = TimeSpan.FromMilliseconds(AppearanceSettings.DefaultRepeatIntervalMs);

    private TimeSpan _initialDelay = InitialDelay;
    private TimeSpan _repeatInterval = RepeatInterval;

    internal void Configure(AppearanceSettings settings)
    {
        if (!settings.HasValidNavigationTiming) throw new ArgumentOutOfRangeException(nameof(settings));
        Stop();
        _initialDelay = TimeSpan.FromMilliseconds(settings.NavigationInitialDelayMs);
        _repeatInterval = TimeSpan.FromMilliseconds(settings.NavigationRepeatIntervalMs);
    }

    private CancellationTokenSource? _hold;
    private bool _jumpInProgress;

    internal async Task JumpOnceAsync(int direction)
    {
        if (_hold is not null || _jumpInProgress) return;
        await RunJumpAsync(direction);
    }

    private async Task RunJumpAsync(int direction)
    {
        _jumpInProgress = true;
        try { await jump(direction); }
        finally { _jumpInProgress = false; }
    }

    internal async Task HoldAsync(int direction, Func<bool> canRepeat)
    {
        if (_hold is not null || _jumpInProgress) return;
        using var hold = new CancellationTokenSource();
        _hold = hold;
        try
        {
            await Task.WhenAll(WaitAsync(_initialDelay, hold.Token), RunJumpAsync(direction));

            while (!hold.IsCancellationRequested && canRepeat())
            {
                await Task.WhenAll(WaitAsync(_repeatInterval, hold.Token), RunJumpAsync(direction));
            }
        }
        catch (OperationCanceledException) when (hold.IsCancellationRequested) { }
        finally
        {
            if (ReferenceEquals(_hold, hold)) _hold = null;
        }
    }

    internal void Stop()
    {
        var hold = _hold;
        _hold = null;
        hold?.Cancel();
    }

    private Task WaitAsync(TimeSpan interval, CancellationToken token) =>
        delay is null ? Task.Delay(interval, token) : delay(interval, token);
}
