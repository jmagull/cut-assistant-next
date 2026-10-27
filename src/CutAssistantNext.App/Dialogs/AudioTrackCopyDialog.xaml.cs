using System.IO;
using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Dialogs;

public partial class AudioTrackCopyDialog : Window
{
    private readonly MediaAnalysisResult _media;
    internal AudioTrackCopyChoice? SelectedChoice => ChoiceList.SelectedItem as AudioTrackCopyChoice;

    internal AudioTrackCopyDialog(string source, MediaAnalysisResult media)
    {
        _media = media;
        InitializeComponent();
        FileText.Text = "Geladene Datei: " + Path.GetFileName(source);
        ChoiceList.ItemsSource = AudioTrackCopyChoice.Choices(media);
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AcceptButton is null || NoticeText is null) return;
        AcceptButton.IsEnabled = SelectedChoice is not null;
        NoticeText.Text = SelectedChoice is { } choice
            ? choice.Notice(_media) : "Bitte die passende Tonspur bewusst auswählen.";
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (SelectedChoice is not null) DialogResult = true;
    }
}
