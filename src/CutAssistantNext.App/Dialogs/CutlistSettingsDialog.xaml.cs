using System.Net.Http;
using CutAssistantNext.App.Services;
using System.Windows;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

public partial class CutlistSettingsDialog : Window
{
    internal CutlistSettingsDialog(
        CutlistSettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        DataContext =
            viewModel;
    }

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not CutlistSettingsViewModel viewModel)
        {
            return;
        }

        if (!viewModel.TryValidatePersonalServerUrl(
                out var errorMessage))
        {
            MessageBox.Show(
                this,
                errorMessage,
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        DialogResult = true;
    }

    private async void TestConnectionButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not CutlistSettingsViewModel viewModel)
        {
            return;
        }

        if (!viewModel.TryValidatePersonalServerUrl(
                out var errorMessage))
        {
            MessageBox.Show(
                this,
                errorMessage,
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (string.IsNullOrWhiteSpace(
                viewModel.PersonalServerUrl))
        {
            MessageBox.Show(
                this,
                "Bitte zuerst die persönliche Server-URL eintragen.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        TestConnectionButton.IsEnabled =
            false;

        try
        {
            using var httpClient =
                new HttpClient
                {
                    Timeout =
                        TimeSpan.FromSeconds(15)
                };

            var client =
                new CutlistServerClient(
                    httpClient);

            await client.SearchRawAsync(
                viewModel.PersonalServerUrl,
                "CutAssistantNextConnectionTest");

            MessageBox.Show(
                this,
                "Verbindung zum Cutlist-Server erfolgreich.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (HttpRequestException exception)
        {
            MessageBox.Show(
                this,
                $"Der Cutlist-Server hat die Anfrage abgelehnt: {exception.Message}",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch (TaskCanceledException)
        {
            MessageBox.Show(
                this,
                "Der Cutlist-Server hat nicht rechtzeitig geantwortet.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch
        {
            MessageBox.Show(
                this,
                "Die Verbindung zum Cutlist-Server ist fehlgeschlagen.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        finally
        {
            TestConnectionButton.IsEnabled =
                true;
        }
    }
    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}