using System.Diagnostics;
using System.Globalization;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public sealed class FfprobeFrameRunner : IFrameAnalysisRunner
{
    private readonly string _ffprobePath;

    public FfprobeFrameRunner(string ffprobePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ffprobePath);
        _ffprobePath = ffprobePath;
    }

    internal static IReadOnlyList<string> BuildArguments(
        string mediaFilePath,
        int videoStreamIndex,
        TimeSpan intervalStart,
        TimeSpan intervalEnd)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);
        if (videoStreamIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(videoStreamIndex));
        }

        if (intervalEnd <= intervalStart)
        {
            throw new ArgumentOutOfRangeException(nameof(intervalEnd));
        }

        var start = ((decimal)intervalStart.Ticks / TimeSpan.TicksPerSecond)
            .ToString(CultureInfo.InvariantCulture);
        var end = ((decimal)intervalEnd.Ticks / TimeSpan.TicksPerSecond)
            .ToString(CultureInfo.InvariantCulture);

        return [
            "-v", "error",
            "-select_streams", videoStreamIndex.ToString(CultureInfo.InvariantCulture),
            "-read_intervals", $"{start}%{end}",
            "-show_frames",
            "-show_entries",
            "frame=media_type,stream_index,pts,best_effort_timestamp,pkt_dts,duration,pkt_duration,key_frame,pict_type:" +
            "stream=index,codec_type,time_base,start_time:format=start_time",
            "-of", "json",
            mediaFilePath
        ];
    }

    public async Task<VideoFrameWindow> RunAsync(
        string mediaFilePath,
        int videoStreamIndex,
        TimeSpan intervalStart,
        TimeSpan intervalEnd,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var arguments = BuildArguments(mediaFilePath, videoStreamIndex, intervalStart, intervalEnd);

        if (!File.Exists(_ffprobePath))
        {
            throw new FileNotFoundException("ffprobe.exe wurde nicht gefunden.", _ffprobePath);
        }

        var fullMediaFilePath = Path.GetFullPath(mediaFilePath);
        if (!File.Exists(fullMediaFilePath))
        {
            throw new FileNotFoundException(
                "Die zu analysierende Mediendatei wurde nicht gefunden.", fullMediaFilePath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffprobePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var argument in arguments.Take(arguments.Count - 1))
        {
            startInfo.ArgumentList.Add(argument);
        }

        startInfo.ArgumentList.Add(fullMediaFilePath);

        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException("ffprobe konnte nicht gestartet werden.");
        }

        using var cancellationRegistration = cancellationToken.Register(() =>
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
                // The process has already exited.
            }
        });

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        // Drain both pipes and reap the process even when cancellation kills it.
        await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
        var json = await outputTask.ConfigureAwait(false);
        var errorOutput = await errorTask.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Frameanalyse: ffprobe wurde mit Exit-Code {process.ExitCode} beendet." +
                Environment.NewLine + errorOutput.Trim());
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("ffprobe hat keine Frameanalyse geliefert.");
        }

        return FfprobeFrameJsonParser.Parse(json, videoStreamIndex);
    }
}
