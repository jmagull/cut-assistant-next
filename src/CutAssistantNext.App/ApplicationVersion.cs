using System.Linq;
using System.Reflection;

namespace CutAssistantNext.App;

public static class ApplicationVersion
{
    internal const string AudioRecoveryCandidateName = "Audio-Fehlerbehandlung-Entwicklung";

    private static readonly Version FileVersion = new(
        typeof(ApplicationVersion).Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version);

    private static readonly string? Candidate =
        typeof(ApplicationVersion).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attribute => attribute.Key == "CanCandidate")
            ?.Value;

    public static string NumericVersion => FileVersion.ToString(3);
    internal static string? CandidateName => Candidate;

    public static string DisplayName =>
        $"Cut Assistant Next · {NumericVersion} · Build {FileVersion.Revision}" +
        (string.IsNullOrWhiteSpace(Candidate) || Candidate == AudioRecoveryCandidateName
            ? string.Empty
            : $" · {Candidate}");
}
