using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private InterfaceLayout _preferredLayout;
    private InterfaceLayout? _activeLayout;
    private double _cutColumnWidth = 460;
    private double _power2MarkerWidth;
    private bool _layoutUpdateQueued;

    private void InitializeAppearance()
    {
        InitializeCutToolLayout();
        _preferredLayout = new AppearanceSettingsStore().Load().Layout;
        MainContentScrollViewer.SizeChanged += (_, _) => ApplyInterfaceLayout();
        Power2ToolStrip.RequiredMarkerWidthChanged += (_, _) =>
        {
            _power2MarkerWidth = Power2ToolStrip.RequiredMarkerWidth;
            QueueInterfaceLayout();
        };
        Power2EditingDock.SizeChanged += (_, _) => QueueInterfaceLayout();
        ApplyInterfaceLayout();
    }

    private void QueueInterfaceLayout()
    {
        if (_layoutUpdateQueued || _isClosed) return;
        _layoutUpdateQueued = true;
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
        {
            _layoutUpdateQueued = false;
            if (!_isClosed) ApplyInterfaceLayout();
        }));
    }

    private void AppearanceSettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AppearanceSettingsDialog(new AppearanceSettingsStore()) { Owner = this };
        if (dialog.ShowDialog() != true) return;
        _preferredLayout = dialog.Selection.Layout;
        ApplyInterfaceLayout();
    }

    internal void ApplyInterfaceLayout()
    {
        // Resizing keeps the HwndHost in the same visual tree and preserves its HWND.
        var availableWidth = _preferredLayout == InterfaceLayout.PowerUser2 && MainContentScrollViewer.ViewportWidth > 0
            ? MainContentScrollViewer.ViewportWidth : MainContentScrollViewer.ActualWidth;
        var power = _preferredLayout != InterfaceLayout.Standard && availableWidth >= 1100;
        var cutWidth = _activeLayout is InterfaceLayout.PowerUser or InterfaceLayout.PowerUser2
            ? CutColumn.Width.Value : _cutColumnWidth;
        cutWidth = Math.Clamp(cutWidth, 430, Math.Max(430, availableWidth - 618));
        if (_preferredLayout == InterfaceLayout.PowerUser2 && power &&
            _power2MarkerWidth > availableWidth - cutWidth - 8 - PlayerSection.Padding.Left - PlayerSection.Padding.Right)
            power = false;
        var effective = power ? _preferredLayout : InterfaceLayout.Standard;
        MainLayoutGrid.Tag = _preferredLayout != InterfaceLayout.Standard && !power
            ? "Player · Für die gewünschte Power-User-Anordnung das Fenster verbreitern" : "Player";
        if (_activeLayout != effective)
        {
            if (_activeLayout is InterfaceLayout.PowerUser or InterfaceLayout.PowerUser2 && CutColumn.ActualWidth > 0)
                _cutColumnWidth = CutColumn.ActualWidth;
            _activeLayout = effective;
            PlayerColumn.MinWidth = power ? 610 : 0;
            CutColumn.MinWidth = power ? 430 : 0;
            PlayerColumn.Width = new GridLength(1, GridUnitType.Star);
            SplitterColumn.Width = new GridLength(power ? 8 : 0);
            CutColumn.Width = new GridLength(power ? Math.Clamp(_cutColumnWidth, 430, Math.Max(430, availableWidth - 618)) : 0);
            LayoutSplitter.Visibility = power ? Visibility.Visible : Visibility.Collapsed;
            Grid.SetColumnSpan(PlayerSection, power ? 1 : 3);
            Grid.SetRowSpan(PlayerSection, power ? 2 : 1);
            Grid.SetColumn(CutPlanSection, power ? 2 : 0);
            Grid.SetColumnSpan(CutPlanSection, power ? 1 : 3);
            Grid.SetRow(CutPlanSection, power ? 3 : 4);
            Grid.SetRowSpan(CutPlanSection, power ? 2 : 1);
            MainLayoutGrid.RowDefinitions[3].Height = power ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
            MainLayoutGrid.RowDefinitions[4].Height = power ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
            VideoRow.Height = power ? new GridLength(1, GridUnitType.Star) : new GridLength(350);
            ApplyCutToolLayout(effective);
        }
        if (power)
        {
            // A previously widened cut column must still fit after resizing the window.
            CutColumn.Width = new GridLength(Math.Clamp(CutColumn.Width.Value, 430,
                Math.Max(430, availableWidth - 618)));
            PlayerColumn.Width = new GridLength(1, GridUnitType.Star);
        }
        var height = MainContentScrollViewer.ViewportHeight;
        var minimumHeight = effective == InterfaceLayout.PowerUser2 ? 560 : 840;
        MainLayoutGrid.Height = power && double.IsFinite(height) ? Math.Max(minimumHeight, height) : double.NaN;
        UpdateCutToolContentHeight();
    }
}
