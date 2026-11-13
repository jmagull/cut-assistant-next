using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;
using CanWindow = CutAssistantNext.App.MainWindow;
using SpotWindow = SpotlightCan.App.MainWindow;

namespace SpotlightIntegrationChecks;
internal static partial class Program
{
    private static readonly List<string> Checks = [];
    private static readonly Assembly CanAssembly = typeof(CanWindow).Assembly;
    private static string _output = "";
    private static CanWindow? _window;
    private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    [STAThread]
    public static int Main(string[] args)
    {
        if (args is ["--main-eof", var media, var mode, var output, ..] && args.Length <= 5)
            return RunMainEof(media, mode, output, args.Length == 5 ? args[4] : null);
        if(args.Length != 4) { Console.Error.WriteLine("Usage: media.mp4 ffmpeg.exe ffprobe.exe output-directory"); return 2; }
        _output=Path.GetFullPath(args[3]); Directory.CreateDirectory(_output);
        var app=new Application { ShutdownMode=ShutdownMode.OnExplicitShutdown };
        app.Startup += async (_,_) =>
        {
            Exception? failure=null;
            try { await RunAsync(Path.GetFullPath(args[0]),Path.GetFullPath(args[1]),Path.GetFullPath(args[2])); }
            catch(Exception error) { failure=error; }
            finally
            {
                foreach(var child in app.Windows.OfType<SpotWindow>().ToArray()) await child.CloseFromHostAsync();
                if(_window is { } root)
                {
                    var closed=new TaskCompletionSource(); root.Closed+=(_,_)=>closed.TrySetResult(); root.Close();
                    await closed.Task.WaitAsync(TimeSpan.FromSeconds(20));
                }
                await File.WriteAllTextAsync(Path.Combine(_output,"results.json"),JsonSerializer.Serialize(new { passed=failure is null, count=Checks.Count, checks=Checks,error=failure?.ToString() },new JsonSerializerOptions{WriteIndented=true}));
                Console.WriteLine(failure?.ToString() ?? $"PASS {Checks.Count} native integration checks");
                app.Shutdown(failure is null ? 0 : 1);
            }
        };
        return app.Run();
    }
    private static void Check(bool condition,string text) { if(!condition) throw new InvalidOperationException(text); Checks.Add(text); Console.WriteLine("PASS "+text); }
    private static T Field<T>(object target,string name)=>(T)target.GetType().GetField(name,Private)!.GetValue(target)!;
    private static void Set(object target,string name,object value)=>target.GetType().GetField(name,Private)!.SetValue(target,value);
    private static async Task Until(Func<bool> ready)
    {
        var until=DateTime.UtcNow.AddSeconds(15);
        while(!ready()) { if(DateTime.UtcNow>until) throw new TimeoutException("WPF operation did not complete"); await Task.Delay(25); }
    }
    private static async Task KeyframeShortcutAsync(Key key)
    {
        var args=new KeyEventArgs(Keyboard.PrimaryDevice,PresentationSource.FromVisual(_window!),0,key)
            { RoutedEvent=Keyboard.PreviewKeyDownEvent,Source=_window };
        var jump=(Task?)typeof(CanWindow).GetMethod("TryHandleKeyframeShortcutAsync",Private)!.Invoke(_window,[args,ModifierKeys.Control]);
        Check(jump is not null && args.Handled,"Control arrow uses existing keyframe route: "+key);
        if(jump is not null) await jump;
    }
    private static async Task CapturePlayerGeometryAsync()
    {
        var entries = new List<object>();
        var layoutType=CanAssembly.GetType("CutAssistantNext.App.Settings.InterfaceLayout")!;
        foreach (var layout in new[] { "Standard", "PowerUser", "PowerUser2" })
        {
            Set(_window!, "_preferredLayout", Enum.Parse(layoutType, layout));
            _window!.WindowState=WindowState.Normal; _window.Width=1900; _window.Height=1000;
            typeof(CanWindow).GetMethod("ApplyInterfaceLayout",Private)!.Invoke(_window,null);
            _window.UpdateLayout(); await Dispatcher.Yield(DispatcherPriority.ContextIdle); _window.UpdateLayout();
            var controls=(Grid)_window.FindName("PlaybackControls");
            var video=(FrameworkElement)_window.FindName("VideoHost");
            var buttons=LogicalButtons(controls).ToArray();
            var keyButtons=((StackPanel)_window.FindName("KeyframeButtons")).Children.OfType<Button>().ToArray();
            Check(keyButtons[0].ToolTip.ToString()!.Contains("Vorheriger Keyframe (Strg + ←)") &&
                keyButtons[1].ToolTip.ToString()!.Contains("Nächster Keyframe (Strg + →)"),"Keyframe shortcut tooltips in layout: "+layout);
            entries.Add(new { layout, playerWidth=video.ActualWidth, playerHeight=video.ActualHeight,
                rowHeight=controls.RowDefinitions[1].ActualHeight,
                buttons=buttons.Select(b=>new { name=b.Name, text=b.Content?.ToString(),height=b.ActualHeight,
                    x=b.TransformToAncestor(controls).Transform(new Point()).X,
                    y=b.TransformToAncestor(controls).Transform(new Point()).Y }).ToArray() });
            Capture(_window,"player-"+layout+".png");
        }
        await File.WriteAllTextAsync(Path.Combine(_output,"player-geometry.json"),JsonSerializer.Serialize(entries,new JsonSerializerOptions{WriteIndented=true}));
    }
    private static async Task CheckSchnittshuffleAsync(CutPlanViewModel editor, PlaybackViewModel playback,
        IMediaPlayerService player, TimeSpan duration)
    {
        editor.Initialize(duration);
        editor.SetStart(TimeSpan.FromSeconds(8));
        editor.SetEnd(duration);
        var segment = editor.MarkedSegments.Single();
        editor.SelectEdge(segment, CutEdgeSide.Start);
        await Dispatcher.Yield(DispatcherPriority.ContextIdle);
        var group = (StackPanel)_window!.FindName("SchnittshuffleControls");
        var table = (DataGrid)_window.FindName("RemoveSegmentsGrid");
        var slider = (Slider)_window.FindName("TimelineSlider");
        var left = (Button)_window.FindName("SchnittshuffleStartButton");
        var right = (Button)_window.FindName("SchnittshuffleEndButton");
        Check(group.IsVisible && ReferenceEquals(table.SelectedItem, segment),
            "Schnittshuffle appears for the selected edit in Power-User 2");

        left.Focus();
        left.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Until(() => Math.Abs(player.Position.TotalSeconds - 8) < .002 && !playback.IsSeeking);
        Check(Math.Abs(slider.Value - 8) < .002 && ReferenceEquals(table.SelectedItem, segment),
            "Schnittshuffle start updates the real player and slider without deselection");

        await playback.PlayAsync();
        await Until(() => player.State == MediaPlayerState.Playing);
        right.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        var lastFrame = duration.TotalSeconds - .04;
        await Until(() => Math.Abs(player.Position.TotalSeconds - lastFrame) < .002 && !playback.IsSeeking);
        Check(player.State == MediaPlayerState.Paused && segment.End == duration &&
            Math.Abs(slider.Value - lastFrame) < .002,
            "Schnittshuffle reaches the final 25-fps picture while preserving the exclusive EOF cut edge");

        left.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Until(() => Math.Abs(player.Position.TotalSeconds - 8) < .002 && !playback.IsSeeking);
        Check(ReferenceEquals(table.SelectedItem, segment) && editor.SelectedEdgeSide == CutEdgeSide.Start,
            "Repeated real jumps preserve the row and the selected edit edge");
        Capture(_window, "schnittshuffle-native.png");
        table.SelectedItem = null;
        await Dispatcher.Yield(DispatcherPriority.ContextIdle);
        Check(group.Visibility == Visibility.Hidden, "Clearing the selection hides Schnittshuffle");
    }
    private static IEnumerable<Button> LogicalButtons(DependencyObject parent)
    {
        foreach(var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
            if(child is Button button) yield return button;
            else foreach(var nested in LogicalButtons(child)) yield return nested;
    }
    private static object ToolSettings(string ffmpeg,string ffprobe)
    {
        var type=CanAssembly.GetType("CutAssistantNext.App.Settings.FfmpegSettings")!; var settings=Activator.CreateInstance(type, nonPublic:true)!;
        type.GetProperty("FfmpegExecutablePath")!.SetValue(settings,ffmpeg); type.GetProperty("FfprobeExecutablePath")!.SetValue(settings,ffprobe); return settings;
    }
    private static async Task RunAsync(string media,string ffmpeg,string ffprobe)
    {
        _window=new CanWindow();
        var storeType=CanAssembly.GetType("CutAssistantNext.App.Settings.WindowSettingsStore")!;
        var store=Activator.CreateInstance(storeType,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,[Path.Combine(_output,"window-settings.json")],null)!;
        Set(_window,"_windowSettingsStore",store); // Isolate test window persistence; never save user settings.
        var spotStore=Activator.CreateInstance(storeType,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,[Path.Combine(_output,"spotlight-window-settings.json")],null)!;
        _window.Show(); await (Task)typeof(CanWindow).GetMethod("EnsureMediaPlayerInitializedAsync",Private)!.Invoke(_window,null)!;
        var view=Field<MainWindowViewModel>(_window,"_viewModel"); var editor=Field<CutPlanViewModel>(_window,"_cutPlanViewModel");
        Check(view.AnalysisResult is null,"CAN starts without a film");
        Check(((MenuItem)_window.FindName("SpotlightMenuItem")).Header.ToString()!.Contains("Spotlight-CAN"),"Werkzeuge menu offers Spotlight-CAN");
        var counter=new CountingAnalysis(new FfprobeRunner(ffprobe)); Set(view,"_mediaAnalysisRunner",counter);
        await view.AnalyzeAsync(media); Check(view.AnalysisResult is not null && !view.HasError,"CAN media analysis succeeds");
        var player=Field<IMediaPlayerService>(_window,"_mediaPlayerService"); await player.SetVolumeAsync(0); await player.LoadAsync(media); await Until(() => player.Duration > TimeSpan.Zero || player.State == MediaPlayerState.Error); await player.PauseAsync();
        Check(player.Duration>TimeSpan.Zero,"CAN existing libmpv player loads the source");
        var keys=typeof(CanWindow).GetField("_keyframeNavigation",Private)!.GetValue(_window)!;
        await (Task)keys.GetType().GetMethod("LoadAsync")!.Invoke(keys,[media,view.AnalysisResult!,new FfprobeFrameRunner(ffprobe)])!;
        Check(((StackPanel)_window.FindName("KeyframeButtons")).Visibility==Visibility.Visible,"Existing keyframe controls remain available");
        editor.Initialize(view.MediaDuration!.Value); editor.SetStart(TimeSpan.FromSeconds(2)); editor.SetEnd(TimeSpan.FromSeconds(3));
        var originalCuts=editor.CreateCutPlanSnapshot().RemoveSegments.Select(s=>(s.Start,s.End)).ToArray();
        var playback=Field<PlaybackViewModel>(_window,"_playbackViewModel");
        var seekStates=new System.Collections.Concurrent.ConcurrentQueue<MediaPlayerState>();
        EventHandler seekStateChanged=(_,_)=>seekStates.Enqueue(player.State);
        player.StateChanged+=seekStateChanged;
        await player.SeekAsync(TimeSpan.FromSeconds(1)); playback.JumpSecondsText="5";
        ((Button)_window.FindName("JumpForwardButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Until(()=>Math.Abs(player.Position.TotalSeconds-6)<0.08);
        Check(player.State==MediaPlayerState.Paused,"CAN seconds button seeks and preserves pause");
        Check(seekStates.All(s=>s==MediaPlayerState.Paused),"Paused seconds seek has no transient Playing state");
        seekStates.Clear();
        await player.PlayAsync();
        ((Button)_window.FindName("JumpBackwardButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Until(()=>player.Position.TotalSeconds<2);
        Check(player.State==MediaPlayerState.Playing,"CAN seconds button preserves playback");
        Check(seekStates.All(s=>s==MediaPlayerState.Playing),"Playing seconds seek has no transient Paused state");
        player.StateChanged-=seekStateChanged;
        await player.PauseAsync();
        var position=player.Position;
        ((Button)_window.FindName("StepForwardButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Until(()=>player.Position>position);
        Check(Math.Abs((player.Position-position).TotalSeconds-.04)<.002,"Existing single-frame forward action remains exact");
        var next=player.Position;
        ((Button)_window.FindName("StepBackwardButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await Until(()=>player.Position<next);
        Check(Math.Abs((next-player.Position).TotalSeconds-.04)<.002,"Existing single-frame backward action remains exact");
        var editable=(ComboBox)_window.FindName("JumpSecondsPicker");
        var arrow=new KeyEventArgs(Keyboard.PrimaryDevice,PresentationSource.FromVisual(_window),0,Key.Left)
            { RoutedEvent=Keyboard.PreviewKeyDownEvent,Source=editable };
        _window.RaiseEvent(arrow);
        Check(!arrow.Handled,"Editable seconds dropdown keeps its arrow keys");
        Check(originalCuts.SequenceEqual(editor.CreateCutPlanSnapshot().RemoveSegments.Select(s=>(s.Start,s.End))),"Navigation does not move cut positions");
        await KeyframeShortcutAsync(Key.Right);
        Check(player.Position==TimeSpan.FromSeconds(10),"Existing next-keyframe navigation remains exact");
        await KeyframeShortcutAsync(Key.Left);
        Check(player.Position==TimeSpan.Zero,"Existing previous-keyframe navigation remains exact");
        await CapturePlayerGeometryAsync();
        await CheckSchnittshuffleAsync(editor, playback, player, view.MediaDuration!.Value);
        var settings=ToolSettings(ffmpeg,ffprobe);
        foreach(var mode in new[]{CutEditingMode.Remove,CutEditingMode.Keep})
        {
            editor.Initialize(view.MediaDuration!.Value,mode);
            var opening=(Task)typeof(CanWindow).GetMethod("OpenSpotlightAsync",Private)!.Invoke(_window,[null,settings,spotStore])!;
            await Until(()=>Application.Current.Windows.OfType<SpotWindow>().Any(w=>w.IsVisible));
            var spot=Application.Current.Windows.OfType<SpotWindow>().Single(w=>w.IsVisible);
            Check(spot.Model.Media!.FilePath==media && spot.Model.Media.Duration==view.MediaDuration && spot.Model.Media.VideoStreamIndex==view.AnalysisResult!.VideoStreams[0].Index,"Host reuses CAN media: "+mode);
            Check(counter.Calls==1 && spot.Model.FfmpegPath==ffmpeg && spot.Model.FfprobePath==ffprobe,"Host uses CAN paths without another media probe: "+mode);
            Check(((Button)spot.FindName("VideoOpenButton")).Visibility==Visibility.Collapsed && ((Grid)spot.FindName("ToolPathsPanel")).Visibility==Visibility.Collapsed,"Integrated input controls removed: "+mode);
            Check(!(bool)typeof(CanWindow).GetProperty("CanOpenMedia",Private)!.GetValue(_window)!,"Source replacement blocked while Spotlight is open: "+mode);
            var writeBlocked=false; try { using var write=new FileStream(media,FileMode.Open,FileAccess.Write,FileShare.ReadWrite); } catch(IOException) {writeBlocked=true;}
            Check(writeBlocked,"Source file protected while open: "+mode);
            await spot.Model.OverviewAsync(0); await spot.Model.DetailsAsync();
            Check(spot.Model.Overview.Count>0 && spot.Model.Details.Count>0,"Real overview and detail thumbnails: "+mode);
            Check(spot.FindName("JumpPicker") is null,"No seconds controls in Spotlight: "+mode);
            var eofButton=(Button)spot.FindName("CompleteToVideoEndButton");
            Check(!eofButton.IsEnabled,"EOF button disabled without pending start: "+mode);
            Add(spot,"00:00:00.000","00:00:01.000"); Add(spot,"00:00:04.000","00:00:05.000"); spot.Model.PositionText="00:00:11.000"; spot.Model.MarkStart();
            await Dispatcher.Yield(DispatcherPriority.DataBind);
            Check(eofButton.IsEnabled,"EOF button enabled with pending start: "+mode);
            eofButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Check(!spot.Model.HasPendingStart && spot.Model.Blocks.Last().Start==TimeSpan.FromSeconds(11) && spot.Model.Blocks.Last().End==spot.Model.Media.Duration,
                "Open start completes to exact EOF without selecting another image: "+mode);
            Check(spot.Model.RemovedDurationText=="00:00:03.000" && spot.Model.RemainingDurationText=="00:00:09.000","Runtime totals show complete removal ranges: "+mode);
            ((ListBox)spot.FindName("DetailsList")).SelectedIndex=2;
            await Dispatcher.Yield(DispatcherPriority.ContextIdle); Capture(spot,"spotlight-"+mode+".png");
            ((Button)spot.FindName("ExportActionButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await opening;
            Check(editor.CreateCutPlanSnapshot().RemoveSegments.Count==3 && editor.EstimatedOutputDuration==TimeSpan.FromSeconds(9),"Direct complete transfer in CAN mode: "+mode);
            Check(ReferenceEquals(((FrameworkElement)_window.FindName("CutTimelineTrack")).DataContext,editor) && editor.MarkedSegments.Count==(mode==CutEditingMode.Keep?2:3),"Shared table and timeline reflect transferred plan: "+mode);
            var selected=editor.MarkedSegments[0]; editor.SelectedRemoveSegment=selected; editor.SetEnd(selected.End-TimeSpan.FromMilliseconds(40));
            Check(editor.MarkedSegments[0].End==selected.End-TimeSpan.FromMilliseconds(40),"Existing CAN boundary correction remains usable: "+mode);
            var keeps=CutlistKeepSegmentBuilder.Build(editor.CreateCutPlanSnapshot());
            var document=new CutlistDocument(new CutlistGeneralMetadata{NoOfCuts=keeps.Count,ApplyToFile=Path.GetFileName(media)},keeps,new CutlistInfoMetadata());
            var cutlist=Path.Combine(_output,"can-"+mode+".cutlist"); CutlistFileWriter.Write(cutlist,document);
            editor.LoadCutPlan(CutlistCutPlanBuilder.Build(CutlistFileReader.Read(cutlist),view.MediaDuration.Value));
            Check(editor.MarkedSegments[0].End==selected.End-TimeSpan.FromMilliseconds(40),"CAN cutlist save/read roundtrip: "+mode);
        }
        var before=editor.CreateCutPlanSnapshot().RemoveSegments.Select(s=>(s.Start,s.End)).ToArray(); var consent=false; var confirmations=0;
        var reopen=(Task)typeof(CanWindow).GetMethod("OpenSpotlightAsync",Private)!.Invoke(_window,[new Func<bool>(()=>{confirmations++;return consent;}),settings,spotStore])!;
        await Until(()=>Application.Current.Windows.OfType<SpotWindow>().Any(w=>w.IsVisible));
        var reopened=Application.Current.Windows.OfType<SpotWindow>().Single(w=>w.IsVisible);
        Check(reopened.Model.Blocks.Count==0,"Reopen starts a clean Spotlight session");
        Add(reopened,"00:00:03.000","00:00:06.000");
        ((Button)reopened.FindName("ExportActionButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Check(confirmations==1 && reopened.IsVisible && before.SequenceEqual(editor.CreateCutPlanSnapshot().RemoveSegments.Select(s=>(s.Start,s.End))),"Declined replacement keeps CAN and Spotlight unchanged");
        consent=true; ((Button)reopened.FindName("ExportActionButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); await reopen;
        Check(confirmations==2 && editor.CreateCutPlanSnapshot().RemoveSegments.Single().Start==TimeSpan.FromSeconds(3),"Explicit replacement commits the complete new plan");
        using(var unlocked=new FileStream(media,FileMode.Open,FileAccess.Write,FileShare.ReadWrite)) Check(true,"Source file lease released after window close");
        var finalOpen=(Task)typeof(CanWindow).GetMethod("OpenSpotlightAsync",Private)!.Invoke(_window,[new Func<bool>(()=>false),settings,spotStore])!;
        await Until(()=>Application.Current.Windows.OfType<SpotWindow>().Any(w=>w.IsVisible)); var final=Application.Current.Windows.OfType<SpotWindow>().Single(w=>w.IsVisible);
        var generate=final.Model.OverviewAsync(0); final.Model.Cancel(); await generate;
        Check(final.Model.Status.Contains("abgebrochen"),"Integrated thumbnail generation is cancellable");
        await final.CloseFromHostAsync(); await finalOpen;
        Check(editor.CreateCutPlanSnapshot().RemoveSegments.Single().Start==TimeSpan.FromSeconds(3),"Closing without transfer preserves CAN cuts");
        Capture(_window,"can-after-transfer.png");
    }
    private static void Add(SpotWindow window,string start,string end) { window.Model.PositionText=start;window.Model.MarkStart();window.Model.PositionText=end;window.Model.MarkEnd(); }
    private static void Capture(Window window,string name)
    {
        window.UpdateLayout();var image=new RenderTargetBitmap((int)window.ActualWidth,(int)window.ActualHeight,96,96,PixelFormats.Pbgra32);image.Render(window);
        var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(image));using var file=File.Create(Path.Combine(_output,name));encoder.Save(file);
    }
    private sealed class CountingAnalysis(IMediaAnalysisRunner inner):IMediaAnalysisRunner
    {
        internal int Calls;
        public Task<MediaAnalysisResult> RunAsync(string path,CancellationToken token=default) { Calls++;return inner.RunAsync(path,token); }
    }
}
