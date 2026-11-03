using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Controls;

public sealed class TimelineSeekRequestedEventArgs(TimeSpan position) : EventArgs
{
    public TimeSpan Position { get; } = position;
}

public sealed class CutTimelineTrack : FrameworkElement
{
    private readonly DrawingVisual _pendingVisual = new();
    private static readonly Brush KeepSegmentBrush = FrozenBrush(Color.FromRgb(74, 222, 128));
    private static readonly Brush SelectedKeepSegmentBrush = FrozenBrush(Color.FromRgb(22, 101, 52));
    private static readonly Brush PendingRemoveBrush = FrozenBrush(Color.FromArgb(85, 239, 68, 68));
    private static readonly Brush PendingKeepBrush = FrozenBrush(Color.FromArgb(85, 34, 197, 94));

    public CutTimelineTrack() { AddVisualChild(_pendingVisual); ClipToBounds = true; }
    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int index) => index == 0 ? _pendingVisual : throw new ArgumentOutOfRangeException(nameof(index));

    public static readonly DependencyProperty EditingModeProperty = DependencyProperty.Register(
        nameof(EditingMode), typeof(CutEditingMode), typeof(CutTimelineTrack),
        new FrameworkPropertyMetadata(CutEditingMode.Remove, FrameworkPropertyMetadataOptions.AffectsRender, OnPreviewChanged));
    public static readonly DependencyProperty PendingStartProperty = DependencyProperty.Register(
        nameof(PendingStart), typeof(TimeSpan?), typeof(CutTimelineTrack), new PropertyMetadata(null, OnPreviewChanged));
    public static readonly DependencyProperty CurrentPositionSecondsProperty = DependencyProperty.Register(
        nameof(CurrentPositionSeconds), typeof(double), typeof(CutTimelineTrack), new PropertyMetadata(0d, OnPreviewChanged));

    public CutEditingMode EditingMode { get => (CutEditingMode)GetValue(EditingModeProperty); set => SetValue(EditingModeProperty, value); }
    public TimeSpan? PendingStart { get => (TimeSpan?)GetValue(PendingStartProperty); set => SetValue(PendingStartProperty, value); }
    public double CurrentPositionSeconds { get => (double)GetValue(CurrentPositionSecondsProperty); set => SetValue(CurrentPositionSecondsProperty, value); }

    private static void OnPreviewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((CutTimelineTrack)sender).DrawPending();

    private void DrawPending()
    {
        using var drawing = _pendingVisual.RenderOpen();
        if (MediaDuration is not { } duration || duration <= TimeSpan.Zero || PendingStart is not { } start ||
            start < TimeSpan.Zero || start > duration || ActualWidth <= 0 || ActualHeight <= 0) return;
        var startX = Math.Clamp(start.TotalSeconds / duration.TotalSeconds, 0, 1) * ActualWidth;
        var current = double.IsFinite(CurrentPositionSeconds) ? CurrentPositionSeconds : start.TotalSeconds;
        var currentX = Math.Clamp(current / duration.TotalSeconds, 0, 1) * ActualWidth;
        var keep = EditingMode == CutEditingMode.Keep;
        if (currentX != startX)
            drawing.DrawRectangle(keep ? PendingKeepBrush : PendingRemoveBrush, null,
                new Rect(Math.Min(startX, currentX), 0, Math.Max(1, Math.Abs(currentX - startX)), ActualHeight));
        var pen = new Pen(keep ? SelectedKeepSegmentBrush : SelectedRemoveSegmentBrush, 2);
        var markerX = Math.Clamp(startX, 1, Math.Max(1, ActualWidth - 1));
        drawing.DrawLine(pen, new Point(markerX, 0), new Point(markerX, ActualHeight));
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        DrawPending();
    }
    private static Brush FrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
    private static readonly Brush TrackBrush = FrozenBrush(Color.FromRgb(217, 222, 229));
    private static readonly Brush RemoveSegmentBrush = FrozenBrush(Color.FromRgb(248, 113, 113));
    private static readonly Brush SelectedRemoveSegmentBrush = FrozenBrush(Color.FromRgb(153, 27, 27));

    public static readonly DependencyProperty MediaDurationProperty =
        DependencyProperty.Register(
            nameof(MediaDuration),
            typeof(TimeSpan?),
            typeof(CutTimelineTrack),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty RemoveSegmentsProperty =
        DependencyProperty.Register(
            nameof(RemoveSegments),
            typeof(IEnumerable<RemoveSegment>),
            typeof(CutTimelineTrack),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnRemoveSegmentsChanged));

    public static readonly DependencyProperty SelectedRemoveSegmentProperty =
        DependencyProperty.Register(
            nameof(SelectedRemoveSegment),
            typeof(RemoveSegment),
            typeof(CutTimelineTrack),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty KeyframePositionsProperty = DependencyProperty.Register(
        nameof(KeyframePositions), typeof(IReadOnlyList<TimeSpan>), typeof(CutTimelineTrack),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public IReadOnlyList<TimeSpan>? KeyframePositions
    {
        get => (IReadOnlyList<TimeSpan>?)GetValue(KeyframePositionsProperty);
        set => SetValue(KeyframePositionsProperty, value);
    }
    public event EventHandler<TimelineSeekRequestedEventArgs>? SeekRequested;

    public TimeSpan? MediaDuration
    {
        get =>
            (TimeSpan?)GetValue(
                MediaDurationProperty);

        set =>
            SetValue(
                MediaDurationProperty,
                value);
    }

    public IEnumerable<RemoveSegment>? RemoveSegments
    {
        get =>
            (IEnumerable<RemoveSegment>?)GetValue(
                RemoveSegmentsProperty);

        set =>
            SetValue(
                RemoveSegmentsProperty,
                value);
    }

    public RemoveSegment? SelectedRemoveSegment
    {
        get =>
            (RemoveSegment?)GetValue(
                SelectedRemoveSegmentProperty);

        set =>
            SetValue(
                SelectedRemoveSegmentProperty,
                value);
    }

    protected override void OnRender(
        DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        DrawPending();

        if (ActualWidth <= 0 ||
            ActualHeight <= 0)
        {
            return;
        }

        drawingContext.DrawRoundedRectangle(
            TrackBrush,
            null,
            new Rect(
                0,
                0,
                ActualWidth,
                ActualHeight),
            3,
            3);

        if (!MediaDuration.HasValue ||
            MediaDuration.Value <= TimeSpan.Zero)
        {
            return;
        }

        foreach (var segment in RemoveSegments ?? [])
        {
            var rectangle =
                GetSegmentRectangle(
                    segment,
                    MediaDuration.Value);

            if (rectangle.Width <= 0)
            {
                continue;
            }

            var brush =
                ReferenceEquals(
                    segment,
                    SelectedRemoveSegment)
                    ? EditingMode == CutEditingMode.Keep ? SelectedKeepSegmentBrush : SelectedRemoveSegmentBrush
                    : EditingMode == CutEditingMode.Keep ? KeepSegmentBrush : RemoveSegmentBrush;

            drawingContext.DrawRectangle(
                brush,
                null,
                rectangle);
        }
        // Merge markers in the same display column; drawing cost stays bounded by track width.
        var columns = new HashSet<int>();
        var pen = new Pen(Brushes.RoyalBlue, 1);
        foreach (var position in KeyframePositions ?? [])
        {
            if (position < TimeSpan.Zero || position >= MediaDuration.Value) continue;
            int column = Math.Clamp((int)Math.Floor(position.TotalSeconds / MediaDuration.Value.TotalSeconds * ActualWidth),
                0, Math.Max(0, (int)ActualWidth - 1));
            if (columns.Add(column)) drawingContext.DrawLine(pen, new Point(column + 0.5, 0),
                new Point(column + 0.5, Math.Min(4, ActualHeight)));
        }
    }

    protected override void OnMouseLeftButtonDown(
        MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        var duration = MediaDuration;
        var mousePosition = e.GetPosition(this);
        var seekPosition = TimelinePositionCalculator.GetPosition(
            mousePosition.X,
            ActualWidth,
            duration);

        if (!seekPosition.HasValue || !duration.HasValue)
        {
            return;
        }

        if (RemoveSegments is not null)
        {
            foreach (var segment in RemoveSegments)
            {
                var rectangle = GetSegmentRectangle(segment, duration.Value);
                if (!rectangle.Contains(mousePosition))
                {
                    continue;
                }

                // Keep the two-way selection binding while changing the current value.
                SetCurrentValue(
                    SelectedRemoveSegmentProperty,
                    ReferenceEquals(SelectedRemoveSegment, segment) ? null : segment);
                break;
            }
        }

        e.Handled = true;
        SeekRequested?.Invoke(this, new TimelineSeekRequestedEventArgs(seekPosition.Value));
    }

    private Rect GetSegmentRectangle(
        RemoveSegment segment,
        TimeSpan mediaDuration)
    {
        var durationSeconds =
            mediaDuration.TotalSeconds;

        var startRatio =
            segment.Start.TotalSeconds /
            durationSeconds;

        var endRatio =
            segment.End.TotalSeconds /
            durationSeconds;

        var left =
            Math.Clamp(
                startRatio,
                0,
                1) *
            ActualWidth;

        var right =
            Math.Clamp(
                endRatio,
                0,
                1) *
            ActualWidth;

        return new Rect(
            left,
            0,
            Math.Max(
                1,
                right - left),
            ActualHeight);
    }

    private static void OnRemoveSegmentsChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        var track =
            (CutTimelineTrack)dependencyObject;

        if (e.OldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -=
                track.OnCollectionChanged;
        }

        if (e.NewValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged +=
                track.OnCollectionChanged;
        }

        track.InvalidateVisual();
    }

    private void OnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        InvalidateVisual();
    }
}
