using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CutAssistantNext.App.Controls;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Serialization;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class CutToolLayoutTests
{
    [Fact]
    public void KeepModeLabelsAndSharedTableBindingsSurviveLayoutChanges() => OnUiThread(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        plan.Initialize(TimeSpan.FromSeconds(100), CutEditingMode.Keep);
        plan.SetStart(TimeSpan.FromSeconds(10)); plan.SetEnd(TimeSpan.FromSeconds(20));
        foreach (var power in new[] { false, true, false })
        {
            window.ApplyCutToolLayout(power); Pump();
            Assert.Equal("Behalteanfang setzen", Get<Button>(window, "CutStartButton").Content);
            Assert.Equal("Behalteende setzen", Get<Button>(window, "CutEndButton").Content);
            Assert.Same(plan.MarkedSegments, Get<DataGrid>(window, "RemoveSegmentsGrid").ItemsSource);
            Assert.Same(plan.MarkedSegments, Get<CutTimelineTrack>(window, "CutTimelineTrack").RemoveSegments);
            Assert.Equal(CutEditingMode.Keep, Get<CutTimelineTrack>(window, "CutTimelineTrack").EditingMode);
            Assert.True(BindingOperations.IsDataBound(Get<Button>(window, "CutStartButton"), ContentControl.ContentProperty));
        }
    });

    [Fact]
    public void PowerUserToolsUseTheRequestedOrderAndKeepTheSameButtons() => OnUiThread(window =>
    {
        var start = Get<Button>(window, "VideoStartButton");
        var table = Get<DataGrid>(window, "RemoveSegmentsGrid");
        var tableParent = table.Parent;
        window.ApplyCutToolLayout(true);
        Assert.Same(start, Get<Button>(window, "VideoStartButton"));
        Assert.Same(tableParent, table.Parent);
        AssertCell(window, "VideoStartButton", "PowerCutMarkersGrid", 0, 0);
        AssertCell(window, "CutStartButton", "PowerCutMarkersGrid", 0, 2);
        AssertCell(window, "CutEndButton", "PowerCutMarkersGrid", 2, 0);
        AssertCell(window, "VideoEndButton", "PowerCutMarkersGrid", 2, 2);
        AssertCell(window, "RemoveCutSegmentButton", "PowerCutActionsGrid", 0, 0);
        Assert.Equal(HorizontalAlignment.Right, Get<Button>(window, "RemoveCutSegmentButton").HorizontalAlignment);
        AssertCell(window, "CutMp4BoxButton", "PowerCutEnginesGrid", 0, 0);
        AssertCell(window, "CutOtrCanButton", "PowerCutEnginesGrid", 0, 2);
        AssertCell(window, "ReopenCutReviewButton", "PowerCutEnginesGrid", 1, 0);
        Assert.Equal(3, Grid.GetColumnSpan(Get<Button>(window, "ReopenCutReviewButton")));
        AssertCell(window, "LoadCutlistButton", "PowerCutlistsGrid", 0, 0);
        AssertCell(window, "LoadServerCutlistButton", "PowerCutlistsGrid", 0, 2);
        AssertCell(window, "GenerateCutlistButton", "PowerCutlistsGrid", 2, 0);
        AssertCell(window, "UploadCutlistButton", "PowerCutlistsGrid", 2, 2);
        AssertCell(window, "NamingSettingsButton", "PowerNamingGrid", 0, 0);
        Assert.True(Grid.GetRow(Get<Grid>(window, "PowerCutEnginesGrid")) <
            Grid.GetRow(Get<Grid>(window, "PowerCutlistsGrid")));
    });

    [Fact]
    public void RoundTripsRestoreStandardOrderLabelsAndGeometry() => OnUiThread(window =>
    {
        var markers = Get<WrapPanel>(window, "CutMarkersPanel");
        var pending = Get<WrapPanel>(window, "PendingStartPanel");
        var actions = Get<WrapPanel>(window, "CutActionsPanel");
        var markerChildren = markers.Children.Cast<UIElement>().ToArray();
        var pendingChildren = pending.Children.Cast<UIElement>().ToArray();
        var actionChildren = actions.Children.Cast<UIElement>().ToArray();
        var buttons = markerChildren.Concat(pendingChildren).Concat(actionChildren).OfType<Button>()
            .Select(button => new
            {
                Button = button,
                button.Content,
                button.MinWidth,
                button.Height,
                button.Margin,
                button.Padding,
                button.HorizontalAlignment,
                button.ContentTemplate
            }).ToArray();
        for (var repeat = 0; repeat < 3; repeat++)
        {
            window.ApplyCutToolLayout(true);
            window.ApplyCutToolLayout(false);
            Assert.Equal(markerChildren, markers.Children.Cast<UIElement>());
            Assert.Equal(pendingChildren, pending.Children.Cast<UIElement>());
            Assert.Equal(actionChildren, actions.Children.Cast<UIElement>());
            foreach (var original in buttons)
            {
                Assert.Equal(original.Content, original.Button.Content);
                Assert.Equal(original.MinWidth, original.Button.MinWidth);
                Assert.Equal(original.Height, original.Button.Height);
                Assert.Equal(original.Margin, original.Button.Margin);
                Assert.Equal(original.Padding, original.Button.Padding);
                Assert.Equal(original.HorizontalAlignment, original.Button.HorizontalAlignment);
                Assert.Same(original.ContentTemplate, original.Button.ContentTemplate);
            }
            Assert.Equal(Visibility.Visible, Get<TextBlock>(window, "StandardCutHeader").Visibility);
            Assert.Equal(Visibility.Collapsed, Get<Grid>(window, "PowerCutActionsGrid").Visibility);
        }
    });

    [Fact]
    public void CutBindingsAndClickHandlersSurviveMovingTools() => OnUiThread(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        plan.Initialize(TimeSpan.FromMinutes(20));
        var start = Get<Button>(window, "VideoStartButton");
        var delete = Get<Button>(window, "RemoveCutSegmentButton");
        var loupe = Get<Button>(window, "FrameLoupeButton");
        foreach (var power in new[] { true, false, true })
        {
            window.ApplyCutToolLayout(power);
            Pump();
            Assert.True(BindingOperations.IsDataBound(start, UIElement.IsEnabledProperty));
            Assert.True(BindingOperations.IsDataBound(delete, UIElement.IsEnabledProperty));
            Assert.True(BindingOperations.IsDataBound(loupe, FrameworkElement.ToolTipProperty));
            Assert.False(delete.IsEnabled);
            start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(TimeSpan.Zero, plan.PendingStart);
            plan.SetEnd(TimeSpan.FromSeconds(2));
            plan.SelectedRemoveSegment = plan.RemoveSegments.Single();
            Pump();
            Assert.True(delete.IsEnabled);
            delete.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Empty(plan.RemoveSegments);
            Pump();
            Assert.False(delete.IsEnabled);
        }
    });

    [Fact]
    public void PendingTimeKeepsFullHoursAndMotorDetailsRemainAccessible() => OnUiThread(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        plan.Initialize(TimeSpan.MaxValue);
        window.ApplyCutToolLayout(true);
        plan.SetStart(TimeSpan.FromHours(12345) + TimeSpan.FromMilliseconds(678));
        Pump();
        var time = Get<TextBlock>(window, "PendingStartTime");
        Assert.Equal("12345:00:00.678", time.Text);
        Assert.Equal(150, Get<Grid>(window, "PowerPendingGrid").ColumnDefinitions[1].Width.Value);
        AssertCell(window, "FrameLoupeButton", "PowerPendingGrid", 0, 2);
        Assert.Equal("MP4Box (schnell)", Get<Button>(window, "CutMp4BoxButton").Content);
        Assert.Equal("Schneiden MP4Box (schnell)", Get<Button>(window, "CutMp4BoxButton").ToolTip);
        Assert.Equal("otr-can (framegenau)", Get<Button>(window, "CutOtrCanButton").Content);
        Assert.Equal("Schneiden otr-can (framegenau, langsam)", Get<Button>(window, "CutOtrCanButton").ToolTip);
    });

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 1)]
    [InlineData(false, 2)]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    public void RenderedCutCellsKeepTimesAndMarkOnlyMatchingBoundaries(bool keep, int layout) => OnUiThread(window =>
    {
        var mode = keep ? CutEditingMode.Keep : CutEditingMode.Remove;
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        var duration = TimeSpan.FromSeconds(12);
        var keyframeTimes = new[] { 0, 1200, 3000, 7280 };
        var timeline = new VideoFrameWindow(0, new VideoTimeBase(1, 1000), 0, 0,
            keyframeTimes.Select(pts => new VideoFrameInfo(0, pts, null, null, null, true, "I")).ToArray());
        var keyframes = VideoKeyframeIndex.Create(timeline, 0, duration, 0);
        var navigation = Field<KeyframeNavigationViewModel>(window, "_keyframeNavigation");
        typeof(KeyframeNavigationViewModel).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(navigation, keyframes);

        // Exercise the production XAML and the actual layout without showing a window to the user.
        window.ApplyCutToolLayout((InterfaceLayout)layout);
        window.ShowActivated = false;
        window.ShowInTaskbar = false;
        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = -10000;
        window.Top = -10000;
        window.Width = 1340;
        window.Height = 950;
        window.Show();
        Pump();

        var grid = Get<DataGrid>(window, "RemoveSegmentsGrid");
        plan.Initialize(duration, mode);
        plan.SetStart(TimeSpan.FromMilliseconds(1200));
        plan.SetEnd(TimeSpan.FromMilliseconds(2440));
        plan.SetStart(TimeSpan.FromMilliseconds(4360));
        plan.SetEnd(TimeSpan.FromMilliseconds(7280));
        VerifyRows(2);

        var previous = plan.MarkedSegments[0];
        plan.Replace(previous, TimeSpan.FromMilliseconds(1240), TimeSpan.FromMilliseconds(3000));
        VerifyRows(2);

        var saved = CutlistParser.Parse("""
            [General]
            NoOfCuts=2
            FramesPerSecond=25
            [Cut0]
            Start=1.2
            Duration=1.24
            [Cut1]
            Start=3
            Duration=4.28
            [Info]
            RatingByAuthor=0
            """);
        plan.LoadCutPlan(CutlistCutPlanBuilder.Build(saved, duration));
        VerifyRows(keep ? 2 : 3);

        void VerifyRows(int expected)
        {
            Pump();
            Assert.Equal(expected, grid.Items.Count);
            foreach (var row in plan.MarkedSegments)
            {
                grid.ScrollIntoView(row);
                grid.UpdateLayout();
                Pump();
                for (int col = 0; col < grid.Columns.Count; col++)
                {
                    var cell = grid.Columns[col].GetCellContent(row);
                    Assert.NotNull(cell);
                    var present = cell is ContentPresenter presenter ? VisualTreeHelper.GetChild(presenter, 0) : cell;
                    var (value, isBoundary) = col switch
                    {
                        0 => (row.Start, true),
                        1 => (row.End, true),
                        _ => (row.Duration, false)
                    };
                    if (isBoundary)
                    {
                        var panel = Assert.IsType<StackPanel>(present);
                        var time = Assert.IsType<TextBlock>(panel.Children[0]);
                        var marker = Assert.IsType<TextBlock>(panel.Children[1]);
                        VerifyTime(time, value);
                        Assert.Equal("K", marker.Text);
                        Assert.Equal(keyframes.Contains(value) ? Visibility.Visible : Visibility.Collapsed, marker.Visibility);
                        if (marker.Visibility == Visibility.Visible)
                        {
                            Assert.True(marker.ActualWidth > 0);
                            Assert.True(marker.TranslatePoint(new Point(), panel).X >= time.ActualWidth);
                            Assert.Equal(Color.FromRgb(0x1D, 0x4E, 0xD8), Assert.IsType<SolidColorBrush>(marker.Foreground).Color);
                        }
                    }
                    else
                    {
                        VerifyTime(Assert.IsType<TextBlock>(present), value);
                    }

                    var bitmap = new RenderTargetBitmap((int)Math.Ceiling(cell.ActualWidth),
                        (int)Math.Ceiling(cell.ActualHeight), 96, 96, PixelFormats.Pbgra32);
                    bitmap.Render(cell);
                }
            }
        }

        static void VerifyTime(TextBlock time, TimeSpan value)
        {
            Assert.Equal(value.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture), time.Text);
            Assert.True(time.IsVisible && time.ActualWidth > 0);
        }
    });

    private static void AssertCell(MainWindow window, string element, string parent, int row, int column)
    {
        var control = Get<FrameworkElement>(window, element);
        Assert.Same(Get<Panel>(window, parent), control.Parent);
        Assert.Equal(row, Grid.GetRow(control));
        Assert.Equal(column, Grid.GetColumn(control));
    }

    private static T Get<T>(MainWindow window, string name) => (T)window.FindName(name);
    private static T Field<T>(object target, string name) => (T)target.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target)!;

    private static void Pump()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
            new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static void OnUiThread(Action<MainWindow> assertion)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            MainWindow? window = null;
            try { window = new MainWindow(); assertion(window); }
            catch (Exception error) { failure = error; }
            finally
            {
                if (window is not null)
                {
                    // Avoid writing the user's window settings, including for off-screen render tests.
                    var closing = window.GetType().GetMethod("MainWindow_Closing", BindingFlags.Instance | BindingFlags.NonPublic)!;
                    window.Closing -= (System.ComponentModel.CancelEventHandler)closing.CreateDelegate(
                        typeof(System.ComponentModel.CancelEventHandler), window);
                    Field<PlayerFrameSearchViewModel>(window, "_playerFrameSearch").Dispose();
                    Field<KeyframeNavigationViewModel>(window, "_keyframeNavigation").Dispose();
                    Field<PlaybackViewModel>(window, "_playbackViewModel").Dispose();
                    Field<IMediaPlayerService>(window, "_mediaPlayerService").DisposeAsync().AsTask().GetAwaiter().GetResult();
                    Field<CancellationTokenSource>(window, "_mediaInputCancellation").Dispose();
                    window.Close();
                }
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        })
        { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "WPF layout test timed out.");
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
