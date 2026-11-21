namespace CutAssistantNext.App.ViewModels;

internal static class CutFailureDiagnosis
{
    internal static string? Explain(string? technical)
    {
        if (string.IsNullOrWhiteSpace(technical)) return null;
        if (Contains("No space left on device", "ENOSPC", "ERROR_DISK_FULL", "0x80070070", "0x80070027",
            "not enough space on the disk", "disk is full", "Datenträger ist voll", "nicht genügend Speicherplatz"))
            return "Speicherplatzproblem gemeldet. Bitte Arbeits- und Ziellaufwerk anhand der protokollierten Messwerte prüfen. Die Meldung kann auch eine Speicherquote betreffen.";
        if (Contains("write error", "error writing", "Schreibfehler", "failed to write"))
            return "Schreibfehler gemeldet. Die Ursache ist damit noch nicht nachgewiesen. Bitte freien Speicherplatz, Schreibrechte und Datenträger anhand des vollständigen Protokolls prüfen.";
        return null;

        bool Contains(params string[] patterns) => patterns.Any(pattern => technical.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}
