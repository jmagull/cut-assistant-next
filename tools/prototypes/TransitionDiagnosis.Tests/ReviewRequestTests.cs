using System.Security.Cryptography;
using System.Text.Json.Nodes;
using CutAssistantNext.Core.Cutting;
using DiagnosisReview;

namespace TransitionDiagnosis.Tests;

public sealed class ReviewRequestTests
{
    private static JsonObject Fixture() => JsonNode.Parse("""
        {
          "SchemaVersion": 1,
          "Source": { "Path": "C:/test/source.mp4", "Length": 100, "LastWriteUtc": "2026-10-22T12:00:00Z", "Sha256": "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" },
          "Output": { "Path": "C:/test/output.mp4", "Length": 80, "LastWriteUtc": "2026-10-22T12:00:00Z", "Sha256": "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB" },
          "OutputMetadata": { "VideoDuration": 12 },
          "Diagnosis": {
            "AllPacketsUniquelyMapped": true, "KeepOrderMatchesPlan": true, "DiscardOrCorruptPackets": 0,
            "Segments": [{}, {}],
            "Boundaries": [{ "NewKeep": 2, "PacketCandidateSeconds": 2.1,
              "Frames": [
                { "Keep": 1, "PtsSeconds": 2.12, "SourcePtsSeconds": 20.0 },
                { "Keep": 2, "PtsSeconds": 2.1, "SourcePtsSeconds": 35.0 },
                { "Keep": 2, "PtsSeconds": 2.14, "SourcePtsSeconds": 35.04 }
              ]
            }]
          }
        }
        """)!.AsObject();

    [Fact]
    public void PreservesBackwardBoundaryAndBothSourceSidesWithoutCertifyingCandidate()
    {
        var request = ReviewRequest.Parse(Fixture().ToJsonString());
        var candidate = Assert.Single(request.Candidates);
        Assert.Equal([20m, 35m, 35.04m], candidate.References.Select(r => r.SourceSeconds));
        Assert.True(candidate.References[1].IsFirstNewFrame);
        Assert.False(candidate.References[0].IsFirstNewFrame);
        var result = request.CreateCutResult();
        Assert.Equal(CutReviewTimingKind.Estimated, result.TimingKind);
        Assert.NotNull(result.Timeline.Identity);
        Assert.Equal(TimeSpan.FromSeconds(2.1), Assert.Single(result.Timeline.Transitions));
    }

    [Theory]
    [InlineData("incomplete")]
    [InlineData("order")]
    [InlineData("corrupt")]
    [InlineData("candidate")]
    [InlineData("anchor")]
    [InlineData("old-side")]
    [InlineData("hash")]
    [InlineData("version")]
    [InlineData("count")]
    public void RejectsUnusableOrMismatchedEvidence(string error)
    {
        var json = Fixture();
        var diagnosis = json["Diagnosis"]!;
        var boundary = diagnosis["Boundaries"]![0]!;
        switch (error)
        {
            case "incomplete": diagnosis["AllPacketsUniquelyMapped"] = false; break;
            case "order": diagnosis["KeepOrderMatchesPlan"] = false; break;
            case "corrupt": diagnosis["DiscardOrCorruptPackets"] = 1; break;
            case "candidate": boundary["PacketCandidateSeconds"] = 12; break;
            case "anchor": boundary["Frames"]![1]!["PtsSeconds"] = 2.2; break;
            case "old-side": boundary["Frames"]![0]!["Keep"] = null; break;
            case "hash": json["Output"]!["Sha256"] = "bad"; break;
            case "version": json["SchemaVersion"] = 2; break;
            case "count": diagnosis["Segments"]!.AsArray().RemoveAt(0); break;
        }
        Assert.Throws<InvalidDataException>(() => ReviewRequest.Parse(json.ToJsonString()));
    }

    [Fact]
    public async Task IdentityRejectsSameLengthContentChangeEvenWithRestoredTimestamp()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp4");
        try
        {
            await File.WriteAllTextAsync(path, "first");
            var stamp = File.GetLastWriteTimeUtc(path);
            var identity = new ReviewFile(path, new FileInfo(path).Length, stamp,
                Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(path))));
            await identity.ValidateAsync(default);
            await File.WriteAllTextAsync(path, "other");
            File.SetLastWriteTimeUtc(path, stamp);
            await Assert.ThrowsAsync<IOException>(() => identity.ValidateAsync(default));
            using var canceled = new CancellationTokenSource();
            canceled.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => identity.ValidateAsync(canceled.Token));
        }
        finally { File.Delete(path); }
    }
}
