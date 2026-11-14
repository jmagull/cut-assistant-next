using System.IO;
using System.Text;
using CutAssistantNext.App.Services;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.App.Tests.Services;

public sealed class CutlistEndReviewTests
{
    private const string Movie = "Aufnahme.HQ.mp4";
    private static readonly TimeSpan VideoEnd = T(8999.08);
    private static readonly MediaAnalysisResult Media = new("mp4", null, 1272489817, VideoEnd,
        [new(0, "h264", null, 720, 400, null, null, 25, null) { StartTimeSeconds = 0, DurationSeconds = 8999.08 }],
        [new(1, "aac", null, 48000, 2, null) { StartTimeSeconds = 0, DurationSeconds = 8998.632 }])
        { StartTimeSeconds = 0 };

    [Theory]
    [InlineData("FramesPerSecond=0")]
    [InlineData("")]
    [InlineData("FramesPerSecond=")]
    [InlineData("FramesPerSecond=-25")]
    [InlineData("FramesPerSecond=NaN")]
    [InlineData("FramesPerSecond=Infinity")]
    public async Task DirectLocalAndServerDocumentsShareReviewAndPreserveBothChoices(string fps)
    {
        var bytes = Encoding.UTF8.GetBytes(Content(fps));
        var direct = CutlistFileReader.ReadBytes(bytes);
        var local = CutlistResultRow.FromLocal(new LocalCutlistCandidate(Movie + ".cutlist", bytes),
            new CutlistMediaContext(Movie, Media, 1));
        var server = CutlistResultRow.FromServer(new CutlistSearchResult
            { Id = "2081774", Author = "Test", CutlistFileName = Movie + ".cutlist" }, "http://cutlist.at/test/");
        var imported = await CutlistSelectionReader.ReadAsync(server, (_, _, _) => Task.FromResult(bytes));

        Assert.True(local.CanLoad);
        Assert.Contains("Kurzer Endbereich", local.Details);
        foreach (var document in new[] { direct, local.Document!, imported.Document })
        {
            var review = CutlistImportReview.Analyze(document, VideoEnd, Media, Tail(8999000, 8999040));
            var fragment = Assert.IsType<CutlistKeepSegment>(review.Fragment);
            Assert.Equal(T(8999.03), fragment.Start);
            Assert.Equal(1, review.MeasuredFrameCount);
            Assert.True(review.AudioEndsBeforeFragment);
            Assert.Equal(6, CutlistKeepSegmentBuilder.Build(review.Plan).Count);
            var corrected = CutlistEndFragmentCorrector.BuildCutPlan(document, VideoEnd, fragment);
            Assert.Equal(5, CutlistKeepSegmentBuilder.Build(corrected).Count);
            Assert.Equal(T(8334.22), CutlistKeepSegmentBuilder.Build(corrected)[^1].End);
            Assert.Equal(6, document.Cuts.Count);
            Assert.Equal(VideoEnd, review.BuildMp4BoxRange(25).End);
        }
    }

    [Fact]
    public void CorrectedCutlistRetainsAllFiveIntervalsWithoutWarning()
    {
        var document = CutlistParser.Parse(Content("FramesPerSecond=25", includeTail: false));
        var review = CutlistImportReview.Analyze(document, VideoEnd, Media, Tail(8999000, 8999040));
        Assert.Null(review.Fragment);
        Assert.Equal(5, CutlistKeepSegmentBuilder.Build(review.Plan).Count);
    }

    [Theory]
    [InlineData("50", true)]
    [InlineData("25", false)]
    public void ValidFrameRateMismatchRemainsVisible(string fps, bool mismatch)
    {
        var review = CutlistImportReview.Analyze(CutlistParser.Parse(Content("FramesPerSecond=" + fps)), VideoEnd, Media);
        Assert.Equal(mismatch, review.FrameRateMismatch);
        Assert.Equal(mismatch, review.FrameRateNotice is not null);
    }

