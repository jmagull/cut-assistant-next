using System.Windows;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private bool ConfirmKeepSelection()
    {
        if (!_cutPlanViewModel.IsKeepMode) return true;
        if (_cutPlanViewModel.PendingStart.HasValue)
        {
            MessageBox.Show(this, CutlistSaveGuard.Message, CutlistSaveGuard.Title,
                MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }
        return MessageBox.Show(this, _cutPlanViewModel.KeepResultConfirmation, "Behaltebereiche verwenden",
            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;
    }
}
