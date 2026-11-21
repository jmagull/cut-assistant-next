using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CutAssistantNext.App.Services;

internal sealed record WorkingDriveInfo(long? CapacityBytes, long? FreeBytes, string? Notice = null);
internal sealed record WorkingFolderSize(long? Bytes, string? Notice = null);

internal interface IWorkingDirectoryInfoReader
{
    Task<WorkingDriveInfo> ReadDriveAsync(string path, CancellationToken token);
    Task<WorkingFolderSize> ReadFolderAsync(string path, CancellationToken token);
}

/// <summary>Read-only settings display. The cut-time preflight is independent and unchanged.</summary>
internal sealed class WorkingDirectoryInfoReader(TimeSpan? sizeBudget = null, int entryLimit = 200_000)
    : IWorkingDirectoryInfoReader
{
    private readonly TimeSpan _sizeBudget = sizeBudget ?? TimeSpan.FromSeconds(10);

    public Task<WorkingDriveInfo> ReadDriveAsync(string path, CancellationToken token) => Task.Run(() =>
    {
        token.ThrowIfCancellationRequested();
        if (!Directory.Exists(path)) return new WorkingDriveInfo(null, null, "Der Ordner ist nicht erreichbar. Bitte Pfad und Zugriffsrechte prüfen.");
        if (!GetDiskFreeSpaceEx(path, out var free, out var capacity, out _))
            return new(null, null, "Datenträgerwerte nicht ermittelbar: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
        token.ThrowIfCancellationRequested();
        return new WorkingDriveInfo(ToLong(capacity), ToLong(free));
    }, token);

    public Task<WorkingFolderSize> ReadFolderAsync(string path, CancellationToken token) => Task.Run(() =>
    {
        var timer = Stopwatch.StartNew();
        var pending = new Stack<DirectoryInfo>();
        pending.Push(new(path));
        long size = 0;
        var entries = 0;
        try
        {
            while (pending.TryPop(out var folder))
            {
                token.ThrowIfCancellationRequested();
                if ((folder.Attributes & FileAttributes.ReparsePoint) != 0)
                    return new WorkingFolderSize(null, "Ordnergröße nicht vollständig ermittelbar: Verknüpfungen werden nicht verfolgt.");
                foreach (var entry in folder.EnumerateFileSystemInfos("*", new EnumerationOptions
                { AttributesToSkip = 0, IgnoreInaccessible = false }))
                {
                    token.ThrowIfCancellationRequested();
                    if (++entries > entryLimit || timer.Elapsed > _sizeBudget)
                        return new(null, "Ordnergröße nicht vollständig ermittelt: Der Verzeichnisbestand ist zu groß für eine kurze Hintergrundabfrage. Bei Bedarf erneut aktualisieren.");
                    var attributes = entry.Attributes;
                    if ((attributes & FileAttributes.ReparsePoint) != 0)
                        return new(null, "Ordnergröße nicht vollständig ermittelbar: Verknüpfungen werden nicht verfolgt.");
                    if (entry is DirectoryInfo child) pending.Push(child);
                    else if (entry is FileInfo file) size = checked(size + file.Length);
                }
            }
            token.ThrowIfCancellationRequested();
            return new WorkingFolderSize(size);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or System.Security.SecurityException or OverflowException)
        {
            return new(null, "Ordnergröße nicht vollständig ermittelbar. Dateien können sich geändert haben oder Zugriffsrechte fehlen: " + error.Message);
        }
    }, token);

    private static long ToLong(ulong bytes) => bytes > long.MaxValue ? long.MaxValue : (long)bytes;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetDiskFreeSpaceEx(string directory, out ulong available, out ulong total, out ulong free);
}
