using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Input;
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

[Collection("WpfUi")]
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
        var navigation = Get<WrapPanel>(window, "CutNavigationControls");
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
                Assert.Same(navigation, search.Parent);
                Assert.Same(Get<CutToolStripPanel>(window, "Power2ToolStrip"), navigation.Parent);
                Assert.Equal(1, Grid.GetColumnSpan(navigation)); Assert.Equal(0, Grid.GetRow(navigation));
                Assert.Equal(Visibility.Collapsed, Get<Grid>(window, "PowerCutMarkersGrid").Visibility);
            }
            else
            {
                Assert.Same(navigation, search.Parent);
                Assert.Same(Get<Grid>(window, "PlaybackControls"), navigation.Parent);
                Assert.Equal(8, Grid.GetColumnSpan(navigation)); Assert.Equal(2, Grid.GetRow(navigation));
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
        var step = Get<Button>(window, "StepForwardButton");
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

    [Theory]
    [InlineData("Standard")]
    [InlineData("PowerUser")]
    [InlineData("PowerUser2")]
    public void SecondsControlsReuseExistingButtonHeightAndKeepSingleRow(string layoutName) => WithWindow(window =>
    {
        var layout = Enum.Parse<InterfaceLayout>(layoutName);
        Apply(window, layout, 1900, 1000);
        var strip = Get<WrapPanel>(window, "PlayerButtonStrip");
        var group = Get<Grid>(window, "SecondsSeekControls");
        Assert.Equal(new[] { "PlayPauseButton", "SecondsSeekControls", "StepBackwardButton", "StepForwardButton", "KeyframeButtons" },
            strip.Children.OfType<FrameworkElement>().Take(5).Select(c => c.Name));
        Assert.Null(window.FindName("StepBackwardFramesButton")); Assert.Null(window.FindName("StepForwardFramesButton"));
        var frame = Get<Button>(window, "StepForwardButton");
        Assert.Equal(layout == InterfaceLayout.PowerUser2 ? 30 : 36, frame.ActualHeight);
        foreach (var name in new[] { "JumpBackwardButton", "JumpForwardButton", "JumpSecondsPicker" })
        {
            var control = Get<FrameworkElement>(window, name);
            Assert.Equal(frame.ActualHeight, control.ActualHeight);
            Assert.InRange(Math.Abs(control.TransformToAncestor(strip).Transform(new Point()).Y -
                frame.TransformToAncestor(strip).Transform(new Point()).Y), 0, .5);
        }
        Assert.True(MainWindow.IsPlaybackTextInput(new TextBox()));
        Assert.True(MainWindow.IsPlaybackTextInput(Get<ComboBox>(window, "JumpSecondsPicker")));
        Assert.False(MainWindow.IsPlaybackTextInput(Get<Button>(window, "JumpBackwardButton")));
        Assert.Same(Get<Grid>(window, "PlaybackControls"), strip.Parent);
    });

    [Theory]
    [InlineData("Standard", CutEditingMode.Remove)]
    [InlineData("Standard", CutEditingMode.Keep)]
    [InlineData("PowerUser", CutEditingMode.Remove)]
    [InlineData("PowerUser", CutEditingMode.Keep)]
    [InlineData("PowerUser2", CutEditingMode.Remove)]
    [InlineData("PowerUser2", CutEditingMode.Keep)]
    public void SchnittshuffleUsesSelectedBoundariesWithoutChangingTheEdit(string layoutName, CutEditingMode mode) => WithWindow(window =>
    {
        var layout = Enum.Parse<InterfaceLayout>(layoutName);
        var duration = TimeSpan.FromSeconds(100);
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        var group = Get<StackPanel>(window, "SchnittshuffleControls");
        var table = Get<DataGrid>(window, "RemoveSegmentsGrid");
        var slider = Get<Slider>(window, "TimelineSlider");
        var start = Get<Button>(window, "SchnittshuffleStartButton");
        var end = Get<Button>(window, "SchnittshuffleEndButton");
        var player = UseSchnittshuffleTestPlayer(window, duration);

        Apply(window, layout, 1900);
        Assert.Equal(Visibility.Hidden, group.Visibility);
        plan.Initialize(duration, mode);
        plan.SetStart(TimeSpan.FromSeconds(10));
        Pump();
        Assert.Equal(Visibility.Hidden, group.Visibility);
        start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Empty(player.SeekPositions);
        plan.SetEnd(TimeSpan.FromSeconds(20));
        plan.SetStart(TimeSpan.FromSeconds(40));
        plan.SetEnd(duration);
        Pump();
        Assert.Equal(Visibility.Hidden, group.Visibility);

        var first = plan.MarkedSegments[0];
        var second = plan.MarkedSegments[1];
        var savedRanges = plan.CreateCutPlanSnapshot().RemoveSegments.Select(x => (x.Start, x.End)).ToArray();
        plan.SelectEdge(first, CutEdgeSide.End);
        Apply(window, layout, 1900);
        Assert.Equal(Visibility.Visible, group.Visibility);
        Assert.Same(first, table.SelectedItem);
        FocusManager.SetFocusedElement(window, start);
        foreach (var button in new[] { start, end, start, end })
        {
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Pump();
            Assert.Same(first, table.SelectedItem);
            Assert.Same(first, plan.SelectedRemoveSegment);
            Assert.Equal(CutEdgeSide.End, plan.SelectedEdgeSide);
            Assert.Equal(mode, plan.EditingMode);
            Assert.Equal(player.Position.TotalSeconds, slider.Value, 5);
        }
        Assert.Equal(new[] { 10d, 20d, 10d, 20d }, player.SeekPositions.Select(x => x.TotalSeconds));

        table.SelectedItem = second;
        Pump();
        start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        end.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Pump();
        Assert.Equal(TimeSpan.FromSeconds(40), player.SeekPositions[^2]);
        Assert.Equal(TimeSpan.FromSeconds(99.96), player.SeekPositions[^1]);
        Assert.Same(second, table.SelectedItem);
        Assert.Equal(duration, second.End);
        Assert.Equal(savedRanges, plan.CreateCutPlanSnapshot().RemoveSegments.Select(x => (x.Start, x.End)));

        plan.Replace(second, TimeSpan.FromSeconds(41), duration);
        Pump();
        start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Pump();
        Assert.Equal(TimeSpan.FromSeconds(41), player.SeekPositions[^1]);
        Assert.Same(plan.SelectedRemoveSegment, table.SelectedItem);

        table.SelectedItem = null;
        Pump();
        Assert.Equal(Visibility.Hidden, group.Visibility);
        plan.SelectEdge(first, CutEdgeSide.Start);
        player.State = MediaPlayerState.Empty;
        player.RaiseStateChanged();
        Pump();
        Assert.Equal(Visibility.Hidden, group.Visibility);
    });

    [Theory]
    [InlineData("Standard", 1d)]
    [InlineData("Standard", 1.25d)]
    [InlineData("Standard", 1.5d)]
    [InlineData("Standard", 2d)]
    [InlineData("PowerUser", 1d)]
    [InlineData("PowerUser", 1.25d)]
    [InlineData("PowerUser", 1.5d)]
    [InlineData("PowerUser", 2d)]
    [InlineData("PowerUser2", 1d)]
    [InlineData("PowerUser2", 1.25d)]
    [InlineData("PowerUser2", 1.5d)]
    [InlineData("PowerUser2", 2d)]
    public void SchnittshuffleGeometryRemainsCompactWhenSelectionChanges(string layoutName, double scale) => WithWindow(window =>
    {
        VisualTreeHelper.SetRootDpi(window, new DpiScale(scale, scale));
        var layout = Enum.Parse<InterfaceLayout>(layoutName);
        var duration = TimeSpan.FromSeconds(100);
        UseSchnittshuffleTestPlayer(window, duration);
        var plan = Field<CutPlanViewModel>(window, "_cutPlanViewModel");
        plan.Initialize(duration);
        plan.SetStart(TimeSpan.FromSeconds(5));
        plan.SetEnd(TimeSpan.FromSeconds(15));
        var nav = Get<WrapPanel>(window, "CutNavigationControls");
        var search = Get<WrapPanel>(window, "PlayerFrameSearchControls");
        var shuffle = Get<StackPanel>(window, "SchnittshuffleControls");
        var left = Get<Button>(window, "SchnittshuffleStartButton");
        var right = Get<Button>(window, "SchnittshuffleEndButton");
        var step = Get<Button>(window, "StepForwardButton");
        var loupe = Get<Button>(window, "FrameLoupeButton");
        var markers = Get<Button>(window, "VideoEndButton");
        foreach (var width in new[] { 1900d, layout == InterfaceLayout.Standard ? 760d : 1180d })
        {
            plan.SelectedRemoveSegment = null;
            Apply(window, layout, width, 900);
            Assert.Equal(layout, Field<InterfaceLayout?>(window, "_activeLayout"));
            var previousPosition = search.TransformToAncestor(nav).Transform(new Point());
            var previousSizes = (nav.ActualHeight, search.RenderSize, step.RenderSize, loupe.RenderSize, markers.RenderSize);
            Assert.Equal(Visibility.Hidden, shuffle.Visibility);
            plan.SelectedRemoveSegment = plan.MarkedSegments.Single();
            Apply(window, layout, width, 900);
            Assert.Equal(Visibility.Visible, shuffle.Visibility);
            Assert.Equal(previousPosition, search.TransformToAncestor(nav).Transform(new Point()));
            Assert.Equal(previousSizes, (nav.ActualHeight, search.RenderSize, step.RenderSize, loupe.RenderSize, markers.RenderSize));
            Assert.Equal(new Size(34, 30), left.RenderSize);
            Assert.Equal(left.RenderSize, right.RenderSize);
            var leftPosition = left.TransformToAncestor(shuffle).Transform(new Point());
            var rightPosition = right.TransformToAncestor(shuffle).Transform(new Point());
            Assert.Equal(leftPosition.Y, rightPosition.Y);
            Assert.Equal(4, rightPosition.X - leftPosition.X - left.ActualWidth, 6);
            Assert.Equal(left.ToolTip, AutomationProperties.GetName(left));
            Assert.Equal(right.ToolTip, AutomationProperties.GetName(right));
            Assert.True(shuffle.TransformToAncestor(nav).TransformBounds(new Rect(shuffle.RenderSize)).Right <= nav.ActualWidth + 1);
            Assert.True(search.TransformToAncestor(nav).TransformBounds(new Rect(search.RenderSize)).Right <= nav.ActualWidth + 1);
            var searchBounds = search.TransformToAncestor(nav).TransformBounds(new Rect(search.RenderSize));
            var shuffleBounds = shuffle.TransformToAncestor(nav).TransformBounds(new Rect(shuffle.RenderSize));
            Assert.False(Rect.Intersect(searchBounds, shuffleBounds) is { Width: > 0, Height: > 0 });
            if (layout == InterfaceLayout.PowerUser2)
            {
                var dock = Get<CutToolStripPanel>(window, "Power2ToolStrip");
                Assert.Equal(width < 1900, dock.IsSearchBelow);
                Capture(dock, $"schnittshuffle-{layout}-{width:0}-{scale:0.00}.png", scale);
            }
            else
            {
                Capture(nav, $"schnittshuffle-{layout}-{width:0}-{scale:0.00}.png", scale);
            }
        }
    });

    [Theory]
    [InlineData(10d, 20d, 25d, 10d)]
    [InlineData(20d, 20d, 25d, 19.96d)]
    [InlineData(20d, 20d, 50d, 19.98d)]
    [InlineData(20d, 20d, null, 19.96d)]
    [InlineData(0.01d, 0.01d, 25d, 0d)]
    public void SchnittshuffleEofSeekStaysInsideMedia(double boundary, double duration, double? fps, double expected)
    {
        Assert.Equal(TimeSpan.FromSeconds(expected),
            MainWindow.GetSchnittshuffleSeekPosition(TimeSpan.FromSeconds(boundary), TimeSpan.FromSeconds(duration), fps));
    }

    private static PlaybackViewModelTests.StubMediaPlayerService UseSchnittshuffleTestPlayer(MainWindow window, TimeSpan duration)
    {
        var player = new PlaybackViewModelTests.StubMediaPlayerService
        {
            Duration = duration,
            State = MediaPlayerState.Paused
        };
        Field<PlaybackViewModel>(window, "_playbackViewModel").Dispose();
        var playback = new PlaybackViewModel(player);
        typeof(MainWindow).GetField("_playbackViewModel", Private)!.SetValue(window, playback);
        Get<Grid>(window, "PlaybackControls").DataContext = playback;
        Get<Grid>(window, "PlaybackControls").IsEnabled = true;
        return player;
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
