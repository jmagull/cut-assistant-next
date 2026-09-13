using System.Globalization;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Serialization;

public static class CutlistParser
{
    public static CutlistDocument Parse(
        string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            content);

        var sections =
            ParseSections(
                content);

        var generalSection =
            GetRequiredSection(
                sections,
                "General");

        var numberOfCuts =
            ParseInt32(
                GetRequiredValue(
                    generalSection,
                    "NoOfCuts"),
                "NoOfCuts");

        if (numberOfCuts < 0)
        {
            throw new FormatException(
                "NoOfCuts darf nicht negativ sein.");
        }

        var cuts =
            new List<CutlistKeepSegment>(
                numberOfCuts);

        for (var index = 0;
             index < numberOfCuts;
             index++)
        {
            var cutSection =
                GetRequiredSection(
                    sections,
                    $"Cut{index}");

            var start =
                ParseDouble(
                    GetRequiredValue(
                        cutSection,
                        "Start"),
                    $"Cut{index}.Start");

            var duration =
                ParseDouble(
                    GetRequiredValue(
                        cutSection,
                        "Duration"),
                    $"Cut{index}.Duration");

            // Some legacy cutlists contain an empty end marker, not a keep range.
            if (duration == 0 && double.IsFinite(start) && start >= 0)
            {
                var frameDuration = GetValue(cutSection, "DurationFrames");
                if (!string.IsNullOrWhiteSpace(frameDuration) &&
                    ParseDouble(frameDuration, $"Cut{index}.DurationFrames") != 0)
                    throw new FormatException($"Cut{index}: Duration und DurationFrames widersprechen sich.");
                continue;
            }

            cuts.Add(
                new CutlistKeepSegment(
                    TimeSpan.FromSeconds(
                        start),
                    TimeSpan.FromSeconds(
                        duration)));
        }

        if (numberOfCuts > 0 && cuts.Count == 0)
            throw new FormatException("Die Cutlist enthält ausschließlich leere Bereiche und keinen nutzbaren Schnitt.");

        var general =
            new CutlistGeneralMetadata
            {
                Application =
                    GetValue(
                        generalSection,
                        "Application"),

                Version =
                    GetValue(
                        generalSection,
                        "Version"),

                FramesPerSecond =
                    ParseNullableDouble(
                        GetValue(
                            generalSection,
                            "FramesPerSecond"),
                        "FramesPerSecond"),

                DisplayAspectRatio =
                    GetNullableValue(
                        generalSection,
                        "DisplayAspectRatio"),

                IntendedCutApplicationName =
                    GetValue(
                        generalSection,
                        "IntendedCutApplicationName"),

                IntendedCutApplication =
                    GetValue(
                        generalSection,
                        "IntendedCutApplication"),

                IntendedCutApplicationVersion =
                    GetValue(
                        generalSection,
                        "IntendedCutApplicationVersion"),

                IntendedCutApplicationOptions =
                    GetValue(
                        generalSection,
                        "IntendedCutApplicationOptions"),

                NoOfCuts =
                    cuts.Count,

                ApplyToFile =
                    GetValue(
                        generalSection,
                        "ApplyToFile"),

                OriginalFileSizeBytes =
                    ParseNullableInt64(
                        GetValue(
                            generalSection,
                            "OriginalFileSizeBytes"),
                        "OriginalFileSizeBytes")
            };

        var infoSection =
            GetRequiredSection(
                sections,
                "Info");

