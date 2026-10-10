using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class OtrCanProgressReporterTests
{
    private sealed class Sink : IProgress<CutProgressUpdate>
    {
        public List<CutProgressUpdate> Updates { get; } = [];
        public void Report(CutProgressUpdate value) => Updates.Add(value);
    }

    [Fact]
    public void IndexPercentagesAreMeasuredAndOriginalOutputIsPreserved()
    {
        var sink = new Sink();
        var reporter = new OtrCanProgressReporter(sink, 2, indexing: true);
        foreach (var line in new[] { "Indexing, please wait... 0% ", "Indexing, please wait... 78%", "Writing index... done." })
            reporter.Report(new(CutProgressKind.Output, line));
        var measured = sink.Updates.Where(u => u.Kind == CutProgressKind.Progress).ToArray();
        Assert.Equal([0d, 78d, 100d], measured.Select(u => u.Percentage!.Value));
        Assert.Equal(3, sink.Updates.Count(u => u.Kind == CutProgressKind.Output));
    }

    [Fact]
    public void NativeStepsIncludeKeepNumberOperationAndSourceTimesWithoutInventingPercentages()
    {
        var sink = new Sink();
        var reporter = new OtrCanProgressReporter(sink, 2, indexing: false);
        foreach (var line in new[] {
            "TRACE: Processing interval no 2 ...", "TRACE: Re-encoding interval [4814.560000, 4816.420000] ...",
            "TRACE: Copying interval [4816.440000, 8375.799999] ...", "TRACE: Extracted interval",
            "TRACE: Concatenating interval files ..." }) reporter.Report(new(CutProgressKind.Output, line));
        var steps = sink.Updates.Where(u => u.Kind == CutProgressKind.Progress).ToArray();
        Assert.Contains(steps, u => u.Message.Contains("Behaltebereich 2 von 2") && u.Message.Contains("CPU") &&
            u.Message.Contains("01:20:14.560") && u.Message.Contains("01:20:16.420"));
        Assert.Contains(steps, u => u.Message.Contains("Abschnitt kopieren"));
        Assert.Equal("OTR-CAN: Behaltebereiche zusammenfügen", steps[^1].Message);
        Assert.All(steps, u => Assert.Null(u.Percentage));
    }

    [Fact]
    public void UnknownMalformedOrOutOfRangeLinesRemainOnlyInTheProtocol()
    {
        var sink = new Sink();
        var index = new OtrCanProgressReporter(sink, 2, indexing: true);
        index.Report(new(CutProgressKind.Output, "Indexing, please wait... 999%"));
        var native = new OtrCanProgressReporter(sink, 2, indexing: false);
        native.Report(new(CutProgressKind.Output, "TRACE: Processing interval no 999 ..."));
        native.Report(new(CutProgressKind.Output, "TRACE: Re-encoding interval [NaN, 2.0] ..."));
        native.Report(new(CutProgressKind.Output, "unknown tool output"));
        Assert.Equal(4, sink.Updates.Count);
        Assert.All(sink.Updates, u => Assert.Equal(CutProgressKind.Output, u.Kind));
    }

    [Fact]
    public void NonOutputStatusesArePassedThroughWithoutTranslation()
    {
        var sink = new Sink();
        var update = new CutProgressUpdate(CutProgressKind.Status, "Ausgabe prüfen");
        new OtrCanProgressReporter(sink, 2, false).Report(update);
        Assert.Same(update, Assert.Single(sink.Updates));
        new OtrCanProgressReporter(null, 2, false).Report(update);
    }
}
