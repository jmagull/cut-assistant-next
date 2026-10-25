using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class CutlistIntendedApplicationResolverTests
{
    [Fact]
    public void LoadedOtrCanCutlistIsPreselectedWithoutEarlierCut()
    {
        Assert.Equal(CutEngineKind.OtrCan,
            CutlistIntendedApplicationResolver.PreferredEngine(
                @"C:\Films\Peppermint.mp4", "otr-can", null, null));
    }

    [Fact]
    public void SuccessfulCutOfCurrentMediaTakesPrecedenceOverLoadedCutlist()
    {
        Assert.Equal(CutEngineKind.OtrCan,
            CutlistIntendedApplicationResolver.PreferredEngine(
                @"C:\Films\Peppermint.mp4", "MP4Box",
                @"c:\films\PEPPERMINT.mp4", CutEngineKind.OtrCan));

        Assert.Equal(CutEngineKind.Mp4Box,
            CutlistIntendedApplicationResolver.PreferredEngine(
                @"C:\Films\Peppermint.mp4", "otr-can",
                @"C:\Films\Peppermint.mp4", CutEngineKind.Mp4Box));
    }

    [Fact]
    public void SuccessfulCutOfAnotherMediaDoesNotCarryOver()
    {
        Assert.Equal(CutEngineKind.Mp4Box,
            CutlistIntendedApplicationResolver.PreferredEngine(
                @"C:\Films\Peppermint.mp4", null,
                @"C:\Films\Other.mp4", CutEngineKind.OtrCan));
    }

    [Fact]
    public async Task OtrCanMetadataUsesSelectedExecutableAndReportedVersion()
    {
        var probe = new FakeProbe(new ToolProbeResult(0, "otr_can 0.1.6\n", ""));
        var info = await CutlistIntendedApplicationResolver.ForOtrCanAsync(
            new OtrCanSettings { ExecutablePath = @"C:\Tools\otr_can.exe" }, probe);

        Assert.Equal("otr-can", info.Name);
        Assert.Equal("otr_can.exe", info.Executable);
        Assert.Equal("0.1.6", info.Version);
        Assert.Equal("", info.Options);
        Assert.Equal("--version", Assert.Single(probe.Arguments!));
    }

    [Fact]
    public async Task UnknownVersionDoesNotInsertMp4BoxVersion()
    {
        var info = await CutlistIntendedApplicationResolver.ForOtrCanAsync(
            new OtrCanSettings { ExecutablePath = @"C:\Tools\otr_can.exe" },
            new FakeProbe(new ToolProbeResult(1, "", "error")));

        Assert.Equal("otr-can", info.Name);
        Assert.Equal("otr_can.exe", info.Executable);
        Assert.Equal("", info.Version);
    }

    private sealed class FakeProbe(ToolProbeResult result) : IToolProbeRunner
    {
        public IReadOnlyList<string>? Arguments { get; private set; }

        public Task<ToolProbeResult> RunAsync(string executablePath,
            IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
        {
            Arguments = arguments;
            return Task.FromResult(result);
        }
    }
}
