using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Interop;
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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        var monitor = MonitorFromWindow(handle, 2); // Nearest monitor.
        var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        var source = HwndSource.FromHwnd(handle);
        if (!GetMonitorInfo(monitor, ref info) || source?.CompositionTarget is null)
        {
            return;
        }

        var transform = source.CompositionTarget.TransformFromDevice;
        var topLeft = transform.Transform(new Point(info.Work.Left, info.Work.Top));
        var bottomRight = transform.Transform(new Point(info.Work.Right, info.Work.Bottom));
        var availableHeight = Math.Max(1, bottomRight.Y - topLeft.Y - 24);
        MinHeight = Math.Min(MinHeight, availableHeight);
        MaxHeight = availableHeight;
        Height = Math.Min(940, availableHeight);
        Top = topLeft.Y + 12;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr window, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public MonitorRectangle Monitor;
        public MonitorRectangle Work;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorRectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
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

        ServerUrlHelp.Visibility = Visibility.Collapsed;

        if (!viewModel.TryValidatePersonalServerUrl(
                out var errorMessage))
        {
            ServerUrlHelp.Visibility = Visibility.Visible;

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
            ServerUrlHelp.Visibility = Visibility.Visible;

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
            ServerUrlHelp.Visibility = Visibility.Visible;

            MessageBox.Show(
                this,
                $"Der Cutlist-Server hat die Anfrage abgelehnt: {exception.Message}",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch (TaskCanceledException)
        {
            ServerUrlHelp.Visibility = Visibility.Visible;

            MessageBox.Show(
                this,
                "Der Cutlist-Server hat nicht rechtzeitig geantwortet.",
                "Cutlist-Server",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch
        {
            ServerUrlHelp.Visibility = Visibility.Visible;

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