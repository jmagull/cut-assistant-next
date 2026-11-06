using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Media.Tools;

internal static class Program
{
    private static void Require(bool condition, string text) { if (!condition) throw new InvalidOperationException(text); }
    private sealed class Updates(Action<CutProgressUpdate> update) : IProgress<CutProgressUpdate> { public void Report(CutProgressUpdate value) => update(value); }
    private sealed class RecordingRunner(string root, bool failRepair) : ICutToolRunner
    {
        private readonly CutToolRunner runner = new();
        internal List<string> NormalWork { get; } = [];
        internal int Analyses { get; private set; }
        internal int Repairs { get; private set; }
        public async Task RunAsync(string path, IReadOnlyList<string> args, IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            string Value(string key) => args[Array.IndexOf(args.ToArray(), key) + 1];
            if (args.Contains("cut-can")) NormalWork.Add(Value("--temp-dir"));
            if (args.Count >= 2 && args[0] == "aac-gap-dev" && args[1] == "analyze") Analyses++;
            var repairing = args.Count >= 2 && args[0] == "aac-gap-dev" && args[1] == "cut";
            if (repairing)
            {
                Repairs++;
                if (failRepair) { var edited = args.ToArray(); edited[Array.IndexOf(edited, "--confirm-plan-sha256") + 1] = new string('0', 64); args = edited; }
            }
            await runner.RunAsync(path, args, progress, token);
            if (repairing)
            {
                var work = Value("--work");
                File.Copy(Path.Combine(work, "completed.json"), Path.Combine(root, "motor-completed.json"));
                File.Copy(Path.Combine(work, "independent-final-check", "mp4-verification.json"), Path.Combine(root, "motor-proof.json"));
            }
        }
    }
    private static void Render(AudioRecoveryOffer offer, string output)
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try
            {
                var dialog = new AudioMp4BoxFallbackDialog(offer);
                var content = (FrameworkElement)dialog.Content;
                dialog.Content = null;
                var surface = new System.Windows.Controls.Grid { Background = dialog.Background }; surface.Children.Add(content);
                surface.Measure(new Size(760, 650)); surface.Arrange(new Rect(0, 0, 760, surface.DesiredSize.Height)); surface.UpdateLayout();
                var image = new RenderTargetBitmap(760, (int)Math.Ceiling(surface.ActualHeight), 96, 96, PixelFormats.Pbgra32); image.Render(surface);
                var png = new PngBitmapEncoder(); png.Frames.Add(BitmapFrame.Create(image)); using var file = new FileStream(output, FileMode.CreateNew); png.Save(file);
                dialog.Close();
            }
            catch (Exception e) { error = e; }
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); Require(thread.Join(TimeSpan.FromSeconds(15)), "WPF render timeout"); if (error is not null) throw new InvalidOperationException("WPF render failed", error);
    }
    private static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length is < 7 or > 9) throw new ArgumentException("ENGINE INDEXER FFMPEG FFPROBE MP4BOX FIXTURE-DIR NEW-RESULT-DIR [NCIS-INPUT [JOHN-WICK-INPUT]]");
            var only = Environment.GetEnvironmentVariable("CAN_AUDIO_RECOVERY_CASE");
            var paths = new OtrCanToolPaths(Path.GetFullPath(args[0]), Path.GetFullPath(args[1]), Path.GetFullPath(args[2]), Path.GetFullPath(args[3]));
            var mp4box = Path.GetFullPath(args[4]); var fixtures = Path.GetFullPath(args[5]); var root = Path.GetFullPath(args[6]);
            Require(!Directory.Exists(root) && !File.Exists(root), "Existing test destination protected"); Directory.CreateDirectory(root);
            var mp4Factory = new ConfiguredMp4BoxCutServiceFactory(() => new CutApplicationSettings { ExecutablePath = mp4box }, (path, progress) => new Mp4BoxRunner(path, progress: progress));
            var app = await AacGapRepairService.IdentifyAsync(typeof(AudioMp4BoxFallbackDialog).Assembly.Location, default);
            var motor = await AacGapRepairService.IdentifyAsync(paths.Engine, default);
            var results = new List<object>();
            async Task Case(string name, string source, (long Start, long End)[] keeps, string action, bool repairable,
                int missing = 0, int? retainedTrack = null)
            {
                if (only is not null && only != name) return;
                var work = Path.Combine(root, name); Directory.CreateDirectory(work);
                var output = Path.Combine(work, "result.mp4");
                var protect = action is "cancel" or "unsupported" or "fail-cancel" or "running-cancel";
                if (protect) File.WriteAllText(output, "existing-output-sentinel");
                var request = new CutRequest(source, output, keeps.Select(k => new KeepSegment(T(k.Start), T(k.End - k.Start))), 25, protect);
                static TimeSpan T(long ticks) => TimeSpan.FromTicks(ticks * TimeSpan.TicksPerSecond / 48000);
                var before = await AacGapRepairService.IdentifyAsync(source, default);
                var runner = new RecordingRunner(work, action.StartsWith("fail-", StringComparison.Ordinal));
                var normal = new OtrCanCutService(paths, runner: runner);
                var repair = new AacGapRepairService(paths, runner: runner);
                var count = 0; var mp4Starts = 0; using var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(25));
                using var file = new StreamWriter(Path.Combine(work, "can.log")) { AutoFlush = true };
                var progress = new Updates(u =>
                {
                    lock (file) file.WriteLine(u.Message);
                    if (action == "running-cancel" && u.Message.Contains("[aac-gap-dev] generator-original-decode", StringComparison.Ordinal)) cancellation.Cancel();
                });
                var engine = new OtrCanMp4BoxFallbackCutService(normal, async (updates, ct) =>
                {
                    mp4Starts++; Require(action is "mp4" or "fail-mp4", "No automatic MP4Box start");
                    return await mp4Factory.CreateCheckedAsync(updates, ct);
                }, (offer, _) =>
                {
                    count++; Require(runner.NormalWork.Count == 1 && runner.NormalWork.All(p => !Directory.Exists(p)), "Normal work not cleaned before choice");
                    Require(offer.Repair.IsAvailable == (count == 1 && repairable), "Wrong repair eligibility: " + offer.Repair.Description);
                    if (count == 1 && repairable) Require(offer.Repair.Description.Contains("Fehlende Samples: " + missing, StringComparison.Ordinal), "Wrong displayed missing sample count");
                    Require(offer.TrackRecovery?.IsAvailable == retainedTrack.HasValue, "Wrong track eligibility: " + offer.TrackRecovery?.Description);
                    if (retainedTrack is int retained)
                    {
                        Require(offer.TrackRecovery!.Description.Contains($"Erhalten: Spur {retained}", StringComparison.Ordinal), "Wrong retained track");
                        Require(offer.TrackRecovery.Description.Contains($"Ausgeschlossen: Spur {(retained == 1 ? 2 : 1)}", StringComparison.Ordinal), "Wrong excluded track");
                    }
                    if (name is "single-repair" or "large-gap" or "repair-fails-then-mp4" or "ac3-damaged") Render(offer, Path.Combine(work, $"dialog-{count}.png"));
                    if (count > 1) { Require(!offer.Repair.IsAvailable && offer.RepairFailure is not null && mp4Starts == 0, "Failed repair must not run an automatic fallback"); return Task.FromResult(action == "fail-mp4" ? AudioRecoveryChoice.Mp4Box : AudioRecoveryChoice.Cancel); }
                    return Task.FromResult(action switch { "mp4" => AudioRecoveryChoice.Mp4Box, "track" => AudioRecoveryChoice.RetainVerifiedTracks, "repair" or "fail-cancel" or "fail-mp4" or "running-cancel" => AudioRecoveryChoice.RepairAacGaps, _ => AudioRecoveryChoice.Cancel });
                }, repair, mp4Factory.CheckAvailabilityAsync, new OtrCanAudioTrackRecovery(paths,
                    indices => new OtrCanCutService(paths, runner: runner, verifiedAudioStreamIndices: indices), runner));
                CutResult? result = null;
                try { result = await engine.RunWithResultAsync(request, progress, cancellation.Token); }
                catch (OperationCanceledException) when (protect) { }
                Require((result is null) == protect, "Wrong result/cancel state");
                Require(count == (action == "normal" ? 0 : action.StartsWith("fail-", StringComparison.Ordinal) ? 2 : 1), "Unexpected decision count");
                Require(runner.Repairs == (action is "repair" or "fail-cancel" or "fail-mp4" or "running-cancel" ? 1 : 0), "Unexpected repair count");
                Require(mp4Starts == (action is "mp4" or "fail-mp4" ? 1 : 0), "Unexpected MP4Box count");
                Require(engine.UsedAacRepair == (action == "repair") && engine.UsedMp4Box == (action is "mp4" or "fail-mp4"), "Incorrect actual engine marker");
                Require(await AacGapRepairService.IdentifyAsync(source, default) == before, "Source changed");
                Require(!Directory.EnumerateFileSystemEntries(work, ".can-*").Any(), "Owned temporary files left behind");
                if (protect) Require(File.ReadAllText(output) == "existing-output-sentinel", "Existing output changed");
                if (result is not null) Require(new FileInfo(output).Length > 0, "No final output");
                if (action == "track")
                {
                    Require(runner.NormalWork.Count == 2 && count == 1, "Expected one confirmed restart");
                    var actual = await new FfprobeRunner(paths.Ffprobe).RunAsync(output, default);
                    Require(actual.AudioStreams.Count == 1 && actual.AudioStreams[0].CodecName == (retainedTrack == 1 ? "aac" : "ac3"), "Unexpected retained output tracks");
                    Require(result!.Notice!.Contains("Ausgeschlossen:", StringComparison.Ordinal), "Missing track result notice");
                }
                if (name == "ncis-real-full")
                {
                    var outputHash = (await AacGapRepairService.IdentifyAsync(output, default)).Sha256;
                    Require(outputHash == "90948259c6823edc23c1e4af5b6778e5a7ba654c16109cab9f226d020a3fea0a", "NCIS output differs from Build 51 reference");
                }
                results.Add(new { Name = name, Passed = true, Action = action, Decisions = count, Preflight = runner.Analyses, Repairs = runner.Repairs, Mp4BoxRuns = mp4Starts, SourceSha256 = before.Sha256, AppSha256 = app.Sha256, MotorSha256 = motor.Sha256, Output = output });
                Console.WriteLine("PASS " + name);
            }
            string F(string name) => Path.Combine(fixtures, name + ".mp4");
            (long, long)[] whole = [(0, 384000)];
            await Case("healthy-normal", F("regular-generated"), whole, "normal", false);
            await Case("single-repair", F("single"), whole, "repair", true, 512);
            await Case("multiple-repair", F("multiple"), whole, "repair", true, 1024);
            await Case("outside-gaps-normal", F("single"), [(19200, 384000)], "normal", false);
            await Case("single-frame-at-splice", F("multiple"), [(46080, 96000), (142080, 192000), (238080, 288000)], "repair", true, 512);
            await Case("unsupported-511", F("unsupported"), whole, "unsupported", false);
            await Case("large-gap", F("large"), whole, "unsupported", false);
            await Case("premature-end", F("regular-generated"), [(0, 460800)], "unsupported", false);
            await Case("multitrack-preserved", F("multi-audio"), whole, "unsupported", false);
            await Case("explicit-cancel", F("single"), whole, "cancel", true, 512);
            await Case("explicit-mp4box", F("single"), whole, "mp4", true, 512);
            await Case("repair-fails-then-cancel", F("single"), whole, "fail-cancel", true, 512);
            await Case("repair-fails-then-mp4", F("single"), whole, "fail-mp4", true, 512);
            await Case("manual-cancel-running-repair", F("single"), whole, "running-cancel", true, 512);
            if (File.Exists(F("ac3-damaged")))
            {
                (long, long)[] twoKeeps = [(38400, 192000), (384000, 583680)];
                await Case("healthy-two-tracks", F("healthy-two-tracks"), twoKeeps, "normal", false);
                await Case("ac3-damaged", F("ac3-damaged"), twoKeeps, "track", false, retainedTrack: 1);
                await Case("aac-damaged", F("aac-damaged"), twoKeeps, "unsupported", false);
                await Case("aac-damaged-aligned-ac3", F("aac-damaged-aligned-ac3"), twoKeeps, "track", false, retainedTrack: 2);
                await Case("two-damaged", F("two-damaged"), twoKeeps, "unsupported", false);
                await Case("track-explicit-cancel", F("ac3-damaged"), twoKeeps, "cancel", false, retainedTrack: 1);
            }
            if (args.Length >= 8)
            {
                await Case("ncis-real-short-keep", Path.GetFullPath(args[7]), [(185212800, 185216640)], "repair", true, 512);
                if (only == "ncis-real-full")
                    await Case("ncis-real-full", Path.GetFullPath(args[7]),
                        [(20676480, 68305920), (91969920, 140280960), (165576960, 185216640)], "repair", true, 512);
            }
            if (args.Length == 9)
                await Case("john-wick-real", Path.GetFullPath(args[8]), [(123989760, 168449280), (206376960, 280575360)],
                    "track", false, retainedTrack: 1);
            Require(results.Count > 0, "No native case matched the selected name");
            File.WriteAllText(Path.Combine(root, "results.json"), JsonSerializer.Serialize(new { Passed = true, Count = results.Count, Cases = results }, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
