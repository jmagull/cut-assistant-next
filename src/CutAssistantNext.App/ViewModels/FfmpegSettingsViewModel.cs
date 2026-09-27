using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class FfmpegSettingsViewModel :
    INotifyPropertyChanged
{
    private string _ffprobeExecutablePath;
    private string _ffmpegExecutablePath;

    public FfmpegSettingsViewModel(FfmpegSettings settings)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        _ffprobeExecutablePath =
            settings.FfprobeExecutablePath;

        _ffmpegExecutablePath =
            settings.FfmpegExecutablePath;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string FfprobeExecutablePath
    {
        get => _ffprobeExecutablePath;
        set
        {
            if (_ffprobeExecutablePath == value)
            {
                return;
            }

            _ffprobeExecutablePath =
                value;

            OnPropertyChanged();
        }
    }

    public string FfmpegExecutablePath
    {
        get => _ffmpegExecutablePath;
        set
        {
            if (_ffmpegExecutablePath == value)
            {
                return;
            }

            _ffmpegExecutablePath =
                value;

            OnPropertyChanged();
        }
    }

    public FfmpegSettings CreateSettings()
    {
        return new FfmpegSettings
        {
            FfprobeExecutablePath =
                FfprobeExecutablePath,

            FfmpegExecutablePath =
                FfmpegExecutablePath
        };
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
