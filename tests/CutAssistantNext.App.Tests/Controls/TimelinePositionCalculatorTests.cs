using CutAssistantNext.App.Controls;

namespace CutAssistantNext.App.Tests.Controls;

public sealed class TimelinePositionCalculatorTests
{
    [Theory]
    [InlineData(-20, 200, 0)]
    [InlineData(0, 200, 0)]
    [InlineData(50, 200, 30)]
    [InlineData(100, 200, 60)]
    [InlineData(150, 200, 90)]
    [InlineData(200, 200, 120)]
    [InlineData(250, 200, 120)]
    public void GetPosition_MapsAndClampsHorizontalPosition(
        double offset,
        double width,
        double expectedSeconds)
    {
        Assert.Equal(
            TimeSpan.FromSeconds(expectedSeconds),
            TimelinePositionCalculator.GetPosition(
                offset, width, TimeSpan.FromSeconds(120)));
    }

    [Theory]
    [InlineData(double.NaN, 200)]
    [InlineData(double.PositiveInfinity, 200)]
    [InlineData(double.NegativeInfinity, 200)]
    [InlineData(100, double.NaN)]
    [InlineData(100, double.PositiveInfinity)]
    [InlineData(100, double.NegativeInfinity)]
    [InlineData(100, 0)]
    [InlineData(100, -200)]
    public void GetPosition_RejectsInvalidCoordinates(double offset, double width)
    {
        Assert.Null(TimelinePositionCalculator.GetPosition(
            offset, width, TimeSpan.FromSeconds(120)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0d)]
    [InlineData(-1d)]
    public void GetPosition_WithoutPositiveDuration_ReturnsNull(double? seconds)
    {
        var duration = seconds.HasValue
            ? TimeSpan.FromSeconds(seconds.Value)
            : (TimeSpan?)null;

        Assert.Null(TimelinePositionCalculator.GetPosition(100, 200, duration));
    }

    [Fact]
    public void GetPosition_RoundsToNearestTick()
    {
        Assert.Equal(
            TimeSpan.FromTicks(2),
            TimelinePositionCalculator.GetPosition(1, 2, TimeSpan.FromTicks(3)));
    }

    [Fact]
    public void GetPosition_AtMaximumDurationEnd_DoesNotOverflow()
    {
        Assert.Equal(
            TimeSpan.MaxValue,
            TimelinePositionCalculator.GetPosition(200, 200, TimeSpan.MaxValue));
    }
}
