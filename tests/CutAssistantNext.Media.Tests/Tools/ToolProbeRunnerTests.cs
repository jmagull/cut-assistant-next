using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Tests.Tools;

public sealed class ToolProbeRunnerTests
{
    private static string PowerShell => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
        "WindowsPowerShell", "v1.0", "powershell.exe");

    [Fact]
    public async Task ReadsBothOutputStreamsAndPreservesFailureExitCode()
    {
        var result = await new ToolProbeRunner().RunAsync(PowerShell,
            ["-NoProfile", "-NonInteractive", "-Command", "[Console]::WriteLine('stdout'); [Console]::Error.WriteLine('stderr'); exit 7"]);
        Assert.Equal(7, result.ExitCode);
        Assert.Contains("stdout", result.StandardOutput);
        Assert.Contains("stderr", result.StandardError);
    }

    [Fact]
    public async Task ConcurrentReadersAvoidDeadlockOnLargeHelpOutput()
    {
        var result = await new ToolProbeRunner().RunAsync(PowerShell,
            ["-NoProfile", "-NonInteractive", "-Command", "for ($i=0; $i -lt 2000; $i++) { [Console]::WriteLine('output-' + $i); [Console]::Error.WriteLine('error-' + $i) }"]);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("output-1999", result.StandardOutput);
        Assert.Contains("error-1999", result.StandardError);
    }

    [Fact]
    public async Task HungQueryIsTerminatedWithTimeout()
    {
        var runner = new ToolProbeRunner(TimeSpan.FromMilliseconds(150));
        await Assert.ThrowsAsync<TimeoutException>(() => runner.RunAsync(PowerShell,
            ["-NoProfile", "-NonInteractive", "-Command", "Start-Sleep -Seconds 20"]));
    }

    [Fact]
    public async Task CancelledQueryIsTerminatedAndCallerCancellationIsPreserved()
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new ToolProbeRunner().RunAsync(PowerShell,
            ["-NoProfile", "-NonInteractive", "-Command", "Start-Sleep -Seconds 20"], cancellation.Token));
    }

    [Fact]
    public async Task MissingOrPreCancelledQueriesNeverStart()
    {
        var path = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.exe");
        await Assert.ThrowsAsync<FileNotFoundException>(() => new ToolProbeRunner().RunAsync(path, []));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new ToolProbeRunner().RunAsync(path, [], new CancellationToken(true)));
    }

    [Fact]
    public void RejectsInvalidTimeout()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ToolProbeRunner(TimeSpan.Zero));
    }
}
