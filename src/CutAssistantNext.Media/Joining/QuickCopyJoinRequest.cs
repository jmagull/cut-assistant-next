using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Joining;

public sealed record QuickCopyTrackPair(int FirstAudioIndex, int SecondAudioIndex);

public sealed record QuickCopyJoinRequest(string FirstPath, string SecondPath,
    IReadOnlyList<QuickCopyTrackPair> Tracks, string OutputPath, bool MappingConfirmed)
{
    public void ValidateSelection()
    {
        ArgumentNullException.ThrowIfNull(Tracks);
        if (!MappingConfirmed || Tracks.Count is < 1 or > 16 ||
            Tracks.Any(t => t.FirstAudioIndex < 0 || t.SecondAudioIndex < 0) ||
            Tracks.Select(t => t.FirstAudioIndex).Distinct().Count() != Tracks.Count ||
            Tracks.Select(t => t.SecondAudioIndex).Distinct().Count() != Tracks.Count)
            throw new ArgumentException("Bitte ein bis sechzehn eindeutige Tonspurpaare auswählen und die Zuordnung bestätigen.");
    }
}

/// <summary>Only unambiguous matches are proposed. The caller must display and confirm all omissions.</summary>
public static class QuickCopyAudioPairing
{
    public static IReadOnlyList<QuickCopyTrackPair> Suggest(IReadOnlyList<AudioStreamInfo> first, IReadOnlyList<AudioStreamInfo> second)
    {
        static string Language(string? value) => value?.ToLowerInvariant() switch
        {
            "de" or "ger" => "deu",
            "en" => "eng",
            null or "" => "und",
            var text => text
        };
        static bool Supported(AudioStreamInfo a) => a.CodecName is "aac" or "ac3" &&
            a.SampleRate is 44100 or 48000 && a.Channels is 1 or 2 or 6 && !string.IsNullOrEmpty(a.ChannelLayout);
        static bool Match(AudioStreamInfo a, AudioStreamInfo b) => Supported(a) && Supported(b) &&
            a.CodecName == b.CodecName && a.SampleRate == b.SampleRate && a.Channels == b.Channels &&
            a.ChannelLayout == b.ChannelLayout && Language(a.Language) == Language(b.Language);
        return first.Where(a => second.Count(b => Match(a, b)) == 1 && first.Count(b => Match(a, b)) == 1)
            .Select(a => new QuickCopyTrackPair(a.Index, second.Single(b => Match(a, b)).Index)).ToArray();
    }
}
