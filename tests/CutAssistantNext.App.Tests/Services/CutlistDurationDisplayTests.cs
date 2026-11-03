using System.Globalization;
using System.Text;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistDurationDisplayTests
{
    [Fact]
    public void ActualServerSecondsAndLocalTickResidueHaveTheSameMillisecondDisplay()
    {
        var path = Path.Combine(Path.GetTempPath(), "Aufnahme.HQ.mp4");
        var context = LocalCutlistDiscoveryTests.Context(path);
        context = context with { Analysis = context.Analysis with { Duration = TimeSpan.FromSeconds(5000) } };
        var bytes = Encoding.UTF8.GetBytes("""
            [General]
            ApplyToFile=Aufnahme.HQ.mp4
            OriginalFileSizeBytes=1234
            FramesPerSecond=25
            NoOfCuts=1
            [Cut0]
            Start=10
            Duration=2459.0799999
            [Info]
            RatingByAuthor=5
            """);
        var local = CutlistResultRow.FromLocal(new LocalCutlistCandidate(path + ".cutlist", bytes), context);
        var server = CutlistResultRow.FromServer(new CutlistSearchResult { Duration = "2459.08" }, "https://example.invalid/");
        var ticksBeforeDisplay = local.Document!.Cuts.Single().Duration.Ticks;
        Assert.Equal("00:40:59.080", server.Duration);
        Assert.Equal(server.Duration, local.Duration);
        Assert.Equal(ticksBeforeDisplay, local.Document.Cuts.Single().Duration.Ticks);
        Assert.Equal("2459.08", server.Server!.Duration);
    }

    [Theory]
    [InlineData("0", "00:00:00.000")]
    [InlineData("3599.9999", "01:00:00.000")]
    [InlineData("90061.005", "25:01:01.005")]
    [InlineData("", "–")]
    [InlineData("unknown", "–")]
    [InlineData("NaN", "–")]
    [InlineData("Infinity", "–")]
    [InlineData("-1", "–")]
    [InlineData("1e30", "–")]
    public void ServerDurationHasStableFormattingAndNeverInventsMissingValues(string seconds, string expected)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var row = CutlistResultRow.FromServer(new CutlistSearchResult { Duration = seconds }, "https://example.invalid/");
            Assert.Equal(expected, row.Duration);
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }
}
