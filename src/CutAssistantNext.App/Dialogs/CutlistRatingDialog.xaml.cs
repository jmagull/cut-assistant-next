using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Dialogs;

internal sealed record CutlistRatingChoice(int Value, string Description);

internal partial class CutlistRatingDialog : Window
{
    private readonly UsedCutlistRatingContext _context;
    private readonly CutlistRatingService _service;
    private readonly CancellationTokenSource _cancellation = new();
    private bool _sending;
    private bool _attempted;
    internal bool Submitted { get; private set; }

    internal CutlistRatingDialog(UsedCutlistRatingContext context, CutlistRatingService service)
    {
        _context = context;
        _service = service;
        InitializeComponent();
        IdentityText.Text = $"{context.Source.Name}\nAutor: {context.Source.Author}\n" +
            $"Server: {context.Source.Endpoint.Host} · ID: {context.Source.CutlistId}\nFilm: {Path.GetFileName(context.MediaPath)}";
        RatingChoices.ItemsSource = new CutlistRatingChoice[]
        {
            new(0, "0 – Test-/Dummy-Cutlist"),
            new(1, "1 – Anfang und Ende gekürzt, Werbung teilweise vorhanden"),
            new(2, "2 – Werbung entfernt, aber ungenau (±5 Sekunden)"),
            new(3, "3 – Relativ genau (±1 Sekunde)"),
            new(4, "4 – Framegenau geschnitten"),
            new(5, "5 – Perfekt, gegebenenfalls einschließlich entfernter doppelter Szenen")
        };
        Closed += (_, _) => _cancellation.Dispose();
    }

    private void RatingSelectionChanged(object sender, SelectionChangedEventArgs e) =>
        SendButton.IsEnabled = !_attempted && !_sending && RatingChoices.SelectedItem is CutlistRatingChoice;

    private async void SendClick(object sender, RoutedEventArgs e) => await SendSelectedAsync();

    internal async Task SendSelectedAsync()
    {
        if (_sending || _attempted || RatingChoices.SelectedItem is not CutlistRatingChoice choice) return;
        _sending = true;
        _attempted = true;
        SendButton.IsEnabled = false;
        RatingChoices.IsEnabled = false;
        CancelButton.IsEnabled = false;
        StatusText.Text = "Bewertung wird übermittelt …";
        try
        {
            var result = await _service.SubmitAsync(_context.Source, choice.Value, _cancellation.Token);
            Submitted = result.Accepted;
            StatusText.Text = result.Message;
            if (Submitted)
            {
                SendButton.Content = "Weiter";
                SendButton.Click -= SendClick;
                SendButton.Click += (_, _) => DialogResult = true;
                SendButton.IsEnabled = true;
            }
        }
        finally
        {
            _sending = false;
            CancelButton.IsEnabled = true;
            CancelButton.Content = Submitted ? "Schließen" : "Zur Schnittarbeit";
        }
    }

    private void DialogClosing(object? sender, CancelEventArgs e)
    {
        if (!_sending) return;
        _cancellation.Cancel();
        e.Cancel = true; // Wait for the result before disposing resources or continuing a transition.
        StatusText.Text = "Übertragung wird abgebrochen; das Ergebnis kann unklar sein …";
    }
}
