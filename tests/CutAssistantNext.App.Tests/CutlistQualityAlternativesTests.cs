using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Tests;

public sealed class CutlistQualityAlternativesTests
{
    [Theory]
    [InlineData(".HQ.avi", ".HD.mp4", "HD")]
    [InlineData(".HD.mp4", ".HQ.avi", "HQ")]
    [InlineData(".avi", ".HD.mp4", "HD")]
    public void QueriesOnlyOtherQualitiesOfTheExactBroadcast(string original, string alternative, string quality)
    {
        const string stem = "Titel_26.11.09_20-15_sender_60_TVOON_DE";
        var file = stem + alternative;
        var queries = CutlistQualityAlternatives.GetQueries(stem + original);
        Assert.Contains((file, quality), queries);
        Assert.DoesNotContain(queries, q => q.FileName == stem + original);
        Assert.Empty(CutlistQualityAlternatives.GetQueries("Eigener.Film.HQ.avi"));
        var result = new CutlistSearchResult { Id = "42", CutlistFileName = file + ".cutlist" };
        Assert.True(CutlistQualityAlternatives.Matches(result, file));
        Assert.False(CutlistQualityAlternatives.Matches(result, file.Replace("20-15", "20-30")));
        var row = CutlistResultRow.FromServer(result, "http://fixture.invalid/", quality);
        Assert.True(row.IsAlternativeQuality);
        Assert.Contains("Dateigröße", row.Details);
    }
}
