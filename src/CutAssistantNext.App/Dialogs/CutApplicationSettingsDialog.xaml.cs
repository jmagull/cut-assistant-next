using System.Windows;
using CutAssistantNext.App.Settings;
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

    private CancellationTokenSource? _functionCheck;

    protected override void OnClosed(EventArgs e)
    {
        _functionCheck?.Cancel();
        base.OnClosed(e);
    }

    private async void CheckFunctionButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not CutApplicationSettingsViewModel model || _functionCheck is not null) return;
        using var check = new CancellationTokenSource();
        _functionCheck = check;
        FunctionCheckButton.IsEnabled = false;
        FunctionStatus.Text = "MP4Box-Trennen und Zusammenfügen werden geprüft …";
        var path = model.ExecutablePath;
        try
        {
            var result = await Task.Run(() => new ToolFunctionChecks().CheckMp4BoxAsync(path,
                new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault(), force: true, check.Token), check.Token);
            if (!check.IsCancellationRequested)
                FunctionStatus.Text = path == model.ExecutablePath ? result.Message :
                    "Der MP4Box-Pfad wurde geändert. Bitte erneut prüfen.";
        }
        catch (OperationCanceledException) when (check.IsCancellationRequested) { }
        catch (Exception error) { FunctionStatus.Text = "Funktionsprüfung fehlgeschlagen. " + error.Message; }
        finally
        {
            _functionCheck = null;
            FunctionCheckButton.IsEnabled = true;
        }
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
