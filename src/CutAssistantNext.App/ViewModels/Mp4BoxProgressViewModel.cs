using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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
        Mp4BoxProgressUpdate update)
    {
        ArgumentNullException.ThrowIfNull(
            update);

        if (update.Kind == Mp4BoxProgressKind.Status)
        {
            StatusText =
                update.Message;
        }

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
        CanCancel = true;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
    }

    public void MarkSucceeded()
    {
        CanCancel = false;

        AutoCloseSecondsRemaining =
            AutoCloseCountdownSeconds;

        ShouldAutoClose = true;
    }

    public void MarkFailed()
    {
        CanCancel = false;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
    }

    public void MarkCancelRequested()
    {
        CanCancel = false;
        ShouldAutoClose = false;
        AutoCloseSecondsRemaining = 0;
        StatusText = "Abbruch wird angefordert …";
    }
    public void MarkCancelled()
    {
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
