namespace CutAssistantNext.Core.Cutting;

public enum CutReviewTimingKind { Planned, Measured, Estimated }

public sealed record CutOutputIdentity(long Length, long LastWriteUtcTicks, string Sha256);

/// <summary>Presentation times in the completed output, never source cut boundaries.</summary>
public sealed class CutOutputTimeline
{
    public CutOutputTimeline(TimeSpan duration, IEnumerable<TimeSpan> transitions, CutOutputIdentity? identity = null)
    {
        ArgumentNullException.ThrowIfNull(transitions);
        if (duration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(duration));
        var positions = transitions.ToArray();
        for (var i = 0; i < positions.Length; i++)
            if (positions[i] <= TimeSpan.Zero || positions[i] >= duration || i > 0 && positions[i] <= positions[i - 1])
                throw new ArgumentException("Ausgabeübergänge müssen aufsteigend innerhalb des Films liegen.", nameof(transitions));
        if (identity is not null && (identity.Length <= 0 || identity.LastWriteUtcTicks <= 0 ||
            identity.Sha256.Length != 64 || !identity.Sha256.All(Uri.IsHexDigit)))
            throw new ArgumentException("Ungültige Identität der Ausgabedatei.", nameof(identity));
        Duration = duration;
        Transitions = Array.AsReadOnly(positions);
        Identity = identity;
    }
    public TimeSpan Duration { get; }
    public IReadOnlyList<TimeSpan> Transitions { get; }
    public CutOutputIdentity? Identity { get; }
}

public sealed class CutResult
{
    public CutResult(string outputFilePath, CutOutputTimeline timeline, CutReviewTimingKind timingKind,
        string? notice = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath);
        ArgumentNullException.ThrowIfNull(timeline);
        if (!Enum.IsDefined(timingKind)) throw new ArgumentOutOfRangeException(nameof(timingKind));
        if (timingKind == CutReviewTimingKind.Measured && timeline.Identity is null)
            throw new ArgumentException("Geprüfte Ausgabezeiten benötigen eine Dateizuordnung.", nameof(timeline));
        OutputFilePath = outputFilePath;
        Timeline = timeline;
        TimingKind = timingKind;
        Notice = notice;
    }
    public string OutputFilePath { get; }
    public CutOutputTimeline Timeline { get; }
    public CutReviewTimingKind TimingKind { get; }
    public string? Notice { get; }

    public static CutResult Planned(string outputFilePath, IEnumerable<KeepSegment> segments,
        string? notice = null, bool analysisFailed = false)
    {
        ArgumentNullException.ThrowIfNull(segments);
        var keeps = segments.ToArray();
        if (keeps.Length == 0) throw new ArgumentException("Der Schnittplan enthält keinen Filmabschnitt.", nameof(segments));
        var transitions = new List<TimeSpan>();
        var duration = TimeSpan.Zero;
        for (var i = 0; i < keeps.Length; i++)
        {
            ArgumentNullException.ThrowIfNull(keeps[i]);
            duration += keeps[i].Duration;
            if (i < keeps.Length - 1) transitions.Add(duration);
        }
        return new(outputFilePath, new(duration, transitions),
            analysisFailed ? CutReviewTimingKind.Estimated : CutReviewTimingKind.Planned, notice);
    }
}
