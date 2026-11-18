using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class Eac3FallbackTests : IDisposable
{
    private const string NativeReason = "ERROR: [audio-processing-failure-v1] Audio processing rejected: Unsupported audio codec eac3";
    private const string Title = "otr-can – EAC3-Tonspur erkannt";
    private const string Message = "Eine EAC3-Tonspur wurde erkannt. Der verwendete otr-can-Schnittmotor unterstützt dieses Audioformat derzeit noch nicht.";
    private readonly string _root = Directory.CreateTempSubdirectory("can-eac3-dialog-").FullName;

    private static MediaAnalysisResult Media(bool mixed) => new("mp4", "MP4", 1, TimeSpan.FromSeconds(10), [],
        mixed ? [new(1, "aac", "LC", 48000, 2, "stereo"), new(2, "eac3", null, 48000, 2, "stereo")] :
        [new(1, "eac3", null, 48000, 2, "stereo")]);

    private static OtrCanAudioFailureException Failure(bool mixed = false, string reason = NativeReason) =>
        new(Media(mixed), Media(mixed), new InvalidOperationException(reason));

    [Theory]
    [InlineData(NativeReason, true)]
    [InlineData("Unsupported audio codec eac3", true)]
    [InlineData("otr_can.exe failed:\r\n" + NativeReason + "\r\n", true)]
    [InlineData(NativeReason + " \t", true)]
    [InlineData("Unsupported audio codec ac3", false)]
    [InlineData("Unsupported audio codec opus", false)]
    [InlineData("Unsupported audio codec eac3_extra", false)]
    [InlineData("Unsupported audio codec eac3: decoder failed", false)]
    [InlineData("PCM assembly failed: eac3", false)]
    public void RecognizesOnlyTheExplicitCodecRejection(string reason, bool expected) =>
        Assert.Equal(expected, Failure(reason: reason).IsUnsupportedEac3);

    [Fact]
    public void RecognizesARejectionWrappedByTheToolRunner()
    {
        var error = new OtrCanAudioFailureException(Media(true), Media(true),
            new InvalidOperationException("Tool failed", new IOException(NativeReason)));
        Assert.True(error.IsUnsupportedEac3);
        Assert.Null(error.Diagnostic);
        Assert.Null(error.Offer);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void ShowsExactEac3MessageAndOnlyExplicitAlternativeOrCancel(bool mixed, bool legacy)
    {
        OnSta(() =>
        {
            var error = Failure(mixed);
            var dialog = legacy ? new AudioMp4BoxFallbackDialog(error) :
                new AudioMp4BoxFallbackDialog(new AudioRecoveryOffer(error, new AacGapRepairOffer("unavailable")));
            Assert.Equal(Title, dialog.Title);
            Assert.Equal(Title, ((TextBlock)dialog.FindName("HeadingText")).Text);
            Assert.Equal(Message + Environment.NewLine + Environment.NewLine +
                "Möchtest du den Film stattdessen mit MP4Box schneiden?", ((TextBlock)dialog.FindName("DiagnosisText")).Text);
            var retry = (Button)dialog.FindName("RetryButton");
            Assert.Equal("Mit MP4Box schneiden", retry.Content);
            Assert.True(retry.IsEnabled);
            Assert.False(retry.IsDefault);
            var buttons = ((StackPanel)retry.Parent).Children.OfType<Button>().ToArray();
            Assert.Equal(2, buttons.Count(button => button.Visibility == Visibility.Visible));
            var cancel = buttons[0];
            Assert.True(cancel.IsCancel);
            Assert.True(cancel.IsDefault);
            Assert.Equal(AudioRecoveryChoice.Cancel, dialog.SelectedChoice);
            foreach (var name in new[] { "RepairButton", "TrackButton" })
            {
                var button = (Button)dialog.FindName(name);
                Assert.False(button.IsEnabled);
                Assert.Equal(Visibility.Collapsed, button.Visibility);
            }
            foreach (var name in new[] { "RepairStatusText", "TrackStatusText", "Mp4BoxStatusText", "Mp4BoxHintText" })
                Assert.Equal(Visibility.Collapsed, ((FrameworkElement)dialog.FindName(name)).Visibility);
            dialog.Close();
        });
    }

    [Fact]
    public void UnavailableMp4BoxCannotBeStartedFromTheEac3Dialog()
    {
        OnSta(() =>
        {
            var dialog = new AudioMp4BoxFallbackDialog(new AudioRecoveryOffer(Failure(),
                new AacGapRepairOffer("unavailable"), false, "Programmdatei nicht gefunden"));
            Assert.False(((Button)dialog.FindName("RetryButton")).IsEnabled);
            Assert.Equal(AudioRecoveryChoice.Cancel, dialog.SelectedChoice);
            Assert.Contains("Programmdatei nicht gefunden", ((TextBlock)dialog.FindName("Mp4BoxStatusText")).Text);
            dialog.Close();
        });
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task ExplicitChoicePreservesTheRequestAndSkipsUnrelatedRecoveryProbes(bool mixed, bool accept)
    {
        var source = Path.Combine(_root, "source.mp4");
        var output = Path.Combine(_root, "output.mp4");
        File.WriteAllText(source, "source-sentinel");
        File.WriteAllText(output, "existing-output");
        var request = new CutRequest(source, output, [new(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))], 25, true);
        var repair = new RepairSpy();
        var tracks = new TracksSpy();
        var primary = new Engine(_ => throw Failure(mixed));
        var alternative = new Engine(r =>
        {
            Assert.Same(request, r);
            File.WriteAllText(r.OutputFilePath, "confirmed-output");
            return Task.CompletedTask;
        });
        var choices = 0;
        var engine = new OtrCanMp4BoxFallbackCutService(primary, (_, _) => Task.FromResult<ICutEngine>(alternative),
            (offer, _) =>
            {
                choices++;
                Assert.True(offer.Failure.IsUnsupportedEac3);
                Assert.False(offer.Repair.IsAvailable);
                Assert.Null(offer.TrackRecovery);
                Assert.Equal("existing-output", File.ReadAllText(output));
                return Task.FromResult(accept ? AudioRecoveryChoice.Mp4Box : AudioRecoveryChoice.Cancel);
            }, repair, _ => Task.FromResult<(bool, string?)>((true, null)), tracks);
        if (accept)
            await engine.RunAsync(request);
        else
            await Assert.ThrowsAsync<OperationCanceledException>(() => engine.RunAsync(request));
        Assert.Equal(1, primary.Calls);
        Assert.Equal(1, choices);
        Assert.Equal(accept ? 1 : 0, alternative.Calls);
        Assert.Equal(0, repair.Calls);
        Assert.Equal(0, tracks.Calls);
        Assert.Equal(accept, engine.UsedMp4Box);
        Assert.False(engine.UsedAacRepair);
        Assert.Equal(accept ? "confirmed-output" : "existing-output", File.ReadAllText(output));
        Assert.Equal("source-sentinel", File.ReadAllText(source));
        Assert.Equal(TimeSpan.FromSeconds(1), request.KeepSegments[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(2), request.KeepSegments[0].Duration);
    }

    [Theory]
    [InlineData("[audio-processing-failure-v1] Unsupported audio codec opus")]
    [InlineData("[audio-diagnostic-v1] PCM count differs for audio stream 1")]
    [InlineData("PCM assembly emitted warnings; cut rejected")]
    public async Task OtherAudioFailuresKeepTheirRecoveryAssessment(string reason)
    {
        var request = new CutRequest(Path.Combine(_root, "unused.mp4"), Path.Combine(_root, "out.mp4"),
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))], 25, false);
        var repair = new RepairSpy();
        var tracks = new TracksSpy();
        var engine = new OtrCanMp4BoxFallbackCutService(new Engine(_ => throw Failure(reason: reason)),
            (_, _) => throw new Exception("Not confirmed"), (offer, _) =>
            {
                Assert.False(offer.Failure.IsUnsupportedEac3);
                return Task.FromResult(AudioRecoveryChoice.Cancel);
            }, repair, _ => Task.FromResult<(bool, string?)>((true, null)), tracks);
        await Assert.ThrowsAsync<OperationCanceledException>(() => engine.RunAsync(request));
        Assert.Equal(1, repair.Calls);
        Assert.Equal(1, tracks.Calls);
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

    private sealed class RepairSpy : IAacGapRepair
    {
        internal int Calls { get; private set; }
        public Task<AacGapRepairOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
            IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            Calls++;
            return Task.FromResult(new AacGapRepairOffer("not available"));
        }
    }

    private sealed class TracksSpy : IAudioTrackRecovery
    {
        internal int Calls { get; private set; }
        public Task<AudioTrackRecoveryOffer> AssessAsync(CutRequest request, OtrCanAudioFailureException failure,
            IProgress<CutProgressUpdate>? progress, CancellationToken token)
        {
            Calls++;
            return Task.FromResult(new AudioTrackRecoveryOffer("not available"));
        }
    }

    private static void OnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception error) { failure = error; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)));
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);
}
