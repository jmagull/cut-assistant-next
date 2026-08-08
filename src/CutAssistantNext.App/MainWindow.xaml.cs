using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;
using Microsoft.Win32;

namespace CutAssistantNext.App;

public partial class MainWindow : Window
{
    private readonly IAppLogger _logger;
    private readonly WindowSettingsStore _windowSettingsStore;
    private readonly MainWindowViewModel _viewModel;
    private readonly IMediaPlayerService _mediaPlayerService;
    private readonly PlaybackViewModel _playbackViewModel;
    private readonly CutPlanViewModel _cutPlanViewModel;

    private Task? _mediaPlayerInitializationTask;
    private bool _isClosed;
    private bool _shutdownStarted;
    private bool _allowClose;

    public MainWindow()
        : this(NullAppLogger.Instance)
    {
    }

    internal MainWindow(IAppLogger logger)
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        InitializeComponent();

        _windowSettingsStore =
            new WindowSettingsStore();

        RestoreWindowSettings();

        _mediaPlayerService =
            new MpvMediaPlayerService(_logger);

        _playbackViewModel =
            new PlaybackViewModel(_mediaPlayerService);

        _cutPlanViewModel =
            new CutPlanViewModel();

        _viewModel = new MainWindowViewModel(
            new FfprobeRunner(),
            _logger);

        DataContext = _viewModel;
        PlaybackControls.DataContext = _playbackViewModel;
        CutPlanSection.DataContext = _cutPlanViewModel;
        CutTimelineTrack.DataContext = _cutPlanViewModel;

        VideoHost.VideoWindowHandleCreated +=
            VideoHost_VideoWindowHandleCreated;

        Closing += MainWindow_Closing;
        PreviewKeyDown += MainWindow_PreviewKeyDown;

