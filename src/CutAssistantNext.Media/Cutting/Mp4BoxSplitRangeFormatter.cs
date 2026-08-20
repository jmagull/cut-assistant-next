using System.Globalization;

namespace CutAssistantNext.Media.Cutting;

public static class Mp4BoxSplitRangeFormatter
{
    private const int MaximumDecimalPlaces =
        7;

    private static readonly decimal OneTickInSeconds =
        1m / TimeSpan.TicksPerSecond;

    public static string Format(
        Mp4BoxSplitRange range)
    {
        ArgumentNullException.ThrowIfNull(range);

        return
            $"{FormatSeconds(range.Start)}:{FormatSeconds(range.End)}";
    }

    private static string FormatSeconds(
        TimeSpan value)
    {
        var seconds =
            value.Ticks /
            (decimal)TimeSpan.TicksPerSecond;

        for (var decimalPlaces = 0;
             decimalPlaces <= MaximumDecimalPlaces;
             decimalPlaces++)
        {
            var rounded =
                decimal.Round(
                    seconds,
                    decimalPlaces,
                    MidpointRounding.AwayFromZero);

            if (Math.Abs(
                    seconds - rounded) <=
                OneTickInSeconds)
            {
                return rounded.ToString(
                    "0.#######",
                    CultureInfo.InvariantCulture);
            }
        }

        return seconds.ToString(
            "0.#######",
            CultureInfo.InvariantCulture);
    }
}
