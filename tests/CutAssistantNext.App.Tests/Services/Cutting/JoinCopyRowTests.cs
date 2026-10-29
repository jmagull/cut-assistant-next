using CutAssistantNext.App.Dialogs;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class JoinCopyRowTests
{
    [Fact]
    public void EveryMappingOrInclusionChangeInvalidatesConfirmationAndKeepsExplicitTrackIndices()
    {
        AudioStreamInfo[] a = [new(1, "aac", null, 48000, 2, "stereo"), new(2, "ac3", null, 48000, 2, "stereo")];
        AudioStreamInfo[] b = [new(3, "aac", null, 48000, 2, "stereo"), new(4, "ac3", null, 48000, 2, "stereo")];
        var changes = 0;
        var row = new JoinCopyRow(a, b, 1, 3, true, () => changes++);
        Assert.Equal(0, changes); Assert.True(row.Include);
        Assert.Equal(1, row.First!.Stream.Index); Assert.Equal(3, row.Second!.Stream.Index);
        row.Second = row.SecondChoices[1]; row.First = row.FirstChoices[1]; row.Include = false;
        Assert.Equal(3, changes); Assert.Equal(4, row.Second.Stream.Index); Assert.Equal(2, row.First.Stream.Index);
    }

    [Fact]
    public void UnmatchedTrackStartsUnselectedAndDoesNotInventSecondSource()
    {
        var row = new JoinCopyRow([new(2, "ac3", null, 48000, 2, "stereo")],
            [new(1, "aac", null, 48000, 2, "stereo")], 2, null, false, () => { });
        Assert.False(row.Include); Assert.Null(row.Second); Assert.Equal(2, row.First!.Stream.Index);
    }
}
