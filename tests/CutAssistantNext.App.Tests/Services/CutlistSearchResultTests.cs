using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistSearchResultTests
{
    [Theory]
    [InlineData(
        "Film_26.09.05_20-15_sender_60_TVOON_DE.HQ.mp4.cutlist",
        "MP4 HQ")]
    [InlineData(
        "Film_26.09.05_20-15_sender_60_TVOON_DE.HD.mp4.cutlist",
        "MP4 HD")]
    [InlineData(
        "Film_26.09.05_20-15_sender_60_TVOON_DE.mp4.cutlist",
        "MP4")]
    [InlineData(
        "Film_26.09.05_20-15_sender_60_TVOON_DE.HQ.avi.cutlist",
        "AVI HQ")]
    [InlineData(
        "Film_26.09.05_20-15_sender_60_TVOON_DE.avi.cutlist",
        "AVI")]
    public void DisplayFormat_DerivesFormatFromCutlistFileName(
        string cutlistFileName,
        string expectedFormat)
    {
        var result =
            new CutlistSearchResult
            {
                CutlistFileName =
                    cutlistFileName
            };

        Assert.Equal(
            expectedFormat,
            result.DisplayFormat);
    }
}