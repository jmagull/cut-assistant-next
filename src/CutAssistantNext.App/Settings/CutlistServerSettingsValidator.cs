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

        var isCutlistAt = string.Equals(
            uri.Host, "cutlist.at", StringComparison.OrdinalIgnoreCase);
        var isSniplist = string.Equals(
            uri.Host, "sniplist.mepaso.net", StringComparison.OrdinalIgnoreCase);

        if (!isCutlistAt && !isSniplist)
        {
            errorMessage = "Diese Cutlist-Server-Adresse wird nicht unterst\u00fctzt.";
            return false;
        }

        var expectedScheme = isSniplist ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        if (!string.Equals(uri.Scheme, expectedScheme, StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = $"Die pers\u00f6nliche Cutlist-Server-URL muss mit {expectedScheme}:// beginnen.";
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