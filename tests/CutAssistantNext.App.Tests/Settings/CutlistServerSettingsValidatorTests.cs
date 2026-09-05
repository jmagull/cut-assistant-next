using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class CutlistServerSettingsValidatorTests
{
    [Fact]
    public void TryValidatePersonalServerUrl_AcceptsValidCutlistAtUrl()
    {
        var isValid =
            CutlistServerSettingsValidator.TryValidatePersonalServerUrl(
                "http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/",
                out var errorMessage);

        Assert.True(
            isValid);

        Assert.Null(
            errorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("https://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("http://cutlist.at/0123456789abcdef/")]
    [InlineData("http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")]
    public void TryValidatePersonalServerUrl_RejectsInvalidUrls(
        string url)
    {
        var isValid =
            CutlistServerSettingsValidator.TryValidatePersonalServerUrl(
                url,
                out var errorMessage);

        Assert.False(
            isValid);

        Assert.False(
            string.IsNullOrWhiteSpace(
                errorMessage));
    }
}