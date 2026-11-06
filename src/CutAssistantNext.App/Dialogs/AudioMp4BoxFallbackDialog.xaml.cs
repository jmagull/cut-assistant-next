using System.Windows;
using CutAssistantNext.App.Services.Cutting;

namespace CutAssistantNext.App.Dialogs;

public partial class AudioMp4BoxFallbackDialog : Window
{
    internal AudioMp4BoxFallbackDialog(OtrCanAudioFailureException failure)
    {
        InitializeComponent();
        Title = "Tonverarbeitung abgebrochen";
        DiagnosisText.Text = failure.Message;
    }

    internal AudioRecoveryChoice SelectedChoice { get; private set; } = AudioRecoveryChoice.Cancel;
    internal AudioMp4BoxFallbackDialog(AudioRecoveryOffer offer)
    {
        InitializeComponent();
        HeadingText.Text = "otr-can – Audioproblem erkannt";
        DiagnosisText.Text = offer.Diagnosis;
        RepairStatusText.Visibility = Visibility.Visible;
        RepairStatusText.Text = offer.Repair.IsAvailable ? offer.Repair.Description : "Experimentelle AAC-Reparatur nicht verfügbar: " + offer.Repair.Description;
        RepairHintText.Visibility = offer.Repair.IsAvailable ? Visibility.Visible : Visibility.Collapsed;
        RepairButton.Visibility = offer.Repair.IsAvailable ? Visibility.Visible : Visibility.Collapsed;
        RepairButton.IsEnabled = offer.Repair.IsAvailable;
        if (offer.TrackRecovery is { } tracks)
        {
            TrackStatusText.Visibility = Visibility.Visible;
            TrackStatusText.Text = tracks.IsAvailable ? tracks.Description : "Spur-Abwahl nicht verfügbar. " + tracks.Description;
            TrackButton.Visibility = tracks.IsAvailable ? Visibility.Visible : Visibility.Collapsed;
            TrackButton.IsEnabled = tracks.IsAvailable;
        }
        RetryButton.Content = "Mit MP4Box schneiden";
        RetryButton.IsEnabled = offer.Mp4BoxAvailable;
        RetryButton.Visibility = offer.Mp4BoxAvailable ? Visibility.Visible : Visibility.Collapsed;
        if (!offer.Mp4BoxAvailable) Mp4BoxStatusText.Text = "MP4Box ist über die aktuellen Einstellungen nicht verfügbar: " + offer.Mp4BoxReason;
    }
    private void RetryClick(object sender, RoutedEventArgs e)
    { SelectedChoice = AudioRecoveryChoice.Mp4Box; DialogResult = true; }
    private void TrackClick(object sender, RoutedEventArgs e)
    { if (TrackButton.IsEnabled && TrackButton.Visibility == Visibility.Visible) { SelectedChoice = AudioRecoveryChoice.RetainVerifiedTracks; DialogResult = true; } }
    private void RepairClick(object sender, RoutedEventArgs e)
    { if (RepairButton.IsEnabled) { SelectedChoice = AudioRecoveryChoice.RepairAacGaps; DialogResult = true; } }
}
