using CutAssistantNext.Core.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>The cut command selects an engine independently of saved tool paths.</summary>
internal sealed class ConfiguredCutEngineFactory
{
    private readonly ConfiguredMp4BoxCutServiceFactory _mp4BoxFactory;
    private readonly Func<OtrCanSettings, ICutEngine> _otrCanFactory;
    private readonly Func<OtrCanAudioFailureException, CancellationToken, Task<bool>>? _confirmMp4BoxFallback;

    public ConfiguredCutEngineFactory(ConfiguredMp4BoxCutServiceFactory mp4BoxFactory,
        Func<OtrCanSettings, ICutEngine>? otrCanFactory = null,
        Func<AacEmergencyOffer, CancellationToken, Task<AacEmergencyChoice?>>? confirmAudioFallback = null,
        Func<OtrCanAudioFailureException, CancellationToken, Task<bool>>? confirmMp4BoxFallback = null)
    {
        ArgumentNullException.ThrowIfNull(mp4BoxFactory);
        _mp4BoxFactory = mp4BoxFactory;
        _otrCanFactory = otrCanFactory ?? (native => CreateOtrCan(native, confirmAudioFallback));
        _confirmMp4BoxFallback = confirmMp4BoxFallback;
    }

    public ICutEngine Create(IProgress<CutProgressUpdate>? progress = null) =>
        _mp4BoxFactory.Create(progress);

    public ICutEngine Create(CutEngineKind kind, OtrCanSettings settings, IProgress<CutProgressUpdate>? progress = null) =>
        kind switch
        {
            CutEngineKind.Mp4Box => _mp4BoxFactory.Create(progress),
            CutEngineKind.OtrCan => CreateNativeWithFallback(settings, progress),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

    private ICutEngine CreateNativeWithFallback(OtrCanSettings settings, IProgress<CutProgressUpdate>? progress)
    {
        var normal = _otrCanFactory(settings);
        if (_confirmMp4BoxFallback is null) return normal;
        return new OtrCanMp4BoxFallbackCutService(normal, async (reporter, token) =>
        {
            var alternative = await _mp4BoxFactory.CreateCheckedAsync(reporter, token).ConfigureAwait(false);
            var ffmpeg = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
            var resolver = new ToolPathResolver();
            var ffmpegPath = resolver.Resolve(BundledToolKind.Ffmpeg, ffmpeg.FfmpegExecutablePath);
            var ffprobePath = resolver.Resolve(BundledToolKind.Ffprobe, ffmpeg.FfprobeExecutablePath);
            return new Mp4BoxFallbackPreparation(alternative, new FfprobeRunner(ffprobePath).RunAsync,
                (input, output, analysis, updates, cancellation) => VideoPreparation.PrepareAsync(
                    input, output, analysis, ffmpegPath, ffprobePath, updates ?? new Progress<CutProgressUpdate>(), cancellation));
        }, _confirmMp4BoxFallback);
    }

    internal static bool RequiresPreparation(CutEngineKind kind, Core.Media.MediaAnalysisResult analysis) =>
        kind == CutEngineKind.Mp4Box && !VideoPreparation.IsMp4(analysis) ||
        kind == CutEngineKind.OtrCan &&
        analysis.FormatName?.Split(',').Any(x => x.Trim().Equals("avi", StringComparison.OrdinalIgnoreCase)) == true &&
        analysis.VideoStreams.Count == 1 && analysis.VideoStreams[0].Index == 0 &&
        analysis.VideoStreams[0].CodecName == "h264";

    private static ICutEngine CreateOtrCan(OtrCanSettings native,
        Func<AacEmergencyOffer, CancellationToken, Task<AacEmergencyChoice?>>? confirm)
    {
        native = AudioDevelopmentEngine.Resolve(native, AppContext.BaseDirectory, ApplicationVersion.CandidateName);
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
