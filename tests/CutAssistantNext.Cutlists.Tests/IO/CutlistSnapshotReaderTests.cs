using System.Text;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.Tests.IO;

public sealed class CutlistSnapshotReaderTests
{
    private const string Content = "[General]\nNoOfCuts=1\n[Cut0]\nStart=10\nDuration=20\n[Info]\nRatingByAuthor=5\nAuthor=Grüße";

    [Theory]
    [InlineData("utf8")]
    [InlineData("utf8-bom")]
    [InlineData("utf16")]
    [InlineData("windows1252")]
    public void SnapshotAndExistingFileReaderDecodeTheSameLegacyAndBomData(string encodingName)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encoding = encodingName switch
        {
            "utf8" => new UTF8Encoding(false),
            "utf8-bom" => new UTF8Encoding(true),
            "utf16" => Encoding.Unicode,
            _ => Encoding.GetEncoding(1252)
        };
        var bytes = encoding.GetPreamble().Concat(encoding.GetBytes(Content)).ToArray();
        var path = Path.Combine(Path.GetTempPath(), $"can-snapshot-{Guid.NewGuid():N}.cutlist");
        try
        {
            File.WriteAllBytes(path, bytes);
            var snapshot = CutlistFileReader.ReadBytes(bytes);
            var existing = CutlistFileReader.Read(path);
            Assert.Equal("Grüße", snapshot.Info.Author);
            Assert.Equal(existing.Info.Author, snapshot.Info.Author);
            Assert.Equal(existing.Cuts.Single().Start, snapshot.Cuts.Single().Start);
            Assert.Equal(bytes, File.ReadAllBytes(path));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void AutomaticCountLimitDoesNotTightenTheExistingManualParser()
    {
        Assert.Throws<FormatException>(() => CutlistFileReader.ReadBytes(Encoding.UTF8.GetBytes(Content), maximumCuts: 0));
        Assert.Single(CutlistParser.Parse(Content).Cuts);
    }

    [Theory]
    [InlineData("Film.HQ.mp4", "Film.HQ.mp4.cutlist")]
    [InlineData("Film.mpg.HQ.avi", "Film.mpg.HQ.avi.cutlist")]
    [InlineData("Änderung.avi.mp4", "Änderung.avi.mp4.cutlist")]
    public void ExpectedNameRetainsEveryOriginalExtension(string original, string expected) =>
        Assert.Equal(expected, CutlistFileName.FromOriginal(original));
}
