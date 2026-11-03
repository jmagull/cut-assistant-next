using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.Services;

internal static class CutlistSaveGuard
{
    internal const string Title = "Schnitt noch nicht abgeschlossen";
    internal const string Message = "Es werden nur vollständige Schnitte gespeichert. Bitte den begonnenen Schnitt zuerst beenden oder die offene Schnittmarkierung entfernen.";

    internal static CutPlan CreateSnapshot(CutPlanViewModel plan)
    {
        if (plan.PendingStart.HasValue) throw new InvalidOperationException(Message);
        return plan.CreateCutPlanSnapshot();
    }
}
