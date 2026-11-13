using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.Metadata;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Cutlists.Serialization;
using CutAssistantNext.Media.Tools;
using SpotlightSegment = SpotlightCan.Core.RemoveSegment;
namespace CutAssistantNext.App.Tests.Services;

public sealed class SpotlightMediaSessionTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-spotlight-" + Guid.NewGuid().ToString("N"));
    private string Source => Path.Combine(_root, "film.mp4");
    public SpotlightMediaSessionTests() { Directory.CreateDirectory(_root); File.WriteAllBytes(Source, new byte[32]); }
    public void Dispose() => Directory.Delete(_root, true);
    private MediaAnalysisResult Analysis() => new("mp4", "fixture", 32, T(10), [new VideoStreamInfo(3, "h264", null, 640, 360, null, null, 30000d / 1001, "progressive")], []);
    private CutlistMediaContext Context() => new(Source, Analysis(), 4);
    private static TimeSpan T(double value) => TimeSpan.FromSeconds(value);
    private static SpotlightSegment S(double start, double end) => new(T(start), T(end));
    private static CutPlanViewModel Editor(CutEditingMode mode = CutEditingMode.Remove) { var editor = new CutPlanViewModel(); editor.Initialize(T(10), mode); return editor; }
    private static void Add(CutPlanViewModel editor, double start, double end) { editor.SelectedRemoveSegment = null; editor.SetStart(T(start)); editor.SetEnd(T(end)); }

    [Fact]
    public async Task ReusesCanMediaAndLocksOnlySourceWhileOpen()
    {
        var context = Context(); using (var session = await SpotlightMediaSession.CaptureAsync(context))
        {
            Assert.Same(context, session.Context); Assert.Equal(Source, session.Media.FilePath); Assert.Equal(T(10), session.Media.Duration); Assert.Equal(3, session.Media.VideoStreamIndex);
            Assert.InRange(session.Media.FrameRate.Value, 29.97m, 29.971m);
            using var reader = File.Open(Source, FileMode.Open, FileAccess.Read, FileShare.Read);
            Assert.Throws<IOException>(() => File.WriteAllText(Source, "changed")); Assert.Throws<IOException>(() => File.Delete(Source));
        }
        File.WriteAllText(Source, "now writable");
    }
    [Fact]
    public async Task NoFilmOrChangedSizeIsRejected()
    {
        var invalid = new CutlistMediaContext(Source, new(null, null, null, null, [], []), 1);
        await Assert.ThrowsAsync<InvalidOperationException>(() => SpotlightMediaSession.CaptureAsync(invalid));
        var context = Context(); File.WriteAllText(Source, "different size");
        await Assert.ThrowsAsync<IOException>(() => SpotlightMediaSession.CaptureAsync(context));
    }
    [Theory]
    [InlineData(CutEditingMode.Remove)]
    [InlineData(CutEditingMode.Keep)]
    public async Task MultipleRemoveSegmentsReachExistingEditorInBothModes(CutEditingMode mode)
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(mode);
        Assert.True(session.TryApply([S(9, 10), S(0, 1), S(4, 5)], editor, () => true, () => throw new Exception("No existing work")));
        Assert.Equal([0d, 4d, 9d], editor.CreateCutPlanSnapshot().RemoveSegments.Select(s => s.Start.TotalSeconds));
        Assert.Equal(T(7), editor.EstimatedOutputDuration); Assert.Equal(mode, editor.EditingMode);
        if (mode == CutEditingMode.Keep) Assert.Equal([1d, 5d], editor.MarkedSegments.Select(s => s.Start.TotalSeconds));
    }
    [Theory]
    [InlineData(CutEditingMode.Remove, false)]
    [InlineData(CutEditingMode.Remove, true)]
    [InlineData(CutEditingMode.Keep, false)]
    [InlineData(CutEditingMode.Keep, true)]
    public async Task NineBlocksAndOptionalEndReachCanWithoutLosingRanges(CutEditingMode mode, bool markEnd)
    {
        var analysis = Analysis() with { Duration = T(120) };
        using var session = await SpotlightMediaSession.CaptureAsync(new(Source, analysis, 4));
        var editor = new CutPlanViewModel(); editor.Initialize(T(120), mode);
        var blocks = Enumerable.Range(0, 9).Select(i => S(5 + 10 * i, 7 + 10 * i)).ToArray();
        var complete = SpotlightCan.Core.CutPlan.Validate(markEnd ? blocks.Append(new SpotlightSegment(TimeSpan.FromMilliseconds(95040), T(120))) : blocks, T(120));
        Assert.True(session.TryApply(complete, editor, () => true, () => true));
        Assert.Equal(complete.Select(s => (s.Start, s.End)), editor.CreateCutPlanSnapshot().RemoveSegments.Select(s => (s.Start, s.End)));
        Assert.Equal(markEnd ? TimeSpan.FromMilliseconds(77040) : T(102), editor.EstimatedOutputDuration);
    }

    [Theory]
    [InlineData(CutEditingMode.Remove, false)]
    [InlineData(CutEditingMode.Remove, true)]
    [InlineData(CutEditingMode.Keep, false)]
    [InlineData(CutEditingMode.Keep, true)]
    public async Task PracticalEofCaseReachesCorrectRedOrGreenPlan(CutEditingMode mode, bool previousAds)
    {
        var duration = SpotlightCan.Core.TimeText.Parse("02:09:58.000");
        var start = SpotlightCan.Core.TimeText.Parse("02:00:03.560");
        using var session = await SpotlightMediaSession.CaptureAsync(new(Source, Analysis() with { Duration = duration }, 4));
        var model = new SpotlightCan.App.MainViewModel(); await model.SetMediaAsync(session.Media);
        try
        {
            if (previousAds) { model.Blocks.Add(S(2, 4)); model.Blocks.Add(S(8, 10)); }
            model.PositionText = "02:00:03.560"; model.MarkStart(); model.CompleteToVideoEnd();
            var complete = model.CreateTransferSnapshot();
            Assert.Equal(new(start, duration), complete.Last());
            Assert.Equal(SpotlightCan.Core.TimeText.Parse("00:09:54.440"), complete.Last().End - complete.Last().Start);
            var editor = new CutPlanViewModel(); editor.Initialize(duration, mode);
            Assert.True(session.TryApply(complete, editor, () => true, () => true));
            Assert.Equal(complete.Select(s => (s.Start, s.End)), editor.CreateCutPlanSnapshot().RemoveSegments.Select(s => (s.Start, s.End)));
            Assert.Equal(start - (previousAds ? T(4) : TimeSpan.Zero), editor.EstimatedOutputDuration);
            Assert.Equal(model.Runtime.Remaining, editor.EstimatedOutputDuration);
            Assert.Equal(mode == CutEditingMode.Keep, editor.IsKeepMode);
            if (mode == CutEditingMode.Keep)
            {
                var expected = SpotlightCan.Core.CutPlan.Complement(complete, duration);
                Assert.Equal(expected.Select(s => (s.Start, s.End)), editor.MarkedSegments.Select(s => (s.Start, s.End)));
                Assert.Equal(start, editor.MarkedSegments.Last().End);
            }
            else Assert.Equal(complete.Select(s => (s.Start, s.End)), editor.MarkedSegments.Select(s => (s.Start, s.End)));
        }
        finally { await model.CloseAsync(); }
    }

    [Fact]
    public async Task DeclinedConfirmationPreservesIdentitySelectionAndOpenStart()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); Add(editor, 2, 3); var original = editor.MarkedSegments.Single();
        editor.SetStart(T(6));
        Assert.False(session.TryApply([S(4, 5)], editor, () => true, () => false));
        Assert.Same(original, editor.MarkedSegments.Single()); Assert.Equal(T(6), editor.PendingStart);
        editor.SelectedRemoveSegment = original; // Existing CAN selection cancels the pending start.
        Assert.False(session.TryApply([S(4, 5)], editor, () => true, () => false));
        Assert.Same(original, editor.SelectedRemoveSegment); Assert.Null(editor.PendingStart);
    }
    [Fact]
    public async Task ConfirmedReplacementUsesOneWholeValidPlan()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); Add(editor, 2, 3); editor.SetStart(T(6)); var calls = 0;
        Assert.True(session.TryApply([S(4, 5)], editor, () => true, () => { calls++; return true; })); Assert.Equal(1, calls); Assert.Null(editor.PendingStart); Assert.Equal(T(4), editor.MarkedSegments.Single().Start);
    }
    [Theory]
    [InlineData("overlap")]
    [InlineData("negative")]
    [InlineData("end")]
    [InlineData("empty")]
    public async Task InvalidTransferNeverTouchesEditor(string kind)
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); Add(editor, 2, 3); var original = editor.MarkedSegments.Single();
        SpotlightSegment[] segments = kind switch { "overlap" => [S(4, 7), S(6, 8)], "negative" => [S(-1, 2)], "end" => [S(9, 11)], _ => [] };
        var asked = false;
        void Transfer() => session.TryApply(segments, editor, () => true, () => { asked = true; return true; });
        if (kind is "negative" or "end") Assert.Throws<ArgumentOutOfRangeException>(Transfer);
        else Assert.Throws<InvalidOperationException>(Transfer);
        Assert.False(asked);
        Assert.Same(original, editor.MarkedSegments.Single());
    }
    [Fact]
    public async Task ChangedMediaBeforeOrDuringConfirmationIsRejected()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); Add(editor, 2, 3); var original = editor.MarkedSegments.Single();
        Assert.Throws<InvalidOperationException>(() => session.TryApply([S(4, 5)], editor, () => false, () => true));
        var current = true;
        Assert.Throws<InvalidOperationException>(() => session.TryApply([S(4, 5)], editor, () => current, () => { current = false; return true; }));
        Assert.Same(original, editor.MarkedSegments.Single());
    }
    [Fact]
    public async Task ContextGenerationAndAnalysisIdentityMustMatch()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var context = session.Context;
        Assert.True(context.IsCurrent(Source, context.Analysis, 4)); Assert.False(context.IsCurrent(Source, context.Analysis, 5));
        Assert.False(context.IsCurrent(Source, Analysis(), 4)); Assert.False(context.IsCurrent(Source + ".other", context.Analysis, 4));
    }
    [Fact]
    public async Task ReentrantEditsAreNeverSilentlyOverwritten()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); Add(editor, 2, 3);
        Assert.Throws<InvalidOperationException>(() => session.TryApply([S(4, 5)], editor, () => true, () => { Add(editor, 6, 7); return true; }));
        Assert.Equal(2, editor.MarkedSegments.Count);
    }
    [Fact]
    public async Task LoadedMetadataAloneStillRequiresConfirmation()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); var asked = false;
        Assert.False(session.TryApply([S(4, 5)], editor, () => true, () => { asked = true; return false; }, true)); Assert.True(asked); Assert.Empty(editor.MarkedSegments);
    }
    [Fact]
    public async Task SubsequentCanCorrectionAndCutlistRoundtripRemainAvailable()
    {
        using var session = await SpotlightMediaSession.CaptureAsync(Context()); var editor = Editor(); session.TryApply([S(0, 1), S(4, 5), S(9, 10)], editor, () => true, () => true);
        editor.SelectedRemoveSegment = editor.MarkedSegments[1]; editor.SetEnd(T(6)); var plan = editor.CreateCutPlanSnapshot();
        var keeps = CutlistKeepSegmentBuilder.Build(plan); var document = new CutlistDocument(new CutlistGeneralMetadata { NoOfCuts = keeps.Count, ApplyToFile = "film.mp4" }, keeps, new CutlistInfoMetadata());
        var read = CutlistParser.Parse(CutlistSerializer.Serialize(document)); var roundtrip = CutlistCutPlanBuilder.Build(read, T(10));
        Assert.Equal([1d, 6d], read.Cuts.Select(s => s.Start.TotalSeconds)); Assert.Equal([3d, 3d], read.Cuts.Select(s => s.Duration.TotalSeconds));
        Assert.Equal(plan.RemoveSegments.Select(s => (s.Start, s.End)), roundtrip.RemoveSegments.Select(s => (s.Start, s.End)));
    }
    [Fact]
    public async Task BothConfiguredPathsUseExistingToolVersionChecks()
    {
        var ffmpeg = Path.Combine(_root, "ffmpeg.exe"); var ffprobe = Path.Combine(_root, "ffprobe.exe"); File.WriteAllText(ffmpeg, "fixture"); File.WriteAllText(ffprobe, "fixture");
        var settings = new FfmpegSettings { FfmpegExecutablePath = ffmpeg, FfprobeExecutablePath = ffprobe }; var probe = new VersionProbe(); var resolver = new ToolPathResolver(_root);
        var tools = await SpotlightMediaSession.ResolveToolsAsync(settings, new FfmpegToolChecker(probe, resolver), resolver);
        Assert.Equal(ffmpeg, tools.Ffmpeg); Assert.Equal(ffprobe, tools.Ffprobe); Assert.Equal(2, probe.Calls);
        File.Delete(ffmpeg); await Assert.ThrowsAsync<InvalidOperationException>(() => SpotlightMediaSession.ResolveToolsAsync(settings, new FfmpegToolChecker(probe, resolver), resolver));
    }
    private sealed class VersionProbe : IToolProbeRunner
    {
        internal int Calls;
        public Task<ToolProbeResult> RunAsync(string path, IReadOnlyList<string> args, CancellationToken token = default)
        { Assert.Equal(["-version"], args); Calls++; return Task.FromResult(new ToolProbeResult(0, Path.GetFileNameWithoutExtension(path) + " version 9.0.2", "")); }
    }
}
