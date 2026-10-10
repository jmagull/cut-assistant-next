using System.Collections.Concurrent;
using System.Diagnostics;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.Media.Tests.Tools;

public sealed class CutToolRunnerTests
{
    private static string PowerShell => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
        "WindowsPowerShell", "v1.0", "powershell.exe");
    private static string[] Args(string command) => ["-NoProfile", "-NonInteractive", "-Command", command];
    private sealed class Sink : IProgress<CutProgressUpdate>
    {
        public ConcurrentQueue<string> Lines { get; } = new();
        public void Report(CutProgressUpdate update) => Lines.Enqueue(update.Message);
    }

    [Fact]
    public async Task StreamsBothPipesWithoutDeadlockAndReportsExitFailure()
    {
        var sink = new Sink();
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => new CutToolRunner().RunAsync(PowerShell,
            Args("for($i=0;$i -lt 2000;$i++){[Console]::WriteLine('out-'+$i);[Console]::Error.WriteLine('err-'+$i)};exit 7"),
            sink, CancellationToken.None));
        Assert.Contains("Code 7", error.Message);
        Assert.Contains("err-1999", error.Message);
        Assert.Contains("out-1999", sink.Lines);
        Assert.Contains("err-1999", sink.Lines);
    }

    [Fact]
    public async Task ArgumentListPreservesSpacesQuotesAndUnicode()
    {
        var sink = new Sink();
        var text = "Pfad mit Leerzeichen und ä 'Zitat'";
        var script = Path.Combine(Path.GetTempPath(), $"can argument {Guid.NewGuid():N}.ps1");
        try
        {
            File.WriteAllText(script, "param([string]$Value)\n[Console]::OutputEncoding=[Text.Encoding]::UTF8;[Console]::WriteLine($Value)");
            await new CutToolRunner().RunAsync(PowerShell,
                ["-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-File", script, "-Value", text], sink, CancellationToken.None);
            Assert.Contains(text, sink.Lines);
        }
        finally { File.Delete(script); }
    }

    [Fact]
    public async Task CancellationTerminatesChildProcessBeforeReturning()
    {
        var root = Path.Combine(Path.GetTempPath(), "can-process-tree-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var marker = Path.Combine(root, "child.pid");
        using var cancellation = new CancellationTokenSource();
        Process? child = null;
        try
        {
            var script = "$p=Start-Process -FilePath '" + PowerShell.Replace("'", "''") +
                "' -ArgumentList '-NoProfile -NonInteractive -Command Start-Sleep -Seconds 30' -WindowStyle Hidden -PassThru;" +
                "[IO.File]::WriteAllText('" + marker.Replace("'", "''") + "',$p.Id);$p.WaitForExit()";
            var run = new CutToolRunner().RunAsync(PowerShell, Args(script), null, cancellation.Token);
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (!File.Exists(marker)) await Task.Delay(30, deadline.Token);
            child = Process.GetProcessById(int.Parse(File.ReadAllText(marker)));
            cancellation.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => run);
            await child.WaitForExitAsync(deadline.Token);
            Assert.True(child.HasExited);
        }
        finally
        {
            cancellation.Cancel();
            if (child is not null)
            {
                if (!child.HasExited) child.Kill(entireProcessTree: true);
                child.Dispose();
            }
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public async Task MissingOrPreCancelledToolDoesNotStart()
    {
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe");
        var runner = new CutToolRunner();
        await Assert.ThrowsAsync<FileNotFoundException>(() => runner.RunAsync(missing, [], null, CancellationToken.None));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runner.RunAsync(missing, [], null, new(true)));
    }
}
