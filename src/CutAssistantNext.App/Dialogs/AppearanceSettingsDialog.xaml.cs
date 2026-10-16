using System.Windows;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Dialogs;

internal partial class AppearanceSettingsDialog : Window
{
    private readonly AppearanceSettingsStore _store;
    internal AppearanceSettings Selection { get; private set; }

    internal AppearanceSettingsDialog(AppearanceSettingsStore store)
    {
        InitializeComponent();
        _store = store;
        Selection = store.Load();
        StandardOption.IsChecked = Selection.Layout == InterfaceLayout.Standard;
        PowerOption.IsChecked = Selection.Layout == InterfaceLayout.PowerUser;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var selection = new AppearanceSettings(PowerOption.IsChecked == true
            ? InterfaceLayout.PowerUser : InterfaceLayout.Standard);
        if (!_store.Save(selection))
        {
            MessageBox.Show(this, "Die Darstellung konnte nicht gespeichert werden.", "Darstellung",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Selection = selection;
        DialogResult = true;
    }
}
