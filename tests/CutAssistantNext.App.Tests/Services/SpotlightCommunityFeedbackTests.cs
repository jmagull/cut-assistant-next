using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CutAssistantNext.Cutlists.IO;
using SpotlightCan.App;
using SpotlightCan.Core;
using SpotlightCan.Media;
using SpotlightWindow = SpotlightCan.App.MainWindow;
using Model = SpotlightCan.App.MainViewModel;

namespace CutAssistantNext.App.Tests.Services;

[Collection("WpfUi")]
public sealed class SpotlightCommunityFeedbackTests
{
    [Theory]
    [InlineData("00:00:00.000", "00:00:20.000")]
    [InlineData("00:00:15.1234567", "00:00:20.1234567")]
    [InlineData("00:00:19.9999999", "00:00:20.000")]
    [InlineData("02:00:03.560", "02:09:58.000")]
    public Task OpenStartCompletesAtExactEofWithoutAnotherImageOrProbe(string startText, string durationText) => OnSta(async () =>
    {
        var model = await Open(durationText);
        try
        {
            var start = TimeText.Parse(startText); var duration = TimeText.Parse(durationText);
            model.SelectImage(Card(start), false); model.MarkStart();
            Assert.True(model.CanCompleteToVideoEnd);
            // EOF must use the pending start, independent of the current selection or editable position.
            model.PositionText = "kein weiteres Bild";
            model.CompleteToVideoEnd();
            Assert.Equal(new(start, duration), model.Blocks.Single());
            Assert.False(model.HasPendingStart); Assert.False(model.CanCompleteToVideoEnd);
            Assert.Equal(duration - start, model.Runtime.Removed); Assert.Equal(start, model.Runtime.Remaining);
            Assert.Equal(model.Blocks, model.CreateTransferSnapshot());
            if (startText == "02:00:03.560") Assert.Equal(TimeText.Parse("00:09:54.440"), model.Runtime.Removed);
        }
        finally { await model.CloseAsync(); }
    });

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task WithoutPendingStartNoAreaIsCreatedOrModified(bool existing) => OnSta(async () =>
    {
        var model = await Open();
        try
        {
            if (existing) Add(model, 2, 4);
            var before = model.Blocks.ToArray(); var dirty = model.Dirty;
            model.PositionText = "00:00:15.000"; model.CompleteToVideoEnd();
            Assert.False(model.CanCompleteToVideoEnd); Assert.False(model.HasPendingStart);
            Assert.Equal(before, model.Blocks); Assert.Equal(dirty, model.Dirty); Assert.Contains("Beginn setzen", model.Status);
        }
        finally { await model.CloseAsync(); }
    });

    [Fact]
    public Task CompletedAdsAndEofAreaShareListDurationAndNormalDeletion() => OnSta(async () =>
    {
        var model = await Open();
        try
        {
            Add(model, 2, 4); Add(model, 8, 10); var before = model.Blocks.ToArray();
            model.PositionText = "00:00:15.000"; model.MarkStart(); model.CompleteToVideoEnd();
            Assert.Equal(3, model.Blocks.Count); Assert.Same(before[0], model.Blocks[0]); Assert.Same(before[1], model.Blocks[1]);
            Assert.Equal("00:00:09.000", model.RemovedDurationText); Assert.Equal("00:00:11.000", model.RemainingDurationText);
            var complete = model.CreateTransferSnapshot(); Assert.Equal(model.Blocks, complete);
            model.CompleteToVideoEnd(); Assert.Equal(complete, model.Blocks); // A second click cannot add a duplicate.
            model.SelectedBlock = model.Blocks.Last(); model.DeleteBlock();
            Assert.Equal(before, model.Blocks); Assert.Equal("00:00:16.000", model.RemainingDurationText);
        }
        finally { await model.CloseAsync(); }
    });

    [Theory]
    [InlineData("00:00:06.000")]
    [InlineData("00:00:09.000")]
    public Task OverlapRejectsEofAtomicallyAndKeepsOpenStart(string start) => OnSta(async () =>
    {
        var model = await Open();
        try
        {
            Add(model, 8, 10); var original = model.Blocks.Single();
            model.PositionText = start; model.MarkStart(); model.CompleteToVideoEnd();
            Assert.Same(original, model.Blocks.Single()); Assert.True(model.HasPendingStart);
            Assert.Contains("überschneiden", model.Status);
            Assert.Throws<InvalidOperationException>(() => model.CreateTransferSnapshot());
            model.DiscardStart(); Assert.Equal(original, model.CreateTransferSnapshot().Single());
        }
        finally { await model.CloseAsync(); }
    });

