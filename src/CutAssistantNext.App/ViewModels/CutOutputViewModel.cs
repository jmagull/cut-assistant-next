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

        if (_suggestedMovieName == suggestedMovieName)
        {
            return;
        }

        _suggestedMovieName =
            suggestedMovieName;

        OnPropertyChanged(
            nameof(SuggestedMovieName));
    }

    public CutNamingState CreateNamingState()
    {
        return new CutNamingState(
            _nameTemplate,
            _nameContext)
            .UseSuggestedMovieName(
                _suggestedMovieName);
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
