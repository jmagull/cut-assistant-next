using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Controls;

public sealed class CutTimelineTrack : FrameworkElement
{
    private static readonly Brush TrackBrush =
        new SolidColorBrush(
            Color.FromRgb(
                217,
                222,
                229));

    private static readonly Brush RemoveSegmentBrush =
        new SolidColorBrush(
            Color.FromRgb(
                248,
                113,
                113));

    private static readonly Brush SelectedRemoveSegmentBrush =
        new SolidColorBrush(
            Color.FromRgb(
                153,
                27,
                27));

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
            MediaDuration.Value <= TimeSpan.Zero ||
            RemoveSegments is null)
        {
            return;
        }

        foreach (var segment in RemoveSegments)
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
                    ? SelectedRemoveSegmentBrush
                    : RemoveSegmentBrush;

            drawingContext.DrawRectangle(
                brush,
                null,
                rectangle);
        }
    }

    protected override void OnMouseLeftButtonDown(
        MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (!MediaDuration.HasValue ||
            MediaDuration.Value <= TimeSpan.Zero ||
            RemoveSegments is null ||
            ActualWidth <= 0)
        {
            return;
        }

        var position =
            e.GetPosition(this);

        foreach (var segment in RemoveSegments)
        {
            var rectangle =
                GetSegmentRectangle(
                    segment,
                    MediaDuration.Value);

            if (!rectangle.Contains(position))
            {
                continue;
            }

            SelectedRemoveSegment =
                ReferenceEquals(
                    SelectedRemoveSegment,
                    segment)
                    ? null
                    : segment;
            e.Handled = true;
            return;
        }
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