using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private PlaybackShortcuts _shortcuts = PlaybackShortcuts.Load(PlaybackShortcuts.DefaultPath);
    private readonly HashSet<Key> _secondsJumpKeys = [];
    private Key _navigationKey;
    private bool _standaloneSPending;
    private ShortcutGesture _heldGesture;
    private Button? _navigationButton;

    private void InitializeSecondsJump()
    {
        PreviewKeyUp += Navigation_PreviewKeyUp;
        PreviewLostKeyboardFocus += (_, _) => StopSecondsJump();
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

    private bool CanNavigate(PlaybackShortcut action) => !_isClosed && !_mediaLoadInProgress && !_frameLoupeOpening &&
        IsEnabled && PlaybackControls.IsEnabled && !_playerFrameSearch.IsNavigating && !_keyframeNavigation.IsNavigating &&
        !_playbackViewModel.IsSeeking && (action switch
        {
            PlaybackShortcut.FrameBackward or PlaybackShortcut.FrameForward => _playbackViewModel.CanStepFrame,
            PlaybackShortcut.KeyframeBackward => _keyframeNavigation.CanPrevious,
            PlaybackShortcut.KeyframeForward => _keyframeNavigation.CanNext,
            _ => _playbackViewModel.CanSeek
        });

    internal bool TryHandleSecondsJumpKey(KeyEventArgs e, ModifierKeys modifiers, bool sDown)
    {
        if (e.Handled || IsPlaybackTextInput(e.OriginalSource as DependencyObject) ||
            IsPlaybackTextInput(Keyboard.FocusedElement as DependencyObject)) return false;
        if (_secondsJumpKeys.Contains(e.Key)) return true;
        var action = _shortcuts.Match(e.Key, modifiers, sDown);
        if (action is not (PlaybackShortcut.SecondsBackward or PlaybackShortcut.SecondsForward)) return false;
        if (!e.IsRepeat)
        {
            _secondsJumpKeys.Add(e.Key);
            if (CanNavigate(action.Value)) _ = HoldNavigationKeyAsync(action.Value, e.Key);
        }
        return true;
    }

    private async Task HoldNavigationKeyAsync(PlaybackShortcut action, Key key)
    {
        if (_navigationKey != Key.None || _navigationButton is not null) return;
        _navigationKey = key;
        _heldGesture = _shortcuts[action];
        try
        {
            await _secondsJump.HoldAsync((int)action, () => _navigationKey == key &&
                Keyboard.IsKeyDown(key) && Keyboard.Modifiers == _heldGesture.Modifiers &&
                (!_heldGesture.WithS || Keyboard.IsKeyDown(Key.S)) &&
                !IsPlaybackTextInput(Keyboard.FocusedElement as DependencyObject) && CanRepeatNavigation(action));
        }
        finally
        {
            if (_navigationKey == key) _navigationKey = Key.None;
        }
    }

    private async void Navigation_PreviewKeyUp(object sender, KeyEventArgs e) =>
        await HandlePlaybackKeyUpAsync(e, Keyboard.Modifiers);

    internal async Task HandlePlaybackKeyUpAsync(KeyEventArgs e, ModifierKeys modifiers)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        _secondsJumpKeys.Remove(key);
        if (_navigationKey != Key.None && (key == _navigationKey || (_heldGesture.WithS && key == Key.S) ||
            modifiers != _heldGesture.Modifiers)) StopSecondsJump();

        if (key != Key.S || !_standaloneSPending) return;
        _standaloneSPending = false;
        if (e.Handled || modifiers != ModifierKeys.None ||
            IsPlaybackTextInput(e.OriginalSource as DependencyObject) ||
            IsPlaybackTextInput(Keyboard.FocusedElement as DependencyObject)) return;
        if (_shortcuts.Match(Key.S, ModifierKeys.None, sDown: false) is { } action)
            await ExecutePlaybackShortcutAsync(e, action, single: true);
    }

    private bool CanRepeatNavigation(PlaybackShortcut action) => CanNavigate(action) &&
        ((int)action < 0 ? _playbackViewModel.Position > TimeSpan.Zero : _playbackViewModel.Position < _playbackViewModel.Duration);

    private void SecondsJumpButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ReferenceEquals(sender, _navigationButton)) StopSecondsJump();
    }

    private void SecondsJumpButton_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (ReferenceEquals(sender, _navigationButton)) StopSecondsJump();
    }

    private void StopSecondsJump()
    {
        _standaloneSPending = false;
        _secondsJump.Stop();
        _navigationKey = Key.None;
        var button = _navigationButton;
        _navigationButton = null;
        if (button?.IsMouseCaptured == true) button.ReleaseMouseCapture();
    }

    private Task ExecuteNavigationAsync(int code) => ExecutePlaybackActionAsync(async () =>
    {
        try
        {
            var token = _mediaInputCancellation.Token;
            switch ((PlaybackShortcut)code)
            {
                case PlaybackShortcut.FrameBackward: await _playbackViewModel.StepBackwardAsync(token); break;
                case PlaybackShortcut.FrameForward: await _playbackViewModel.StepForwardAsync(token); break;
                case PlaybackShortcut.KeyframeBackward:
                case PlaybackShortcut.KeyframeForward: await _keyframeNavigation.JumpAsync(Math.Sign(code)); break;
                default: await _playbackViewModel.JumpAsync(Math.Sign(code), token); break;
            }
        }
        catch
        {
            StopSecondsJump();
            throw;
        }
    });

    private void SecondsJumpButton_Click(object sender, RoutedEventArgs e) => NavigationButton_Click(sender, e);

    private async void NavigationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var action = button == JumpBackwardButton ? PlaybackShortcut.SecondsBackward :
            button == JumpForwardButton ? PlaybackShortcut.SecondsForward :
            button == StepBackwardButton ? PlaybackShortcut.FrameBackward :
            button == StepForwardButton ? PlaybackShortcut.FrameForward :
            button == PreviousKeyframeButton ? PlaybackShortcut.KeyframeBackward : PlaybackShortcut.KeyframeForward;

        if (!CanNavigate(action)) return;
        if (!button.IsMouseCaptured || Mouse.LeftButton != MouseButtonState.Pressed)
        {
            await _secondsJump.JumpOnceAsync((int)action);
            return;
        }
        if (_navigationButton is not null || _navigationKey != Key.None) return;
        _navigationButton = button;
        await _secondsJump.HoldAsync((int)action, () => button.IsMouseCaptured &&
            Mouse.LeftButton == MouseButtonState.Pressed && CanRepeatNavigation(action));
    }

    private void ShortcutSettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        StopSecondsJump();
        var dialog = new ShortcutSettingsDialog(_shortcuts) { Owner = this };
        if (dialog.ShowDialog() != true) return;
        _shortcuts = dialog.Shortcuts;
        UpdateShortcutTooltips();
    }

    private void UpdateShortcutTooltips()
    {
        string Caption(PlaybackShortcut action)
        {
            var definition = PlaybackShortcuts.Definitions.First(d => d.Action == action);
            var binding = _shortcuts[action];
            return binding.Key == Key.None ? definition.Name : $"{definition.Name} ({binding})";
        }
        var cancel = _shortcuts[PlaybackShortcut.CancelCut].ToString();
        _cutPlanViewModel.UpdateCancelCutGesture(cancel);
        PendingStartLabel.ToolTip = $"{cancel} verwirft den offenen Anfang samt Vorschau.";
        PlayPauseButton.ToolTip = Caption(PlaybackShortcut.TogglePlayback);
        JumpBackwardButton.ToolTip = Caption(PlaybackShortcut.SecondsBackward);
        JumpForwardButton.ToolTip = Caption(PlaybackShortcut.SecondsForward);
        StepBackwardButton.ToolTip = Caption(PlaybackShortcut.FrameBackward);
        StepForwardButton.ToolTip = Caption(PlaybackShortcut.FrameForward);
        _keyframeNavigation.UpdateShortcutCaptions(Caption(PlaybackShortcut.KeyframeBackward), Caption(PlaybackShortcut.KeyframeForward));
        _viewModel.UpdateFrameLoupeCaption(_shortcuts[PlaybackShortcut.FrameLoupe] == new ShortcutGesture(Key.F, ModifierKeys.Shift)
            ? "Frame-Lupe öffnen (Shift+F)" : Caption(PlaybackShortcut.FrameLoupe));
    }
}
