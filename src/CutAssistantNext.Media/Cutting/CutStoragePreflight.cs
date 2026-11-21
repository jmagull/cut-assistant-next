using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.Media.Cutting;

public sealed record CutStorageEstimate(long WorkingBytes, long TargetBytes, string Basis)
{
    private const long MiB = 1024 * 1024;
    public static CutStorageEstimate Mp4Box(long sourceBytes) => new(
        checked(sourceBytes * 3 + 256 * MiB), checked(sourceBytes * 3 / 2 + 64 * MiB),
        "MP4Box: volle Quelldateigröße als vorsichtige Basis; Segmente, Zusammenfügen, Werkzeugreserve und zusätzliche Zielkopie.");

    public static CutStorageEstimate OtrCan(long sourceBytes, TimeSpan keptDuration, long pcmBytesPerSecond, TimeSpan? sourceDuration = null) => new(
        checked(sourceBytes * 6 + (long)Math.Ceiling(((sourceDuration ?? keptDuration).TotalSeconds + keptDuration.TotalSeconds) * pcmBytesPerSecond) + 512 * MiB),
        checked(sourceBytes * 3 / 2 + 64 * MiB),
        "otr-can: volle Quelldateigröße, Segmente, Zusammenfügen, PCM der vollen Quelle und Behaltebereiche, Index-/Werkzeugreserve und zusätzliche Zielkopie.");
}

public sealed record CutDiskSpace(string Volume, long? AvailableBytes, string? Error = null);
public sealed record CutStorageCheck(CutDiskSpace Working, CutDiskSpace Target, CutStorageEstimate Estimate,
    bool LowSpace, IReadOnlyList<string> Warnings);

/// <summary>Measurements are point-in-time values; estimates are deliberately not guarantees.</summary>
public static class CutStoragePreflight
{
    public static CutStorageCheck Check(string work, string output, CutStorageEstimate estimate,
        IProgress<CutProgressUpdate>? progress, Func<string, CutDiskSpace>? measure = null)
    {
        measure ??= Measure;
        var target = Path.GetDirectoryName(Path.GetFullPath(output))!;
        var working = measure(work);
        var destination = measure(target);
        var warnings = new List<string>();
        bool? sameVolume = string.IsNullOrEmpty(working.Volume) || string.IsNullOrEmpty(destination.Volume)
            ? null : string.Equals(working.Volume, destination.Volume, StringComparison.OrdinalIgnoreCase);
        var combined = checked(estimate.WorkingBytes + estimate.TargetBytes);
        // Unknown volume identity is treated conservatively as shared capacity.
        var workNeeded = sameVolume == false ? estimate.WorkingBytes : combined;
        var targetNeeded = sameVolume == false ? estimate.TargetBytes : combined;
        var low = working.AvailableBytes < workNeeded || destination.AvailableBytes < targetNeeded;
        if (working.AvailableBytes is null || destination.AvailableBytes is null)
            warnings.Add("Freier Speicherplatz nicht vollständig messbar; die Prüfung ist unvollständig.");
        if (low) warnings.Add("Absehbarer Speicherplatzmangel nach Schätzung. Der Schnitt wird vorsorglich nicht gestartet. Bitte Platz freigeben oder einen anderen Arbeits-/Zielordner wählen.");
        progress?.Report(new(CutProgressKind.Output,
            $"Speicherplatzprüfung vor Schnittbeginn ({DateTimeOffset.Now:O})\n" +
            $"Arbeitsordner: {work}\nZielverzeichnis: {target}\n" +
            $"Gemessen frei im Arbeitsordner: {Bytes(working.AvailableBytes)}; Datenträger: {working.Volume}\n" +
            $"Gemessen frei im Zielverzeichnis: {Bytes(destination.AvailableBytes)}; Datenträger: {destination.Volume}\n" +
            $"Geschätzter zusätzlicher Speicherbedarf Arbeit: {Bytes(estimate.WorkingBytes)}\n" +
            $"Geschätzter zusätzlicher Speicherbedarf Ziel: {Bytes(estimate.TargetBytes)}\n" +
            $"Gemeinsamer Datenträger: {(sameVolume is null ? "unbekannt; gemeinsame Reserve angesetzt" : sameVolume.Value ? "ja; Bedarf addiert" : "nein")}\n" +
            $"Schätzgrundlage: {estimate.Basis}\n" +
            "Schätzung ohne Platzgarantie: Bitraten, Neukodierung, Dateisystem und parallele Nutzung können den Bedarf verändern.\n" +
            (working.Error is null ? "" : "Messfehler Arbeitslaufwerk: " + working.Error + "\n") +
            (destination.Error is null ? "" : "Messfehler Ziellaufwerk: " + destination.Error + "\n") +
            (warnings.Count == 0 ? "Ergebnis: kein absehbarer Platzmangel anhand der Messwerte und Schätzung." :
                string.Join("\n", warnings.Select(w => "WARNUNG: " + w)))));
        return new(working, destination, estimate, low, warnings);
    }

    public static void EnsureSufficient(string source, string work, string output, CutStorageEstimate estimate,
        IProgress<CutProgressUpdate>? progress)
    {
        var check = Check(work, output, estimate, progress);
        if (check.LowSpace) throw new IOException(check.Warnings.Last());
    }

    public static string Bytes(long? value) => value is { } bytes ? $"{bytes:N0} Bytes ({bytes / 1073741824d:0.00} GiB)" : "nicht ermittelbar";

    public static void ReportFailure(Exception error, string work, string output, IProgress<CutProgressUpdate>? progress)
    {
        var working = Measure(work);
        var target = Measure(Path.GetDirectoryName(Path.GetFullPath(output))!);
        progress?.Report(new(CutProgressKind.Output,
            $"Festgestellter technischer Fehler (HRESULT 0x{error.HResult:X8}):\n{error}\n" +
            $"Messung nach Fehler, Arbeitsordner {work}: {Bytes(working.AvailableBytes)}\n" +
            $"Messung nach Fehler, Ziel {output}: {Bytes(target.AvailableBytes)}\n" +
            "Diese späteren Messwerte ersetzen nicht die Messung vor Schnittbeginn und beweisen allein keine Fehlerursache.\n" +
            (working.Error ?? "") + "\n" + (target.Error ?? "")));
    }

    public static CutDiskSpace Measure(string directory)
    {
        if (!OperatingSystem.IsWindows())
        {
            try { var root = Path.GetPathRoot(Path.GetFullPath(directory))!; return new(root, new DriveInfo(root).AvailableFreeSpace); }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException)
            { return new("", null, error.ToString()); }
        }
        if (!GetDiskFreeSpaceEx(directory, out var available, out _, out _))
            return new("", null, new Win32Exception(Marshal.GetLastWin32Error()).ToString());
        var rootBuffer = new StringBuilder(1024);
        var volumeBuffer = new StringBuilder(1024);
        var volume = GetVolumePathName(directory, rootBuffer, rootBuffer.Capacity)
            ? GetVolumeNameForVolumeMountPoint(rootBuffer.ToString(), volumeBuffer, volumeBuffer.Capacity)
                ? volumeBuffer.ToString() : rootBuffer.ToString()
            : "";
        return new(volume, available <= long.MaxValue ? (long)available : long.MaxValue);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetDiskFreeSpaceEx(string directory, out ulong available, out ulong total, out ulong free);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetVolumePathName(string fileName, StringBuilder volumePath, int size);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetVolumeNameForVolumeMountPoint(string volumeMountPoint, StringBuilder volumeName, int size);
}
