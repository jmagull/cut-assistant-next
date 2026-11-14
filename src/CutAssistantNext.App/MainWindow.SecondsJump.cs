using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private readonly HashSet<Key> _secondsJumpKeys = [];
    private Key _secondsJumpKey;
    private Button? _secondsJumpButton;

    private void InitializeSecondsJump()
    {
        PreviewKeyUp += SecondsJump_PreviewKeyUp;
        PreviewLostKeyboardFocus += (_, e) =>
        {
            if (_secondsJumpKey != Key.None && (e.NewFocus is null || IsPlaybackTextInput(e.NewFocus as DependencyObject)))
                StopSecondsJump();
        };
        Deactivated += (_, _) =>
        {
            _secondsJumpKeys.Clear();
            StopSecondsJump();
        };
        PlaybackControls.IsEnabledChanged += (_, _) =>
        {
            if (!PlaybackControls.IsEnabled) StopSecondsJump();
        };
        _playbackViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PlaybackViewModel.CanSeek) && !_playbackViewModel.CanSeek)
                StopSecondsJump();
        };
    }

    private bool CanJumpSeconds => !_isClosed && !_mediaLoadInProgress && !_frameLoupeOpening &&
        IsEnabled && PlaybackControls.IsEnabled && _playbackViewModel.CanSeek &&
        !_playerFrameSearch.IsNavigating && !_keyframeNavigation.IsNavigating;

    internal bool TryHandleSecondsJumpKey(KeyEventArgs e, ModifierKeys modifiers, bool sDown)
    {
        if (e.Handled || IsPlaybackTextInput(e.OriginalSource as DependencyObject) ||
            IsPlaybackTextInput(Keyboard.FocusedElement as DependencyObject)) return false;

        var key = e.Key;
        if (modifiers != ModifierKeys.None)
        {
            if (_secondsJumpKey != Key.None) StopSecondsJump();
            return false;
        }

        // A chord's arrow must be released before it can become an ordinary frame step.
        if (_secondsJumpKeys.Contains(key))
            return true;

        if (!sDown || key is not (Key.Left or Key.Right))
            return false;

        if (!e.IsRepeat)
        {
            _secondsJumpKeys.Add(key);
            if (CanJumpSeconds && !_playbackViewModel.IsSeeking && _secondsJumpButton is null && _secondsJumpKey == Key.None)
            {
                _secondsJumpKey = key;
                _ = HoldSecondsJumpAsync(key == Key.Left ? -1 : 1, () =>
                    Keyboard.Modifiers == ModifierKeys.None && Keyboard.IsKeyDown(Key.S) && Keyboard.IsKeyDown(key) &&
                    !IsPlaybackTextInput(Keyboard.FocusedElement as DependencyObject));
            }
        }

        return true;
    }

    private void SecondsJump_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        _secondsJumpKeys.Remove(key);
        if (_secondsJumpKey != Key.None && (key == Key.S || key == _secondsJumpKey))
            StopSecondsJump();
    }

    private void SecondsJumpButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ReferenceEquals(sender, _secondsJumpButton)) StopSecondsJump();
    }

    private void SecondsJumpButton_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (ReferenceEquals(sender, _secondsJumpButton)) StopSecondsJump();
    }

    private Task HoldSecondsJumpAsync(int direction, Func<bool> isHeld) =>
        _secondsJump.HoldAsync(direction, () => CanJumpSeconds && isHeld() &&
            (direction < 0 ? _playbackViewModel.Position > TimeSpan.Zero : _playbackViewModel.Position < _playbackViewModel.Duration));

    private void StopSecondsJump()
    {
        _secondsJump.Stop();
        _secondsJumpKey = Key.None;
        var button = _secondsJumpButton;
        _secondsJumpButton = null;
        if (button?.IsMouseCaptured == true) button.ReleaseMouseCapture();
    }

    private Task JumpSecondsAsync(int direction) => ExecutePlaybackActionAsync(async () =>
    {
        try { await _playbackViewModel.JumpAsync(direction, _mediaInputCancellation.Token); }
        catch
        {
            StopSecondsJump();
            throw;
        }
    });

    private async void SecondsJumpButton_Click(object sender, RoutedEventArgs e)
    {
        if (!CanJumpSeconds || _playbackViewModel.IsSeeking || sender is not Button button) return;
        var direction = ReferenceEquals(button, JumpBackwardButton) ? -1 : 1;
        if (!button.IsMouseCaptured || Mouse.LeftButton != MouseButtonState.Pressed)
        {
            await _secondsJump.JumpOnceAsync(direction);
            return;
        }

        if (_secondsJumpButton is not null) return;
        StopSecondsJump();
        _secondsJumpButton = button;
        await HoldSecondsJumpAsync(direction, () => button.IsMouseCaptured && Mouse.LeftButton == MouseButtonState.Pressed);
    }
}
