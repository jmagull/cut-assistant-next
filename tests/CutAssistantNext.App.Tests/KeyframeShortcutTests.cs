using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class KeyframeShortcutTests
{
    [Theory]
    [InlineData("Standard", Key.Left, 2)]
    [InlineData("Standard", Key.Right, 4)]
    [InlineData("PowerUser", Key.Left, 2)]
    [InlineData("PowerUser", Key.Right, 4)]
    [InlineData("PowerUser2", Key.Left, 2)]
    [InlineData("PowerUser2", Key.Right, 4)]
    public Task ControlArrowsUseExistingKeyframeNavigationInEveryLayout(string layout, Key key, int expected) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load(); await fixture.Layout(layout);
        var plan = fixture.Field<CutPlanViewModel>("_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(10));
        plan.SetStart(TimeSpan.FromSeconds(1));
        var args = fixture.Key(key);
        var jump = fixture.Window.TryHandleKeyframeShortcutAsync(args, ModifierKeys.Control);
        Assert.NotNull(jump); Assert.True(args.Handled); await jump;
        Assert.Equal(TimeSpan.FromSeconds(expected), fixture.Player.SeekPositions.Single());
        Assert.Equal(TimeSpan.FromSeconds(1), plan.PendingStart); Assert.Empty(plan.MarkedSegments);
        Assert.Equal(0, fixture.Player.StepForwardCallCount); Assert.Equal(0, fixture.Player.StepBackwardCallCount);
        Assert.Empty(fixture.Player.StepFrameCounts);
    });

    [Theory]
    [InlineData(ModifierKeys.None)]
    [InlineData(ModifierKeys.Shift)]
    [InlineData(ModifierKeys.Alt)]
    [InlineData(ModifierKeys.Control | ModifierKeys.Shift)]
    [InlineData(ModifierKeys.Control | ModifierKeys.Alt)]
    [InlineData(ModifierKeys.Control | ModifierKeys.Windows)]
    public Task OtherModifiersRemainUnclaimed(ModifierKeys modifiers) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load();
        foreach (var key in new[] { Key.Left, Key.Right })
        {
            var args = fixture.Key(key);
            Assert.Null(fixture.Window.TryHandleKeyframeShortcutAsync(args, modifiers)); Assert.False(args.Handled);
        }
        Assert.Empty(fixture.Player.SeekPositions);
    });

    [Theory]
    [InlineData("text")]
    [InlineData("richtext")]
    [InlineData("editable_combo")]
    [InlineData("combo")]
    [InlineData("password")]
    public Task TextAndSelectionInputsKeepControlArrows(string kind) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load();
        DependencyObject input = kind switch
        {
            "text" => new TextBox(),
            "richtext" => new RichTextBox(),
            "password" => new PasswordBox(),
            _ => new ComboBox { IsEditable = kind == "editable_combo" }
        };
        foreach (var key in new[] { Key.Left, Key.Right })
        {
            var args = fixture.Key(key, input);
            Assert.Null(fixture.Window.TryHandleKeyframeShortcutAsync(args, ModifierKeys.Control)); Assert.False(args.Handled);
        }
        Assert.Empty(fixture.Player.SeekPositions);
    });

    [Theory]
    [InlineData("no_index")]
    [InlineData("avi")]
    [InlineData("error")]
    [InlineData("seeking")]
    [InlineData("window_disabled")]
    [InlineData("player_disabled")]
    [InlineData("loading")]
    [InlineData("loupe_opening")]
    [InlineData("closed")]
    public Task UnsafeOrUnavailableNavigationIsNotExecuted(string reason) => Sta(async () =>
    {
        await using var fixture = new Fixture();
        if (reason != "no_index") await fixture.Load(reason == "avi" ? "avi" : "mp4", reason == "error");
        if (reason == "seeking") fixture.Playback.BeginSeek();
        if (reason == "window_disabled") fixture.Window.IsEnabled = false;
        if (reason == "player_disabled") ((Grid)fixture.Window.FindName("PlaybackControls")).IsEnabled = false;
        if (reason == "loading") fixture.Set("_mediaLoadInProgress", true);
        if (reason == "loupe_opening") fixture.Set("_frameLoupeOpening", true);
        if (reason == "closed") fixture.Set("_isClosed", true);
        var args = fixture.Key(Key.Right);
        Assert.Null(fixture.Window.TryHandleKeyframeShortcutAsync(args, ModifierKeys.Control));
        Assert.False(args.Handled); Assert.Empty(fixture.Player.SeekPositions);
    });

    [Theory]
    [InlineData(0, Key.Left)]
    [InlineData(8, Key.Right)]
    public Task MissingAdjacentKeyframeDoesNotConsumeKey(int position, Key key) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load(); fixture.Player.Position = TimeSpan.FromSeconds(position);
        var args = fixture.Key(key);
        Assert.Null(fixture.Window.TryHandleKeyframeShortcutAsync(args, ModifierKeys.Control));
        Assert.False(args.Handled); Assert.Empty(fixture.Player.SeekPositions);
    });

    [Fact]
    public Task ConcurrentAndAlreadyHandledKeysCannotStartAnotherJump() => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load();
        var handled = fixture.Key(Key.Left); handled.Handled = true;
        Assert.Null(fixture.Window.TryHandleKeyframeShortcutAsync(handled, ModifierKeys.Control));
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Player.SeekCompletion = completion.Task;
        var first = fixture.Key(Key.Right);
        var jump = fixture.Window.TryHandleKeyframeShortcutAsync(first, ModifierKeys.Control);
        Assert.NotNull(jump); Assert.True(first.Handled); Assert.True(fixture.Keys.IsNavigating);
        var second = fixture.Key(Key.Left);
        Assert.Null(fixture.Window.TryHandleKeyframeShortcutAsync(second, ModifierKeys.Control)); Assert.False(second.Handled);
        completion.SetResult(); await jump; Assert.Single(fixture.Player.SeekPositions);
    });

    [Fact]
    public Task ShortcutIsAvailableWhilePlayingJustLikeKeyframeButtons() => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load(); fixture.Player.State = MediaPlayerState.Playing;
        Assert.False(fixture.Playback.CanStepFrame);
        var jump = fixture.Window.TryHandleKeyframeShortcutAsync(fixture.Key(Key.Right), ModifierKeys.Control);
        Assert.NotNull(jump); await jump;
        Assert.Equal(1, fixture.Player.PauseCallCount); Assert.Equal(TimeSpan.FromSeconds(4), fixture.Player.SeekPositions.Single());
    });

    [Theory]
    [InlineData("Standard")]
    [InlineData("PowerUser")]
    [InlineData("PowerUser2")]
    public Task TooltipsKeepShortcutStatusAndFailureWithoutChangingButtonGeometry(string layout) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load(); await fixture.Layout(layout);
        var buttons = ((StackPanel)fixture.Window.FindName("KeyframeButtons")).Children.OfType<Button>().ToArray();
        var before = buttons.Select(b => (b.Height, b.Width, b.Margin, b.Padding, b.Style, b.RenderSize)).ToArray();
        Assert.Contains("Vorheriger Keyframe (Strg + ←)", buttons[0].ToolTip.ToString());
        Assert.Contains("Nächster Keyframe (Strg + →)", buttons[1].ToolTip.ToString());
        foreach (var button in buttons)
        {
            Assert.Contains(fixture.Keys.Status, button.ToolTip.ToString()); Assert.True(ToolTipService.GetShowOnDisabled(button));
        }
        fixture.Player.SeekError = new InvalidOperationException("Testfehler beim Keyframe-Sprung");
        await fixture.Window.TryHandleKeyframeShortcutAsync(fixture.Key(Key.Right), ModifierKeys.Control)!;
        await Dispatcher.Yield(DispatcherPriority.DataBind);
        foreach (var button in buttons) Assert.Contains("Testfehler beim Keyframe-Sprung", button.ToolTip.ToString());
        await fixture.Load("mp4", fail: true); await Dispatcher.Yield(DispatcherPriority.DataBind);
        foreach (var button in buttons)
        {
            Assert.False(button.IsEnabled); Assert.Contains(fixture.Keys.Status, button.ToolTip.ToString());
        }
        Assert.Equal(before, buttons.Select(b => (b.Height, b.Width, b.Margin, b.Padding, b.Style, b.RenderSize)).ToArray());
    });

    [Theory]
    [InlineData("Standard")]
    [InlineData("PowerUser")]
    [InlineData("PowerUser2")]
    public Task SecondsJumpAndPlayerTooltipsUseTheSameShortcutsInEveryLayout(string layout) => Sta(async () =>
    {
        await using var fixture = new Fixture();
        await fixture.Load();
        await fixture.Layout(layout);
        var window = fixture.Window;
        var player = fixture.Player;
        player.Duration = TimeSpan.FromSeconds(200);
        player.Position = TimeSpan.FromSeconds(100);
        fixture.Playback.JumpSecondsText = "5";

        foreach (var (buttonName, shortcut) in new[]
        {
            ("PlayPauseButton", "Leertaste"), ("StepBackwardButton", "←"), ("StepForwardButton", "→"),
            ("JumpBackwardButton", "S + ←"), ("JumpForwardButton", "S + →")
        })
        {
            var button = (Button)window.FindName(buttonName);
            Assert.Contains(shortcut, Assert.IsType<string>(button.ToolTip));
        }

        Assert.False(window.TryHandleSecondsJumpKey(fixture.Key(Key.Right), ModifierKeys.None));
        Assert.False(window.TryHandleSecondsJumpKey(fixture.Key(Key.F), ModifierKeys.Shift, isDown: key => key == Key.S));
        Assert.False(window.TryHandleSecondsJumpKey(fixture.Key(Key.Space), ModifierKeys.None, isDown: key => key == Key.S));
        Assert.False(window.TryHandleSecondsJumpKey(fixture.Key(Key.Left), ModifierKeys.Control, isDown: key => key == Key.S));
        Assert.True(window.TryHandleSecondsJumpKey(fixture.Key(Key.Left), ModifierKeys.None, isDown: key => key == Key.S));
        Assert.Equal(TimeSpan.FromSeconds(95), player.Position);

        var controls = (Grid)window.FindName("PlaybackControls");
        controls.IsEnabled = false;
        await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
        Assert.Single(player.SeekPositions);
    });

    [Theory]
    [InlineData("text")]
    [InlineData("richtext")]
    [InlineData("editable_combo")]
    [InlineData("combo")]
    [InlineData("password")]
    public Task SecondsJumpDoesNotClaimArrowsInInputFields(string kind) => Sta(async () =>
    {
        await using var fixture = new Fixture();
        await fixture.Load();
        DependencyObject input = kind switch
        {
            "text" => new TextBox(),
            "richtext" => new RichTextBox(),
            "password" => new PasswordBox(),
            _ => new ComboBox { IsEditable = kind == "editable_combo" }
        };

        foreach (var key in new[] { Key.Left, Key.Right })
        {
            var args = fixture.Key(key, input);
            Assert.False(fixture.Window.TryHandleSecondsJumpKey(args, ModifierKeys.None, isDown: key => key == Key.S));
            Assert.False(args.Handled);
        }
        Assert.Empty(fixture.Player.SeekPositions);
    });

    [Theory]
    [InlineData(0x51, Key.Q)]
    [InlineData(0x53, Key.S)]
    [InlineData(0xDE, Key.Oem7)] // German Ä
    [InlineData(0xC0, Key.Oem3)] // German Ö
    [InlineData(0xBA, Key.Oem1)] // German Ü
    public Task DialogCapturesWindowsKeyAndPlayerExecutesTheBinding(int virtualKey, Key expectedKey) => Sta(async () =>
    {
        await using var fixture = new Fixture();
        await fixture.Load();
        var key = KeyInterop.KeyFromVirtualKey(virtualKey);
        Assert.Equal(expectedKey, key);

        var dialog = new ShortcutSettingsDialog(new PlaybackShortcuts());
        try
        {
            var rows = (DataGrid)dialog.FindName("Rows");
            rows.SelectedIndex = PlaybackShortcuts.Definitions.ToList().FindIndex(d => d.Action == PlaybackShortcut.FrameForward);
            typeof(ShortcutSettingsDialog).GetMethod("Capture_Click", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(dialog, [dialog, new RoutedEventArgs()]);
            var captured = fixture.Key(key, dialog);
            dialog.RaiseEvent(captured);
            Assert.True(captured.Handled);
            dialog.RaiseEvent(fixture.KeyUp(key, dialog));
            Assert.Equal(new ShortcutGesture(key), dialog.Shortcuts[PlaybackShortcut.FrameForward]);
            fixture.Set("_shortcuts", dialog.Shortcuts);

            var input = new TextBox();
            var textKey = fixture.Key(key, input);
            await fixture.Window.HandlePlaybackKeyDownAsync(textKey, ModifierKeys.None, isDown: pressed => pressed == key);
            await fixture.Window.HandlePlaybackKeyUpAsync(fixture.KeyUp(key, input), ModifierKeys.None);
            Assert.False(textKey.Handled);
            Assert.Equal(0, fixture.Player.StepForwardCallCount);

            var playerKey = fixture.Key(key);
            await fixture.Window.HandlePlaybackKeyDownAsync(playerKey, ModifierKeys.None, isDown: pressed => pressed == key);
            Assert.True(playerKey.Handled);
            if (key == Key.S)
            {
                Assert.Equal(0, fixture.Player.StepForwardCallCount);
                await fixture.Window.HandlePlaybackKeyUpAsync(fixture.KeyUp(key), ModifierKeys.None);
            }
            Assert.Equal(1, fixture.Player.StepForwardCallCount);
        }
        finally { dialog.Close(); }
    });


    [Fact]
    public Task DialogCapturesUWithArrowAndPlayerKeepsTextInputPriority() => Sta(async () =>
    {
        await using var fixture = new Fixture();
        await fixture.Load();
        var dialog = new ShortcutSettingsDialog(new PlaybackShortcuts());
        try
        {
            var rows = (DataGrid)dialog.FindName("Rows");
            rows.SelectedIndex = PlaybackShortcuts.Definitions.ToList().FindIndex(d => d.Action == PlaybackShortcut.SecondsForward);
            typeof(ShortcutSettingsDialog).GetMethod("Capture_Click", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(dialog, [dialog, new RoutedEventArgs()]);
            dialog.RaiseEvent(fixture.Key(Key.U, dialog));
            dialog.RaiseEvent(fixture.Key(Key.Escape, dialog));
            dialog.RaiseEvent(fixture.KeyUp(Key.U, dialog));
            Assert.Equal(new ShortcutGesture(Key.Right, Prefix: Key.S), dialog.Shortcuts[PlaybackShortcut.SecondsForward]);

            typeof(ShortcutSettingsDialog).GetMethod("Capture_Click", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(dialog, [dialog, new RoutedEventArgs()]);
            dialog.RaiseEvent(fixture.Key(Key.U, dialog));
            Assert.Equal(new ShortcutGesture(Key.Right, Prefix: Key.S), dialog.Shortcuts[PlaybackShortcut.SecondsForward]);
            dialog.RaiseEvent(fixture.Key(Key.Right, dialog));
            dialog.RaiseEvent(fixture.KeyUp(Key.U, dialog));
            Assert.Equal(new ShortcutGesture(Key.Right, Prefix: Key.U), dialog.Shortcuts[PlaybackShortcut.SecondsForward]);
            fixture.Set("_shortcuts", dialog.Shortcuts);
            fixture.Playback.JumpSecondsText = "2";

            var textArrow = fixture.Key(Key.Right, new TextBox());
            await fixture.Window.HandlePlaybackKeyDownAsync(textArrow, ModifierKeys.None, key => key == Key.U);
            Assert.False(textArrow.Handled);
            Assert.Empty(fixture.Player.SeekPositions);

            var playerArrow = fixture.Key(Key.Right);
            await fixture.Window.HandlePlaybackKeyDownAsync(playerArrow, ModifierKeys.None, key => key == Key.U);
            await fixture.Window.HandlePlaybackKeyUpAsync(fixture.KeyUp(Key.U), ModifierKeys.None);
            await fixture.Window.HandlePlaybackKeyUpAsync(fixture.KeyUp(Key.Right), ModifierKeys.None);
            Assert.True(playerArrow.Handled);
            Assert.Equal(TimeSpan.FromSeconds(5), fixture.Player.SeekPositions.Single());
        }
        finally { dialog.Close(); }
    });

    [Theory]
    [InlineData(Key.S)]
    [InlineData(Key.U)]
    public Task SecondsChordSuppressesStandalonePrefixAndControlArrowStillJumpsToKeyframe(Key prefix) => Sta(async () =>
    {
        await using var fixture = new Fixture();
        await fixture.Load();
        var shortcuts = new PlaybackShortcuts();
        Assert.Null(shortcuts.Assign(PlaybackShortcut.FrameForward, new(prefix)));
        Assert.Null(shortcuts.Assign(PlaybackShortcut.SecondsForward, new(Key.Right, Prefix: prefix)));
        fixture.Set("_shortcuts", shortcuts);
        fixture.Playback.JumpSecondsText = "2";

        await fixture.Window.HandlePlaybackKeyDownAsync(fixture.Key(prefix), ModifierKeys.None);
        var arrow = fixture.Key(Key.Right);
        await fixture.Window.HandlePlaybackKeyDownAsync(arrow, ModifierKeys.None, isDown: key => key == prefix);
        await fixture.Window.HandlePlaybackKeyUpAsync(fixture.KeyUp(prefix), ModifierKeys.None);
        await fixture.Window.HandlePlaybackKeyUpAsync(fixture.KeyUp(Key.Right), ModifierKeys.None);
        Assert.True(arrow.Handled);
        Assert.Equal(TimeSpan.FromSeconds(5), fixture.Player.SeekPositions.Single());
        Assert.Equal(0, fixture.Player.StepForwardCallCount);

        var controlArrow = fixture.Key(Key.Left);
        await fixture.Window.HandlePlaybackKeyDownAsync(controlArrow, ModifierKeys.Control);
        Assert.True(controlArrow.Handled);
        Assert.Equal(TimeSpan.FromSeconds(4), fixture.Player.SeekPositions.Last());
    });

    [Fact]
    public Task ShiftFIsHandledByThePlayerButNotInsideAnInputField() => Sta(async () =>
    {
        await using var fixture = new Fixture();
        fixture.Set("_shortcuts", new PlaybackShortcuts());
        var textKey = fixture.Key(Key.F, new TextBox());
        await fixture.Window.HandlePlaybackKeyDownAsync(textKey, ModifierKeys.Shift);
        Assert.False(textKey.Handled);
        var playerKey = fixture.Key(Key.F);
        await fixture.Window.HandlePlaybackKeyDownAsync(playerKey, ModifierKeys.Shift);
        Assert.True(playerKey.Handled);
    });

    [Theory]
    [InlineData("Standard", CutEditingMode.Remove)]
    [InlineData("Standard", CutEditingMode.Keep)]
    [InlineData("PowerUser", CutEditingMode.Remove)]
    [InlineData("PowerUser", CutEditingMode.Keep)]
    [InlineData("PowerUser2", CutEditingMode.Remove)]
    [InlineData("PowerUser2", CutEditingMode.Keep)]
    public Task FourCutShortcutsUseExistingButtonsInEveryLayoutAndMode(string layout, CutEditingMode mode) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load(); await fixture.Layout(layout);
        var plan = fixture.Field<CutPlanViewModel>("_cutPlanViewModel");
        plan.Initialize(TimeSpan.FromSeconds(10), mode);
        var shortcuts = new PlaybackShortcuts();
        foreach (var (action, key) in new[] { (PlaybackShortcut.CutStart, Key.F6), (PlaybackShortcut.CutEnd, Key.F7),
            (PlaybackShortcut.VideoStart, Key.F8), (PlaybackShortcut.VideoEnd, Key.F9) })
            Assert.Null(shortcuts.Assign(action, new(key)));
        fixture.Set("_shortcuts", shortcuts);
        async Task Press(Key key)
        {
            await Dispatcher.Yield(DispatcherPriority.DataBind);
            var args = fixture.Key(key);
            await fixture.Window.HandlePlaybackKeyDownAsync(args, ModifierKeys.None);
            Assert.True(args.Handled);
        }
        await Press(Key.F6); Assert.Equal(TimeSpan.FromSeconds(3), plan.PendingStart);
        fixture.Player.Position = TimeSpan.FromSeconds(4);
        await Press(Key.F7);
        var first = Assert.Single(plan.MarkedSegments);
        Assert.Equal(TimeSpan.FromSeconds(3), first.Start); Assert.Equal(TimeSpan.FromSeconds(4), first.End);
        plan.Initialize(TimeSpan.FromSeconds(10), mode);
        await Press(Key.F8); Assert.Equal(TimeSpan.Zero, plan.PendingStart);
        await Press(Key.F9);
        var full = Assert.Single(plan.MarkedSegments); Assert.Equal(TimeSpan.Zero, full.Start); Assert.Equal(TimeSpan.FromSeconds(10), full.End);
        Assert.Empty(fixture.Player.SeekPositions); Assert.Equal(mode, plan.EditingMode);
    });

    [Theory]
    [InlineData(Key.F6)]
    [InlineData(Key.F7)]
    [InlineData(Key.F8)]
    [InlineData(Key.F9)]
    public Task CutShortcutsRespectTextInputsAndDoNotRepeat(Key key) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load();
        var plan = fixture.Field<CutPlanViewModel>("_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(10));
        plan.SetStart(TimeSpan.FromSeconds(1));
        var shortcuts = new PlaybackShortcuts();
        var action = key switch
        {
            Key.F6 => PlaybackShortcut.CutStart,
            Key.F7 => PlaybackShortcut.CutEnd,
            Key.F8 => PlaybackShortcut.VideoStart,
            _ => PlaybackShortcut.VideoEnd
        };
        Assert.Null(shortcuts.Assign(action, new(key))); fixture.Set("_shortcuts", shortcuts);
        foreach (var input in new DependencyObject[] { new TextBox(), new RichTextBox(), new PasswordBox(), new ComboBox { IsEditable = true } })
        {
            var args = fixture.Key(key, input);
            await fixture.Window.HandlePlaybackKeyDownAsync(args, ModifierKeys.None); Assert.False(args.Handled);
        }
        var repeat = fixture.Key(key);
        typeof(KeyEventArgs).GetField("_isRepeat", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(repeat, true);
        Assert.True(repeat.IsRepeat);
        await fixture.Window.HandlePlaybackKeyDownAsync(repeat, ModifierKeys.None);
        Assert.True(repeat.Handled); Assert.Equal(TimeSpan.FromSeconds(1), plan.PendingStart); Assert.Empty(plan.MarkedSegments);
        fixture.Window.IsEnabled = false;
        await fixture.Window.HandlePlaybackKeyDownAsync(fixture.Key(key), ModifierKeys.None);
        Assert.Equal(TimeSpan.FromSeconds(1), plan.PendingStart); Assert.Empty(plan.MarkedSegments);
    });

    [Theory]
    [InlineData("Standard", CutEditingMode.Remove)]
    [InlineData("Standard", CutEditingMode.Keep)]
    [InlineData("PowerUser", CutEditingMode.Remove)]
    [InlineData("PowerUser", CutEditingMode.Keep)]
    [InlineData("PowerUser2", CutEditingMode.Remove)]
    [InlineData("PowerUser2", CutEditingMode.Keep)]
    public Task FactoryCutGesturesExecuteAfterReloadInAllLayoutsAndModes(string layout, CutEditingMode mode) => Sta(async () =>
    {
        await using var fixture = new Fixture(); await fixture.Load(); await fixture.Layout(layout);
        var plan = fixture.Field<CutPlanViewModel>("_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(10), mode);
        var directory = Directory.CreateTempSubdirectory("can-default-keys-");
        try
        {
            var path = Path.Combine(directory.FullName, "shortcuts.json");
            new PlaybackShortcuts().Save(path); fixture.Set("_shortcuts", PlaybackShortcuts.Load(path));
            async Task Press(Key key, ModifierKeys modifiers)
            {
                await Dispatcher.Yield(DispatcherPriority.DataBind);
                var args = fixture.Key(key);
                await fixture.Window.HandlePlaybackKeyDownAsync(args, modifiers);
                Assert.True(args.Handled);
            }
            await Press(Key.A, ModifierKeys.None); Assert.Equal(TimeSpan.FromSeconds(3), plan.PendingStart);
            fixture.Player.Position = TimeSpan.FromSeconds(4);
            await Press(Key.E, ModifierKeys.None);
            Assert.Equal(TimeSpan.FromSeconds(3), Assert.Single(plan.MarkedSegments).Start);
            Assert.Equal(TimeSpan.FromSeconds(4), Assert.Single(plan.MarkedSegments).End);
            plan.Initialize(TimeSpan.FromSeconds(10), mode);
            await Press(Key.A, ModifierKeys.Shift); Assert.Equal(TimeSpan.Zero, plan.PendingStart);
            await Press(Key.E, ModifierKeys.Shift);
            Assert.Equal(TimeSpan.Zero, Assert.Single(plan.MarkedSegments).Start);
            Assert.Equal(TimeSpan.FromSeconds(10), Assert.Single(plan.MarkedSegments).End);
            Assert.Empty(fixture.Player.SeekPositions);
        }
        finally { directory.Delete(true); }
    });

    private sealed class Fixture : IAsyncDisposable
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private readonly HwndSource _source = new(new HwndSourceParameters("CAN Keyframe Shortcut Tests") { Width = 1, Height = 1, WindowStyle = 0 });
        public MainWindow Window { get; } = new();
        public PlaybackViewModelTests.StubMediaPlayerService Player { get; } = new()
        { State = MediaPlayerState.Paused, Duration = TimeSpan.FromSeconds(10), Position = TimeSpan.FromSeconds(3) };
        public PlaybackViewModel Playback { get; }
        public KeyframeNavigationViewModel Keys { get; }
        public Fixture()
        {
            Field<KeyframeNavigationViewModel>("_keyframeNavigation").Dispose();
            Field<PlaybackViewModel>("_playbackViewModel").Dispose();
            Playback = new(Player); Keys = new(Playback);
            Set("_playbackViewModel", Playback); Set("_keyframeNavigation", Keys);
            ((Grid)Window.FindName("PlaybackControls")).DataContext = Playback;
            ((Grid)Window.FindName("PlaybackControls")).IsEnabled = true;
            ((StackPanel)Window.FindName("KeyframeButtons")).DataContext = Keys;
        }
        public T Field<T>(string name) => (T)typeof(MainWindow).GetField(name, Private)!.GetValue(Window)!;
        public void Set(string name, object value) => typeof(MainWindow).GetField(name, Private)!.SetValue(Window, value);
        public KeyEventArgs Key(Key key, DependencyObject? source = null) => new(Keyboard.PrimaryDevice, _source, 0, key)
        { RoutedEvent = Keyboard.PreviewKeyDownEvent, Source = source ?? Window };
        public KeyEventArgs KeyUp(Key key, DependencyObject? source = null) => new(Keyboard.PrimaryDevice, _source, 0, key)
        { RoutedEvent = Keyboard.PreviewKeyUpEvent, Source = source ?? Window };
        public Task Load(string format = "mp4", bool fail = false) => Keys.LoadAsync("synthetic.mp4",
            new(format, null, null, TimeSpan.FromSeconds(10), [new VideoStreamInfo(0, "h264", null, 320, 180, null, null, 25, null)], []), new Runner(fail));
        public async Task Layout(string name)
        {
            Set("_preferredLayout", Enum.Parse<InterfaceLayout>(name));
            var content = (FrameworkElement)Window.Content;
            for (var i = 0; i < 3; i++)
            {
                content.Measure(new Size(1900, 1000)); content.Arrange(new Rect(0, 0, 1900, 1000));
                content.UpdateLayout(); Window.ApplyInterfaceLayout(); await Dispatcher.Yield(DispatcherPriority.ContextIdle);
            }
        }
        public async ValueTask DisposeAsync()
        {
            Window.Closing -= (CancelEventHandler)typeof(MainWindow).GetMethod("MainWindow_Closing", Private)!.CreateDelegate(typeof(CancelEventHandler), Window);
            Field<PlayerFrameSearchViewModel>("_playerFrameSearch").Dispose(); Keys.Dispose(); Playback.Dispose();
            await Field<IMediaPlayerService>("_mediaPlayerService").DisposeAsync();
            Field<CancellationTokenSource>("_mediaInputCancellation").Dispose(); Window.Close(); _source.Dispose();
        }
    }
    private sealed class Runner(bool fail) : IKeyframeAnalysisRunner
    {
        public Task<VideoFrameWindow> RunKeyframesAsync(string path, int stream, CancellationToken token = default) => fail
            ? Task.FromException<VideoFrameWindow>(new InvalidOperationException("Testfehler bei der Analyse"))
            : Task.FromResult(new VideoFrameWindow(0, new(1, 1000), 0, 0,
                new[] { 0L, 2000, 4000, 8000 }.Select((pts, i) => new VideoFrameInfo(i, pts, null, null, null, true, "I")).ToArray()));
    }
    private static Task Sta(Func<Task> action)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
            dispatcher.InvokeAsync(async () =>
            {
                try { await action(); completion.SetResult(); }
                catch (Exception error) { completion.SetException(error); }
                finally { dispatcher.InvokeShutdown(); }
            });
            Dispatcher.Run();
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); return completion.Task;
    }
}
