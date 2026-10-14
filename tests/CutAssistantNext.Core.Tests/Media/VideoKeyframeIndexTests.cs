using CutAssistantNext.Core.Media;

namespace CutAssistantNext.Core.Tests.Media;

public sealed class VideoKeyframeIndexTests
{
    private static VideoFrameWindow Window(params VideoFrameInfo[] frames) => new(0, new VideoTimeBase(1, 1000), 0, 0, frames);
    private static VideoFrameInfo Frame(long? pts, bool? key = true, string type = "I", long? best = null) => new(0, pts, best, 777, null, key, type);
    private static VideoKeyframeIndex Index() => VideoKeyframeIndex.Create(Window(Frame(0), Frame(1000), Frame(2000), Frame(9999)), 0, TimeSpan.FromSeconds(10), 0);

    [Theory]
    [InlineData(0, -1, null)]
    [InlineData(0, 1, 1000)]
    [InlineData(500, -1, 0)]
    [InlineData(500, 1, 1000)]
    [InlineData(1000, -1, 0)]
    [InlineData(1000, 1, 2000)]
    [InlineData(1000.3, 1, 2000)]
    [InlineData(9999, 1, null)]
    [InlineData(10000, -1, 9999)]
    public void FindsStrictAdjacentKeyframeWithoutRepeatingCurrent(double current, int direction, int? expected)
    {
        Assert.Equal(expected.HasValue ? TimeSpan.FromMilliseconds(expected.Value) : (TimeSpan?)null,
            Index().FindAdjacent(TimeSpan.FromMilliseconds(current), direction));
    }
    [Theory]
    [InlineData(1000, true)]
    [InlineData(1000.4, true)]
    [InlineData(999.6, true)]
    [InlineData(1001, false)]
    [InlineData(1040, false)]
    [InlineData(500, false)]
    public void IndicatorToleratesSerializationNoiseOnly(double position, bool expected) =>
        Assert.Equal(expected, Index().Contains(TimeSpan.FromMilliseconds(position)));

    [Fact]
    public void IFrameWithoutKeyFlagIsNotPromoted()
    {
        var index = VideoKeyframeIndex.Create(Window(Frame(0, false), Frame(1000, null), Frame(2000, true, "P")), 0, TimeSpan.FromSeconds(10), 0);
        Assert.Equal([TimeSpan.FromSeconds(2)], index.Positions);
    }
    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(2)]
    public void NormalizesOriginalPtsByContainerStart(int origin)
    {
        var index = VideoKeyframeIndex.Create(Window(Frame(origin * 1000 + 40, best: 9999)), 0, TimeSpan.FromSeconds(10), origin);
        Assert.Equal(TimeSpan.FromMilliseconds(40), Assert.Single(index.Positions));
    }
    [Fact]
    public void PtsHasPriorityOverBestEffort() => Assert.Equal(TimeSpan.FromSeconds(1), Assert.Single(
        VideoKeyframeIndex.Create(Window(Frame(1000, best: 2000)), 0, TimeSpan.FromSeconds(10), 0).Positions));
    [Theory]
    [InlineData("duplicate")]
    [InlineData("decreasing")]
    [InlineData("missing")]
    [InlineData("mp4-best-effort")]
    [InlineData("empty")]
    public void RejectsUnsafeOrIncompleteKeyframeTimes(string reason)
    {
        var frames = reason switch
        {
            "duplicate" => new[] { Frame(1000), Frame(1000) },
            "decreasing" => new[] { Frame(1000), Frame(500) },
            "missing" => new[] { Frame(null) },
            "mp4-best-effort" => new[] { Frame(null, best: 1000) },
            _ => Array.Empty<VideoFrameInfo>()
        };
        Assert.Throws<InvalidOperationException>(() => VideoKeyframeIndex.Create(Window(frames), 0, TimeSpan.FromSeconds(10), 0));
    }
    [Fact]
    public void ExcludesPrerollAndEndBoundary()
    {
        var index = VideoKeyframeIndex.Create(Window(Frame(-1000), Frame(0), Frame(10000)), 0, TimeSpan.FromSeconds(10), 0);
        Assert.Equal([TimeSpan.Zero], index.Positions);
    }
    [Fact]
    public void RejectsWrongStream() => Assert.Throws<ArgumentException>(() => VideoKeyframeIndex.Create(Window(Frame(0)), 1, TimeSpan.FromSeconds(10), 0));
}
