using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class AudioMp4BoxFallbackDialogTests
{
    [Fact]
    public void CancelIsDefaultAndRetryRequiresItsOwnExplicitAction()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var media = new MediaAnalysisResult("mp4", "MP4", 1, TimeSpan.FromSeconds(10), [], [new(1, "aac", null, 48000, 2, "stereo")]);
                var error = new OtrCanAudioFailureException(media, media, new InvalidOperationException("[audio-diagnostic-v1] gap"));
                var dialog = new AudioMp4BoxFallbackDialog(error);
                dialog.Measure(new Size(760, 700)); dialog.Arrange(new Rect(0, 0, 760, dialog.DesiredSize.Height)); dialog.UpdateLayout();
                var retry = Assert.IsType<Button>(dialog.FindName("RetryButton"));
                Assert.False(retry.IsDefault); Assert.Equal("Mit MP4Box erneut schneiden", retry.Content);
                var panel = Assert.IsType<StackPanel>(retry.Parent);
                var cancel = Assert.IsType<Button>(panel.Children[0]); Assert.True(cancel.IsDefault); Assert.True(cancel.IsCancel);
                var text = Assert.IsType<TextBlock>(dialog.FindName("DiagnosisText")); Assert.Equal(error.Message, text.Text);
                Assert.True(dialog.DesiredSize.Height <= 700); dialog.Close();
            }
            catch (Exception error) { failure = error; }
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); Assert.True(thread.Join(TimeSpan.FromSeconds(15)));
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
    [Theory]
    [InlineData(true, true, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    public void ShowsOnlyProvenActionsWithCancelAsDefault(bool repairable, bool mp4Available, bool canOmitTrack)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var media = new MediaAnalysisResult("mp4", "MP4", 1, TimeSpan.FromSeconds(10), [], [new(1, "aac", null, 48000, 2, "stereo")]);
                var error = new OtrCanAudioFailureException(media, media, new InvalidOperationException("[audio-diagnostic-v1] gap"));
                var repair = new AacGapRepairOffer(repairable ? "512 fehlende Samples" : "Fehlender Endton", repairable ? (_, _, _) => throw new Exception("should only render") : null);
                var tracks = canOmitTrack ? new AudioTrackRecoveryOffer("AC-3 entfällt; AAC bleibt", _ => throw new Exception("only render")) :
                    new AudioTrackRecoveryOffer("Keine nachgewiesene Restspur");
                var dialog = new AudioMp4BoxFallbackDialog(new AudioRecoveryOffer(error, repair, mp4Available, "nicht gefunden", TrackRecovery: tracks));
                dialog.Measure(new Size(620, 700)); dialog.Arrange(new Rect(0, 0, 620, dialog.DesiredSize.Height)); dialog.UpdateLayout();
                var retry = Assert.IsType<Button>(dialog.FindName("RetryButton"));
                var action = Assert.IsType<Button>(dialog.FindName("RepairButton"));
                Assert.Equal("Mit MP4Box schneiden", retry.Content); Assert.Equal(mp4Available, retry.IsEnabled);
                Assert.Equal(mp4Available ? Visibility.Visible : Visibility.Collapsed, retry.Visibility);
                var trackButton = Assert.IsType<Button>(dialog.FindName("TrackButton"));
                Assert.Equal(canOmitTrack ? Visibility.Visible : Visibility.Collapsed, trackButton.Visibility);
                Assert.Equal(canOmitTrack, trackButton.IsEnabled);
                Assert.False(trackButton.IsDefault);
                Assert.Contains(canOmitTrack ? "AAC bleibt" : "Keine nachgewiesene Restspur",
                    Assert.IsType<TextBlock>(dialog.FindName("TrackStatusText")).Text);
                Assert.Equal(repairable ? Visibility.Visible : Visibility.Collapsed, action.Visibility);
                Assert.Equal(repairable, action.IsEnabled); Assert.False(action.IsDefault); Assert.False(retry.IsDefault);
                var panel = Assert.IsType<StackPanel>(retry.Parent);
                var cancel = Assert.IsType<Button>(panel.Children[0]); Assert.True(cancel.IsDefault); Assert.True(cancel.IsCancel);
                Assert.Equal(AudioRecoveryChoice.Cancel, dialog.SelectedChoice);
                Assert.Contains(repairable ? "512" : "Fehlender Endton", Assert.IsType<TextBlock>(dialog.FindName("RepairStatusText")).Text);
                Assert.True(dialog.DesiredSize.Height <= 700);
                dialog.Close();
            }
            catch (Exception error) { failure = error; }
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); Assert.True(thread.Join(TimeSpan.FromSeconds(15)));
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
