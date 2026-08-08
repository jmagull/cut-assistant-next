using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using CutAssistantNext.Core.Logging;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private const string NotAvailable = "Nicht verfügbar";

    private readonly IMediaAnalysisRunner _mediaAnalysisRunner;
    private readonly IAppLogger _logger;

    private bool _isAnalyzing;
    private string _statusMessage = "Bereit.";
    private string _errorMessage = string.Empty;
    private string _fileName = NotAvailable;
    private string _filePath = NotAvailable;
    private string _containerFormat = NotAvailable;
    private string _fileSize = NotAvailable;
    private string _duration = NotAvailable;
    private TimeSpan? _mediaDuration;
    private string _videoCodec = NotAvailable;
    private string _resolution = NotAvailable;
    private string _sampleAspectRatio = NotAvailable;
    private string _displayAspectRatio = NotAvailable;
    private string _frameRate = NotAvailable;
    private string _fieldOrder = NotAvailable;
    private string _audioCodec = NotAvailable;
    private string _sampleRate = NotAvailable;
    private string _channelCount = NotAvailable;
    private string _channelLayout = NotAvailable;

    public MainWindowViewModel(
        IMediaAnalysisRunner mediaAnalysisRunner,
        IAppLogger? logger = null)
    {
        _mediaAnalysisRunner = mediaAnalysisRunner
            ?? throw new ArgumentNullException(nameof(mediaAnalysisRunner));

        _logger = logger ?? NullAppLogger.Instance;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsAnalyzing
    {
        get => _isAnalyzing;
        private set
        {
            if (SetProperty(ref _isAnalyzing, value))
            {
                OnPropertyChanged(nameof(CanAnalyze));
            }
        }
    }

    public bool CanAnalyze => !IsAnalyzing;

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string FileName
    {
        get => _fileName;
        private set => SetProperty(ref _fileName, value);
    }

    public string FilePath
    {
        get => _filePath;
        private set => SetProperty(ref _filePath, value);
    }

    public string ContainerFormat
    {
        get => _containerFormat;
        private set => SetProperty(ref _containerFormat, value);
    }

    public string FileSize
    {
        get => _fileSize;
        private set => SetProperty(ref _fileSize, value);
    }

    public string Duration
    {
        get => _duration;
        private set => SetProperty(ref _duration, value);
    }

    public TimeSpan? MediaDuration
    {
        get => _mediaDuration;
        private set => SetProperty(ref _mediaDuration, value);
    }

    public string VideoCodec
    {
        get => _videoCodec;
        private set => SetProperty(ref _videoCodec, value);
    }

    public string Resolution
    {
        get => _resolution;
        private set => SetProperty(ref _resolution, value);
    }

    public string SampleAspectRatio
    {
        get => _sampleAspectRatio;
        private set => SetProperty(ref _sampleAspectRatio, value);
    }

    public string DisplayAspectRatio
    {
        get => _displayAspectRatio;
        private set => SetProperty(ref _displayAspectRatio, value);
    }

    public string FrameRate
    {
        get => _frameRate;
        private set => SetProperty(ref _frameRate, value);
    }

    public string FieldOrder
    {
        get => _fieldOrder;
        private set => SetProperty(ref _fieldOrder, value);
    }

    public string AudioCodec
    {
        get => _audioCodec;
        private set => SetProperty(ref _audioCodec, value);
    }

    public string SampleRate
    {
        get => _sampleRate;
        private set => SetProperty(ref _sampleRate, value);
    }

    public string ChannelCount
    {
        get => _channelCount;
        private set => SetProperty(ref _channelCount, value);
    }

    public string ChannelLayout
    {
        get => _channelLayout;
        private set => SetProperty(ref _channelLayout, value);
    }

    public async Task AnalyzeAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);

        ResetDisplayValues();

        ErrorMessage = string.Empty;
        StatusMessage = "Datei wird analysiert …";
        IsAnalyzing = true;

        var logMediaFilePath = mediaFilePath;

        try
        {
            var fullMediaFilePath = Path.GetFullPath(mediaFilePath);
            logMediaFilePath = fullMediaFilePath;

            _logger.Information(
                $"Medienanalyse wurde gestartet: {fullMediaFilePath}");

            FileName = Path.GetFileName(fullMediaFilePath);
            FilePath = fullMediaFilePath;

            var result = await _mediaAnalysisRunner.RunAsync(
                fullMediaFilePath,
                cancellationToken);

            ApplyResult(result);

            StatusMessage = "Analyse erfolgreich abgeschlossen.";

            _logger.Information(
                $"Medienanalyse wurde erfolgreich abgeschlossen: " +
                $"{fullMediaFilePath}");
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            StatusMessage = "Analyse wurde abgebrochen.";

            _logger.Warning(
                $"Medienanalyse wurde abgebrochen: " +
                $"{logMediaFilePath}");
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            StatusMessage = "Analyse fehlgeschlagen.";

            _logger.Error(
                $"Medienanalyse ist fehlgeschlagen: " +
                $"{logMediaFilePath}",
                exception);
        }
        finally
        {
            IsAnalyzing = false;
        }
    }

    private void ApplyResult(MediaAnalysisResult result)
    {
        ContainerFormat = FormatDescription(
            result.FormatName,
            result.FormatLongName);

        FileSize = FormatFileSize(result.FileSizeBytes);
        Duration = FormatDuration(result.Duration);
        MediaDuration = result.Duration;

        var videoStream = result.VideoStreams.FirstOrDefault();

        if (videoStream is not null)
        {
            VideoCodec = FormatDescription(
                videoStream.CodecName,
                videoStream.CodecLongName);

            Resolution =
                videoStream.Width.HasValue &&
                videoStream.Height.HasValue
                    ? $"{videoStream.Width.Value} × {videoStream.Height.Value}"
                    : NotAvailable;

            SampleAspectRatio =
                FormatOptionalText(videoStream.SampleAspectRatio);

            DisplayAspectRatio =
                FormatOptionalText(videoStream.DisplayAspectRatio);

            FrameRate = videoStream.FramesPerSecond.HasValue
                ? $"{videoStream.FramesPerSecond.Value.ToString(
                    "0.###",
                    CultureInfo.CurrentCulture)} fps"
                : NotAvailable;

            FieldOrder = FormatOptionalText(videoStream.FieldOrder);
        }

        var audioStream = result.AudioStreams.FirstOrDefault();

        if (audioStream is not null)
        {
            AudioCodec = FormatDescription(
                audioStream.CodecName,
                audioStream.CodecLongName);

            SampleRate = audioStream.SampleRate.HasValue
                ? $"{audioStream.SampleRate.Value.ToString(
                    "N0",
                    CultureInfo.CurrentCulture)} Hz"
                : NotAvailable;

            ChannelCount = audioStream.Channels.HasValue
                ? audioStream.Channels.Value.ToString(
                    CultureInfo.CurrentCulture)
                : NotAvailable;

            ChannelLayout =
                FormatOptionalText(audioStream.ChannelLayout);
        }
    }

    private void ResetDisplayValues()
    {
        FileName = NotAvailable;
        FilePath = NotAvailable;
        ContainerFormat = NotAvailable;
        FileSize = NotAvailable;
        Duration = NotAvailable;
        MediaDuration = null;
        VideoCodec = NotAvailable;
        Resolution = NotAvailable;
        SampleAspectRatio = NotAvailable;
        DisplayAspectRatio = NotAvailable;
        FrameRate = NotAvailable;
        FieldOrder = NotAvailable;
        AudioCodec = NotAvailable;
        SampleRate = NotAvailable;
        ChannelCount = NotAvailable;
        ChannelLayout = NotAvailable;
    }

    private static string FormatDescription(
        string? shortName,
        string? longName)
    {
        if (!string.IsNullOrWhiteSpace(shortName) &&
            !string.IsNullOrWhiteSpace(longName))
        {
            return $"{longName} ({shortName})";
        }

        return FormatOptionalText(longName ?? shortName);
    }

    private static string FormatFileSize(long? fileSizeBytes)
    {
        if (!fileSizeBytes.HasValue || fileSizeBytes.Value < 0)
        {
            return NotAvailable;
        }

        string[] units = ["Bytes", "KiB", "MiB", "GiB", "TiB"];

        var size = (double)fileSizeBytes.Value;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return unitIndex == 0
            ? $"{fileSizeBytes.Value.ToString(
                "N0",
                CultureInfo.CurrentCulture)} Bytes"
            : $"{size.ToString(
                "N2",
                CultureInfo.CurrentCulture)} {units[unitIndex]}";
    }

    private static string FormatDuration(TimeSpan? duration)
    {
        if (!duration.HasValue)
        {
            return NotAvailable;
        }

        var value = duration.Value;

        return $"{(int)value.TotalHours:00}:" +
               $"{value.Minutes:00}:" +
               $"{value.Seconds:00}." +
               $"{value.Milliseconds:000}";
    }

    private static string FormatOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? NotAvailable
            : value;
    }

    private bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
