using System.IO;
using CutAssistantNext.App.Settings;
using System.Windows;
using CutAssistantNext.App.ViewModels;
using System.Windows.Controls;

namespace CutAssistantNext.App.Dialogs;

public partial class CutlistGenerationDialog : Window
{
    private readonly WindowSettingsStore _windowSettingsStore;

    internal CutlistGenerationDialog(
        CutlistGenerationViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        var mainSettingsPath =
            WindowSettingsStore.GetDefaultSettingsFilePath();

        var cutlistSettingsPath = Path.Combine(
            Path.GetDirectoryName(mainSettingsPath)!,
            "cutlist-window-settings.json");

        _windowSettingsStore =
            new WindowSettingsStore(cutlistSettingsPath);

        RestoreWindowSize();

        Closed += (_, _) => SaveWindowSize();

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

    private void RestoreWindowSize()
    {
        var settings = _windowSettingsStore.Load();

        if (settings is null)
        {
            return;
        }

        if (double.IsFinite(settings.Width) &&
            settings.Width >= MinWidth)
        {
            Width = Math.Clamp(
                settings.Width,
                MinWidth,
                Math.Max(
                    MinWidth,
                    SystemParameters.WorkArea.Width));
        }

        if (double.IsFinite(settings.Height) &&
            settings.Height >= MinHeight)
        {
            Height = Math.Clamp(
                settings.Height,
                MinHeight,
                Math.Max(
                    MinHeight,
                    SystemParameters.WorkArea.Height));
        }
    }

    private void SaveWindowSize()
    {
        var bounds =
            WindowState == WindowState.Normal
                ? new Rect(0, 0, ActualWidth, ActualHeight)
                : RestoreBounds;

        if (!double.IsFinite(bounds.Width) ||
            !double.IsFinite(bounds.Height) ||
            bounds.Width <= 0 ||
            bounds.Height <= 0)
        {
            return;
        }

        _windowSettingsStore.Save(
            new WindowSettings
            {
                Width = bounds.Width,
                Height = bounds.Height
            });
    }
}
