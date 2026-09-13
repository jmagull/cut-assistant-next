using System.Reflection;

namespace CutAssistantNext.App;

public static class ApplicationVersion
{
    private static readonly Version FileVersion = new(
        typeof(ApplicationVersion).Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version);

    public static string DisplayName =>
        $"Cut Assistant Next · {FileVersion.ToString(3)} · Build {FileVersion.Revision}";
}
