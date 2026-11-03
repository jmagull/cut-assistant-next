using System.Collections;
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

namespace CutAssistantNext.App.Tests;

public sealed class CutEditingTimelineTests
{
    [Theory]
    [InlineData(CutEditingMode.Remove, false)]
    [InlineData(CutEditingMode.Keep, false)]
    [InlineData(CutEditingMode.Remove, true)]
    [InlineData(CutEditingMode.Keep, true)]
    public void ActualTimelineShowsCompletedAndPendingRangesAndSliderOnlyUpdatesPreview(CutEditingMode mode, bool dark) => Sta(() =>
    {
        var model = new CutPlanViewModel(); model.Initialize(TimeSpan.FromSeconds(100), mode);
        model.SetStart(TimeSpan.FromSeconds(10)); model.SetEnd(TimeSpan.FromSeconds(20));
        var original = model.MarkedSegments.Single();
        var keyframes = new CountingKeyframes();
        var slider = new Slider { Minimum = 0, Maximum = 100, Value = 40 };
        var track = new CutTimelineTrack { Height = 28, KeyframePositions = keyframes, DataContext = model };
        track.SetBinding(CutTimelineTrack.MediaDurationProperty, new Binding(nameof(model.MediaDuration)));
        track.SetBinding(CutTimelineTrack.RemoveSegmentsProperty, new Binding(nameof(model.MarkedSegments)));
        track.SetBinding(CutTimelineTrack.EditingModeProperty, new Binding(nameof(model.EditingMode)));
        track.SetBinding(CutTimelineTrack.PendingStartProperty, new Binding(nameof(model.PendingStart)));
        track.SetBinding(CutTimelineTrack.CurrentPositionSecondsProperty, new Binding(nameof(slider.Value)) { Source = slider });
        var text = new TextBlock { Text = model.MarkingHelp, TextWrapping = TextWrapping.Wrap, Foreground = dark ? Brushes.White : Brushes.Black };
        var table = new DataGrid { ItemsSource = model.MarkedSegments, AutoGenerateColumns = true, Height = 120, IsReadOnly = true };
        var panel = new StackPanel { Margin = new Thickness(20) };
        panel.Children.Add(text); panel.Children.Add(track); panel.Children.Add(slider); panel.Children.Add(table);
        var window = new Window { Width = 700, Height = 300, Content = panel, Background = dark ? Brushes.Black : Brushes.White };
        try
        {
            window.Show(); Pump(); window.UpdateLayout();
            var closed = VisualTreeHelper.GetDrawing(track).Children.OfType<GeometryDrawing>().Skip(1).First();
            var closedColor = Assert.IsType<SolidColorBrush>(closed.Brush).Color;
            Assert.Equal(mode == CutEditingMode.Keep, closedColor.G > closedColor.R);
            model.SetStart(TimeSpan.FromSeconds(40)); Pump();
            var visual = Assert.IsType<DrawingVisual>(VisualTreeHelper.GetChild(track, 0));
            Assert.Single(visual.Drawing.Children); // Start line is visible before the slider moves.
            var renders = keyframes.Enumerations;
            Assert.True(renders > 0);
            foreach (var position in new[] { 65d, 25d, 75d, 40.04d })
            {
                slider.Value = position; Pump();
                Assert.Equal(TimeSpan.FromSeconds(40), model.PendingStart);
                Assert.Same(original, model.MarkedSegments.Single());
                Assert.Equal(position, track.CurrentPositionSeconds);
                var preview = visual.Drawing.Children.OfType<GeometryDrawing>().Single(d => d.Geometry is RectangleGeometry);
                var rectangle = Assert.IsType<RectangleGeometry>(preview.Geometry).Rect;
                Assert.Equal(Math.Min(40, position) / 100 * track.ActualWidth, rectangle.X, 5);
                Assert.Equal(Math.Max(1, Math.Abs(40 - position) / 100 * track.ActualWidth), rectangle.Width, 5);
                var color = Assert.IsType<SolidColorBrush>(preview.Brush).Color;
                Assert.InRange(color.A, 1, 254);
                Assert.Equal(mode == CutEditingMode.Keep, color.G > color.R);
                var marker = visual.Drawing.Children.OfType<GeometryDrawing>().Single(d => d.Geometry is LineGeometry);
                Assert.Equal(track.ActualWidth * .4, ((LineGeometry)marker.Geometry).StartPoint.X, 5);
            }
            // Seeking and frame steps all update the same slider value; no static keyframe redraw per update.
            for (var frame = 0; frame < 200; frame++) { slider.Value = 50 + frame * .02; Pump(); }
            Assert.Equal(renders, keyframes.Enumerations);
            Capture(window, $"{mode}-{(dark ? "dark" : "light")}-pending.png");
            model.CancelPendingStart(); Pump();
            Assert.True(visual.Drawing is null || visual.Drawing.Children.Count == 0);
            Assert.Same(original, model.MarkedSegments.Single());
            model.SetStart(TimeSpan.FromSeconds(40)); slider.Value = 60; Pump();
            model.SetEnd(TimeSpan.FromSeconds(60)); Pump();
            Assert.Null(model.PendingStart); Assert.True(visual.Drawing is null || visual.Drawing.Children.Count == 0);
            Assert.Equal(2, model.MarkedSegments.Count); Assert.Equal(2, table.Items.Count);
            Capture(window, $"{mode}-{(dark ? "dark" : "light")}-complete.png");
            model.Reset(); Pump(); Assert.True(visual.Drawing is null || visual.Drawing.Children.Count == 0);
        }
        finally { window.Close(); }
    });

