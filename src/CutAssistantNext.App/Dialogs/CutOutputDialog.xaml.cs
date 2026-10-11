using System.Windows;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

public partial class CutOutputDialog : Window
{
    internal CutOutputDialog(
        CutOutputViewModel viewModel, WindowSettingsStore? windowSettingsStore = null)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();
        DialogWindowPersistence.Attach(this, CutDialogKind.Output, windowSettingsStore);

        DataContext =
            viewModel;
    }

    internal event EventHandler? CutRequested;

    private void CutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        CutRequested?.Invoke(
            this,
            EventArgs.Empty);
    }

    private void GenerateFromTemplateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is CutOutputViewModel viewModel)
        {
            viewModel.GenerateSuggestedMovieName();
        }
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