    [Fact]
    public Task AdjacentAreaIsValidButStartAtEofCannotCreateZeroLengthCut() => OnSta(async () =>
    {
        var model = await Open();
        try
        {
            Add(model, 2, 4); model.PositionText = "00:00:04.000"; model.MarkStart(); model.CompleteToVideoEnd();
            Assert.Equal(2, model.Blocks.Count); Assert.Equal("00:00:02.000", model.RemainingDurationText);
            model.PositionText = model.BoundaryPositions[1]; model.MarkStart(); model.CompleteToVideoEnd();
            Assert.False(model.HasPendingStart); Assert.Equal(2, model.Blocks.Count);
        }
        finally { await model.CloseAsync(); }
    });

    [Fact]
    public Task EofDoesNotCommitOrOverwriteCorrectionDraft() => OnSta(async () =>
    {
        var model = await Open();
        try
        {
            Add(model, 2, 4); var original = model.Blocks.Single(); model.SelectedBlock = original;
            model.PositionText = "00:00:03.000"; model.MarkStart();
            Assert.False(model.CanCompleteToVideoEnd); model.CompleteToVideoEnd();
            Assert.Same(original, model.Blocks.Single()); Assert.True(model.HasDraftChanges); Assert.Contains("Korrektur", model.Status);
        }
        finally { await model.CloseAsync(); }
    });

    [Fact]
    public Task NineAdsAndEofUseSameOrdinaryPlanForListDurationTransferAndExport() => OnSta(async () =>
    {
        var model = await Open("00:02:00.000"); var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".cutlist");
        try
        {
            for (var i = 0; i < 9; i++) Add(model, 5 + i * 10, 7 + i * 10);
            model.PositionText = "00:01:35.040"; model.MarkStart(); model.CompleteToVideoEnd();
            var snapshot = model.CreateTransferSnapshot();
            Assert.Equal(10, snapshot.Count); Assert.Equal(snapshot, model.Blocks);
            Assert.Equal("00:01:17.040", model.RemainingDurationText);
            await model.ExportAsync(path, false); var document = CutlistFileReader.Read(path);
            Assert.Equal(10, document.Cuts.Count);
            Assert.InRange(Math.Abs(model.Runtime.Remaining.Ticks - document.Cuts.Sum(s => s.Duration.Ticks)), 0, 1);
            Assert.InRange(Math.Abs(document.Cuts.Last().End.Ticks - TimeText.Parse("00:01:35.040").Ticks), 0, 1);
        }
        finally { await model.CloseAsync(); File.Delete(path); }
    });

    [Fact]
    public Task EofButtonFollowsPendingStartAndUsesActualClickRoute() => OnSta(async () =>
    {
        var media = Media("00:00:20.000");
        var window = new SpotlightWindow(new SpotlightHostOptions(media, "unused", "unused", _ => false), new NoToolsRunner());
        await window.InitializeHostAsync();
        try
        {
            await Dispatcher.Yield(DispatcherPriority.Loaded);
            var button = (Button)window.FindName("CompleteToVideoEndButton");
            Assert.Equal("Schnitt bis Videoende abschließen", button.Content);
            Assert.False(button.IsEnabled); Assert.Null(window.FindName("SetVideoEndButton")); Assert.Null(window.FindName("JumpPicker"));
            Assert.NotNull(window.FindName("OverviewList")); Assert.NotNull(window.FindName("DetailsList"));
            window.Model.PositionText = "00:00:15.000"; window.Model.MarkStart();
            await Dispatcher.Yield(DispatcherPriority.DataBind); Assert.True(button.IsEnabled);
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await Dispatcher.Yield(DispatcherPriority.DataBind); Assert.False(button.IsEnabled);
            Assert.Equal(new(TimeSpan.FromSeconds(15), media.Duration), window.Model.Blocks.Single());
        }
        finally { await window.CloseFromHostAsync(); }
    });

    private static MediaInfo Media(string duration) => new("synthetic.mp4", TimeText.Parse(duration), new(25, 1), "h264", 640, 360, 10, 0);
    private static async Task<Model> Open(string duration = "00:00:20.000")
    {
        var model = new Model(new NoToolsRunner()); await model.SetMediaAsync(Media(duration)); Assert.True(model.IsReady); return model;
    }
    private static void Add(Model model, double start, double end)
    { model.PositionText = TimeText.Format(TimeSpan.FromSeconds(start)); model.MarkStart(); model.PositionText = TimeText.Format(TimeSpan.FromSeconds(end)); model.MarkEnd(); }
    private static ImageCard Card(TimeSpan time)
    {
        var image = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgr24, null, new byte[] { 0, 0, 0 }, 3); image.Freeze();
        return new(new Thumbnail(time, "unused.jpg"), image);
    }
    private sealed class NoToolsRunner : IToolRunner
    {
        public Task<ToolResult> RunAsync(string executable, IReadOnlyList<string> arguments, CancellationToken token, Action<string>? errorLine = null) =>
            throw new InvalidOperationException("EOF darf keine weitere Frameanalyse oder Bilderzeugung starten.");
    }
    private static Task OnSta(Func<Task> action)
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
