namespace CutAssistantNext.App.Controls;

internal static class TimelinePositionCalculator
{
    internal static TimeSpan? GetPosition(
        double horizontalOffset,
        double width,
        TimeSpan? duration)
    {
        if (!double.IsFinite(horizontalOffset) ||
            !double.IsFinite(width) ||
            width <= 0 ||
            !duration.HasValue ||
            duration.Value <= TimeSpan.Zero)
        {
            return null;
        }

        var fraction = Math.Clamp(horizontalOffset / width, 0, 1);
        var ticks = decimal.ToInt64(decimal.Round(
            duration.Value.Ticks * (decimal)fraction,
            0,
            MidpointRounding.AwayFromZero));

        return TimeSpan.FromTicks(ticks);
    }
}
