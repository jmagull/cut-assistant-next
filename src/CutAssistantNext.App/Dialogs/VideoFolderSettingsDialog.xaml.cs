using System.Windows;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using Microsoft.Win32;

namespace CutAssistantNext.App.Dialogs;

internal partial class VideoFolderSettingsDialog : Window
{
    private readonly VideoFolderSettingsViewModel _viewModel;
    private readonly VideoFolderSettingsStore _store;
    private bool _closed;

    internal VideoFolderSettingsDialog(VideoFolderSettingsViewModel viewModel, VideoFolderSettingsStore store)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(store);
        _viewModel = viewModel;
        _store = store;
        InitializeComponent();
        DataContext = viewModel;
        Width = Math.Min(Width, SystemParameters.WorkArea.Width);
        Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        MinWidth = Math.Min(MinWidth, Width);
        MinHeight = Math.Min(MinHeight, Height);
    }

    protected override void OnClosed(EventArgs e)
    {
        _closed = true;
        base.OnClosed(e);
    }

    private async void BrowseOriginal_Click(object sender, RoutedEventArgs e)
    {
        var folder = await ChooseFolderAsync("Ordner für Originalvideos auswählen", _viewModel.OriginalVideosDirectory);
        if (folder is not null) _viewModel.OriginalVideosDirectory = folder;
    }

    private async void BrowseCut_Click(object sender, RoutedEventArgs e)
    {
        var folder = await ChooseFolderAsync("Ordner für geschnittene Videos auswählen", _viewModel.CutVideosDirectory);
        if (folder is not null) _viewModel.CutVideosDirectory = folder;
    }

    private async void BrowseCutlists_Click(object sender, RoutedEventArgs e)
    {
        var folder = await ChooseFolderAsync("Ordner für eigene Cutlists auswählen", _viewModel.OwnCutlistsDirectory);
        if (folder is not null) _viewModel.OwnCutlistsDirectory = folder;
    }

    private async Task<string?> ChooseFolderAsync(string title, string currentDirectory)
    {
        FolderFields.IsEnabled = false;
        SaveButton.IsEnabled = false;
        try
        {
            var initialDirectory = await Task.Run(() => VideoFolderPathResolver.Resolve(currentDirectory));
            if (_closed) return null;
            var dialog = new OpenFolderDialog
            {
                Title = title,
                InitialDirectory = initialDirectory,
                Multiselect = false
            };
            return dialog.ShowDialog(this) == true ? dialog.FolderName : null;
        }
        finally
        {
            if (!_closed)
            {
                FolderFields.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
        }
    }

    private void ClearOriginal_Click(object sender, RoutedEventArgs e) =>
        _viewModel.OriginalVideosDirectory = string.Empty;

    private void ClearCut_Click(object sender, RoutedEventArgs e) =>
        _viewModel.CutVideosDirectory = string.Empty;

    private void ClearCutlists_Click(object sender, RoutedEventArgs e) =>
        _viewModel.OwnCutlistsDirectory = string.Empty;

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        var settings = _viewModel.CreateSettings();
        FolderFields.IsEnabled = false;
        SaveButton.IsEnabled = false;
        ErrorText.Text = string.Empty;
        try
        {
            var error = await Task.Run(() => VideoFolderPathResolver.GetValidationError(settings));
            if (_closed) return;
            if (error is not null)
            {
                ErrorText.Text = error;
                return;
            }

            if (!_store.Save(settings))
            {
                ErrorText.Text = "Die Standardordner konnten nicht gespeichert werden. Bitte erneut versuchen.";
                return;
            }

            DialogResult = true;
        }
        finally
        {
            if (!_closed)
            {
                FolderFields.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
        }
    }
}
