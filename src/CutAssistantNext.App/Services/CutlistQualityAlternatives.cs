using System.IO;

namespace CutAssistantNext.App.Services;

internal static class CutlistQualityAlternatives
{
    private static readonly (string Suffix, string Quality)[] Formats =
    [
        (".HQ.avi", "HQ"), (".HQ.mp4", "HQ"),
        (".HD.mp4", "HD"), (".HD.avi", "HD"), (".avi", "Standard")
    ];

    internal static IReadOnlyList<(string FileName, string Quality)> GetQueries(string fileName)
    {
        var format = Formats.FirstOrDefault(f => fileName.EndsWith(f.Suffix, StringComparison.OrdinalIgnoreCase));
        if (format.Suffix is null) return [];
        var stem = fileName[..^format.Suffix.Length];
        if (!stem.EndsWith("_TVOON_DE", StringComparison.OrdinalIgnoreCase)) return [];
        return Formats.Where(candidate => candidate.Quality != format.Quality)
            .Select(candidate => (stem + candidate.Suffix, candidate.Quality)).ToArray();
    }

    internal static bool Matches(CutlistSearchResult result, string fileName) =>
        !string.IsNullOrWhiteSpace(result.Id) && string.Equals(Path.GetFileName(result.CutlistFileName),
            fileName + ".cutlist", StringComparison.OrdinalIgnoreCase);
}
