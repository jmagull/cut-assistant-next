using System.Windows;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Services;

internal sealed class CutReviewController(Window owner, IAppLogger logger,
    Func<CutReviewWindow>? createWindow = null)
{
    private CutReviewWindow? _window;
    private bool _closed;

    internal async Task ReviewAsync(CutCompletionViewModel completion, int index)
    {
        if (_closed) return;
        // A closing HWND must finish disposal before a replacement can be created.
        if (_window is { IsClosing: true } closing)
            await closing.CloseSafelyAsync();
        if (_closed) return;
        if (_window is null)
        {
            var window = createWindow?.Invoke() ?? new CutReviewWindow(new MpvMediaPlayerService(logger));
            window.Owner = owner;
            window.Closed += (_, _) => { if (ReferenceEquals(_window, window)) _window = null; };
            _window = window;
            try { window.Show(); }
            catch
            {
                await window.CloseSafelyAsync();
                throw;
            }
        }
        var current = _window;
        if (current.WindowState == WindowState.Minimized) current.WindowState = WindowState.Normal;
        if (current.ShowActivated) current.Activate();
        await current.ReviewAsync(completion, index);
    }

    internal async Task CloseAsync()
    {
        _closed = true;
        if (_window is { } window) await window.CloseSafelyAsync();
    }
}
