using System.IO;
using System.Windows;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Dialogs;

internal partial class CutlistRatingQuestionDialog : Window
{
    internal CutlistRatingDecision Decision { get; private set; } = CutlistRatingDecision.Cancel;
    internal CutlistRatingQuestionDialog(UsedCutlistRatingContext context)
    {
        InitializeComponent();
        IdentityText.Text = $"{context.Source.Name}\nAutor: {context.Source.Author}\n" +
            $"Server: {context.Source.Endpoint.Host} · ID: {context.Source.CutlistId}\nFilm: {Path.GetFileName(context.MediaPath)}";
    }
    private void RateClick(object sender, RoutedEventArgs e) { Decision = CutlistRatingDecision.Rate; DialogResult = true; }
    private void SkipClick(object sender, RoutedEventArgs e) { Decision = CutlistRatingDecision.Skip; DialogResult = true; }
}
