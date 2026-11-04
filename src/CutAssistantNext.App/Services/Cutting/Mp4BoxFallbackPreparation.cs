using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>Uses the existing MP4 preparation for a confirmed fallback from another container.</summary>
internal sealed class Mp4BoxFallbackPreparation(ICutEngine engine,
    Func<string, CancellationToken, Task<MediaAnalysisResult>> analyze,
    Func<string, string, MediaAnalysisResult, IProgress<CutProgressUpdate>?, CancellationToken, Task> prepare) : ICutEngine
{
    public async Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default) =>
        _ = await RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false);

    public async Task<CutResult> RunWithResultAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var analysis = await analyze(request.SourceFilePath, cancellationToken).ConfigureAwait(false);
        if (VideoPreparation.IsMp4(analysis))
            return await engine.RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false);
        var parent = Path.GetDirectoryName(request.OutputFilePath)!;
        var name = ".can-mp4box-fallback-" + Guid.NewGuid().ToString("N");
        var work = Path.Combine(parent, name);
        Directory.CreateDirectory(work);
        try
        {
            var source = Path.Combine(work, "source.mp4");
            progress?.Report(new(CutProgressKind.Status, "Bestätigter MP4Box-Neustart: MP4-Arbeitskopie vorbereiten …"));
            await prepare(request.SourceFilePath, source, analysis, progress, cancellationToken).ConfigureAwait(false);
            return await engine.RunWithResultAsync(request.WithSourceFilePath(source), progress, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            try
            {
                var resolved = Path.GetFullPath(work);
                if (Path.GetDirectoryName(resolved) != parent || Path.GetFileName(resolved) != name ||
                    new DirectoryInfo(resolved).LinkTarget is not null)
                    throw new IOException("Der MP4Box-Arbeitsbereich hat sich verändert.");
                Directory.Delete(resolved, recursive: true);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                progress?.Report(new(CutProgressKind.Output, "MP4Box-Arbeitskopie konnte nicht vollständig gelöscht werden: " + error.Message));
            }
        }
    }
}
