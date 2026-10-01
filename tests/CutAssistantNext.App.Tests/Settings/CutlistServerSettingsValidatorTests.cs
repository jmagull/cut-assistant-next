using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class CutlistServerSettingsValidatorTests
{
    [Theory]
    [InlineData("http://cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    public void TryValidatePersonalServerUrl_AcceptsSupportedServerUrl(string url)
    {
        var isValid =
            CutlistServerSettingsValidator.TryValidatePersonalServerUrl(
                url,
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
    [InlineData("http://www.cutlist.at/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("http://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("https://sniplist.mepaso.net.example.org/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/")]
    [InlineData("https://sniplist.mepaso.net/FRED/")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/#")]
    [InlineData("https://sniplist.mepaso.net/0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef/?x=1")]
    [InlineData("https://sniplist.mepaso.net/zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz/")]
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