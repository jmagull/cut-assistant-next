using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Playback;
using Microsoft.Win32;

namespace CutAssistantNext.App;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IMediaPlayerService _mediaPlayerService;
    private readonly PlaybackViewModel _playbackViewModel;

    private Task? _mediaPlayerInitializationTask;
    private bool _isClosed;
    private bool _shutdownStarted;
    private bool _allowClose;

    public MainWindow()
    {
        InitializeComponent();

        _mediaPlayerService =
            new MpvMediaPlayerService();

        _playbackViewModel =
            new PlaybackViewModel(_mediaPlayerService);

        _viewModel = new MainWindowViewModel(
            new FfprobeRunner());

        DataContext = _viewModel;
        PlaybackControls.DataContext = _playbackViewModel;

        VideoHost.VideoWindowHandleCreated +=
            VideoHost_VideoWindowHandleCreated;

        Closing += MainWindow_Closing;

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

        VideoHost.VideoWindowHandleCreated -=
            VideoHost_VideoWindowHandleCreated;

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

    private async void PlayButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.PlayAsync());
    }

    private async void PauseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await ExecutePlaybackActionAsync(
            () => _playbackViewModel.PauseAsync());
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
            Filter = "MP4-Dateien (*.mp4)|*.mp4|Alle Dateien (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        await _viewModel.AnalyzeAsync(dialog.FileName);

        try
        {
            await EnsureMediaPlayerInitializedAsync();

            await _mediaPlayerService.LoadAsync(
                dialog.FileName);

            await _mediaPlayerService.PlayAsync();
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
