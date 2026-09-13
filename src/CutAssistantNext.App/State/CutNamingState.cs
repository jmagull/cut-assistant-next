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
        string suggestedMovieName)
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
    }

    public string NameTemplate { get; }

    public NameTemplateContext NameContext { get; }

    public string SuggestedMovieName { get; }

    public CutNamingState UseSuggestedMovieName(
        string suggestedMovieName)
    {
        return new CutNamingState(
            NameTemplate,
            NameContext,
            suggestedMovieName);
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
