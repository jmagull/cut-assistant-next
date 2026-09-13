using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class VideoPreparationTests
{
    private static MediaAnalysisResult Sample(string format = "avi") => new(
        format, null, 1000, TimeSpan.FromSeconds(10),
        [new VideoStreamInfo(0, "h264", null, 640, 360, null, null, 25, null)
            { StartTimeSeconds = 0, FrameCount = 250 }],
        [new AudioStreamInfo(1, "mp3", null, 48000, 2, null) { StartTimeSeconds = 0 }])
        { StartTimeSeconds = 0 };

    [Theory]
    [InlineData("mov,mp4,m4a,3gp,3g2,mj2", true)]
    [InlineData("avi", false)]
    [InlineData("matroska,webm", false)]
    [InlineData(null, false)]
    public void RoutingUsesDetectedContainer(string? format, bool direct) =>
        Assert.Equal(direct, VideoPreparation.IsMp4(Sample() with { FormatName = format }));

    [Fact]
    public void UnchangedStreamsAndTimelinePass() =>
        VideoPreparation.Validate(Sample(), Sample("mp4"));

    [Fact]
    public void ShiftedAudioIsRejected()
    {
        var prepared = Sample("mp4");
        prepared = prepared with { AudioStreams = [prepared.AudioStreams[0] with { StartTimeSeconds = 0.5 }] };
        Assert.Throws<InvalidOperationException>(() => VideoPreparation.Validate(Sample(), prepared));
    }

    [Fact]
    public void LostFramesAreRejected()
    {
        var prepared = Sample("mp4");
        prepared = prepared with { VideoStreams = [prepared.VideoStreams[0] with { FrameCount = 247 }] };
        Assert.Throws<InvalidOperationException>(() => VideoPreparation.Validate(Sample(), prepared));
    }

    [Fact]
    public void ChangedDurationIsRejected() =>
        Assert.Throws<InvalidOperationException>(() => VideoPreparation.Validate(Sample(),
            Sample("mp4") with { Duration = TimeSpan.FromSeconds(11) }));

    [Fact]
    public void LostAudioIsRejected() =>
        Assert.Throws<InvalidOperationException>(() => VideoPreparation.Validate(Sample(),
            Sample("mp4") with { AudioStreams = [] }));

    [Fact]
    public void MissingTimeOriginIsReportedWithoutBlocking()
    {
        var details = new List<string>();
        VideoPreparation.Validate(Sample(), Sample("mp4") with { StartTimeSeconds = null }, details.Add);
        Assert.Contains(details, line => line.Contains("Startzeit nicht vergleichbar"));
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void SmallFrameDifferencesAreReportedWithoutBlocking(int difference)
    {
        var prepared = Sample("mp4");
        prepared = prepared with { VideoStreams = [prepared.VideoStreams[0] with
            { FrameCount = 250 + difference, FramesPerSecond = (250 + difference) / 10.0 }] };
        var details = new List<string>();
        VideoPreparation.Validate(Sample(), prepared, details.Add);
        Assert.Contains(details, line => line.Contains("Frame-Angaben"));
        Assert.Contains(details, line => line.Contains("Mittlere Bildrate"));
    }

    [Fact]
    public void SignificantFrameRateDriftStillBlocks()
    {
        var prepared = Sample("mp4");
        prepared = prepared with { VideoStreams = [prepared.VideoStreams[0] with { FramesPerSecond = 24 }] };
        Assert.Throws<InvalidOperationException>(() => VideoPreparation.Validate(Sample(), prepared));
    }

    [Fact]
    public void DiplomatinTwoFramesWithLongerContainerDurationPasses()
    {
        var original = Sample() with
        {
            Duration = TimeSpan.FromSeconds(6347.98),
            VideoStreams = [Sample().VideoStreams[0] with
                { FramesPerSecond = 50, FrameCount = 317399 }]
        };
        var prepared = original with
        {
            FormatName = "mp4",
            Duration = TimeSpan.FromSeconds(6348),
            VideoStreams = [original.VideoStreams[0] with
                { FramesPerSecond = 15869850.0 / 317399, FrameCount = 317397, StartTimeSeconds = 0.02 }]
        };
        var details = new List<string>();
        VideoPreparation.Validate(original, prepared, details.Add);
        Assert.Contains(details, line => line.Contains("Frame-Angaben"));
        Assert.Contains(details, line => line.Contains("Mittlere Bildrate"));
    }

    [Fact]
    public void MissingFrameCountIsReportedWithoutBlocking()
    {
        var prepared = Sample("mp4");
        prepared = prepared with { VideoStreams = [prepared.VideoStreams[0] with { FrameCount = null }] };
        var details = new List<string>();
        VideoPreparation.Validate(Sample(), prepared, details.Add);
        Assert.Contains(details, line => line.Contains("Frame-Anzahl nicht vergleichbar"));
    }

    [Fact]
    public void ParserReadsTimingAndFrameCount()
    {
        var parsed = FfprobeJsonParser.Parse("""
            {"format":{"start_time":"0.5"},"streams":[
            {"codec_type":"video","start_time":"0.5","nb_frames":"250"},
            {"codec_type":"audio","start_time":"0.52"}]}
            """);
        Assert.Equal(0.5, parsed.StartTimeSeconds);
        Assert.Equal(0.5, parsed.VideoStreams[0].StartTimeSeconds);
        Assert.Equal(250, parsed.VideoStreams[0].FrameCount);
        Assert.Equal(0.52, parsed.AudioStreams[0].StartTimeSeconds);
    }
}
