using System.ComponentModel;
using System.IO;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class WorkingDirectoryInfoViewModel(IWorkingDirectoryInfoReader? reader = null) : INotifyPropertyChanged, IDisposable
{
    private readonly IWorkingDirectoryInfoReader _reader = reader ?? new WorkingDirectoryInfoReader();
    private CancellationTokenSource? _cancellation;
    private long _generation;
    private bool _disposed;
    public event PropertyChangedEventHandler? PropertyChanged;
    public string ResolvedPath { get; private set; } = "Noch kein Arbeitsordner ausgewählt.";
    public string CapacityText { get; private set; } = "Nicht ermittelt";
    public string FreeText { get; private set; } = "Nicht ermittelt";
    public string FolderSizeText { get; private set; } = "Nicht ermittelt";
    public string Notice { get; private set; } = "";
    public string UpdatedText { get; private set; } = "";
    public bool IsBusy { get; private set; }

    internal async Task RefreshAsync(VideoFolderSettings settings, bool debounce = false)
    {
        if (_disposed) return;
        var generation = ++_generation;
        _cancellation?.Cancel();
        var cancellation = new CancellationTokenSource();
        _cancellation = cancellation;
        var token = cancellation.Token;
        IsBusy = true;
        CapacityText = FreeText = "Wird ermittelt …";
        FolderSizeText = "Wird im Hintergrund berechnet …";
        Notice = UpdatedText = "";
        ResolvedPath = "Kein gültiger Arbeitsordner eingestellt.";
        try
        {
            var path = settings.WorkingDirectoryMode == CutWorkingDirectoryMode.OutputDirectory
                ? settings.CutVideosDirectory.Trim()
                : settings.ResolveWorkingDirectory(Path.Combine(Path.GetTempPath(), "output.mp4"));
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
                throw new ArgumentException(settings.WorkingDirectoryMode == CutWorkingDirectoryMode.OutputDirectory
                    ? "Bitte zuerst einen vollständigen Ausgabeordner für geschnittene Videos einstellen."
                    : "Bitte einen vollständigen benutzerdefinierten Arbeitsordner auswählen.");
            ResolvedPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
            Notify();
            if (debounce) await Task.Delay(250, token);
            var drive = await _reader.ReadDriveAsync(ResolvedPath, token);
            if (!Current()) return;
            CapacityText = Format(drive.CapacityBytes);
            FreeText = Format(drive.FreeBytes);
            Notice = drive.Notice ?? "";
            UpdatedText = "Datenträgerwerte: " + DateTime.Now.ToString("HH:mm:ss");
            Notify();
            var folder = await _reader.ReadFolderAsync(ResolvedPath, token);
            if (!Current()) return;
            FolderSizeText = Format(folder.Bytes);
            Notice = string.Join(Environment.NewLine, new[] { drive.Notice, folder.Notice }.Where(s => !string.IsNullOrWhiteSpace(s)));
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or System.Security.SecurityException)
        {
            if (!Current()) return;
            CapacityText = FreeText = FolderSizeText = "Nicht ermittelbar";
            Notice = error.Message;
            if (error is ArgumentException) ResolvedPath = "Kein gültiger Arbeitsordner eingestellt.";
        }
        finally
        {
            if (Current())
            {
                IsBusy = false;
                _cancellation = null;
                Notify();
            }
            cancellation.Dispose();
        }

        bool Current() => !_disposed && generation == _generation;
    }

    private static string Format(long? bytes) => bytes is { } value ? $"{value / 1073741824d:N2} GiB" : "Nicht ermittelbar";
    private void Notify()
    {
        foreach (var property in new[] { nameof(ResolvedPath), nameof(CapacityText), nameof(FreeText), nameof(FolderSizeText), nameof(Notice), nameof(UpdatedText), nameof(IsBusy) })
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    public void Dispose()
    {
        _disposed = true;
        ++_generation;
        _cancellation?.Cancel();
        _cancellation = null;
    }
}
