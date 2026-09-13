using CutAssistantNext.Cutlists.Compatibility;

namespace CutAssistantNext.Cutlists.Tests.Compatibility;

public sealed class CutlistFileSizeCompatibilityDetectorTests
{
    [Fact]
    public void FindMismatch_WithLargeDifference_ReturnsMismatch()
    {
        var mismatch =
            CutlistFileSizeCompatibilityDetector.FindMismatch(
                expectedFileSizeBytes: 903_892_393,
                actualFileSizeBytes: 548_381_235);

        Assert.NotNull(
            mismatch);

        Assert.Equal(
            903_892_393,
            mismatch.ExpectedFileSizeBytes);

        Assert.Equal(
            548_381_235,
            mismatch.ActualFileSizeBytes);
    }

    [Fact]
    public void FindMismatch_WithTinyDifference_ReturnsNull()
    {
        var mismatch =
            CutlistFileSizeCompatibilityDetector.FindMismatch(
                expectedFileSizeBytes: 903_892_393,
                actualFileSizeBytes: 903_892_333);

        Assert.Null(
            mismatch);
    }

    [Fact]
    public void FindMismatch_WithExactlyFivePercentDifference_ReturnsNull()
    {
        var mismatch =
            CutlistFileSizeCompatibilityDetector.FindMismatch(
                expectedFileSizeBytes: 100_000_000,
                actualFileSizeBytes: 95_000_000);

        Assert.Null(
            mismatch);
    }

    [Fact]
    public void FindMismatch_WithMissingExpectedSize_ReturnsNull()
    {
        var mismatch =
            CutlistFileSizeCompatibilityDetector.FindMismatch(
                expectedFileSizeBytes: null,
                actualFileSizeBytes: 548_381_235);

        Assert.Null(
            mismatch);
    }

    [Fact]
    public void FindMismatch_WithMissingActualSize_ReturnsNull()
    {
        var mismatch =
            CutlistFileSizeCompatibilityDetector.FindMismatch(
                expectedFileSizeBytes: 903_892_393,
                actualFileSizeBytes: null);

        Assert.Null(
            mismatch);
    }
}
