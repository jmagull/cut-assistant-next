using System.Windows;
using CutAssistantNext.App.ViewModels;
using Microsoft.Win32;

namespace CutAssistantNext.App.Dialogs;

public partial class FfmpegSettingsDialog : Window
{
    internal FfmpegSettingsDialog(
        FfmpegSettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        DataContext =
            viewModel;
    }

    private void BrowseFfprobeButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog =
            new OpenFileDialog
            {
                Title = "ffprobe.exe auswählen",
                Filter =
                    "ffprobe.exe|ffprobe.exe|" +
                    "Programme (*.exe)|*.exe|" +
                    "Alle Dateien (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        if (DataContext is not FfmpegSettingsViewModel viewModel)
        {
            return;
        }

        viewModel.FfprobeExecutablePath =
            dialog.FileName;
    }

    private void BrowseFfmpegButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog =
            new OpenFileDialog
            {
                Title = "ffmpeg.exe auswählen",
                Filter =
                    "ffmpeg.exe|ffmpeg.exe|" +
                    "Programme (*.exe)|*.exe|" +
                    "Alle Dateien (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        if (DataContext is not FfmpegSettingsViewModel viewModel)
        {
            return;
        }

        viewModel.FfmpegExecutablePath =
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
