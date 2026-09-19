using System.Windows;
using CutAssistantNext.App.ViewModels;
using System.Windows.Controls;

namespace CutAssistantNext.App.Dialogs;

public partial class CutlistGenerationDialog : Window
{
    internal CutlistGenerationDialog(
        CutlistGenerationViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        DataContext =
            viewModel;
    }
    internal event EventHandler? SaveRequested;

    private void RatingButton_Checked(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton ||
            radioButton.Tag is not string ratingText ||
            !int.TryParse(
                ratingText,
                out var rating))
        {
            return;
        }

        if (DataContext is not CutlistGenerationViewModel viewModel)
        {
            return;
        }

        viewModel.SelectedRating =
            rating;
    }

    private void QuickTextButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not string quickText)
        {
            return;
        }

        if (DataContext is not CutlistGenerationViewModel viewModel)
        {
            return;
        }

        viewModel.ApplyQuickText(
            quickText);
    }

    private void GenerateSuggestedMovieNameButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not CutlistGenerationViewModel viewModel)
        {
            return;
        }

        viewModel.GenerateSuggestedMovieName();
    }

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not CutlistGenerationViewModel viewModel)
        {
            return;
        }

        if (!viewModel.SelectedRating.HasValue)
        {
            MessageBox.Show(
                this,
                "Bitte eine Bewertung von 0 bis 5 auswählen.",
                "Cutlist speichern",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        SaveRequested?.Invoke(
            this,
            EventArgs.Empty);
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
