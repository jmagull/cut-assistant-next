using System.ComponentModel;
using System.Runtime.CompilerServices;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Core.Metadata;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.ViewModels;

public sealed class CutlistGenerationViewModel : INotifyPropertyChanged
{
    private NameTemplateContext _nameContext;
    private string _nameTemplate;
    private string _suggestedMovieName;
    private string _author;
    private string _userComment = string.Empty;
    private int? _selectedRating;
    private bool _epgError;
    private string? _actualContent;
    private bool _missingBeginning;
    private bool _missingEnding;
    private bool _missingVideo;
    private bool _missingAudio;
    private bool _otherError;
    private string? _otherErrorDescription;

    internal CutlistGenerationViewModel(
        CutlistSettings settings,
        NameTemplateContext nameContext,
        IReadOnlyCollection<TechnicalNotice>? technicalNotices = null)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(nameContext);

        _nameContext =
            nameContext;

        _nameTemplate =
            settings.DefaultNameTemplate;

        _author =
            settings.DefaultAuthor;

        QuickTexts =
            settings.QuickTexts.ToArray();

        TechnicalNotices =
            technicalNotices?.ToArray()
            ?? [];

        _suggestedMovieName =
            NameTemplateRenderer.Render(
                _nameTemplate,
                _nameContext);
    }

    internal CutlistGenerationViewModel(
        CutlistSettings settings,
        CutNamingState namingState,
        IReadOnlyCollection<TechnicalNotice>? technicalNotices = null)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        ArgumentNullException.ThrowIfNull(
            namingState);

        _nameContext =
            namingState.NameContext;

        _nameTemplate =
            namingState.NameTemplate;

        _author =
            settings.DefaultAuthor;

        QuickTexts =
            settings.QuickTexts.ToArray();

        TechnicalNotices =
            technicalNotices?.ToArray()
            ?? [];

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

            var suggestedMovieName =
                NameTemplateRenderer.Render(
                    value,
                    _nameContext);

            _nameTemplate = value;
            _suggestedMovieName = suggestedMovieName;

            OnPropertyChanged();
            OnPropertyChanged(nameof(SuggestedMovieName));
        }
    }

    public string? Name
    {
        get => _nameContext.Name;
        set => UpdateNameContext(
            _nameContext with
            {
                Name = value
            });
    }

    public string? Season
    {
        get => _nameContext.Season;
        set => UpdateNameContext(
            _nameContext with
            {
                Season = value
            });
    }

    public string? Episode
    {
        get => _nameContext.Episode;
        set => UpdateNameContext(
            _nameContext with
            {
                Episode = value
            });
    }

    public string? EpisodeTitle
    {
        get => _nameContext.EpisodeTitle;
        set => UpdateNameContext(
            _nameContext with
            {
                EpisodeTitle = value
            });
    }

    public string Author
    {
        get => _author;
        set
        {
            if (_author == value)
            {
                return;
            }

            _author = value;
            OnPropertyChanged();
        }
    }

    public string UserComment
    {
        get => _userComment;
        set
        {
            if (_userComment == value)
            {
                return;
            }

            _userComment = value;
            OnPropertyChanged();
        }
    }

    public int? SelectedRating
    {
        get => _selectedRating;
        set
        {
            if (value is < 0 or > 5)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Die Bewertung muss zwischen 0 und 5 liegen.");
            }

            if (_selectedRating == value)
            {
                return;
            }

            _selectedRating = value;
            OnPropertyChanged();
        }
    }
    public bool EpgError
    {
        get => _epgError;
        set
        {
            if (_epgError == value)
            {
                return;
            }

            _epgError = value;
            OnPropertyChanged();
        }
    }

    public string? ActualContent
    {
        get => _actualContent;
        set
        {
            if (_actualContent == value)
            {
                return;
            }

            _actualContent = value;
            OnPropertyChanged();
        }
    }

    public bool MissingBeginning
    {
        get => _missingBeginning;
        set
        {
            if (_missingBeginning == value)
            {
                return;
            }

            _missingBeginning = value;
            OnPropertyChanged();
        }
    }

    public bool MissingEnding
    {
        get => _missingEnding;
        set
        {
            if (_missingEnding == value)
            {
                return;
            }

            _missingEnding = value;
            OnPropertyChanged();
        }
    }

    public bool MissingVideo
    {
        get => _missingVideo;
        set
        {
            if (_missingVideo == value)
            {
                return;
            }

            _missingVideo = value;
            OnPropertyChanged();
        }
    }

    public bool MissingAudio
    {
        get => _missingAudio;
        set
        {
            if (_missingAudio == value)
            {
                return;
            }

            _missingAudio = value;
            OnPropertyChanged();
        }
    }

    public bool OtherError
    {
        get => _otherError;
        set
        {
            if (_otherError == value)
            {
                return;
            }

            _otherError = value;
            OnPropertyChanged();
        }
    }

    public string? OtherErrorDescription
    {
        get => _otherErrorDescription;
        set
        {
            if (_otherErrorDescription == value)
            {
                return;
            }

            _otherErrorDescription = value;
            OnPropertyChanged();
        }
    }

    public IReadOnlyList<string> QuickTexts { get; }

    public IReadOnlyList<TechnicalNotice> TechnicalNotices { get; }

    public void ApplyQuickText(
        string quickText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            quickText);

        UserComment =
            string.IsNullOrWhiteSpace(UserComment)
                ? quickText
                : $"{UserComment} {quickText}";
    }

    public CutlistDocument CreateDocument(
        CutPlan cutPlan,
        string applyToFile,
        string applicationVersion,
        MediaAnalysisResult analysis,
        CutApplicationInfo? intendedCutApplication = null)
    {
        ArgumentNullException.ThrowIfNull(
            cutPlan);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            applyToFile);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            applicationVersion);

        ArgumentNullException.ThrowIfNull(
            analysis);

        var keepSegments =
            CutlistKeepSegmentBuilder.Build(
                cutPlan);

        var general =
            CutlistGeneralMetadata.Create(
                applyToFile,
                applicationVersion,
                analysis,
                intendedCutApplication,
                keepSegments);

        var info =
            CutlistInfoMetadata.Create(
                SuggestedMovieName,
                UserComment,
                TechnicalNotices,
                SelectedRating
                    ?? throw new InvalidOperationException(
                        "Vor dem Erzeugen der Cutlist muss eine Bewertung ausgewählt werden."),
                author: Author,
                epgError: EpgError,
                actualContent: ActualContent,
                missingBeginning: MissingBeginning,
                missingEnding: MissingEnding,
                missingVideo: MissingVideo,
                missingAudio: MissingAudio,
                otherError: OtherError,
                otherErrorDescription: OtherErrorDescription);

        return new CutlistDocument(
            general,
            keepSegments,
            info);
    }

    public CutNamingState CreateNamingState()
    {
        return new CutNamingState(
            _nameTemplate,
            _nameContext);
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

        _nameContext = nameContext;
        _suggestedMovieName = suggestedMovieName;

        OnPropertyChanged(propertyName);
        OnPropertyChanged(nameof(SuggestedMovieName));
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
