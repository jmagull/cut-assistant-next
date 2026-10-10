namespace CutAssistantNext.Core.Cutting;

public enum CutProgressKind
{
    Status,
    Output
}

public sealed record CutProgressUpdate(
    CutProgressKind Kind,
    string Message);
