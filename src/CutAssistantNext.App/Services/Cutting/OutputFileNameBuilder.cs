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

    /// <summary>Validates the chosen movie name without silently changing its spelling.</summary>
    public static string BuildExactMovieFileName(string movieName)
    {
        if (string.IsNullOrWhiteSpace(movieName))
        {
            throw new ArgumentException("Bitte einen Ausgabedateinamen eingeben.", nameof(movieName));
        }

        if (movieName.Any(IsInvalidWindowsFileNameCharacter))
        {
            throw new ArgumentException(
                "Der Ausgabedateiname enthält ein unzulässiges Zeichen. Nicht erlaubt sind " +
                "< > : \" / \\ | ? * sowie Steuerzeichen. Bitte nur den Namen ohne Ordnerpfad eingeben.",
                nameof(movieName));
        }

        if (movieName.EndsWith(' ') || movieName.EndsWith('.'))
        {
            throw new ArgumentException(
                "Der Ausgabedateiname darf nicht mit einem Leerzeichen oder Punkt enden.",
                nameof(movieName));
        }

        const string extension = ".mp4";
        var baseName = movieName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
            ? movieName[..^extension.Length]
            : movieName;
        if (string.IsNullOrWhiteSpace(baseName))
        {
            throw new ArgumentException("Bitte einen Namen vor der Endung .mp4 eingeben.", nameof(movieName));
        }

        var firstNamePart = baseName.Split('.', 2)[0].TrimEnd(' ');
        if (ReservedWindowsNames.Contains(firstNamePart) ||
            firstNamePart.Equals("CONIN$", StringComparison.OrdinalIgnoreCase) ||
            firstNamePart.Equals("CONOUT$", StringComparison.OrdinalIgnoreCase) ||
            (firstNamePart.Length == 4 &&
             (firstNamePart.StartsWith("COM", StringComparison.OrdinalIgnoreCase) ||
              firstNamePart.StartsWith("LPT", StringComparison.OrdinalIgnoreCase)) &&
             firstNamePart[3] is '¹' or '²' or '³'))
        {
            throw new ArgumentException(
                "Dieser Ausgabedateiname ist unter Windows reserviert. Bitte einen anderen Namen wählen.",
                nameof(movieName));
        }

        var fileName = movieName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
            ? movieName
            : movieName + extension;
        if (fileName.Length > 255)
        {
            throw new ArgumentException(
                "Der Ausgabedateiname ist zu lang. Bitte auf höchstens 255 Zeichen einschließlich .mp4 kürzen.",
                nameof(movieName));
        }

        return fileName;
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
