using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxOutputReaderTests
{
    [Fact]
    public async Task ReadAsync_WithMultipleLines_ReportsEveryLineInOrder()
    {
        using var reader =
            new StringReader(
                "splitting: file 1 done" +
                Environment.NewLine +
                "0.500 secs Interleaving");

        var updates =
            new List<CutProgressUpdate>();

        var progress =
            new InlineProgress<CutProgressUpdate>(
                updates.Add);

        await Mp4BoxOutputReader.ReadAsync(
            reader,
            progress);

        Assert.Collection(
            updates,
            update =>
            {
                Assert.Equal(
                    CutProgressKind.Output,
                    update.Kind);

                Assert.Equal(
                    "splitting: file 1 done",
                    update.Message);
            },
            update =>
            {
                Assert.Equal(
                    CutProgressKind.Output,
                    update.Kind);

                Assert.Equal(
                    "0.500 secs Interleaving",
                    update.Message);
            });
    }

    [Fact]
    public async Task ReadAsync_ReturnsReportedLines()
    {
        using var reader =
            new StringReader(
                "erste Zeile" +
                Environment.NewLine +
                "zweite Zeile");

        var lines =
            await Mp4BoxOutputReader.ReadAsync(
                reader,
                progress: null);

        Assert.Equal(
            new[]
            {
                "erste Zeile",
                "zweite Zeile"
            },
            lines);
    }
    private sealed class InlineProgress<T> :
        IProgress<T>
    {
        private readonly Action<T> _report;

        public InlineProgress(
            Action<T> report)
        {
            _report =
                report
                ?? throw new ArgumentNullException(
                    nameof(report));
        }

        public void Report(
            T value)
        {
            _report(
                value);
        }
    }

    [Fact]
    public async Task ReadAsync_FiltersVerboseProgressOnlyForLiveReporting()
    {
        var inputLines =
            new[]
            {
                "splitting:  0.01 %",
                "splitting:  0.99 %",
                "splitting:  1.00 %",
                "splitting: file 1 done",
                "Appending: |                    | (01/100)",
                "Appending: |=                   | (05/100)",
                "ISO File Writing: |=================== | (99/100)",
                "ISO File Writing: |====================| (100/100)"
            };

        using var reader =
            new StringReader(
                string.Join(
                    Environment.NewLine,
                    inputLines));

        var reportedUpdates =
            new List<CutProgressUpdate>();

        var progress =
            new SynchronousProgress(
                reportedUpdates);

        var returnedLines =
            await Mp4BoxOutputReader.ReadAsync(
                reader,
                progress);

        Assert.Equal(
            inputLines,
            returnedLines);

        Assert.Equal(
            new[]
            {
                "splitting:  1.00 %",
                "splitting: file 1 done",
                "Appending: |=                   | (05/100)",
                "ISO File Writing: |====================| (100/100)"
            },
            reportedUpdates.Select(
                update => update.Message));
    }

    private sealed class SynchronousProgress :
        IProgress<CutProgressUpdate>
    {
        private readonly List<CutProgressUpdate> _updates;

        public SynchronousProgress(
            List<CutProgressUpdate> updates)
        {
            _updates =
                updates;
        }

        public void Report(
            CutProgressUpdate value)
        {
            _updates.Add(
                value);
        }
    }
}
