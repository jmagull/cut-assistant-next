using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services;

internal sealed record CutlistRatingResult(CutlistRatingState State, string Message)
{
    internal bool Accepted => State == CutlistRatingState.Accepted;
}

internal sealed class CutlistRatingService(ICutlistRatingStore store,
    Func<CutlistRatingSource, int, CancellationToken, Task<CutlistRatingResult>> send)
{
    private int _sending;

    internal async Task<CutlistRatingResult> SubmitAsync(CutlistRatingSource source, int rating, CancellationToken token = default)
    {
        if (rating is < 0 or > 5) throw new ArgumentOutOfRangeException(nameof(rating));
        if (Interlocked.CompareExchange(ref _sending, 1, 0) != 0)
            return new(CutlistRatingState.Unknown, "Eine Bewertung wird bereits übermittelt.");
        try
        {
            if (token.IsCancellationRequested) return new(CutlistRatingState.Rejected, "Bewertung abgebrochen; nichts gesendet.");
            try
            {
                if (!await Task.Run(() => store.TryBegin(source.Key)))
                    return new(CutlistRatingState.Unknown, "Für diese Cutlist wurde bereits eine Bewertung versucht. Kein erneuter Versand.");
            }
            catch
            {
                return new(CutlistRatingState.Rejected, "Der Bewertungsstatus konnte nicht sicher gespeichert werden. Es wurde nichts gesendet.");
            }

            CutlistRatingResult result;
            try { result = await send(source, rating, token); }
            catch
            {
                // Never expose HTTP exceptions: they may contain credentials or echoed request data.
                result = new(CutlistRatingState.Unknown, "Übertragungsergebnis unklar. Keine automatische Wiederholung.");
            }
            try { await Task.Run(() => store.Complete(source.Key, result.State)); }
            catch
            {
                result = result with
                {
                    Message = result.Message +
                    "\nDer Abschlussstatus konnte nicht gespeichert werden. Der gespeicherte Versandversuch verhindert erneutes Senden."
                };
            }
            return result;
        }
        finally { Volatile.Write(ref _sending, 0); }
    }
}

internal enum CutlistRatingDecision { Rate, Skip, Cancel }

internal sealed class CutlistRatingWorkflow(ICutlistRatingStore store)
{
    internal UsedCutlistRatingContext? Current { get; private set; }

    // Called only after the existing cut-plan import has completed successfully.
    internal void Use(CutlistRatingSource? source, string mediaPath, string ownAuthor, string? documentAuthor)
    {
        Current = source is not null && !string.IsNullOrWhiteSpace(source.Author) &&
            !SameAuthor(source.Author, ownAuthor) && !SameAuthor(documentAuthor, ownAuthor)
            ? new(source, mediaPath) : null;
    }

    private static bool SameAuthor(string? a, string b) => !string.IsNullOrWhiteSpace(b) &&
        string.Equals(a?.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);

    internal void Clear() => Current = null;

    internal async Task<bool> CanLeaveAsync(
        Func<UsedCutlistRatingContext, Task<CutlistRatingDecision>> ask,
        Func<UsedCutlistRatingContext, Task<bool>> rate,
        CutlistRatingSource? next = null)
    {
        var current = Current;
        if (current is null || current.Source.Key == next?.Key) return true;
        if (await Task.Run(() => store.Get(current.Source.Key)) is not null) return true;
        return (await ask(current)) switch
        {
            CutlistRatingDecision.Skip => true,
            CutlistRatingDecision.Rate => await rate(current),
            _ => false
        };
    }
}
