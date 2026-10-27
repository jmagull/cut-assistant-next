using CutAssistantNext.Core.Cutting;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>The cut command selects an engine independently of saved tool paths.</summary>
internal sealed class ConfiguredCutEngineFactory
{
    private readonly ConfiguredMp4BoxCutServiceFactory _mp4BoxFactory;
    private readonly Func<OtrCanSettings, ICutEngine> _otrCanFactory;

    public ConfiguredCutEngineFactory(ConfiguredMp4BoxCutServiceFactory mp4BoxFactory,
        Func<OtrCanSettings, ICutEngine>? otrCanFactory = null,
        Func<AacEmergencyOffer, CancellationToken, Task<AacEmergencyChoice?>>? confirmAudioFallback = null)
    {
        ArgumentNullException.ThrowIfNull(mp4BoxFactory);
        _mp4BoxFactory = mp4BoxFactory;
        _otrCanFactory = otrCanFactory ?? (native => CreateOtrCan(native, confirmAudioFallback));
    }

    public ICutEngine Create(IProgress<CutProgressUpdate>? progress = null) =>
        _mp4BoxFactory.Create(progress);

    public ICutEngine Create(CutEngineKind kind, OtrCanSettings settings, IProgress<CutProgressUpdate>? progress = null) =>
        kind switch
        {
            CutEngineKind.Mp4Box => _mp4BoxFactory.Create(progress),
            CutEngineKind.OtrCan => _otrCanFactory(settings),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

    internal static bool RequiresPreparation(CutEngineKind kind, Core.Media.MediaAnalysisResult analysis) =>
        kind == CutEngineKind.Mp4Box && !VideoPreparation.IsMp4(analysis) ||
        kind == CutEngineKind.OtrCan &&
        analysis.FormatName?.Split(',').Any(x => x.Trim().Equals("avi", StringComparison.OrdinalIgnoreCase)) == true &&
        analysis.VideoStreams.Count == 1 && analysis.VideoStreams[0].Index == 0 &&
        analysis.VideoStreams[0].CodecName == "h264";

    private static ICutEngine CreateOtrCan(OtrCanSettings native,
        Func<AacEmergencyOffer, CancellationToken, Task<AacEmergencyChoice?>>? confirm)
    {
        var ffmpeg = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
        var resolver = new ToolPathResolver();
        var paths = new OtrCanToolPaths(native.ExecutablePath, native.FfmsIndexExecutablePath,
            resolver.Resolve(BundledToolKind.Ffmpeg, ffmpeg.FfmpegExecutablePath),
            resolver.Resolve(BundledToolKind.Ffprobe, ffmpeg.FfprobeExecutablePath));
        ICutEngine Create(int? selected) => new OtrCanCutService(paths, checkTools: async (requireMp3, token) =>
        {
            var results = await new OtrCanToolChecker().CheckAsync(native, ffmpeg, token, requireMp3).ConfigureAwait(false);
            if (results.Any(r => !r.IsAvailable))
                throw new InvalidOperationException(string.Join(Environment.NewLine,
                    results.Where(r => !r.IsAvailable).Select(r => $"{r.Name}: {r.Message}")));
        }, retainedAudioStreamIndex: selected);
        var normal = Create(null);
        return confirm is null ? normal : new OtrCanAacEmergencyCutService(
            normal, selected => Create(selected), new AacEmergencySource(paths), confirm);
    }
}
