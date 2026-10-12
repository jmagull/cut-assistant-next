using CutAssistantNext.Core.Naming;

namespace CutAssistantNext.App.Naming;

internal static class NamingEditorSample
{
    internal const string OriginalFileName =
        "Ghostbusters__Hexenprobleme_26.09.19_10-50_pro7maxx_25_TVOON_DE.HQ.mp4";

    internal static NameTemplateContext CreateContext() =>
        NameTemplateContextFactory.Create(OriginalFileName) with
        {
            Name = "Ghostbusters",
            Series = "Ghostbusters",
            Season = "01",
            Episode = "01",
            EpisodeTitle = "Hexenprobleme"
        };
}
