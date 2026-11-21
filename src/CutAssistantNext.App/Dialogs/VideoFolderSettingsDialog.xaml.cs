using System.ComponentModel;
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
        Loaded += async (_, _) => await RefreshStorageAsync();
        _viewModel.PropertyChanged += FolderSettingsChanged;
        Width = Math.Min(Width, SystemParameters.WorkArea.Width);
        Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        MinWidth = Math.Min(MinWidth, Width);
        MinHeight = Math.Min(MinHeight, Height);
    }

    protected override void OnClosed(EventArgs e)
    {
        _closed = true;
        _viewModel.PropertyChanged -= FolderSettingsChanged;
        _viewModel.StorageInfo.Dispose();
        base.OnClosed(e);
    }

    private Task RefreshStorageAsync(bool debounce = false) => _closed
        ? Task.CompletedTask : _viewModel.StorageInfo.RefreshAsync(_viewModel.CreateSettings(), debounce);

    private async void RefreshStorage_Click(object sender, RoutedEventArgs e) => await RefreshStorageAsync();

    private async void FolderSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(VideoFolderSettingsViewModel.WorkingDirectoryModeIndex) or
            nameof(VideoFolderSettingsViewModel.CustomWorkingDirectory) or nameof(VideoFolderSettingsViewModel.CutVideosDirectory))
            await RefreshStorageAsync(debounce: true);
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

    private async void BrowseWork_Click(object sender, RoutedEventArgs e)
    {
        var folder = await ChooseFolderAsync("Temporären Arbeitsordner auswählen", _viewModel.CustomWorkingDirectory);
        if (folder is not null) _viewModel.CustomWorkingDirectory = folder;
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
            var error = await Task.Run(() =>
            {
                var validation = VideoFolderPathResolver.GetValidationError(settings);
                if (validation is not null) return validation;
                if (settings.WorkingDirectoryMode != CutWorkingDirectoryMode.OutputDirectory)
                {
                    try
                    {
                        CutAssistantNext.Media.Cutting.CutWorkspace.ValidateWritable(settings.ResolveWorkingDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "output.mp4")));
                    }
                    catch (Exception failure) when (failure is System.IO.IOException or UnauthorizedAccessException)
                    { return "Temporärer Arbeitsordner: " + failure.Message; }
                }
                return null;
            });
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
