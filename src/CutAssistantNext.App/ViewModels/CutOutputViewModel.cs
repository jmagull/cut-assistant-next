using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.ViewModels;

public sealed class CutOutputViewModel : INotifyPropertyChanged
{
    private NameTemplateContext _nameContext;
    private string _nameTemplate;
    private string _suggestedMovieName;

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

        _nameContext =
            nameContext;

        _suggestedMovieName =
            NameTemplateRenderer.Render(
                _nameTemplate,
                _nameContext);
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

            var suggestedMovieName =
                NameTemplateRenderer.Render(
                    value,
                    _nameContext);

            _nameTemplate =
                value;

            _suggestedMovieName =
                suggestedMovieName;

            OnPropertyChanged();
            OnPropertyChanged(
                nameof(SuggestedMovieName));
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

        var suggestedMovieName =
            NameTemplateRenderer.Render(
                _nameTemplate,
                nameContext);

        _nameContext =
            nameContext;

        _suggestedMovieName =
            suggestedMovieName;

        OnPropertyChanged(
            propertyName);

        OnPropertyChanged(
            nameof(SuggestedMovieName));
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
