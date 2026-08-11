using System.Globalization;
using System.Text;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Serialization;

public static class CutlistSerializer
{
    public static string Serialize(
        CutlistDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var builder = new StringBuilder();

        builder.AppendLine("[General]");

        AppendValue(
            builder,
            "Application",
            document.General.Application);

        AppendValue(
            builder,
            "Version",
            document.General.Version);

        AppendValue(
            builder,
            "comment1",
            "The following parts of the movie will be kept, the rest will be cut out.");

        AppendValue(
            builder,
            "comment2",
            "All values are given in seconds.");

        AppendValue(
            builder,
            "FramesPerSecond",
            document.General.FramesPerSecond.HasValue
                ? FormatNumber(document.General.FramesPerSecond.Value)
                : string.Empty);

        AppendValue(
            builder,
            "DisplayAspectRatio",
            document.General.DisplayAspectRatio);

        AppendValue(
            builder,
            "ApplyToFile",
            document.General.ApplyToFile);

        AppendValue(
            builder,
            "OriginalFileSizeBytes",
            document.General.OriginalFileSizeBytes?.ToString(
                CultureInfo.InvariantCulture));

        AppendValue(
            builder,
            "IntendedCutApplicationName",
            document.General.IntendedCutApplicationName);

        AppendValue(
            builder,
            "IntendedCutApplication",
            document.General.IntendedCutApplication);

        AppendValue(
            builder,
            "IntendedCutApplicationVersion",
            document.General.IntendedCutApplicationVersion);

        AppendValue(
            builder,
            "IntendedCutApplicationOptions",
            document.General.IntendedCutApplicationOptions);

        AppendValue(
            builder,
            "NoOfCuts",
            document.General.NoOfCuts.ToString(
                CultureInfo.InvariantCulture));

        for (var index = 0;
             index < document.Cuts.Count;
             index++)
        {
            var cut = document.Cuts[index];

            builder.AppendLine();
            builder.Append('[');
            builder.Append("Cut");
            builder.Append(index);
            builder.AppendLine("]");

            AppendValue(
                builder,
                "Start",
                FormatSeconds(cut.Start));

            AppendValue(
                builder,
                "Duration",
                FormatSeconds(cut.Duration));
        }

        builder.AppendLine();
        builder.AppendLine("[Info]");

        AppendValue(
            builder,
            "RatingByAuthor",
            document.Info.RatingByAuthor.ToString(
                CultureInfo.InvariantCulture));

        AppendValue(
            builder,
            "Author",
            document.Info.Author);

        AppendValue(
            builder,
            "UserComment",
            document.Info.UserComment);

        AppendValue(
            builder,
            "EPGError",
            FormatBoolean(document.Info.EpgError));

        AppendValue(
            builder,
            "ActualContent",
            document.Info.ActualContent);

        AppendValue(
            builder,
            "MissingBeginning",
            FormatBoolean(document.Info.MissingBeginning));

        AppendValue(
            builder,
            "MissingEnding",
            FormatBoolean(document.Info.MissingEnding));

        AppendValue(
            builder,
            "MissingVideo",
            FormatBoolean(document.Info.MissingVideo));

        AppendValue(
            builder,
            "MissingAudio",
            FormatBoolean(document.Info.MissingAudio));

        AppendValue(
            builder,
            "OtherError",
            FormatBoolean(document.Info.OtherError));

        AppendValue(
            builder,
            "OtherErrorDescription",
            document.Info.OtherErrorDescription);

        AppendValue(
            builder,
            "SuggestedMovieName",
            document.Info.SuggestedMovieName);

        return builder.ToString();
    }

    private static string FormatBoolean(
        bool value)
    {
        return value
            ? "1"
            : "0";
    }

    private static void AppendValue(
        StringBuilder builder,
        string name,
        string? value)
    {
        builder.Append(name);
        builder.Append('=');
        builder.AppendLine(
            value ?? string.Empty);
    }

    private static string FormatSeconds(
        TimeSpan value)
    {
        return value.TotalSeconds.ToString(
            "0.##########",
            CultureInfo.InvariantCulture);
    }

    private static string FormatNumber(
        double value)
    {
        return value.ToString(
            "0.##########",
            CultureInfo.InvariantCulture);
    }
}
