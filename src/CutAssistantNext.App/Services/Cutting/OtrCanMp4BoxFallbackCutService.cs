using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>One explicit alternative attempt after the failed engine has finished cleanup.</summary>
internal sealed class OtrCanMp4BoxFallbackCutService : ICutEngine
{
    private readonly ICutEngine normal;
    private readonly Func<IProgress<CutProgressUpdate>?, CancellationToken, Task<ICutEngine>> alternative;
    private readonly Func<OtrCanAudioFailureException, CancellationToken, Task<bool>>? confirm;
    private readonly Func<AudioRecoveryOffer, CancellationToken, Task<AudioRecoveryChoice>>? choose;
    private readonly IAacGapRepair? repair;
    private readonly Func<CancellationToken, Task<(bool Available, string? Reason)>>? availability;
    internal bool UsedAacRepair { get; private set; }
    internal OtrCanMp4BoxFallbackCutService(ICutEngine normal, Func<IProgress<CutProgressUpdate>?, CancellationToken, Task<ICutEngine>> alternative, Func<OtrCanAudioFailureException, CancellationToken, Task<bool>> confirm)
    { this.normal = normal; this.alternative = alternative; this.confirm = confirm; }
    internal OtrCanMp4BoxFallbackCutService(ICutEngine normal, Func<IProgress<CutProgressUpdate>?, CancellationToken, Task<ICutEngine>> alternative, Func<AudioRecoveryOffer, CancellationToken, Task<AudioRecoveryChoice>> choose, IAacGapRepair repair, Func<CancellationToken, Task<(bool Available, string? Reason)>> availability)
    { this.normal = normal; this.alternative = alternative; this.choose = choose; this.repair = repair; this.availability = availability; }
    internal const string Notice = "MP4Box-Fallback: Nach dem otr-can-Tonfehler ausdrücklich gewählte alternative Schnittmethode. " +
        "Dies ist keine nachgewiesene Audioreparatur. Bitte Bild, Ton, Tonsynchronität und alle Schnittübergänge persönlich kontrollieren.";
    internal bool UsedMp4Box { get; private set; }

    public async Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default) =>
        _ = await RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false);

    public async Task<CutResult> RunWithResultAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        UsedMp4Box = false; UsedAacRepair = false;
        try { return await normal.RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false); }
        catch (OtrCanAudioFailureException failure)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new(CutProgressKind.Output, failure.Message));
            progress?.Report(new(CutProgressKind.Status, choose is null ? "otr-can-Tonfehler – Entscheidung über MP4Box …" : "otr-can-Tonfehler – verfügbare Möglichkeiten werden geprüft …"));
            if (choose is not null)
            {
                // Normal cleanup has ended before any preflight, decision or restart.
                await using var assessment = await repair!.AssessAsync(request, failure, progress, cancellationToken).ConfigureAwait(false);
                var mp4 = await availability!(cancellationToken).ConfigureAwait(false);
                var offer = new AudioRecoveryOffer(failure, assessment, mp4.Available, mp4.Reason);
                var choice = await choose(offer, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                if (choice == AudioRecoveryChoice.RepairAacGaps)
                {
                    if (!assessment.IsAvailable) throw new InvalidOperationException("Eine nicht nachgewiesene Reparatur wurde nicht angeboten.");
                    progress?.Report(new(CutProgressKind.Output, "Ausdrücklich bestätigt: vollständiger otr-can-Neustart mit experimenteller, positionsgenauer AAC-Lückenreparatur."));
                    try
                    {
                        var repaired = await assessment.RunAsync(request, progress, cancellationToken).ConfigureAwait(false);
                        UsedAacRepair = true;
                        return repaired;
                    }
                    catch (Exception error) when (error is not OperationCanceledException)
                    {
                        progress?.Report(new(CutProgressKind.Output, "AAC-Reparatur fehlgeschlagen; keine erfolgreiche Ausgabe: " + error));
                        var unavailable = new AacGapRepairOffer("Der bestätigte Reparaturversuch ist fehlgeschlagen. Kein weiterer Reparaturversuch für diesen Auftrag.");
                        choice = await choose(offer with { Repair = unavailable, RepairFailure = error.Message }, cancellationToken).ConfigureAwait(false);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                if (choice == AudioRecoveryChoice.Cancel) throw new OperationCanceledException("Schnitt durch Benutzerentscheidung abgebrochen; kein weiterer Motor gestartet.", failure, cancellationToken);
                if (choice != AudioRecoveryChoice.Mp4Box || !mp4.Available) throw new InvalidOperationException("Diese weitere Verarbeitung ist für den Auftrag nicht verfügbar.");
            }
            else if (!await confirm!(failure, cancellationToken).ConfigureAwait(false)) throw;
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
