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
            new("Anfang", TimeSpan.Zero)
        };
        var outputPosition = TimeSpan.Zero;
        for (var index = 0; index < keepSegments.Count; index++)
        {
            outputPosition += keepSegments[index].Duration;
            if (index < keepSegments.Count - 1)
            {
                checkPoints.Add(new($"Übergang {index + 1}", outputPosition));
            }
        }

        checkPoints.Add(new("Ende", outputPosition));
        CheckPoints = checkPoints.AsReadOnly();

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
    public string ClipboardText { get; }

    public string TimeReferenceText =>
        "Zeitangaben im geschnittenen Film, aus dem Schnittplan berechnet. Kleine Abweichungen sind möglich.";

    public string CheckInstructionText =>
        "Bitte im eigenen Player Deiner Wahl öffnen.";
}

public sealed record CutCompletionCheckPoint(string Label, TimeSpan Position)
{
    public string PositionText => string.Create(CultureInfo.InvariantCulture,
        $"{Position.Ticks / TimeSpan.TicksPerHour:00}:{Position.Minutes:00}:{Position.Seconds:00}.{Position.Milliseconds:000}");
}
