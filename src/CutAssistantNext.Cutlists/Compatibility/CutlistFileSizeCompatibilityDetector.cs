namespace CutAssistantNext.Cutlists.Compatibility;

public sealed record CutlistFileSizeMismatch(
    long ExpectedFileSizeBytes,
    long ActualFileSizeBytes,
    double DifferenceRatio);

public static class CutlistFileSizeCompatibilityDetector
{
    private const double MaximumAcceptedDifferenceRatio =
        0.05;

    public static CutlistFileSizeMismatch? FindMismatch(
        long? expectedFileSizeBytes,
        long? actualFileSizeBytes)
    {
        if (!expectedFileSizeBytes.HasValue ||
            !actualFileSizeBytes.HasValue ||
            expectedFileSizeBytes.Value <= 0 ||
            actualFileSizeBytes.Value <= 0)
        {
            return null;
        }

        var differenceBytes =
            Math.Abs(
                (double)expectedFileSizeBytes.Value -
                actualFileSizeBytes.Value);

        var differenceRatio =
            differenceBytes /
            expectedFileSizeBytes.Value;

        if (differenceRatio <=
            MaximumAcceptedDifferenceRatio)
        {
            return null;
        }

        return new CutlistFileSizeMismatch(
            expectedFileSizeBytes.Value,
            actualFileSizeBytes.Value,
            differenceRatio);
    }
}
