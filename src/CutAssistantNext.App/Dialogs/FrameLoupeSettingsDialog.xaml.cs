using System.Globalization;
using System.Windows;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Dialogs;

internal partial class FrameLoupeSettingsDialog : Window
{
    private readonly FrameLoupeSettingsStore _store = new();

    internal FrameLoupeSettingsDialog()
    {
        InitializeComponent();
        InitialFramesBox.Text = _store.Load().InitialSearchFrames.ToString(CultureInfo.InvariantCulture);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(InitialFramesBox.Text, out var frames) ||
            frames < 1 || frames > FrameLoupeSettings.MaximumInitialSearchFrames)
        {
            MessageBox.Show(this, "Bitte eine ganze Zahl zwischen 1 und 100000 eingeben.",
                "Startweite ungültig", MessageBoxButton.OK, MessageBoxImage.Information);
            InitialFramesBox.Focus();
            InitialFramesBox.SelectAll();
            return;
        }

        if (!_store.Save(new FrameLoupeSettings(frames)))
        {
            MessageBox.Show(this, "Die Einstellung konnte nicht gespeichert werden.",
                "Frame-Lupe", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DialogResult = true;
    }
}
