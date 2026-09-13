using System.Windows;
using System.Windows.Navigation;

namespace CutAssistantNext.App.Dialogs;

public partial class CreditsDialog : Window
{
    private readonly Action<string> _openLink;

    internal CreditsDialog(Action<string> openLink)
    {
        ArgumentNullException.ThrowIfNull(openLink);
        _openLink = openLink;
        InitializeComponent();
    }

    private void ProjectLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        e.Handled = true;
        _openLink(e.Uri.AbsoluteUri);
    }
}
