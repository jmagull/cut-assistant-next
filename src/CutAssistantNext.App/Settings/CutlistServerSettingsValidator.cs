namespace CutAssistantNext.App.Settings;

internal static class CutlistServerSettingsValidator
{
    internal static bool TryValidatePersonalServerUrl(
        string? personalServerUrl,
        out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(
                personalServerUrl))
        {
            errorMessage =
                "Bitte die persönliche Cutlist-Server-URL eintragen.";

            return false;
        }

        if (!Uri.TryCreate(
                personalServerUrl,
                UriKind.Absolute,
                out var uri))
        {
            errorMessage =
                "Die persönliche Cutlist-Server-URL ist ungültig.";

            return false;
        }

        if (!string.Equals(
                uri.Scheme,
                Uri.UriSchemeHttp,
                StringComparison.OrdinalIgnoreCase))
        {
            errorMessage =
                "Die persönliche Cutlist-Server-URL muss mit http:// beginnen.";

            return false;
        }

        if (!string.Equals(
                uri.Host,
                "cutlist.at",
                StringComparison.OrdinalIgnoreCase))
        {
            errorMessage =
                "Die persönliche Cutlist-Server-URL muss auf cutlist.at verweisen.";

            return false;
        }

        if (!string.IsNullOrEmpty(
                uri.Query) ||
            !string.IsNullOrEmpty(
                uri.Fragment))
        {
            errorMessage =
                "Die persönliche Cutlist-Server-URL darf keine zusätzlichen Parameter enthalten.";

            return false;
        }

        var path =
            uri.AbsolutePath;

        if (!path.StartsWith(
                "/",
                StringComparison.Ordinal) ||
            !path.EndsWith(
                "/",
                StringComparison.Ordinal))
        {
            errorMessage =
                "Die persönliche Cutlist-Server-URL muss mit / enden.";

            return false;
        }

        var fred =
            path.Trim('/');

        if (fred.Length != 64)
        {
            errorMessage =
                "Der FRED in der persönlichen Cutlist-Server-URL muss genau 64 Zeichen lang sein.";

            return false;
        }

        if (!fred.All(
                character =>
                    Uri.IsHexDigit(
                        character)))
        {
            errorMessage =
                "Der FRED darf nur die Zeichen 0–9 und a–f enthalten.";

            return false;
        }

        errorMessage = null;

        return true;
    }
}