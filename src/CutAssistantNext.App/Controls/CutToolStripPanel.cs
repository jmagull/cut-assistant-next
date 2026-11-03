using System.Windows;
using System.Windows.Controls;

namespace CutAssistantNext.App.Controls;

/// <summary>Arranges the existing marker row and search group without splitting the marker row.</summary>
public sealed class CutToolStripPanel : Panel
{
    private const double HorizontalGap = 8;
    private const double VerticalGap = 4;

    public double RequiredMarkerWidth { get; private set; }
    public bool IsSearchBelow { get; private set; }
    public event EventHandler? RequiredMarkerWidthChanged;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (InternalChildren.Count == 0) return default;
        var markers = InternalChildren[0];
        markers.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var required = markers.DesiredSize.Width;
        if (Math.Abs(RequiredMarkerWidth - required) > .5)
        {
            RequiredMarkerWidth = required;
            RequiredMarkerWidthChanged?.Invoke(this, EventArgs.Empty);
        }
        var search = InternalChildren.Count > 1 ? InternalChildren[1] : null;
        search?.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var hasSearch = search is { Visibility: not Visibility.Collapsed };
        var searchSize = hasSearch ? search!.DesiredSize : default;
        IsSearchBelow = hasSearch && required + HorizontalGap + searchSize.Width > availableSize.Width;
        if (IsSearchBelow && searchSize.Width > availableSize.Width)
        {
            search!.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            searchSize = search.DesiredSize;
        }
        var width = IsSearchBelow ? Math.Max(required, searchSize.Width)
            : required + (hasSearch ? HorizontalGap + searchSize.Width : 0);
        var height = IsSearchBelow ? markers.DesiredSize.Height + VerticalGap + searchSize.Height
            : Math.Max(markers.DesiredSize.Height, searchSize.Height);
        return new Size(Math.Min(width, availableSize.Width), height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (InternalChildren.Count == 0) return finalSize;
        var markers = InternalChildren[0];
        markers.Arrange(new Rect(new Point(), markers.DesiredSize));
        if (InternalChildren.Count > 1 && InternalChildren[1] is { Visibility: not Visibility.Collapsed } search)
        {
            var width = Math.Min(finalSize.Width, search.DesiredSize.Width);
            var x = IsSearchBelow ? 0 : Math.Max(markers.DesiredSize.Width + HorizontalGap, finalSize.Width - width);
            var y = IsSearchBelow ? markers.DesiredSize.Height + VerticalGap : 0;
            search.Arrange(new Rect(x, y, width, search.DesiredSize.Height));
        }
        return finalSize;
    }
}
