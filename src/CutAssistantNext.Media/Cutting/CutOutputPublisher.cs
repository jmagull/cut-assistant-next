using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Cutting;

/// <summary>Copy into a private target sibling, then rename on the target volume.</summary>
public static class CutOutputPublisher
{
    public static async Task PublishAsync(string pending, string output, bool overwrite,
        IProgress<CutProgressUpdate>? progress, CancellationToken token, CutOutputIdentity? expectedTarget = null)
    {
        var staging = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(output))!, ".can-publish-" + Guid.NewGuid().ToString("N") + ".mp4");
        var owned = false;
        var modified = File.GetLastWriteTimeUtc(pending);
        try
        {
            token.ThrowIfCancellationRequested();
            if (expectedTarget is null && overwrite && File.Exists(output))
                expectedTarget = await CutOutputIdentityVerifier.ReadAsync(output, token).ConfigureAwait(false);
            progress?.Report(new(CutProgressKind.Status, "Geprüfte Ausgabe wird sicher ins Zielverzeichnis übernommen …"));
            progress?.Report(new(CutProgressKind.Output, $"Dateiübernahme: {pending}\nZiel: {output}\nTemporäre Zielkopie: {staging}"));
            await using (var input = new FileStream(pending, FileMode.Open, FileAccess.Read, FileShare.Read, 1048576, FileOptions.Asynchronous | FileOptions.SequentialScan))
            await using (var target = new FileStream(staging, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1048576, FileOptions.Asynchronous))
            {
                owned = true;
                var buffer = new byte[1048576];
                long copied = 0;
                int read;
                while ((read = await input.ReadAsync(buffer, token).ConfigureAwait(false)) != 0)
                {
                    await target.WriteAsync(buffer.AsMemory(0, read), token).ConfigureAwait(false);
                    copied += read;
                    progress?.Report(new(CutProgressKind.Progress, "Geprüfte Ausgabe ins Zielverzeichnis kopieren")
                    { Percentage = input.Length == 0 ? 100 : 100d * copied / input.Length });
                }
                await target.FlushAsync(token).ConfigureAwait(false);
                target.Flush(flushToDisk: true);
                if (input.Length != target.Length) throw new IOException("Die Zielkopie ist unvollständig.");
            }
            token.ThrowIfCancellationRequested();
            File.SetLastWriteTimeUtc(staging, modified);
            if (expectedTarget is not null)
                await CutOutputIdentityVerifier.ValidateAsync(output, expectedTarget, fullHash: true, token).ConfigureAwait(false);
            File.Move(staging, output, overwrite);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            progress?.Report(new(CutProgressKind.Output, $"Festgestellter Fehler bei der Dateiübernahme: {pending} -> {output}\n{error}"));
            throw;
        }
        finally
        {
            if (owned)
            {
                try { File.Delete(staging); }
                catch (Exception error) when (error is IOException or UnauthorizedAccessException)
                { progress?.Report(new(CutProgressKind.Output, $"Bereinigungsfehler der Zielkopie: {staging}\n{error}")); }
            }
        }
    }
}
