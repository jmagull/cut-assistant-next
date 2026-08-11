using System.Text.RegularExpressions;

namespace CutAssistantNext.Core.Naming;

public static class NameTemplateRenderer
{
    public static string Render(
        string template,
        NameTemplateContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);
        ArgumentNullException.ThrowIfNull(context);

        var result = ReplacePrefixedTokens(
            template,
            context);

        result = ReplaceToken(result, "%OriginalName%", context.OriginalName);
        result = ReplaceToken(result, "%Name%", context.Name);
        result = ReplaceToken(result, "%YYYY%", context.Year);
        result = ReplaceToken(result, "%YY%", context.ShortYear);
        result = ReplaceToken(result, "%Monat%", context.Month);
        result = ReplaceToken(result, "%Tag%", context.Day);
        result = ReplaceToken(result, "%Stunde%", context.Hour);
        result = ReplaceToken(result, "%Minute%", context.Minute);
        result = ReplaceToken(result, "%Sender%", context.Sender);
        result = ReplaceToken(result, "%Serie%", context.Series);
        result = ReplaceToken(result, "%Staffel%", context.Season);
        result = ReplaceToken(result, "%Folge%", context.Episode);
        result = ReplaceToken(result, "%Folgentitel%", context.EpisodeTitle);

        var unknownToken = Regex.Match(
            result,
            "%[^%]+%");

        if (unknownToken.Success)
        {
            throw new ArgumentException(
                $"Unbekannte Variable in der Namensmaske: {unknownToken.Value}",
                nameof(template));
        }

        return Regex.Replace(result.Trim(), "[ \t]+", " ");
    }

    private static string ReplacePrefixedTokens(
        string template,
        NameTemplateContext context)
    {
        return Regex.Replace(
            template,
            "%(?<name>[^:%]+):(?<prefix>[^%]*)%",
            match =>
            {
                var tokenName =
                    match.Groups["name"].Value;

                var prefix =
                    match.Groups["prefix"].Value;

                var value = tokenName switch
                {
                    "OriginalName" => context.OriginalName,
                    "Name" => context.Name,
                    "YYYY" => context.Year,
                    "YY" => context.ShortYear,
                    "Monat" => context.Month,
                    "Tag" => context.Day,
                    "Stunde" => context.Hour,
                    "Minute" => context.Minute,
                    "Sender" => context.Sender,
                    "Serie" => context.Series,
                    "Staffel" => context.Season,
                    "Folge" => context.Episode,
                    "Folgentitel" => context.EpisodeTitle,
                    _ => throw new ArgumentException(
                        $"Unbekannte Variable in der Namensmaske: %{tokenName}%",
                        nameof(template))
                };

                return string.IsNullOrEmpty(value)
                    ? string.Empty
                    : prefix + value;
            });
    }

    private static string ReplaceToken(
        string template,
        string token,
        string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return template.Replace(
                token,
                value);
        }

        return template
            .Replace(
                $" {token}",
                string.Empty)
            .Replace(
                $"{token} ",
                string.Empty)
            .Replace(
                token,
                string.Empty);
    }
}
