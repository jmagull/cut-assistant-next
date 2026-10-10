using System.Diagnostics;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Logging;

namespace CutAssistantNext.Media.Cutting;

public sealed class Mp4BoxRunner : IMp4BoxRunner
{
    private readonly string _executablePath;
    private readonly IAppLogger _logger;
    private readonly IProgress<CutProgressUpdate>? _progress;

    public Mp4BoxRunner(
        string executablePath,
        IAppLogger? logger = null,
        IProgress<CutProgressUpdate>? progress = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            executablePath);

        _executablePath =
            Path.GetFullPath(executablePath);

        _logger =
            logger ?? NullAppLogger.Instance;

        _progress =
            progress;
    }

    public async Task RunSplitAsync(
        string sourceFilePath,
        string outputFilePath,
        Mp4BoxSplitRange range,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            sourceFilePath);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            outputFilePath);

        ArgumentNullException.ThrowIfNull(range);

        if (!File.Exists(_executablePath))
        {
            throw new FileNotFoundException(
                "Die konfigurierte MP4Box-Anwendung wurde nicht gefunden.",
                _executablePath);
        }

        var fullSourceFilePath =
            Path.GetFullPath(sourceFilePath);

        if (!File.Exists(fullSourceFilePath))
        {
            throw new FileNotFoundException(
                "Die zu schneidende Mediendatei wurde nicht gefunden.",
                fullSourceFilePath);
        }

        var fullOutputFilePath =
            Path.GetFullPath(outputFilePath);

        if (string.Equals(
            fullSourceFilePath,
            fullOutputFilePath,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Quell- und Zieldatei dürfen nicht identisch sein.",
                nameof(outputFilePath));
        }

        var formattedRange =
            Mp4BoxSplitRangeFormatter.Format(
                range);

        var startInfo =
            new ProcessStartInfo
            {
                FileName = _executablePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

        startInfo.ArgumentList.Add(
            fullSourceFilePath);
        startInfo.ArgumentList.Add(
            "-splitx");
        startInfo.ArgumentList.Add(
            formattedRange);
        startInfo.ArgumentList.Add(
            "-out");
        startInfo.ArgumentList.Add(
            fullOutputFilePath);

        var commandLine =
            Mp4BoxCommandLineFormatter.Format(
                _executablePath,
                startInfo.ArgumentList);

        _progress?.Report(
            new CutProgressUpdate(
                CutProgressKind.Output,
                $"> {commandLine}"));

        _logger.Information(
            $"MP4Box-Aufruf: {commandLine}");

        _logger.Information(
            $"MP4Box -splitx wird gestartet: {formattedRange}");

        using var process =
            new Process
            {
                StartInfo = startInfo
            };

        if (!process.Start())
        {
            throw new InvalidOperationException(
                "MP4Box konnte nicht gestartet werden.");
        }

        using var cancellationRegistration =
            cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(
                            entireProcessTree: true);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Der Prozess wurde bereits beendet.
                }
            });

        var outputTask =
            Mp4BoxOutputReader.ReadAsync(
                process.StandardOutput,
                _progress,
                cancellationToken);

        var errorTask =
            Mp4BoxOutputReader.ReadAsync(
                process.StandardError,
                _progress,
                cancellationToken);

        await process.WaitForExitAsync(
            cancellationToken);

        var standardOutputLines =
            await outputTask;

        var errorOutputLines =
            await errorTask;

        var standardOutput =
            string.Join(
                Environment.NewLine,
                standardOutputLines);

        var errorOutput =
            string.Join(
                Environment.NewLine,
                errorOutputLines);

        if (process.ExitCode != 0)
        {
            var exception =
                new InvalidOperationException(
                    $"MP4Box wurde mit Exit-Code {process.ExitCode} beendet." +
                    Environment.NewLine +
                    errorOutput.Trim());

            _logger.Error(
                "MP4Box -splitx ist fehlgeschlagen.",
                exception);

            throw exception;
        }

        if (!File.Exists(fullOutputFilePath))
        {
            throw new InvalidOperationException(
                "MP4Box wurde erfolgreich beendet, hat aber keine Zieldatei erzeugt.");
        }

        if (!string.IsNullOrWhiteSpace(standardOutput))
        {
            _logger.Information(
                standardOutput.Trim());
        }

        _logger.Information(
            "MP4Box -splitx wurde erfolgreich abgeschlossen.");
    }

    public async Task RunConcatAsync(
        IReadOnlyList<string> segmentFilePaths,
        string outputFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            segmentFilePaths);

        if (segmentFilePaths.Count == 0)
        {
            throw new ArgumentException(
                "Mindestens eine Segmentdatei ist erforderlich.",
                nameof(segmentFilePaths));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            outputFilePath);

        if (!File.Exists(_executablePath))
        {
            throw new FileNotFoundException(
                "Die konfigurierte MP4Box-Anwendung wurde nicht gefunden.",
                _executablePath);
        }

        var fullOutputFilePath =
            Path.GetFullPath(outputFilePath);

        var fullSegmentFilePaths =
            new List<string>(
                segmentFilePaths.Count);

        foreach (var segmentFilePath in segmentFilePaths)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                segmentFilePath);

            var fullSegmentFilePath =
                Path.GetFullPath(segmentFilePath);

            if (!File.Exists(fullSegmentFilePath))
            {
                throw new FileNotFoundException(
                    "Die zusammenzufügende Segmentdatei wurde nicht gefunden.",
                    fullSegmentFilePath);
            }

            if (string.Equals(
                fullSegmentFilePath,
                fullOutputFilePath,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Zieldatei und Segmentdatei dürfen nicht identisch sein.",
                    nameof(outputFilePath));
            }

            fullSegmentFilePaths.Add(
                fullSegmentFilePath);
        }

        var startInfo =
            new ProcessStartInfo
            {
                FileName = _executablePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

        foreach (var segmentFilePath in fullSegmentFilePaths)
        {
            startInfo.ArgumentList.Add(
                "-cat");

            startInfo.ArgumentList.Add(
                segmentFilePath);
        }

        startInfo.ArgumentList.Add(
            fullOutputFilePath);

        var commandLine =
            Mp4BoxCommandLineFormatter.Format(
                _executablePath,
                startInfo.ArgumentList);

        _progress?.Report(
            new CutProgressUpdate(
                CutProgressKind.Output,
                $"> {commandLine}"));

        _logger.Information(
            $"MP4Box-Aufruf: {commandLine}");

        _logger.Information(
            $"MP4Box -cat wird gestartet: {fullSegmentFilePaths.Count} Segment(e).");

        using var process =
            new Process
            {
                StartInfo = startInfo
            };

        if (!process.Start())
        {
            throw new InvalidOperationException(
                "MP4Box konnte nicht gestartet werden.");
        }

        using var cancellationRegistration =
            cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(
                            entireProcessTree: true);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Der Prozess wurde bereits beendet.
                }
            });

        var outputTask =
            Mp4BoxOutputReader.ReadAsync(
                process.StandardOutput,
                _progress,
                cancellationToken);

        var errorTask =
            Mp4BoxOutputReader.ReadAsync(
                process.StandardError,
                _progress,
                cancellationToken);

        await process.WaitForExitAsync(
            cancellationToken);

        var standardOutputLines =
            await outputTask;

        var errorOutputLines =
            await errorTask;

        var standardOutput =
            string.Join(
                Environment.NewLine,
                standardOutputLines);

        var errorOutput =
            string.Join(
                Environment.NewLine,
                errorOutputLines);

        if (process.ExitCode != 0)
        {
            var exception =
                new InvalidOperationException(
                    $"MP4Box wurde mit Exit-Code {process.ExitCode} beendet." +
                    Environment.NewLine +
                    errorOutput.Trim());

            _logger.Error(
                "MP4Box -cat ist fehlgeschlagen.",
                exception);

            throw exception;
        }

        if (!File.Exists(fullOutputFilePath))
        {
            throw new InvalidOperationException(
                "MP4Box wurde erfolgreich beendet, hat aber keine Zieldatei erzeugt.");
        }

        if (!string.IsNullOrWhiteSpace(standardOutput))
        {
            _logger.Information(
                standardOutput.Trim());
        }

        if (!string.IsNullOrWhiteSpace(errorOutput))
        {
            _logger.Information(
                errorOutput.Trim());
        }

        _logger.Information(
            "MP4Box -cat wurde erfolgreich abgeschlossen.");
    }
}
