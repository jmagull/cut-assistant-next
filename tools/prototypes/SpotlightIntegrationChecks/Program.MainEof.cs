using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;
using CanWindow = CutAssistantNext.App.MainWindow;

namespace SpotlightIntegrationChecks;

internal static partial class Program
{
    private static int RunMainEof(string media, string mode, string output, string? ffprobe)
    {
        Directory.CreateDirectory(output);
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var logger = new EofLogger();
        var trace = new List<string>();
        app.Startup += async (_, _) =>
        {
            Exception? failure = null;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
            timer.Tick += (_, _) =>
            {
                var handle = GetActiveWindow();
                var title = new StringBuilder(100);
                GetWindowText(handle, title, title.Capacity);
                if (title.ToString() != "Wiedergabefehler") return;
                trace.Add("Playback dialog was open and the dispatcher timer was still running");
                PostMessage(handle, 0x111, 1, 0); // WM_COMMAND / IDOK
            };
            try
            {
                _window = (CanWindow)Activator.CreateInstance(typeof(CanWindow), Private, null,
                    [logger, Array.Empty<string>(), null], null)!;
                var storeType = CanAssembly.GetType("CutAssistantNext.App.Settings.WindowSettingsStore")!;
                var store = Activator.CreateInstance(storeType, BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic, null, [Path.Combine(output, "window-settings.json")], null)!;
                Set(_window, "_windowSettingsStore", store);
                _window.Show();
                timer.Start();
                await CallMainAsync("EnsureMediaPlayerInitializedAsync");
                await TraceMainEofAsync(Path.GetFullPath(media), mode, trace, ffprobe);
            }
            catch (Exception error) { failure = error; }
            finally
            {
                timer.Stop();
                File.WriteAllText(Path.Combine(output, "results.json"), JsonSerializer.Serialize(
                    new { Version = FileVersionInfo.GetVersionInfo(CanAssembly.Location).FileVersion, Mode = mode, Trace = trace,
                        Checks, Error = failure?.ToString(), Messages = logger.Messages.ToArray() },
                    new JsonSerializerOptions { WriteIndented = true }));
                if (_window is { } window)
                {
                    var closed = new TaskCompletionSource();
                    window.Closed += (_, _) => closed.TrySetResult();
                    window.Close();
                    await closed.Task.WaitAsync(TimeSpan.FromSeconds(20));
                }
                app.Shutdown(failure is null ? 0 : 1);
            }
        };
        return app.Run();
    }

    private static async Task TraceMainEofAsync(string media, string mode, List<string> trace, string? ffprobe)
    {
        var window = _window!;
        var verify = mode.StartsWith("verify-", StringComparison.Ordinal);
        var playback = Field<PlaybackViewModel>(window, "_playbackViewModel");
        var player = Field<IMediaPlayerService>(window, "_mediaPlayerService");
        var slider = (Slider)window.FindName("TimelineSlider");
        var play = (Button)window.FindName("PlayPauseButton");
        var editor = Field<CutPlanViewModel>(window, "_cutPlanViewModel");

        var client = Field<object>(player, "_client");
        async Task<bool> AtEof() => await (Task<string?>)client.GetType()
            .GetMethod("GetStringPropertyAsync")!.Invoke(client, ["eof-reached", CancellationToken.None])! == "yes";

        async Task WaitForEof()
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (!await AtEof() || player.State != MediaPlayerState.Paused)
                await Task.Delay(20, deadline.Token);
        }

        async Task Snapshot(string name)
        {
            await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
            var tracks = await ((IAudioTrackSelection)player).GetAudioTracksAsync();
            var eof = await AtEof();
            trace.Add(FormattableString.Invariant(
                $"{name}: {player.State}, pos={player.Position.TotalSeconds:F3}, slider={slider.Value:F3}, seek={slider.IsEnabled}, play={play.IsEnabled}, tracks={tracks.Count}, eof={eof}, error={player.ErrorMessage}"));
        }

