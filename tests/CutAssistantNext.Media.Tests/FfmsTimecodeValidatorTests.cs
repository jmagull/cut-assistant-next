using System.Globalization;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests;

public sealed class FfmsTimecodeValidatorTests
{
    [Theory]
    [InlineData("0.00\n40.00\n80.00\n", 3)]
    [InlineData("20.00\n40.00\n100.00", 3)]
    [InlineData("0\n33.3667\n75.5\n125.002\n", 4)]
    [InlineData("0.000\n0.001\n", 2)]
    [InlineData("0.00", 1)]
    public async Task AcceptsStrictlyIncreasingCfrVfrAndOffsetTimes(string body, int count)
    {
        using var reader = new StringReader("# timecode format v2\n" + body);
        Assert.Equal(count, await FfmsTimecodeValidator.ValidateAsync(reader));
    }

    [Theory]
    [InlineData("0\n40\n40\n80\n")]
    [InlineData("20\n80\n100\n80\n")]
    [InlineData("0.0001\n0.0009\n")]
    [InlineData("NaN\n")]
    [InlineData("Infinity\n")]
    [InlineData("-Infinity\n")]
    [InlineData("-0.01\n")]
    [InlineData("1e300\n")]
    [InlineData("0,00\n")]
    [InlineData("\n")]
    [InlineData("0\n\n40\n")]
    [InlineData("0\nnot a time\n")]
    [InlineData(" 0\n")]
    [InlineData("")]
    public async Task RejectsAmbiguousInvalidOrEmptyTimes(string body)
    {
        using var reader = new StringReader("# timecode format v2\n" + body);
        await Assert.ThrowsAsync<InvalidDataException>(() => FfmsTimecodeValidator.ValidateAsync(reader));
    }

    [Theory]
    [InlineData("")]
    [InlineData("index")]
    [InlineData("0.00\n40.00\n")]
    [InlineData("# timecode format v1\n0\n40\n")]
    public async Task RejectsMissingOrUnsupportedHeader(string text)
    {
        using var reader = new StringReader(text);
        await Assert.ThrowsAsync<InvalidDataException>(() => FfmsTimecodeValidator.ValidateAsync(reader));
    }

    [Fact]
    public async Task IdentifiesTheFirstOffendingFrameAndLine()
    {
        using var reader = new StringReader("# timecode format v2\n0\n40\n80\n40\n");
        var error = await Assert.ThrowsAsync<InvalidDataException>(() => FfmsTimecodeValidator.ValidateAsync(reader));
        Assert.Contains("Bild 4 (Zeile 5)", error.Message);
    }

    [Fact]
    public async Task ParsesInvariantDecimalsUnderGermanCulture()
    {
        var old = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            using var reader = new StringReader("# timecode format v2\n0.00\n40.25\n");
            Assert.Equal(2, await FfmsTimecodeValidator.ValidateAsync(reader));
        }
        finally { CultureInfo.CurrentCulture = old; }
    }

    [Fact]
    public async Task HonorsCancellationBeforeReading()
    {
        using var reader = new StringReader("# timecode format v2\n0\n40\n");
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => FfmsTimecodeValidator.ValidateAsync(reader, new(true)));
    }

    [Fact]
    public async Task HonorsCancellationDuringReading()
    {
        using var cancellation = new CancellationTokenSource();
        using var reader = new CancellingReader(cancellation);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => FfmsTimecodeValidator.ValidateAsync(reader, cancellation.Token));
    }

    [Theory]
    [InlineData("0\n40\n80\n120\n")]
    [InlineData("0\n20\n40\n60\n")]
    [InlineData("0\n40\n80.001\n120\n")]
    [InlineData("0\n20\n40.001\n60\n")]
    public async Task MotorModeAcceptsZeroStart25And50FpsWithAllowedJitter(string body)
    {
        using var reader = new StringReader("# timecode format v2\n" + body);
        Assert.Equal(4, await FfmsTimecodeValidator.ValidateForOtrCanAsync(reader));
    }

    [Theory]
    [InlineData("0\n840\n880\n920\n")]
    [InlineData("0\n40\n80\n160\n")]
    [InlineData("0\n40\n80.004\n120\n")]
    [InlineData("0\n33.333\n66.666\n")]
    [InlineData("0\n41.666\n83.332\n")]
    [InlineData("20\n40\n60\n")]
    [InlineData("0\n")]
    public async Task MonotonicButUnsupportedMotorTimesAreRejected(string body)
    {
        using var generic = new StringReader("# timecode format v2\n" + body);
        Assert.True(await FfmsTimecodeValidator.ValidateAsync(generic) > 0);
        using var motor = new StringReader("# timecode format v2\n" + body);
        await Assert.ThrowsAsync<NotSupportedException>(() => FfmsTimecodeValidator.ValidateForOtrCanAsync(motor));
    }

    [Fact]
    public async Task AmbiguityKeepsPriorityOverAnUnsupportedStartOffset()
    {
        using var reader = new StringReader("# timecode format v2\n20\n80\n100\n100\n");
        var error = await Assert.ThrowsAsync<InvalidDataException>(() => FfmsTimecodeValidator.ValidateForOtrCanAsync(reader));
        Assert.Contains("Bild 4", error.Message);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(40)]
    public async Task MotorModeChecksTheWholeTimelineRatherThanOnlyAverageRate(int step)
    {
        var lines = Enumerable.Range(0, 10000).Select(i => (i * (double)step).ToString("F2", CultureInfo.InvariantCulture)).ToArray();
        string prefix = "# timecode format v2\n";
        using (var reader = new StringReader(prefix + string.Join('\n', lines)))
            Assert.Equal(10000, await FfmsTimecodeValidator.ValidateForOtrCanAsync(reader));
        lines[^1] = (9999 * step + 0.01).ToString("F2", CultureInfo.InvariantCulture);
        using var drift = new StringReader(prefix + string.Join('\n', lines));
        await Assert.ThrowsAsync<NotSupportedException>(() => FfmsTimecodeValidator.ValidateForOtrCanAsync(drift));
    }
    private sealed class CancellingReader(CancellationTokenSource cancellation) : StringReader("# timecode format v2\n0\n40\n80\n")
    {
        private int _lines;
        public override ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken)
        {
            var result = base.ReadLineAsync(cancellationToken);
            if (++_lines == 3) cancellation.Cancel();
            return result;
        }
    }
}
