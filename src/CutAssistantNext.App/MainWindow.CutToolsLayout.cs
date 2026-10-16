using System.Windows;
using System.Windows.Controls;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private FrameworkElement[] _standardMarkerChildren = [];
    private FrameworkElement[] _standardPendingChildren = [];
    private FrameworkElement[] _standardActionChildren = [];
    private readonly Dictionary<Button, CutToolAppearance> _standardToolAppearance = [];
    private bool _cutToolsPowerUser;
    private object? _standardMp4ToolTip;
    private object? _standardOtrToolTip;
    private double _standardPendingLabelFontSize;
    private FontWeight _standardPendingLabelFontWeight;
    private TextWrapping _standardPendingLabelWrapping;

    private sealed record CutToolAppearance(double MinWidth, double Height, Thickness Margin,
        Thickness Padding, HorizontalAlignment Alignment, object Content, DataTemplate? ContentTemplate);

    private void InitializeCutToolLayout()
    {
        _standardMarkerChildren = CutMarkersPanel.Children.Cast<FrameworkElement>().ToArray();
        _standardPendingChildren = PendingStartPanel.Children.Cast<FrameworkElement>().ToArray();
        _standardActionChildren = CutActionsPanel.Children.Cast<FrameworkElement>().ToArray();
        foreach (var button in _standardMarkerChildren.Concat(_standardPendingChildren)
            .Concat(_standardActionChildren).OfType<Button>())
        {
            _standardToolAppearance.Add(button, new(button.MinWidth, button.Height, button.Margin,
                button.Padding, button.HorizontalAlignment, button.Content, button.ContentTemplate));
        }
        _standardMp4ToolTip = CutMp4BoxButton.ToolTip;
        _standardOtrToolTip = CutOtrCanButton.ToolTip;
        _standardPendingLabelFontSize = PendingStartLabel.FontSize;
        _standardPendingLabelFontWeight = PendingStartLabel.FontWeight;
        _standardPendingLabelWrapping = PendingStartLabel.TextWrapping;
        CutContentGrid.LayoutUpdated += (_, _) => UpdateCutToolContentHeight();
    }

    internal void ApplyCutToolLayout(bool powerUser)
    {
        if (_cutToolsPowerUser == powerUser) return;
        _cutToolsPowerUser = powerUser;
        StandardCutHeader.Visibility = CutMarkersPanel.Visibility = CutActionsPanel.Visibility =
            powerUser ? Visibility.Collapsed : Visibility.Visible;
        PowerCutHeader.Visibility = PowerCutMarkersGrid.Visibility = PowerCutActionsGrid.Visibility =
            powerUser ? Visibility.Visible : Visibility.Collapsed;

        if (powerUser)
        {
            var template = (DataTemplate)FindResource("PowerToolButtonContentTemplate");
            foreach (var button in _standardToolAppearance.Keys)
            {
                button.MinWidth = 0;
                button.Height = 36;
                button.Margin = new Thickness(0);
                button.Padding = new Thickness(8, 0, 8, 0);
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.ContentTemplate = template;
            }
            // Reuse only the existing cutting tools. The player and table never change parents.
            MoveCutTool(VideoStartButton, PowerCutMarkersGrid, 0, 0);
            MoveCutTool(CutStartButton, PowerCutMarkersGrid, 0, 2);
            MoveCutTool(CutEndButton, PowerCutMarkersGrid, 2, 0);
            MoveCutTool(VideoEndButton, PowerCutMarkersGrid, 2, 2);
            MoveCutTool(PendingStartLabel, PowerPendingGrid, 0, 0);
            MoveCutTool(PendingStartTime, PowerPendingGrid, 0, 1);
            MoveCutTool(FrameLoupeButton, PowerPendingGrid, 0, 2);
            PendingStartLabel.FontSize = 12;
            PendingStartLabel.FontWeight = FontWeights.Normal;
            PendingStartLabel.TextWrapping = TextWrapping.Wrap;
            FrameLoupeButton.Margin = new Thickness(8, 0, 0, 0);

            MoveCutTool(RemoveCutSegmentButton, PowerCutActionsGrid, 0, 0);
            RemoveCutSegmentButton.MinWidth = 110;
            RemoveCutSegmentButton.HorizontalAlignment = HorizontalAlignment.Right;
            MoveCutTool(CutMp4BoxButton, PowerCutEnginesGrid, 0, 0);
            MoveCutTool(CutOtrCanButton, PowerCutEnginesGrid, 0, 2);
            CutMp4BoxButton.Content = "MP4Box (schnell)";
            CutOtrCanButton.Content = "otr-can (framegenau)";
            CutMp4BoxButton.ToolTip = _standardToolAppearance[CutMp4BoxButton].Content;
            CutOtrCanButton.ToolTip = _standardToolAppearance[CutOtrCanButton].Content;
            MoveCutTool(LoadCutlistButton, PowerCutlistsGrid, 0, 0);
            MoveCutTool(LoadServerCutlistButton, PowerCutlistsGrid, 0, 2);
            MoveCutTool(GenerateCutlistButton, PowerCutlistsGrid, 2, 0);
            MoveCutTool(UploadCutlistButton, PowerCutlistsGrid, 2, 2);
            MoveCutTool(NamingSettingsButton, PowerNamingGrid, 0, 0);
        }
        else
        {
            foreach (var element in _standardMarkerChildren) MoveCutTool(element, CutMarkersPanel);
            foreach (var element in _standardPendingChildren) MoveCutTool(element, PendingStartPanel);
            foreach (var element in _standardActionChildren) MoveCutTool(element, CutActionsPanel);
            foreach (var (button, appearance) in _standardToolAppearance)
            {
                button.MinWidth = appearance.MinWidth;
                button.Height = appearance.Height;
                button.Margin = appearance.Margin;
                button.Padding = appearance.Padding;
                button.HorizontalAlignment = appearance.Alignment;
                button.Content = appearance.Content;
                button.ContentTemplate = appearance.ContentTemplate;
            }
            PendingStartLabel.FontSize = _standardPendingLabelFontSize;
            PendingStartLabel.FontWeight = _standardPendingLabelFontWeight;
            PendingStartLabel.TextWrapping = _standardPendingLabelWrapping;
            CutMp4BoxButton.ToolTip = _standardMp4ToolTip;
            CutOtrCanButton.ToolTip = _standardOtrToolTip;
        }
        UpdateCutToolContentHeight();
    }

    private static void MoveCutTool(FrameworkElement element, Panel target, int row = 0, int column = 0)
    {
        if (element.Parent is Panel parent) parent.Children.Remove(element);
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
        target.Children.Add(element);
    }

    private void UpdateCutToolContentHeight()
    {
        if (!_cutToolsPowerUser)
        {
            CutContentGrid.Height = double.NaN;
            CutContentGrid.MinHeight = 0;
            return;
        }
        // Bound the table to the remaining height so a long cutlist scrolls inside the table.
        // If the fixed tools need more space, the surrounding viewport scrolls instead of clipping.
        var minimum = PowerCutHeader.DesiredSize.Height + PowerCutMarkersGrid.DesiredSize.Height +
            PowerCutActionsGrid.DesiredSize.Height + RemoveSegmentsGrid.MinHeight + RemoveSegmentsGrid.Margin.Top;
        var height = Math.Max(minimum, CutContentScrollViewer.ViewportHeight);
        if (!double.IsFinite(height)) return;
        if (double.IsNaN(CutContentGrid.Height) || Math.Abs(CutContentGrid.Height - height) > 0.5)
        {
            CutContentGrid.Height = height;
        }
        CutContentGrid.MinHeight = 0;
    }
}
