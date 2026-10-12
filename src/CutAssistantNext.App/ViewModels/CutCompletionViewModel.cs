using System.Globalization;
using System.IO;
using System.Text;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.ViewModels;

public sealed class CutCompletionViewModel
{
    public CutCompletionViewModel(string outputFilePath, CutPlan cutPlan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath);
        ArgumentNullException.ThrowIfNull(cutPlan);

        var keepSegments = CutlistKeepSegmentBuilder.Build(cutPlan);
        if (keepSegments.Count == 0)
        {
            throw new ArgumentException("Der Schnittplan enthält keinen Filmabschnitt.", nameof(cutPlan));
        }

        OutputFilePath = outputFilePath;
        FileName = Path.GetFileName(outputFilePath);

        var checkPoints = new List<CutCompletionCheckPoint>
        {
            new("Anfang", TimeSpan.Zero) { Kind = CutCompletionCheckPointKind.Beginning }
        };
        var outputPosition = TimeSpan.Zero;
        for (var index = 0; index < keepSegments.Count; index++)
        {
            outputPosition += keepSegments[index].Duration;
            if (index < keepSegments.Count - 1)
            {
                checkPoints.Add(new($"Übergang {index + 1}", outputPosition) { TransitionIndex = index });
            }
        }

        checkPoints.Add(new("Ende", outputPosition) { Kind = CutCompletionCheckPointKind.End });
        CheckPoints = checkPoints.Select((point, index) => point with { CheckPointIndex = index }).ToList().AsReadOnly();
        Transitions = CheckPoints.Where(point => point.IsTransition).ToList().AsReadOnly();

        var text = new StringBuilder();
        text.AppendLine("Schneiden abgeschlossen");
        text.AppendLine(OutputFilePath);
        text.AppendLine();
        text.AppendLine(TimeReferenceText);
        foreach (var checkPoint in CheckPoints)
        {
            text.AppendLine($"{checkPoint.PositionText} – {checkPoint.Label}");
        }

        text.AppendLine();
        text.Append(CheckInstructionText);
        ClipboardText = text.ToString();
    }

    public string OutputFilePath { get; }
    public string FileName { get; }
    public IReadOnlyList<CutCompletionCheckPoint> CheckPoints { get; }
    public IReadOnlyList<CutCompletionCheckPoint> Transitions { get; }
    public string ClipboardText { get; }

    public string TimeReferenceText =>
        "Zeitangaben im geschnittenen Film, aus dem Schnittplan berechnet. Kleine Abweichungen sind möglich.";

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
