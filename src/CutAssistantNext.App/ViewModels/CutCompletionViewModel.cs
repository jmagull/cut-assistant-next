using System.Globalization;
using System.IO;
using System.Text;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.ViewModels;

public sealed class CutCompletionViewModel
{
    public CutCompletionViewModel(string outputFilePath, CutPlan cutPlan)
        : this(CreatePlannedResult(outputFilePath, cutPlan)) { }

    private static CutResult CreatePlannedResult(string outputFilePath, CutPlan cutPlan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath);
        ArgumentNullException.ThrowIfNull(cutPlan);
        return CutResult.Planned(outputFilePath, CutlistKeepSegmentBuilder.Build(cutPlan)
            .Select(s => new KeepSegment(s.Start, s.Duration)));
    }

    public CutCompletionViewModel(CutResult result, string completionTitle = "Schneiden abgeschlossen")
    {
        ArgumentNullException.ThrowIfNull(result);
        Result = result;
        CompletionTitle = completionTitle;
        OutputFilePath = result.OutputFilePath;
        FileName = Path.GetFileName(OutputFilePath);
        var checkPoints = new List<CutCompletionCheckPoint>
        {
            new("Anfang", TimeSpan.Zero) { Kind = CutCompletionCheckPointKind.Beginning }
        };
        for (var index = 0; index < result.Timeline.Transitions.Count; index++)
            checkPoints.Add(new($"Übergang {index + 1}", result.Timeline.Transitions[index]) { TransitionIndex = index });
        checkPoints.Add(new("Ende", result.Timeline.Duration) { Kind = CutCompletionCheckPointKind.End });
        CheckPoints = checkPoints.Select((point, index) => point with { CheckPointIndex = index }).ToList().AsReadOnly();
        Transitions = CheckPoints.Where(point => point.IsTransition).ToList().AsReadOnly();

        var text = new StringBuilder();
        text.AppendLine(CompletionTitle);
        text.AppendLine(OutputFilePath);
        text.AppendLine();
        text.AppendLine(TimeReferenceText);
        if (!string.IsNullOrWhiteSpace(ReviewNotice)) text.AppendLine(ReviewNotice);
        foreach (var checkPoint in CheckPoints)
        {
            text.AppendLine($"{checkPoint.PositionText} – {checkPoint.Label}");
        }

        text.AppendLine();
        text.Append(CheckInstructionText);
        ClipboardText = text.ToString();
    }

    public CutResult Result { get; }
    public string CompletionTitle { get; }
    public bool AreTransitionsMeasured => Result.TimingKind == CutReviewTimingKind.Measured;
    public string ReviewNotice => Result.Notice ?? string.Empty;
    public string OutputFilePath { get; }
    public string FileName { get; }
    public IReadOnlyList<CutCompletionCheckPoint> CheckPoints { get; }
    public IReadOnlyList<CutCompletionCheckPoint> Transitions { get; }
    public string ClipboardText { get; }

    public string TimeReferenceText => Result.TimingKind switch
    {
        CutReviewTimingKind.Measured => "Zeitangaben im fertigen Film geprüft.",
        CutReviewTimingKind.Located => "Prüfstellen im fertigen Film zugeordnet; einzelne Bildzeiten überlappen sich geringfügig.",
        CutReviewTimingKind.Estimated => "Nur berechnete Prüfstellen: Die tatsächlichen Übergänge sind nicht bestätigt.",
        _ => "Zeitangaben im geschnittenen Film, aus dem Schnittplan berechnet. Kleine Abweichungen sind möglich."
    };

    public string CheckInstructionText =>
        Transitions.Count > 0
        ? "Mit „Prüfen“ Anfang, Übergänge und Ende im Kontrollfenster ansehen. Bitte Bild, Ton und Tonsynchronität kontrollieren."
        : "Mit „Prüfen“ Anfang und Ende im Kontrollfenster ansehen. Bitte Bild, Ton und Tonsynchronität kontrollieren.";
}

public enum CutCompletionCheckPointKind
{
    Beginning,
    Transition,
    End
}

public sealed record CutCompletionCheckPoint(string Label, TimeSpan Position)
{
    public int CheckPointIndex { get; init; }
    public CutCompletionCheckPointKind Kind { get; init; } = CutCompletionCheckPointKind.Transition;
    public int? TransitionIndex { get; init; }
    public bool IsTransition => TransitionIndex.HasValue;

    public string PositionText => string.Create(CultureInfo.InvariantCulture,
        $"{Position.Ticks / TimeSpan.TicksPerHour:00}:{Position.Minutes:00}:{Position.Seconds:00}.{Position.Milliseconds:000}");
}
