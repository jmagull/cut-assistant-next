namespace CutAssistantNext.Core.Cutting;

public enum CutProgressKind
{
    Status,
    Output,
    Progress
}

public sealed record CutProgressUpdate(
    CutProgressKind Kind,
    string Message)
{
    /// <summary>Measured completion of the named phase, never an estimated overall percentage.</summary>
    public double? Percentage { get; init; }
}