    [Fact]
    public void VariableIntervalsUseMeasuredFrameCountAndLastIncludedTimestamp()
    {
        var document = CutlistParser.Parse(Content("FramesPerSecond=0", "8998.79", "0.29"));
        var review = CutlistImportReview.Analyze(document, VideoEnd, Media, Tail(8998700, 8998800, 8998930, 8999040));
        Assert.Equal(3, review.MeasuredFrameCount);
        Assert.Equal(T(8999.04), review.BuildMp4BoxRange(25).End);
        Assert.Null(CutlistImportReview.Analyze(document, VideoEnd, Media).Fragment);
    }

    [Theory]
    [InlineData("8999.03", "0.051")]
    [InlineData("8999.08", "0.04")]
    public void OutOfBoundsIsRejectedRatherThanTreatedAsAShortFragment(string start, string duration)
    {
        var document = CutlistParser.Parse(Content("FramesPerSecond=25", start, duration));
        var error = Assert.Throws<InvalidDataException>(() => CutlistImportReview.Analyze(document, VideoEnd, Media));
        Assert.Contains("Das Videoende", error.Message);
    }

    [Fact]
    public void OnlyKeepSegmentCanBeFlaggedButCannotBeRemoved()
    {
        var document = CutlistParser.Parse("""
            [General]
            NoOfCuts=1
            FramesPerSecond=0
            [Cut0]
            Start=8999.03
            Duration=0.05
            [Info]
            RatingByAuthor=0
            """);
        Assert.True(CutlistImportReview.NeedsTailProbe(document.Cuts, VideoEnd));
        var review = CutlistImportReview.Analyze(document, VideoEnd, Media, Tail(8999000, 8999040));
        var fragment = Assert.IsType<CutlistKeepSegment>(review.Fragment);
        Assert.Single(CutlistKeepSegmentBuilder.Build(review.Plan));
        Assert.Throws<ArgumentException>(() => CutlistEndFragmentCorrector.BuildCutPlan(document, VideoEnd, fragment));
    }

    [Fact]
    public void TruncatedFrameProbeFallsBackToMediaRateWithoutInventingAFrameCount()
    {
        var document = CutlistParser.Parse(Content("FramesPerSecond=0"));
        var review = CutlistImportReview.Analyze(document, VideoEnd, Media, Tail(8998800, 8998840));
        Assert.NotNull(review.Fragment);
        Assert.Null(review.MeasuredFrameCount);
    }

    [Fact]
    public void MalformedFrameRateIsReportedAndFrameOnlyInputUsesItsOwnTimeBase()
    {
        Assert.Throws<FormatException>(() => CutlistParser.Parse(Content("FramesPerSecond=unlesbar")));
        var frameOnly = Content("FramesPerSecond=50").Replace("Start=8999.03", "StartFrame=449950")
            .Replace("Duration=0.05", "DurationFrames=4");
        var document = CutlistParser.Parse(frameOnly);
        Assert.Equal(T(8999), document.Cuts[^1].Start);
        Assert.Equal(VideoEnd, document.Cuts[^1].End);
        Assert.True(CutlistImportReview.Analyze(document, VideoEnd, Media).FrameRateMismatch);
        Assert.Throws<FormatException>(() => CutlistParser.Parse(frameOnly.Replace("=50", "=0")));
    }

    private static VideoFrameWindow Tail(params long[] pts) => new(0, new VideoTimeBase(1, 1000), 0, 0,
        pts.Select((time, index) => new VideoFrameInfo(index, time, null, null, 40, false, null)).ToArray());

    private static TimeSpan T(double seconds) => TimeSpan.FromSeconds(seconds);

    private static string Content(string fps, string start = "8999.03", string duration = "0.05", bool includeTail = true) =>
        $"""
        [General]
        {fps}
        ApplyToFile={Movie}
        OriginalFileSizeBytes=1272489817
        NoOfCuts={(includeTail ? 6 : 5)}
        [Cut0]
        Start=629.72
        Duration=1199.47
        [Cut1]
        Start=2391.74
        Duration=1179.14
        [Cut2]
        Start=4127.87
        Duration=1218.53
        [Cut3]
        Start=5895.03
        Duration=1208.29
        [Cut4]
        Start=7668.65
        Duration=665.57
        {(includeTail ? $"""
        [Cut5]
        Start={start}
        Duration={duration}
        """ : "")}
        [Info]
        RatingByAuthor=0
        """;
}
