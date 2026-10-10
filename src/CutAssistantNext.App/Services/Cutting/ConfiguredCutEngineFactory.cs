using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>MP4Box remains the only enabled engine during integration preparation.</summary>
internal sealed class ConfiguredCutEngineFactory
{
    private readonly ConfiguredMp4BoxCutServiceFactory _mp4BoxFactory;

    public ConfiguredCutEngineFactory(ConfiguredMp4BoxCutServiceFactory mp4BoxFactory)
    {
        ArgumentNullException.ThrowIfNull(mp4BoxFactory);
        _mp4BoxFactory = mp4BoxFactory;
    }

    public ICutEngine Create(IProgress<CutProgressUpdate>? progress = null) =>
        _mp4BoxFactory.Create(progress);
}
