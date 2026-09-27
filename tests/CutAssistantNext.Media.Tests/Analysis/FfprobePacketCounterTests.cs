using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class FfprobePacketCounterTests
{
    [Fact]
    public void ArgumentsCountVideoPacketsWithoutDecodingFrames()
    {
        var arguments =
            FfprobePacketCounter.BuildArguments(
                @"C:\video files\input.avi");

        Assert.Equal(
            new[]
            {
                "-v", "error",
                "-select_streams", "v",
                "-count_packets",
                "-show_entries", "stream=index,nb_read_packets",
                "-of", "json",
                @"C:\video files\input.avi"
            },
            arguments);

        Assert.DoesNotContain("-count_frames", arguments);
    }

    [Fact]
    public void ParseReadsPacketCountsByStreamIndex()
    {
        var result = FfprobePacketCounter.Parse("""
            {
              "streams": [
                {
                  "index": 0,
                  "nb_read_packets": "224965"
                },
                {
                  "index": 2,
                  "nb_read_packets": "123"
                }
              ]
            }
            """);

        Assert.Equal(224965, result[0]);
        Assert.Equal(123, result[2]);
    }

    [Fact]
    public void ParseRejectsMissingPacketCount()
    {
        Assert.Throws<InvalidOperationException>(() =>
            FfprobePacketCounter.Parse("""
                {
                  "streams": [
                    {
                      "index": 0
                    }
                  ]
                }
                """));
    }

    [Fact]
    public void ParseRejectsDuplicateStreamIndex()
    {
        Assert.Throws<InvalidOperationException>(() =>
            FfprobePacketCounter.Parse("""
                {
                  "streams": [
                    {
                      "index": 0,
                      "nb_read_packets": "100"
                    },
                    {
                      "index": 0,
                      "nb_read_packets": "100"
                    }
                  ]
                }
                """));
    }
}