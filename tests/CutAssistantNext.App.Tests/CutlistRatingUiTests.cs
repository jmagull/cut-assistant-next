using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.Tests.Services;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class CutlistRatingUiTests
{
    [Fact]
    public Task DialogDoesNotSelectOrSendAutomaticallyAndDisplaysSafeIdentity() => Sta(async () =>
    {
        using var fixture = new RatingFixture();
        var choices = fixture.Choices;
        Assert.Equal(6, choices.Items.Count);
        Assert.Equal(-1, choices.SelectedIndex); Assert.False(fixture.Send.IsEnabled);
        await fixture.Dialog.SendSelectedAsync();
        Assert.Equal(0, fixture.Calls);
        var identity = ((TextBlock)fixture.Dialog.FindName("IdentityText")).Text;
        Assert.Contains("Community", identity); Assert.Contains("12345", identity);
        Assert.Contains("Aufnahme.mp4.cutlist", identity); Assert.DoesNotContain(CutlistRatingTests.Fred, identity);
    });

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public Task EachExplicitChoiceCanBeSentOnlyOnce(int grade) => Sta(async () =>
    {
        using var fixture = new RatingFixture();
        fixture.Choices.SelectedIndex = grade;
        Assert.True(fixture.Send.IsEnabled);
        await fixture.Dialog.SendSelectedAsync();
        Assert.Equal(grade, fixture.Grade); Assert.Equal(1, fixture.Calls);
        Assert.True(fixture.Dialog.Submitted);
        Assert.Equal("Bewertung an den Server übermittelt.", fixture.Status.Text);
        await fixture.Dialog.SendSelectedAsync();
        Assert.Equal(1, fixture.Calls);
    });

    [Fact]
    public Task DoubleClickIsDisabledBeforeAwaitAndFailureCanReturnToWork() => Sta(async () =>
    {
        var completion = new TaskCompletionSource<CutlistRatingResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var fixture = new RatingFixture(async (_, _, _) => { entered.SetResult(); return await completion.Task; });
        fixture.Choices.SelectedIndex = 4;
        var send = fixture.Dialog.SendSelectedAsync();
        Assert.False(fixture.Send.IsEnabled);
        await entered.Task;
        await fixture.Dialog.SendSelectedAsync();
        Assert.Equal(1, fixture.Calls);
        var closing = new CancelEventArgs();
        typeof(CutlistRatingDialog).GetMethod("DialogClosing", Private)!.Invoke(fixture.Dialog, [fixture.Dialog, closing]);
        Assert.True(closing.Cancel); Assert.True(fixture.Token.IsCancellationRequested);
        completion.SetResult(new(CutlistRatingState.Unknown, "Ergebnis unklar; keine automatische Wiederholung."));
        await send;
        Assert.False(fixture.Dialog.Submitted);
        Assert.Contains("Ergebnis unklar", fixture.Status.Text);
        Assert.True(((Button)fixture.Dialog.FindName("CancelButton")).IsEnabled);
        Assert.False(fixture.Send.IsEnabled);
    });

    [Theory]
    [InlineData("Rate")]
    [InlineData("Skip")]
    [InlineData("Cancel")]
    public Task QuestionOffersThreeDistinctDecisions(string choice) => Sta(() =>
    {
        var context = new UsedCutlistRatingContext(CutlistRatingTests.Source(), "Original.mp4");
        var dialog = new CutlistRatingQuestionDialog(context);
        Assert.Equal(CutlistRatingDecision.Cancel, dialog.Decision);
        if (choice == "Cancel") { dialog.Close(); return Task.CompletedTask; }
        // Show the real modal window; dispatch an ordinary button click on its WPF dispatcher.
        dialog.Loaded += (_, _) => dialog.Dispatcher.BeginInvoke(() =>
            ((Button)dialog.FindName(choice == "Rate" ? "RateButton" : "SkipButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
        Assert.True(dialog.ShowDialog());
        Assert.Equal(Enum.Parse<CutlistRatingDecision>(choice), dialog.Decision);
        return Task.CompletedTask;
    });

    [Theory]
    [InlineData("film", false)]
    [InlineData("cutlist", false)]
    [InlineData("close", false)]
    [InlineData("film", true)]
    [InlineData("cutlist", true)]
    [InlineData("close", true)]
    public Task CancelAtEveryActualEntryPreservesWorkAndResources(string entry, bool failedRating) => Sta(async () =>
    {
        await using var fixture = new WindowFixture();
        var before = fixture.Plan.RemoveSegments.Select(x => (x.Start, x.End)).ToArray();
        var pending = fixture.Plan.PendingStart;
        var workflow = fixture.Workflow;
        var original = workflow.Current;
        var questions = 0; var replacements = 0;
        fixture.Window.RatingQuestion = context =>
        {
            Assert.Same(original, context);
            questions++;
            return Task.FromResult(failedRating ? CutlistRatingDecision.Rate : CutlistRatingDecision.Cancel);
        };
        fixture.Window.RatingEditor = _ => failedRating ? Task.FromResult(false) : throw new Exception("No send");
        fixture.Window.CutlistReplacementPrompt = () => { replacements++; return true; };
        fixture.Window.MediaReplacementPrompt = () => { replacements++; return true; };
        if (entry == "film")
            await fixture.Window.OpenMediaFileAsync(fixture.NextMovie);
        else if (entry == "cutlist")
        {
            var document = CutlistFileReader.ReadBytes(LocalCutlistDiscoveryTests.Bytes());
            await (Task)typeof(MainWindow).GetMethod("LoadCutlistDocument", Private)!.Invoke(
                fixture.Window, [document, TimeSpan.FromSeconds(100), null, CutlistRatingTests.Source("777")])!;
        }
        else
        {
            var close = new CancelEventArgs();
            typeof(MainWindow).GetMethod("MainWindow_Closing", Private)!.Invoke(fixture.Window, [fixture.Window, close]);
            Assert.True(close.Cancel);
            await Until(() => questions > 0 && !fixture.Field<bool>("_ratingTransitionInProgress"));
        }
        Assert.Equal(1, questions);
        Assert.Equal(entry == "close" ? 0 : 1, replacements);
        Assert.Equal(before, fixture.Plan.RemoveSegments.Select(x => (x.Start, x.End)).ToArray());
        Assert.Equal(pending, fixture.Plan.PendingStart);
        Assert.Same(original, workflow.Current);
        Assert.False(fixture.Field<bool>("_shutdownStarted")); Assert.False(fixture.Field<bool>("_isClosed"));
        Assert.False(fixture.Field<CancellationTokenSource>("_mediaInputCancellation").IsCancellationRequested);
        Assert.True(fixture.Window.IsEnabled);
    });

    [Theory]
    [InlineData("film")]
    [InlineData("cutlist")]
    public Task ExistingDiscardRefusalDoesNotAskForRatingOrLoseEdits(string entry) => Sta(async () =>
    {
        await using var fixture = new WindowFixture();
        fixture.Window.RatingQuestion = _ => throw new Exception("Existing refusal must win");
        fixture.Window.MediaReplacementPrompt = () => false;
        fixture.Window.CutlistReplacementPrompt = () => false;
        if (entry == "film") await fixture.Window.OpenMediaFileAsync(fixture.NextMovie);
        else await (Task)typeof(MainWindow).GetMethod("LoadCutlistDocument", Private)!.Invoke(fixture.Window,
            [CutlistFileReader.ReadBytes(LocalCutlistDiscoveryTests.Bytes()), TimeSpan.FromSeconds(100), null, null])!;
        Assert.Equal(TimeSpan.FromSeconds(40), fixture.Plan.PendingStart);
        Assert.Single(fixture.Plan.RemoveSegments);
        Assert.NotNull(fixture.Workflow.Current);
    });

    [Fact]
    public Task PendingDecisionBlocksConcurrentCloseAndFilmInput() => Sta(async () =>
    {
        await using var fixture = new WindowFixture();
        var decision = new TaskCompletionSource<CutlistRatingDecision>(TaskCreationOptions.RunContinuationsAsynchronously);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var questions = 0;
        fixture.Window.MediaReplacementPrompt = () => true;
        fixture.Window.RatingQuestion = _ => { questions++; entered.SetResult(); return decision.Task; };
        var opening = fixture.Window.OpenMediaFileAsync(fixture.NextMovie);
        await entered.Task;
        await fixture.Window.OpenMediaFileAsync(fixture.NextMovie);
        var close = new CancelEventArgs();
        typeof(MainWindow).GetMethod("MainWindow_Closing", Private)!.Invoke(fixture.Window, [fixture.Window, close]);
        Assert.True(close.Cancel); Assert.False(fixture.Field<bool>("_shutdownStarted"));
        decision.SetResult(CutlistRatingDecision.Cancel);
        await opening;
        Assert.Equal(1, questions); Assert.True(fixture.Window.IsEnabled);
        Assert.NotNull(fixture.Workflow.Current);
    });

    [Theory]
    [InlineData("Skip")]
    [InlineData("Rate")]
    public Task AcceptedDepartureCanCompleteActualApplicationShutdown(string decision) => Sta(async () =>
    {
        await using var fixture = new WindowFixture();
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Window.Closed += (_, _) => closed.TrySetResult();
        fixture.Window.RatingQuestion = _ => Task.FromResult(Enum.Parse<CutlistRatingDecision>(decision));
        var editorCalls = 0;
        fixture.Window.RatingEditor = _ => { editorCalls++; return Task.FromResult(true); };
        fixture.Window.Close();
        await closed.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal(decision == "Rate" ? 1 : 0, editorCalls);
        Assert.True(fixture.Field<bool>("_shutdownStarted"));
        Assert.True(fixture.Field<bool>("_isClosed"));
        Assert.True(fixture.Field<bool>("_allowClose"));
    });

    [Fact]
    public Task RenderRatingDialogForVisualReview() => Sta(async () =>
    {
        using var fixture = new RatingFixture();
        var directory = Path.Combine(FindWorktree(), ".build", "rating-dialog-visual");
        Directory.CreateDirectory(directory);
        fixture.Dialog.Show();
        await Dispatcher.Yield(DispatcherPriority.Render);
        var content = (FrameworkElement)fixture.Dialog.Content;
        content.UpdateLayout();
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(content.ActualWidth), (int)Math.Ceiling(content.ActualHeight), 96, 96, PixelFormats.Pbgra32);
        var drawing = new DrawingVisual();
        using (var dc = drawing.RenderOpen())
        {
            var rect = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(244, 246, 248)), null, rect);
            dc.DrawRectangle(new VisualBrush(content), null, rect);
        }
        bitmap.Render(drawing);
        using var file = File.Create(Path.Combine(directory, "rating-dialog.png"));
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap)); encoder.Save(file);
    });

    private static string FindWorktree()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CutAssistantNext.sln"))) directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException();
    }
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    private sealed class RatingFixture : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "can-rating-dialog-" + Guid.NewGuid().ToString("N"));
        internal CutlistRatingDialog Dialog { get; }
        internal int Calls { get; private set; }
        internal int Grade { get; private set; }
        internal CancellationToken Token { get; private set; }
        internal ListBox Choices => (ListBox)Dialog.FindName("RatingChoices");
        internal Button Send => (Button)Dialog.FindName("SendButton");
        internal TextBlock Status => (TextBlock)Dialog.FindName("StatusText");
        internal RatingFixture(Func<CutlistRatingSource, int, CancellationToken, Task<CutlistRatingResult>>? send = null)
        {
            var service = new CutlistRatingService(new CutlistRatingSettingsStore(Path.Combine(_root, "ratings.json")), (s, r, t) =>
            {
                Calls++; Grade = r; Token = t;
                return send?.Invoke(s, r, t) ?? Task.FromResult(new CutlistRatingResult(CutlistRatingState.Accepted, "Bewertung an den Server übermittelt."));
            });
            Dialog = new(new(CutlistRatingTests.Source(), "Aufnahme.mp4"), service);
        }
        public void Dispose() { Dialog.Close(); if (Directory.Exists(_root)) Directory.Delete(_root, true); }
    }

    private sealed class WindowFixture : IAsyncDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "can-rating-main-" + Guid.NewGuid().ToString("N"));
        internal MainWindow Window { get; } = new();
        internal CutPlanViewModel Plan => Field<CutPlanViewModel>("_cutPlanViewModel");
        internal CutlistRatingWorkflow Workflow { get; }
        internal string NextMovie => Path.Combine(_root, "Next.mp4");
        internal WindowFixture()
        {
            Directory.CreateDirectory(_root); File.WriteAllBytes(NextMovie, [1]);
            typeof(MainWindow).GetField("_windowSettingsStore", Private)!.SetValue(Window,
                new WindowSettingsStore(Path.Combine(_root, "window.json")));
            Workflow = new(new CutlistRatingSettingsStore(Path.Combine(_root, "ratings.json")));
            Workflow.Use(CutlistRatingTests.Source(), "Original.mp4", "Ich", "Community");
            typeof(MainWindow).GetField("_cutlistRatings", Private)!.SetValue(Window, Workflow);
            Plan.Initialize(TimeSpan.FromSeconds(100));
            Plan.SetStart(TimeSpan.FromSeconds(10)); Plan.SetEnd(TimeSpan.FromSeconds(20)); Plan.SetStart(TimeSpan.FromSeconds(40));
        }
        internal T Field<T>(string name) => (T)typeof(MainWindow).GetField(name, Private)!.GetValue(Window)!;
        public async ValueTask DisposeAsync()
        {
            if (Field<bool>("_allowClose")) { Directory.Delete(_root, true); return; }
            Window.Closing -= (CancelEventHandler)typeof(MainWindow).GetMethod("MainWindow_Closing", Private)!
                .CreateDelegate(typeof(CancelEventHandler), Window);
            Field<PlayerFrameSearchViewModel>("_playerFrameSearch").Dispose();
            Field<KeyframeNavigationViewModel>("_keyframeNavigation").Dispose();
            Field<PlaybackViewModel>("_playbackViewModel").Dispose();
            await Field<IMediaPlayerService>("_mediaPlayerService").DisposeAsync();
            Field<CancellationTokenSource>("_mediaInputCancellation").Dispose();
            Window.Close(); Directory.Delete(_root, true);
        }
    }
    private static async Task Until(Func<bool> done)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!done()) await Task.Delay(10, timeout.Token);
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
