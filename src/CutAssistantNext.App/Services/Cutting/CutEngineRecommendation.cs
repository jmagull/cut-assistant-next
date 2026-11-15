using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.Services.Cutting;

internal static class CutEngineRecommendation
{
    internal static string Evaluate(MediaAnalysisResult media, CutPlan plan, VideoKeyframeIndex? keyframes,
        bool mp4BoxAvailable, bool otrCanAvailable)
    {
        const string unclear = "Keine eindeutige Motor-Empfehlung möglich.";
        if (!VideoPreparation.IsMp4(media) || media.VideoStreams is not [{ CodecName: "h264", Index: 0, FramesPerSecond: > 0 }] ||
            media.AudioStreams.Any(audio => audio.CodecName is not ("aac" or "ac3")) || keyframes is null)
            return unclear;

        var ranges = CutlistKeepSegmentBuilder.Build(plan);
        if (ranges.Count == 0) return unclear;
        var allOnKeyframes = ranges.All(range =>
            keyframes.Contains(range.Start) &&
            (range.End == plan.MediaDuration || keyframes.Contains(range.End)));
        var audio = media.AudioStreams.Count > 1
            ? $" {media.AudioStreams.Count} AAC-/AC-3-Tonspuren erkannt."
            : "";
        if (allOnKeyframes && mp4BoxAvailable)
            return "Empfehlung: MP4Box – H.264 in MP4, Schnittgrenzen liegen auf Keyframes." + audio;
        if (!allOnKeyframes && otrCanAvailable)
            return "Empfehlung: otr-can – Schnittgrenzen zwischen Keyframes erfordern einen bildgenauen Schnitt." + audio;
        return unclear;
    }
}
