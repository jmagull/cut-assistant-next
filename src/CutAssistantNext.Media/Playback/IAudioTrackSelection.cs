namespace CutAssistantNext.Media.Playback;

public sealed record PlayerAudioTrack(int Id, string Description, bool Selected);

/// <summary>Optional capability for the independent review player.</summary>
public interface IAudioTrackSelection
{
    Task<IReadOnlyList<PlayerAudioTrack>> GetAudioTracksAsync(CancellationToken token = default);
    Task SelectAudioTrackAsync(int id, CancellationToken token = default);
}
