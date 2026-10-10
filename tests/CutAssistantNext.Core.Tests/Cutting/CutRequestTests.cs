using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Core.Tests.Cutting;

public sealed class CutRequestTests
{
    private static string FilePath(string name) => Path.Combine(Path.GetTempPath(), "can-request-tests", name);

    [Fact]
    public void Constructor_PreservesTimesAndSnapshotsCallerCollection()
    {
        var segment = new KeepSegment(TimeSpan.FromTicks(1234567), TimeSpan.FromTicks(7));
        var segments = new List<KeepSegment> { segment };
        var request = new CutRequest(FilePath("original.mp4"), FilePath("output.mp4"), segments);
        segments.Clear();

        Assert.Same(segment, Assert.Single(request.KeepSegments));
        Assert.Equal(1234574, request.KeepSegments[0].End.Ticks);
        Assert.Equal(request.OriginalFilePath, request.SourceFilePath);
        Assert.Null(request.FramesPerSecond);
        Assert.False(request.OverwriteExistingOutput);
        Assert.Throws<NotSupportedException>(() => ((IList<KeepSegment>)request.KeepSegments).Clear());
    }

    [Fact]
    public void WithSourceFilePath_PreservesOriginalTargetAndTiming()
    {
        var request = new CutRequest(FilePath("original.avi"), FilePath("output.mp4"),
            [new KeepSegment(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20))], 25, true);
        var prepared = request.WithSourceFilePath(FilePath("working.mp4"));

        Assert.Equal(request.OriginalFilePath, request.SourceFilePath);
        Assert.Equal(request.OriginalFilePath, prepared.OriginalFilePath);
        Assert.Equal(FilePath("working.mp4"), prepared.SourceFilePath);
        Assert.Equal(request.OutputFilePath, prepared.OutputFilePath);
        Assert.Equal(request.KeepSegments[0].Start, prepared.KeepSegments[0].Start);
        Assert.Equal(request.KeepSegments[0].End, prepared.KeepSegments[0].End);
        Assert.Equal(25, prepared.FramesPerSecond);
        Assert.True(prepared.OverwriteExistingOutput);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("working.mp4")]
    public void WithSourceFilePath_RejectsInvalidExplicitWorkingPath(string? source)
    {
        var request = new CutRequest(FilePath("original.mp4"), FilePath("output.mp4"),
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))]);

        Assert.ThrowsAny<ArgumentException>(() => request.WithSourceFilePath(source!));
        Assert.Equal(request.OriginalFilePath, request.SourceFilePath);
    }

    [Theory]
    [InlineData("relative.mp4", "output.mp4")]
    [InlineData("", "output.mp4")]
    public void Constructor_RejectsMissingOrRelativeOriginal(string original, string output)
    {
        Assert.Throws<ArgumentException>(() => new CutRequest(original, FilePath(output),
            [new KeepSegment(TimeSpan.Zero, TimeSpan.FromSeconds(1))]));
    }

    [Fact]
    public void Constructor_RejectsRelativeOutputAndWorkingPaths()
    {
        KeepSegment[] segments = [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))];
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("original.mp4"), "output.mp4", segments));
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("original.mp4"), FilePath("output.mp4"), segments,
            sourceFilePath: "working.mp4"));
    }

    [Fact]
    public void Constructor_ProtectsOriginalAndWorkingFileFromOutput()
    {
        KeepSegment[] segments = [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))];
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("original.mp4"), FilePath("ORIGINAL.MP4"), segments));
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("original.avi"), FilePath("working.mp4"), segments,
            sourceFilePath: FilePath("working.mp4")));
    }

    [Fact]
    public void Constructor_RejectsEmptyNullAndOverlappingSegments()
    {
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("source.mp4"), FilePath("out.mp4"), []));
        Assert.Throws<ArgumentNullException>(() => new CutRequest(FilePath("source.mp4"), FilePath("out.mp4"), [null!]));
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("source.mp4"), FilePath("out.mp4"),
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(10)), new(TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(2))]));
        Assert.Throws<ArgumentException>(() => new CutRequest(FilePath("source.mp4"), FilePath("out.mp4"),
            [new(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10)), new(TimeSpan.Zero, TimeSpan.FromSeconds(10))]));
    }

    [Fact]
    public void Constructor_AllowsAdjacentSegmentsWithoutChangingThem()
    {
        var request = new CutRequest(FilePath("source.mp4"), FilePath("out.mp4"),
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(10)), new(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5))]);
        Assert.Equal(2, request.KeepSegments.Count);
        Assert.Equal(TimeSpan.FromSeconds(10), request.KeepSegments[1].Start);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-25)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Constructor_RejectsInvalidOptionalFrameRate(double rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CutRequest(FilePath("source.mp4"), FilePath("out.mp4"),
            [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))], rate));
    }

    [Fact]
    public void KeepSegment_RejectsInvalidDurationAndTimeOverflow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new KeepSegment(TimeSpan.FromTicks(-1), TimeSpan.FromTicks(1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new KeepSegment(TimeSpan.Zero, TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => new KeepSegment(TimeSpan.MaxValue, TimeSpan.FromTicks(1)));
    }
}
