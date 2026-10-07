using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class VideoFolderSettingsViewModel : INotifyPropertyChanged
{
    private string _originalVideosDirectory;
    private string _cutVideosDirectory;
    private string _ownCutlistsDirectory;

    internal VideoFolderSettingsViewModel(VideoFolderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var normalized = settings.Normalize();
        _originalVideosDirectory = normalized.OriginalVideosDirectory;
        _cutVideosDirectory = normalized.CutVideosDirectory;
        _ownCutlistsDirectory = normalized.OwnCutlistsDirectory;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
