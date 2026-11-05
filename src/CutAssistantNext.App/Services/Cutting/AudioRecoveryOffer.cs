using CutAssistantNext.Core.Cutting;
namespace CutAssistantNext.App.Services.Cutting;

internal enum AudioRecoveryChoice { Cancel, Mp4Box, RepairAacGaps }
internal interface IAacGapRepair
{
    Task<AacGapRepairOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure, IProgress<CutProgressUpdate>? progress, CancellationToken token);
}
/// <summary>Owns immutable preflight inputs until the decision and at most one restart.</summary>
internal sealed class AacGapRepairOffer(string description,
 Func<CutRequest, IProgress<CutProgressUpdate>?, CancellationToken, Task<CutResult>>? run = null,
 Func<ValueTask>? dispose = null) : IAsyncDisposable
{
    private int _started;
    internal bool IsAvailable => run is not null;
    internal string Description => description;
    internal Task<CutResult> RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (run is null || Interlocked.Exchange(ref _started, 1) != 0) throw new InvalidOperationException("Für diesen Auftrag ist kein weiterer Reparaturversuch freigegeben.");
        return run(request, progress, token);
    }
    public ValueTask DisposeAsync() => dispose?.Invoke() ?? ValueTask.CompletedTask;
}
internal sealed record AudioRecoveryOffer(OtrCanAudioFailureException Failure, AacGapRepairOffer Repair,
 bool Mp4BoxAvailable = true, string? Mp4BoxReason = null, string? RepairFailure = null)
{
    internal string Diagnosis => RepairFailure is null ? Failure.Message :
     "Auch die experimentelle AAC-Reparatur wurde sicher abgebrochen. Es wurde keine geprüfte Ausgabe übernommen. " + RepairFailure;
}
