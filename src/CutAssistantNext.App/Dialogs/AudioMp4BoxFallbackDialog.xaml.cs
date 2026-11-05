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
        RetryButton.Content = "Mit MP4Box schneiden";
        RetryButton.IsEnabled = offer.Mp4BoxAvailable;
        if (!offer.Mp4BoxAvailable) Mp4BoxStatusText.Text = "MP4Box ist über die aktuellen Einstellungen nicht verfügbar: " + offer.Mp4BoxReason;
    }
    private void RetryClick(object sender, RoutedEventArgs e)
    { SelectedChoice = AudioRecoveryChoice.Mp4Box; DialogResult = true; }
    private void RepairClick(object sender, RoutedEventArgs e)
    { if (RepairButton.IsEnabled) { SelectedChoice = AudioRecoveryChoice.RepairAacGaps; DialogResult = true; } }
}
