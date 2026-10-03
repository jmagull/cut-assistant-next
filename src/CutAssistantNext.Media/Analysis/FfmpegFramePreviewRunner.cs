using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public sealed class FfmpegFramePreviewRunner : IFramePreviewRunner
{
    private readonly string _ffmpegPath;

    public FfmpegFramePreviewRunner(string ffmpegPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ffmpegPath);
        _ffmpegPath = ffmpegPath;
    }

    internal static IReadOnlyList<string> BuildArguments(
        string mediaFilePath, int streamIndex, VideoTimeBase timeBase, long pts)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);
        ArgumentNullException.ThrowIfNull(timeBase);
        if (streamIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(streamIndex));
        }

        // select evaluates numbers as doubles; reject integer timestamps it cannot distinguish.
        if (pts is < -9007199254740991L or > 9007199254740991L)
        {
            throw new ArgumentOutOfRangeException(nameof(pts),
                "Dieser PTS ist für eine eindeutige Bildvorschau zu groß.");
        }

        var arguments = new List<string>
        {
            "-hide_banner", "-nostdin", "-loglevel", "info", "-copyts"
        };
        var seconds = timeBase.ToSeconds(pts);
        if (seconds > 2)
        {
            // Absolute source timestamps, including non-zero container start offsets.
            arguments.AddRange(["-seek_timestamp", "1", "-ss",
                (seconds - 2).ToString(CultureInfo.InvariantCulture)]);
        }

        arguments.AddRange([
            "-t", "6", "-i", mediaFilePath,
            "-map", $"0:{streamIndex.ToString(CultureInfo.InvariantCulture)}",
            "-an", "-sn", "-dn",
            "-vf", $"settb=expr={timeBase.Numerator}/{timeBase.Denominator}," +
                $"select=eq(pts\\,{pts.ToString(CultureInfo.InvariantCulture)}),showinfo," +
                "scale=w='min(1280,iw*sar)':h='min(720,ih)'," +
                "scale=w='max(2,trunc(ih*dar/2)*2)':h=ih,setsar=1",
            "-frames:v", "1", "-fps_mode", "passthrough",
            "-c:v", "png", "-f", "image2pipe", "pipe:1"
        ]);
        return arguments;
    }

    internal static void ValidateEvidence(string log, long pts, VideoTimeBase timeBase)
    {
        var bases = Regex.Matches(log, @"config in time_base:\s*(\d+)/(\d+)", RegexOptions.CultureInvariant);
        var frames = Regex.Matches(log, @"\bn:\s*\d+\s+pts:\s*(-?\d+)\s+pts_time:", RegexOptions.CultureInvariant);
        if (bases.Count != 1 || frames.Count != 1 ||
            !long.TryParse(frames[0].Groups[1].Value, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var actualPts) || actualPts != pts ||
            !int.TryParse(bases[0].Groups[1].Value, out var numerator) ||
            !int.TryParse(bases[0].Groups[2].Value, out var denominator) ||
            numerator <= 0 || denominator <= 0 ||
            (long)numerator * timeBase.Denominator != (long)denominator * timeBase.Numerator)
        {
            throw new InvalidOperationException(
                "Das Vorschaubild konnte dem ausgewählten Quellzeitstempel nicht eindeutig zugeordnet werden.");
        }
    }

    public async Task<FramePreview> RunAsync(
        string mediaFilePath, int videoStreamIndex, VideoTimeBase timeBase, long pts,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var fullPath = Path.GetFullPath(mediaFilePath);
        var arguments = BuildArguments(fullPath, videoStreamIndex, timeBase, pts);
        if (!File.Exists(_ffmpegPath))
        {
            throw new FileNotFoundException("ffmpeg.exe wurde nicht gefunden.", _ffmpegPath);
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Die Quelldatei wurde nicht gefunden.", fullPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException("FFmpeg konnte nicht gestartet werden.");
        }

        using var registration = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (InvalidOperationException)
            {
                // Already exited.
            }
        });

        using var output = new MemoryStream();
        var imageTask = process.StandardOutput.BaseStream.CopyToAsync(output);
        var errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
        await imageTask.ConfigureAwait(false);
        var log = await errorTask.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        var bytes = output.ToArray();
        if (process.ExitCode != 0 || bytes.Length < 8 ||
            !bytes.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
        {
            throw new InvalidOperationException(
                "Für diesen Quellframe konnte kein Vorschaubild erzeugt werden. " +
                $"FFmpeg-Exit-Code: {process.ExitCode}.",
                new InvalidOperationException(log.Trim()));
        }

        ValidateEvidence(log, pts, timeBase);
        return new FramePreview(bytes, pts, timeBase);
    }
}
