using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.ViewModels;

internal sealed class CutlistSearchResultsViewModel : INotifyPropertyChanged
{
    private readonly TaskCompletionSource<bool> _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _generation;
    private bool _serverDone;
    private bool _localDone;
    private string _serverStatus = "Suche läuft …";
    private string _localStatus = "Suche läuft …";
    private CutlistResultRow? _selected;

    internal CutlistSearchResultsViewModel(string movieFileName) => MovieFileName = movieFileName;
    public string MovieFileName { get; }
    public ObservableCollection<CutlistResultRow> Results { get; } = [];
    public string ServerStatus { get => _serverStatus; private set { _serverStatus = value; Changed(); Changed(nameof(Summary)); } }
    public string LocalStatus { get => _localStatus; private set { _localStatus = value; Changed(); Changed(nameof(Summary)); } }
    public string Summary => $"Server: {ServerStatus}    ·    Lokal: {LocalStatus}";
    public bool IsSearching => !_serverDone || !_localDone;
    public bool CanLoad => !IsSearching && SelectedResult?.CanLoad == true;
    public CutlistResultRow? SelectedResult
    {
        get => _selected;
        set { _selected = value; Changed(); Changed(nameof(CanLoad)); }
    }
    internal Task<bool> Ready => _ready.Task;
    public event PropertyChangedEventHandler? PropertyChanged;

    internal async Task SearchAsync(Func<CancellationToken, Task<CutlistSourceResult>> server,
        Func<CancellationToken, Task<CutlistSourceResult>> local, CancellationToken cancellationToken)
    {
        var generation = ++_generation;
        await Task.WhenAll(RunSourceAsync(server, true), RunSourceAsync(local, false));

        async Task RunSourceAsync(Func<CancellationToken, Task<CutlistSourceResult>> search, bool isServer)
        {
            CutlistSourceResult result;
            try { result = await search(cancellationToken).WaitAsync(cancellationToken); }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            { _ready.TrySetCanceled(cancellationToken); return; }
            catch (OperationCanceledException) { result = new([], "Zeitlimit der Anfrage erreicht.", true); }
            catch (Exception) { result = new([], isServer ? "Serveranfrage fehlgeschlagen." : "Lokale Suche fehlgeschlagen.", true); }
            if (generation != _generation || cancellationToken.IsCancellationRequested) return;
            foreach (var row in result.Rows) Results.Add(row);
            if (isServer) { ServerStatus = result.Status; _serverDone = true; }
            else { LocalStatus = result.Status; _localDone = true; }
            Changed(nameof(IsSearching));
            Changed(nameof(CanLoad));
            // No extra empty window on an automatic search without results. Errors remain visible with any other results.
            if (Results.Count > 0 || result.HasIssue) _ready.TrySetResult(true);
            else if (!IsSearching) _ready.TrySetResult(false);
        }
    }

    internal void Invalidate() { ++_generation; _ready.TrySetResult(false); }
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
