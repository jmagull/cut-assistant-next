using System.Globalization;
using System.Windows;
using CutAssistantNext.App.Controls;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests;

public sealed class KeyframeBoundaryVisibilityTests
{
    [Theory]
    [InlineData(1000, Visibility.Visible)]
    [InlineData(1000.3, Visibility.Visible)]
    [InlineData(1040, Visibility.Collapsed)]
    [InlineData(2000, Visibility.Collapsed)]
    public void MarksOnlyTheBoundaryTimestamp(double milliseconds, Visibility expected)
    {
        var frames = new VideoFrameInfo[]
        {
            new(0, 1000, null, null, null, true, "I"),
            new(0, 2000, null, null, null, true, "I")
        };
        var window = new VideoFrameWindow(0, new VideoTimeBase(1, 1000), 0, 0, frames);
        var index = VideoKeyframeIndex.Create(window, 0, TimeSpan.FromSeconds(2), 0);
        var converter = new KeyframeBoundaryVisibilityConverter();

        Assert.Equal(expected, converter.Convert([TimeSpan.FromMilliseconds(milliseconds), index],
            typeof(Visibility), null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void HidesMarkerUntilIndexIsAvailable()
    {
        var converter = new KeyframeBoundaryVisibilityConverter();
        Assert.Equal(Visibility.Collapsed, converter.Convert([TimeSpan.Zero, DependencyProperty.UnsetValue],
            typeof(Visibility), null!, CultureInfo.InvariantCulture));
    }
}
