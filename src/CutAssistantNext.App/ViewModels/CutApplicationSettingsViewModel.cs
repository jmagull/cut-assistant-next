using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class CutApplicationSettingsViewModel :
    INotifyPropertyChanged
{
    private string _name;
    private string _executablePath;
    private string _version;
    private string _options;

    internal CutApplicationSettingsViewModel(
        CutApplicationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _name =
            settings.Name;

        _executablePath =
            settings.ExecutablePath;

        _version =
            settings.Version;

        _options =
            settings.Options;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;

            OnPropertyChanged();
        }
    }

    public string ExecutablePath
    {
        get => _executablePath;
        set
        {
            if (_executablePath == value)
            {
                return;
            }

            _executablePath = value;

            OnPropertyChanged();
        }
    }

    public string Version
    {
        get => _version;
        set
        {
            if (_version == value)
            {
                return;
            }

            _version = value;

            OnPropertyChanged();
        }
    }

    public string Options
    {
        get => _options;
        set
        {
            if (_options == value)
            {
                return;
            }

            _options = value;

            OnPropertyChanged();
        }
    }

    internal CutApplicationSettings CreateSettings()
    {
        return new CutApplicationSettings
        {
            Name = Name,
            ExecutablePath = ExecutablePath,
            Version = Version,
            Options = Options
        };
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
