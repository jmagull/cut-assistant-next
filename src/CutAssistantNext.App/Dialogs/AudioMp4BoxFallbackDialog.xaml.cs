using System.Windows;
using CutAssistantNext.App.Services.Cutting;

namespace CutAssistantNext.App.Dialogs;

public partial class AudioMp4BoxFallbackDialog : Window
{
    internal AudioMp4BoxFallbackDialog(OtrCanAudioFailureException failure)
    {
        InitializeComponent();
        DiagnosisText.Text = failure.Message;
    }

    private void RetryClick(object sender, RoutedEventArgs e) => DialogResult = true;
}
