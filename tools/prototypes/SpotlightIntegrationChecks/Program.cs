using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CutAssistantNext.App.ViewModels;
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
internal static class Program
{
    private static readonly List<string> Checks = [];
    private static readonly Assembly CanAssembly = typeof(CanWindow).Assembly;
    private static string _output = "";
    private static CanWindow? _window;
    private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    [STAThread]
    public static int Main(string[] args)
    {
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
        _window.Show(); await (Task)typeof(CanWindow).GetMethod("EnsureMediaPlayerInitializedAsync",Private)!.Invoke(_window,null)!;
        var view=Field<MainWindowViewModel>(_window,"_viewModel"); var editor=Field<CutPlanViewModel>(_window,"_cutPlanViewModel");
        Check(view.AnalysisResult is null,"CAN starts without a film");
        Check(((MenuItem)_window.FindName("SpotlightMenuItem")).Header.ToString()!.Contains("Spotlight-CAN"),"Werkzeuge menu offers Spotlight-CAN");
        var counter=new CountingAnalysis(new FfprobeRunner(ffprobe)); Set(view,"_mediaAnalysisRunner",counter);
        await view.AnalyzeAsync(media); Check(view.AnalysisResult is not null && !view.HasError,"CAN media analysis succeeds");
        var player=Field<IMediaPlayerService>(_window,"_mediaPlayerService"); await player.SetVolumeAsync(0); await player.LoadAsync(media); await Until(() => player.Duration > TimeSpan.Zero || player.State == MediaPlayerState.Error); await player.PauseAsync();
        Check(player.Duration>TimeSpan.Zero,"CAN existing libmpv player loads the source");
        var settings=ToolSettings(ffmpeg,ffprobe);
        foreach(var mode in new[]{CutEditingMode.Remove,CutEditingMode.Keep})
        {
            editor.Initialize(view.MediaDuration!.Value,mode);
            var opening=(Task)typeof(CanWindow).GetMethod("OpenSpotlightAsync",Private)!.Invoke(_window,[null,settings])!;
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
            Add(spot,"00:00:00.000","00:00:01.000"); Add(spot,"00:00:04.000","00:00:05.000"); Add(spot,"00:00:11.000",spot.Model.BoundaryPositions[1]);
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
        var reopen=(Task)typeof(CanWindow).GetMethod("OpenSpotlightAsync",Private)!.Invoke(_window,[new Func<bool>(()=>{confirmations++;return consent;}),settings])!;
        await Until(()=>Application.Current.Windows.OfType<SpotWindow>().Any(w=>w.IsVisible));
        var reopened=Application.Current.Windows.OfType<SpotWindow>().Single(w=>w.IsVisible);
        Check(reopened.Model.Blocks.Count==0,"Reopen starts a clean Spotlight session");
        Add(reopened,"00:00:03.000","00:00:06.000");
        ((Button)reopened.FindName("ExportActionButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Check(confirmations==1 && reopened.IsVisible && before.SequenceEqual(editor.CreateCutPlanSnapshot().RemoveSegments.Select(s=>(s.Start,s.End))),"Declined replacement keeps CAN and Spotlight unchanged");
        consent=true; ((Button)reopened.FindName("ExportActionButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); await reopen;
        Check(confirmations==2 && editor.CreateCutPlanSnapshot().RemoveSegments.Single().Start==TimeSpan.FromSeconds(3),"Explicit replacement commits the complete new plan");
        using(var unlocked=new FileStream(media,FileMode.Open,FileAccess.Write,FileShare.ReadWrite)) Check(true,"Source file lease released after window close");
        var finalOpen=(Task)typeof(CanWindow).GetMethod("OpenSpotlightAsync",Private)!.Invoke(_window,[new Func<bool>(()=>false),settings])!;
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
