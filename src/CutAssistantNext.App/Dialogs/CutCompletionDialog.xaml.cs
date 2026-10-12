using System.Runtime.InteropServices;
using CutAssistantNext.App.Settings;
using System.Windows;
using System.Windows.Input;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

internal partial class CutCompletionDialog : Window
{
    private readonly CutCompletionViewModel _viewModel;
    private readonly Func<CutCompletionViewModel, int, Task>? _reviewAsync;

    internal CutCompletionDialog(CutCompletionViewModel viewModel, string protocolText, WindowSettingsStore? windowSettingsStore = null,
        Func<CutCompletionViewModel, int, Task>? reviewAsync = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        _reviewAsync = reviewAsync;
        InitializeComponent();
        DataContext = viewModel;
        ProtocolTextBox.Text = protocolText;
        DialogWindowPersistence.Attach(this, CutDialogKind.Completion, windowSettingsStore);
    }

    private async void ReviewButton_Click(object sender, RoutedEventArgs e)
    {
        if (_reviewAsync is null || sender is not System.Windows.Controls.Button button ||
            button.DataContext is not CutCompletionCheckPoint { CheckPointIndex: int index }) return;
        button.IsEnabled = false;
        try { await _reviewAsync(_viewModel, index); }
        catch (Exception exception) { CopyStatusText.Text = "Kontrolle nicht möglich: " + exception.Message; }
        finally { button.IsEnabled = true; }
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(_viewModel.ClipboardText);
            CopyStatusText.Text = "Hinweise kopiert.";
        }
        catch (ExternalException)
        {
            CopyStatusText.Text = "Die Zwischenablage ist gerade belegt. Bitte erneut kopieren.";
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
        }
    }
}
