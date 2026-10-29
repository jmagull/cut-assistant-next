using System.Text.Json.Nodes;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Joining;

namespace CutAssistantNext.Media.Tests.Analysis;

public sealed class Mp4JoinTests : IDisposable
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WorkspaceCleanupRemovesProtocolOnlyAfterSuccess(bool completed)
    {
        var work = Path.Combine(_root, "work");
        Directory.CreateDirectory(work);
        var protocol = Path.Combine(work, "protocol.txt");
        File.WriteAllText(protocol, "diagnostic details");
        File.WriteAllText(Path.Combine(work, "joined.wav"), "temporary audio");
        var output = Path.Combine(_root, "finished.mp4");
        File.WriteAllText(output, "verified movie");

        Mp4JoinService.CleanupWorkspace(work, completed, null);

        Assert.Equal("verified movie", File.ReadAllText(output));
        Assert.Equal(!completed, Directory.Exists(work));
        if (!completed)
        {
            Assert.Equal("diagnostic details", File.ReadAllText(protocol));
            Assert.Single(Directory.GetFiles(work));
        }
    }

    [Fact]
    public void WorkspaceCleanupKeepsDiagnosticWhenTemporaryFileIsLocked()
    {
        var work = Path.Combine(_root, "locked");
        Directory.CreateDirectory(work);
        var protocol = Path.Combine(work, "protocol.txt");
        File.WriteAllText(protocol, "diagnostic details");
        var temporary = Path.Combine(work, "joined.wav");
        using var locked = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None);

        Mp4JoinService.CleanupWorkspace(work, true, null);

        Assert.Equal("diagnostic details", File.ReadAllText(protocol));
        Assert.True(File.Exists(temporary));
    }

    [Fact]
    public void WorkspaceCleanupNeverRecursivelyDeletesUnexpectedContents()
    {
        var work = Path.Combine(_root, "unexpected");
        Directory.CreateDirectory(work);
        var other = Path.Combine(work, "other-data");
        Directory.CreateDirectory(other);
        File.WriteAllText(Path.Combine(other, "keep.txt"), "preserve");
        var protocol = Path.Combine(work, "protocol.txt");
        File.WriteAllText(protocol, "diagnostic details");

        Mp4JoinService.CleanupWorkspace(work, true, null);

        Assert.Equal("preserve", File.ReadAllText(Path.Combine(other, "keep.txt")));
        Assert.Equal("diagnostic details", File.ReadAllText(protocol));
    }

    private static FfprobeStreamCopyVerifier.CopyPacket Audio(long pts, long skip = 0, long discard = 0) =>
        new(pts, pts, 1024, 100, "SHA256:" + new string('a', 64), "K", skip, discard, "");

    [Fact]
    public void SourceAudioClockAcceptsPrimingAndBothFinalPaddingDurationConventions()
    {
        foreach (var trimmed in new[] { false, true })
        {
            var clock = new FfprobeStreamCopyVerifier.JoinAudioClock(1500);
            clock.Add(Audio(-1024, skip: 1024));
            clock.Add(Audio(0));
            clock.Add(Audio(1024, discard: 548) with { Duration = trimmed ? 476 : 1024 });
            clock.Complete();
        }
    }

    [Theory]
    [InlineData(1023)]
    [InlineData(1025)]
    public void SourceAudioClockRejectsGapsOrOverlapsEvenIfTotalSampleCountCouldMatch(long next)
    {
        var clock = new FfprobeStreamCopyVerifier.JoinAudioClock(2048);
        clock.Add(Audio(0));
        Assert.Throws<InvalidDataException>(() => clock.Add(Audio(next)));
    }

    [Theory]
    [InlineData("start")]
    [InlineData("skip-inside")]
    [InlineData("discard-inside")]
    [InlineData("end")]
    public void SourceAudioClockRejectsUnexplainedStartInteriorPaddingOrEnd(string error)
    {
        var clock = new FfprobeStreamCopyVerifier.JoinAudioClock(2048);
        Assert.Throws<InvalidDataException>(() =>
        {
            clock.Add(Audio(error == "start" ? 1 : 0, discard: error == "discard-inside" ? 1 : 0));
            clock.Add(Audio(1024, skip: error == "skip-inside" ? 1 : 0));
            if (error == "end") clock.Add(Audio(2048));
            clock.Complete();
        });
    }
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-join-test-" + Guid.NewGuid().ToString("N"));
    public Mp4JoinTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, true);

    private static JsonObject Metadata() => JsonNode.Parse("""
        {"format":{"format_name":"mov,mp4,m4a,3gp,3g2,mj2"},"streams":[
        {"index":0,"codec_type":"video","codec_name":"h264","profile":"High","level":40,
        "width":1920,"height":1080,"pix_fmt":"yuv420p","sample_aspect_ratio":"1:1",
        "r_frame_rate":"25/1","avg_frame_rate":"25/1","field_order":"progressive",
        "time_base":"1/12800","start_pts":0,"duration_ts":25600,"extradata_hash":"SHA256:abc"},
        {"index":1,"codec_type":"audio","codec_name":"aac","sample_rate":"48000","channels":2,
        "channel_layout":"stereo","time_base":"1/48000","start_pts":0,"duration_ts":96000,"tags":{"language":"deu"}}]}
        """)!.AsObject();

    [Fact]
    public void UsesExactVideoAndAudioClocksRatherThanRoundedContainerDuration()
    {
        var part = FfprobeStreamCopyVerifier.ParseJoinPart("part.mp4", 1, Metadata().ToJsonString());
        Assert.Equal(TimeSpan.FromSeconds(2), part.Duration);
        Assert.Equal(96000, part.AudioSamples);
        Assert.Equal("deu", part.Language);
    }

    [Theory]
    [InlineData("r_frame_rate", "30000/1001")]
    [InlineData("avg_frame_rate", "24/1")]
    [InlineData("time_base", "0/12800")]
    [InlineData("field_order", "tt")]
    [InlineData("codec_name", "hevc")]
    [InlineData("extradata_hash", "")]
    public void RejectsUnsupportedOrUnprovenVideo(string key, string value)
    {
        var json = Metadata(); json["streams"]![0]![key] = value;
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ParseJoinPart("part.mp4", 1, json.ToJsonString()));
    }

    [Theory]
    [InlineData(0, "start_pts", "1")]
    [InlineData(1, "start_pts", "1")]
    [InlineData(1, "duration_ts", "95999")]
    [InlineData(1, "sample_rate", "32000")]
    [InlineData(1, "channel_layout", "unknown")]
    [InlineData(1, "codec_name", "mp3")]
    public void RejectsOffsetTruncationOrUnsupportedAudio(int index, string key, string value)
    {
        var json = Metadata(); json["streams"]![index]![key] = value;
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ParseJoinPart("part.mp4", 1, json.ToJsonString()));
    }

    [Fact]
    public void MissingChosenTrackIsNeverReplacedByFirstTrack()
    {
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ParseJoinPart("part.mp4", 2, Metadata().ToJsonString()));
    }

    [Fact]
    public void RejectsUnaccountedVideoRotationOrOtherStreamSideData()
    {
        var json = Metadata();
        json["streams"]![0]!["side_data_list"] = JsonNode.Parse("[{\"side_data_type\":\"Display Matrix\",\"rotation\":90}]");
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ParseJoinPart("part.mp4", 1, json.ToJsonString()));
    }

    [Fact]
    public void CompatibleDifferentTimeBasesUseTheSamePresentationDuration()
    {
        var first = FfprobeStreamCopyVerifier.ParseJoinPart("one.mp4", 1, Metadata().ToJsonString());
        var json = Metadata(); json["streams"]![0]!["time_base"] = "1/25600"; json["streams"]![0]!["duration_ts"] = 51200;
        var second = FfprobeStreamCopyVerifier.ParseJoinPart("two.mp4", 1, json.ToJsonString());
        FfprobeStreamCopyVerifier.ValidateJoinCompatibility(first, second);
        Assert.Equal(first.Duration, second.Duration);
    }

    [Theory]
    [InlineData("video")]
    [InlineData("rate")]
    [InlineData("layout")]
    public void RejectsIncompatibleParts(string change)
    {
        var first = FfprobeStreamCopyVerifier.ParseJoinPart("one.mp4", 1, Metadata().ToJsonString());
        var second = change switch
        {
            "video" => first with { VideoConfiguration = "different" },
            "rate" => first with { SampleRate = 44100 },
            _ => first with { Layout = "5.1", Channels = 6 }
        };
        Assert.Throws<InvalidDataException>(() => FfprobeStreamCopyVerifier.ValidateJoinCompatibility(first, second));
    }

    private static JoinPart PcmPart(int channels = 2, string layout = "stereo", int rate = 48000) =>
        new("unused", 1, 0, 12800, 0, 25, rate, channels, layout, "deu", "unused", 0);

    [Fact]
    public async Task DirectWaveRetainsEverySampleAtTheBoundaryAndTheOriginals()
    {
        var first = Path.Combine(_root, "one.pcm"); var second = Path.Combine(_root, "two.pcm");
        var output = Path.Combine(_root, "result.wav");
        var a = Enumerable.Range(0, 4096).Select(i => (byte)(i % 251)).ToArray();
        byte[] b = [99, 98, 97, 96, 95, 94, 93, 92];
        await File.WriteAllBytesAsync(first, a); await File.WriteAllBytesAsync(second, b);
        await Mp4JoinService.JoinWaveAsync([first, second], output, PcmPart(), (a.Length + b.Length) / 8, CancellationToken.None);
        Assert.Equal(a.Concat(b), (await File.ReadAllBytesAsync(output)).Skip(104));
        Assert.Equal(a, await File.ReadAllBytesAsync(first));
        Assert.Equal(b, await File.ReadAllBytesAsync(second));
    }

    [Theory]
    [InlineData(1, "mono", 4u)]
    [InlineData(2, "stereo", 3u)]
    [InlineData(6, "5.1", 63u)]
    [InlineData(6, "5.1(side)", 1551u)]
    public void Rf64HeaderRetains64BitSizesSamplesAndExplicitChannelMask(int channels, string layout, uint mask)
    {
        var bytes = ((long)uint.MaxValue / (channels * 4) + 1) * channels * 4;
        var header = Mp4JoinService.WaveHeader(bytes, PcmPart(channels, layout));
        Assert.Equal(104, header.Length);
        Assert.Equal("RF64", System.Text.Encoding.ASCII.GetString(header, 0, 4));
        Assert.Equal(uint.MaxValue, BitConverter.ToUInt32(header, 4));
        Assert.Equal((ulong)bytes + 96, BitConverter.ToUInt64(header, 20));
        Assert.Equal((ulong)bytes, BitConverter.ToUInt64(header, 28));
        Assert.Equal((ulong)bytes / (uint)(channels * 4), BitConverter.ToUInt64(header, 36));
        Assert.Equal(48000u, BitConverter.ToUInt32(header, 60));
        Assert.Equal(mask, BitConverter.ToUInt32(header, 76));
        Assert.Equal(uint.MaxValue, BitConverter.ToUInt32(header, 100));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("extra")]
    [InlineData("unaligned")]
    [InlineData("missing")]
    public async Task DirectWaveRejectsMissingOrIncorrectPcmData(string kind)
    {
        var a = Path.Combine(_root, "a.pcm"); var b = Path.Combine(_root, "b.pcm");
        File.WriteAllBytes(a, new byte[8]);
        if (kind != "missing") File.WriteAllBytes(b, new byte[kind == "short" ? 0 : kind == "extra" ? 16 : 7]);
        var operation = () => Mp4JoinService.JoinWaveAsync([a, b], Path.Combine(_root, "bad.wav"), PcmPart(), 2, CancellationToken.None);
        if (kind == "missing") await Assert.ThrowsAsync<FileNotFoundException>(operation);
        else await Assert.ThrowsAsync<InvalidDataException>(operation);
    }

    [Theory]
    [InlineData("header")]
    [InlineData("payload")]
    [InlineData("truncated")]
    [InlineData("appended")]
    public async Task IndependentWaveReadbackRejectsChangedHeaderPayloadOrLength(string change)
    {
        var a = Path.Combine(_root, "a.pcm"); var b = Path.Combine(_root, "b.pcm"); var output = Path.Combine(_root, "out.wav");
        File.WriteAllBytes(a, new byte[8]); File.WriteAllBytes(b, new byte[8]);
        await Mp4JoinService.JoinWaveAsync([a, b], output, PcmPart(), 2, CancellationToken.None);
        var data = File.ReadAllBytes(output);
        if (change == "header") data[76] ^= 1;
        if (change == "payload") data[^1] ^= 1;
        if (change == "truncated") data = data[..^1];
        if (change == "appended") data = [.. data, 0];
        File.WriteAllBytes(output, data);
        await Assert.ThrowsAsync<InvalidDataException>(() => Mp4JoinService.VerifyWaveAsync(output, PcmPart(), 2,
            System.Security.Cryptography.SHA256.HashData(new byte[16]), CancellationToken.None));
    }

    [Fact]
    public async Task DirectWaveDoesNotOverwriteAndHonoursCancellationBeforeCreatingOutput()
    {
        var a = Path.Combine(_root, "a.pcm"); var b = Path.Combine(_root, "b.pcm"); var existing = Path.Combine(_root, "existing.wav");
        File.WriteAllBytes(a, new byte[8]); File.WriteAllBytes(b, new byte[8]); File.WriteAllText(existing, "keep");
        await Assert.ThrowsAsync<IOException>(() => Mp4JoinService.JoinWaveAsync([a, b], existing, PcmPart(), 2, CancellationToken.None));
        Assert.Equal("keep", File.ReadAllText(existing));
        using var cancel = new CancellationTokenSource(); cancel.Cancel();
        var output = Path.Combine(_root, "cancelled.wav");
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Mp4JoinService.JoinWaveAsync([a, b], output, PcmPart(), 2, cancel.Token));
        Assert.False(File.Exists(output));
        await Assert.ThrowsAsync<OverflowException>(() =>
            Mp4JoinService.JoinWaveAsync([a, b], output, PcmPart(), long.MaxValue, CancellationToken.None));
    }

    [Theory]
    [InlineData("same-input")]
    [InlineData("source-output")]
    [InlineData("exists")]
    [InlineData("missing")]
    [InlineData("extension")]
    [InlineData("relative")]
    public void InvalidOrExistingPathsAreRejectedBeforeWorkStarts(string kind)
    {
        var first = Path.Combine(_root, "one.mp4"); var second = Path.Combine(_root, "two.mp4");
        var output = Path.Combine(_root, "out.mp4"); File.WriteAllText(first, "one"); File.WriteAllText(second, "two");
        if (kind == "same-input") second = first;
        if (kind == "source-output") output = first;
        if (kind == "exists") File.WriteAllText(output, "keep");
        if (kind == "missing") File.Delete(second);
        if (kind == "extension") output += ".avi";
        if (kind == "relative") output = "relative.mp4";
        Assert.ThrowsAny<Exception>(() => Mp4JoinService.ValidatePaths(new(first, 1, second, 1, output)));
        Assert.Equal("one", File.ReadAllText(first));
    }

    [Fact]
    public void DiskEstimateIncludesUncompressedAudioAndDetectsOverflow()
    {
        Assert.True(Mp4JoinService.RequiredFreeBytes(100, 200, 1000000) > 3000000);
        Assert.Throws<OverflowException>(() => Mp4JoinService.RequiredFreeBytes(long.MaxValue, 1, 0));
        Assert.Equal("C:/Film'\\''s/Teil 1.mp4", Mp4JoinService.EscapeConcatPath(@"C:\Film's\Teil 1.mp4"));
    }
}
