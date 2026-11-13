using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
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
