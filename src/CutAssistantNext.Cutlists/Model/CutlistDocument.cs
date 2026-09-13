using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;

namespace CutAssistantNext.Cutlists.Model;

public sealed class CutlistDocument
{
    private readonly IReadOnlyList<CutlistKeepSegment> _cuts;

    public CutlistDocument(
        CutlistGeneralMetadata general,
        IReadOnlyList<CutlistKeepSegment> cuts,
        CutlistInfoMetadata info)
    {
        ArgumentNullException.ThrowIfNull(general);
        ArgumentNullException.ThrowIfNull(cuts);
        ArgumentNullException.ThrowIfNull(info);

        if (general.NoOfCuts != cuts.Count)
        {
            throw new ArgumentException(
                "Die Anzahl der Cutlist-Bereiche stimmt nicht mit NoOfCuts überein.",
                nameof(cuts));
        }

        for (var index = 1; index < cuts.Count; index++)
        {
            if (cuts[index].Start < cuts[index - 1].End)
            {
                throw new InvalidDataException(
                    "Diese Cutlist enthält überlappende oder falsch sortierte Schnittbereiche " +
                    "und wurde nicht geladen. Bitte wähle eine andere Cutlist.");
            }
        }

        General = general;
        _cuts = cuts;
        Info = info;
    }

    public CutlistGeneralMetadata General { get; }

    public IReadOnlyList<CutlistKeepSegment> Cuts =>
        _cuts;

    public CutlistInfoMetadata Info { get; }
}
