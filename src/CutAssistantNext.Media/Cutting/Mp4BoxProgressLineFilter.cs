using System.Globalization;

namespace CutAssistantNext.Media.Cutting;

public static class Mp4BoxProgressLineFilter
{
    public static bool ShouldReport(
        string line)
    {
        ArgumentNullException.ThrowIfNull(
            line);

        if (TryParseSplittingPercentage(
            line,
            out var splittingPercentage))
        {
            return splittingPercentage ==
                decimal.Truncate(
                    splittingPercentage);
        }

        if (TryParseHundredStepProgress(
            line,
            out var progressValue))
        {
            return progressValue % 5 == 0;
        }

        return true;
    }

    private static bool TryParseSplittingPercentage(
        string line,
        out decimal percentage)
    {
        percentage = 0;

        const string prefix =
            "splitting:";

        if (!line.StartsWith(
            prefix,
            StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var percentIndex =
            line.IndexOf(
                '%',
                prefix.Length);

        if (percentIndex < 0)
        {
            return false;
        }

        var numberText =
            line[
                prefix.Length..
                percentIndex]
            .Trim();

        return decimal.TryParse(
            numberText,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out percentage);
    }

    private static bool TryParseHundredStepProgress(
        string line,
        out int progressValue)
    {
        progressValue = 0;

        if (!line.StartsWith(
                "Appending:",
                StringComparison.OrdinalIgnoreCase) &&
            !line.StartsWith(
                "ISO File Writing:",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var slashIndex =
            line.LastIndexOf(
                "/100)",
                StringComparison.Ordinal);

        if (slashIndex < 0)
        {
            return false;
        }

        var openParenthesisIndex =
            line.LastIndexOf(
                '(',
                slashIndex);

        if (openParenthesisIndex < 0)
        {
            return false;
        }

        var numberText =
            line[
                (openParenthesisIndex + 1)..
                slashIndex];

        return int.TryParse(
            numberText,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out progressValue);
    }
}
