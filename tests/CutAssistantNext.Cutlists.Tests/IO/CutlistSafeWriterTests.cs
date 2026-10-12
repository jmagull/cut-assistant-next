using System.Text;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.Cutlists.Tests.IO;

public sealed class CutlistSafeWriterTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-safe-cutlist-{Guid.NewGuid():N}");
    private string Target => Path.Combine(_root, "Aufnahme.HQ.mp4.cutlist");

    public CutlistSafeWriterTests() => Directory.CreateDirectory(_root);
    public void Dispose() => Directory.Delete(_root, recursive: true);

    private static CutlistDocument Document() => new(
        new CutlistGeneralMetadata
        {
            ApplyToFile = "Aufnahme.HQ.mp4",
            NoOfCuts = 2,
            FramesPerSecond = 25,
            Application = "Cut Assistant Next",
            Version = "0.2.1"
        },
        [new CutlistKeepSegment(TimeSpan.FromSeconds(1.28), TimeSpan.FromSeconds(9.08)),
         new CutlistKeepSegment(TimeSpan.FromSeconds(19.12), TimeSpan.FromSeconds(20.96))],
        new CutlistInfoMetadata
        {
            SuggestedMovieName = "Ärger [19.09.2026]",
            UserComment = "Werbung vollständig entfernt.",
            Author = "Tester",
            RatingByAuthor = 4
        });

    [Fact]
    public void SavesIdenticalUtf8BytesAndOriginalMetadataWithoutTemporaryResidue()
    {
        var document = Document();
        var expected = CutlistFileWriter.CreateBytes(document);
        CutlistFileWriter.WriteSafely(Target, document);

        var bytes = File.ReadAllBytes(Target);
        Assert.Equal(expected, bytes);
        Assert.False(bytes.Take(3).SequenceEqual(new byte[] { 0xEF, 0xBB, 0xBF }));
        Assert.Contains("Werbung vollständig entfernt.", new UTF8Encoding(false, true).GetString(bytes));
        var restored = CutlistFileReader.Read(Target);
        Assert.Equal(document.General.ApplyToFile, restored.General.ApplyToFile);
        Assert.Equal(document.Info.UserComment, restored.Info.UserComment);
        Assert.Equal(document.Cuts.Select(cut => cut.Start), restored.Cuts.Select(cut => cut.Start));
        Assert.Equal(document.Cuts.Select(cut => cut.Duration), restored.Cuts.Select(cut => cut.Duration));
        Assert.Single(Directory.GetFileSystemEntries(_root));
    }

    [Fact]
    public void DefaultRefusesExistingCutlistWithoutChangingIt()
    {
        File.WriteAllText(Target, "keep old cutlist");
        Assert.Throws<IOException>(() => CutlistFileWriter.WriteSafely(Target, Document()));
        Assert.Equal("keep old cutlist", File.ReadAllText(Target));
        Assert.Single(Directory.GetFileSystemEntries(_root));
    }

    [Fact]
    public void ExplicitConsentReplacesWithCompleteDocumentAndRemovesItsTemporaryFile()
    {
        File.WriteAllText(Target, "old");
        var document = Document();
        CutlistFileWriter.WriteSafely(Target, document, overwriteExisting: true);
        Assert.Equal(CutlistFileWriter.CreateBytes(document), File.ReadAllBytes(Target));
        Assert.Single(Directory.GetFileSystemEntries(_root));
    }

    [Fact]
    public void DoesNotCreateMissingDirectoryOrUseAnAlternative()
    {
        var missing = Path.Combine(_root, "Missing");
        var target = Path.Combine(missing, "Aufnahme.cutlist");
        Assert.Throws<DirectoryNotFoundException>(() => CutlistFileWriter.WriteSafely(target, Document()));
        Assert.False(Directory.Exists(missing));
        Assert.Empty(Directory.GetFileSystemEntries(_root));
    }

    [Fact]
    public void RequiresAbsolutePathForProtectedSave()
    {
        Assert.Throws<ArgumentException>(() => CutlistFileWriter.WriteSafely("relative.cutlist", Document()));
        Assert.Empty(Directory.GetFileSystemEntries(_root));
    }

    [Fact]
    public void NeverDeletesUnrelatedTemporaryFiles()
    {
        var unrelated = Path.Combine(_root, ".can-cutlist-unrelated.tmp");
        File.WriteAllText(unrelated, "not owned by this save");
        CutlistFileWriter.WriteSafely(Target, Document());
        Assert.Equal("not owned by this save", File.ReadAllText(unrelated));
        Assert.Equal(2, Directory.GetFileSystemEntries(_root).Length);
    }

    [Fact]
    public void PublicationFailureAtDirectoryTargetCleansOnlyItsOwnTemporaryFile()
    {
        Directory.CreateDirectory(Target);
        var child = Path.Combine(Target, "preserved.txt");
        File.WriteAllText(child, "preserve");
        Assert.Throws<IOException>(() => CutlistFileWriter.WriteSafely(Target, Document(), overwriteExisting: true));
        Assert.Equal("preserve", File.ReadAllText(child));
        Assert.Single(Directory.GetFileSystemEntries(_root));
    }
}
