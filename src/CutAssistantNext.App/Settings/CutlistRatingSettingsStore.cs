using System.IO;
using System.Text.Json;
using CutAssistantNext.Core;

namespace CutAssistantNext.App.Settings;

internal enum CutlistRatingState { Attempted, Accepted, Rejected, Unknown }

internal interface ICutlistRatingStore
{
    CutlistRatingState? Get(string key);
    bool TryBegin(string key);
    void Complete(string key, CutlistRatingState state);
}

// Same application Settings directory and JSON mechanism as the existing stores.
// A durable Attempted entry precedes network I/O and survives crashes/unknown responses.
internal sealed class CutlistRatingSettingsStore : ICutlistRatingStore
{
    private static readonly object Gate = new();
    private readonly string _path;

    internal CutlistRatingSettingsStore(string? path = null) => _path = path ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ApplicationInfo.ProductName, "Settings", "cutlist-ratings.json");

    public CutlistRatingState? Get(string key)
    {
        lock (Gate) return Read().TryGetValue(key, out var state) ? state : null;
    }

    public bool TryBegin(string key)
    {
        lock (Gate)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_path))!);
            using var fileLock = new FileStream(_path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            var entries = Read();
            if (entries.ContainsKey(key)) return false;
            entries.Add(key, CutlistRatingState.Attempted);
            Write(entries);
            return true;
        }
    }

    public void Complete(string key, CutlistRatingState state)
    {
        lock (Gate)
        {
            using var fileLock = new FileStream(_path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            var entries = Read();
            if (!entries.ContainsKey(key)) throw new InvalidDataException("Bewertungsversuch fehlt.");
            entries[key] = state;
            Write(entries);
        }
    }

    private Dictionary<string, CutlistRatingState> Read()
    {
        if (!File.Exists(_path)) return [];
        var entries = JsonSerializer.Deserialize<Dictionary<string, CutlistRatingState>>(File.ReadAllText(_path))
            ?? throw new InvalidDataException("Bewertungsspeicher ist ungültig.");
        if (entries.Any(x => x.Key.Length != 64 || !x.Key.All(Uri.IsHexDigit) || !Enum.IsDefined(x.Value)))
            throw new InvalidDataException("Bewertungsspeicher ist ungültig.");
        return entries;
    }

    private void Write(Dictionary<string, CutlistRatingState> entries)
    {
        var temporary = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var file = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(file, entries, new JsonSerializerOptions { WriteIndented = true });
                file.Flush(flushToDisk: true);
            }
            File.Move(temporary, _path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }
}
