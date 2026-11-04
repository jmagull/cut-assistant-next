using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>One explicit alternative attempt after the failed engine has finished cleanup.</summary>
internal sealed class OtrCanMp4BoxFallbackCutService(ICutEngine normal,
    Func<IProgress<CutProgressUpdate>?, CancellationToken, Task<ICutEngine>> alternative,
    Func<OtrCanAudioFailureException, CancellationToken, Task<bool>> confirm) : ICutEngine
{
    internal const string Notice = "MP4Box-Fallback: Nach dem otr-can-Tonfehler ausdrücklich gewählte alternative Schnittmethode. " +
        "Dies ist keine nachgewiesene Audioreparatur. Bitte Bild, Ton, Tonsynchronität und alle Schnittübergänge persönlich kontrollieren.";
    internal bool UsedMp4Box { get; private set; }

    public async Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default) =>
        _ = await RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false);

    public async Task<CutResult> RunWithResultAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        UsedMp4Box = false;
        try { return await normal.RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false); }
        catch (OtrCanAudioFailureException failure)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new(CutProgressKind.Output, failure.Message));
            progress?.Report(new(CutProgressKind.Status, "otr-can-Tonfehler – Entscheidung über MP4Box …"));
            if (!await confirm(failure, cancellationToken).ConfigureAwait(false)) throw;
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new(CutProgressKind.Output, "Ausdrücklich bestätigt: vollständiger Neustart mit MP4Box und unveränderten Behaltebereichen."));
            progress?.Report(new(CutProgressKind.Status, "MP4Box-Verfügbarkeit wird geprüft …"));
            var engine = await alternative(progress, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var result = await engine.RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false);
            UsedMp4Box = true;
            progress?.Report(new(CutProgressKind.Output, Notice));
            return new(result.OutputFilePath, result.Timeline, result.TimingKind,
                Notice + (string.IsNullOrWhiteSpace(result.Notice) ? "" : " " + result.Notice));
        }
    }
}
