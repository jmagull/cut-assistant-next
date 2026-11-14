using System.Windows;

namespace CutAssistantNext.App.Dialogs;

public partial class CutlistEndFragmentDialog : Window
{
    internal CutlistEndFragmentDialog(string message, bool allowUnchangedImport, bool allowRemoval = true)
    {
        InitializeComponent();
        Finding.Text = message;
        KeepButton.Visibility = allowUnchangedImport ? Visibility.Visible : Visibility.Collapsed;
        RemoveButton.Visibility = allowRemoval ? Visibility.Visible : Visibility.Collapsed;
    }

    internal bool RemoveFragment { get; private set; }

    private void Keep(object sender, RoutedEventArgs e) => DialogResult = true;

    private void Remove(object sender, RoutedEventArgs e)
    {
        RemoveFragment = true;
        DialogResult = true;
    }
}
