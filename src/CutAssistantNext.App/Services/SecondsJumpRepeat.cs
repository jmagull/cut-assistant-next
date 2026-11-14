namespace CutAssistantNext.App.Services;

// Used on the UI thread. Releasing the input cancels the delay, while an
// already dispatched seek finishes before another input can start a seek.
internal sealed class SecondsJumpRepeat(
    Func<int, Task> jump,
    Func<TimeSpan, CancellationToken, Task>? delay = null)
{
    internal static readonly TimeSpan InitialDelay = TimeSpan.FromMilliseconds(350);
    internal static readonly TimeSpan RepeatInterval = TimeSpan.FromMilliseconds(200);

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
            await Task.WhenAll(WaitAsync(InitialDelay, hold.Token), RunJumpAsync(direction));

            while (!hold.IsCancellationRequested && canRepeat())
            {
                await Task.WhenAll(WaitAsync(RepeatInterval, hold.Token), RunJumpAsync(direction));
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
