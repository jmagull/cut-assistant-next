using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media.Imaging;
using CutAssistantNext.App.Controls;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace TransitionReview;

internal sealed record NativeReference(string SourceFilePath, IReadOnlyList<decimal> TransitionSourceStarts);

internal static class NativeChecks
{
    public static async Task RunAsync(AnalysisResult result, NativeReference reference, string proofPath, CancellationToken token)
    {
        if (reference.TransitionSourceStarts.Count != result.Transitions.Count)
            throw new ArgumentException("Referenz benötigt einen Originalbeginn je Übergang.");
        var output = Path.GetDirectoryName(Path.GetFullPath(proofPath))!;
        if (File.Exists(proofPath)) throw new IOException("Prüfnachweis existiert bereits.");
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token);
        deadline.CancelAfter(TimeSpan.FromMinutes(2));
        token = deadline.Token;
        var player = new MpvMediaPlayerService();
        var recordedPlayer = new RecordingPlayer(player);
        var window = Program.CreateReviewWindow(recordedPlayer, hidden: true);
        var host = new MpvVideoHost();
        var referenceWindow = new Window { Content = host, Width = 320, Height = 240, Opacity = 0, ShowActivated = false, ShowInTaskbar = false };
        await using var sourcePlayer = new MpvMediaPlayerService();
        var checks = new List<object>();
        try
        {
            window.Show();
            referenceWindow.Show();
            await WaitAsync(() => host.IsVideoWindowReady, token);
            await sourcePlayer.InitializeAsync(host.VideoWindowHandle, token);
            NativeVideoCapture.EnableLog(sourcePlayer);
            await sourcePlayer.LoadAsync(reference.SourceFilePath, token);
            await WaitAsync(() => sourcePlayer.State == MediaPlayerState.Paused && sourcePlayer.Duration > TimeSpan.Zero, token);
            await sourcePlayer.SetVolumeAsync(0, token);
            var completion = new CutCompletionViewModel(result);
            var model = (CutReviewViewModel)window.DataContext;
            for (var i = 0; i < result.Transitions.Count; i++)
            {
                var item = result.Transitions[i];
                var target = (double)item.Seconds;
                var reviewStart = Math.Max(0, item.Seconds - 3);
                // Same production review VM: load -> measured transition minus 3 -> play.
                await window.ReviewAsync(completion, i + 1);
                if (recordedPlayer.RequestedSeeks[^1] != Formatting.Time(reviewStart))
                    throw new InvalidOperationException("Kontrollplayer hat eine abweichende Prüfposition angefordert.");
                await player.SetVolumeAsync(0, token);
                await WaitAsync(() => player.State == MediaPlayerState.Playing &&
                    player.Position.TotalSeconds >= (double)reviewStart - .08 && player.Position.TotalSeconds < (double)reviewStart + .6, token);
                var observedStart = player.Position.TotalSeconds;
                await WaitAsync(() => player.Position.TotalSeconds >= target + .12 || player.State == MediaPlayerState.Error, token);
                if (player.State != MediaPlayerState.Playing || player.ErrorMessage is not null)
                    throw new InvalidOperationException("Playerfehler beim Überqueren des Übergangs: " + player.ErrorMessage);
                await player.PauseAsync(token);
                var crossed = player.Position.TotalSeconds;
                await sourcePlayer.SeekAsync(Formatting.Time(reference.TransitionSourceStarts[i]), token);
                await WaitAsync(() => Math.Abs(sourcePlayer.Position.TotalSeconds - (double)reference.TransitionSourceStarts[i]) < .021, token);
                await Task.Delay(150, token);
                var referenceHash = await ScreenshotHashAsync(sourcePlayer, Path.Combine(output, $"source-{i + 1}.png"), token);
                await player.SeekAsync(Formatting.Time(item.Seconds - .12m), token);
                await WaitAsync(() => Math.Abs(player.Position.TotalSeconds - (target - .12)) < .05, token);
                await Task.Delay(150, token);
                var frames = new List<NativeFrame>();
                for (var step = 0; step < 8; step++)
                {
                    var hash = await ScreenshotHashAsync(player, Path.Combine(output, $"transition-{i + 1}-step-{step}.png"), token);
                    frames.Add(new(player.Position.TotalSeconds, hash));
                    await player.StepForwardAsync(token);
                    await Task.Delay(100, token);
                }
                var matches = frames.Where(f => f.PixelSha256 == referenceHash).ToArray();
                if (matches.Length == 0) throw new InvalidOperationException($"Original-Anfangsbild von Übergang {i + 1} wurde im libmpv-Schrittfenster nicht angezeigt.");
                if (!matches.Any(f => Math.Abs(f.Position - target) < .081))
                    throw new InvalidOperationException("Angezeigtes Anfangsbild liegt außerhalb der gemessenen Übergangsumgebung.");
                checks.Add(new
                {
                    Transition = i + 1,
                    RequestedReviewStart = reviewStart,
                    ObservedStart = observedStart,
                    PlaybackCrossedTo = crossed,
                    ReferencePixelSha256 = referenceHash,
                    MatchingDisplayedFrames = matches,
                    DuplicatePtsAtBoundary = result.Algorithm == "prototype-v1" ? (bool?)item.DuplicatePtsAtBoundary : null,
                    SteppedFrames = frames
                });
                Console.WriteLine($"PASS native Übergang {i + 1}: Start {observedStart:F3}, Anfangsbild bei {string.Join(", ", matches.Select(m => m.Position.ToString("F3")))}");
            }
            await model.SetVolumeAsync(0);
        }
        finally
        {
            await sourcePlayer.DisposeAsync();
            referenceWindow.Close();
            await window.CloseSafelyAsync();
        }
        Program.SaveNew(proofPath, new
        {
            Passed = true,
            InputMode = result.Algorithm,
            ActualLinkedReviewVm = true,
            PlayersDisposed = true,
            Checks = checks,
            Note = "Real HWND/libmpv, hidden muted windows; pixel comparisons, no personal scene/audio acceptance."
        });
    }

    private static Task<string> ScreenshotHashAsync(MpvMediaPlayerService player, string path, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return Task.FromResult(NativeVideoCapture.Capture(player, path));
    }
    private static async Task WaitAsync(Func<bool> ready, CancellationToken token)
    {
        while (!ready()) await Task.Delay(20, token);
    }

    private sealed record NativeFrame(double Position, string PixelSha256);
}
