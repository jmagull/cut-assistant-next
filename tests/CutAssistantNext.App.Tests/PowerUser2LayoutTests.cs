using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CutAssistantNext.App.Controls;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Media.Playback;

namespace CutAssistantNext.App.Tests;

public sealed class PowerUser2LayoutTests
{
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    private static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, Private)!.GetValue(target)!;
    private static T Get<T>(MainWindow window, string name) => (T)window.FindName(name);

    [Theory]
    [InlineData(0, "StandardOption", "Standard")]
    [InlineData(1, "PowerOption", "Power-User 1")]
    [InlineData(2, "Power2Option", "Power-User 2")]
    public void SettingsDialogSavesThreeLayoutsAndPreservesIndependentEditingMode(int value, string option, string caption) => Sta(() =>
    {
        var layout = (InterfaceLayout)value;
        var directory = Path.Combine(Path.GetTempPath(), $"can-power2-settings-{Guid.NewGuid():N}");
        try
        {
            var path = Path.Combine(directory, "appearance.json");
            var store = new AppearanceSettingsStore(path);
            Assert.True(store.Save(new(InterfaceLayout.PowerUser, CutEditingMode.Keep)));
            var dialog = new AppearanceSettingsDialog(store);
            Exception? failure = null;
            dialog.Loaded += (_, _) => dialog.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var radio = (RadioButton)dialog.FindName(option);
                    Assert.Equal(caption, radio.Content);
                    radio.IsChecked = true;
                    if (layout == InterfaceLayout.PowerUser2)
                    {
                        dialog.UpdateLayout();
                        Capture(dialog, "settings-power-user2.png", 1);
                    }
                    var save = Descendants<Button>(dialog).Single(button => Equals(button.Content, "Übernehmen"));
                    save.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                catch (Exception error) { failure = error; dialog.Close(); }
            }));
            Assert.True(dialog.ShowDialog());
            if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
            Assert.Equal(new AppearanceSettings(layout, CutEditingMode.Keep), new AppearanceSettingsStore(path).Load());
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    });

    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void EveryLayoutTransitionPreservesInstancesBindingsWorkAndOriginalGeometry(CutEditingMode mode) => WithWindow(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        plan.Initialize(TimeSpan.FromSeconds(100), mode);
        plan.SetStart(TimeSpan.FromSeconds(10)); plan.SetEnd(TimeSpan.FromSeconds(20));
        plan.SetStart(TimeSpan.FromSeconds(40));
        var segment = plan.MarkedSegments.Single();
        var names = new[] { "VideoStartButton", "CutStartButton", "CutEndButton", "VideoEndButton", "FrameLoupeButton", "PlayerFrameSearchControls", "CutTimelineTrack", "RemoveSegmentsGrid", "VideoHost" };
        var controls = names.Select(name => Get<FrameworkElement>(window, name)).ToArray();
        var parents = new[] { Get<FrameworkElement>(window, "VideoHost").Parent, Get<FrameworkElement>(window, "CutTimelineTrack").Parent, Get<DataGrid>(window, "RemoveSegmentsGrid").Parent };
        var playback = Get<Grid>(window, "PlaybackControls").DataContext;
        var search = Get<WrapPanel>(window, "PlayerFrameSearchControls"); var searchContext = search.DataContext;
        Apply(window, InterfaceLayout.Standard);
        var geometry = controls.Take(5).Cast<Control>().Select(c => (c.Height, c.MinHeight, c.MinWidth, c.Padding, c.Margin)).ToArray();
        foreach (var layout in new[] { InterfaceLayout.PowerUser, InterfaceLayout.PowerUser2, InterfaceLayout.Standard,
            InterfaceLayout.PowerUser2, InterfaceLayout.PowerUser, InterfaceLayout.Standard, InterfaceLayout.PowerUser2 })
        {
            Apply(window, layout);
            Assert.Equal(controls, names.Select(name => Get<FrameworkElement>(window, name)));
            Assert.Same(segment, plan.MarkedSegments.Single()); Assert.Equal(TimeSpan.FromSeconds(40), plan.PendingStart);
            Assert.Equal(parents, new[] { Get<FrameworkElement>(window, "VideoHost").Parent, Get<FrameworkElement>(window, "CutTimelineTrack").Parent, Get<DataGrid>(window, "RemoveSegmentsGrid").Parent });
            Assert.Same(playback, Get<Grid>(window, "PlaybackControls").DataContext); Assert.Same(searchContext, search.DataContext);
            Assert.True(BindingOperations.IsDataBound(Get<Button>(window, "CutStartButton"), ContentControl.ContentProperty));
            Assert.True(BindingOperations.IsDataBound(Get<Button>(window, "CutEndButton"), UIElement.IsEnabledProperty));
            Assert.Equal(mode == CutEditingMode.Keep ? "Behalteanfang setzen" : "Schnittanfang setzen", Get<Button>(window, "CutStartButton").Content);
            if (layout == InterfaceLayout.PowerUser2)
            {
                Assert.Same(plan, Get<Button>(window, "CutStartButton").DataContext);
                Assert.Same(Get<CutToolStripPanel>(window, "Power2ToolStrip"), search.Parent);
                Assert.Equal(1, Grid.GetColumnSpan(search)); Assert.Equal(0, Grid.GetRow(search));
                Assert.Equal(Visibility.Collapsed, Get<Grid>(window, "PowerCutMarkersGrid").Visibility);
            }
            else
            {
                Assert.Same(Get<Grid>(window, "PlaybackControls"), search.Parent);
                Assert.Equal(8, Grid.GetColumnSpan(search)); Assert.Equal(2, Grid.GetRow(search));
                Assert.Equal("Vorgemerkter Anfang:", Get<TextBlock>(window, "PendingStartLabel").Text);
            }
            if (layout == InterfaceLayout.Standard)
                Assert.Equal(geometry, controls.Take(5).Cast<Control>().Select(c => (c.Height, c.MinHeight, c.MinWidth, c.Padding, c.Margin)));
        }
        // A second handler would try to finish the already-completed range again.
        plan.Initialize(TimeSpan.FromSeconds(100), mode);
        Get<Button>(window, "VideoStartButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Get<Button>(window, "VideoEndButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Single(plan.MarkedSegments);
        plan.SelectedRemoveSegment = plan.MarkedSegments.Single(); Pump();
        Get<Button>(window, "RemoveCutSegmentButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Empty(plan.MarkedSegments);
    });

    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public void PendingAppearanceTracksOnlyTheOpenStartAndUsesReadableThemeColors(CutEditingMode mode) => WithWindow(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(100), mode);
        Apply(window, InterfaceLayout.PowerUser2);
        var label = Get<TextBlock>(window, "PendingStartLabel"); var time = Get<TextBlock>(window, "PendingStartTime");
        Assert.Equal("Vorgemerkter Anfang:", label.Text);
        plan.SetStart(TimeSpan.FromSeconds(40)); Pump();
        Assert.Equal(mode == CutEditingMode.Keep ? "BEHALTEANFANG VORGEMERKT:" : "SCHNITTANFANG VORGEMERKT:", label.Text);
        Assert.Equal(FontWeights.Bold, label.FontWeight); Assert.Equal("00:00:40.000", time.Text);
        var track = Get<CutTimelineTrack>(window, "CutTimelineTrack");
        var slider = Get<Slider>(window, "TimelineSlider"); slider.Maximum = 100;
        foreach (var position in new[] { 65d, 25d }) { slider.Value = position; Pump(); Assert.Equal("00:00:40.000", time.Text); Assert.Equal(position, track.CurrentPositionSeconds); Assert.Empty(plan.MarkedSegments); }
        Assert.Throws<ArgumentOutOfRangeException>(() => plan.SetEnd(TimeSpan.FromSeconds(30)));
        Assert.Equal(FontWeights.Bold, label.FontWeight); Assert.NotNull(plan.PendingStart);
        Get<Border>(window, "Power2PendingBorder").Background = Brushes.Black; Pump();
        var dark = Assert.IsType<SolidColorBrush>(label.Foreground).Color;
        Assert.True(Contrast(dark, Colors.Black) >= 4.5);
        Get<Border>(window, "Power2PendingBorder").Background = Brushes.White; Pump();
        Assert.True(Contrast(Assert.IsType<SolidColorBrush>(label.Foreground).Color, Colors.White) >= 4.5);
        plan.CancelPendingStart(); Pump(); Assert.Equal("Vorgemerkter Anfang:", label.Text); Assert.Equal(FontWeights.Normal, label.FontWeight);
        plan.SetStart(TimeSpan.FromSeconds(40)); plan.SetEnd(TimeSpan.FromSeconds(50)); Pump();
        Assert.Equal("Vorgemerkter Anfang:", label.Text); Assert.Single(plan.MarkedSegments);
        plan.Initialize(TimeSpan.MaxValue, mode); plan.SetStart(TimeSpan.MaxValue - TimeSpan.FromSeconds(1));
        Apply(window, InterfaceLayout.PowerUser2, 1180, 700);
        var formatted = new FormattedText(time.Text, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
            new Typeface(time.FontFamily, time.FontStyle, time.FontWeight, time.FontStretch), time.FontSize, Brushes.Black, VisualTreeHelper.GetDpi(time).PixelsPerDip);
        Assert.True(time.ActualWidth + .5 >= formatted.WidthIncludingTrailingWhitespace);
        Assert.Same(SystemColors.WindowTextBrush, MainWindow.Power2PendingBrush(mode == CutEditingMode.Keep, Colors.Black, true));
    });

    [Fact]
    public void SelectedEdgeAndTableSortSurviveAllLayoutsAndRemainEditable() => WithWindow(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        plan.Initialize(TimeSpan.FromSeconds(100), CutEditingMode.Keep);
        plan.SetStart(TimeSpan.FromSeconds(1)); plan.SetEnd(TimeSpan.FromSeconds(3));
        plan.SetStart(TimeSpan.FromSeconds(5)); plan.SetEnd(TimeSpan.FromSeconds(7));
        var selected = plan.MarkedSegments[0]; plan.SelectEdge(selected, CutEdgeSide.Start);
        var table = Get<DataGrid>(window, "RemoveSegmentsGrid");
        table.Items.SortDescriptions.Add(new SortDescription(nameof(RemoveSegment.Start), ListSortDirection.Descending));
        foreach (var layout in new[] { InterfaceLayout.PowerUser2, InterfaceLayout.Standard, InterfaceLayout.PowerUser, InterfaceLayout.PowerUser2 })
        {
            Apply(window, layout);
            Assert.Same(selected, plan.SelectedRemoveSegment);
            Assert.Same(selected, table.SelectedItem);
            Assert.Equal(CutEdgeSide.Start, plan.SelectedEdgeSide);
            Assert.Single(table.Items.SortDescriptions);
            Assert.Equal(TimeSpan.FromSeconds(5), ((RemoveSegment)table.Items[0]).Start);
        }
        plan.ApplyFrameEdge(selected, CutEdgeSide.Start, TimeSpan.FromMilliseconds(500)); Pump();
        Assert.Equal(TimeSpan.FromMilliseconds(500), plan.SelectedRemoveSegment!.Start);
        Assert.Single(table.Items.SortDescriptions);
    });

    [Fact]
    public void OversizedMarkerTextFallsBackInsteadOfSplittingTheFourButtons() => WithWindow(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(100));
        Get<Button>(window, "CutStartButton").FontSize = 70;
        Apply(window, InterfaceLayout.PowerUser2, 1180, 800);
        Assert.Equal(InterfaceLayout.Standard, Field<InterfaceLayout?>(window, "_activeLayout"));
        Assert.Equal(InterfaceLayout.PowerUser2, Field<InterfaceLayout>(window, "_preferredLayout"));
        Apply(window, InterfaceLayout.PowerUser2, 2400, 1000);
        Assert.Equal(InterfaceLayout.PowerUser2, Field<InterfaceLayout?>(window, "_activeLayout"));
        Assert.Equal(4, Get<Grid>(window, "Power2MarkersGrid").Children.Count);
    });

    [Fact]
    public void SearchReflowsWithoutDuplicatingItAndNarrowWindowRetainsPreferredLayout() => WithWindow(window =>
    {
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(100));
        var strip = Get<CutToolStripPanel>(window, "Power2ToolStrip"); var search = Get<WrapPanel>(window, "PlayerFrameSearchControls");
        search.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
        Apply(window, InterfaceLayout.PowerUser2, 1900, 900); Assert.False(strip.IsSearchBelow);
        Apply(window, InterfaceLayout.PowerUser2, 1180, 700); Assert.True(strip.IsSearchBelow);
        Assert.Equal(4, Get<Grid>(window, "Power2MarkersGrid").Children.Count);
        search.Visibility = Visibility.Collapsed; Apply(window, InterfaceLayout.PowerUser2, 1180, 700); Assert.False(strip.IsSearchBelow);
        search.Visibility = Visibility.Visible;
        Apply(window, InterfaceLayout.PowerUser2, 960, 700);
        Assert.Equal(InterfaceLayout.Standard, Field<InterfaceLayout?>(window, "_activeLayout"));
        Assert.Equal(InterfaceLayout.PowerUser2, Field<InterfaceLayout>(window, "_preferredLayout"));
        Apply(window, InterfaceLayout.PowerUser2, 1900, 900); Assert.Equal(InterfaceLayout.PowerUser2, Field<InterfaceLayout?>(window, "_activeLayout"));
    });

    [Theory]
    [InlineData(1d)]
    [InlineData(1.25d)]
    [InlineData(1.5d)]
    [InlineData(2d)]
    public void StepAndKeyframeButtonsAlignAndGroupGeometryIsRestored(double scale) => WithWindow(window =>
    {
        VisualTreeHelper.SetRootDpi(window, new DpiScale(scale, scale));
        var group = Get<StackPanel>(window, "KeyframeButtons");
        group.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
        var originalMargin = group.Margin; var originalAlignment = group.VerticalAlignment;
        Apply(window, InterfaceLayout.PowerUser2, 1900, 900);
        var player = Get<Grid>(window, "PlaybackControls");
        var step = Get<Button>(window, "StepForwardFramesButton");
        var stepBounds = step.TransformToAncestor(player).TransformBounds(new Rect(step.RenderSize));
        foreach (var key in group.Children.OfType<Button>())
        {
            var bounds = key.TransformToAncestor(player).TransformBounds(new Rect(key.RenderSize));
            Assert.InRange(Math.Abs(bounds.Top - stepBounds.Top), 0, .5 / scale);
            Assert.InRange(Math.Abs(bounds.Bottom - stepBounds.Bottom), 0, .5 / scale);
        }
        foreach (var layout in new[] { InterfaceLayout.Standard, InterfaceLayout.PowerUser })
        {
            Apply(window, layout, 1900, 900);
            Assert.Equal(originalMargin, group.Margin);
            Assert.Equal(originalAlignment, group.VerticalAlignment);
        }
    });

    [Theory]
    [InlineData(1d)]
    [InlineData(1.25d)]
    [InlineData(1.5d)]
    [InlineData(2d)]
    public void WpfRootDpiAndSmallWindowsKeepAllActionsReachable(double scale) => WithWindow(window =>
    {
        VisualTreeHelper.SetRootDpi(window, new DpiScale(scale, scale));
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel"); plan.Initialize(TimeSpan.FromSeconds(100), CutEditingMode.Keep);
        for (var i = 0; i < 40; i++) { plan.SetStart(TimeSpan.FromSeconds(i * 2)); plan.SetEnd(TimeSpan.FromSeconds(i * 2 + 1)); }
        plan.SetStart(TimeSpan.FromSeconds(85));
        Apply(window, InterfaceLayout.PowerUser2, 1920 / scale, 1080 / scale);
        var expected = 1920 / scale >= 1100 ? InterfaceLayout.PowerUser2 : InterfaceLayout.Standard;
        Assert.Equal(expected, Field<InterfaceLayout?>(window, "_activeLayout"));
        Assert.Equal(40, Get<DataGrid>(window, "RemoveSegmentsGrid").Items.Count);
        Assert.True(Get<DataGrid>(window, "RemoveSegmentsGrid").ActualHeight >= 80);
        foreach (var name in new[] { "CutMp4BoxButton", "CutOtrCanButton", "LoadCutlistButton", "LoadServerCutlistButton", "GenerateCutlistButton", "UploadCutlistButton", "NamingSettingsButton" })
        {
            var button = Get<Button>(window, name);
            Assert.Equal(Visibility.Visible, button.Visibility); Assert.True(button.ActualHeight >= 30);
            var parent = Assert.IsAssignableFrom<FrameworkElement>(button.Parent);
            var bounds = button.TransformToAncestor(parent).TransformBounds(new Rect(button.RenderSize));
            Assert.True(bounds.Right <= parent.ActualWidth + 1, $"{name}: {bounds} exceeds {parent.ActualWidth}");
        }
        Capture((FrameworkElement)window.Content, $"power2-dpi-{scale.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}.png", scale);
    });

    private static double Contrast(Color first, Color second)
    {
        static double L(Color c) { static double F(byte b) { var x = b / 255d; return x <= .04045 ? x / 12.92 : Math.Pow((x + .055) / 1.055, 2.4); } return .2126 * F(c.R) + .7152 * F(c.G) + .0722 * F(c.B); }
        var a = L(first); var b = L(second); return (Math.Max(a, b) + .05) / (Math.Min(a, b) + .05);
    }

    private static void Apply(MainWindow window, InterfaceLayout layout, double width = 1600, double height = 900)
    {
        typeof(MainWindow).GetField("_preferredLayout", Private)!.SetValue(window, layout);
        var content = (FrameworkElement)window.Content;
        for (var i = 0; i < 4; i++) { content.Measure(new Size(width, height)); content.Arrange(new Rect(0, 0, width, height)); content.UpdateLayout(); window.ApplyInterfaceLayout(); Pump(); }
    }

    private static void Capture(FrameworkElement control, string name, double scale)
    {
        var directory = Environment.GetEnvironmentVariable("CAN_POWER2_UI_PROOF"); if (string.IsNullOrWhiteSpace(directory)) return;
        Directory.CreateDirectory(directory);
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(control.ActualWidth * scale), (int)Math.Ceiling(control.ActualHeight * scale), 96 * scale, 96 * scale, PixelFormats.Pbgra32);
        var visual = new DrawingVisual();
        using (var drawing = visual.RenderOpen())
        {
            var bounds = new Rect(0, 0, control.ActualWidth, control.ActualHeight);
            drawing.DrawRectangle(new SolidColorBrush(Color.FromRgb(244, 246, 248)), null, bounds);
            drawing.DrawRectangle(new VisualBrush(control), null, bounds);
        }
        bitmap.Render(visual); var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var file = File.Create(Path.Combine(directory, name)); encoder.Save(file);
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        { var child = VisualTreeHelper.GetChild(parent, i); if (child is T value) yield return value; foreach (var value2 in Descendants<T>(child)) yield return value2; }
    }

    private static void Pump()
    {
        var frame = new DispatcherFrame(); Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false)); Dispatcher.PushFrame(frame);
    }

    private static void WithWindow(Action<MainWindow> assertion) => Sta(() =>
    {
        var window = new MainWindow();
        try { assertion(window); }
        finally
        {
            var closing = typeof(MainWindow).GetMethod("MainWindow_Closing", Private)!;
            window.Closing -= (CancelEventHandler)closing.CreateDelegate(typeof(CancelEventHandler), window);
            Field<PlayerFrameSearchViewModel>(window, "_playerFrameSearch").Dispose();
            Field<KeyframeNavigationViewModel>(window, "_keyframeNavigation").Dispose();
            Field<PlaybackViewModel>(window, "_playbackViewModel").Dispose();
            Field<IMediaPlayerService>(window, "_mediaPlayerService").DisposeAsync().AsTask().GetAwaiter().GetResult();
            Field<CancellationTokenSource>(window, "_mediaInputCancellation").Dispose(); window.Close();
        }
    });

    private static void Sta(Action assertion)
    {
        Exception? failure = null;
        var thread = new Thread(() => { try { assertion(); } catch (Exception error) { failure = error; } finally { Dispatcher.CurrentDispatcher.InvokeShutdown(); } }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA); thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "Power-User-2 WPF test timed out.");
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
