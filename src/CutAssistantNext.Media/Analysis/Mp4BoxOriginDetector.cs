using System.Buffers.Binary;

namespace CutAssistantNext.Media.Analysis;

/// <summary>Reads GPAC's producer marker without scanning media payloads or relying on filenames.</summary>
public static class Mp4BoxOriginDetector
{
    public const string EnthusiastUnavailableMessage =
        "MP4Box-Filmteil erkannt. AUDIO-ENTHUSIAST steht dafür nicht zur Verfügung. Bitte QUICK AND COPY verwenden.";

    private static readonly byte[] Marker = "IsoMedia File Produced with GPAC"u8.ToArray();

    public static async Task<bool> IsMp4BoxOutputAsync(string path, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, FileOptions.Asynchronous | FileOptions.RandomAccess);
        return await HasGpacMarkerAsync(stream, token).ConfigureAwait(false);
    }

    internal static async Task<bool> HasGpacMarkerAsync(Stream stream, CancellationToken token)
    {
        var header = new byte[8];
        var payload = new byte[Marker.Length];
        // Bound header work even for unusual files with thousands of tiny boxes.
        for (var count = 0; count < 4096; count++)
        {
            token.ThrowIfCancellationRequested();
            var offset = stream.Position;
            var remaining = stream.Length - offset;
            if (remaining < 8) return false;
            await stream.ReadExactlyAsync(header, token).ConfigureAwait(false);
            ulong size = BinaryPrimitives.ReadUInt32BigEndian(header);
            var free = header.AsSpan(4).SequenceEqual("free"u8);
            var headerSize = 8;
            if (size == 1)
            {
                if (remaining < 16) return false;
                await stream.ReadExactlyAsync(header, token).ConfigureAwait(false);
                size = BinaryPrimitives.ReadUInt64BigEndian(header);
                headerSize = 16;
            }
            else if (size == 0)
            {
                size = (ulong)remaining;
            }
            // Invalid/truncated boxes do not establish a producer. Normal media validation still applies.
            if (size < (ulong)headerSize || size > (ulong)remaining) return false;
            if (free && size - (ulong)headerSize >= (ulong)Marker.Length)
            {
                await stream.ReadExactlyAsync(payload, token).ConfigureAwait(false);
                if (payload.AsSpan().SequenceEqual(Marker)) return true;
            }
            stream.Position = offset + (long)size;
        }
        return false;
    }
}
