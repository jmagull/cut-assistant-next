using System.Windows;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

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
        Power2Option.IsChecked = Selection.Layout == InterfaceLayout.PowerUser2;
        RemoveOption.IsChecked = Selection.CutEditing == CutEditingMode.Remove;
        KeepOption.IsChecked = Selection.CutEditing == CutEditingMode.Keep;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var selection = new AppearanceSettings(Power2Option.IsChecked == true ? InterfaceLayout.PowerUser2
            : PowerOption.IsChecked == true ? InterfaceLayout.PowerUser : InterfaceLayout.Standard,
            KeepOption.IsChecked == true ? CutEditingMode.Keep : CutEditingMode.Remove);
        if (!_store.Save(selection))
        {
            MessageBox.Show(this, "Die Einstellungen konnten nicht gespeichert werden.", "Darstellung und Schnittbearbeitung",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Selection = selection;
        DialogResult = true;
    }
}
