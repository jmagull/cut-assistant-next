using CutAssistantNext.Core.Editing;

namespace CutAssistantNext.Core.Tests.Editing;

public sealed class RemoveSegmentTests
{
    [Fact]
    public void Constructor_SetsStartEndAndDuration()
    {
        var segment = new RemoveSegment(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(25.5));

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            segment.Start);

        Assert.Equal(
            TimeSpan.FromSeconds(25.5),
            segment.End);

        Assert.Equal(
            TimeSpan.FromSeconds(15.5),
            segment.Duration);
    }

    [Fact]
    public void Constructor_WithNegativeStart_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RemoveSegment(
                TimeSpan.FromSeconds(-1),
                TimeSpan.FromSeconds(10)));
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void Constructor_WithEndNotAfterStart_ThrowsArgumentOutOfRangeException(
        double startSeconds,
        double endSeconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RemoveSegment(
                TimeSpan.FromSeconds(startSeconds),
                TimeSpan.FromSeconds(endSeconds)));
    }
}