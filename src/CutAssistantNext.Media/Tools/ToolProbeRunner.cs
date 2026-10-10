using System.ComponentModel;
using System.Diagnostics;

namespace CutAssistantNext.Media.Tools;

public sealed record ToolProbeResult(int ExitCode, string StandardOutput, string StandardError);

public interface IToolProbeRunner
{
    Task<ToolProbeResult> RunAsync(string executablePath, IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default);
}

/// <summary>Runs a short help/version query without a shell, with bounded time and process cleanup.</summary>
public sealed class ToolProbeRunner : IToolProbeRunner
{
    private readonly TimeSpan _timeout;

    public ToolProbeRunner(TimeSpan? timeout = null)
    {
        _timeout = timeout ?? TimeSpan.FromSeconds(10);
        if (_timeout <= TimeSpan.Zero || _timeout.TotalMilliseconds > uint.MaxValue - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }
    }

    public async Task<ToolProbeResult> RunAsync(string executablePath, IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        ArgumentNullException.ThrowIfNull(arguments);
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException("Die Programmdatei wurde nicht gefunden.", executablePath);
        }

        var start = new ProcessStartInfo(executablePath)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = start };
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(_timeout);
        if (!process.Start())
        {
            throw new InvalidOperationException("Die Werkzeugprüfung konnte nicht gestartet werden.");
        }

        var output = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var error = process.StandardError.ReadToEndAsync(timeout.Token);
        try
        {
            await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
            return new ToolProbeResult(process.ExitCode,
                await output.ConfigureAwait(false), await error.ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            Stop(process);
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            try { await Task.WhenAll(output, error).ConfigureAwait(false); }
            catch (OperationCanceledException) { }
            cancellationToken.ThrowIfCancellationRequested();
            throw new TimeoutException("Das Werkzeug hat nicht rechtzeitig auf die Prüfung geantwortet.");
        }
        finally
        {
            Stop(process);
        }
    }

    private static void Stop(Process process)
    {
        try
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException) { }
        catch (Win32Exception) when (process.HasExited) { }
    }
}
