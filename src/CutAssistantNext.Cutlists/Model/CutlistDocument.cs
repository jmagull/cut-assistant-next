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

        General = general;
        _cuts = cuts;
        Info = info;
    }

    public CutlistGeneralMetadata General { get; }

    public IReadOnlyList<CutlistKeepSegment> Cuts =>
        _cuts;

    public CutlistInfoMetadata Info { get; }
}
