using System.IO;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Settings;
using SpotlightWindow = SpotlightCan.App.MainWindow;

namespace CutAssistantNext.App.Tests.Settings;

[Collection("WpfUi")]
public sealed class SpotlightWindowPersistenceTests
{
    [Fact]
    public void ActualSpotlightCloseAndReopenKeepsResizedAndMovedBounds() => OnSta(path =>
    {
        var first = Open(path);
        first.Width = 1150; first.Height = 820; first.Left = 90; first.Top = 70;
        Pump();
        var expected = NativeWindowGeometry.GetBounds(new WindowInteropHelper(first).Handle)!.Value;
        Close(first);
        var saved = new WindowSettingsStore(path).Load()!;
        AssertRect(expected, new(saved.Left!.Value, saved.Top!.Value, saved.Width, saved.Height));
        var second = Open(path);
        var available = NativeWindowGeometry.GetMonitors();
        var restored = SpotlightWindowSettings.Restore(saved, available, available[0].Bounds, 1320, 920, 1000, 800);
        AssertRect(restored.Bounds, NativeWindowGeometry.GetBounds(new WindowInteropHelper(second).Handle)!.Value);
        Close(second);
    });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MaximizedAndThenMinimizedKeepNormalReturnGeometry(bool minimize) => OnSta(path =>
    {
        var first = Open(path);
        first.Width = 1100; first.Height = 820; first.Left = 100; first.Top = 80;
        Pump();
        var normal = NativeWindowGeometry.GetBounds(new WindowInteropHelper(first).Handle)!.Value;
        first.WindowState = WindowState.Maximized; Pump();
        if (minimize) { first.WindowState = WindowState.Minimized; Pump(); }
        Close(first);
        var saved = new WindowSettingsStore(path).Load()!;
        Assert.True(saved.IsMaximized);
        AssertRect(normal, new(saved.Left!.Value, saved.Top!.Value, saved.Width, saved.Height));
        var second = Open(path);
        Assert.Equal(WindowState.Maximized, second.WindowState);
        second.WindowState = WindowState.Normal; Pump();
        AssertRect(normal, NativeWindowGeometry.GetBounds(new WindowInteropHelper(second).Handle)!.Value);
        Close(second);
    });

    [Fact]
    public void MinimizedNormalWindowReopensNormally() => OnSta(path =>
    {
        var first = Open(path);
        first.WindowState = WindowState.Minimized; Pump(); Close(first);
        Assert.False(new WindowSettingsStore(path).Load()!.IsMaximized);
        var second = Open(path);
        Assert.Equal(WindowState.Normal, second.WindowState);
        Close(second);
    });

    [Fact]
    public void RegularSpotlightCloseWaitsForCleanupAndSavesOnlyOnClosed() => OnSta(path =>
    {
        var window = Open(path);
        var closed = false;
        window.Closing += (_, _) => Assert.False(File.Exists(path));
        window.Closed += (_, _) => closed = true;
        window.Close();
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!closed && DateTime.UtcNow < deadline) Pump();
        Assert.True(closed);
        Assert.NotNull(new WindowSettingsStore(path).Load());
    });

    [Fact]
    public void EveryAvailableMonitorPreservesNativePosition() => OnSta(path =>
    {
        foreach (var monitor in NativeWindowGeometry.GetMonitors())
        {
            var saved = new WindowSettings
            {
                Left = monitor.Bounds.Left + 30,
                Top = monitor.Bounds.Top + 30,
                Width = 1100 * monitor.DpiScale,
                Height = 820 * monitor.DpiScale,
                DpiScale = monitor.DpiScale
            };
            Assert.True(new WindowSettingsStore(path).Save(saved));
            var window = Open(path);
            var expected = SpotlightWindowSettings.Restore(saved, [monitor], monitor.Bounds, 1320, 920, 1000, 800);
            AssertRect(expected.Bounds, NativeWindowGeometry.GetBounds(new WindowInteropHelper(window).Handle)!.Value);
            Close(window);
            var actual = new WindowSettingsStore(path).Load()!;
            AssertRect(expected.Bounds, new(actual.Left!.Value, actual.Top!.Value, actual.Width, actual.Height));
        }
    });

    [Fact]
    public void CancelledCloseDoesNotSaveAndFinalCloseCapturesLatestBounds() => OnSta(path =>
    {
        var window = new Window { Width = 1100, Height = 820 };
        SpotlightWindowPersistence.Attach(window, new(path));
        System.ComponentModel.CancelEventHandler cancel = (_, e) => e.Cancel = true;
        window.Closing += cancel;
        window.Show(); Pump(); window.Close();
        Assert.False(File.Exists(path));
        window.Closing -= cancel;
        window.Left = 120; window.Top = 90; Pump();
        var expected = NativeWindowGeometry.GetBounds(new WindowInteropHelper(window).Handle)!.Value;
        window.Close();
        var saved = new WindowSettingsStore(path).Load()!;
        AssertRect(expected, new(saved.Left!.Value, saved.Top!.Value, saved.Width, saved.Height));
    });

    private static SpotlightWindow Open(string path)
    {
        var window = new SpotlightWindow();
        SpotlightWindowPersistence.Attach(window, new(path));
        window.Show(); Pump();
        return window;
    }

    private static void Close(SpotlightWindow window)
    {
        var closing = window.CloseFromHostAsync();
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!closing.IsCompleted && DateTime.UtcNow < deadline) Pump();
        Assert.True(closing.IsCompleted, "Spotlight cleanup did not finish.");
        closing.GetAwaiter().GetResult(); Pump();
    }

    private static void AssertRect(Rect expected, Rect actual)
    {
        Assert.InRange(Math.Abs(expected.Left - actual.Left), 0, 2);
        Assert.InRange(Math.Abs(expected.Top - actual.Top), 0, 2);
        Assert.InRange(Math.Abs(expected.Width - actual.Width), 0, 2);
        Assert.InRange(Math.Abs(expected.Height - actual.Height), 0, 2);
    }

    private static void Pump()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static void OnSta(Action<string> action)
    {
        Exception? failure = null;
        var directory = Path.Combine(Path.GetTempPath(), "can-spotlight-window-" + Guid.NewGuid().ToString("N"));
        var thread = new Thread(() =>
        {
            try { Directory.CreateDirectory(directory); action(Path.Combine(directory, "window.json")); }
            catch (Exception error) { failure = error; }
            finally
            {
                foreach (var window in Dispatcher.CurrentDispatcher.Invoke(() =>
                    PresentationSource.CurrentSources.OfType<HwndSource>().Select(s => s.RootVisual).OfType<Window>().ToArray()))
                {
                    if (window is SpotlightWindow spot) Close(spot); else window.Close();
                }
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        })
        { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(25)), "Spotlight window test timed out.");
        try { if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw(); }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true); }
    }
}
