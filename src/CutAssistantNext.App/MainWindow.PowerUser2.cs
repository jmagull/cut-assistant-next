using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private readonly Dictionary<Button, CutToolAppearance> _standardPlaybackAppearance = [];
    private readonly Dictionary<FrameworkElement, Thickness> _standardPowerMargins = [];
    private (int Row, int Column, int RowSpan, int ColumnSpan, Thickness Margin) _standardCutNavigationPlacement;
    private string _standardPendingLabelText = "";
    private object _standardPendingLabelForeground = DependencyProperty.UnsetValue;
    private object _standardPendingTimeForeground = DependencyProperty.UnsetValue;
    private double _standardPendingTimeWidth;
    private double _standardPendingTimeFontSize;
    private FontWeight _standardPendingTimeFontWeight;
    private DependencyPropertyDescriptor? _pendingBackgroundDescriptor;
    private Thickness _standardKeyframeGroupMargin;
    private VerticalAlignment _standardKeyframeGroupAlignment;

    private void InitializePowerUser2Appearance()
    {
        foreach (var button in PlaybackButtons(PlaybackControls)
            .Where(button => button != SchnittshuffleStartButton && button != SchnittshuffleEndButton))
            _standardPlaybackAppearance.Add(button, CaptureCutTool(button));
        _standardKeyframeGroupMargin = KeyframeButtons.Margin;
        _standardKeyframeGroupAlignment = KeyframeButtons.VerticalAlignment;
        foreach (var element in PowerCutActionsGrid.Children.OfType<FrameworkElement>()
            .Where(element => element is TextBlock or Border).Concat([PowerCutHeader, PowerCutActionsGrid, PowerNamingGrid]))
            _standardPowerMargins.Add(element, element.Margin);
        _standardCutNavigationPlacement = (Grid.GetRow(CutNavigationControls), Grid.GetColumn(CutNavigationControls),
            Grid.GetRowSpan(CutNavigationControls), Grid.GetColumnSpan(CutNavigationControls), CutNavigationControls.Margin);
        _standardPendingLabelText = PendingStartLabel.Text;
        _standardPendingLabelForeground = PendingStartLabel.ReadLocalValue(TextBlock.ForegroundProperty);
        _standardPendingTimeForeground = PendingStartTime.ReadLocalValue(TextBlock.ForegroundProperty);
        _standardPendingTimeWidth = PendingStartTime.Width;
        _standardPendingTimeFontSize = PendingStartTime.FontSize;
        _standardPendingTimeFontWeight = PendingStartTime.FontWeight;
        _cutPlanViewModel.PropertyChanged += CutPlanAppearanceChanged;
        Closed += (_, _) =>
        {
            _pendingBackgroundDescriptor?.RemoveValueChanged(Power2PendingBorder, PendingBackgroundChanged);
            _cutPlanViewModel.PropertyChanged -= CutPlanAppearanceChanged;
        };
    }

    private static IEnumerable<Button> PlaybackButtons(DependencyObject element)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(element).OfType<DependencyObject>())
        {
            if (child is Button button) yield return button;
            else foreach (var nested in PlaybackButtons(child)) yield return nested;
        }
    }

    private static void CompactButton(Button button, bool wrap)
    {
        button.Height = double.NaN;
        button.MinHeight = 30;
        button.MinWidth = 0;
        button.Padding = new Thickness(8, 3, 8, 3);
        button.VerticalAlignment = VerticalAlignment.Center;
        button.Margin = new Thickness(0);
        if (!wrap) button.ContentTemplate = null;
    }

    private void ApplyPowerUser2Tools()
    {
        if (_pendingBackgroundDescriptor is null)
        {
            _pendingBackgroundDescriptor = DependencyPropertyDescriptor.FromProperty(Border.BackgroundProperty, typeof(Border));
            _pendingBackgroundDescriptor.AddValueChanged(Power2PendingBorder, PendingBackgroundChanged);
        }
        // Defer system-resource lookup until this layout is actually used, not during XAML construction.
        Power2PendingBorder.SetResourceReference(Border.BackgroundProperty, SystemColors.WindowBrushKey);
        PowerCutMarkersGrid.Visibility = Visibility.Collapsed;
        Power2EditingDock.Visibility = Visibility.Visible;
        foreach (var button in _standardToolAppearance.Keys) CompactButton(button, true);
        foreach (var button in _standardPlaybackAppearance.Keys)
        {
            CompactButton(button, false);
            button.Margin = new Thickness(0, 0, 4, 4);
        }
        SecondsSeekControls.Width = 144;
        SecondsSeekControls.Height = JumpSecondsPicker.Height = 30;
        SecondsSeekControls.Margin = new Thickness(0, 0, 4, 4);
        SecondsSeekControls.ColumnDefinitions[0].Width = SecondsSeekControls.ColumnDefinitions[2].Width = new GridLength(32);
        JumpBackwardButton.Margin = JumpForwardButton.Margin = new Thickness(0);
        KeyframeButtons.Margin = new Thickness(0, 0, 4, 4);
        KeyframeButtons.VerticalAlignment = VerticalAlignment.Center;
        foreach (var button in KeyframeButtons.Children.OfType<Button>())
            button.Margin = new Thickness(0, 0, 4, 0);
        var markers = new[] { VideoStartButton, CutStartButton, CutEndButton, VideoEndButton };
        for (var index = 0; index < markers.Length; index++)
        {
            CompactButton(markers[index], false);
            MoveCutTool(markers[index], Power2MarkersGrid, 0, index * 2);
        }
        MoveCutTool(PendingStartLabel, Power2PendingText);
        MoveCutTool(PendingStartTime, Power2PendingText);
        MoveCutTool(FrameLoupeButton, Power2PendingGrid, 0, 1);
        FrameLoupeButton.Margin = new Thickness(8, 0, 0, 0);
        PendingStartTime.Width = double.NaN;
        MoveCutTool(CutNavigationControls, Power2ToolStrip);
        CutNavigationControls.Margin = new Thickness(0);
        foreach (var element in _standardPowerMargins.Keys)
        {
            var margin = _standardPowerMargins[element];
            element.Margin = new Thickness(margin.Left, Math.Min(6, margin.Top), margin.Right, Math.Min(6, margin.Bottom));
        }
        PowerCutlistsGrid.RowDefinitions[1].Height = new GridLength(4);
        UpdatePower2PendingAppearance();
    }

    private void RestorePowerUser2Appearance()
    {
        Power2EditingDock.Visibility = Visibility.Collapsed;
        foreach (var (button, appearance) in _standardPlaybackAppearance) RestoreCutTool(button, appearance);
        SecondsSeekControls.Width = 190;
        SecondsSeekControls.Height = JumpSecondsPicker.Height = 36;
        SecondsSeekControls.Margin = new Thickness(0, 0, 10, 8);
        SecondsSeekControls.ColumnDefinitions[0].Width = SecondsSeekControls.ColumnDefinitions[2].Width = new GridLength(40);
        KeyframeButtons.Margin = _standardKeyframeGroupMargin;
        KeyframeButtons.VerticalAlignment = _standardKeyframeGroupAlignment;
        MoveCutTool(CutNavigationControls, PlaybackControls, _standardCutNavigationPlacement.Row, _standardCutNavigationPlacement.Column);
        Grid.SetRowSpan(CutNavigationControls, _standardCutNavigationPlacement.RowSpan);
        Grid.SetColumnSpan(CutNavigationControls, _standardCutNavigationPlacement.ColumnSpan);
        CutNavigationControls.Margin = _standardCutNavigationPlacement.Margin;
        foreach (var (element, margin) in _standardPowerMargins) element.Margin = margin;
        PowerCutlistsGrid.RowDefinitions[1].Height = new GridLength(8);
        PendingStartLabel.Text = _standardPendingLabelText;
        RestoreForeground(PendingStartLabel, _standardPendingLabelForeground);
        RestoreForeground(PendingStartTime, _standardPendingTimeForeground);
        PendingStartTime.Width = _standardPendingTimeWidth;
        PendingStartTime.FontSize = _standardPendingTimeFontSize;
        PendingStartTime.FontWeight = _standardPendingTimeFontWeight;
    }

    private static void RestoreForeground(TextBlock text, object value)
    {
        if (value == DependencyProperty.UnsetValue) text.ClearValue(TextBlock.ForegroundProperty);
        else text.SetValue(TextBlock.ForegroundProperty, value);
    }

    private void CutPlanAppearanceChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(CutPlanViewModel.PendingStart) or nameof(CutPlanViewModel.EditingMode))
            UpdatePower2PendingAppearance();
    }

    private void PendingBackgroundChanged(object? sender, EventArgs args) => UpdatePower2PendingAppearance();

    internal static Brush Power2PendingBrush(bool keep, Color background, bool highContrast)
    {
        if (highContrast) return SystemColors.WindowTextBrush;
        var dark = .2126 * background.R + .7152 * background.G + .0722 * background.B < 128;
        var brush = new SolidColorBrush(keep
            ? dark ? Color.FromRgb(134, 239, 172) : Color.FromRgb(22, 101, 52)
            : dark ? Color.FromRgb(252, 165, 165) : Color.FromRgb(185, 28, 28));
        brush.Freeze();
        return brush;
    }

    private void UpdatePower2PendingAppearance()
    {
        if (_cutToolsLayout != InterfaceLayout.PowerUser2) return;
        var pending = _cutPlanViewModel.PendingStart.HasValue;
        var background = (Power2PendingBorder.Background as SolidColorBrush)?.Color ?? SystemColors.WindowColor;
        var foreground = pending ? Power2PendingBrush(_cutPlanViewModel.IsKeepMode, background, SystemParameters.HighContrast)
            : SystemColors.WindowTextBrush;
        PendingStartLabel.Text = pending ? _cutPlanViewModel.IsKeepMode
            ? "BEHALTEANFANG VORGEMERKT:" : "SCHNITTANFANG VORGEMERKT:" : _standardPendingLabelText;
        PendingStartLabel.FontSize = PendingStartTime.FontSize = pending ? 14 : _standardPendingTimeFontSize;
        PendingStartLabel.FontWeight = PendingStartTime.FontWeight = pending ? FontWeights.Bold : FontWeights.Normal;
        PendingStartLabel.TextWrapping = TextWrapping.NoWrap;
        PendingStartLabel.Foreground = PendingStartTime.Foreground = foreground;
        Power2PendingBorder.BorderBrush = pending ? foreground : SystemColors.ControlDarkBrush;
    }
}
