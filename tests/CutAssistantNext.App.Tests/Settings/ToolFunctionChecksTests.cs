using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class ToolFunctionChecksTests
{
    [Fact]
    public async Task ConfiguredNativeToolsVerifyReferenceFixturesAndCacheResult()
    {
        var ffmpeg = Environment.GetEnvironmentVariable("CAN_FFMPEG_TEST_EXE");
        var ffprobe = Environment.GetEnvironmentVariable("CAN_FFPROBE_TEST_EXE");
        var mp4box = Environment.GetEnvironmentVariable("CAN_MP4BOX_TEST_EXE");
        if (ffmpeg is null || ffprobe is null || mp4box is null) return;
        var temp = Directory.CreateTempSubdirectory("can-test-cache-");
        try
        {
            var checks = new ToolFunctionChecks(temp.FullName);
            var settings = new FfmpegSettings { FfmpegExecutablePath = ffmpeg, FfprobeExecutablePath = ffprobe };
            var streamcopy = await checks.CheckFfmpegAsync(settings, force: true, default);
            Assert.True(streamcopy.Compatible, streamcopy.Message);
            Assert.Contains("200 sichtbare", streamcopy.Message);
            var cached = await checks.CheckFfmpegAsync(settings, force: false, default);
            Assert.Contains("gespeicherter Test", cached.Message);
            var mp4 = await checks.CheckMp4BoxAsync(mp4box, settings, force: true, default);
            Assert.True(mp4.Compatible, mp4.Message);

            var fixedFfmpeg = Environment.GetEnvironmentVariable("CAN_FFMPEG_FIXED_TEST_EXE");
            var fixedProbe = Environment.GetEnvironmentVariable("CAN_FFPROBE_FIXED_TEST_EXE");
            if (fixedFfmpeg is not null && fixedProbe is not null)
            {
                var fixedSettings = new FfmpegSettings { FfmpegExecutablePath = fixedFfmpeg, FfprobeExecutablePath = fixedProbe };
                var fixedResult = await checks.CheckFfmpegAsync(fixedSettings, force: true, default);
                Assert.True(fixedResult.Compatible, fixedResult.Message);
                Assert.Contains("200 sichtbare", fixedResult.Message);
            }

            var bad = Environment.GetEnvironmentVariable("CAN_FFMPEG_BAD_TEST_EXE");
            var badProbe = Environment.GetEnvironmentVariable("CAN_FFPROBE_BAD_TEST_EXE");
            if (bad is null || badProbe is null) return;
            var failing = new FfmpegSettings { FfmpegExecutablePath = bad, FfprobeExecutablePath = badProbe };
            var mismatch = await checks.CheckFfmpegAsync(
                new FfmpegSettings { FfmpegExecutablePath = bad, FfprobeExecutablePath = ffprobe }, true, default);
            Assert.Contains("demselben unterstützten Build", mismatch.Message);
            var failure = await checks.CheckFfmpegAsync(failing, force: true, default);
            Assert.False(failure.Compatible);
            Assert.Contains("50 von 200", failure.Message);
            await Assert.ThrowsAsync<InvalidOperationException>(() => checks.EnsureNoKnownFailureAsync(
                ToolFunction.FfmpegStreamcopy, checks.FfmpegPaths(failing), default));
            await checks.EnsureNoKnownFailureAsync(ToolFunction.FfmpegStreamcopy, checks.FfmpegPaths(settings), default);
        }
        finally
        {
            foreach (var file in temp.EnumerateFiles()) file.Delete();
            temp.Delete();
        }
    }
}