    [Fact]
    public void ActualSettingsDialogPersistsGreenModeWithoutReinterpretingLoadedFilm() => Sta(() =>
    {
        var directory = Path.Combine(Path.GetTempPath(), $"can-settings-dialog-{Guid.NewGuid():N}");
        try
        {
            var store = new AppearanceSettingsStore(Path.Combine(directory, "settings.json"));
            var current = new CutPlanViewModel(); current.Initialize(TimeSpan.FromSeconds(100));
            current.SetStart(TimeSpan.FromSeconds(10)); current.SetEnd(TimeSpan.FromSeconds(20));
            var before = current.MarkedSegments.Single();
            var dialog = new AppearanceSettingsDialog(store);
            Exception? failure = null;
            dialog.Loaded += (_, _) => dialog.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Assert.True(((RadioButton)dialog.FindName("RemoveOption")).IsChecked);
                    ((RadioButton)dialog.FindName("KeepOption")).IsChecked = true;
                    dialog.UpdateLayout(); Capture(dialog, "settings-green.png");
                    var save = Descendants<Button>(dialog).Single(button => Equals(button.Content, "Übernehmen"));
                    save.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                catch (Exception error) { failure = error; dialog.Close(); }
            }));
            Assert.True(dialog.ShowDialog());
            if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
            Assert.Equal(CutEditingMode.Keep, new AppearanceSettingsStore(Path.Combine(directory, "settings.json")).Load().CutEditing);
            Assert.Equal(CutEditingMode.Remove, current.EditingMode); Assert.Same(before, current.MarkedSegments.Single());
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    });

    private sealed class CountingKeyframes : IReadOnlyList<TimeSpan>
    {
        public int Enumerations { get; private set; }
        public int Count => 10000;
        public TimeSpan this[int index] => TimeSpan.FromSeconds(index * .01);
        public IEnumerator<TimeSpan> GetEnumerator() { Enumerations++; for (var i = 0; i < Count; i++) yield return this[i]; }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match) yield return match;
            foreach (var nested in Descendants<T>(child)) yield return nested;
        }
    }

    private static void Capture(FrameworkElement control, string name)
    {
        var directory = Environment.GetEnvironmentVariable("CAN_CUT_EDIT_UI_PROOF");
        if (string.IsNullOrWhiteSpace(directory)) return;
        Directory.CreateDirectory(directory);
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(control.ActualWidth), (int)Math.Ceiling(control.ActualHeight), 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(control);
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var output = File.Create(Path.Combine(directory, name)); encoder.Save(output);
    }

    private static void Pump()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static void Sta(Action assertion)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { assertion(); }
            catch (Exception error) { failure = error; }
            finally { Dispatcher.CurrentDispatcher.InvokeShutdown(); }
        })
        { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA); thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "WPF editing test timed out.");
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
