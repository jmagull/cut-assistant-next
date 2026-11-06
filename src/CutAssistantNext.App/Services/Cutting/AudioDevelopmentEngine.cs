using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>The named private review build uses its own motor without writing shared settings.</summary>
internal static class AudioDevelopmentEngine
{
    internal static OtrCanSettings Resolve(OtrCanSettings settings, string applicationDirectory, string? candidate)
    {
        if (candidate is not ("Audio-Diagnose-Entwicklung" or ApplicationVersion.AudioRecoveryCandidateName)) return settings;
        var path = Path.Combine(Path.GetFullPath(applicationDirectory), "runtime", "otr-can", "otr_can.exe");
        if (!File.Exists(path))
            throw new FileNotFoundException("Der Diagnosemotor dieser Audio-Prüffassung fehlt. Bitte das vollständige Entwicklungspaket verwenden.", path);
        return settings with { ExecutablePath = path };
    }
}
