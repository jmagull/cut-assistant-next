using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class OtrCanSettingsViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Func<OtrCanSettings, CancellationToken, Task<IReadOnlyList<OtrCanToolCheckResult>>> _check;
    private readonly Func<OtrCanSettings, Task> _save;
    private string _executablePath;
    private string _ffmsIndexExecutablePath;
    private string _statusMessage = string.Empty;
    private bool _isBusy;
    private bool _isSaving;
    private bool _disposed;
    private CancellationTokenSource? _operation;

    internal OtrCanSettingsViewModel(OtrCanSettings settings,
        Func<OtrCanSettings, CancellationToken, Task<IReadOnlyList<OtrCanToolCheckResult>>> check,
        Func<OtrCanSettings, Task> save)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(check);
        ArgumentNullException.ThrowIfNull(save);
        var normalized = settings.Normalize();
        _executablePath = normalized.ExecutablePath;
        _ffmsIndexExecutablePath = normalized.FfmsIndexExecutablePath;
        _check = check;
        _save = save;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ExecutablePath
    {
        get => _executablePath;
        set => SetPath(ref _executablePath, value);
    }

    public string FfmsIndexExecutablePath
    {
        get => _ffmsIndexExecutablePath;
        set => SetPath(ref _ffmsIndexExecutablePath, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set { _statusMessage = value; Notify(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; Notify(); Notify(nameof(CanEdit)); }
    }

    public bool CanEdit => !IsBusy && !_disposed;
    public bool IsSaving => _isSaving;

    internal OtrCanSettings CreateSettings() => new OtrCanSettings
    {
        ExecutablePath = ExecutablePath,
        FfmsIndexExecutablePath = FfmsIndexExecutablePath
    }.Normalize();

    public async Task CheckAsync()
    {
        if (!CanEdit) return;
        var settings = CreateSettings();
        IsBusy = true;
        StatusMessage = "Werkzeuge werden geprüft …";
        using var operation = new CancellationTokenSource();
        _operation = operation;
        try
        {
            // Path access and starting external programs must stay off the WPF thread.
            var results = await Task.Run(() => _check(settings, operation.Token), operation.Token);
            if (!_disposed)
            {
                StatusMessage = string.Join(Environment.NewLine,
                    results.Select(result => $"{result.Name}: {(result.IsAvailable ? "OK – " : string.Empty)}{result.Message}"));
            }
        }
        catch (OperationCanceledException)
        {
            if (!_disposed) StatusMessage = "Prüfung abgebrochen.";
        }
        catch (Exception error)
        {
            if (!_disposed) StatusMessage = $"Die Prüfung konnte nicht abgeschlossen werden: {error.Message}";
        }
        finally
        {
            _operation = null;
            if (!_disposed) IsBusy = false;
        }
    }

    public async Task<bool> SaveAsync()
    {
        if (!CanEdit) return false;
        var settings = CreateSettings();
        IsBusy = true;
        _isSaving = true;
        StatusMessage = "Einstellungen werden gespeichert …";
        try
        {
            OtrCanToolChecker.ValidateOptionalPath(settings.ExecutablePath);
            OtrCanToolChecker.ValidateOptionalPath(settings.FfmsIndexExecutablePath);
            await Task.Run(() => _save(settings));
            if (!_disposed) StatusMessage = "Einstellungen gespeichert.";
            return true;
        }
        catch (Exception error)
        {
            if (!_disposed) StatusMessage = $"Speichern fehlgeschlagen: {error.Message}";
            return false;
        }
        finally
        {
            _isSaving = false;
            if (!_disposed) IsBusy = false;
        }
    }

    private void SetPath(ref string field, string value, [CallerMemberName] string? name = null)
    {
        if (!CanEdit || field == value) return;
        field = value;
        Notify(name);
        StatusMessage = string.Empty;
    }

    private void Notify([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try { _operation?.Cancel(); }
        catch (ObjectDisposedException) { }
    }
}