        var info =
            new CutlistInfoMetadata
            {
                RatingByAuthor =
                    ParseInt32(
                        GetRequiredValue(
                            infoSection,
                            "RatingByAuthor"),
                        "RatingByAuthor"),

                Author =
                    GetNullableValue(
                        infoSection,
                        "Author"),

                UserComment =
                    GetNullableValue(
                        infoSection,
                        "UserComment"),

                EpgError =
                    ParseBoolean(
                        GetValue(
                            infoSection,
                            "EPGError"),
                        "EPGError"),

                ActualContent =
                    GetNullableValue(
                        infoSection,
                        "ActualContent"),

                MissingBeginning =
                    ParseBoolean(
                        GetValue(
                            infoSection,
                            "MissingBeginning"),
                        "MissingBeginning"),

                MissingEnding =
                    ParseBoolean(
                        GetValue(
                            infoSection,
                            "MissingEnding"),
                        "MissingEnding"),

                MissingVideo =
                    ParseBoolean(
                        GetValue(
                            infoSection,
                            "MissingVideo"),
                        "MissingVideo"),

                MissingAudio =
                    ParseBoolean(
                        GetValue(
                            infoSection,
                            "MissingAudio"),
                        "MissingAudio"),

                OtherError =
                    ParseBoolean(
                        GetValue(
                            infoSection,
                            "OtherError"),
                        "OtherError"),

                OtherErrorDescription =
                    GetNullableValue(
                        infoSection,
                        "OtherErrorDescription"),

                SuggestedMovieName =
                    GetNullableValue(
                        infoSection,
                        "SuggestedMovieName")
            };

        return new CutlistDocument(
            general,
            cuts,
            info);
    }

    private static Dictionary<string, Dictionary<string, string>>
        ParseSections(
            string content)
    {
        var sections =
            new Dictionary<string, Dictionary<string, string>>(
                StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string>? currentSection =
            null;

        using var reader =
            new StringReader(
                content);

        while (reader.ReadLine() is { } line)
        {
            var trimmedLine =
                line.Trim();

            if (trimmedLine.Length == 0)
            {
                continue;
            }

            if (trimmedLine.StartsWith('[') &&
                trimmedLine.EndsWith(']'))
            {
                var sectionName =
                    trimmedLine[1..^1];

                currentSection =
                    new Dictionary<string, string>(
                        StringComparer.OrdinalIgnoreCase);

                sections[sectionName] =
                    currentSection;

                continue;
            }

            if (currentSection is null)
            {
                continue;
            }

            var separatorIndex =
                line.IndexOf('=');

            if (separatorIndex < 0)
            {
                continue;
            }

            var name =
                line[..separatorIndex].Trim();

            var value =
                line[(separatorIndex + 1)..];

            currentSection[name] =
                value;
        }

        return sections;
    }

    private static Dictionary<string, string> GetRequiredSection(
        IReadOnlyDictionary<string, Dictionary<string, string>> sections,
        string sectionName)
    {
        if (sections.TryGetValue(
            sectionName,
            out var section))
        {
            return section;
        }

        throw new FormatException(
            $"Der Abschnitt [{sectionName}] fehlt.");
    }

    private static string GetRequiredValue(
        IReadOnlyDictionary<string, string> section,
        string name)
    {
        if (section.TryGetValue(
            name,
            out var value) &&
            !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new FormatException(
            $"Der Wert {name} fehlt.");
    }

    private static string GetValue(
        IReadOnlyDictionary<string, string> section,
        string name)
    {
        return section.TryGetValue(
            name,
            out var value)
            ? value
            : string.Empty;
    }

    private static string? GetNullableValue(
        IReadOnlyDictionary<string, string> section,
        string name)
    {
        var value =
            GetValue(
                section,
                name);

        return string.IsNullOrEmpty(value)
            ? null
            : value;
    }

    private static int ParseInt32(
        string value,
        string name)
    {
        if (int.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var parsed))
        {
            return parsed;
        }

        throw new FormatException(
            $"Der Wert {name} ist keine gültige Ganzzahl.");
    }

    private static long? ParseNullableInt64(
        string value,
        string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (long.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var parsed))
        {
            return parsed;
        }

        throw new FormatException(
            $"Der Wert {name} ist keine gültige Ganzzahl.");
    }

    private static double ParseDouble(
        string value,
        string name)
    {
        if (double.TryParse(
            value,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var parsed))
        {
            return parsed;
        }

        throw new FormatException(
            $"Der Wert {name} ist keine gültige Zahl.");
    }

    private static double? ParseNullableDouble(
        string value,
        string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseDouble(
            value,
            name);
    }

    private static bool ParseBoolean(
        string value,
        string name)
    {
        return value switch
        {
            "" => false,
            "0" => false,
            "1" => true,
            _ => throw new FormatException(
                $"Der Wert {name} ist kein gültiger Wahrheitswert.")
        };
    }
}
