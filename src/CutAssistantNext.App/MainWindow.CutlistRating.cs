using System.Net.Http;
using System.Windows;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App;

public partial class MainWindow
{
    private CutlistRatingWorkflow _cutlistRatings = new(new CutlistRatingSettingsStore());
    private bool _ratingTransitionInProgress;

    internal Func<bool>? CutlistReplacementPrompt { get; set; }
    internal Func<bool>? MediaReplacementPrompt { get; set; }

    // The UI decisions can be supplied by controlled WPF tests; production always uses the dialogs.
    internal Func<UsedCutlistRatingContext, Task<CutlistRatingDecision>>? RatingQuestion { get; set; }
    internal Func<UsedCutlistRatingContext, Task<bool>>? RatingEditor { get; set; }

    private async Task<bool> ConfirmRatingDepartureAsync(CutlistRatingSource? next = null)
    {
        if (_ratingTransitionInProgress) return false;
        _ratingTransitionInProgress = true;
        var enabled = IsEnabled;
        IsEnabled = false;
        try
        {
            return await _cutlistRatings.CanLeaveAsync(
                RatingQuestion ?? ShowRatingQuestionAsync, RatingEditor ?? ShowRatingEditorAsync, next);
        }
        catch
        {
            return MessageBox.Show(this,
                "Der Bewertungsstatus konnte nicht sicher gelesen werden. Ohne Bewertung fortfahren? Bei Nein bleibt die Schnittarbeit erhalten.",
                "Cutlist-Bewertung", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes;
        }
        finally
        {
            _ratingTransitionInProgress = false;
            if (!_isClosed) IsEnabled = enabled;
        }
    }

    private Task<CutlistRatingDecision> ShowRatingQuestionAsync(UsedCutlistRatingContext context)
    {
        var dialog = new CutlistRatingQuestionDialog(context) { Owner = this };
        dialog.ShowDialog();
        return Task.FromResult(dialog.Decision);
    }

    private Task<bool> ShowRatingEditorAsync(UsedCutlistRatingContext context)
    {
        // Do not forward the credential to a redirect target or replay a POST after a redirect.
        using var handler = new HttpClientHandler { AllowAutoRedirect = false };
        using var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
        var client = new CutlistServerClient(http);
        var service = new CutlistRatingService(new CutlistRatingSettingsStore(), client.RateAsync);
        var dialog = new CutlistRatingDialog(context, service) { Owner = this };
        dialog.ShowDialog();
        return Task.FromResult(dialog.Submitted);
    }
}
