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
}
