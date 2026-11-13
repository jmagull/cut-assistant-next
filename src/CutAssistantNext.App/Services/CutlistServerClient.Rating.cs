using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services;

internal sealed partial class CutlistServerClient
{
    internal async Task<CutlistRatingResult> RateAsync(CutlistRatingSource source, int rating, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (rating is < 0 or > 5) throw new ArgumentOutOfRangeException(nameof(rating));
        using var request = new HttpRequestMessage(HttpMethod.Post, source.Endpoint);
        request.Headers.Add("X-Authorization", "Bearer " + source.Fred);
        request.Headers.Accept.ParseAdd("application/json");
        request.Headers.UserAgent.ParseAdd("CutAssistantNext");
        request.Content = JsonContent.Create(new { id = source.CutlistId, rating });
        try
        {
            using var response = await _httpClient.SendAsync(request, token);
            if (response.StatusCode != HttpStatusCode.OK)
                return new((int)response.StatusCode >= 500 ? CutlistRatingState.Unknown : CutlistRatingState.Rejected,
                    $"Der Server hat die Bewertung nicht bestätigt (HTTP {(int)response.StatusCode}). " +
                    "Mögliche Gründe sind eine eigene oder bereits bewertete Cutlist oder eine ungültige Benutzerkennung. Kein erneuter Versand.");

            var body = await response.Content.ReadAsStringAsync(token);
            if (HasExplicitRatingError(body))
                return new(CutlistRatingState.Rejected, "Der Server meldet einen Fehler oder eine Ablehnung der Bewertung. Kein erneuter Versand.");
            return new(CutlistRatingState.Accepted, "Bewertung an den Server übermittelt.");
        }
        catch (OperationCanceledException)
        {
            return new(CutlistRatingState.Unknown,
                "Übertragung abgebrochen oder Zeitüberschreitung. Ergebnis unklar; keine automatische Wiederholung.");
        }
        catch (Exception)
        {
            return new(CutlistRatingState.Unknown,
                "Der Server konnte nicht sicher erreicht werden. Ergebnis unklar; keine automatische Wiederholung.");
        }
    }

    internal static bool HasExplicitRatingError(string body)
    {
        var text = body.Trim();
        if (text.Equals("false", StringComparison.OrdinalIgnoreCase) ||
            text.Equals("error", StringComparison.OrdinalIgnoreCase) ||
            text.Equals("failed", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("Error ", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("Fehler:", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("<title>Error", StringComparison.OrdinalIgnoreCase)) return true;
        if (text.StartsWith("error=", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("error:", StringComparison.OrdinalIgnoreCase))
        {
            var value = text[6..].Trim();
            return value.Length != 0 && value != "0" && !value.Equals("false", StringComparison.OrdinalIgnoreCase);
        }
        try
        {
            using var json = JsonDocument.Parse(text);
            return HasError(json.RootElement);
        }
        catch (JsonException) { return false; } // HTTP 200 is the current web client's success criterion.
    }

    private static bool HasError(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object) return false;
        foreach (var field in element.EnumerateObject())
        {
            if ((field.Name.Equals("success", StringComparison.OrdinalIgnoreCase) ||
                 field.Name.Equals("ok", StringComparison.OrdinalIgnoreCase)) && field.Value.ValueKind == JsonValueKind.False)
                return true;
            if (field.Name.Equals("status", StringComparison.OrdinalIgnoreCase) && field.Value.ValueKind == JsonValueKind.String &&
                field.Value.GetString() is { } status &&
                (status.Equals("error", StringComparison.OrdinalIgnoreCase) ||
                 status.Equals("failed", StringComparison.OrdinalIgnoreCase) ||
                 status.Equals("failure", StringComparison.OrdinalIgnoreCase))) return true;
            if (field.Name.Equals("error", StringComparison.OrdinalIgnoreCase) ||
                field.Name.Equals("errors", StringComparison.OrdinalIgnoreCase))
            {
                var value = field.Value;
                if (value.ValueKind is JsonValueKind.Null or JsonValueKind.False) continue;
                if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var number) && number == 0) continue;
                if (value.ValueKind == JsonValueKind.String && value.GetString() is { } s &&
                    (string.IsNullOrWhiteSpace(s) || s == "0" || s.Equals("false", StringComparison.OrdinalIgnoreCase))) continue;
                if (value.ValueKind == JsonValueKind.Array && value.GetArrayLength() == 0) continue;
                return true;
            }
        }
        return false;
    }
}
