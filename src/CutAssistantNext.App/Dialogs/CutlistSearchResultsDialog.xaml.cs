using System.Windows;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Dialogs;

internal partial class CutlistSearchResultsDialog
    : Window
{
    internal CutlistSearchResultsDialog(
        string movieFileName,
        IReadOnlyList<CutlistSearchResult> results)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            movieFileName);

        ArgumentNullException.ThrowIfNull(
            results);

        MovieFileName =
            movieFileName;

        Results =
            results;

        InitializeComponent();

        DataContext =
            this;

        if (Results.Count > 0)
        {
            ResultsGrid.SelectedIndex =
                0;
        }
    }

    public string MovieFileName { get; }

    public IReadOnlyList<CutlistSearchResult> Results { get; }

    internal CutlistSearchResult? SelectedResult =>
        ResultsGrid.SelectedItem
            as CutlistSearchResult;

    private void LoadButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (SelectedResult is null)
        {
            MessageBox.Show(
                this,
                "Bitte zuerst eine Cutlist auswählen.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        DialogResult =
            true;
    }

    private void ContinueButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult =
            false;
    }
}