        async Task Slide(double position)
        {
            trace.Add(FormattableString.Invariant($"Slider target: {position:F3}"));
            if (!slider.IsEnabled) return;
            playback.BeginSeek(); // Same entry point as clicking the slider thumb.
            Set(window, "_timelineSliderDragging", true);
            slider.SetCurrentValue(Slider.ValueProperty, position);
            await CallMainAsync("TimelineSlider_PreviewMouseLeftButtonUp", slider,
                new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
            await Until(() => !playback.IsSeeking);
        }

        void Click(Button button) => button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        if (verify)
        {
            await CallMainAsync("ExecutePlaybackActionAsync", (Func<Task>)(() => player.SeekAsync(TimeSpan.FromSeconds(1))));
            var status = (TextBlock)window.FindName("PlaybackActionStatus");
            Check(status.IsVisible && player.State == MediaPlayerState.Empty && player.ErrorMessage is null,
                "Unavailable native time-pos is reported inline without changing player state");
        }
        await player.LoadAsync(media);
        await Until(() => player.State == MediaPlayerState.Paused && player.Duration > TimeSpan.Zero);
        await player.SetVolumeAsync(0);
        var duration = player.Duration!.Value;
        await Snapshot("Opened");

        if (verify)
        {
            var probe = Path.GetFullPath(ffprobe ?? throw new ArgumentException("Pass the ffprobe path in verify mode."));
            var view = Field<MainWindowViewModel>(window, "_viewModel");
            Set(view, "_mediaAnalysisRunner", new FfprobeRunner(probe));
            await view.AnalyzeAsync(media);
            var keys = Field<object>(window, "_keyframeNavigation");
            await (Task)keys.GetType().GetMethod("LoadAsync")!.Invoke(keys,
                [media, view.AnalysisResult!, new FfprobeFrameRunner(probe)])!;
            await VerifyMainEofAsync();
            return;
        }

        if (mode == "shuffle")
        {
            editor.Initialize(duration);
            editor.SetStart(TimeSpan.FromSeconds(8));
            editor.SetEnd(duration);
            editor.SelectEdge(editor.MarkedSegments.Single(), CutAssistantNext.Core.Editing.CutEdgeSide.Start);
            await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
            Click((Button)window.FindName("SchnittshuffleEndButton"));
            await Until(() => !playback.IsSeeking && Math.Abs(player.Position.TotalSeconds - (duration.TotalSeconds - .04)) < .002);
            await Snapshot("Schnittshuffle at EOF cut edge");
            await Slide(8);
            await Snapshot("Slider back before Play");
        }
        else
        {
            await Slide(duration.TotalSeconds);
            await Task.Delay(250);
            await Snapshot("Slider at full duration");
        }

        if (play.IsEnabled) Click(play);
        await Task.Delay(400);
        await Snapshot("After Play/Pause");
        if (player.State == MediaPlayerState.Playing)
        {
            await Until(() => player.State != MediaPlayerState.Playing || player.Position.TotalSeconds >= 9);
            if (play.IsEnabled) Click(play);
            await Task.Delay(80);
        }

        await Slide(3);
        await Snapshot("Slider back after Play/Pause");

        if (player.State is MediaPlayerState.Paused or MediaPlayerState.Playing)
        {
            if (player.State == MediaPlayerState.Playing) await player.PauseAsync();
            await Slide(duration.TotalSeconds - .12);
            await Until(() => !playback.IsSeeking && player.Position.TotalSeconds > duration.TotalSeconds - .2);
            if (play.IsEnabled) Click(play);
            await Until(() => player.State == MediaPlayerState.Ended || player.State == MediaPlayerState.Error);
            await Snapshot("Natural EOF");
            if (play.IsEnabled) Click(play);
            await Snapshot("Play at actual EOF");
            await Slide(4);
            await Snapshot("Slider back from actual EOF");
        }

        async Task VerifyMainEofAsync()
        {
            async Task WaitAt(double target)
            {
                await Until(() => !playback.IsSeeking && Math.Abs(player.Position.TotalSeconds - target) < .002 &&
                    Math.Abs(slider.Value - target) < .002);
            }

            async Task PlayPauseAt(double target)
            {
                await Slide(target);
                await WaitAt(target);
                await CallMainAsync("ExecutePlaybackActionAsync", (Func<Task>)(() => playback.TogglePlaybackAsync()));
                await Until(() => player.State == MediaPlayerState.Playing && player.Position.TotalSeconds > target + .04);
                await CallMainAsync("ExecutePlaybackActionAsync", (Func<Task>)(() => playback.TogglePlaybackAsync()));
                await Until(() => player.State == MediaPlayerState.Paused);
            }

            await PlayPauseAt(0);
            await PlayPauseAt(5);
            Check(((TextBlock)window.FindName("PlaybackActionStatus")).Visibility == Visibility.Collapsed,
                "Play/Pause at the beginning and in the middle works and clears the prior hint");

            await Slide(duration.TotalSeconds - .12);
            await WaitAt(duration.TotalSeconds - .12);
            await CallMainAsync("ExecutePlaybackActionAsync", (Func<Task>)(() => playback.TogglePlaybackAsync()));
            await WaitForEof();
            await Snapshot("Natural EOF");
            for (var i = 0; i < 8; i++) Click(play);
            await Task.Delay(250);
            Check(await AtEof() && player.State == MediaPlayerState.Paused && slider.IsEnabled &&
                (await ((IAudioTrackSelection)player).GetAudioTracksAsync()).Count > 0,
                "Rapid Play/Pause at actual EOF stays paused and keeps the seekable file loaded");

            await Slide(3);
            await WaitAt(3);
            Check(!await AtEof(), "Slider returns from actual EOF to an earlier position");
            await PlayPauseAt(3);
            Click((Button)window.FindName("StepForwardButton"));
            await Until(() => player.State == MediaPlayerState.Paused && player.Position.TotalSeconds > 3.08);
            playback.JumpSecondsText = "1";
            Click((Button)window.FindName("JumpForwardButton"));
            await Until(() => player.Position.TotalSeconds > 4 && !playback.IsSeeking);
            Check(slider.IsEnabled, "Slider, Play and subsequent frame/seconds navigation work after EOF");
            await Slide(4.4);
            await WaitAt(4.4);
            await KeyframeShortcutAsync(Key.Right);
            await WaitAt(10);
            Check(player.State == MediaPlayerState.Paused, "Keyframe navigation works after actual EOF recovery");

            if (mode == "verify-shuffle")
            {
                editor.Initialize(duration);
                editor.SetStart(TimeSpan.FromSeconds(8));
                editor.SetEnd(duration);
                editor.SelectEdge(editor.MarkedSegments.Single(), CutAssistantNext.Core.Editing.CutEdgeSide.Start);
                var table = (DataGrid)window.FindName("RemoveSegmentsGrid");
                var selected = table.SelectedItem;
                var right = (Button)window.FindName("SchnittshuffleEndButton");
                var left = (Button)window.FindName("SchnittshuffleStartButton");
                for (var i = 0; i < 3; i++)
                {
                    Click(right);
                    await WaitAt(duration.TotalSeconds - .04);
                    await Slide(8);
                    await WaitAt(8);
                    await PlayPauseAt(8);
                    Click(left);
                    await WaitAt(8);
                    Check(selected is not null && ReferenceEquals(table.SelectedItem, selected) &&
                        editor.SelectedEdgeSide == CutAssistantNext.Core.Editing.CutEdgeSide.Start &&
                        editor.MarkedSegments.Single().End == duration,
                        $"Schnittshuffle round {i + 1}: EOF edge, slider back, Play, navigation and selection");
                }
                Click(right);
                await WaitAt(duration.TotalSeconds - .04);
                Click(play);
                await WaitForEof();
                await Slide(8);
                await WaitAt(8);
                Check(ReferenceEquals(table.SelectedItem, selected),
                    "Actual EOF after playing from Schnittshuffle preserves the row and slider recovery");
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    await Slide(duration.TotalSeconds);
                    await WaitAt(duration.TotalSeconds - .04);
                    await Slide(0);
                    await WaitAt(0);
                }
                Check(true, "Three repeated slider seeks between the beginning and the file end");
            }

            Check(player.ErrorMessage is null && !trace.Any(line => line.Contains("dialog was open")),
                "No modal playback error or disabled player in the EOF workflows");
        }
    }

    private static Task CallMainAsync(string method, params object[] args) =>
        typeof(CanWindow).GetMethod(method, Private)!.Invoke(_window, args) as Task ?? Task.CompletedTask;

    [DllImport("user32.dll")]
    private static extern nint GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(nint window, StringBuilder text, int size);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(nint window, uint message, nint wParam, nint lParam);

    private sealed class EofLogger : IAppLogger
    {
        public System.Collections.Concurrent.ConcurrentQueue<string> Messages { get; } = new();
        public void Information(string message) => Messages.Enqueue(message);
        public void Warning(string message) => Messages.Enqueue(message);
        public void Error(string message, Exception? exception = null) => Messages.Enqueue($"{message} {exception}");
    }
}