        if (VideoHost.IsVideoWindowReady)
        {
            _ = EnsureMediaPlayerInitializedAsync();
        }
    }

    private async void VideoHost_VideoWindowHandleCreated(
        object? sender,
        EventArgs e)
    {
        await EnsureMediaPlayerInitializedAsync();
    }

    private Task EnsureMediaPlayerInitializedAsync()
    {
        if (_isClosed ||
            !VideoHost.IsVideoWindowReady)
        {
            return Task.CompletedTask;
        }

        _mediaPlayerInitializationTask ??=
            InitializeMediaPlayerCoreAsync(
                VideoHost.VideoWindowHandle);

        return _mediaPlayerInitializationTask;
    }

    private async Task InitializeMediaPlayerCoreAsync(
        nint videoWindowHandle)
    {
        try
        {
            await _mediaPlayerService.InitializeAsync(
                videoWindowHandle);
        }
        catch (Exception exception)
        {
            if (!_isClosed)
            {
                MessageBox.Show(
                    this,
                    $"libmpv konnte nicht initialisiert werden:" +
                    $"{Environment.NewLine}{exception.Message}",
                    "Wiedergabeinitialisierung fehlgeschlagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private async void MainWindow_Closing(
        object? sender,
        CancelEventArgs e)
    {
        if (_allowClose)
        {
            return;
        }

        e.Cancel = true;

        if (_shutdownStarted)
        {
            return;
        }

        _shutdownStarted = true;
        _isClosed = true;

        SaveWindowSettings();

        VideoHost.VideoWindowHandleCreated -=
            VideoHost_VideoWindowHandleCreated;

        PreviewKeyDown -= MainWindow_PreviewKeyDown;

        _playbackViewModel.Dispose();

        try
        {
            if (_mediaPlayerInitializationTask is not null)
            {
                await _mediaPlayerInitializationTask;
            }

            await _mediaPlayerService.DisposeAsync();
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Fehler beim Freigeben von libmpv: " +
                $"{exception.Message}");
        }
        finally
        {
            _allowClose = true;

            Closing -= MainWindow_Closing;

            Close();
        }
    }

    private void RestoreWindowSettings()
    {
        var settings =
            _windowSettingsStore.Load();

        if (settings is null)
        {
            return;
        }

        if (double.IsFinite(settings.Width) &&
            settings.Width >= MinWidth)
        {
            Width = Math.Max(
                MinWidth,
                Math.Min(
                    settings.Width,
                    SystemParameters.WorkArea.Width));
        }

        if (double.IsFinite(settings.Height) &&
            settings.Height >= MinHeight)
        {
            Height = Math.Max(
                MinHeight,
                Math.Min(
                    settings.Height,
                    SystemParameters.WorkArea.Height));
        }

        if (settings.IsMaximized)
        {
            WindowState =
                System.Windows.WindowState.Maximized;
        }
    }

    private void SaveWindowSettings()
    {
        var bounds = RestoreBounds;

        var width =
            double.IsFinite(bounds.Width) &&
            bounds.Width > 0
                ? bounds.Width
                : ActualWidth;

        var height =
            double.IsFinite(bounds.Height) &&
            bounds.Height > 0
                ? bounds.Height
                : ActualHeight;

        _windowSettingsStore.Save(
            new WindowSettings
            {
                Width = width,
                Height = height,
                IsMaximized =
                    WindowState ==
                    System.Windows.WindowState.Maximized
            });
    }

    private void TimelineSlider_PreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (!IsInitialized)
        {
            return;
        }

        _playbackViewModel.BeginSeek();
    }

    private void TimelineSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsInitialized)
        {
            return;
        }

        _playbackViewModel.UpdateSeekPosition(
            e.NewValue);
    }

    private async void TimelineSlider_PreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (!IsInitialized)
        {
            return;
        }

        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.CommitSeekAsync());
    }

    private async void VolumeSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsInitialized ||
            !_playbackViewModel.CanSetVolume)
        {
            return;
        }

        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.SetVolumeAsync(
                e.NewValue));
    }

    private async void MainWindow_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        var modifiers = Keyboard.Modifiers;

        if (e.Key == Key.Space &&
            modifiers == ModifierKeys.None &&
            _playbackViewModel.CanTogglePlayback)
        {
            e.Handled = true;

            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.TogglePlaybackAsync());

            return;
        }

        if (!_playbackViewModel.CanStepFrame)
        {
            return;
        }

        if (e.Key == Key.Left &&
            modifiers == ModifierKeys.None)
        {
            e.Handled = true;

            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepBackwardAsync());

            return;
        }

        if (e.Key == Key.Right &&
            modifiers == ModifierKeys.None)
        {
            e.Handled = true;

            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepForwardAsync());

            return;
        }

        if (e.Key == Key.Left &&
            modifiers == ModifierKeys.Control)
        {
            e.Handled = true;

            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepBackwardTenFramesAsync());

            return;
        }

        if (e.Key == Key.Right &&
            modifiers == ModifierKeys.Control)
        {
            e.Handled = true;

            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepForwardTenFramesAsync());
        }
    }

    private async void PlayButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.TogglePlaybackAsync());
    }

    private async void StepBackwardTenFramesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.StepBackwardTenFramesAsync());
    }

    private async void StepBackwardButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.StepBackwardAsync());
    }

    private async void StepForwardButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.StepForwardAsync());
    }

    private async void StepForwardTenFramesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.StepForwardTenFramesAsync());
    }

    private void SetCutStartButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                _cutPlanViewModel.SetStart(
                    _playbackViewModel.Position);

                _logger.Information(
                    $"Schnittanfang wurde gesetzt: " +
                    $"{_playbackViewModel.Position:c}");
            });
    }

    private void SetCutEndButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                var endPosition =
                    _playbackViewModel.Position;

                _cutPlanViewModel.SetEnd(
                    endPosition);

                _logger.Information(
                    $"Schnittende wurde gesetzt: " +
                    $"{endPosition:c}");
            });
    }

    private void CorrectCutStartButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                var selectedSegment =
                    _cutPlanViewModel.SelectedRemoveSegment
                    ?? throw new InvalidOperationException(
                        "Es ist kein Schnittbereich ausgewählt.");

                var position =
                    _playbackViewModel.Position;

                if (position >= selectedSegment.End)
                {
                    throw new InvalidOperationException(
                        "Die aktuelle Videoposition liegt nicht vor dem Ende des ausgewählten Schnittbereichs. " +
                        "Verschiebe die Zeitleiste auf die gewünschte neue Anfangsposition.");
                }
                _cutPlanViewModel.Replace(
                    selectedSegment,
                    position,
                    selectedSegment.End);

                _logger.Information(
                    $"Schnittanfang wurde korrigiert: " +
                    $"{selectedSegment.Start:c} -> {position:c}");
            });
    }

    private void CorrectCutEndButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                var selectedSegment =
                    _cutPlanViewModel.SelectedRemoveSegment
                    ?? throw new InvalidOperationException(
                        "Es ist kein Schnittbereich ausgewählt.");

                var position =
                    _playbackViewModel.Position;

                if (position <= selectedSegment.Start)
                {
                    throw new InvalidOperationException(
                        "Die aktuelle Videoposition liegt nicht hinter dem Anfang des ausgewählten Schnittbereichs. " +
                        "Verschiebe die Zeitleiste auf die gewünschte neue Endposition.");
                }
                _cutPlanViewModel.Replace(
                    selectedSegment,
                    selectedSegment.Start,
                    position);

                _logger.Information(
                    $"Schnittende wurde korrigiert: " +
                    $"{selectedSegment.End:c} -> {position:c}");
            });
    }

    private void RemoveCutSegmentButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                var selectedSegment =
                    _cutPlanViewModel.SelectedRemoveSegment
                    ?? throw new InvalidOperationException(
                        "Es ist kein Schnittbereich ausgewählt.");

                if (!_cutPlanViewModel.Remove(selectedSegment))
                {
                    throw new InvalidOperationException(
                        "Der ausgewählte Schnittbereich konnte nicht gelöscht werden.");
                }

                _logger.Information(
                    $"Schnittbereich wurde gelöscht: " +
                    $"{selectedSegment.Start:c} - {selectedSegment.End:c}");
            });
    }

    private void ExecuteCutPlanAction(
        Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            _logger.Warning(
                $"Schnittmarke konnte nicht übernommen werden: " +
                $"{exception.Message}");

            if (!_isClosed)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    "Schnittmarke ungültig",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }

    private async Task ExecutePlaybackActionAsync(
        Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            if (!_isClosed)
            {
                MessageBox.Show(
                    this,
                    $"Die Wiedergabeaktion ist fehlgeschlagen:" +
                    $"{Environment.NewLine}{exception.Message}",
                    "Wiedergabefehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private async void SelectMediaFileButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "MP4-Datei auswählen",
            Filter = "OTR-Videodateien (*.mp4;*.avi)|*.mp4;*.avi|Alle Dateien (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        _logger.Information(
            $"Mediendatei wurde ausgewählt: {dialog.FileName}");

        _cutPlanViewModel.Reset();

        await _viewModel.AnalyzeAsync(dialog.FileName);

        if (_viewModel.MediaDuration.HasValue &&
            _viewModel.MediaDuration.Value > TimeSpan.Zero)
        {
            _cutPlanViewModel.Initialize(
                _viewModel.MediaDuration.Value);
        }

        try
        {
            await EnsureMediaPlayerInitializedAsync();

            await _mediaPlayerService.LoadAsync(
                dialog.FileName);

        }
        catch (Exception exception)
        {
            if (!_isClosed)
            {
                MessageBox.Show(
                    this,
                    $"Die Mediendatei konnte nicht wiedergegeben werden:" +
                    $"{Environment.NewLine}{exception.Message}",
                    "Wiedergabe fehlgeschlagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
