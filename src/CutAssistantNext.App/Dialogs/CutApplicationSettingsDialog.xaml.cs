using System.Windows;
using CutAssistantNext.App.ViewModels;
using Microsoft.Win32;

namespace CutAssistantNext.App.Dialogs;

public partial class CutApplicationSettingsDialog : Window
{
    internal CutApplicationSettingsDialog(
        CutApplicationSettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        DataContext =
            viewModel;
    }

    private void BrowseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog =
            new OpenFileDialog
            {
                Title = "Schnittanwendung auswählen",
                Filter =
                    "Programme (*.exe)|*.exe|" +
                    "Alle Dateien (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        if (DataContext is not CutApplicationSettingsViewModel viewModel)
        {
            return;
        }

        viewModel.ExecutablePath =
            dialog.FileName;
    }

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
