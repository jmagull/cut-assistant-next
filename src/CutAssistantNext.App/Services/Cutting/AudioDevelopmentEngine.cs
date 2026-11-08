using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>Uses the matching bundled motor without rewriting the user's saved tool paths.</summary>
internal static class AudioDevelopmentEngine
{
    internal static OtrCanSettings Resolve(OtrCanSettings settings, string applicationDirectory)
    {
        var path = Path.Combine(Path.GetFullPath(applicationDirectory), "tools", "otr-can", "otr_can.exe");
        if (!File.Exists(path))
            throw new FileNotFoundException("Der mitgelieferte otr-can-Motor fehlt. Bitte den vollständigen Cut-Assistant-Next-Programmordner verwenden.", path);
        return settings with { ExecutablePath = path };
    }
}
