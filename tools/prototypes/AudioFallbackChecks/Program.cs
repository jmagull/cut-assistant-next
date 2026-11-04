using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

internal static class Program
{
    private static void Check(bool success, string message) { if (!success) throw new InvalidOperationException(message); }
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (args[0] == "render") Render(args[1]);
            else RunAsync(args).GetAwaiter().GetResult();
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }

    private sealed class Log : IProgress<CutProgressUpdate>
    {
        public void Report(CutProgressUpdate update) { if (!update.Message.StartsWith("Indexing", StringComparison.Ordinal)) Console.WriteLine(update.Message); }
    }
    private static async Task<string> HashAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        return Convert.ToHexString(await SHA256.HashDataAsync(stream));
    }
    private static async Task RunAsync(string[] args)
    {
        var mode = args[0]; var input = Path.GetFullPath(args[1]); var job = Path.GetFullPath(args[2]);
        Directory.CreateDirectory(job);
        var tools = new OtrCanToolPaths(args[3], args[4], args[5], args[6]);
        var probe = new FfprobeRunner(tools.Ffprobe); var source = await probe.RunAsync(input);
        var hash = await HashAsync(input);
        var plan = JsonSerializer.Deserialize<double[][]>(File.ReadAllText(Path.Combine(job, "plan.json")))!;
        var keeps = plan.Select(k => new KeepSegment(TimeSpan.FromSeconds(k[0]), TimeSpan.FromSeconds(k[1] - k[0]))).ToArray();
        var output = Path.Combine(job, "result.mp4");
        var reject = mode is "decline" or "unknown" or "cancel" or "aac-decline" or "bad-aac";
        if (reject) File.WriteAllText(output, "existing valid destination sentinel");
        var request = new CutRequest(input, output, keeps, 25, overwriteExistingOutput: reject);
        var cutText = OtrCanCutService.BuildCutlist(request);
        var log = new Log(); var aacQuestions = 0; var mp4Questions = 0; var rejected = false;
        using var cancel = new CancellationTokenSource();
        ICutEngine normal = new OtrCanCutService(tools);
        if (mode.StartsWith("aac", StringComparison.Ordinal) || mode == "bad-aac")
            normal = new OtrCanAacEmergencyCutService(normal, index => new OtrCanCutService(tools, retainedAudioStreamIndex: index),
                new AacEmergencySource(tools), (offer, _) =>
                {
                    aacQuestions++;
                    Console.WriteLine("TEST: explicit AAC choice simulated; " + offer.Notice(offer.Choices.Single()));
                    return Task.FromResult<AacEmergencyChoice?>(mode == "aac-decline" ? null : offer.Choices.Single());
                });
        OtrCanAudioDiagnostic? diagnosis = null;
        var engine = new OtrCanMp4BoxFallbackCutService(normal, async (progress, token) =>
        {
            var factory = new ConfiguredMp4BoxCutServiceFactory(() => new CutApplicationSettings { ExecutablePath = args[7] },
                path => new Mp4BoxRunner(path, progress: progress));
            return await factory.CreateCheckedAsync(progress, token);
        }, (failure, _) =>
        {
            mp4Questions++; diagnosis = failure.Diagnostic;
            Check(!Directory.EnumerateFileSystemEntries(job, ".can-*").Any(), "otr-can/AAC workspace remains before consent");
            if (reject) Check(File.ReadAllText(output) == "existing valid destination sentinel", "existing destination changed before consent");
            if (mode == "cancel") cancel.Cancel();
            Console.WriteLine("TEST: explicit MP4Box consent simulated: " + (mode == "accept"));
            return Task.FromResult(mode == "accept");
        });
        CutResult? result = null;
        try { result = await engine.RunWithResultAsync(request, log, cancel.Token); }
        catch (Exception error) when (reject && error is OtrCanAudioFailureException or OperationCanceledException)
        { rejected = true; Console.WriteLine("EXPECTED: " + error.Message); }
        Check(!Directory.EnumerateFileSystemEntries(job, ".can-*").Any(), "private workspace remains");
        Check(!Directory.EnumerateFileSystemEntries(job, ".cut-assistant-next-*").Any(), "MP4Box partial output remains");
        Check(cutText == OtrCanCutService.BuildCutlist(request), "cut times changed");
        Check(hash == await HashAsync(input), "original changed");
        if (reject)
        {
            Check(rejected && File.ReadAllText(output) == "existing valid destination sentinel", "rejection failed to preserve output");
            Check(mp4Questions == 1 && !engine.UsedMp4Box, "unexpected MP4Box retry");
            if (mode == "unknown") Check(diagnosis?.Classification == "unclassified", "unknown cause incorrectly proven");
        }
        else
        {
            Check(result is not null && File.Exists(output), "missing output");
            var actual = await probe.RunAsync(output);
            Check(actual.VideoStreams.Count == 1 && actual.AudioStreams.Count == (mode == "aac" ? 1 : source.AudioStreams.Count), "unexpected tracks");
            Check(mp4Questions == (mode == "accept" ? 1 : 0), "unexpected consent count");
            if (mode == "accept") Check(engine.UsedMp4Box && result!.Notice!.Contains("keine nachgewiesene Audioreparatur"), "missing completion notice");
        }
        if (mode.StartsWith("aac", StringComparison.Ordinal) || mode == "bad-aac") Check(aacQuestions == 1, "AAC policy changed");
        File.WriteAllText(Path.Combine(job, "results.json"), JsonSerializer.Serialize(new
        {
            Mode = mode, Passed = true, AacQuestions = aacQuestions, Mp4BoxQuestions = mp4Questions,
            Rejected = rejected, engine.UsedMp4Box, Diagnosis = diagnosis, Notice = result?.Notice,
            InputSha256 = hash, EngineSha256 = await HashAsync(tools.Engine),
            AppAssemblySha256 = await HashAsync(typeof(OtrCanCutService).Assembly.Location),
            Output = output, CutTimesUnchanged = true, WorkspacesCleaned = true
        }, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("PASS " + mode);
    }

    private static void Render(string root)
    {
        Directory.CreateDirectory(root); _ = new Application();
        var source = new MediaAnalysisResult("mp4", "MP4", 1, TimeSpan.FromHours(1), [], [new(1, "aac", null, 48000, 2, "stereo")]);
        var diagnostic = new OtrCanAudioDiagnostic(1, "source_coverage_gap", 1, "aac", 48000, 115580160, 115579648,
            [new("internal_gap", 3, 185214976, 185215488, 512)], "historical cause unknown");
        var dialog = new AudioMp4BoxFallbackDialog(new(source, source, new InvalidOperationException("audio failure"), diagnostic));
        var content = (FrameworkElement)dialog.Content;
        content.Measure(new Size(760, double.PositiveInfinity));
        content.Arrange(new Rect(0, 0, 760, content.DesiredSize.Height)); content.UpdateLayout();
        var bitmap = new RenderTargetBitmap(760, (int)Math.Ceiling(content.DesiredSize.Height), 96, 96, PixelFormats.Pbgra32);
        var visual = new DrawingVisual(); using (var draw = visual.RenderOpen())
            draw.DrawRectangle(new SolidColorBrush(Color.FromRgb(244, 246, 248)), null, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
        bitmap.Render(visual); bitmap.Render(content);
        using var file = File.Create(Path.Combine(root, "audio-mp4box-dialog.png"));
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap)); encoder.Save(file); dialog.Close();
    }
}
