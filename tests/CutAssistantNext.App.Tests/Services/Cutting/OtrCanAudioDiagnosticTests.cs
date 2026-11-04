using System.Text.Json.Nodes;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class OtrCanAudioDiagnosticTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "can-diagnostic-" + Guid.NewGuid().ToString("N"));
    private readonly CutRequest _request;
    private static MediaAnalysisResult Source => new("mp4", "MP4", 1, TimeSpan.FromSeconds(1), [], [new(1, "aac", null, 48000, 2, "stereo")]);
    public OtrCanAudioDiagnosticTests()
    {
        Directory.CreateDirectory(_root);
        _request = new(Path.Combine(_root, "in.mp4"), Path.Combine(_root, "out.mp4"), [new(TimeSpan.Zero, TimeSpan.FromSeconds(1))]);
    }
    public void Dispose() => Directory.Delete(_root, true);
    private OtrCanAudioDiagnostic? Read(string json)
    { File.WriteAllText(Path.Combine(_root, "audio-diagnostic.json"), json); return OtrCanAudioDiagnostic.Read(_root, Source, _request); }
    [Fact]
    public void ShowsCodecKeepOriginalTimeAndExactSampleCount()
    {
        var diagnostic = Read(AudioMp4BoxFallbackTests.Json()); Assert.NotNull(diagnostic);
        Assert.Contains("Tonspur 1 (AAC, 48000 Hz)", diagnostic.Description);
        Assert.Contains("00:00:00.042", diagnostic.Description); Assert.Contains("Behaltebereich 1", diagnostic.Description);
        Assert.Contains("512 fehlende Samples", diagnostic.Description); Assert.Contains("ungeklärt", diagnostic.Description);
        Assert.Contains("Erwartet: 48000 Samples, vorhanden: 47488", diagnostic.Description);
    }
    [Theory]
    [InlineData("schema")]
    [InlineData("stream")]
    [InlineData("codec")]
    [InlineData("rate")]
    [InlineData("total")]
    [InlineData("overlap")]
    [InlineData("keep")]
    [InlineData("kind")]
    [InlineData("negative")]
    [InlineData("overflow")]
    [InlineData("null")]
    public void RejectsUnsupportedMalformedOrInconsistentEvidence(string change)
    {
        var node = JsonNode.Parse(AudioMp4BoxFallbackTests.Json())!;
        switch (change)
        {
            case "schema": node["schema_version"] = 2; break;
            case "stream": node["stream_index"] = 9; break;
            case "codec": node["codec"] = "ac3"; break;
            case "rate": node["sample_rate"] = 44100; break;
            case "total": node["actual_samples"] = 47487; break;
            case "overlap": node["issues"]!.AsArray().Add(node["issues"]![0]!.DeepClone()); break;
            case "keep": node["issues"]![0]!["keep"] = 2; break;
            case "kind": node["issues"]![0]!["kind"] = "safe_repair"; break;
            case "negative": node["actual_samples"] = -1; break;
            case "overflow": node["issues"]![0]!["end_sample"] = long.MaxValue; break;
            case "null": node["issues"] = null; break;
        }
        Assert.Null(Read(node.ToJsonString()));
    }
    [Fact]
    public void UnknownDeviationNeverClaimsProvenGap()
    {
        var node = JsonNode.Parse(AudioMp4BoxFallbackTests.Json())!;
        node["classification"] = "unclassified"; node["issues"] = new JsonArray();
        var result = Read(node.ToJsonString()); Assert.NotNull(result);
        Assert.Contains("nicht eindeutig als Zeitlücke", result.Description); Assert.DoesNotContain("Innere Zeitlücke", result.Description);
    }
    [Fact]
    public void EndShortfallIsShownSeparately()
    {
        var node = JsonNode.Parse(AudioMp4BoxFallbackTests.Json())!; node["issues"]![0]!["kind"] = "audio_end";
        Assert.Contains("Fehlender Ton am Dateiende", Read(node.ToJsonString())!.Description);
    }
    [Fact]
    public void MissingTruncatedAndExcessiveReportsAreSafelyIgnored()
    {
        Assert.Null(OtrCanAudioDiagnostic.Read(_root, Source, _request));
        Assert.Null(Read("{")); Assert.Null(Read(new string(' ', 65_537)));
    }
    [Theory]
    [InlineData("[audio-processing-failure-v1] unsupported audio", true)]
    [InlineData("[audio-diagnostic-v1] sample difference", true)]
    [InlineData("PCM count differs for audio stream 1: expected 99, got 98", true)]
    [InlineData("PCM assembly failed: error", true)]
    [InlineData("PCM assembly emitted warnings; cut rejected", true)]
    [InlineData("Video packet count differs", false)]
    [InlineData("Output already exists", false)]
    public void OnlyRecognizedAudioFailuresOfferTheAlternative(string error, bool expected) =>
        Assert.Equal(expected, OtrCanAudioFailureException.IsAudioFailure(error));
    [Fact]
    public void PacketGapDoesNotEnableNewAutomaticTrackExclusion()
    {
        var source = Source with { AudioStreams = [Source.AudioStreams[0], new(2, "ac3", null, 48000, 2, "stereo")] };
        Assert.NotNull(new OtrCanAudioFailureException(source, source, new InvalidOperationException("PCM assembly failed: decoder error")).Offer);
        Assert.Null(new OtrCanAudioFailureException(source, source, new InvalidOperationException("[audio-diagnostic-v1] gap")).Offer);
        Assert.Null(new OtrCanAudioFailureException(source, source, new InvalidOperationException("PCM assembly failed: decoder error"), allowAacEmergency: false).Offer);
    }
}
