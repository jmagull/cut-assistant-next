using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

internal static class Program
{
    private static readonly List<object> Results = [];
    private static void Check(bool condition, string name, string? detail = null)
    {
        if (!condition) throw new InvalidOperationException(name + ": " + detail);
        Results.Add(new { Name = name, Passed = true, Detail = detail });
    }

    private sealed class Engine(Func<CutRequest, Task> run) : ICutEngine
    {
        internal int Calls { get; private set; }
        public Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            return run(request);
        }
    }

    private sealed class NoRepair : IAacGapRepair
    {
        public Task<AacGapRepairOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
            IProgress<CutProgressUpdate>? progress, CancellationToken token) =>
            Task.FromResult(new AacGapRepairOffer("EAC3 ist keine freigegebene AAC-Reparaturklasse"));
    }

    private sealed class NoTracks : IAudioTrackRecovery
    {
        public Task<AudioTrackRecoveryOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
            IProgress<CutProgressUpdate>? progress, CancellationToken token) =>
            Task.FromResult(new AudioTrackRecoveryOffer("Keine eindeutig beschädigte, sicher abwählbare Spur"));
    }

    private static async Task Main(string[] args)
    {
        if (args.Length != 6) throw new ArgumentException("MEDIA-DIR NEW-RESULT-DIR ENGINE INDEXER FFMPEG FFPROBE");
        var fixtures = Path.GetFullPath(args[0]);
        var output = Path.GetFullPath(args[1]);
        if (Directory.Exists(output)) throw new IOException("Existing test directory protected");
        Directory.CreateDirectory(output);
        var tools = new OtrCanToolPaths(args[2], args[3], args[4], args[5]);
        var probe = new FfprobeRunner(tools.Ffprobe);
        var aac = await probe.RunAsync(Path.Combine(fixtures, "aac.mp4"), default);
        var eac3 = await probe.RunAsync(Path.Combine(fixtures, "eac3.mp4"), default);
        var mixed = await probe.RunAsync(Path.Combine(fixtures, "mixed.mp4"), default);
        var duration = TimeSpan.FromSeconds(12);
        var window = new VideoFrameWindow(0, new(1, 50), 0, 0,
            Enumerable.Range(0, 12).Select(i => new VideoFrameInfo(i, i * 50, null, null, null, true, "I")).ToArray());
        var keyframes = VideoKeyframeIndex.Create(window, 0, duration, 0);
        CutPlan Plan(bool off)
        {
            var plan = new CutPlan(duration);
            var cursor = TimeSpan.Zero;
            foreach (var i in new[] { 1, 3, 5, 7, 9 })
            {
                plan.Add(new RemoveSegment(cursor, TimeSpan.FromSeconds(i)));
                cursor = TimeSpan.FromSeconds(i + 1 + (off ? .02 : 0));
            }
            plan.Add(new RemoveSegment(cursor, duration));
            return plan;
        }
        var ac3 = aac with { AudioStreams = [new(1, "ac3", null, 48000, 2, "stereo")] };
        foreach (var (name, media) in new[] { ("AAC", aac), ("AC3", ac3), ("EAC3", eac3), ("AAC+EAC3", mixed) })
        {
            foreach (var off in new[] { false, true })
            {
                var text = CutEngineRecommendation.Evaluate(media, Plan(off), keyframes, true, true);
                var expected = name.Contains("EAC3", StringComparison.Ordinal) ? "Keine eindeutige" : off ? "Empfehlung: otr-can" : "Empfehlung: MP4Box";
                Check(text.StartsWith(expected, StringComparison.Ordinal), $"Recommendation-{name}-{(off ? "off" : "on")}", text);
            }
        }
        Check(CutEngineRecommendation.Evaluate(aac, Plan(false), keyframes, false, true).StartsWith("Keine eindeutige", StringComparison.Ordinal), "Recommendation-no-MP4Box");
        Check(CutEngineRecommendation.Evaluate(aac, Plan(true), keyframes, true, false).StartsWith("Keine eindeutige", StringComparison.Ordinal), "Recommendation-no-otr-can");
        Check(CutEngineRecommendation.Evaluate(aac, Plan(false), null, true, true).StartsWith("Keine eindeutige", StringComparison.Ordinal), "Recommendation-no-keyframes");

        const string nativeError = "[audio-processing-failure-v1] Audio processing rejected: Unsupported audio codec eac3";
        OtrCanAudioFailureException Failure(MediaAnalysisResult media) => new(media, media, new InvalidOperationException(nativeError));
        var failure = Failure(mixed);
        Check(OtrCanAudioFailureException.IsAudioFailure(nativeError) && failure.Diagnostic is null && failure.Offer is null,
            "Unsupported-codec-is-generic-audio-failure-without-gap-or-legacy-AAC-offer", failure.Message);
        var keeps = new[] { 1, 3, 5, 7, 9 }.Select(i => new KeepSegment(TimeSpan.FromSeconds(i), TimeSpan.FromSeconds(1))).ToArray();
        CutRequest Request(string name, string source, IReadOnlyList<KeepSegment> ranges) =>
            new(source, Path.Combine(output, name + ".mp4"), ranges, 50, false);
        var requests = new[]
        {
            Request("single-eac3", Path.Combine(fixtures, "eac3.mp4"), keeps),
            Request("mixed", Path.Combine(fixtures, "mixed.mp4"), keeps)
        };
        foreach (var request in requests)
        {
            await using var repair = await new AacGapRepairService(tools).AssessAsync(request,
                Failure(request == requests[0] ? eac3 : mixed), null, default);
            Check(!repair.IsAvailable && repair.Description.Contains("48 kHz", StringComparison.Ordinal),
                "AAC-repair-denied-" + Path.GetFileNameWithoutExtension(request.OutputFilePath), repair.Description);
        }
        var trackService = new OtrCanAudioTrackRecovery(tools, _ => throw new InvalidOperationException("Must not restart"));
        using var on = await trackService.AssessAsync(requests[1], failure, null, default);
        Check(!on.IsAvailable, "No-unproven-EAC3-track-drop-on-exact-grid", on.Description);
        var offRequest = Request("off-grid", Path.Combine(fixtures, "mixed.mp4"), [new(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.02))]);
        using var offOffer = await trackService.AssessAsync(offRequest, failure, null, default);
        Check(!offOffer.IsAvailable && offOffer.Description.Contains("40 ms", StringComparison.Ordinal), "Track-drop-rejects-50fps-off-40ms-grid", offOffer.Description);
        var tickRequest = Request("one-tick", Path.Combine(fixtures, "mixed.mp4"), [new(TimeSpan.FromSeconds(1).Add(TimeSpan.FromTicks(1)), TimeSpan.FromSeconds(1))]);
        using var tickOffer = await trackService.AssessAsync(tickRequest, failure, null, default);
        Check(!tickOffer.IsAvailable && tickOffer.Description.Contains("40 ms", StringComparison.Ordinal), "Track-drop-rejects-one-TimeSpan-tick", tickOffer.Description);
        var printedRanges = new decimal[] { 602.04m, 1754.4m, 2131.84m, 3237m, 3619.88m, 4956m, 5282.44m, 6484m, 6865.72m, 7137.92m };
        Check(printedRanges.All(value => value * TimeSpan.TicksPerSecond % 400000 == 0), "All-ten-printed-real-film-boundaries-fit-40ms-exactly");

        foreach (var choice in new[] { AudioRecoveryChoice.Cancel, AudioRecoveryChoice.Mp4Box, AudioRecoveryChoice.RepairAacGaps, AudioRecoveryChoice.RetainVerifiedTracks })
        {
            var request = Request("orchestration-" + choice, Path.Combine(fixtures, "mixed.mp4"), keeps);
            var confirmed = false;
            var primary = new Engine(_ => throw failure);
            var alternative = new Engine(r =>
            {
                if (!confirmed || !ReferenceEquals(r, request)) throw new InvalidOperationException("Changed request or unconfirmed restart");
                return Task.CompletedTask;
            });
            var fallback = new OtrCanMp4BoxFallbackCutService(primary, (_, _) => Task.FromResult<ICutEngine>(alternative),
                (offer, _) =>
                {
                    if (offer.Repair.IsAvailable || offer.TrackRecovery?.IsAvailable == true || !offer.Mp4BoxAvailable)
                        throw new InvalidOperationException("Unsafe offered action");
                    confirmed = true;
                    return Task.FromResult(choice);
                }, new NoRepair(), _ => Task.FromResult<(bool, string?)>((true, null)), new NoTracks());
            Exception? caught = null;
            try { await fallback.RunAsync(request); } catch (Exception error) { caught = error; }
            var expected = choice == AudioRecoveryChoice.Cancel ? caught is OperationCanceledException :
                choice == AudioRecoveryChoice.Mp4Box ? caught is null && fallback.UsedMp4Box : caught is InvalidOperationException;
            Check(expected && primary.Calls == 1 && alternative.Calls == (choice == AudioRecoveryChoice.Mp4Box ? 1 : 0) && !fallback.UsedAacRepair,
                "Fallback-orchestration-" + choice, caught?.Message ?? "Explicit alternative invoked once (stub; no real cut)");
        }

        Exception? uiError = null;
        var ui = new Thread(() =>
        {
            try
            {
                var dialog = new AudioMp4BoxFallbackDialog(new AudioRecoveryOffer(failure,
                    new AacGapRepairOffer("Nicht unterstützte AAC-Reparaturklasse"), TrackRecovery: new AudioTrackRecoveryOffer(offOffer.Description)));
                var retry = (Button)dialog.FindName("RetryButton");
                var repair = (Button)dialog.FindName("RepairButton");
                var track = (Button)dialog.FindName("TrackButton");
                var cancel = ((StackPanel)retry.Parent).Children.OfType<Button>().First();
                Check(retry.IsEnabled && retry.Visibility == Visibility.Visible && !retry.IsDefault &&
                    !repair.IsEnabled && repair.Visibility == Visibility.Collapsed &&
                    !track.IsEnabled && track.Visibility == Visibility.Collapsed && cancel.IsDefault && cancel.IsCancel &&
                    dialog.SelectedChoice == AudioRecoveryChoice.Cancel, "WPF-dialog-safe-actions-and-default-cancel");
                Check(!((TextBlock)dialog.FindName("DiagnosisText")).Text.Contains("eac3", StringComparison.OrdinalIgnoreCase),
                    "WPF-dialog-generic-diagnosis-does-not-explain-codec-exclusion");
                dialog.Close();
            }
            catch (Exception error) { uiError = error; }
        });
        ui.SetApartmentState(ApartmentState.STA);
        ui.Start();
        if (!ui.Join(TimeSpan.FromSeconds(15))) throw new TimeoutException("WPF probe");
        if (uiError is not null) throw new InvalidOperationException("WPF diagnosis failed", uiError);
        var result = new { Passed = true, Count = Results.Count, Cases = Results };
        await File.WriteAllTextAsync(Path.Combine(output, "results.json"), JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
}
