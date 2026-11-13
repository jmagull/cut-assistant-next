using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using CutAssistantNext.Media.Cutting;

namespace CutAssistantNext.Media.Tests.Cutting;

public sealed class Mp4BoxRunnerConcatIntegrationTests
{
    private const string Mp4BoxVariable = "CAN_MP4BOX_TEST_EXE";
    private const string FfmpegVariable = "CAN_FFMPEG_TEST_EXE";
    private const string FfprobeVariable = "CAN_FFPROBE_TEST_EXE";

    [Mp4BoxIntegrationTheory]
    [InlineData("aac", null)]
    [InlineData("aac", "ac3")]
    [InlineData("aac", "aac")]
    [InlineData("ac3", "ac3")]
    public async Task ConcatenatesThreeSegmentsWithAllStreamsInOriginalOrder(string firstCodec, string? secondCodec)
    {
        var directory = Directory.CreateTempSubdirectory("can-concat-media-");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var token = timeout.Token;

        try
        {
            var paths = new List<string>();
            var inputs = new List<Track[]>();
            for (var part = 0; part < 3; part++)
            {
                var path = Path.Combine(directory.FullName, $"part {part + 1}.mp4");
                await CreateSegmentAsync(path, part, firstCodec, secondCodec, token);
                paths.Add(path);
                inputs.Add(await ReadTracksAsync(path, token));
            }

            var outputPath = Path.Combine(directory.FullName, "joined.mp4");
            await new Mp4BoxRunner(GetTool(Mp4BoxVariable)).RunConcatAsync(paths, outputPath, token);
            var output = await ReadTracksAsync(outputPath, token);

            Assert.Equal(secondCodec is null ? 2 : 3, output.Length);
            Assert.Equal("h264", output[0].Codec);
            Assert.Equal(firstCodec, output[1].Codec);
            Assert.Equal("deu", output[1].Language);
            if (secondCodec is not null)
            {
                Assert.Equal(secondCodec, output[2].Codec);
                Assert.Equal(firstCodec == secondCodec ? "deu" : "eng", output[2].Language);
            }

            for (var index = 0; index < output.Length; index++)
            {
                var track = output[index];
                var expected = inputs.Select(part => part[index]).ToArray();
                Assert.All(expected, part => Assert.Equal((track.Type, track.Codec, track.Language),
                    (part.Type, part.Codec, part.Language)));
                Assert.NotEmpty(track.Packets);
                Assert.Equal(expected.SelectMany(part => part.Packets.Select(packet => packet.Hash)),
                    track.Packets.Select(packet => packet.Hash));

                var expectedDuration = expected.Sum(part => part.Duration);
                Assert.InRange(track.Duration, expectedDuration - 0.12, expectedDuration + 0.12);
                for (var packet = 1; packet < track.Packets.Length; packet++)
                {
                    Assert.True(track.Packets[packet].Dts > track.Packets[packet - 1].Dts);
                    Assert.True(track.Packets[packet].Pts > track.Packets[packet - 1].Pts);
                }
            }
        }
        finally
        {
            foreach (var path in Directory.EnumerateFiles(directory.FullName))
                File.Delete(path);
            directory.Delete();
        }
    }

    private static async Task CreateSegmentAsync(string path, int part, string firstCodec, string? secondCodec,
        CancellationToken token)
    {
        var args = new List<string>
        {
            "-nostdin", "-v", "error", "-n",
            "-f", "lavfi", "-i", "testsrc2=size=96x64:rate=25",
            "-f", "lavfi", "-i", $"sine=frequency={400 + part * 41}:sample_rate=48000"
        };
        if (secondCodec is not null)
            args.AddRange(["-f", "lavfi", "-i", $"sine=frequency={710 + part * 53}:sample_rate=48000"]);

        args.AddRange(["-map", "0:v", "-map", "1:a"]);
        if (secondCodec is not null)
            args.AddRange(["-map", "2:a"]);

        args.AddRange(["-t", "1.28", "-vf", $"hue=h={part * 90}", "-c:v", "libx264",
            "-preset", "ultrafast", "-g", "16", "-bf", "0", "-threads:v", "1",
            "-c:a:0", firstCodec, "-metadata:s:a:0", "language=deu"]);
        if (secondCodec is not null)
            args.AddRange(["-c:a:1", secondCodec, "-metadata:s:a:1",
                firstCodec == secondCodec ? "language=deu" : "language=eng"]);

        args.AddRange(["-use_editlist", "0", path]);
        await RunAsync(GetTool(FfmpegVariable), args, token);
    }

    private static async Task<Track[]> ReadTracksAsync(string path, CancellationToken token)
    {
        var result = await RunAsync(GetTool(FfprobeVariable),
            ["-v", "error", "-show_packets", "-show_data_hash", "sha256",
                "-show_entries", "stream=index,codec_type,codec_name,duration:stream_tags=language:packet=stream_index,pts,dts,data_hash",
                "-of", "json", path], token);
        using var json = JsonDocument.Parse(result);
        var packets = json.RootElement.GetProperty("packets").EnumerateArray().ToLookup(
            packet => packet.GetProperty("stream_index").GetInt32(),
            packet => new Packet(packet.GetProperty("pts").GetInt64(), packet.GetProperty("dts").GetInt64(),
                packet.GetProperty("data_hash").GetString()!));
        return json.RootElement.GetProperty("streams").EnumerateArray().Select(track => new Track(
            track.GetProperty("codec_type").GetString()!,
            track.GetProperty("codec_name").GetString()!,
            track.GetProperty("tags").GetProperty("language").GetString()!,
            double.Parse(track.GetProperty("duration").GetString()!, CultureInfo.InvariantCulture),
            packets[track.GetProperty("index").GetInt32()].ToArray())).ToArray();
    }

    private static async Task<string> RunAsync(string executable, IEnumerable<string> arguments,
        CancellationToken token)
    {
        var start = new ProcessStartInfo(executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var argument in arguments)
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start) ?? throw new InvalidOperationException("Prüfwerkzeug konnte nicht starten.");
        var stdout = process.StandardOutput.ReadToEndAsync(token);
        var stderr = process.StandardError.ReadToEndAsync(token);
        try
        {
            await process.WaitForExitAsync(token);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync();
            throw;
        }

        var error = await stderr;
        Assert.True(process.ExitCode == 0, $"{Path.GetFileName(executable)}: Exit-Code {process.ExitCode}\n{error}");
        return await stdout;
    }

    private static string GetTool(string variable) => Environment.GetEnvironmentVariable(variable)!;

    private sealed record Packet(long Pts, long Dts, string Hash);
    private sealed record Track(string Type, string Codec, string Language, double Duration, Packet[] Packets);

    private sealed class Mp4BoxIntegrationTheoryAttribute : TheoryAttribute
    {
        public Mp4BoxIntegrationTheoryAttribute()
        {
            if (new[] { Mp4BoxVariable, FfmpegVariable, FfprobeVariable }.Any(
                name => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name))))
                Skip = "Set CAN_MP4BOX_TEST_EXE, CAN_FFMPEG_TEST_EXE and CAN_FFPROBE_TEST_EXE to run the native matrix.";
        }
    }
}
