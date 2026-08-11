namespace CutAssistantNext.Core.Naming;

public sealed record NameTemplateContext(
    string? Name = null,
    string? Year = null,
    string? Month = null,
    string? Day = null,
    string? Season = null,
    string? Episode = null,
    string? OriginalName = null,
    string? ShortYear = null,
    string? Hour = null,
    string? Minute = null,
    string? Sender = null,
    string? Series = null,
    string? EpisodeTitle = null);
