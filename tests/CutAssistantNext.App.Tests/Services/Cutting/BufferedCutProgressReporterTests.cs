using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class BufferedCutProgressReporterTests
{
    [Fact]
    public void LargeWarningBurstIsBatchedWithoutLosingOutputOrPhaseOrder()
    {
        List<CutProgressUpdate> delivered = [];
        var reporter = new BufferedCutProgressReporter(delivered.Add);
        reporter.Report(new(CutProgressKind.Status, "Audio lesen"));
        var lines = Enumerable.Range(0, 20000).Select(i => $"Warnung {i}").ToArray();
        foreach (var line in lines) reporter.Report(new(CutProgressKind.Output, line));
        reporter.Report(new(CutProgressKind.Progress, "Audio prüfen") { Percentage = 75 });
        reporter.Report(new(CutProgressKind.Output, "Letzte Meldung"));
        Assert.Empty(delivered);
        reporter.Flush();
        Assert.Equal(4, delivered.Count);
        Assert.Equal("Audio lesen", delivered[0].Message);
        Assert.Equal(string.Join(Environment.NewLine, lines), delivered[1].Message);
        Assert.Equal(CutProgressKind.Progress, delivered[2].Kind);
        Assert.Equal(75, delivered[2].Percentage);
        Assert.Equal("Letzte Meldung", delivered[3].Message);
        reporter.Flush();
        Assert.Equal(4, delivered.Count);
    }

    [Fact]
    public void NewOutputDuringFlushIsKeptForTheNextFlush()
    {
        List<string> delivered = [];
        BufferedCutProgressReporter? reporter = null;
        reporter = new(update =>
        {
            delivered.Add(update.Message);
            if (update.Message == "first")
                reporter!.Report(new(CutProgressKind.Output, "second"));
        });
        reporter.Report(new(CutProgressKind.Output, "first"));
        reporter.Flush();
        Assert.Equal(["first"], delivered);
        reporter.Flush();
        Assert.Equal(["first", "second"], delivered);
    }
}
