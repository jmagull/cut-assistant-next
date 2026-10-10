using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.ViewModels;

internal sealed class CutlistSettingsViewModel
{
    internal CutlistSettingsViewModel(
        CutlistSettings settings)
        : this(
            settings,
            CutlistServerSettings.CreateDefault())
    {
    }

    internal CutlistSettingsViewModel(
        CutlistSettings settings,
        CutlistServerSettings serverSettings)
    {
        ArgumentNullException.ThrowIfNull(
            settings);

        ArgumentNullException.ThrowIfNull(
            serverSettings);

        DefaultAuthor =
            settings.DefaultAuthor;

        QuickText1 =
            GetQuickText(
                settings,
                0);

        QuickText2 =
            GetQuickText(
                settings,
                1);

        QuickText3 =
            GetQuickText(
                settings,
                2);

        QuickText4 =
            GetQuickText(
                settings,
                3);

        QuickText5 =
            GetQuickText(
                settings,
                4);

        QuickText6 = settings.QuickText6 ?? string.Empty;

        PersonalServerUrl =
            serverSettings.PersonalServerUrl;
    }

    public string DefaultAuthor { get; set; }

    public string QuickText1 { get; set; }

    public string QuickText2 { get; set; }

    public string QuickText3 { get; set; }

    public string QuickText4 { get; set; }

    public string QuickText5 { get; set; }

    public string QuickText6 { get; set; }

    public string PersonalServerUrl { get; set; }

    internal CutlistSettings CreateSettings()
    {
        var quickTexts =
            new[]
            {
                QuickText1,
                QuickText2,
                QuickText3,
                QuickText4,
                QuickText5
            }
            .Where(
                text =>
                    !string.IsNullOrWhiteSpace(text))
            .ToList();

        return new CutlistSettings
        {
            DefaultAuthor =
                DefaultAuthor,

            QuickTexts =
                quickTexts,

            QuickText6 = QuickText6
        };
    }

    internal bool TryValidatePersonalServerUrl(
        out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(
                PersonalServerUrl))
        {
            errorMessage = null;

            return true;
        }

        return CutlistServerSettingsValidator.TryValidatePersonalServerUrl(
            PersonalServerUrl,
            out errorMessage);
    }

    internal CutlistServerSettings CreateServerSettings()
    {
        return new CutlistServerSettings
        {
            PersonalServerUrl =
                PersonalServerUrl
        };
    }
    private static string GetQuickText(
        CutlistSettings settings,
        int index)
    {
        if (settings.QuickTexts is null || index >= settings.QuickTexts.Count)
        {
            return string.Empty;
        }

        return settings.QuickTexts[index]
            ?? string.Empty;
    }
}
