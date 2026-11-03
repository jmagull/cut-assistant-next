using System.Collections.Concurrent;
using System.Text;
using CutAssistantNext.App.Services;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests.Services;

public sealed class LocalCutlistDiscoveryTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"can-local-{Guid.NewGuid():N}");
    private string Movie => Path.Combine(_root, "Original", "Aufnahme.HQ.mp4");
    private string Own => Path.Combine(_root, "Cutlists");
    private string Alongside => Movie + ".cutlist";
    private string Configured => Path.Combine(Own, "Aufnahme.HQ.mp4.cutlist");
    internal static CutlistMediaContext Context(string movie) => new(movie,
        new MediaAnalysisResult("mp4", null, 1234, TimeSpan.FromSeconds(100),
            [new VideoStreamInfo(0, "h264", null, 1920, 1080, null, null, 25, null)], []), 1);
    internal static byte[] Bytes(string comment = "bisherige Arbeit", string apply = "Aufnahme.HQ.mp4", string size = "1234", string fps = "25") =>
        Encoding.UTF8.GetBytes($"""
            [General]
            ApplyToFile={apply}
            OriginalFileSizeBytes={size}
            FramesPerSecond={fps}
            NoOfCuts=3
            [Cut0]
            Start=0
            Duration=10
            [Cut1]
            Start=20
            Duration=20
            [Cut2]
            Start=50
            Duration=50
            [Info]
            RatingByAuthor=4
            Author=Testautor
            UserComment={comment}
            """);

    public LocalCutlistDiscoveryTests()
    {
        Directory.CreateDirectory(Own);
        Directory.CreateDirectory(Path.GetDirectoryName(Movie)!);
    }
    public void Dispose() => Directory.Delete(_root, true);

    [Fact]
    public async Task ReadsOnlyTwoExactPathsAndNeverSimilarNamesOrDirectories()
    {
        var reads = new ConcurrentBag<string>();
        var service = new LocalCutlistDiscoveryService((path, _) =>
        { reads.Add(path); return Task.FromResult<byte[]?>(Bytes()); });
        var result = await service.SearchAsync(Context(Movie), Own);
        Assert.Equal(new[] { Configured, Alongside }.Order(), reads.Order());
        var row = Assert.Single(result.Rows);
        Assert.Equal(2, row.Local!.Paths.Count);
        Assert.True(row.CanLoad);
        Assert.Equal("Plausibel passend", row.Finding);
        Assert.DoesNotContain("Unfertig", row.Details);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EitherAllowedDirectoryCanSupplyTheOnlyCandidate(bool own)
    {
        var path = own ? Configured : Alongside;
        await File.WriteAllBytesAsync(path, Bytes());
        var before = await File.ReadAllBytesAsync(path);
        var result = await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own);
        Assert.Equal(path, Assert.Single(result.Rows).Local!.Paths.Single());
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    [Fact]
    public void SameDirectoryWithDifferentCaseAndTrailingSeparatorIsProbedOnce()
    {
        var paths = LocalCutlistDiscoveryService.CandidatePaths(Movie, Path.GetDirectoryName(Movie)!.ToUpperInvariant() + "\\");
        Assert.Single(paths);
        Assert.EndsWith("Aufnahme.HQ.mp4.cutlist", paths[0]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("relative")]
    public void MissingOrRelativeConfigurationDoesNotAddAnotherSearchDirectory(string? configured) =>
        Assert.Equal(Alongside, Assert.Single(LocalCutlistDiscoveryService.CandidatePaths(Movie, configured)));

    [Fact]
    public async Task DifferentCommentsOfSameLengthRemainSeparateCandidates()
    {
        await File.WriteAllBytesAsync(Configured, Bytes("Arbeit A"));
        await File.WriteAllBytesAsync(Alongside, Bytes("Arbeit B"));
        var result = await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own);
        Assert.Equal(2, result.Rows.Count);
        Assert.All(result.Rows, row => Assert.Single(row.Local!.Paths));
    }

    [Fact]
    public async Task ReadOnlyCutlistIsFoundAndNeverWritten()
    {
        await File.WriteAllBytesAsync(Configured, Bytes());
        File.SetAttributes(Configured, FileAttributes.ReadOnly);
        try
        {
            var result = await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own);
            Assert.True(Assert.Single(result.Rows).CanLoad);
            Assert.Equal(Bytes(), await File.ReadAllBytesAsync(Configured));
        }
        finally { File.SetAttributes(Configured, FileAttributes.Normal); }
    }

    [Fact]
    public async Task BrokenFirstFileDoesNotHideValidSecondFile()
    {
        await File.WriteAllTextAsync(Configured, "[General]\nNoOfCuts=1");
        await File.WriteAllBytesAsync(Alongside, Bytes());
        var rows = (await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own)).Rows;
        Assert.Equal(2, rows.Count);
        Assert.Single(rows, row => row.CanLoad);
        Assert.Single(rows, row => row.Finding == "Nicht verarbeitbar");
    }

    [Fact]
    public async Task UnreachableOrSlowFirstPathDoesNotPreventSecondResult()
    {
        var service = new LocalCutlistDiscoveryService((path, _) => path == Configured
            ? new TaskCompletionSource<byte[]?>().Task : Task.FromResult<byte[]?>(Bytes()), TimeSpan.FromSeconds(1));
        var result = await service.SearchAsync(Context(Movie), Own);
        Assert.Equal(Alongside, Assert.Single(result.Rows).Local!.Paths.Single());
        Assert.True(result.HasIssue);
    }

    [Fact]
    public async Task InvalidDeclaredCutCountIsBoundedWithoutChangingManualParserRules()
    {
        await File.WriteAllTextAsync(Configured, "[General]\nNoOfCuts=2000000000\n[Info]\nRatingByAuthor=5");
        var row = Assert.Single((await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own)).Rows);
        Assert.False(row.CanLoad);
        Assert.Contains("zu viele", row.Details);
    }

    [Theory]
    [InlineData("", "", "", true, "Unklar")]
    [InlineData("AndererFilm.mp4", "1234", "25", false, "Widerspruch")]
    [InlineData("Aufnahme.HQ.mp4", "1234", "50", false, "Widerspruch")]
    [InlineData("Aufnahme.HQ.mp4", "3000", "25", true, "Unklar")]
    public async Task SeparatesMissingOrContradictoryIdentityFromDocumentValidity(string apply, string size, string fps,
        bool selectable, string finding)
    {
        await File.WriteAllBytesAsync(Configured, Bytes(apply: apply, size: size, fps: fps));
        var row = Assert.Single((await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own)).Rows);
        Assert.NotNull(row.Document);
        Assert.Equal(selectable, row.CanLoad);
        Assert.Equal(finding, row.Finding);
    }

    [Fact]
    public async Task OversizedFilesAreNotParsedAndOtherPathStillWorks()
    {
        using (var file = File.Create(Configured)) file.SetLength(LocalCutlistDiscoveryService.MaximumFileBytes + 1);
        await File.WriteAllBytesAsync(Alongside, Bytes());
        var result = await new LocalCutlistDiscoveryService().SearchAsync(Context(Movie), Own);
        Assert.Single(result.Rows);
        Assert.True(result.HasIssue);
    }

    [Fact]
    public async Task ValidDocumentForLongerMediaIsAnIdentityConflictRatherThanCorrupt()
    {
        await File.WriteAllBytesAsync(Configured, Bytes());
        var context = Context(Movie);
        context = context with { Analysis = context.Analysis with { Duration = TimeSpan.FromSeconds(60) } };
        var row = Assert.Single((await new LocalCutlistDiscoveryService().SearchAsync(context, Own)).Rows);
        Assert.NotNull(row.Document);
        Assert.Equal("Widerspruch", row.Finding);
        Assert.False(row.CanLoad);
        Assert.Contains("Videoende", row.Details);
    }
}
