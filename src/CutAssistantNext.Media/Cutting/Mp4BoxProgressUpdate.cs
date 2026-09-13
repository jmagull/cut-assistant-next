namespace CutAssistantNext.Media.Cutting;

public enum Mp4BoxProgressKind
{
    Status,
    Output
}

public sealed record Mp4BoxProgressUpdate(
    Mp4BoxProgressKind Kind,
    string Message);
