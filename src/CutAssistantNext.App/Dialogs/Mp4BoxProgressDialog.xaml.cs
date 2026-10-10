using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

public partial class Mp4BoxProgressDialog : Window
{
    private readonly Mp4BoxProgressViewModel _viewModel;
    private readonly DispatcherTimer _autoCloseTimer;
    private readonly DispatcherTimer _activityTimer;

    private bool _operationCompleted;
    private bool _allowClose;

    internal Mp4BoxProgressDialog(
        Mp4BoxProgressViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        _viewModel =
            viewModel;

        DataContext =
            viewModel;

        _autoCloseTimer =
            new DispatcherTimer
            {
                Interval =
                    TimeSpan.FromSeconds(1)
            };

        _autoCloseTimer.Tick +=
            AutoCloseTimer_Tick;
        _activityTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _activityTimer.Tick += ActivityTimer_Tick;
        Loaded += (_, _) => { if (!_operationCompleted) _activityTimer.Start(); };
        Closed += (_, _) =>
        {
            _activityTimer.Stop();
            _autoCloseTimer.Stop();
            _activityTimer.Tick -= ActivityTimer_Tick;
            _autoCloseTimer.Tick -= AutoCloseTimer_Tick;
        };
    }

    internal event EventHandler? CancelRequested;

    internal void MarkOperationCompleted(
        bool startAutoClose)
    {
        _operationCompleted = true;
        _activityTimer.Stop();
        CloseButton.IsEnabled = true;

        if (startAutoClose &&
            _viewModel.ShouldAutoClose)
        {
            _autoCloseTimer.Start();
        }
    }

    protected override void OnClosing(
        CancelEventArgs e)
    {
        if (_allowClose)
        {
            base.OnClosing(e);
            return;
        }

        if (!_operationCompleted)
        {
            if (_viewModel.CanCancel)
            {
                RequestCancellation();
            }

            e.Cancel = true;
            return;
        }

        _autoCloseTimer.Stop();

        base.OnClosing(e);
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        RequestCancellation();
    }

    private void RequestCancellation()
    {
        if (!_viewModel.CanCancel)
        {
            return;
        }

        _viewModel.MarkCancelRequested();

        CancelRequested?.Invoke(
            this,
            EventArgs.Empty);
    }

    private void CopyProtocolButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var protocolText =
            _viewModel.ProtocolText;

        if (string.IsNullOrEmpty(
            protocolText))
        {
            return;
        }

        Clipboard.SetText(
            protocolText);
    }

    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_operationCompleted)
        {
            return;
        }

        _autoCloseTimer.Stop();
        _allowClose = true;

        Close();
    }

    private void AutoCloseTimer_Tick(
        object? sender,
        EventArgs e)
    {
        _viewModel.TickAutoCloseCountdown();

        if (!_viewModel.IsAutoCloseDue)
        {
            return;
        }

        _autoCloseTimer.Stop();
        _allowClose = true;

        Close();
    }

    private void ActivityTimer_Tick(object? sender, EventArgs e) => _viewModel.RefreshTimings();

    private void ProtocolTextBox_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e)
    {
        ProtocolTextBox.ScrollToEnd();
    }
}
