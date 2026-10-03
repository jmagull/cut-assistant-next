using System.Globalization;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class FfprobeFrameAnalysisTests
{
    [Fact]
    public void Parser_KeepsLargeRawPtsAndOutputOrderSeparateFromDts()
    {
        const string json = """
            {
              "streams": [{"index":2,"codec_type":"video","time_base":"1/90000","start_time":"2.000000"}],
              "format": {"start_time":"1.500000"},
              "frames": [
                {"media_type":"video","stream_index":2,"pts":9007199254740993,
                 "best_effort_timestamp":9007199254740993,"pkt_dts":9007199254741000,
                 "duration":3600,"key_frame":1,"pict_type":"I"},
                {"media_type":"video","stream_index":2,"pts":"9007199254744593",
                 "pkt_dts":9007199254740000,"key_frame":0,"pict_type":"B"}
              ]
            }
            """;

        var window = FfprobeFrameJsonParser.Parse(json, 2);
        Assert.Equal(2, window.StreamIndex);
        Assert.Equal(1, window.TimeBase.Numerator);
        Assert.Equal(90000, window.TimeBase.Denominator);
        Assert.Equal(2m, window.StreamStartTimeSeconds);
        Assert.Equal(1.5m, window.ContainerStartTimeSeconds);
        Assert.Equal(9007199254740993L, window.Frames[0].Pts);
        Assert.Equal(0, window.Frames[0].LocalIndex);
        Assert.True(window.Frames[0].IsKeyFrame);
        Assert.Equal(1, window.Frames[1].LocalIndex);
        Assert.Equal("B", window.Frames[1].PictureType);
        Assert.False(window.Frames[1].IsKeyFrame);
        Assert.Equal(0.04m, window.TimeBase.ToSeconds(
            window.Frames[1].Pts!.Value - window.Frames[0].Pts!.Value));
    }

    [Fact]
    public void Parser_MissingPtsDoesNotPromoteBestEffortToOriginalPts()
    {
        const string json = """
            {"streams":[{"index":0,"codec_type":"video","time_base":"1/1000"}],
             "frames":[{"media_type":"video","stream_index":0,"pts":"N/A",
                        "best_effort_timestamp":-40,"pkt_dts":"N/A","pkt_duration":40}]}
            """;

        var window = FfprobeFrameJsonParser.Parse(json, 0);
        var frame = Assert.Single(window.Frames);
        Assert.Null(frame.Pts);
        Assert.Null(frame.PacketDts);
        Assert.Null(frame.IsKeyFrame);
        Assert.Equal(-40, frame.BestEffortTimestamp);
        Assert.Equal(40, frame.Duration);
        Assert.Equal(-0.04m, window.TimeBase.ToSeconds(frame.BestEffortTimestamp!.Value));
        Assert.Null(window.StreamStartTimeSeconds);
        Assert.Null(window.ContainerStartTimeSeconds);
    }

    [Theory]
    [InlineData("0/1000")]
    [InlineData("1/0")]
    [InlineData("N/A")]
    public void Parser_InvalidTimeBaseIsRejected(string timeBase)
    {
        var json = "{\"streams\":[{\"index\":0,\"codec_type\":\"video\",\"time_base\":\"" + timeBase + "\"}]}";
        Assert.Throws<FormatException>(() => FfprobeFrameJsonParser.Parse(json, 0));
    }

    [Fact]
    public void Parser_UnexpectedFrameStreamIsRejected()
    {
        const string json = """
            {"streams":[{"index":2,"codec_type":"video","time_base":"1/90000"}],
             "frames":[{"media_type":"video","stream_index":0,"pts":0}]}
            """;
        Assert.Throws<FormatException>(() => FfprobeFrameJsonParser.Parse(json, 2));
    }

    [Fact]
    public void Parser_EmptyWindowIsNotAFabricatedFrame()
    {
        const string json = """
            {"streams":[{"index":0,"codec_type":"video","time_base":"1/1000"}],"frames":[]}
            """;
        Assert.Empty(FfprobeFrameJsonParser.Parse(json, 0).Frames);
    }

    [Fact]
    public void Arguments_UseAbsoluteStreamAndFixedIntervalEndWithInvariantDecimals()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var arguments = FfprobeFrameRunner.BuildArguments(
                "video with spaces.mp4", 2, TimeSpan.FromSeconds(582.16), TimeSpan.FromSeconds(587.16));
            Assert.Equal("2", arguments[3]);
            Assert.Equal("582.16%587.16", arguments[5]);
            Assert.Equal("video with spaces.mp4", arguments[^1]);
            Assert.Contains("-show_frames", arguments);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Theory]
    [InlineData(-1, 0, 10)]
    [InlineData(0, 10, 10)]
    [InlineData(0, 10, 9)]
    public void Arguments_InvalidRequestIsRejected(int stream, double start, double end)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FfprobeFrameRunner.BuildArguments(
            "video.mp4", stream, TimeSpan.FromSeconds(start), TimeSpan.FromSeconds(end)));
    }

    [Fact]
    public async Task Runner_PreCancelledRequestDoesNotStartProcess()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var runner = new FfprobeFrameRunner("missing-ffprobe.exe");
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runner.RunAsync(
            "video.mp4", 0, TimeSpan.Zero, TimeSpan.FromSeconds(1), cancellation.Token));
    }

    [Fact]
    public async Task Runner_MissingToolReportsItsPath()
    {
        var runner = new FfprobeFrameRunner("missing-frame-analysis-ffprobe.exe");
        var error = await Assert.ThrowsAsync<FileNotFoundException>(() => runner.RunAsync(
            "video.mp4", 0, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
        Assert.Equal("missing-frame-analysis-ffprobe.exe", error.FileName);
    }
}
