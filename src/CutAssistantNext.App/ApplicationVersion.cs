using System.Linq;
using System.Reflection;

namespace CutAssistantNext.App;

public static class ApplicationVersion
{
    private static readonly Version FileVersion = new(
        typeof(ApplicationVersion).Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version);

    private static readonly string? Candidate =
        typeof(ApplicationVersion).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attribute => attribute.Key == "CanCandidate")
            ?.Value;

    public static string DisplayName =>
        $"Cut Assistant Next · {FileVersion.ToString(3)} · Build {FileVersion.Revision}" +
        (string.IsNullOrWhiteSpace(Candidate) ? string.Empty : $" · {Candidate}");
}
