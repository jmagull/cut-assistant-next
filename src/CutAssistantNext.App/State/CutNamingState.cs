using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.State;

public sealed class CutNamingState
{
    public CutNamingState(
        string nameTemplate,
        NameTemplateContext nameContext)
        : this(
            nameTemplate,
            nameContext,
            NameTemplateRenderer.Render(
                nameTemplate,
                nameContext))
    {
    }

    private CutNamingState(
        string nameTemplate,
        NameTemplateContext nameContext,
        string suggestedMovieName,
        bool preserveSuggestedMovieName = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            nameTemplate);

        ArgumentNullException.ThrowIfNull(
            nameContext);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            suggestedMovieName);

        NameTemplate =
            nameTemplate;

        NameContext =
            nameContext;

        SuggestedMovieName =
            suggestedMovieName;

        PreserveSuggestedMovieName =
            preserveSuggestedMovieName;
    }

    public string NameTemplate { get; }

    public NameTemplateContext NameContext { get; }

    public string SuggestedMovieName { get; }

    public bool PreserveSuggestedMovieName { get; }

    public CutNamingState UseSuggestedMovieName(
        string suggestedMovieName,
        bool preserveSuggestedMovieName = false)
    {
        return new CutNamingState(
            NameTemplate,
            NameContext,
            suggestedMovieName,
            preserveSuggestedMovieName || PreserveSuggestedMovieName);
    }

    public CutNamingState UseNaming(
        string nameTemplate,
        NameTemplateContext nameContext)
    {
        return new CutNamingState(
            nameTemplate,
            nameContext);
    }
}