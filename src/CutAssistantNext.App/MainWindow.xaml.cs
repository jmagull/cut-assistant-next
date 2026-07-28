using System.Windows;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Analysis;
using Microsoft.Win32;

namespace CutAssistantNext.App;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainWindowViewModel(
            new FfprobeRunner());

        DataContext = _viewModel;
    }

    private async void SelectMediaFileButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "MP4-Datei auswählen",
            Filter = "MP4-Dateien (*.mp4)|*.mp4|Alle Dateien (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        await _viewModel.AnalyzeAsync(dialog.FileName);
    }
}
