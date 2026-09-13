namespace CutAssistantNext.App.Services.Cutting;

internal static class OutputFileNameBuilder
{
    private static readonly HashSet<string> ReservedWindowsNames =
        new(
            new[]
            {
                "CON",
                "PRN",
                "AUX",
                "NUL",
                "COM1",
                "COM2",
                "COM3",
                "COM4",
                "COM5",
                "COM6",
                "COM7",
                "COM8",
                "COM9",
                "LPT1",
                "LPT2",
                "LPT3",
                "LPT4",
                "LPT5",
                "LPT6",
                "LPT7",
                "LPT8",
                "LPT9"
            },
            StringComparer.OrdinalIgnoreCase);

    public static string Build(
        string suggestedMovieName,
        string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            suggestedMovieName);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            extension);

        var sanitizedName =
            new string(
                suggestedMovieName
                    .Trim()
                    .Select(
                        character =>
                            IsInvalidWindowsFileNameCharacter(character)
                                ? '_'
                                : character)
                    .ToArray())
            .TrimEnd(
                ' ',
                '.');

        if (string.IsNullOrWhiteSpace(sanitizedName))
        {
            throw new ArgumentException(
                "Der vorgeschlagene Dateiname ergibt keinen gültigen Windows-Dateinamen.",
                nameof(suggestedMovieName));
        }

        var firstNamePart =
            sanitizedName.Split(
                '.',
                2)[0];

        if (ReservedWindowsNames.Contains(firstNamePart))
        {
            sanitizedName =
                "_" + sanitizedName;
        }

        var normalizedExtension =
            extension.Trim();

        if (!normalizedExtension.StartsWith('.'))
        {
            normalizedExtension =
                "." + normalizedExtension;
        }

        if (normalizedExtension.Length == 1 ||
            normalizedExtension.Any(
                IsInvalidWindowsFileNameCharacter))
        {
            throw new ArgumentException(
                "Die Dateiendung ist ungültig.",
                nameof(extension));
        }

        if (sanitizedName.EndsWith(
            normalizedExtension,
            StringComparison.OrdinalIgnoreCase))
        {
            return sanitizedName;
        }

        return sanitizedName + normalizedExtension;
    }

    private static bool IsInvalidWindowsFileNameCharacter(
        char character)
    {
        return character < ' ' ||
            character is
                '<' or
                '>' or
                ':' or
                '"' or
                '/' or
                '\\' or
                '|' or
                '?' or
                '*';
    }
}
