using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private InterfaceLayout _preferredLayout;
    private bool? _powerLayoutActive;
    private double _cutColumnWidth = 460;

    private void InitializeAppearance()
    {
        InitializeCutToolLayout();
        _preferredLayout = new AppearanceSettingsStore().Load().Layout;
        MainContentScrollViewer.SizeChanged += (_, _) => ApplyInterfaceLayout();
        ApplyInterfaceLayout();
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
        var power = _preferredLayout == InterfaceLayout.PowerUser && MainContentScrollViewer.ActualWidth >= 1100;
        MainLayoutGrid.Tag = _preferredLayout == InterfaceLayout.PowerUser && !power
            ? "Player · Für die Power-User-Anordnung das Fenster verbreitern" : "Player";
        if (_powerLayoutActive != power)
        {
            if (_powerLayoutActive == true) _cutColumnWidth = CutColumn.ActualWidth;
            _powerLayoutActive = power;
            PlayerColumn.MinWidth = power ? 610 : 0;
            CutColumn.MinWidth = power ? 430 : 0;
            PlayerColumn.Width = new GridLength(1, GridUnitType.Star);
            SplitterColumn.Width = new GridLength(power ? 8 : 0);
            CutColumn.Width = new GridLength(power ? Math.Clamp(_cutColumnWidth, 430, Math.Max(430, MainContentScrollViewer.ActualWidth - 618)) : 0);
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
            ApplyCutToolLayout(power);
        }
        if (power)
        {
            // A previously widened cut column must still fit after resizing the window.
            CutColumn.Width = new GridLength(Math.Clamp(CutColumn.Width.Value, 430,
                Math.Max(430, MainContentScrollViewer.ActualWidth - 618)));
            PlayerColumn.Width = new GridLength(1, GridUnitType.Star);
        }
        var height = MainContentScrollViewer.ViewportHeight;
        MainLayoutGrid.Height = power && double.IsFinite(height) ? Math.Max(840, height) : double.NaN;
        UpdateCutToolContentHeight();
    }
}
