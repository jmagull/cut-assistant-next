using System.Globalization;

namespace CutAssistantNext.Media.Playback;

public sealed partial class MpvMediaPlayerService
{
    public async Task<IReadOnlyList<PlayerAudioTrack>> GetAudioTracksAsync(CancellationToken token = default)
    {
        EnsureInitialized();
        if (!int.TryParse(await _client.GetStringPropertyAsync("track-list/count", token), NumberStyles.Integer,
            CultureInfo.InvariantCulture, out var count)) return [];
        if (count is < 0 or > 128) throw new InvalidDataException("Ungültige Anzahl von Wiedergabespuren.");
        var result = new List<PlayerAudioTrack>();
        for (var i = 0; i < count; i++)
        {
            var prefix = $"track-list/{i}/";
            if (await _client.GetStringPropertyAsync(prefix + "type", token) != "audio") continue;
            if (!int.TryParse(await _client.GetStringPropertyAsync(prefix + "id", token), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var id) || id <= 0 || result.Any(t => t.Id == id))
                throw new InvalidDataException("Ungültige Tonspur-ID im Kontrollplayer.");
            var codec = await _client.GetStringPropertyAsync(prefix + "codec", token);
            var language = await _client.GetStringPropertyAsync(prefix + "lang", token);
            var title = await _client.GetStringPropertyAsync(prefix + "title", token);
            var selected = await _client.GetStringPropertyAsync(prefix + "selected", token) == "yes";
            result.Add(new(id, $"Spur {id}: {codec?.ToUpperInvariant()} · {language}" +
                (string.IsNullOrWhiteSpace(title) ? "" : " · " + title), selected));
        }
        return result;
    }

    public async Task SelectAudioTrackAsync(int id, CancellationToken token = default)
    {
        EnsureInitialized();
        if (id <= 0 || !(await GetAudioTracksAsync(token)).Any(t => t.Id == id))
            throw new ArgumentException("Die gewählte Tonspur steht im Kontrollplayer nicht zur Verfügung.", nameof(id));
        await _client.CommandAsync(["set", "aid", id.ToString(CultureInfo.InvariantCulture)], token);
        if (await _client.GetStringPropertyAsync("aid", token) != id.ToString(CultureInfo.InvariantCulture))
            throw new InvalidOperationException("Die gewählte Tonspur wurde vom Kontrollplayer nicht bestätigt.");
    }
}
