using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CutAssistantNext.App.Controls;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Naming;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Services.Analysis;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Cutlists.Compatibility;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;
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
    private readonly KeyframeNavigationViewModel _keyframeNavigation;
    private readonly PlayerFrameSearchViewModel _playerFrameSearch;
    private readonly CutPlanViewModel _cutPlanViewModel;
    private readonly CutReviewController _cutReviewController;
    private CutNamingState? _cutNamingState;
    private string? _loadedCutlistAuthor;
    private string? _lastSavedCutlistFilePath;
    private bool _cutlistUploadInProgress;
    private bool _cutlistSearchInProgress;
    private bool _mediaLoadInProgress;
    private bool _frameLoupeOpening;
    private bool _otrCanSettingsOpening;
    private bool _cutOutputOpening;
    private bool _timelineSliderDragging;

    private Task? _mediaPlayerInitializationTask;
    private bool _isClosed;
    private bool _shutdownStarted;
    private bool _allowClose;

    public MainWindow()
        : this(NullAppLogger.Instance)
    {
    }

    internal MainWindow(IAppLogger logger, string[]? startupArguments = null, string? startupDirectory = null)
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        InitializeComponent();
        _cutReviewController = new CutReviewController(this, _logger);

        _windowSettingsStore =
            new WindowSettingsStore();

        RestoreWindowSettings();

        _mediaPlayerService =
            new MpvMediaPlayerService(_logger);

        _playbackViewModel =
            new PlaybackViewModel(_mediaPlayerService);

        _keyframeNavigation = new KeyframeNavigationViewModel(_playbackViewModel);
        _playerFrameSearch = new PlayerFrameSearchViewModel(_playbackViewModel);

        _cutPlanViewModel =
            new CutPlanViewModel();

        var ffmpegSettingsStore =
            new FfmpegSettingsStore();

        _viewModel = new MainWindowViewModel(
            new ConfiguredFfprobeRunner(
                ffmpegSettingsStore.Load,
                ffprobePath =>
                    new FfprobeRunner(
                        ffprobePath)),
            _logger);

        DataContext = _viewModel;
        PlaybackControls.DataContext = _playbackViewModel;
        PlayerFrameSearchControls.DataContext = _playerFrameSearch;
        KeyframeButtons.DataContext = _keyframeNavigation;
        KeyframeIndicator.DataContext = _keyframeNavigation;
        CutPlanSection.DataContext = _cutPlanViewModel;
        CutTimelineTrack.DataContext = _cutPlanViewModel;

        VideoHost.VideoWindowHandleCreated +=
            VideoHost_VideoWindowHandleCreated;

        Closing += MainWindow_Closing;
        PreviewKeyDown += MainWindow_PreviewKeyDown;
        PreviewKeyUp += MainWindow_PreviewKeyUp;

        InitializeAppearance();
        InitializeMediaInput(startupArguments ?? [], startupDirectory ?? Environment.CurrentDirectory);

        if (VideoHost.IsVideoWindowReady)
        {
            _ = EnsureMediaPlayerInitializedAsync();
        }
    }

    private async void VideoHost_VideoWindowHandleCreated(
        object? sender,
        EventArgs e)
    {
        RegisterVideoDropTarget();
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

            var windowSettings =
                _windowSettingsStore.Load();

            if (windowSettings?.Volume is double savedVolume &&
                double.IsFinite(savedVolume))
            {
                await _playbackViewModel.SetVolumeAsync(
                    savedVolume);
            }
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

        _mediaInputCancellation.Cancel();
        _videoDropTarget?.Dispose();
        SaveWindowSettings();

        VideoHost.VideoWindowHandleCreated -=
            VideoHost_VideoWindowHandleCreated;

        PreviewKeyDown -= MainWindow_PreviewKeyDown;

        PreviewKeyUp -= MainWindow_PreviewKeyUp;

        await _cutReviewController.CloseAsync();
        _playerFrameSearch.Dispose();
        _keyframeNavigation.Dispose();
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
            _mediaInputCancellation.Dispose();
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
        var bounds = WindowState == System.Windows.WindowState.Normal
            ? new Rect(0, 0, ActualWidth, ActualHeight)
            : RestoreBounds;

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

        var saved = _windowSettingsStore.Save(
            new WindowSettings
            {
                Width = width,
                Height = height,
                IsMaximized =
                    WindowState ==
                    System.Windows.WindowState.Maximized,
                Volume =
                    _playbackViewModel.Volume
            });

        if (saved)
        {
            _logger.Information(
                $"Fenstergröße gespeichert: {width:0.##} × {height:0.##} | " +
                $"Fensterzustand: {WindowState}.");
        }
        else
        {
            _logger.Error("Die Fenstereinstellungen konnten nicht gespeichert werden.");
        }
    }

    private async void TimelineSlider_PreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (!IsInitialized || _isClosed || _mediaLoadInProgress || !_playbackViewModel.CanSeek)
        {
            return;
        }

        if (_playbackViewModel.IsSeeking)
        {
            e.Handled = true;
            return;
        }

        if (TimelineSlider.Template?.FindName("PART_Track", TimelineSlider) is not Track track ||
            track.Thumb is null ||
            e.OriginalSource is DependencyObject source &&
            (ReferenceEquals(source, track.Thumb) || track.Thumb.IsAncestorOf(source)))
        {
            _timelineSliderDragging = true;
            _playbackViewModel.BeginSeek();
            return;
        }

        var positionSeconds = track.ValueFromPoint(e.GetPosition(track));
        if (!double.IsFinite(positionSeconds))
        {
            return;
        }

        _timelineSliderDragging = false;
        e.Handled = true;
        await ExecutePlaybackActionAsync(() =>
            _playbackViewModel.SeekToAsync(TimeSpan.FromSeconds(positionSeconds)));
    }

    private async void CutTimelineTrack_SeekRequested(
        object? sender,
        TimelineSeekRequestedEventArgs e)
    {
        if (!IsInitialized || _isClosed || _mediaLoadInProgress)
        {
            return;
        }

        await ExecutePlaybackActionAsync(() =>
            _playbackViewModel.SeekToAsync(e.Position));
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
        if (!IsInitialized || !_timelineSliderDragging)
        {
            return;
        }

        _timelineSliderDragging = false;
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
        if (_playerFrameSearch.IsNavigating) return;

        var modifiers = Keyboard.Modifiers;

        UpdateFrameStepButtonLabels(
            modifiers);

        if (e.Key == Key.F && modifiers == ModifierKeys.Shift &&
            e.OriginalSource is not System.Windows.Controls.Primitives.TextBoxBase)
        {
            e.Handled = true;
            await OpenFrameLoupeAsync();
            return;
        }

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
                () => _playbackViewModel.StepBackwardTwentyFramesAsync());

            return;
        }

        if (e.Key == Key.Right &&
            modifiers == ModifierKeys.Control)
        {
            e.Handled = true;

            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepForwardTwentyFramesAsync());
        }
    }

    private void MainWindow_PreviewKeyUp(
        object sender,
        KeyEventArgs e)
    {
        UpdateFrameStepButtonLabels(
            Keyboard.Modifiers);
    }

    private void UpdateFrameStepButtonLabels(
        ModifierKeys modifiers)
    {
        var useTwentyFrames =
            (modifiers & ModifierKeys.Control) != 0;

        StepBackwardFramesButton.Content =
            useTwentyFrames
                ? "−20 Bilder"
                : "−10 Bilder";

        StepForwardFramesButton.Content =
            useTwentyFrames
                ? "+20 Bilder"
                : "+10 Bilder";
    }

    private async void PlayButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.TogglePlaybackAsync());
    }

    private async void StepBackwardFramesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepBackwardTwentyFramesAsync());

            return;
        }

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

    private async void StepForwardFramesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            await ExecutePlaybackActionAsync(
                () => _playbackViewModel.StepForwardTwentyFramesAsync());

            return;
        }

        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.StepForwardTenFramesAsync());
    }

    private async void RemoveSegmentsDataGrid_PreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is not DataGrid dataGrid ||
            e.OriginalSource is not DependencyObject source)
        {
            return;
        }

        var clickedRow =
            ItemsControl.ContainerFromElement(
                dataGrid,
                source) as DataGridRow;

        if (clickedRow is null)
        {
            return;
        }

        var clickedCell =
            FindVisualParent<DataGridCell>(
                source);

        // Do not intercept text focus or editing if a cell contains an editor.
        if (clickedCell?.IsEditing == true || FindVisualParent<TextBoxBase>(source) is not null)
        {
            return;
        }

        if (clickedCell is not null &&
            clickedRow.Item is RemoveSegment segment)
        {
            TimeSpan? seekPosition =
                null;

            if (ReferenceEquals(
                    clickedCell.Column,
                    RemoveSegmentStartColumn))
            {
                seekPosition =
                    segment.Start;
            }
            else if (ReferenceEquals(
                         clickedCell.Column,
                         RemoveSegmentEndColumn))
            {
                seekPosition =
                    segment.End;
            }

            if (seekPosition.HasValue)
            {
                e.Handled =
                    true;

                var selectionApplied = false;
                ExecuteCutPlanAction(() =>
                {
                    _cutPlanViewModel.ToggleSegmentSelection(
                        segment,
                        ReferenceEquals(clickedCell.Column, RemoveSegmentStartColumn)
                            ? CutEdgeSide.Start
                            : CutEdgeSide.End);
                    selectionApplied = true;
                });
                if (!selectionApplied)
                {
                    return;
                }

                // Navigation is independent of whether this click selected or deselected the row.
                await ExecutePlaybackActionAsync(
                    () => _playbackViewModel.SeekToAsync(seekPosition.Value));

                return;
            }
        }

        if (!ReferenceEquals(
                dataGrid.SelectedItem,
                clickedRow.Item))
        {
            return;
        }

        e.Handled =
            true;

        dataGrid.SelectedItem =
            null;

        _cutPlanViewModel.SelectedRemoveSegment =
            null;
    }

    private async void RemoveSegmentsDataGrid_PreviewMouseRightButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is not DataGrid dataGrid ||
            e.OriginalSource is not DependencyObject source ||
            _isClosed || _mediaLoadInProgress)
        {
            return;
        }

        var cell = FindVisualParent<DataGridCell>(source);
        if (cell is null || cell.IsEditing || FindVisualParent<TextBoxBase>(source) is not null)
        {
            return;
        }

        var row = ItemsControl.ContainerFromElement(dataGrid, source) as DataGridRow;
        if (row?.Item is not RemoveSegment segment)
        {
            return;
        }

        CutEdgeSide? side = ReferenceEquals(cell.Column, RemoveSegmentStartColumn)
            ? CutEdgeSide.Start
            : ReferenceEquals(cell.Column, RemoveSegmentEndColumn) ? CutEdgeSide.End : null;
        if (side is null)
        {
            return;
        }

        // Right-click navigation must not select another row or change the current loupe target.
        e.Handled = true;
        if (!_playbackViewModel.CanSeek)
        {
            return;
        }

        TimeSpan? position = null;
        var calculated = false;
        ExecuteCutPlanAction(() =>
        {
            position = _cutPlanViewModel.GetEdgeReviewPosition(segment, side.Value);
            calculated = true;
        });
        if (!calculated)
        {
            return;
        }

        if (position is null)
        {
            MessageBox.Show(
                this,
                "An dieser Schnittkante gibt es keinen angrenzenden Filmabschnitt zum Prüfen.",
                "Schnittkante prüfen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        await ExecutePlaybackActionAsync(() => _playbackViewModel.SeekToAsync(position.Value));
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
    private void SetCutStartAtBeginningButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                _cutPlanViewModel.SetStart(
                    TimeSpan.Zero);

                _logger.Information(
                    "Schnittanfang wurde auf den Dateianfang gesetzt.");
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
    private void SetCutEndAtMediaEndButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ExecuteCutPlanAction(
            () =>
            {
                var mediaDuration =
                    _cutPlanViewModel.MediaDuration
                    ?? throw new InvalidOperationException(
                        "Es wurde noch keine Mediendatei für den Schnittplan initialisiert.");

                _cutPlanViewModel.SetEnd(
                    mediaDuration);

                _logger.Information(
                    $"Schnittende wurde auf das Dateiende gesetzt: " +
                    $"{mediaDuration:c}");
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

    private static T? FindVisualParent<T>(
        DependencyObject source)
        where T : DependencyObject
    {
        var current =
            source;

        while (current is not null)
        {
            if (current is T matchingParent)
            {
                return matchingParent;
            }

            current =
                System.Windows.Media.VisualTreeHelper.GetParent(
                    current);
        }

        return null;
    }

    private async Task ExecutePlaybackActionAsync(
        Func<Task> action)
    {
        if (_playerFrameSearch.IsNavigating) return;
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

    private async void SearchLeft_Click(object sender, RoutedEventArgs e) => await SearchPlayerFramesAsync(-1);

    private async void SearchRight_Click(object sender, RoutedEventArgs e) => await SearchPlayerFramesAsync(1);

    private void ResetPlayerSearch_Click(object sender, RoutedEventArgs e) => _playerFrameSearch.ResetSearch();

    private async Task SearchPlayerFramesAsync(int direction)
    {
        if (_isClosed || _mediaLoadInProgress || _keyframeNavigation.IsNavigating || !_viewModel.IsPlayerFrameSearchVisible) return;
        await _playerFrameSearch.SearchAsync(direction);
        if (!_isClosed && _playerFrameSearch.IsSearchError)
            MessageBox.Show(this, _playerFrameSearch.Status, "Halbierungssuche", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void ConfigurePlayerFrameSearch(string path)
    {
        var settings = new FrameLoupeSettingsStore().Load();
        if (_viewModel.AnalysisResult is not { VideoStreams.Count: 1, Duration: TimeSpan duration } media ||
            duration <= TimeSpan.Zero || !_viewModel.IsPlayerFrameSearchVisible)
        {
            _playerFrameSearch.SetMedia(null, settings.InitialSearchFrames);
            return;
        }
        _playerFrameSearch.SetMedia(position =>
        {
            var tools = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var resolver = new ToolPathResolver();
            return new FrameLoupeViewModel(path, media.VideoStreams[0].Index, duration, position, null,
                (decimal)(media.StartTimeSeconds ?? 0), settings.InitialSearchFrames,
                new FfprobeFrameRunner(resolver.Resolve(BundledToolKind.Ffprobe, tools.FfprobeExecutablePath)),
                new FfmpegFramePreviewRunner(resolver.Resolve(BundledToolKind.Ffmpeg, tools.FfmpegExecutablePath)), media.FormatName);
        }, settings.InitialSearchFrames);
    }

    private async void PreviousKeyframe_Click(object sender, RoutedEventArgs e) =>
        await _keyframeNavigation.JumpAsync(-1);

    private async void NextKeyframe_Click(object sender, RoutedEventArgs e) =>
        await _keyframeNavigation.JumpAsync(1);

    private async Task LoadKeyframesAsync(string path)
    {
        try
        {
            if (_isClosed || _viewModel.AnalysisResult is not { } analysis) return;
            var settings = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var executable = new ToolPathResolver().Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath);
            await _keyframeNavigation.LoadAsync(path, analysis, new FfprobeFrameRunner(executable));
        }
        catch (Exception exception)
        {
            _logger.Warning($"Keyframe-Analyse konnte nicht gestartet werden: {exception.Message}");
        }
    }
    private async void FrameLoupeButton_Click(object sender, RoutedEventArgs e)
    {
        await OpenFrameLoupeAsync();
    }

    private async Task OpenFrameLoupeAsync()
    {
        if (_frameLoupeOpening || _mediaLoadInProgress || _isClosed || _playerFrameSearch.IsNavigating || !_viewModel.CanOpenFrameLoupe)
        {
            return;
        }

        if (!_playbackViewModel.CanSeek ||
            _viewModel.AnalysisResult is not { VideoStreams.Count: > 0 } analysis)
        {
            MessageBox.Show(this, "Bitte zuerst eine analysierte Videodatei im Player laden.",
                "Frame-Lupe", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var segment = _cutPlanViewModel.SelectedRemoveSegment;
        if (analysis.VideoStreams.Count != 1)
        {
            MessageBox.Show(this, "Diese Datei enthält mehrere Videostreams. Die Zuordnung zur aktiven mpv-Videospur ist noch nicht verfügbar.",
                "Frame-Lupe", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var side = _cutPlanViewModel.SelectedEdgeSide;
        var path = _viewModel.FilePath;
        _frameLoupeOpening = true;
        try
        {
            if (_playbackViewModel.CanPause)
            {
                await _playbackViewModel.PauseAsync();
            }

            if (_isClosed || _mediaLoadInProgress || !_playbackViewModel.CanSeek ||
                !ReferenceEquals(analysis, _viewModel.AnalysisResult) ||
                !ReferenceEquals(segment, _cutPlanViewModel.SelectedRemoveSegment) ||
                side != _cutPlanViewModel.SelectedEdgeSide || path != _viewModel.FilePath)
            {
                return;
            }

            var target = FrameLoupeTarget.Capture(_cutPlanViewModel, _playbackViewModel);
            var tools = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var resolver = new ToolPathResolver();
            var frameAnalysis = new FfprobeFrameRunner(resolver.Resolve(BundledToolKind.Ffprobe, tools.FfprobeExecutablePath));
            var preview = new FfmpegFramePreviewRunner(resolver.Resolve(BundledToolKind.Ffmpeg, tools.FfmpegExecutablePath));
            var settings = new FrameLoupeSettingsStore().Load();
            var viewModel = new FrameLoupeViewModel(path, analysis.VideoStreams[0].Index,
                target.MediaDuration, target.Position, target.Side,
                (decimal)(analysis.StartTimeSeconds ?? 0), settings.InitialSearchFrames, frameAnalysis, preview, analysis.FormatName);
            Action<FrameLoupeSelection>? applySelection = null;
            if (target.Side is CutEdgeSide edgeSide && target.Segment is { } edgeSegment)
            {
                applySelection = selection =>
                {
                    if (_isClosed || _mediaLoadInProgress || path != _viewModel.FilePath)
                    {
                        throw new InvalidOperationException("Die Mediendatei hat sich inzwischen geändert. Bitte die Frame-Lupe erneut öffnen.");
                    }

                    _cutPlanViewModel.ApplyFrameEdge(edgeSegment, edgeSide, selection.Position);
                    _logger.Information($"Frame-Lupe: {edgeSide}-Kante von {target.Position:c} auf {selection.Position:c} übernommen; " +
                        $"Stream {selection.StreamIndex}, " +
                        $"{(selection.UsesReconstructedTimestamp ? "rekonstruierter AVI-Zeitstempel" : "Original-PTS")} {selection.Pts}, " +
                        $"Zeitbasis {selection.TimeBase.Numerator}/{selection.TimeBase.Denominator}.");
                };
            }

            var dialog = new FrameLoupeDialog(viewModel, applySelection) { Owner = this };
            dialog.ShowDialog();
        }
        catch (Exception exception)
        {
            _logger.Warning($"Frame-Lupe konnte nicht geöffnet werden: {exception.Message}");
            if (!_isClosed)
            {
                MessageBox.Show(this, exception.Message, "Frame-Lupe",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            _frameLoupeOpening = false;
        }
    }

    private void FrameLoupeSettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        new FrameLoupeSettingsDialog { Owner = this }.ShowDialog();
    }

    private void VideoInformationMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog =
            new VideoInformationDialog(
                _viewModel)
            {
                Owner = this
            };

        dialog.ShowDialog();
    }

    private void CreditsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        new CreditsDialog(OpenHelpLink) { Owner = this }.ShowDialog();
    }

    private void UserGuideMenuItem_Click(object sender, RoutedEventArgs e)
    {
        OpenHelpLink("https://github.com/jmagull/cut-assistant-next/blob/main/docs/NUTZERANLEITUNG.md");
    }

    private void GitHubProjectMenuItem_Click(object sender, RoutedEventArgs e)
    {
        OpenHelpLink("https://github.com/jmagull/cut-assistant-next");
    }

    private void OpenHelpLink(string address)
    {
        try
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(address)
                {
                    UseShellExecute = true
                });
        }
        catch (Exception exception)
        {
            _logger.Error("Der Hilfe-Link konnte nicht geöffnet werden.", exception);
            MessageBox.Show(
                this,
                "Der Browser konnte nicht geöffnet werden. " +
                "Bitte öffne diese Adresse in deinem Browser:" +
                Environment.NewLine + Environment.NewLine + address,
                "Hilfe",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void QuitMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private void CutlistSettingsMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        var cutlistSettingsStore =
            new CutlistSettingsStore();

        var cutlistSettings =
            cutlistSettingsStore.Load()
            ?? CutlistSettings.CreateDefault();

        var serverSettingsStore =
            new CutlistServerSettingsStore();

        var serverSettings =
            serverSettingsStore.Load()
            ?? CutlistServerSettings.CreateDefault();

        var viewModel =
            new CutlistSettingsViewModel(
                cutlistSettings,
                serverSettings);

        var dialog =
            new CutlistSettingsDialog(
                viewModel)
            {
                Owner = this
            };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        cutlistSettingsStore.Save(
            viewModel.CreateSettings());

        serverSettingsStore.Save(
            viewModel.CreateServerSettings());
    }

    private void VideoFolderSettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var store = new VideoFolderSettingsStore();
        new VideoFolderSettingsDialog(new VideoFolderSettingsViewModel(store.Load()), store)
        {
            Owner = this
        }.ShowDialog();
    }

    private void CutApplicationSettingsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var store =
            new CutApplicationSettingsStore();

        var settings =
            store.Load()
            ?? CutApplicationSettings.CreateDefault();

        var viewModel =
            new CutApplicationSettingsViewModel(
                settings);

        var dialog =
            new CutApplicationSettingsDialog(
                viewModel)
            {
                Owner = this
            };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        store.Save(
            viewModel.CreateSettings());
    }

    private void NamingSettingsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var store =
            new NamingSettingsStore();

        var settings =
            new NamingSettingsLoader().Load();

        var nameContext = NamingEditorSample.CreateContext();

        var viewModel =
            new NamingSettingsViewModel(
                settings,
                nameContext);

        var dialog =
            new NamingSettingsDialog(
                viewModel)
            {
                Owner = this
            };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        store.Save(
            viewModel.CreateSettings());
    }

    private void FfmpegSettingsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var store =
            new FfmpegSettingsStore();

        var settings =
            store.Load()
            ?? FfmpegSettings.CreateDefault();

        var viewModel =
            new FfmpegSettingsViewModel(
                settings);

        var dialog =
            new FfmpegSettingsDialog(
                viewModel)
            {
                Owner = this
            };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        store.Save(
            viewModel.CreateSettings());
    }
    private async void OtrCanSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (_otrCanSettingsOpening || _shutdownStarted) return;
        _otrCanSettingsOpening = true;
        try
        {
            var store = new OtrCanSettingsStore();
            var settings = await Task.Run(store.Load);
            if (_isClosed || _shutdownStarted) return;

            var checker = new OtrCanToolChecker();
            using var viewModel = new OtrCanSettingsViewModel(settings,
                (candidate, token) => checker.CheckAsync(candidate,
                    new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault(), token),
                candidate => { store.Save(candidate); return Task.CompletedTask; });
            new OtrCanSettingsDialog(viewModel) { Owner = this }.ShowDialog();
        }
        catch (Exception error)
        {
            if (!_isClosed)
            {
                MessageBox.Show(this, $"Die OTR-CAN-Einstellungen konnten nicht geöffnet werden: {error.Message}",
                    "OTR-CAN-Werkzeuge", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            _otrCanSettingsOpening = false;
        }
    }

    private async void CutMp4BoxButton_Click(object sender, RoutedEventArgs e) =>
        await ShowCutOutputAsync(CutEngineKind.Mp4Box);

    private async void CutOtrCanButton_Click(object sender, RoutedEventArgs e) =>
        await ShowCutOutputAsync(CutEngineKind.OtrCan);

    private async Task ShowCutOutputAsync(CutEngineKind engineKind)
    {
        if (_cutOutputOpening || _shutdownStarted) return;
        var analysis =
            _viewModel.AnalysisResult;

        if (analysis is null)
        {
            MessageBox.Show(
                this,
                "Bitte zuerst eine Mediendatei auswählen und erfolgreich analysieren.",
                "Schneiden",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        OtrCanSettings selection;
        _cutOutputOpening = true;
        try
        {
            selection = engineKind == CutEngineKind.OtrCan
                ? await Task.Run(() => new OtrCanSettingsStore().Load())
                : new OtrCanSettings();
        }
        catch (Exception error)
        {
            MessageBox.Show(this, error.Message, "Schnittmotor", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        finally { _cutOutputOpening = false; }
        if (_shutdownStarted || !ReferenceEquals(analysis, _viewModel.AnalysisResult)) return;
        var requiresPreparation = ConfiguredCutEngineFactory.RequiresPreparation(engineKind, analysis);
        if ((requiresPreparation || VideoPreparation.ConvertsMp3(engineKind, analysis)) && MessageBox.Show(
                this,
                VideoPreparation.ConfirmationText(engineKind, analysis),
                engineKind == CutEngineKind.OtrCan ? "Experimentelle otr-can-Verarbeitung" : "Experimentelle Video-Vorbereitung", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
        {
            return;
        }
        var namingSettings =
            new NamingSettingsLoader().Load();

        _cutNamingState ??=
            new CutNamingState(
                namingSettings.DefaultNameTemplate,
                NameTemplateContextFactory.Create(
                    _viewModel.FileName));

        var viewModel =
            new CutOutputViewModel(
                _cutNamingState,
                namingSettings.DefaultNameTemplate);

        var dialog =
            new CutOutputDialog(
                viewModel)
            {
                Owner = this,
                Title = engineKind == CutEngineKind.Mp4Box ? "Schneiden – MP4Box" : "Schneiden – otr-can"
            };

        dialog.CutRequested +=
            async (_, _) =>
            {

                if (!dialog.IsEnabled)
                {
                    return;
                }

                dialog.IsEnabled = false;
                Mp4BoxProgressDialog? progressDialog = null;

                try
                {
                    var movieName = viewModel.OutputMovieName;
                    var originalFilePath = _viewModel.FilePath;
                    var outputTarget = await Task.Run(() => MovieOutputTargetResolver.Resolve(
                        new VideoFolderSettingsStore().Load().CutVideosDirectory,
                        movieName,
                        originalFilePath));

                    if (!dialog.IsVisible)
                    {
                        return;
                    }

                    var overwriteConfirmed = false;
                    if (outputTarget.Exists)
                    {
                        overwriteConfirmed = MessageBox.Show(
                            dialog,
                            $"Die Ausgabedatei existiert bereits:{Environment.NewLine}{outputTarget.FilePath}" +
                            $"{Environment.NewLine}{Environment.NewLine}Möchtest Du diese Datei überschreiben?",
                            "Ausgabedatei überschreiben",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning,
                            MessageBoxResult.No) == MessageBoxResult.Yes;
                        if (!overwriteConfirmed)
                        {
                            return;
                        }
                    }

                    var framesPerSecond =
                        CutMediaAnalysisValidator.GetFramesPerSecond(
                            analysis);

                    var cutPlan =
                        _cutPlanViewModel.CreateCutPlanSnapshot();

                    var cutRequest = CutRequestFactory.Create(
                        originalFilePath,
                        outputTarget.FilePath,
                        cutPlan,
                        framesPerSecond,
                        overwriteExistingOutput: overwriteConfirmed);

                    _cutNamingState = viewModel.CreateNamingState();

                    CutCompletionViewModel? completionViewModel = null;

                    var cutApplicationSettingsStore =
                        new CutApplicationSettingsStore();

                    var progressViewModel =
                        new Mp4BoxProgressViewModel();

                    using var cancellationTokenSource =
                        new CancellationTokenSource();

                    var progress =
                        new BufferedCutProgressReporter(progressViewModel.ApplyProgress);

                    progressDialog =
                        new Mp4BoxProgressDialog(
                            progressViewModel, progress)
                        {
                            Owner = dialog
                        };

                    var cutSucceeded =
                        false;

                    progressDialog.CancelRequested +=
                        (_, _) =>
                        {
                            cancellationTokenSource.Cancel();
                        };

                    progressDialog.Closed +=
                        (_, _) =>
                        {
                            dialog.IsEnabled = true;

                            if (cutSucceeded)
                            {
                                dialog.Close();
                            }
                        };

                    var cutEngineFactory =
                        new ConfiguredCutEngineFactory(new ConfiguredMp4BoxCutServiceFactory(
                            cutApplicationSettingsStore.Load,
                            (executablePath, runnerProgress) =>
                                new Mp4BoxRunner(
                                    executablePath,
                                    _logger,
                                    runnerProgress)));

                    dialog.IsEnabled = false;
                    var cutEngine = await Task.Run(() => cutEngineFactory.Create(engineKind, selection, progress));
                    if (!dialog.IsVisible) return;

                    progressViewModel.MarkRunning();

                    progressDialog.Show();

                    string? temporaryVideo = null;
                    try
                    {
                        var sourceForCut = cutRequest.OriginalFilePath;
                        if (requiresPreparation)
                        {
                            temporaryVideo = Path.Combine(Path.GetTempPath(), $"can-{Guid.NewGuid():N}.mp4");
                            var tools =
                                new FfmpegSettingsStore().Load()
                                ?? FfmpegSettings.CreateDefault();

                            var resolver =
                                new ToolPathResolver();

                            var ffmpegPath =
                                resolver.Resolve(
                                    BundledToolKind.Ffmpeg,
                                    tools.FfmpegExecutablePath);

                            var ffprobePath =
                                resolver.Resolve(
                                    BundledToolKind.Ffprobe,
                                    tools.FfprobeExecutablePath);

                            await VideoPreparation.PrepareAsync(
                                sourceForCut,
                                temporaryVideo,
                                analysis,
                                ffmpegPath,
                                ffprobePath,
                                progress,
                                cancellationTokenSource.Token);
                            cutRequest = cutRequest.WithSourceFilePath(temporaryVideo);
                        }

                        var cutResult = await cutEngine.RunWithResultAsync(
                            cutRequest,
                            progress,
                            cancellationTokenSource.Token);
                        completionViewModel = new CutCompletionViewModel(cutResult);

                        cutSucceeded = true;

                        progress.Flush();
                        progressViewModel.MarkSucceeded();

                        progressDialog.MarkOperationCompleted(
                            startAutoClose: false);
                    }
                    catch (OperationCanceledException)
                    {
                        progressViewModel.ApplyProgress(
                            new CutProgressUpdate(
                                CutProgressKind.Status,
                                "Abgebrochen."));

                        progress.Flush();
                        progressViewModel.MarkCancelled();

                        progressDialog.MarkOperationCompleted(
                            startAutoClose: false);
                    }
                    catch (Exception exception)
                    {
                        progress.Flush();
                        progressViewModel.MarkFailed(exception.Message);

                        progressDialog.MarkOperationCompleted(
                            startAutoClose: false);
                    }
                    finally
                    {
                        if (temporaryVideo is not null)
                        {
                            try { File.Delete(temporaryVideo); }
                            catch (Exception cleanupError)
                            {
                                progressViewModel.ApplyProgress(new CutProgressUpdate(
                                    CutProgressKind.Output,
                                    $"Temporäre Datei konnte nicht gelöscht werden: {temporaryVideo} ({cleanupError.Message})"));
                                _logger.Error($"Temporäre MP4-Datei konnte nicht gelöscht werden: {cleanupError.Message}");
                            }
                        }
                    }

                    if (cutSucceeded && completionViewModel is not null)
                    {
                        progressDialog.Close();
                        new CutCompletionDialog(completionViewModel, progressViewModel.ProtocolText,
                            reviewAsync: _cutReviewController.ReviewAsync)
                        {
                            Owner = this
                        }.Show();
                    }
                }
                catch (Exception exception)
                {
                    if (dialog.IsVisible)
                    {
                        MessageBox.Show(
                            dialog,
                            $"Schneiden ist nicht möglich:{Environment.NewLine}{exception.Message}",
                            "Schneiden",
                            MessageBoxButton.OK,
                            exception is ArgumentException ? MessageBoxImage.Warning : MessageBoxImage.Error);
                    }
                }
                finally
                {
                    if (dialog.IsVisible && (progressDialog is null || !progressDialog.IsVisible))
                    {
                        dialog.IsEnabled = true;
                    }
                }
            };

        dialog.ShowDialog();
    }

    private async void LoadServerCutlistButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_mediaLoadInProgress)
        {
            return;
        }

        await SearchCutlistsForCurrentMediaAsync();
    }

    private async Task SearchCutlistsForCurrentMediaAsync(
        bool skipIfNotConfigured = false)
    {
        if (_cutlistSearchInProgress)
        {
            return;
        }

        _cutlistSearchInProgress = true;
        LoadServerCutlistButton.IsEnabled = false;

        try
        {
            await SearchCutlistsForCurrentMediaCoreAsync(skipIfNotConfigured);
        }
        finally
        {
            _cutlistSearchInProgress = false;
            LoadServerCutlistButton.IsEnabled = true;
        }
    }

    private async Task SearchCutlistsForCurrentMediaCoreAsync(
        bool skipIfNotConfigured = false)
    {
        if (_viewModel.AnalysisResult is null)
        {
            MessageBox.Show(
                this,
                "Bitte zuerst eine Mediendatei auswählen und erfolgreich analysieren.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var mediaDuration =
            _viewModel.MediaDuration;

        if (!mediaDuration.HasValue ||
            mediaDuration.Value <= TimeSpan.Zero)
        {
            MessageBox.Show(
                this,
                "Die Laufzeit der geladenen Mediendatei konnte nicht ermittelt werden.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var serverSettings =
            new CutlistServerSettingsStore().Load();

        if (serverSettings is null ||
            string.IsNullOrWhiteSpace(
                serverSettings.PersonalServerUrl))
        {
            if (skipIfNotConfigured)
            {
                return;
            }

            MessageBox.Show(
                this,
                "Bitte zuerst unter Cutlist-Einstellungen die persönliche Server-URL eintragen.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        if (!CutlistServerSettingsValidator.TryValidatePersonalServerUrl(
                serverSettings.PersonalServerUrl,
                out var errorMessage))
        {
            MessageBox.Show(
                this,
                errorMessage,
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            using var httpClient =
                new HttpClient
                {
                    Timeout =
                        TimeSpan.FromSeconds(15)
                };

            var client =
                new CutlistServerClient(
                    httpClient);

            var results =
                await client.SearchAsync(
                    serverSettings.PersonalServerUrl,
                    _viewModel.FileName);

            _logger.Information(
                $"Cutlist-Serversuche abgeschlossen: {_viewModel.FileName} | " +
                $"Treffer: {results.Count}");

            if (results.Count == 0)
            {
                MessageBox.Show(
                    this,
                    $"Für {_viewModel.FileName} wurde keine Cutlist gefunden.",
                    "Cutlist-Server",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var dialog =
                new CutlistSearchResultsDialog(
                    _viewModel.FileName,
                    results)
                {
                    Owner =
                        this
                };

            var dialogResult =
                dialog.ShowDialog();

            if (dialogResult == true &&
                dialog.SelectedResult is not null)
            {
                var selectedResult =
                    dialog.SelectedResult;

                _logger.Information(
                    $"Cutlist auf Server ausgewählt: " +
                    $"{selectedResult.CutlistFileName} | " +
                    $"ID: {selectedResult.Id}");

                var cutlistBytes =
                    await client.DownloadBytesAsync(
                        serverSettings.PersonalServerUrl,
                        selectedResult.Id);

                var temporaryCutlistFileName =
                    Path.Combine(
                        Path.GetTempPath(),
                        $"{Guid.NewGuid():N}.cutlist");

                try
                {
                    await File.WriteAllBytesAsync(
                        temporaryCutlistFileName,
                        cutlistBytes);

                    LoadCutlistFromFile(
                        temporaryCutlistFileName,
                        mediaDuration.Value,
                        rememberAsUploadCandidate: false);
                }
                finally
                {
                    if (File.Exists(
                            temporaryCutlistFileName))
                    {
                        File.Delete(
                            temporaryCutlistFileName);
                    }
                }
            }
        }
        catch (HttpRequestException)
        {
            MessageBox.Show(
                this,
                "Die Anfrage an den Cutlist-Server ist fehlgeschlagen.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch (TaskCanceledException)
        {
            MessageBox.Show(
                this,
                "Der Cutlist-Server hat nicht rechtzeitig geantwortet.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch (Exception exception)
        {
            _logger.Error(
                $"Cutlist-Serverfehler: {exception}");

            MessageBox.Show(
                this,
                $"Die Cutlist-Serversuche ist fehlgeschlagen:" +
                $"{Environment.NewLine}{Environment.NewLine}" +
                exception.Message,
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private async void LoadCutlistButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_mediaLoadInProgress || _cutlistSearchInProgress || !IsEnabled) return;

        var mediaDuration =
            _viewModel.MediaDuration;

        if (!mediaDuration.HasValue ||
            mediaDuration.Value <= TimeSpan.Zero)
        {
            MessageBox.Show(
                this,
                "Bitte zuerst eine Mediendatei auswählen und erfolgreich analysieren.",
                "Cutlist laden",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        string cutlistDirectory;
        IsEnabled = false;
        try
        {
            cutlistDirectory = await Task.Run(() => VideoFolderPathResolver.ResolveCutlists(
                new VideoFolderSettingsStore().Load()));
        }
        finally
        {
            if (!_isClosed) IsEnabled = true;
        }

        if (_isClosed || _shutdownStarted) return;

        var dialog =
            new OpenFileDialog
            {
                Title = "Cutlist laden",
                InitialDirectory = cutlistDirectory,
                Filter =
                    "Cutlist-Dateien (*.cutlist)|*.cutlist|" +
                    "Alle Dateien (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        LoadCutlistFromFile(
            dialog.FileName,
            mediaDuration.Value,
            rememberAsUploadCandidate: true);
    }
    private void LoadCutlistFromFile(
        string fileName,
        TimeSpan mediaDuration,
        bool rememberAsUploadCandidate)
    {
        try
        {
            var document =
                CutlistFileReader.Read(
                    fileName);

            var fileSizeMismatch =
                CutlistFileSizeCompatibilityDetector.FindMismatch(
                    document.General.OriginalFileSizeBytes,
                    _viewModel.AnalysisResult?.FileSizeBytes);

            if (fileSizeMismatch is not null)
            {
                var continueResult =
                    MessageBox.Show(
                        this,
                        "Die Dateigröße der geladenen Mediendatei weicht deutlich " +
                        "von der in der Cutlist gespeicherten Größe ab." +
                        $"{Environment.NewLine}{Environment.NewLine}" +
                        $"Größe laut Cutlist: {fileSizeMismatch.ExpectedFileSizeBytes:N0} Bytes" +
                        $"{Environment.NewLine}" +
                        $"Geladene Datei: {fileSizeMismatch.ActualFileSizeBytes:N0} Bytes" +
                        $"{Environment.NewLine}" +
                        $"Abweichung: {fileSizeMismatch.DifferenceRatio:P1}" +
                        $"{Environment.NewLine}{Environment.NewLine}" +
                        "Die Cutlist könnte für eine andere oder anders aufgezeichnete " +
                        "Mediendatei erstellt worden sein." +
                        $"{Environment.NewLine}{Environment.NewLine}" +
                        "Trotzdem laden?",
                        "Cutlist laden",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                if (continueResult != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            var endFragment =
                CutlistEndFragmentDetector.Find(
                    document,
                    mediaDuration);

            CutPlan cutPlan;

            if (endFragment is not null)
            {
                var framesPerSecond =
                    document.General.FramesPerSecond.GetValueOrDefault();

                var approximateFrames =
                    (int)Math.Round(
                        endFragment.Duration.TotalSeconds *
                        framesPerSecond);

                var correctionResult =
                    MessageBox.Show(
                        this,
                        "Die Cutlist enthält am Videoende einen ungewöhnlich kurzen " +
                        $"Behaltebereich von {endFragment.Duration.TotalSeconds:0.###} Sekunden " +
                        $"(ca. {approximateFrames} Frames)." +
                        $"{Environment.NewLine}{Environment.NewLine}" +
                        "Dieser Bereich kann beim Schneiden mit MP4Box zu Problemen führen." +
                        $"{Environment.NewLine}{Environment.NewLine}" +
                        "Neues Ende bis Videoende setzen?",
                        "Cutlist laden",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                if (correctionResult != MessageBoxResult.Yes)
                {
                    return;
                }

                cutPlan =
                    CutlistEndFragmentCorrector.BuildCutPlan(
                        document,
                        mediaDuration,
                        endFragment);
            }
            else
            {
                cutPlan =
                    CutlistCutPlanBuilder.Build(
                        document,
                        mediaDuration);
            }

            _cutPlanViewModel.LoadCutPlan(
                cutPlan);

            var suggestedMovieName =
                document.Info.SuggestedMovieName;

            if (!string.IsNullOrWhiteSpace(
                    suggestedMovieName))
            {
                var namingSettings =
                    new NamingSettingsLoader().Load();

                _cutNamingState ??=
                    new CutNamingState(
                        namingSettings.DefaultNameTemplate,
                        NameTemplateContextFactory.Create(
                            _viewModel.FileName));

                _cutNamingState =
                    _cutNamingState.UseSuggestedMovieName(
                        suggestedMovieName,
                        preserveSuggestedMovieName: true);
            }

            if (rememberAsUploadCandidate)
            {
                _lastSavedCutlistFilePath =
                    Path.GetFullPath(
                        fileName);
            }

            _loadedCutlistAuthor =
                document.Info.Author;

            _logger.Information(
                $"Cutlist wurde geladen: {fileName}");

            MessageBox.Show(
                this,
                "Die Cutlist wurde erfolgreich geladen.",
                "Cutlist laden",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"Die Cutlist konnte nicht geladen werden:" +
                $"{Environment.NewLine}{exception.Message}",
                "Cutlist laden",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void GenerateCutlistButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var analysis =
            _viewModel.AnalysisResult;

        if (analysis is null)
        {
            MessageBox.Show(
                this,
                "Bitte zuerst eine Mediendatei auswählen und erfolgreich analysieren.",
                "Cutlist erzeugen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var fileName =
            _viewModel.FileName;

        var settings =
            new CutlistSettingsStore().Load()
            ?? CutlistSettings.CreateDefault();

        var namingSettings =
            new NamingSettingsLoader().Load();

        var cutApplicationSettings =
            new CutApplicationSettingsStore().Load()
            ?? CutApplicationSettings.CreateDefault();

        var intendedCutApplication =
            CutApplicationSettingsMapper.ToCutApplicationInfo(
                cutApplicationSettings);

        var cutlistViewModel =
            _cutNamingState is null
                ? CutlistGenerationViewModelFactory.Create(
                    settings,
                    namingSettings,
                    fileName,
                    analysis)
                : CutlistGenerationViewModelFactory.Create(
                    settings,
                    fileName,
                    analysis,
                    _cutNamingState,
                    namingSettings);

        cutlistViewModel.ApplyTemplateAuthor(
            _loadedCutlistAuthor);

        var cutPlan =
            _cutPlanViewModel.CreateCutPlanSnapshot();

        var dialog =
            new CutlistGenerationDialog(
                cutlistViewModel)
            {
                Owner = this
            };

        var saveInProgress = false;
        dialog.Closing += (_, e) =>
        {
            // Keep the original and upload association stable until this save has finished.
            if (saveInProgress)
            {
                e.Cancel = true;
            }
        };

        dialog.SaveRequested +=
            async (_, _) =>
            {
                if (saveInProgress || !dialog.IsEnabled)
                {
                    return;
                }

                saveInProgress = true;
                dialog.IsEnabled = false;
                try
                {
                    var originalFileName = Path.GetFileName(fileName);
                    var applicationVersion =
                        typeof(MainWindow)
                            .Assembly
                            .GetName()
                            .Version?
                            .ToString(3)
                        ?? throw new InvalidOperationException(
                            "Die Anwendungsversion konnte nicht ermittelt werden.");

                    var document =
                        cutlistViewModel.CreateDocument(
                            cutPlan,
                            originalFileName,
                            applicationVersion,
                            analysis,
                            intendedCutApplication);
                    var namingState = cutlistViewModel.CreateNamingState();

                    var target = await Task.Run(() => CutlistOutputTargetResolver.Resolve(
                        new VideoFolderSettingsStore().Load().OwnCutlistsDirectory,
                        originalFileName));

                    var overwriteConfirmed = false;
                    if (target.Exists)
                    {
                        overwriteConfirmed = MessageBox.Show(
                            dialog,
                            $"Die Cutlist existiert bereits:{Environment.NewLine}{target.FilePath}" +
                            $"{Environment.NewLine}{Environment.NewLine}Möchtest Du diese Cutlist überschreiben?",
                            "Cutlist überschreiben",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning,
                            MessageBoxResult.No) == MessageBoxResult.Yes;
                        if (!overwriteConfirmed)
                        {
                            return;
                        }
                    }

                    await Task.Run(() => CutlistFileWriter.WriteSafely(
                        target.FilePath,
                        document,
                        overwriteExisting: overwriteConfirmed));

                    _lastSavedCutlistFilePath = target.FilePath;
                    _cutNamingState = namingState;

                    MessageBox.Show(
                        dialog,
                        "Die Cutlist wurde erfolgreich gespeichert.",
                        "Cutlist speichern",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    saveInProgress = false;
                    dialog.Close();
                }
                catch (Exception exception)
                {
                    var message = exception.Message;
                    if (exception is IOException or UnauthorizedAccessException &&
                        !message.Contains("Einstellungen", StringComparison.Ordinal))
                    {
                        message += Environment.NewLine +
                            "Bitte überprüfe das Verzeichnis für eigene Cutlists unter Einstellungen.";
                    }

                    MessageBox.Show(
                        dialog,
                        message,
                        "Cutlist konnte nicht gespeichert werden",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                finally
                {
                    saveInProgress = false;
                    if (dialog.IsVisible)
                    {
                        dialog.IsEnabled = true;
                    }
                }
            };

        dialog.ShowDialog();
    }

    private async void UploadCutlistButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_cutlistUploadInProgress)
        {
            return;
        }

        if (_viewModel.AnalysisResult is null)
        {
            MessageBox.Show(
                this,
                "Bitte zuerst eine Mediendatei auswählen und erfolgreich analysieren.",
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        if (string.IsNullOrWhiteSpace(
                _lastSavedCutlistFilePath) ||
            !File.Exists(
                _lastSavedCutlistFilePath))
        {
            MessageBox.Show(
                this,
                "Bitte zuerst über „Cutlist erzeugen …“ eine Cutlist erstellen und speichern.",
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var originalFileName =
            Path.GetFileName(
                _viewModel.FileName);

        CutlistDocument document;

        try
        {
            document =
                CutlistFileReader.Read(
                    _lastSavedCutlistFilePath);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"Die gespeicherte Cutlist konnte nicht gelesen werden:" +
                $"{Environment.NewLine}{exception.Message}",
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        if (!string.Equals(
                document.General.ApplyToFile,
                originalFileName,
                StringComparison.Ordinal))
        {
            MessageBox.Show(
                this,
                "Die zuletzt gespeicherte Cutlist gehört nicht zur aktuell geladenen Mediendatei." +
                $"{Environment.NewLine}{Environment.NewLine}" +
                "Bitte zuerst für diese Mediendatei eine neue Cutlist erzeugen und speichern.",
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var serverSettings =
            new CutlistServerSettingsStore().Load();

        if (serverSettings is null ||
            string.IsNullOrWhiteSpace(
                serverSettings.PersonalServerUrl))
        {
            MessageBox.Show(
                this,
                "Bitte zuerst unter Cutlist-Einstellungen die persönliche Server-URL eintragen.",
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        if (!CutlistServerSettingsValidator.TryValidatePersonalServerUrl(
                serverSettings.PersonalServerUrl,
                out var errorMessage))
        {
            MessageBox.Show(
                this,
                errorMessage,
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var cutlistFileName =
            Path.GetFileName(
                _lastSavedCutlistFilePath);

        var confirmation =
            MessageBox.Show(
                this,
                $"Die gespeicherte Cutlist{Environment.NewLine}" +
                $"{cutlistFileName}{Environment.NewLine}{Environment.NewLine}" +
                $"für{Environment.NewLine}" +
                $"{originalFileName}{Environment.NewLine}{Environment.NewLine}" +
                "wird auf den persönlichen Cutlist-Server hochgeladen." +
                $"{Environment.NewLine}{Environment.NewLine}" +
                "Möchten Sie fortfahren?",
                "Cutlist hochladen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

        if (confirmation !=
            MessageBoxResult.Yes)
        {
            return;
        }

        _cutlistUploadInProgress =
            true;

        if (sender is Button uploadButton)
        {
            uploadButton.IsEnabled =
                false;
        }

        try
        {
            var cutlistBytes =
                CutlistServerUploadPayloadFactory.CreateBytes(
                    document);

            using var httpClient =
                new HttpClient
                {
                    Timeout =
                        TimeSpan.FromSeconds(15)
                };

            var client =
                new CutlistServerClient(
                    httpClient);

            var uploadResult =
                await client.UploadAsync(
                    serverSettings.PersonalServerUrl,
                    cutlistFileName,
                    cutlistBytes,
                    ApplicationVersion.NumericVersion);

            if (string.IsNullOrWhiteSpace(
                    uploadResult.CutlistId))
            {
                throw new InvalidDataException(
                    "Der Cutlist-Server hat keine gültige Cutlist-ID zurückgegeben.");
            }

            var serverMessage =
                string.IsNullOrWhiteSpace(
                    uploadResult.Message)
                    ? "Upload erfolgreich."
                    : uploadResult.Message;

            MessageBox.Show(
                this,
                "Die Cutlist wurde erfolgreich hochgeladen." +
                $"{Environment.NewLine}{Environment.NewLine}" +
                $"Server-ID: {uploadResult.CutlistId}" +
                $"{Environment.NewLine}" +
                serverMessage,
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"Die Cutlist konnte nicht hochgeladen werden:" +
                $"{Environment.NewLine}{exception.Message}",
                "Cutlist hochladen",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _cutlistUploadInProgress =
                false;

            if (sender is Button uploadButtonToEnable)
            {
                uploadButtonToEnable.IsEnabled =
                    true;
            }
        }
    }

    private async void SelectMediaFileButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!CanOpenMedia) return;

        _mediaLoadInProgress = true;

        try
        {
            await SelectMediaFileAsync();
        }
        finally
        {
            _mediaLoadInProgress = false;
        }
    }

    private async Task SelectMediaFileAsync()
    {
        var originalDirectory = await Task.Run(() => VideoFolderPathResolver.Resolve(
            new VideoFolderSettingsStore().Load().OriginalVideosDirectory));

        if (_isClosed || _shutdownStarted) return;

        var dialog = new OpenFileDialog
        {
            Title = "Videodatei laden",
            InitialDirectory = originalDirectory,
            Filter = "Videodateien (*.mp4;*.avi;*.mkv;*.mov;*.ts;*.m2ts;*.mpg;*.mpeg;*.wmv;*.webm)|*.mp4;*.avi;*.mkv;*.mov;*.ts;*.m2ts;*.mpg;*.mpeg;*.wmv;*.webm|Alle Dateien (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        if (!ConfirmMediaReplacement()) return;
        await LoadMediaFileCoreAsync(dialog.FileName);
    }

    private async Task LoadMediaFileCoreAsync(string mediaPath)
    {
        _logger.Information($"Mediendatei wurde ausgewählt: {mediaPath}");

        _playerFrameSearch.SetMedia(null);
        _keyframeNavigation.Reset();
        _cutPlanViewModel.Reset();

        _cutNamingState = null;
        _loadedCutlistAuthor = null;
        _lastSavedCutlistFilePath = null;

        await _viewModel.AnalyzeAsync(mediaPath, _mediaInputCancellation.Token);
        if (_isClosed || _mediaInputCancellation.IsCancellationRequested) return;

        if (_viewModel.MediaDuration.HasValue &&
            _viewModel.MediaDuration.Value > TimeSpan.Zero)
        {
            _cutPlanViewModel.Initialize(
                _viewModel.MediaDuration.Value);
        }

        try
        {
            await EnsureMediaPlayerInitializedAsync();

            if (_isClosed) return;
            await _mediaPlayerService.LoadAsync(
                mediaPath, _mediaInputCancellation.Token);

            ConfigurePlayerFrameSearch(mediaPath);
            _ = LoadKeyframesAsync(mediaPath);

            await SearchCutlistsForCurrentMediaAsync(
                skipIfNotConfigured: true);
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
