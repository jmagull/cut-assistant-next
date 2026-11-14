namespace CutAssistantNext.Media.Cutting;

public sealed class Mp4BoxSegmentException(Mp4BoxSplitRange range, string toolOutput)
    : InvalidOperationException(
        DescribeRange(range) + " MP4Box meldete Exitcode 0, erzeugte aber keine Segmentdatei. " +
        "Bitte die Schnittgrenzen und die Keyframes prüfen." +
        (string.IsNullOrWhiteSpace(toolOutput) ? "" : Environment.NewLine + toolOutput.Trim()))
{
    public static string DescribeRange(Mp4BoxSplitRange range)
    {
        var source = range.RequestedEnd is { } end ? $"Quellbereich {range.Start:c} bis {end:c}. " : "";
        return source + $"Verwendet wurde -splitx {Mp4BoxSplitRangeFormatter.Format(range)}.";
    }
}
