using System.Windows;
using CutAssistantNext.App.Controls;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Playback;
using TransitionReview;

namespace DiagnosisReview;

internal static class NativeReviewChecks
{
    public static async Task<bool> RunAsync(ReviewRequest request, string reportHash, string directory, CancellationToken token,
        CutResult? actualResult = null)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new IOException("Nachweisverzeichnis existiert bereits.");
        Directory.CreateDirectory(directory);
        var player = new MpvMediaPlayerService();
        var recorded = new RecordingPlayer(player);
        var window = Program.CreateWindow(recorded, hidden: true);
        var host = new MpvVideoHost();
        var referenceWindow = new Window { Content = host, Width = 320, Height = 240, Opacity = 0, ShowActivated = false, ShowInTaskbar = false };
        var source = new MpvMediaPlayerService();
        var checks = new List<BoundaryProof>();
        var completion = new CutCompletionViewModel(actualResult ?? request.CreateCutResult());
        var model = (CutReviewViewModel)window.DataContext;
        try
        {
            window.Show();
            referenceWindow.Show();
            await WaitAsync(() => host.IsVideoWindowReady, token);
            await source.InitializeAsync(host.VideoWindowHandle, token);
            NativeVideoCapture.EnableLog(source);
            await source.LoadAsync(request.Source.Path, token);
            await WaitAsync(() => source.State == MediaPlayerState.Paused && source.Duration > TimeSpan.Zero, token);
            await source.SetVolumeAsync(0, token);
            foreach (var candidate in request.Candidates)
            {
                var proof = new BoundaryProof(candidate.Index, candidate.Seconds, Math.Max(0, candidate.Seconds - 3));
                checks.Add(proof);
                try
                {
                    token.ThrowIfCancellationRequested();
                    var references = new List<ReferenceProof>();
                    for (var n = 0; n < candidate.References.Count; n++)
                    {
                        var frame = candidate.References[n];
                        await source.SeekAsync(ReviewRequest.Time(frame.SourceSeconds), token);
                        await WaitAsync(() => Math.Abs(source.Position.TotalSeconds - (double)frame.SourceSeconds) < .021, token);
                        await Task.Delay(180, token);
                        var name = $"source-{candidate.Index}-{n}.png";
                        references.Add(new(frame.SourceSeconds, frame.OutputSeconds, frame.IsFirstNewFrame,
                            NativeVideoCapture.Capture(source, Path.Combine(directory, name)), source.Position.TotalSeconds, name));
                    }
                    proof.References = references;
                    var countBefore = recorded.RequestedSeeks.Count;
                    await window.ReviewAsync(completion, candidate.Index);
                    proof.SeekRequested = recorded.RequestedSeeks.Count == countBefore + 1 &&
                        recorded.RequestedSeeks[^1] == ReviewRequest.Time(proof.RequestedReviewStartSeconds);
                    if (!proof.SeekRequested) throw new InvalidOperationException("Prüfstart nicht wie angefordert ausgeführt.");
                    await WaitAsync(() => player.State == MediaPlayerState.Playing && player.Position.TotalSeconds >=
                        (double)proof.RequestedReviewStartSeconds - .08 && player.Position.TotalSeconds < (double)proof.RequestedReviewStartSeconds + .6, token);
                    proof.ObservedStartSeconds = player.Position.TotalSeconds;
                    await WaitAsync(() => player.Position.TotalSeconds >= (double)candidate.Seconds + .2 || player.State == MediaPlayerState.Error, token);
                    if (player.State != MediaPlayerState.Playing || player.ErrorMessage is not null)
                        throw new InvalidOperationException("Playerfehler beim Übergang: " + player.ErrorMessage);
                    proof.PlaybackCrossedToSeconds = player.Position.TotalSeconds;
                    await player.PauseAsync(token);
                    await player.SeekAsync(ReviewRequest.Time(candidate.Seconds - .16m), token);
                    await WaitAsync(() => Math.Abs(player.Position.TotalSeconds - (double)(candidate.Seconds - .16m)) < .05, token);
                    await Task.Delay(180, token);
                    var frames = new List<DisplayedFrame>();
                    for (var step = 0; step < 12; step++)
                    {
                        var name = $"output-{candidate.Index}-step-{step:D2}.png";
                        var hash = NativeVideoCapture.Capture(player, Path.Combine(directory, name));
                        frames.Add(new(step, player.Position.TotalSeconds, hash, name,
                            references.Where(r => r.PixelSha256 == hash).Select(r => r.SourceSeconds).ToArray()));
                        await player.StepForwardAsync(token);
                        await Task.Delay(100, token);
                    }
                    proof.SteppedFrames = frames;
                    var firstReference = references.Single(r => r.IsFirstNewFrame);
                    proof.FirstNewFrameMatches = frames.Where(f => f.PixelSha256 == firstReference.PixelSha256).ToArray();
                    proof.PreviousLastFrameObserved = frames.Any(f => f.PixelSha256 == references[0].PixelSha256);
                    if (references.Any(r => !r.IsFirstNewFrame && r.PixelSha256 == firstReference.PixelSha256))
                        throw new InvalidDataException("Das erste neue Referenzbild ist pixelgleich zu einem benachbarten Referenzbild; keine eindeutige Bildbestätigung.");
                    proof.Passed = proof.FirstNewFrameMatches.Any(f => Math.Abs(f.ObservedPositionSeconds - (double)candidate.Seconds) < .081);
                    if (!proof.Passed) proof.Error = "Das erste neue Originalbild wurde im Schrittfenster nicht nahe dem Diagnosekandidaten bestätigt.";
                }
                catch (Exception error) when (error is not OperationCanceledException)
                {
                    proof.Error = error.Message;
                    proof.Passed = false;
                }
                finally { Program.SaveNew(Path.Combine(directory, $"boundary-{candidate.Index}.json"), proof); }
            }
        }
        finally
        {
            try { await source.DisposeAsync(); }
            finally
            {
                referenceWindow.Close();
                try { await model.DisposeAsync(); }
                finally { await window.CloseSafelyAsync(); }
            }
        }
        var passed = checks.Count == request.Candidates.Count && checks.All(c => c.Passed && c.SeekRequested);
        Program.SaveNew(Path.Combine(directory, "NATIVE-PROOF.json"), new
        {
            Passed = passed,
            DiagnosisSha256 = reportHash,
            request.Source,
            request.Output,
            ActualProductionReviewSources = true,
            ActualProductionCutResult = actualResult is not null,
            TimingKind = (actualResult ?? request.CreateCutResult()).TimingKind.ToString(),
            PlayersDisposed = true,
            Checks = checks,
            ReviewPositionsAutomaticallyCertified = false,
            Note = "Real HWND/libmpv; continuous playback crossing plus paused frame-step pixel comparisons. Observed positions are player notifications, not independent frame timestamps. No personal scene, audio or three-second acceptance."
        });
        return passed;
    }

    private static async Task WaitAsync(Func<bool> ready, CancellationToken token)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token);
        deadline.CancelAfter(TimeSpan.FromSeconds(25));
        while (!ready()) await Task.Delay(20, deadline.Token);
    }

    internal sealed record ReferenceProof(decimal SourceSeconds, decimal OutputSeconds, bool IsFirstNewFrame,
        string PixelSha256, double ObservedSourcePositionSeconds, string Image);
    internal sealed record DisplayedFrame(int Step, double ObservedPositionSeconds, string PixelSha256, string Image,
        IReadOnlyList<decimal> MatchingSourceSeconds);
    private sealed class BoundaryProof(int index, decimal candidateSeconds, decimal requestedStart)
    {
        public int Transition { get; } = index;
        public decimal CandidateSeconds { get; } = candidateSeconds;
        public decimal RequestedReviewStartSeconds { get; } = requestedStart;
        public bool SeekRequested { get; set; }
        public double? ObservedStartSeconds { get; set; }
        public double? PlaybackCrossedToSeconds { get; set; }
        public bool PreviousLastFrameObserved { get; set; }
        public IReadOnlyList<ReferenceProof> References { get; set; } = [];
        public IReadOnlyList<DisplayedFrame> SteppedFrames { get; set; } = [];
        public IReadOnlyList<DisplayedFrame> FirstNewFrameMatches { get; set; } = [];
        public bool Passed { get; set; }
        public string? Error { get; set; }
    }
}
