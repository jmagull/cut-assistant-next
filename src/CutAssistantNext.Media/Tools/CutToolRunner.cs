using System.Diagnostics;
using System.Text;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Media.Tools;

public interface ICutToolRunner
{
    Task RunAsync(string executablePath, IReadOnlyList<string> arguments,
        IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken);
}

/// <summary>Runs a cut/index process without a shell and drains both output streams.</summary>
public sealed class CutToolRunner : ICutToolRunner
{
    public async Task RunAsync(string executablePath, IReadOnlyList<string> arguments,
        IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(executablePath))
            throw new FileNotFoundException("Die Programmdatei wurde nicht gefunden.", executablePath);
        var start = new ProcessStartInfo(executablePath)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = new Process { StartInfo = start };
        if (!process.Start()) throw new InvalidOperationException("Der Schnittprozess konnte nicht gestartet werden.");
        var errorTail = new StringBuilder();
        var output = DrainAsync(process.StandardOutput, progress, null);
        var error = DrainAsync(process.StandardError, progress, errorTail);
        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            await Task.WhenAll(output, error).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"{Path.GetFileName(executablePath)} ist fehlgeschlagen (Code {process.ExitCode}).\n{errorTail}");
        }
        finally
        {
            // Await exit and the inherited pipes before the caller removes job files.
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) when (process.HasExited) { }
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.WhenAll(output, error).ConfigureAwait(false);
        }
    }

    private static async Task DrainAsync(StreamReader stream, IProgress<CutProgressUpdate>? progress, StringBuilder? tail)
    {
        while (await stream.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            if (tail is not null)
            {
                tail.AppendLine(line);
                if (tail.Length > 16_384) tail.Remove(0, tail.Length - 16_384);
            }
            progress?.Report(new(CutProgressKind.Output, line));
        }
    }
}
