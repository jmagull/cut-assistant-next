using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CutAssistantNext.Core.Naming;

public static class OtrFileNameParser
{
    private static readonly Regex OtrFileNamePattern =
        new(
            @"^(?<name>.+)_(?<year>\d{2})\.(?<month>\d{2})\.(?<day>\d{2})_(?<hour>\d{2})-(?<minute>\d{2})_(?<sender>[^_]+)_",
            RegexOptions.CultureInvariant);

    public static NameTemplateContext Parse(
        string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            fileName);

        if (TryParse(
                fileName,
                out var context))
        {
            return context;
        }

        var name =
            Path.GetFileName(fileName);

        throw new FormatException(
            $"Der OTR-Dateiname konnte nicht ausgewertet werden: {name}");
    }

    public static bool TryParse(
        string fileName,
        [NotNullWhen(true)] out NameTemplateContext? context)
    {
        context = null;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        var name =
            Path.GetFileName(fileName);

        var match =
            OtrFileNamePattern.Match(name);

        if (!match.Success)
        {
            return false;
        }

        var shortYear =
            match.Groups["year"].Value;

        context =
            new NameTemplateContext(
                Name: NormalizeName(
                    match.Groups["name"].Value),
                Year: $"20{shortYear}",
                Month: match.Groups["month"].Value,
                Day: match.Groups["day"].Value,
                OriginalName: name,
                ShortYear: shortYear,
                Hour: match.Groups["hour"].Value,
                Minute: match.Groups["minute"].Value,
                Sender: match.Groups["sender"].Value);

        return true;
    }

    private static string NormalizeName(
        string value)
    {
        return Regex.Replace(
            value.Replace('_', ' '),
            @"\s+",
            " ")
            .Trim();
    }
}
