using System.Diagnostics;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Media.Analysis;

public sealed class FfprobeRunner : IMediaAnalysisRunner
{
    private readonly string _ffprobePath;

    public FfprobeRunner(string ffprobePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            ffprobePath);

        _ffprobePath =
            ffprobePath;
    }

    public async Task<MediaAnalysisResult> RunAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);

        if (!File.Exists(_ffprobePath))
        {
            throw new FileNotFoundException(
                "ffprobe.exe wurde nicht gefunden.",
                _ffprobePath);
        }

        var fullMediaFilePath = Path.GetFullPath(mediaFilePath);

        if (!File.Exists(fullMediaFilePath))
        {
            throw new FileNotFoundException(
                "Die zu analysierende Mediendatei wurde nicht gefunden.",
                fullMediaFilePath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffprobePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("-v");
        startInfo.ArgumentList.Add("error");
        startInfo.ArgumentList.Add("-show_streams");
        startInfo.ArgumentList.Add("-show_format");
        startInfo.ArgumentList.Add("-of");
        startInfo.ArgumentList.Add("json");
        startInfo.ArgumentList.Add(fullMediaFilePath);

        using var process = new Process
        {
            StartInfo = startInfo
        };

        if (!process.Start())
        {
            throw new InvalidOperationException(
                "ffprobe konnte nicht gestartet werden.");
        }

        using var cancellationRegistration =
            cancellationToken.Register(() =>
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
                    // Der Prozess wurde bereits beendet.
                }
            });

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        var json = await outputTask;
        var errorOutput = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"ffprobe wurde mit Exit-Code {process.ExitCode} beendet." +
                Environment.NewLine +
                errorOutput.Trim());
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException(
                "ffprobe hat keine JSON-Ausgabe geliefert.");
        }

        return FfprobeJsonParser.Parse(json);
    }
}
