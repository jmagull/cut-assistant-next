using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using CutAssistantNext.Core.Cutting;
using Microsoft.Win32.SafeHandles;

namespace CutAssistantNext.Media.Tools;

/// <summary>Private Windows job for Klebecenter FFmpeg work; does not alter existing cut engines.</summary>
public sealed class BoundedJoinToolRunner : ICutToolRunner
{
    public async Task RunAsync(string executablePath, IReadOnlyList<string> arguments,
        IProgress<CutProgressUpdate>? progress, CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Das Klebecenter benötigt Windows.");
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(executablePath)) throw new FileNotFoundException("FFmpeg wurde nicht gefunden.", executablePath);
        using var job = CreateJobObjectW(nint.Zero, null);
        if (job.IsInvalid) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        var limits = new ExtendedLimits
        {
            Basic = new BasicLimits { Flags = 0x2000 | 0x100 | 0x8, ActiveProcesses = 1 },
            ProcessMemory = (nuint)(1024L * 1024 * 1024)
        };
        Set(job, 9, limits);
        Set(job, 15, new CpuLimits { Flags = 1 | 4, Rate = 2500 });
        var start = new ProcessStartInfo(executablePath)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardErrorEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8
        };
        foreach (var arg in arguments) start.ArgumentList.Add(arg);
        using var process = Process.Start(start) ?? throw new IOException("FFmpeg konnte nicht gestartet werden.");
        var tail = new StringBuilder();
        var stdout = DrainProgress(process.StandardOutput, progress);
        var stderr = Drain(process.StandardError, progress, tail);
        try
        {
            if (!AssignProcessToJobObject(job, process.Handle))
                throw new IOException("Die Ressourcengrenzen konnten nicht eingerichtet werden.");
            process.PriorityClass = ProcessPriorityClass.BelowNormal;
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromHours(2));
            await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
            await Task.WhenAll(stdout, stderr).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (process.ExitCode != 0) throw new IOException($"FFmpeg abgebrochen (Code {process.ExitCode}).\n{tail}");
        }
        finally
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) when (process.HasExited) { }
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.WhenAll(stdout, stderr).ConfigureAwait(false);
        }
    }

    private static async Task Drain(StreamReader reader, IProgress<CutProgressUpdate>? progress, StringBuilder? tail)
    {
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            tail?.AppendLine(line);
            if (tail?.Length > 16000) tail.Remove(0, tail.Length - 16000);
            progress?.Report(new(CutProgressKind.Output, line));
        }
    }

    private static async Task DrainProgress(StreamReader reader, IProgress<CutProgressUpdate>? progress)
    {
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            if (line.StartsWith("out_time_us=", StringComparison.Ordinal) && long.TryParse(line.AsSpan(12),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out var microseconds) && microseconds >= 0)
            {
                var time = TimeSpan.FromMicroseconds(microseconds);
                progress?.Report(new(CutProgressKind.Progress, $"verarbeitet: {(long)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}"));
            }
            else if (!line.Contains('=')) progress?.Report(new(CutProgressKind.Output, line));
        }
    }

    private static void Set<T>(SafeFileHandle job, int kind, T value) where T : struct
    {
        var size = Marshal.SizeOf<T>(); var ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(value, ptr, false);
            if (!SetInformationJobObject(job, kind, ptr, (uint)size))
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
        finally { Marshal.FreeHGlobal(ptr); }
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct BasicLimits
    {
        public long ProcessTime, JobTime;
        public uint Flags;
        public nuint MinWorkingSet, MaxWorkingSet;
        public uint ActiveProcesses;
        public nuint Affinity;
        public uint Priority, Scheduling;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct IoCounters { public ulong ReadOps, WriteOps, OtherOps, ReadBytes, WriteBytes, OtherBytes; }
    [StructLayout(LayoutKind.Sequential)]
    private struct ExtendedLimits
    {
        public BasicLimits Basic;
        public IoCounters Io;
        public nuint ProcessMemory, JobMemory, PeakProcessMemory, PeakJobMemory;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct CpuLimits { public uint Flags, Rate; }
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateJobObjectW(nint attributes, string? name);
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetInformationJobObject(SafeFileHandle job, int kind, nint info, uint size);
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AssignProcessToJobObject(SafeFileHandle job, nint process);
}
