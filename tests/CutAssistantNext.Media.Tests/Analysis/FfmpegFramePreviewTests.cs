using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class FfmpegFramePreviewTests
{
    [Fact]
    public void Arguments_SelectRawPtsAndAbsoluteTimestampWithOffset()
    {
        var arguments = FfmpegFramePreviewRunner.BuildArguments(
            "source with spaces.mp4", 2, new VideoTimeBase(1, 1000), 104200);
        Assert.Contains("-copyts", arguments);
        Assert.Contains("-seek_timestamp", arguments);
        Assert.Contains("102.2", arguments);
        Assert.Contains("0:2", arguments);
        Assert.Contains("source with spaces.mp4", arguments);
        Assert.Contains(arguments, argument => argument.Contains("select=eq(pts\\,104200),showinfo"));
        Assert.Equal("pipe:1", arguments[^1]);
    }

    [Fact]
    public void NegativePtsPreview_DecodesFromBeginningWithoutZeroSeek()
    {
        var arguments = FfmpegFramePreviewRunner.BuildArguments(
            "video.mp4", 0, new VideoTimeBase(1, 1000), -40);
        Assert.DoesNotContain("-ss", arguments);
        Assert.Contains(arguments, argument => argument.Contains("select=eq(pts\\,-40)"));
    }

    [Theory]
    [InlineData(9007199254740992L)]
    [InlineData(-9007199254740992L)]
    public void PtsNotExactlyRepresentableBySelect_IsRejected(long pts)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FfmpegFramePreviewRunner.BuildArguments(
            "video.mp4", 0, new VideoTimeBase(1, 1000), pts));
    }

    [Theory]
    [InlineData("1/1000")]
    [InlineData("2/2000")]
    public void Evidence_RequiresMatchingPtsAndEquivalentTimeBase(string timeBase)
    {
        var log = $"[showinfo] config in time_base: {timeBase}, frame_rate: 25/1\n" +
            "[showinfo] n: 0 pts: 582160 pts_time:582.16 fmt:yuv420p";
        FfmpegFramePreviewRunner.ValidateEvidence(log, 582160, new VideoTimeBase(1, 1000));
    }

    [Theory]
    [InlineData(582159, "1/1000")]
    [InlineData(582160, "1/90000")]
    public void Evidence_StaleFrameOrWrongTimeBaseIsRejected(long actualPts, string timeBase)
    {
        var log = $"[showinfo] config in time_base: {timeBase}, frame_rate: 25/1\n" +
            $"[showinfo] n: 0 pts: {actualPts} pts_time:582.16 fmt:yuv420p";
        Assert.Throws<InvalidOperationException>(() => FfmpegFramePreviewRunner.ValidateEvidence(
            log, 582160, new VideoTimeBase(1, 1000)));
    }

    [Fact]
    public void Evidence_MultipleMatchingFramesIsRejected()
    {
        const string log = "[showinfo] config in time_base: 1/1000, frame_rate: 25/1\n" +
            "[showinfo] n: 0 pts: 40 pts_time:0.04 fmt:yuv420p\n" +
            "[showinfo] n: 1 pts: 40 pts_time:0.04 fmt:yuv420p";
        Assert.Throws<InvalidOperationException>(() => FfmpegFramePreviewRunner.ValidateEvidence(
            log, 40, new VideoTimeBase(1, 1000)));
    }

    [Fact]
    public void Evidence_NoFrameOutputIsRejected()
    {
        Assert.Throws<InvalidOperationException>(() => FfmpegFramePreviewRunner.ValidateEvidence(
            "Output file is empty", 40, new VideoTimeBase(1, 1000)));
    }

    [Fact]
    public void Probe_AllowsNegativeRawSourceTimes()
    {
        var arguments = FfprobeFrameRunner.BuildArguments(
            "video.mp4", 0, TimeSpan.FromSeconds(-2), TimeSpan.FromSeconds(2));
        Assert.Equal("-2%2", arguments[5]);
    }
}
