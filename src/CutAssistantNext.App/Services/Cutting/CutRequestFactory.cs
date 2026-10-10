using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Cutlists.Editing;

namespace CutAssistantNext.App.Services.Cutting;

internal static class CutRequestFactory
{
    public static CutRequest Create(
        string originalFilePath,
        string outputFilePath,
        CutPlan cutPlan,
        double? framesPerSecond = null,
        bool overwriteExistingOutput = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath);
        ArgumentNullException.ThrowIfNull(cutPlan);

        var segments = CutlistKeepSegmentBuilder.Build(cutPlan)
            .Select(segment => new KeepSegment(segment.Start, segment.Duration));

        return new CutRequest(
            Path.GetFullPath(originalFilePath),
            Path.GetFullPath(outputFilePath),
            segments,
            framesPerSecond,
            overwriteExistingOutput);
    }
}
