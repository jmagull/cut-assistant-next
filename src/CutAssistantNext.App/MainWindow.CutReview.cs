using System.IO;
using System.Windows;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private CutCompletionViewModel? _repeatableCutReview;

    private void ClearRepeatableCutReview()
    {
        _repeatableCutReview = null;
        ReopenCutReviewButton.Visibility = Visibility.Collapsed;
    }

    private async void ReopenCutReviewButton_Click(object sender, RoutedEventArgs e)
    {
        if (_repeatableCutReview is not { } completion || !CanOpenMedia) return;

        ReopenCutReviewButton.IsEnabled = false;
        try
        {
            // Check for a missing, moved or inaccessible output before opening the player.
            await Task.Run(() =>
            {
                using var stream = File.OpenRead(completion.OutputFilePath);
            });
            if (ReferenceEquals(completion, _repeatableCutReview) && CanOpenMedia)
                await _cutReviewController.ReviewAsync(completion, index: 0);
        }
        catch (Exception error)
        {
            if (!_shutdownStarted && ReferenceEquals(completion, _repeatableCutReview))
                MessageBox.Show(this, error.Message, "Schnittprüfung konnte nicht geöffnet werden",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            ReopenCutReviewButton.IsEnabled = true;
        }
    }
}
