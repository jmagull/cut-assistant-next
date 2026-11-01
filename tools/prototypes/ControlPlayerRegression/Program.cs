using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Media.Playback;

namespace ControlPlayerRegression;

// Separate process: an uncaught exception on the pinned wrapper's event thread must fail this run.
internal static class Program
{
    private static readonly List<string> Checks = [];
    private static string _report = "";
    private static string _media = "";
    private static readonly Logger Log = new();
    private static readonly Type ReviewType = typeof(CutCompletionViewModel).Assembly
        .GetType("CutAssistantNext.App.ViewModels.CutReviewViewModel", throwOnError: true)!;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 3) { Console.Error.WriteLine("Usage: ControlPlayerRegression regular|eof|property|two-player|lifetime|wpf media.mp4 report.json"); return 2; }
        _media = Path.GetFullPath(args[1]);
        _report = Path.GetFullPath(args[2]);
        Directory.CreateDirectory(Path.GetDirectoryName(_report)!);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => Save(false, e.ExceptionObject.ToString());
        var application = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        application.Startup += async (_, _) =>
        {
            try
            {
                switch (args[0])
                {
                    case "regular": await RegularSelectionAsync(); break;
                    case "eof": await EofAsync(); break;
                    case "property": await PropertyFailureAsync(); break;
                    case "two-player": await TwoPlayersAsync(); break;
                    case "lifetime": await LifetimeAsync(); break;
                    case "wpf": await WpfAsync(); break;
                    default: throw new ArgumentException("Unknown case");
                }
                Save(true, null);
                application.Shutdown(0);
            }
            catch (Exception error) { Save(false, error.ToString()); application.Shutdown(1); }
        };
        return application.Run();
    }

    private static async Task RegularSelectionAsync()
    {
        using var host = Host();
        await using var player = new MpvMediaPlayerService(Log);
        var model = Model(player);
        var completion = Completion();
        try
        {
            await Call(model, "ReviewAsync", completion, 0, host.Handle);
            await player.SetVolumeAsync(0);
            await WaitAsync(() => player.Position.TotalSeconds >= 1.25);
            for (var round = 0; round < 4; round++)
            {
                var before = player.Position.TotalSeconds;
                var track = (await player.GetAudioTracksAsync()).Single(t => !t.Selected);
                await Call(model, "SelectAudioAsync", track);
                await Dispatcher.Yield(DispatcherPriority.Background);
                var after = player.Position.TotalSeconds;
                Check(Selected(model) == track.Id && player.State == MediaPlayerState.Playing &&
                    after >= before - .1 && after <= before + .35,
                    FormattableString.Invariant($"regular switch {round + 1} keeps playing position: {before:F3} -> {after:F3}"));
            }
            await player.PauseAsync();
            var paused = player.Position.TotalSeconds;
            var other = (await player.GetAudioTracksAsync()).Single(t => !t.Selected);
            await Call(model, "SelectAudioAsync", other);
            await Dispatcher.Yield(DispatcherPriority.Background);
            Check(player.State == MediaPlayerState.Paused && Math.Abs(player.Position.TotalSeconds - paused) < .1,
                FormattableString.Invariant($"paused switch keeps position and pause: {paused:F3} -> {player.Position.TotalSeconds:F3}"));
            await Call(model, "ReviewAsync", completion, 2, host.Handle);
            await player.PauseAsync();
            var middle = player.Position.TotalSeconds;
            await Call(model, "SelectAudioAsync", (await player.GetAudioTracksAsync()).Single(t => !t.Selected));
            Check(Value<string>(model, "CurrentTransitionText").StartsWith("Übergang 2") &&
                Math.Abs(player.Position.TotalSeconds - middle) < .1, "middle checkpoint and its preview position remain selected");
            await Call(model, "MoveAsync", 1, host.Handle);
            await WaitAsync(() => player.Position.TotalSeconds >= 4.8 && player.Position.TotalSeconds < 6);
            Check(player.State == MediaPlayerState.Playing, "explicit next checkpoint still starts three seconds before EOF");
        }
        finally { await ((IAsyncDisposable)model).DisposeAsync(); }
    }

    private static HwndSource Host() => new(new HwndSourceParameters("CAN regression HWND")
    { Width = 16, Height = 16, WindowStyle = unchecked((int)0x80000000) });

    private static object Model(IMediaPlayerService player) => Activator.CreateInstance(ReviewType,
        BindingFlags.Instance | BindingFlags.NonPublic, null, [player, null, null, Log], null)!;

    private static CutCompletionViewModel Completion() => new(new CutResult(_media,
        new(TimeSpan.FromSeconds(8), [TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)]), CutReviewTimingKind.Estimated));

    private static Task Call(object target, string method, params object[] args) =>
        (Task)target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, args)!;

    private static T Value<T>(object model, string name) => (T)model.GetType().GetProperty(name)!.GetValue(model)!;
    private static int? Selected(object model) => Value<PlayerAudioTrack?>(model, "SelectedAudioTrack")?.Id;

    private static void Check(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException(name);
        Checks.Add(name);
        Console.WriteLine("PASS " + name);
    }

    private static async Task WaitAsync(Func<bool> ready)
    {
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        while (!ready()) await Task.Delay(20, deadline.Token);
    }

    private static async Task<Exception> FailureAsync(Func<Task> action)
    {
        try { await action(); }
        catch (Exception error) { return error; }
        throw new InvalidOperationException("Expected an observable task exception");
    }

    private static async Task EofAsync()
    {
        using var host = Host();
        await using var player = new MpvMediaPlayerService(Log);
        var model = Model(player);
        try
        {
            await Call(model, "ReviewAsync", Completion(), 3, host.Handle);
            await player.SetVolumeAsync(0);
            var tracks = await player.GetAudioTracksAsync();
            Check(tracks.Count == 2, "two real audio tracks");
            var alternate = tracks.Single(t => !t.Selected);
            await WaitAsync(() => player.State == MediaPlayerState.Ended);
            Check((await player.GetAudioTracksAsync()).Count == 0, "EOF unloads native inventory");
            await player.PauseAsync();
            Check(player.State == MediaPlayerState.Ended, "pause preserves Ended");
            await Call(model, "SelectAudioAsync", alternate);
            Check(Selected(model) == alternate.Id && player.State == MediaPlayerState.Playing, "audio switch after EOF reloads and plays");
            Check(player.Position.TotalSeconds >= 4.8 && player.Position.TotalSeconds < 6, "end preview keeps three second start");
            await WaitAsync(() => player.State == MediaPlayerState.Ended);
            await Call(model, "MoveAsync", -1, host.Handle);
            Check(Selected(model) == alternate.Id, "ordinary EOF navigation retains selected audio");
            Check(player.Position.TotalSeconds >= 1.8 && player.Position.TotalSeconds < 3, "ordinary EOF navigation reloads and seeks");
            await Call(model, "ReviewAsync", Completion(), 3, host.Handle);
            await WaitAsync(() => player.State == MediaPlayerState.Ended);
            var error = await FailureAsync(() => Call(model, "SelectAudioAsync", new PlayerAudioTrack(999, "unavailable", false)));
            Check(error is ArgumentException, "unavailable audio becomes task error after EOF");
            Check(Value<string>(model, "CurrentTransitionText").StartsWith("Ende") && Value<bool>(model, "CanPrevious"), "failed switch retains last checkpoint and navigation");
            await Call(model, "MoveAsync", -1, host.Handle);
            Check(player.State == MediaPlayerState.Playing, "sfonweb error then previous survives and plays");
        }
        finally { await ((IAsyncDisposable)model).DisposeAsync(); }
    }

    private static async Task PropertyFailureAsync()
    {
        using var host = Host();
        await using var player = new MpvMediaPlayerService(Log);
        await player.InitializeAsync(host.Handle);
        var error = await FailureAsync(() => player.SeekAsync(TimeSpan.FromSeconds(1)));
        Check(error.GetType().Name == "MpvException" && error.Message.Contains("property unavailable"), "native property unavailable arrives at awaiting task");
        await player.LoadAsync(_media);
        await WaitAsync(() => player.State == MediaPlayerState.Paused && player.Duration > TimeSpan.Zero);
        await player.SetVolumeAsync(0);
        await player.SeekAsync(TimeSpan.FromSeconds(1));
        await player.PlayAsync();
        await WaitAsync(() => player.Position.TotalSeconds > 1.2);
        await player.PauseAsync();
        Check(player.State == MediaPlayerState.Paused && player.ErrorMessage is null, "load pause seek play recover after real native error");
        await player.SetVolumeAsync(29);
        Check(player.Volume == 29, "volume setter still works");
    }

    private static async Task TwoPlayersAsync()
    {
        using var mainHost = Host();
        using var reviewHost = Host();
        await using var main = new MpvMediaPlayerService(Log);
        await using var review = new MpvMediaPlayerService(Log);
        await main.InitializeAsync(mainHost.Handle);
        await main.LoadAsync(_media);
        await WaitAsync(() => main.State == MediaPlayerState.Paused && main.Duration > TimeSpan.Zero);
        await main.SetVolumeAsync(0);
        await main.SeekAsync(TimeSpan.FromSeconds(1));
        await WaitAsync(() => Math.Abs(main.Position.TotalSeconds - 1) < .1);
        var before = main.Position;
        var mainAudio = (await main.GetAudioTracksAsync()).Single(t => t.Selected).Id;
        var model = Model(review);
        try
        {
            await Call(model, "ReviewAsync", Completion(), 3, reviewHost.Handle);
            await review.SetVolumeAsync(0);
            var other = (await review.GetAudioTracksAsync()).Single(t => !t.Selected);
            await WaitAsync(() => review.State == MediaPlayerState.Ended);
            await Call(model, "SelectAudioAsync", other);
            await Call(model, "MoveAsync", -1, reviewHost.Handle);
            Check(mainHost.Handle != reviewHost.Handle && main.Position == before && main.State == MediaPlayerState.Paused,
                "review EOF selection and navigation leave main HWND position and state unchanged");
            Check((await main.GetAudioTracksAsync()).Single(t => t.Selected).Id == mainAudio, "review audio does not change main audio");
        }
        finally { await ((IAsyncDisposable)model).DisposeAsync(); }
        await main.PlayAsync();
        await WaitAsync(() => main.Position > before + TimeSpan.FromSeconds(.2));
        await main.PauseAsync();
        await main.SeekAsync(TimeSpan.FromSeconds(2));
        Check(main.State == MediaPlayerState.Paused && main.ErrorMessage is null, "main play pause seek work after review disposal");
    }

    private static async Task LifetimeAsync()
    {
        using var host = Host();
        await using var player = new MpvMediaPlayerService(Log);
        await player.InitializeAsync(host.Handle);
        var client = typeof(MpvMediaPlayerService).GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(player)!;
        var gate = (SemaphoreSlim)client.GetType().GetField("_propertyGate", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(client)!;
        Task Set(double value, CancellationToken token) => (Task)client.GetType().GetMethod("SetDoublePropertyAsync")!.Invoke(client, ["volume", value, token])!;
        await gate.WaitAsync();
        using var cancelled = new CancellationTokenSource();
        var queued = Set(0, cancelled.Token);
        cancelled.Cancel();
        Check(await FailureAsync(() => queued) is OperationCanceledException, "queued native property operation honours cancellation");
        var pending = Set(0, CancellationToken.None);
        var closing = player.DisposeAsync().AsTask();
        Check(!pending.IsCompleted && !closing.IsCompleted, "dispose waits for property ownership");
        gate.Release();
        await pending;
        await closing;
        Check(await FailureAsync(() => Set(0, CancellationToken.None)) is ObjectDisposedException, "property cannot access disposed native context");
        await player.DisposeAsync();
        Check(true, "repeated disposal is safe");
    }

    private static async Task WpfAsync()
    {
        var player = new FaultPlayer(new MpvMediaPlayerService(Log));
        var windowType = typeof(CutCompletionViewModel).Assembly.GetType("CutAssistantNext.App.Dialogs.CutReviewWindow", true)!;
        var window = (Window)Activator.CreateInstance(windowType, BindingFlags.Instance | BindingFlags.NonPublic,
            null, [player, Log], null)!;
        window.Opacity = 0;
        window.ShowActivated = false;
        window.ShowInTaskbar = false;
        try
        {
            window.Show();
            await Call(window, "ReviewAsync", Completion(), 2);
            await player.SetVolumeAsync(0);
            var model = window.DataContext;
            var audio = (ComboBox)window.FindName("ReviewAudio");
            var error = (TextBlock)window.FindName("ErrorText");
            var tracks = Value<IReadOnlyList<PlayerAudioTrack>>(model, "AudioTracks");
            var alternate = tracks.Single(t => t.Id != Selected(model));
            audio.SetCurrentValue(Selector.SelectedItemProperty, alternate);
            await WaitAsync(() => Value<bool>(model, "IsIdle") && Selected(model) == alternate.Id);
            Check(((PlayerAudioTrack)audio.SelectedItem).Id == alternate.Id && error.Text.Length == 0, "WPF successful selection binds confirmed native ID");
            player.RejectAudio = true;
            audio.SetCurrentValue(Selector.SelectedItemProperty, Value<IReadOnlyList<PlayerAudioTrack>>(model, "AudioTracks").Single(t => t.Id != alternate.Id));
            await WaitAsync(() => Value<bool>(model, "IsIdle") && error.Text.Length > 0);
            await Dispatcher.Yield(DispatcherPriority.Background);
            Check(((PlayerAudioTrack)audio.SelectedItem).Id == Selected(model) && Selected(model) == alternate.Id,
                "WPF failed selection restores actual native selection without recursive switch");
            player.RejectAudio = false;
            ((Button)window.FindName("PreviousButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await WaitAsync(() => Value<bool>(model, "IsIdle") && Value<string>(model, "CurrentTransitionText").StartsWith("Übergang 1"));
            Check(error.Text.Length == 0 && player.State == MediaPlayerState.Playing, "WPF previous after audio error recovers");
            ((Button)window.FindName("NextButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await WaitAsync(() => Value<bool>(model, "IsIdle") && Value<string>(model, "CurrentTransitionText").StartsWith("Übergang 2"));
            Check(player.State == MediaPlayerState.Playing, "WPF next after recovery works");
            player.BlockAudio = true;
            audio.SetCurrentValue(Selector.SelectedItemProperty, Value<IReadOnlyList<PlayerAudioTrack>>(model, "AudioTracks").Single(t => t.Id != Selected(model)));
            await WaitAsync(() => !Value<bool>(model, "IsIdle"));
            await Call(window, "CloseSafelyAsync");
            Check(player.Disposed && !window.IsVisible, "WPF close cancels pending selection and disposes native player");
        }
        finally { await Call(window, "CloseSafelyAsync"); }
    }

    private static void Save(bool passed, string? error)
    {
        lock (Log)
            File.WriteAllText(_report, JsonSerializer.Serialize(new { Passed = passed, Checks, Error = error, Log.Messages }, new JsonSerializerOptions { WriteIndented = true }));
    }

    private sealed class Logger : IAppLogger
    {
        public List<string> Messages { get; } = [];
        public void Information(string message) { lock (this) Messages.Add("INFO " + message); }
        public void Warning(string message) { lock (this) Messages.Add("WARN " + message); }
        public void Error(string message, Exception? exception = null) { lock (this) Messages.Add("ERROR " + message + " " + exception); }
    }

    private sealed class FaultPlayer(MpvMediaPlayerService inner) : IMediaPlayerService, IAudioTrackSelection
    {
        public bool RejectAudio { get; set; }
        public bool BlockAudio { get; set; }
        public bool Disposed { get; private set; }
        public MediaPlayerState State => inner.State;
        public TimeSpan Position => inner.Position;
        public TimeSpan? Duration => inner.Duration;
        public long? FrameNumber => inner.FrameNumber;
        public long? EstimatedFrameCount => inner.EstimatedFrameCount;
        public double Volume => inner.Volume;
        public string? ErrorMessage => inner.ErrorMessage;
        public event EventHandler? StateChanged { add => inner.StateChanged += value; remove => inner.StateChanged -= value; }
        public event EventHandler? PositionChanged { add => inner.PositionChanged += value; remove => inner.PositionChanged -= value; }
        public event EventHandler? DurationChanged { add => inner.DurationChanged += value; remove => inner.DurationChanged -= value; }
        public event EventHandler? FrameChanged { add => inner.FrameChanged += value; remove => inner.FrameChanged -= value; }
        public event EventHandler? VolumeChanged { add => inner.VolumeChanged += value; remove => inner.VolumeChanged -= value; }
        public event EventHandler? ErrorOccurred { add => inner.ErrorOccurred += value; remove => inner.ErrorOccurred -= value; }
        public Task InitializeAsync(nint handle, CancellationToken token = default) => inner.InitializeAsync(handle, token);
        public Task LoadAsync(string path, CancellationToken token = default) => inner.LoadAsync(path, token);
        public Task PlayAsync(CancellationToken token = default) => inner.PlayAsync(token);
        public Task PauseAsync(CancellationToken token = default) => inner.PauseAsync(token);
        public Task SeekAsync(TimeSpan position, CancellationToken token = default) => inner.SeekAsync(position, token);
        public Task StopAsync(CancellationToken token = default) => inner.StopAsync(token);
        public Task SetVolumeAsync(double volume, CancellationToken token = default) => inner.SetVolumeAsync(volume, token);
        public Task StepForwardAsync(CancellationToken token = default) => inner.StepForwardAsync(token);
        public Task StepBackwardAsync(CancellationToken token = default) => inner.StepBackwardAsync(token);
        public Task StepFramesAsync(int frames, CancellationToken token = default) => inner.StepFramesAsync(frames, token);
        public Task<IReadOnlyList<PlayerAudioTrack>> GetAudioTracksAsync(CancellationToken token = default) => inner.GetAudioTracksAsync(token);
        public async Task SelectAudioTrackAsync(int id, CancellationToken token = default)
        {
            if (BlockAudio) await Task.Delay(Timeout.Infinite, token);
            if (RejectAudio) throw new ArgumentException("Gezielt abgelehnte Tonspur.", nameof(id));
            await inner.SelectAudioTrackAsync(id, token);
        }
        public async ValueTask DisposeAsync() { await inner.DisposeAsync(); Disposed = true; }
    }
}
