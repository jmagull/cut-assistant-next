using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.State;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.ViewModels;

public sealed class CutOutputViewModel : INotifyPropertyChanged
{
    private NameTemplateContext _nameContext;
    private string _nameTemplate;
    private string _suggestedMovieName;
    private string _outputMovieName;
    private string _defaultNameTemplate;
    private bool _preserveSuggestedMovieName;

    public CutOutputViewModel(
        string nameTemplate,
        NameTemplateContext nameContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            nameTemplate);

        ArgumentNullException.ThrowIfNull(
            nameContext);

        _nameTemplate =
            nameTemplate;

        _defaultNameTemplate =
            nameTemplate;

        _nameContext =
            nameContext;

        _suggestedMovieName =
            NameTemplateRenderer.Render(
                _nameTemplate,
                _nameContext);

        _outputMovieName = _suggestedMovieName;
    }
    public CutOutputViewModel(
        CutNamingState namingState)
    {
        ArgumentNullException.ThrowIfNull(
            namingState);

        _nameTemplate =
            namingState.NameTemplate;

        _defaultNameTemplate =
            namingState.NameTemplate;

        _nameContext =
            namingState.NameContext;

        _suggestedMovieName =
            namingState.SuggestedMovieName;

        _outputMovieName = namingState.OutputMovieName ?? _suggestedMovieName;
        _preserveSuggestedMovieName = namingState.PreserveSuggestedMovieName;
    }

    public CutOutputViewModel(
        CutNamingState namingState,
        string defaultNameTemplate)
    {
        ArgumentNullException.ThrowIfNull(
            namingState);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            defaultNameTemplate);

        _nameTemplate =
            namingState.NameTemplate;

        _defaultNameTemplate =
            defaultNameTemplate;

        _nameContext =
            namingState.NameContext;

        _suggestedMovieName =
            namingState.SuggestedMovieName;

        _outputMovieName = namingState.OutputMovieName ?? _suggestedMovieName;
        _preserveSuggestedMovieName = namingState.PreserveSuggestedMovieName;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string NameTemplate
    {
        get => _nameTemplate;
        set
        {
            if (_nameTemplate == value)
            {
                return;
            }

            _nameTemplate =
                value;

            OnPropertyChanged();
        }
    }

    public string? Name
    {
        get => _nameContext.Name;
        set =>
            UpdateNameContext(
                _nameContext with
                {
                    Name = value
                });
    }

    public string? Season
    {
        get => _nameContext.Season;
        set =>
            UpdateNameContext(
                _nameContext with
                {
                    Season = value
                });
    }

    public string? Episode
    {
        get => _nameContext.Episode;
        set =>
            UpdateNameContext(
                _nameContext with
                {
                    Episode = value
                });
    }

    public string? EpisodeTitle
    {
        get => _nameContext.EpisodeTitle;
        set =>
            UpdateNameContext(
                _nameContext with
                {
                    EpisodeTitle = value
                });
    }

    public void GenerateSuggestedMovieName()
    {
        var suggestedMovieName =
            NameTemplateRenderer.Render(
                _defaultNameTemplate,
                _nameContext);

        if (_nameTemplate != _defaultNameTemplate)
        {
            _nameTemplate = _defaultNameTemplate;
            OnPropertyChanged(nameof(NameTemplate));
        }

        if (_suggestedMovieName != suggestedMovieName)
        {
            _suggestedMovieName = suggestedMovieName;
            OnPropertyChanged(nameof(SuggestedMovieName));
        }

        // Rebuilding the suggestion deliberately replaces any manual output name.
        OutputMovieName = suggestedMovieName;
        _preserveSuggestedMovieName = false;
    }

    public CutNamingState CreateNamingState()
    {
        return new CutNamingState(
            _nameTemplate,
            _nameContext)
        {
            OutputMovieName = _outputMovieName
        }
            .UseSuggestedMovieName(
                _suggestedMovieName, _preserveSuggestedMovieName);
    }

    /// <summary>The session output choice; independent of the cutlist SuggestedMovieName.</summary>
    public string OutputMovieName
    {
        get => _outputMovieName;
        set
        {
            var outputMovieName = value ?? string.Empty;
            if (_outputMovieName == outputMovieName)
            {
                return;
            }

            _outputMovieName = outputMovieName;
            OnPropertyChanged();
        }
    }

    public string SuggestedMovieName =>
        _suggestedMovieName;

    private void UpdateNameContext(
        NameTemplateContext nameContext,
        [CallerMemberName] string? propertyName = null)
    {
        if (_nameContext == nameContext)
        {
            return;
        }

        _nameContext =
            nameContext;

        OnPropertyChanged(
            propertyName);
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
