using System.IO;
using System.Net.Http;
using System.Windows;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private int _cutlistMediaGeneration;

    private bool IsCurrentCutlistContext(CutlistMediaContext context) => !_isClosed && !_shutdownStarted &&
        context.IsCurrent(_viewModel.FilePath, _viewModel.AnalysisResult, _cutlistMediaGeneration);

    private bool ConfirmCutlistReplacement() => !MediaOpenRequest.NeedsReplacementConfirmation(
        _cutPlanViewModel.RemoveSegments.Count, _cutPlanViewModel.PendingStart.HasValue,
        _cutNamingState is not null || _loadedCutlistAuthor is not null || _loadedIntendedCutApplicationName is not null) ||
        MessageBox.Show(this,
            "Die aktuelle Schnittarbeit durch die ausgewählte Cutlist ersetzen? Bitte bei Bedarf vorher speichern.",
            "Schnittarbeit ersetzen", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;

    private async Task SearchCutlistsForCurrentMediaAsync(bool skipIfNotConfigured = false)
    {
        if (_cutlistSearchInProgress || _shutdownStarted || _isClosed) return;
        if (_viewModel.AnalysisResult is not { Duration: { } duration } analysis || duration <= TimeSpan.Zero)
        {
            if (!skipIfNotConfigured)
                MessageBox.Show(this, "Bitte zuerst eine Mediendatei auswählen und erfolgreich analysieren.",
                    "Cutlists", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _cutlistSearchInProgress = true;
        LoadServerCutlistButton.IsEnabled = false;
        var context = new CutlistMediaContext(_viewModel.FilePath, analysis, _cutlistMediaGeneration);
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_mediaInputCancellation.Token);
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        var client = new CutlistServerClient(http);
        var model = new CutlistSearchResultsViewModel(Path.GetFileName(context.FilePath));
        Task? search = null;
        try
        {
            search = model.SearchAsync(SearchServerAsync, SearchLocalAsync, cancellation.Token);
            if (!await model.Ready || !IsCurrentCutlistContext(context))
            {
                if (!skipIfNotConfigured && IsCurrentCutlistContext(context))
                    MessageBox.Show(this, $"Keine Cutlist gefunden.\nServer: {model.ServerStatus}\nLokal: {model.LocalStatus}",
                        "Cutlists", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new CutlistSearchResultsDialog(model) { Owner = this };
            var accepted = dialog.ShowDialog() == true;
            model.Invalidate();
            cancellation.Cancel();
            await search;
            if (!accepted || dialog.SelectedResult is not { } selected || !IsCurrentCutlistContext(context)) return;

            // The selected document is re-read or downloaded without touching any local cutlist for writing.
            var imported = await CutlistSelectionReader.ReadAsync(selected, client.DownloadBytesAsync, _mediaInputCancellation.Token);
            if (IsCurrentCutlistContext(context))
                LoadCutlistDocument(imported.Document, duration, imported.LocalFilePath);
        }
        catch (OperationCanceledException) when (_mediaInputCancellation.IsCancellationRequested) { }
        catch (Exception error)
        {
            if (IsCurrentCutlistContext(context))
                MessageBox.Show(this, error.Message, "Cutlist konnte nicht geladen werden", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            model.Invalidate();
            cancellation.Cancel();
            if (search is not null) await search;
            _cutlistSearchInProgress = false;
            if (!_isClosed) LoadServerCutlistButton.IsEnabled = true;
        }

        async Task<CutlistSourceResult> SearchServerAsync(CancellationToken token)
        {
            var settings = await Task.Run(() => new CutlistServerSettingsStore().Load(), token);
            if (string.IsNullOrWhiteSpace(settings?.PersonalServerUrl)) return new([], "Nicht eingerichtet");
            if (!CutlistServerSettingsValidator.TryValidatePersonalServerUrl(settings.PersonalServerUrl, out _))
                return new([], "Serveradresse in den Einstellungen ungültig.", true);
            var results = await client.SearchAsync(settings.PersonalServerUrl, Path.GetFileName(context.FilePath), token);
            return new(results.Select(result => CutlistResultRow.FromServer(result, settings.PersonalServerUrl)).ToArray(),
                $"{results.Count} Treffer");
        }

        Task<CutlistSourceResult> SearchLocalAsync(CancellationToken token) => Task.Run(async () =>
        {
            var settings = new VideoFolderSettingsStore().Load();
            return await new LocalCutlistDiscoveryService().SearchAsync(context, settings.OwnCutlistsDirectory, token);
        }, token);
    }
}
