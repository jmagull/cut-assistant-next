using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.App.ViewModels;

public sealed class Mp4BoxProgressViewModel :
    INotifyPropertyChanged
{
    private const int AutoCloseCountdownSeconds =
        20;

    private readonly StringBuilder _protocolBuilder =
        new();

    private string _statusText =
        "Bereit.";

    private bool _canCancel;
    private bool _hasFailed;
    private readonly TimeProvider _clock;
    private bool _running;
    private bool _completed;
    private bool _cancelRequested;
    private long _started;
    private long _phaseStarted;
    private long _lastMessage;
    private TimeSpan _elapsed;
    private double? _percentage;
    private string _outcome = "Bereit";

    public Mp4BoxProgressViewModel() : this(TimeProvider.System) { }

    internal Mp4BoxProgressViewModel(TimeProvider clock)
    {
        _clock = clock;
    }

    public bool IsIndeterminate => _running && !_percentage.HasValue;
    public double ProgressValue => _percentage ?? 0;
    public string ProgressText => _running
        ? _percentage.HasValue ? $"{_percentage.Value:0}% dieses Arbeitsschritts" : "Verarbeitung läuft …"
        : _outcome;
    public string TimingText => _running
        ? $"Laufzeit: {Format(_elapsed)} · Schritt: {Format(_clock.GetElapsedTime(_phaseStarted))} · letzte Meldung vor {Format(_clock.GetElapsedTime(_lastMessage))}"
        : $"Laufzeit: {Format(_elapsed)}";

    public void RefreshTimings()
    {
        if (!_running) return;
        _elapsed = _clock.GetElapsedTime(_started);
        OnPropertyChanged(nameof(TimingText));
    }

    private static string Format(TimeSpan time) => $"{(long)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";

    private void NotifyActivity()
    {
        OnPropertyChanged(nameof(IsIndeterminate));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(TimingText));
    }

    private void StopActivity(string outcome)
    {
        RefreshTimings();
        _running = false;
        _completed = true;
        _percentage = outcome == "Abgeschlossen" ? 100 : null;
        _outcome = outcome;
        NotifyActivity();
    }

    private bool _shouldAutoClose;

    private int _autoCloseSecondsRemaining;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string StatusText
    {
        get => _statusText;

        private set
        {
            if (_statusText == value)
            {
                return;
            }

            _statusText =
                value;

            OnPropertyChanged();
        }
    }

    public string ProtocolText =>
        _protocolBuilder.ToString();

    public bool CanCancel
    {
        get => _canCancel;

        private set
        {
            if (_canCancel == value)
            {
                return;
            }

            _canCancel =
                value;

            OnPropertyChanged();
        }
    }

    public bool ShouldAutoClose
    {
        get => _shouldAutoClose;

        private set
        {
            if (_shouldAutoClose == value)
            {
                return;
            }

            _shouldAutoClose =
                value;

            OnPropertyChanged();
            OnPropertyChanged(
                nameof(IsAutoCloseDue));
        }
    }

    public int AutoCloseSecondsRemaining
    {
        get => _autoCloseSecondsRemaining;

        private set
        {
            if (_autoCloseSecondsRemaining == value)
            {
                return;
            }

            _autoCloseSecondsRemaining =
                value;

            OnPropertyChanged();
            OnPropertyChanged(
                nameof(CloseButtonText));
            OnPropertyChanged(
                nameof(IsAutoCloseDue));
        }
    }

    public string CloseButtonText =>
        AutoCloseSecondsRemaining > 0
            ? $"Schließen ({AutoCloseSecondsRemaining})"
            : "Schließen";

    public bool IsAutoCloseDue =>
        ShouldAutoClose &&
        AutoCloseSecondsRemaining == 0;

    public void ApplyProgress(
        CutProgressUpdate update)
    {
        ArgumentNullException.ThrowIfNull(
            update);

        if (_running) _lastMessage = _clock.GetTimestamp();
        if ((update.Kind == CutProgressKind.Status || update.Kind == CutProgressKind.Progress) &&
            !_hasFailed && !_completed && !_cancelRequested)
        {
            if (StatusText != update.Message) _phaseStarted = _clock.GetTimestamp();
            StatusText =
                update.Message;
            _percentage = update.Percentage is { } value && double.IsFinite(value) && value >= 0 && value <= 100
                ? value : null;
            NotifyActivity();
        }

        // Translated progress is displayed above; its original line is already in the protocol.
        if (update.Kind == CutProgressKind.Progress) return;

        if (_protocolBuilder.Length > 0)
        {
            _protocolBuilder.AppendLine();
        }

        _protocolBuilder.Append(
            update.Message);

        OnPropertyChanged(
            nameof(ProtocolText));
    }

    public void MarkRunning()
    {
        _hasFailed = false;
        _completed = false;
        _cancelRequested = false;
        _running = true;
        _started = _phaseStarted = _lastMessage = _clock.GetTimestamp();
        _elapsed = TimeSpan.Zero;
        _percentage = null;
        NotifyActivity();
        CanCancel = true;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
    }

    public void MarkSucceeded()
    {
        StopActivity("Abgeschlossen");
        StatusText = "Schneiden abgeschlossen.";
        CanCancel = false;

        AutoCloseSecondsRemaining =
            AutoCloseCountdownSeconds;

        ShouldAutoClose = true;
    }

    public void MarkFailed(string? reason = null)
    {
        StopActivity("Fehlgeschlagen");
        _hasFailed = true;
        StatusText = string.IsNullOrWhiteSpace(reason)
            ? "Vorbereitung oder Schnitt fehlgeschlagen. Weitere Informationen stehen im Protokoll."
            : $"Vorbereitung oder Schnitt fehlgeschlagen:{Environment.NewLine}{reason.Trim()}";
        ApplyProgress(new CutProgressUpdate(CutProgressKind.Output, StatusText));
        CanCancel = false;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
    }

    public void MarkCancelRequested()
    {
        _cancelRequested = true;
        _percentage = null;
        NotifyActivity();
        CanCancel = false;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
        StatusText = "Abbruch wird angefordert …";
    }
    public void MarkCancelled()
    {
        StopActivity("Abgebrochen");
        StatusText = "Abgebrochen.";
        CanCancel = false;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
    }

    public void TickAutoCloseCountdown()
    {
        if (!ShouldAutoClose ||
            AutoCloseSecondsRemaining <= 0)
        {
            return;
        }

        AutoCloseSecondsRemaining--;
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}
