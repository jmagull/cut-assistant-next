using System.Windows;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

public partial class VideoInformationDialog : Window
{
    internal VideoInformationDialog(
        MainWindowViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        DataContext =
            viewModel;
    }
}