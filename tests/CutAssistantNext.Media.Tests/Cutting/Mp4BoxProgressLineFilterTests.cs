using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxProgressLineFilterTests
{
    [Theory]
    [InlineData("splitting:  0.01 %", false)]
    [InlineData("splitting:  0.99 %", false)]
    [InlineData("splitting:  1.00 %", true)]
    [InlineData("splitting:  7.43 %", false)]
    [InlineData("splitting:  10.00 %", true)]
    [InlineData("splitting: file 1 done", true)]
    public void ShouldReport_SplittingProgress_ReportsOnlyWholePercentages(
        string line,
        bool expected)
    {
        Assert.Equal(
            expected,
            Mp4BoxProgressLineFilter.ShouldReport(line));
    }

    [Theory]
    [InlineData("Appending: |                    | (01/100)", false)]
    [InlineData("Appending: |=                   | (05/100)", true)]
    [InlineData("Appending: |==========          | (50/100)", true)]
    [InlineData("Appending: |=================== | (99/100)", false)]
    [InlineData("Appending: |====================| (100/100)", true)]
    public void ShouldReport_AppendingProgress_ReportsFivePercentSteps(
        string line,
        bool expected)
    {
        Assert.Equal(
            expected,
            Mp4BoxProgressLineFilter.ShouldReport(line));
    }

    [Theory]
    [InlineData("ISO File Writing: |                    | (01/100)", false)]
    [InlineData("ISO File Writing: |=                   | (05/100)", true)]
    [InlineData("ISO File Writing: |====================| (100/100)", true)]
    [InlineData("No suitable destination track found - creating new one (type vide)", true)]
    [InlineData("0.500 secs Interleaving", true)]
    public void ShouldReport_OtherOutput_KeepsMeaningfulLines(
        string line,
        bool expected)
    {
        Assert.Equal(
            expected,
            Mp4BoxProgressLineFilter.ShouldReport(line));
    }
}
