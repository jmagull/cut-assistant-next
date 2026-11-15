using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class FfmpegSettingsViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Func<FfmpegSettings, CancellationToken, Task<IReadOnlyList<FfmpegToolCheckResult>>> _check;
    private string _ffprobeExecutablePath;
    private string _ffmpegExecutablePath;
    private bool _disposed;
    private CancellationTokenSource? _operation;

    public FfmpegSettingsViewModel(FfmpegSettings settings,
        Func<FfmpegSettings, CancellationToken, Task<IReadOnlyList<FfmpegToolCheckResult>>>? check = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _ffprobeExecutablePath = settings.FfprobeExecutablePath;
        _ffmpegExecutablePath = settings.FfmpegExecutablePath;
        var checker = new FfmpegToolChecker();
        _check = check ?? checker.CheckAsync;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public bool IsBusy { get; private set; }
    public bool CanEdit => !IsBusy && !_disposed;
    public string StatusMessage { get; private set; } = string.Empty;

    public string FfprobeExecutablePath
    {
        get => _ffprobeExecutablePath;
        set => SetPath(ref _ffprobeExecutablePath, value);
    }

    public string FfmpegExecutablePath
    {
        get => _ffmpegExecutablePath;
        set => SetPath(ref _ffmpegExecutablePath, value);
    }

    public FfmpegSettings CreateSettings() => new()
    {
        FfprobeExecutablePath = FfprobeExecutablePath,
        FfmpegExecutablePath = FfmpegExecutablePath
    };

    public Task CheckAsync() => CheckCoreAsync(function: false);

    public Task CheckFunctionAsync() => CheckCoreAsync(function: true);

    private async Task CheckCoreAsync(bool function)
    {
        if (!CanEdit) return;
        var settings = CreateSettings();
        using var operation = new CancellationTokenSource();
        _operation = operation;
        IsBusy = true;
        StatusMessage = function ? "H.264/AAC-Streamcopy wird geprüft …" : "Versionen werden geprüft …";
        PublishState();
        try
        {
            // Resolving paths and launching external tools must not block the WPF thread.
            if (function)
            {
                var result = await Task.Run(() => new ToolFunctionChecks().CheckFfmpegAsync(settings,
                    force: true, operation.Token), operation.Token);
                if (!_disposed) StatusMessage = result.Message;
            }
            else
            {
                var results = await Task.Run(() => _check(settings, operation.Token), operation.Token);
                if (!_disposed)
                {
                    StatusMessage = string.Join(Environment.NewLine, results.Select(result =>
                        $"{result.Name}: {(result.IsAvailable ? "OK – " : string.Empty)}{result.Message}"));
                    if (results.All(result => result.IsAvailable && result.BuildIdentity is not null))
                    {
                        if (!FfmpegToolChecker.IsMatchingPair(results))
                            StatusMessage += Environment.NewLine + "FFmpeg und ffprobe stammen aus unterschiedlichen Builds.";
                        else
                        {
                            var checks = new ToolFunctionChecks();
                            var known = await checks.GetKnownResultAsync(ToolFunction.FfmpegStreamcopy,
                                checks.FfmpegPaths(settings), operation.Token);
                            if (!_disposed) StatusMessage += Environment.NewLine + "Funktionsstatus: " +
                                (known is null ? "noch nicht geprüft." : known.Message);
                        }
                    }
                }
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
            IsBusy = false;
            if (!_disposed) PublishState();
        }
    }

    private void SetPath(ref string field, string value, [CallerMemberName] string? name = null)
    {
        if (!CanEdit || field == value) return;
        bool hadStatus = StatusMessage.Length > 0;
        field = value;
        StatusMessage = string.Empty;
        OnPropertyChanged(name);
        if (hadStatus) OnPropertyChanged(nameof(StatusMessage));
    }

    private void PublishState()
    {
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(StatusMessage));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try { _operation?.Cancel(); }
        catch (ObjectDisposedException) { }
        OnPropertyChanged(nameof(CanEdit));
    }
}
