using System.Reflection;
using System.Text.Json;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

namespace DiagnosisReview;

/// <summary>QA only: invoke the built application's actual cut service, then review its result.</summary>
internal static class IntegrationChecks
{
    public static async Task<bool> RunAsync(ReviewRequest reference, string reportPath, string reportHash,
        string directory, string mp4Box, string ffprobe, CancellationToken token)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new IOException("Nachweisverzeichnis existiert bereits.");
        Directory.CreateDirectory(directory);
        await using var log = new StreamWriter(Path.Combine(directory, "CUT-LOG.txt"), append: false) { AutoFlush = true };
        var progress = new InlineProgress(log);
        try
        {
            using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath, token));
            var cutlist = report.RootElement.GetProperty("Cutlist").Deserialize<ReviewFile>()
                ?? throw new InvalidDataException("Cutlist-Identität fehlt.");
            using var cutlistLock = File.Open(cutlist.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            await cutlist.ValidateAsync(token);
            var keeps = CutlistFileReader.Read(cutlist.Path).Cuts.Select(c => new KeepSegment(c.Start, c.Duration)).ToArray();
            var planned = report.RootElement.GetProperty("Keeps").EnumerateArray().Select(k =>
                new KeepSegment(ReviewRequest.Time(k.GetProperty("Start").GetDecimal()),
                    ReviewRequest.Time(k.GetProperty("Duration").GetDecimal()))).ToArray();
            if (!keeps.Select(k => (k.Start, k.Duration)).SequenceEqual(planned.Select(k => (k.Start, k.Duration))))
                throw new InvalidDataException("Die Cutlist passt nicht mehr zur Referenzdiagnose.");
            var fps = report.RootElement.GetProperty("SourceMetadata").GetProperty("AverageFrameRate").GetString();
            if (fps != "25/1") throw new NotSupportedException("Dieser reale Vergleichslauf benötigt die 25-fps-Referenz.");
            var output = Path.Combine(directory, "INTEGRATED-MP4BOX.mp4");
            var type = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "CutAssistantNext.App.dll"))
                .GetType("CutAssistantNext.App.Services.Cutting.Mp4BoxCutService", throwOnError: true)!;
            var engine = (ICutEngine)Activator.CreateInstance(type,
                new Mp4BoxRunner(mp4Box, progress: progress), new FfprobeCutOutputAnalyzer(ffprobe))!;
            var result = await engine.RunWithResultAsync(new(reference.Source.Path, output, keeps, 25), progress, token);
            Program.SaveNew(Path.Combine(directory, "CUT-RESULT.json"), new
            {
                DiagnosisSha256 = reportHash,
                ActualAppService = type.FullName,
                result.OutputFilePath,
                TimingKind = result.TimingKind.ToString(),
                result.Timeline,
                result.Notice
            });
            if (result.TimingKind != CutReviewTimingKind.Located || !result.Timeline.HasOverlappingPresentationTimes ||
                result.Timeline.Identity is not { } identity ||
                !result.Timeline.Transitions.SequenceEqual(reference.Candidates.Select(c => ReviewRequest.Time(c.Seconds))))
                throw new InvalidDataException("Produktiver Schnitt liefert nicht die zugeordneten Referenz-Prüfstellen.");
            using var outputLock = File.Open(output, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = reference with
            {
                Output = new(output, identity.Length, new DateTime(identity.LastWriteUtcTicks, DateTimeKind.Utc), identity.Sha256),
                Duration = result.Timeline.Duration.Ticks / (decimal)TimeSpan.TicksPerSecond
            };
            await request.Output.ValidateAsync(token);
            using var nativeDeadline = CancellationTokenSource.CreateLinkedTokenSource(token);
            nativeDeadline.CancelAfter(TimeSpan.FromMinutes(4));
            return await NativeReviewChecks.RunAsync(request, reportHash, Path.Combine(directory, "native"), nativeDeadline.Token, result);
        }
        catch (Exception error)
        {
            Program.SaveNew(Path.Combine(directory, "FAILED.json"), new { Error = error.ToString() });
            throw;
        }
    }

    private sealed class InlineProgress(StreamWriter log) : IProgress<CutProgressUpdate>
    {
        public void Report(CutProgressUpdate value)
        {
            lock (log) log.WriteLine($"{DateTime.UtcNow:O} {value}");
        }
    }
}
