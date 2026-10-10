using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using Microsoft.Win32;

namespace CutAssistantNext.App.Dialogs;

public partial class OtrCanSettingsDialog : Window
{
    private readonly OtrCanSettingsViewModel _viewModel;

    internal OtrCanSettingsDialog(OtrCanSettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Closing += OnClosing;
        Closed += (_, _) => viewModel.Dispose();
    }

    private void BrowseEngine_Click(object sender, RoutedEventArgs e) => Browse(indexer: false);
    private void BrowseIndexer_Click(object sender, RoutedEventArgs e) => Browse(indexer: true);

    private void Browse(bool indexer)
    {
        var dialog = new OpenFileDialog
        {
            Title = indexer ? "ffmsindex.exe auswählen" : "OTR-CAN-Schnittmotor auswählen",
            Filter = indexer ? "ffmsindex.exe|ffmsindex.exe|Programme (*.exe)|*.exe" : "Programme (*.exe)|*.exe",
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != true) return;
        if (indexer) _viewModel.FfmsIndexExecutablePath = dialog.FileName;
        else _viewModel.ExecutablePath = dialog.FileName;
    }

    private async void Check_Click(object sender, RoutedEventArgs e) => await _viewModel.CheckAsync();

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (await _viewModel.SaveAsync()) DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        // A check can be cancelled; an already authorized save must finish before closing.
        if (_viewModel.IsSaving) e.Cancel = true;
    }

    private void DownloadLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        e.Handled = true;
        if (e.Uri.AbsoluteUri != OtrCanToolChecker.Ffms2DownloadUrl) return;
        try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); }
        catch (Exception error)
        {
            MessageBox.Show(this, $"Der Download-Link konnte nicht geöffnet werden: {error.Message}",
                "FFMS2-Download", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
