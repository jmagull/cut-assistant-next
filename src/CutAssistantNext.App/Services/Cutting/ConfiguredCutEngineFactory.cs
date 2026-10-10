using CutAssistantNext.Core.Cutting;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>The cut command selects an engine independently of saved tool paths.</summary>
internal sealed class ConfiguredCutEngineFactory
{
    private readonly ConfiguredMp4BoxCutServiceFactory _mp4BoxFactory;
    private readonly Func<OtrCanSettings, ICutEngine> _otrCanFactory;

    public ConfiguredCutEngineFactory(ConfiguredMp4BoxCutServiceFactory mp4BoxFactory,
        Func<OtrCanSettings, ICutEngine>? otrCanFactory = null)
    {
        ArgumentNullException.ThrowIfNull(mp4BoxFactory);
        _mp4BoxFactory = mp4BoxFactory;
        _otrCanFactory = otrCanFactory ?? CreateOtrCan;
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
        kind == CutEngineKind.Mp4Box && !VideoPreparation.IsMp4(analysis);

    private static ICutEngine CreateOtrCan(OtrCanSettings native)
    {
        var ffmpeg = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
        var resolver = new ToolPathResolver();
        var paths = new OtrCanToolPaths(native.ExecutablePath, native.FfmsIndexExecutablePath,
            resolver.Resolve(BundledToolKind.Ffmpeg, ffmpeg.FfmpegExecutablePath),
            resolver.Resolve(BundledToolKind.Ffprobe, ffmpeg.FfprobeExecutablePath));
        return new OtrCanCutService(paths, checkTools: async token =>
        {
            var results = await new OtrCanToolChecker().CheckAsync(native, ffmpeg, token).ConfigureAwait(false);
            if (results.Any(r => !r.IsAvailable))
                throw new InvalidOperationException(string.Join(Environment.NewLine,
                    results.Where(r => !r.IsAvailable).Select(r => $"{r.Name}: {r.Message}")));
        });
    }
}
