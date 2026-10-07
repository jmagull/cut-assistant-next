using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

internal partial class CutCompletionDialog : Window
{
    private readonly CutCompletionViewModel _viewModel;

    internal CutCompletionDialog(CutCompletionViewModel viewModel, string protocolText)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        ProtocolTextBox.Text = protocolText;
        Width = Math.Min(Width, SystemParameters.WorkArea.Width);
        Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        MinWidth = Math.Min(MinWidth, Width);
        MinHeight = Math.Min(MinHeight, Height);
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
