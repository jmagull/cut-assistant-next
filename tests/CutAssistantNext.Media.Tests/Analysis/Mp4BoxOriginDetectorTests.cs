using System.Buffers.Binary;
using System.Text;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Joining;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class Mp4BoxOriginDetectorTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-origin-" + Guid.NewGuid().ToString("N"));
    private static readonly byte[] Marker = Encoding.ASCII.GetBytes("IsoMedia File Produced with GPAC 26.07-rev0-test\0");

    public Mp4BoxOriginDetectorTests() => Directory.CreateDirectory(_root);

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task FindsProducerWithNormalExtendedOrEndOfFileBox(bool extended, bool toEnd)
    {
        using var stream = new MemoryStream(Box("free", Marker, extended, toEnd));
        Assert.True(await Mp4BoxOriginDetector.HasGpacMarkerAsync(stream, CancellationToken.None));
    }

    [Fact]
    public async Task SkipsLargeMediaAndPaddingWithoutScanningTheirContents()
    {
        using var stream = new CountingStream([
            .. Box("ftyp", "isom"u8.ToArray()),
            .. Box("mdat", new byte[2 * 1024 * 1024], extended: true),
            .. Box("free", new byte[1024 * 1024]),
            .. Box("free", Marker)]);
        Assert.True(await Mp4BoxOriginDetector.HasGpacMarkerAsync(stream, CancellationToken.None));
        Assert.InRange(stream.BytesRead, 1, 256);
    }

    [Theory]
    [InlineData("moov")]
    [InlineData("mdat")]
    public async Task DoesNotMistakeTextInsideOtherBoxesForAProducer(string type)
    {
        using var stream = new MemoryStream(Box(type, Marker));
        Assert.False(await Mp4BoxOriginDetector.HasGpacMarkerAsync(stream, CancellationToken.None));
    }

    [Fact]
    public async Task HandlesTruncatedAndOutOfBoundsHeadersWithoutAnUnboundedRead()
    {
        byte[][] invalid = [[], [0, 0, 0], [0, 0, 0, 1, 102, 114, 101, 101],
            [0, 0, 0, 7, 102, 114, 101, 101], [255, 255, 255, 255, 102, 114, 101, 101],
            [0, 0, 0, 1, 102, 114, 101, 101, 255, 255, 255, 255, 255, 255, 255, 255]];
        foreach (var bytes in invalid)
        {
            using var stream = new MemoryStream(bytes);
            Assert.False(await Mp4BoxOriginDetector.HasGpacMarkerAsync(stream, CancellationToken.None));
        }
    }

    [Fact]
    public async Task BoundsTheNumberOfHeaders()
    {
        using var stream = new CountingStream(Enumerable.Range(0, 5000).SelectMany(_ => Box("skip", [])).ToArray());
        Assert.False(await Mp4BoxOriginDetector.HasGpacMarkerAsync(stream, CancellationToken.None));
        Assert.Equal(4096 * 8, stream.BytesRead);
    }

    [Fact]
    public async Task RenamingPreservesDetectionAndFilenameAloneDoesNotBlock()
    {
        var original = Path.Combine(_root, "MP4Box.mp4");
        await File.WriteAllBytesAsync(original, Box("ftyp", "isom"u8.ToArray()));
        Assert.False(await Mp4BoxOriginDetector.IsMp4BoxOutputAsync(original));
        await File.WriteAllBytesAsync(original, Box("free", Marker));
        var renamed = Path.Combine(_root, "Film.mp4");
        File.Move(original, renamed);
        Assert.True(await Mp4BoxOriginDetector.IsMp4BoxOutputAsync(renamed));
        // Detection releases its handle, allowing the user to replace or move the file afterwards.
        File.Move(renamed, original);
    }

    [Fact]
    public async Task CancellationStopsBeforeOpeningTheFile()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Mp4BoxOriginDetector.IsMp4BoxOutputAsync(Path.Combine(_root, "missing.mp4"), cancellation.Token));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EnthusiastRejectsEitherMarkedSourceBeforeStartingAnyTool(bool firstMarked)
    {
        var first = Path.Combine(_root, "Part1.mp4");
        var second = Path.Combine(_root, "Part2.mp4");
        var output = Path.Combine(_root, "result.mp4");
        var firstBytes = Box(firstMarked ? "free" : "mdat", Marker);
        var secondBytes = Box(firstMarked ? "mdat" : "free", Marker);
        await File.WriteAllBytesAsync(first, firstBytes);
        await File.WriteAllBytesAsync(second, secondBytes);
        var service = new Mp4JoinService("missing-ffmpeg", "missing-ffprobe");
        var error = await Assert.ThrowsAsync<InvalidDataException>(() => service.RunAsync(
            new(first, 1, second, 1, output), null, CancellationToken.None));
        Assert.Equal(Mp4BoxOriginDetector.EnthusiastUnavailableMessage, error.Message);
        Assert.False(File.Exists(output));
        Assert.Equal(firstBytes, await File.ReadAllBytesAsync(first));
        Assert.Equal(secondBytes, await File.ReadAllBytesAsync(second));
        Assert.Contains(Mp4BoxOriginDetector.EnthusiastUnavailableMessage,
            await File.ReadAllTextAsync(Directory.GetFiles(_root, "protocol.txt", SearchOption.AllDirectories).Single()));
    }

    private static byte[] Box(string type, byte[] payload, bool extended = false, bool toEnd = false)
    {
        var headerSize = extended ? 16 : 8;
        var bytes = new byte[headerSize + payload.Length];
        BinaryPrimitives.WriteUInt32BigEndian(bytes, toEnd ? 0u : extended ? 1u : (uint)bytes.Length);
        Encoding.ASCII.GetBytes(type).CopyTo(bytes, 4);
        if (extended) BinaryPrimitives.WriteUInt64BigEndian(bytes.AsSpan(8), (ulong)bytes.Length);
        payload.CopyTo(bytes, headerSize);
        return bytes;
    }

    private sealed class CountingStream(byte[] bytes) : MemoryStream(bytes)
    {
        public int BytesRead { get; private set; }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var read = await base.ReadAsync(buffer, cancellationToken);
            BytesRead += read;
            return read;
        }
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);
}
