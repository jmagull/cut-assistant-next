using System.Security.Cryptography;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Media.Analysis;

public static class CutOutputIdentityVerifier
{
    public static async Task<CutOutputIdentity> ReadAsync(string path, CancellationToken token = default)
    {
        var stamp = await Task.Run(() => FileStamp.Read(path), token).ConfigureAwait(false);
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true);
        var hash = Convert.ToHexString(await SHA256.HashDataAsync(stream, token).ConfigureAwait(false));
        if (FileStamp.Read(path) != stamp) throw new IOException("Die Ausgabedatei wurde während der Prüfung verändert.");
        return new(stamp.Length, stamp.LastWriteUtcTicks, hash);
    }

    public static async Task ValidateAsync(string path, CutOutputIdentity identity, bool fullHash,
        CancellationToken token = default)
    {
        var stamp = await Task.Run(() => FileStamp.Read(path), token).ConfigureAwait(false);
        if (stamp.Length != identity.Length || stamp.LastWriteUtcTicks != identity.LastWriteUtcTicks ||
            fullHash && (await ReadAsync(path, token).ConfigureAwait(false)).Sha256 != identity.Sha256)
            throw new IOException("Die Ausgabedatei wurde nach dem Schnitt verändert. Die geprüften Übergangszeiten gehören zur damaligen Fassung.");
    }
}
