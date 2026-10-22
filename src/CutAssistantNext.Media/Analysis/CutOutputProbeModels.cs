using System.Globalization;

namespace CutAssistantNext.Media.Analysis;

internal sealed record TimeBase(long Numerator, long Denominator)
{
    public decimal Seconds(long pts) => checked((decimal)pts * Numerator / Denominator);
    public static TimeBase Parse(string value)
    {
        var parts = value.Split('/');
        if (parts.Length != 2 || !long.TryParse(parts[0], out var n) || !long.TryParse(parts[1], out var d) || n <= 0 || d <= 0)
            throw new InvalidDataException("Ungültige Videozeitbasis.");
        return new(n, d);
    }
}
internal sealed record VideoInfo(TimeBase TimeBase, decimal FormatStart, decimal Duration, string? CodecName = null);
internal sealed record Packet(long Pts, string Hash, string Flags, string Position, long? Dts = null, long? Duration = null);
internal sealed record DecodedFrame(long Pts, string Position);
internal sealed record FileStamp(long Length, long LastWriteUtcTicks)
{
    public static FileStamp Read(string path)
    {
        var info = new FileInfo(path);
        if (!info.Exists || info.Length == 0) throw new FileNotFoundException("Mediendatei fehlt oder ist leer.", path);
        return new(info.Length, info.LastWriteTimeUtc.Ticks);
    }
}
internal interface IVideoProbe
{
    Task<VideoInfo> InspectAsync(string path, CancellationToken token);
    IAsyncEnumerable<Packet> ReadPacketsAsync(string path, CancellationToken token);
    Task<IReadOnlyList<DecodedFrame>> DecodeAsync(string path, decimal start, decimal end, CancellationToken token);
}

internal static class Formatting
{
    public static string Seconds(decimal value) => value.ToString(CultureInfo.InvariantCulture);
    public static TimeSpan Time(decimal value) => TimeSpan.FromTicks(checked((long)decimal.Round(value * TimeSpan.TicksPerSecond)));
}
