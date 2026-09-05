using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutlistSettingsViewModelTests
{
    [Fact]
    public void Constructor_UsesAuthorAndPadsQuickTextsToFiveSlots()
    {
        var settings =
            new CutlistSettings
            {
                DefaultAuthor = "joerg",

                QuickTexts =
                [
                    "Keine Werbung gefunden.",
                    "Werbung vollständig entfernt."
                ]
            };

        var viewModel =
            new CutlistSettingsViewModel(
                settings);

        Assert.Equal(
            "joerg",
            viewModel.DefaultAuthor);

        Assert.Equal(
            "Keine Werbung gefunden.",
            viewModel.QuickText1);

        Assert.Equal(
            "Werbung vollständig entfernt.",
            viewModel.QuickText2);

        Assert.Equal(
            string.Empty,
            viewModel.QuickText3);

        Assert.Equal(
            string.Empty,
            viewModel.QuickText4);

        Assert.Equal(
            string.Empty,
            viewModel.QuickText5);
    }
    [Fact]
    public void CreateSettings_UsesAuthorAndNonEmptyQuickTexts()
    {
        var viewModel =
            new CutlistSettingsViewModel(
                CutlistSettings.CreateDefault());

        viewModel.DefaultAuthor =
            "joerg";

        viewModel.QuickText1 =
            "Keine Werbung gefunden.";

        viewModel.QuickText2 =
            string.Empty;

        viewModel.QuickText3 =
            "Werbung vollständig entfernt.";

        viewModel.QuickText4 =
            "   ";

        viewModel.QuickText5 =
            "Teil 1 von 2";

        var settings =
            viewModel.CreateSettings();

        Assert.Equal(
            "joerg",
            settings.DefaultAuthor);

        Assert.Equal(
            [
                "Keine Werbung gefunden.",
                "Werbung vollständig entfernt.",
                "Teil 1 von 2"
            ],
            settings.QuickTexts);
    }
    [Fact]
    public void Constructor_UsesPersonalServerUrl()
    {
        var serverSettings =
            new CutlistServerSettings
            {
                PersonalServerUrl =
                    "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/"
            };

        var viewModel =
            new CutlistSettingsViewModel(
                CutlistSettings.CreateDefault(),
                serverSettings);

        Assert.Equal(
            serverSettings.PersonalServerUrl,
            viewModel.PersonalServerUrl);
    }

    [Fact]
    public void TryValidatePersonalServerUrl_AllowsEmptyUrl()
    {
        var viewModel =
            new CutlistSettingsViewModel(
                CutlistSettings.CreateDefault(),
                CutlistServerSettings.CreateDefault());

        viewModel.PersonalServerUrl =
            string.Empty;

        var isValid =
            viewModel.TryValidatePersonalServerUrl(
                out var errorMessage);

        Assert.True(
            isValid);

        Assert.Null(
            errorMessage);
    }

    [Fact]
    public void TryValidatePersonalServerUrl_RejectsInvalidEnteredUrl()
    {
        var viewModel =
            new CutlistSettingsViewModel(
                CutlistSettings.CreateDefault(),
                CutlistServerSettings.CreateDefault());

        viewModel.PersonalServerUrl =
            "https://cutlist.at/not-a-valid-fred/";

        var isValid =
            viewModel.TryValidatePersonalServerUrl(
                out var errorMessage);

        Assert.False(
            isValid);

        Assert.False(
            string.IsNullOrWhiteSpace(
                errorMessage));
    }
    [Fact]
    public void CreateServerSettings_UsesPersonalServerUrl()
    {
        var viewModel =
            new CutlistSettingsViewModel(
                CutlistSettings.CreateDefault(),
                CutlistServerSettings.CreateDefault());

        viewModel.PersonalServerUrl =
            "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/";

        var settings =
            viewModel.CreateServerSettings();

        Assert.Equal(
            viewModel.PersonalServerUrl,
            settings.PersonalServerUrl);
    }
}
