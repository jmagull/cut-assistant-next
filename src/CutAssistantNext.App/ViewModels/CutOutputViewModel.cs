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

        _outputMovieName = _suggestedMovieName;
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

        _outputMovieName = _suggestedMovieName;
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

        if (_suggestedMovieName != suggestedMovieName)
        {
            _suggestedMovieName = suggestedMovieName;
            OnPropertyChanged(nameof(SuggestedMovieName));
        }

        // Rebuilding the suggestion deliberately replaces any manual output name.
        OutputMovieName = suggestedMovieName;
    }

    public CutNamingState CreateNamingState()
    {
        return new CutNamingState(
            _nameTemplate,
            _nameContext)
            .UseSuggestedMovieName(
                _suggestedMovieName);
    }

    /// <summary>The name for this output only; it is not stored in cutlist naming state.</summary>
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
