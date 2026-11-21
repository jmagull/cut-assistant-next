using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.ViewModels;

internal sealed class VideoFolderSettingsViewModel : INotifyPropertyChanged
{
    private string _originalVideosDirectory;
    private string _customWorkingDirectory;
    private CutWorkingDirectoryMode _workingDirectoryMode;
    private string _cutVideosDirectory;
    private string _ownCutlistsDirectory;

    internal VideoFolderSettingsViewModel(VideoFolderSettings settings, IWorkingDirectoryInfoReader? infoReader = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        StorageInfo = new(infoReader);
        var normalized = settings.Normalize();
        _originalVideosDirectory = normalized.OriginalVideosDirectory;
        _customWorkingDirectory = normalized.CustomWorkingDirectory;
        _workingDirectoryMode = normalized.WorkingDirectoryMode;
        _cutVideosDirectory = normalized.CutVideosDirectory;
        _ownCutlistsDirectory = normalized.OwnCutlistsDirectory;
    }

    public WorkingDirectoryInfoViewModel StorageInfo { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CustomWorkingDirectory
    {
        get => _customWorkingDirectory;
        set => SetField(ref _customWorkingDirectory, value);
    }

    public int WorkingDirectoryModeIndex
    {
        get => (int)_workingDirectoryMode;
        set
        {
            if (_workingDirectoryMode == (CutWorkingDirectoryMode)value) return;
            _workingDirectoryMode = (CutWorkingDirectoryMode)value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDirectoryModeIndex)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCustomWorkingDirectory)));
        }
    }

    public bool IsCustomWorkingDirectory => _workingDirectoryMode == CutWorkingDirectoryMode.Custom;

    public string OriginalVideosDirectory
    {
        get => _originalVideosDirectory;
        set => SetField(ref _originalVideosDirectory, value);
    }

    public string CutVideosDirectory
    {
        get => _cutVideosDirectory;
        set => SetField(ref _cutVideosDirectory, value);
    }

    public string OwnCutlistsDirectory
    {
        get => _ownCutlistsDirectory;
        set => SetField(ref _ownCutlistsDirectory, value);
    }

    internal VideoFolderSettings CreateSettings() => new VideoFolderSettings
    {
        WorkingDirectoryMode = _workingDirectoryMode,
        CustomWorkingDirectory = CustomWorkingDirectory,
        OriginalVideosDirectory = OriginalVideosDirectory,
        CutVideosDirectory = CutVideosDirectory,
        OwnCutlistsDirectory = OwnCutlistsDirectory
    }.Normalize();

    private void SetField(ref string field, string value, [CallerMemberName] string? name = null)
    {
        if (field == value) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
