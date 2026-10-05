using System.Text;
using System.Text.RegularExpressions;

namespace CutAssistantNext.App.Naming;

internal enum NameTemplateBlockKind
{
    Token,
    Literal
}

internal sealed record NameTemplateBlock(
    NameTemplateBlockKind Kind,
    string Value)
{
    public string DisplayText =>
        Value switch
        {
            "%Name%" =>
                "Name",

            "%Staffel:S%" =>
                "Staffel (S)",

            "%Folge:E%" =>
                "Folge (E)",

            "%Folgentitel: %" =>
                "Folgentitel (Leerzeichen)",

            "%Folgentitel: - %" =>
                "Folgentitel (-)",

            "%Tag%" =>
                "Tag",

            "%Monat%" =>
                "Monat",

            "%YY%" =>
                "Jahr (2-stellig)",

            "%YYYY%" =>
                "Jahr (4-stellig)",

            "%Stunde%" =>
                "Stunde",

            "%Minute%" =>
                "Minute",

            "%Sender%" =>
                "Sender",

            "%Serie%" =>
                "Serie",

            "%OriginalName%" =>
                "Originalname",

            " " =>
                "Leerzeichen",

            _ when
                Kind == NameTemplateBlockKind.Literal &&
                Value.Any(char.IsLetterOrDigit) =>
                    $"Text: {Value.Trim()}",

            _ =>
                Value
        };
}

internal static class NameTemplateBlockParser
{
    private static readonly Regex TokenRegex =
        new(
            "%[^%]+%",
            RegexOptions.Compiled);

    internal static IReadOnlyList<NameTemplateBlock> Parse(
        string template)
    {
        ArgumentNullException.ThrowIfNull(
            template);

        var blocks =
            new List<NameTemplateBlock>();

        var currentPosition = 0;

        foreach (Match match in TokenRegex.Matches(
                     template))
        {
            AddLiteralBlocks(
                blocks,
                template[
                    currentPosition..match.Index]);

            blocks.Add(
                new NameTemplateBlock(
                    NameTemplateBlockKind.Token,
                    match.Value));

            currentPosition =
                match.Index + match.Length;
        }

        AddLiteralBlocks(
            blocks,
            template[currentPosition..]);

        return blocks;
    }

    internal static string Render(
        IEnumerable<NameTemplateBlock> blocks)
    {
        ArgumentNullException.ThrowIfNull(
            blocks);

        var result =
            new StringBuilder();

        foreach (var block in blocks)
        {
            result.Append(
                block.Value);
        }

        return result.ToString();
    }

    private static void AddLiteralBlocks(
        ICollection<NameTemplateBlock> blocks,
        string literal)
    {
        if (literal.Length == 0)
        {
            return;
        }

        if (literal.Any(
                char.IsLetterOrDigit))
        {
            blocks.Add(
                new NameTemplateBlock(
                    NameTemplateBlockKind.Literal,
                    literal));

            return;
        }

        foreach (var character in literal)
        {
            blocks.Add(
                new NameTemplateBlock(
                    NameTemplateBlockKind.Literal,
                    character.ToString()));
        }
    }
}