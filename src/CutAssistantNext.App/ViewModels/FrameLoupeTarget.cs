using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.App.ViewModels;

internal sealed record FrameLoupeTarget(
    TimeSpan MediaDuration, TimeSpan Position, CutEdgeSide? Side, RemoveSegment? Segment)
{
    internal static FrameLoupeTarget Capture(CutPlanViewModel cutPlan, PlaybackViewModel playback)
    {
        ArgumentNullException.ThrowIfNull(cutPlan);
        ArgumentNullException.ThrowIfNull(playback);
        if (cutPlan.MediaDuration is not TimeSpan duration || duration <= TimeSpan.Zero || !playback.CanSeek)
        {
            throw new InvalidOperationException("Bitte zuerst eine analysierte Videodatei im Player laden.");
        }

        var side = cutPlan.CanInspectSelectedEdge ? cutPlan.SelectedEdgeSide : null;
        var segment = side.HasValue ? cutPlan.SelectedRemoveSegment : null;
        if (segment is not null && !cutPlan.RemoveSegments.Contains(segment))
        {
            throw new InvalidOperationException("Die ausgewählte Schnittkante gehört nicht zum aktuellen Schnittplan.");
        }

        // Position is reported by the player, independently of the slider's pending seek value.
        var position = side.HasValue ? cutPlan.SelectedEdgePosition!.Value : playback.Position;
        if (position < TimeSpan.Zero || position > duration)
        {
            throw new InvalidOperationException("Die Ausgangsposition für die Frame-Lupe liegt außerhalb der Mediendatei.");
        }

        return new FrameLoupeTarget(duration, position, side, segment);
    }
}
