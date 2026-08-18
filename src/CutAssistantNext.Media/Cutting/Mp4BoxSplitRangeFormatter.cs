using System.Globalization;

namespace CutAssistantNext.Media.Cutting;

public static class Mp4BoxSplitRangeFormatter
{
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
        return value.TotalSeconds.ToString(
            "0.#######",
            CultureInfo.InvariantCulture);
    }
}
