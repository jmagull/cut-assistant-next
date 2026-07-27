using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests;

public class FfprobeJsonParserTests
{
    [Fact]
    public void Parse_ReadsVideoAudioAndFormatInformation()
    {
        const string json = """
        {
          "streams": [
            {
              "index": 0,
              "codec_name": "h264",
              "codec_long_name": "H.264 / AVC",
              "codec_type": "video",
              "width": 720,
              "height": 576,
              "sample_aspect_ratio": "16:15",
              "display_aspect_ratio": "4:3",
              "avg_frame_rate": "25/1",
              "field_order": "progressive"
            },
            {
              "index": 1,
              "codec_name": "aac",
              "codec_long_name": "AAC",
              "codec_type": "audio",
              "sample_rate": "48000",
              "channels": 2,
              "channel_layout": "stereo"
            }
          ],
          "format": {
            "format_name": "mov,mp4,m4a,3gp,3g2,mj2",
            "format_long_name": "QuickTime / MOV",
            "duration": "3740.180000",
            "size": "734003200"
          }
        }
        """;

        var result = FfprobeJsonParser.Parse(json);

        Assert.Equal("mov,mp4,m4a,3gp,3g2,mj2", result.FormatName);
        Assert.Equal("QuickTime / MOV", result.FormatLongName);
        Assert.Equal(734003200, result.FileSizeBytes);
        Assert.Equal(TimeSpan.FromSeconds(3740.18), result.Duration);

        var video = Assert.Single(result.VideoStreams);
        Assert.Equal(0, video.Index);
        Assert.Equal("h264", video.CodecName);
        Assert.Equal(720, video.Width);
        Assert.Equal(576, video.Height);
        Assert.Equal("16:15", video.SampleAspectRatio);
        Assert.Equal("4:3", video.DisplayAspectRatio);
        Assert.Equal(25.0, video.FramesPerSecond);
        Assert.Equal("progressive", video.FieldOrder);

        var audio = Assert.Single(result.AudioStreams);
        Assert.Equal(1, audio.Index);
        Assert.Equal("aac", audio.CodecName);
        Assert.Equal(48000, audio.SampleRate);
        Assert.Equal(2, audio.Channels);
        Assert.Equal("stereo", audio.ChannelLayout);
    }
}
