using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class CutToolLayoutTests
{
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
                    // The unshown window has no native player. Avoid writing the user's window settings.
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
