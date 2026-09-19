using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Services.Analysis;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Cutlists.Compatibility;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Core.Naming;
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
    private readonly CutPlanViewModel _cutPlanViewModel;
    private CutNamingState? _cutNamingState;
    private string? _loadedCutlistAuthor;
    private string? _lastSavedCutlistFilePath;
    private bool _cutlistUploadInProgress;
    private bool _cutlistSearchInProgress;
    private bool _mediaLoadInProgress;

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
        CutPlanSection.DataContext = _cutPlanViewModel;
        CutTimelineTrack.DataContext = _cutPlanViewModel;

        VideoHost.VideoWindowHandleCreated +=
            VideoHost_VideoWindowHandleCreated;

        Closing += MainWindow_Closing;
        PreviewKeyDown += MainWindow_PreviewKeyDown;
        PreviewKeyUp += MainWindow_PreviewKeyUp;

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

        SaveWindowSettings();

        VideoHost.VideoWindowHandleCreated -=
            VideoHost_VideoWindowHandleCreated;

        PreviewKeyDown -= MainWindow_PreviewKeyDown;

        PreviewKeyUp -= MainWindow_PreviewKeyUp;

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

        UpdateFrameStepButtonLabels(
            modifiers);

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

                dataGrid.SelectedItem =
                    clickedRow.Item;

                _cutPlanViewModel.SelectedRemoveSegment =
                    segment;

                _playbackViewModel.BeginSeek();

                if (!_playbackViewModel.IsSeeking)
                {
                    return;
                }

                _playbackViewModel.UpdateSeekPosition(
                    seekPosition.Value.TotalSeconds);

                await ExecutePlaybackActionAsync(
                    () => _playbackViewModel.CommitSeekAsync());

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

        var nameContext =
            new NameTemplateContext(
                Name:
                    "Hunting Party - Die Moerderjagd",
                Year: "2026",
                Month: "08",
                Day: "25",
                Season: "02",
                Episode: "13",
                OriginalName:
                    "Hunting_Party_-_Die_Moerderjagd__Xander_Wax_S02E13_26.08.25_22-10_sat1_60_TVOON_DE.HQ.mp4",
                ShortYear: "26",
                Hour: "22",
                Minute: "10",
                Sender: "sat1",
                Series:
                    "Hunting Party - Die Moerderjagd",
                EpisodeTitle:
                    "Xander Wax");

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
    private void CutOutputButton_Click(
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
                "Schneiden",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var requiresPreparation = !VideoPreparation.IsMp4(analysis);
        if (requiresPreparation && MessageBox.Show(
                this,
                "CAN kann derzeit Videos im MP4-Container schneiden. " +
                $"Deine Datei wurde als {VideoPreparation.ContainerName(analysis)} erkannt.\n\n" +
                "Soll CAN versuchen, das Video mit FFmpeg verlustfrei in eine temporäre MP4-Datei " +
                "umzupacken und anschließend zu schneiden?\n\n" +
                "Die Originaldatei bleibt unverändert.\n\n" +
                "Dieses Feature ist experimentell. Es wurde an einigen Beispielvideodateien getestet.",
                "Experimentelle Video-Vorbereitung", MessageBoxButton.YesNo,
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
                Owner = this
            };

        dialog.CutRequested +=
            async (_, _) =>
            {

                string suggestedOutputFileName;

                try
                {
                    suggestedOutputFileName =
                        OutputFileNameBuilder.Build(
                            viewModel.SuggestedMovieName,
                            ".mp4");

                    _cutNamingState =
                        viewModel.CreateNamingState();
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show(
                        dialog,
                        exception.Message,
                        "Schneiden",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var saveDialog =
                    new SaveFileDialog
                    {
                        Title = "Geschnittene Datei speichern",
                        FileName = suggestedOutputFileName,
                        DefaultExt = ".mp4",
                        AddExtension = true,
                        Filter = "MP4-Datei (*.mp4)|*.mp4",
                    };

                if (saveDialog.ShowDialog(
                    dialog) != true)
                {
                    return;
                }

                try
                {
                    var framesPerSecond =
                        CutMediaAnalysisValidator.GetFramesPerSecond(
                            analysis);

                    if (string.Equals(Path.GetFullPath(_viewModel.FilePath),
                            Path.GetFullPath(saveDialog.FileName), StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Die Ausgabedatei darf nicht die Originaldatei überschreiben.");

                    var cutPlan =
                        _cutPlanViewModel.CreateCutPlanSnapshot();

                    var cutApplicationSettingsStore =
                        new CutApplicationSettingsStore();

                    var progressViewModel =
                        new Mp4BoxProgressViewModel();

                    using var cancellationTokenSource =
                        new CancellationTokenSource();

                    var progress =
                        new Progress<Mp4BoxProgressUpdate>(
                            progressViewModel.ApplyProgress);

                    var progressDialog =
                        new Mp4BoxProgressDialog(
                            progressViewModel)
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

                    var cutServiceFactory =
                        new ConfiguredMp4BoxCutServiceFactory(
                            cutApplicationSettingsStore.Load,
                            (executablePath, runnerProgress) =>
                                new Mp4BoxRunner(
                                    executablePath,
                                    _logger,
                                    runnerProgress));

                    var cutService =
                        cutServiceFactory.Create(
                            progress);

                    dialog.IsEnabled = false;

                    progressViewModel.MarkRunning();

                    progressDialog.Show();

                    string? temporaryVideo = null;
                    try
                    {
                        var sourceForCut = _viewModel.FilePath;
                        if (requiresPreparation)
                        {
                            temporaryVideo = Path.Combine(Path.GetTempPath(), $"can-{Guid.NewGuid():N}.mp4");
                            var tools = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
                            await VideoPreparation.PrepareAsync(sourceForCut, temporaryVideo, analysis,
                                tools.FfmpegExecutablePath, tools.FfprobeExecutablePath,
                                progress, cancellationTokenSource.Token);
                            sourceForCut = temporaryVideo;
                        }

                        await cutService.RunAsync(
                            sourceForCut,
                            saveDialog.FileName,
                            cutPlan,
                            framesPerSecond,
                            progress,
                            cancellationTokenSource.Token,
                            overwriteExistingOutput: true);

                        cutSucceeded = true;

                        progressViewModel.MarkSucceeded();

                        progressDialog.MarkOperationCompleted(
                            startAutoClose: true);
                    }
                    catch (OperationCanceledException)
                    {
                        progressViewModel.ApplyProgress(
                            new Mp4BoxProgressUpdate(
                                Mp4BoxProgressKind.Status,
                                "Abgebrochen."));

                        progressViewModel.MarkCancelled();

                        progressDialog.MarkOperationCompleted(
                            startAutoClose: false);
                    }
                    catch (Exception exception)
                    {
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
                                progressViewModel.ApplyProgress(new Mp4BoxProgressUpdate(
                                    Mp4BoxProgressKind.Output,
                                    $"Temporäre Datei konnte nicht gelöscht werden: {temporaryVideo} ({cleanupError.Message})"));
                                _logger.Error($"Temporäre MP4-Datei konnte nicht gelöscht werden: {cleanupError.Message}");
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        dialog,
                        $"Der Schnitt ist fehlgeschlagen:{Environment.NewLine}{exception.Message}",
                        "Schneiden",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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

    private void LoadCutlistButton_Click(
        object sender,
        RoutedEventArgs e)
    {
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

        var dialog =
            new OpenFileDialog
            {
                Title = "Cutlist laden",
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
                    _cutNamingState);

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

        dialog.SaveRequested +=
            (_, _) =>
            {
                var originalFileName =
                    Path.GetFileName(fileName);

                var saveDialog =
                    new SaveFileDialog
                    {
                        Title = "Cutlist speichern",
                        FileName = $"{originalFileName}.cutlist",
                        Filter =
                            "Cutlist-Dateien (*.cutlist)|*.cutlist|" +
                            "Alle Dateien (*.*)|*.*",
                        DefaultExt = ".cutlist",
                        AddExtension = true,
                        InitialDirectory =
                            Path.GetDirectoryName(fileName)
                    };

                if (saveDialog.ShowDialog(dialog) != true)
                {
                    return;
                }

                try
                {
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

                    CutlistFileWriter.Write(
                        saveDialog.FileName,
                        document);

                    _lastSavedCutlistFilePath =
                        Path.GetFullPath(
                            saveDialog.FileName);

                    _cutNamingState =
                        cutlistViewModel.CreateNamingState();

                    MessageBox.Show(
                        dialog,
                        "Die Cutlist wurde erfolgreich gespeichert.",
                        "Cutlist speichern",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    dialog.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        dialog,
                        $"Die Cutlist konnte nicht gespeichert werden:" +
                        $"{Environment.NewLine}{exception.Message}",
                        "Cutlist speichern",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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
                    "0.26.5.6");

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
        if (_cutlistSearchInProgress || _mediaLoadInProgress)
        {
            return;
        }

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
        var dialog = new OpenFileDialog
        {
            Title = "Videodatei laden",
            Filter = "Videodateien (*.mp4;*.avi;*.mkv;*.mov;*.ts;*.m2ts;*.mpg;*.mpeg;*.wmv;*.webm)|*.mp4;*.avi;*.mkv;*.mov;*.ts;*.m2ts;*.mpg;*.mpeg;*.wmv;*.webm|Alle Dateien (*.*)|*.*",
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

        _cutNamingState = null;
        _loadedCutlistAuthor = null;
        _lastSavedCutlistFilePath = null;

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
