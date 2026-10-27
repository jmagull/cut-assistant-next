using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Services.Cutting;

namespace CutAssistantNext.App.Dialogs;

public partial class AacEmergencyDialog : Window
{
    private readonly AacEmergencyOffer _offer;
    internal AacEmergencyChoice? SelectedChoice => ChoiceList.SelectedItem as AacEmergencyChoice;

    internal AacEmergencyDialog(AacEmergencyOffer offer)
    {
        _offer = offer;
        InitializeComponent();
        ChoiceList.ItemsSource = offer.Choices;
        if (offer.Choices.Count == 1) ChoiceList.SelectedIndex = 0;
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AcceptButton is null || NoticeText is null) return;
        AcceptButton.IsEnabled = SelectedChoice is not null;
        NoticeText.Text = SelectedChoice is { } choice
            ? _offer.Notice(choice)
            : "Bitte eine AAC-Tonspur bewusst auswählen.";
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (SelectedChoice is not null) DialogResult = true;
    }
}
