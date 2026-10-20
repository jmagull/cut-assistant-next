using System.Globalization;
using TransitionReview;

// Prototype-only adapter for the linked, unchanged review VM. It never creates or edits a CutPlan.
namespace CutAssistantNext.App.ViewModels;

internal sealed class CutCompletionViewModel
{
    public CutCompletionViewModel(AnalysisResult result)
    {
        OutputFilePath = result.OutputFilePath;
        var points = new List<CutCompletionCheckPoint> { new("Anfang", TimeSpan.Zero, CutCompletionCheckPointKind.Beginning) };
        points.AddRange(result.Transitions.Select(t => new CutCompletionCheckPoint($"Übergang {t.Index}",
            Formatting.Time(t.Seconds), CutCompletionCheckPointKind.Transition)));
        points.Add(new("Ende", Formatting.Time(result.Duration), CutCompletionCheckPointKind.End));
        CheckPoints = points.AsReadOnly();
        Transitions = points.Where(p => p.IsTransition).ToList().AsReadOnly();
    }
    public string OutputFilePath { get; }
    public string FileName => Path.GetFileName(OutputFilePath);
    public IReadOnlyList<CutCompletionCheckPoint> CheckPoints { get; }
    public IReadOnlyList<CutCompletionCheckPoint> Transitions { get; }
}
internal enum CutCompletionCheckPointKind { Beginning, Transition, End }
internal sealed record CutCompletionCheckPoint(string Label, TimeSpan Position, CutCompletionCheckPointKind Kind)
{
    public bool IsTransition => Kind == CutCompletionCheckPointKind.Transition;
    public string PositionText => string.Create(CultureInfo.InvariantCulture,
        $"{Position.Ticks / TimeSpan.TicksPerHour:00}:{Position.Minutes:00}:{Position.Seconds:00}.{Position.Milliseconds:000}");
}
