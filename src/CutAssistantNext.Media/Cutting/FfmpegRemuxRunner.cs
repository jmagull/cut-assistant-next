using System.Diagnostics;

namespace CutAssistantNext.Media.Cutting;

public sealed class FfmpegRemuxRunner(string executablePath)
{
    public static IReadOnlyList<string> BuildArguments(string source, string destination) =>
        ["-hide_banner", "-nostdin", "-n", "-i", source,
         "-map", "0", "-c", "copy", "-f", "mp4", destination];

    public async Task RunAsync(string source, string destination,
        IProgress<Mp4BoxProgressUpdate>? progress, CancellationToken cancellationToken)
    {
        if (!File.Exists(executablePath))
            throw new FileNotFoundException("Bitte unter FFmpeg-Werkzeuge einen gültigen Pfad zu ffmpeg.exe einstellen.", executablePath);
        if (string.Equals(Path.GetFullPath(source), Path.GetFullPath(destination), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Die Originaldatei darf nicht überschrieben werden.");
        if (File.Exists(destination))
            throw new InvalidOperationException("Die MP4-Arbeitsdatei existiert bereits und wird nicht überschrieben.");
        cancellationToken.ThrowIfCancellationRequested();
        var info = new ProcessStartInfo(executablePath)
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        foreach (var argument in BuildArguments(source, destination))
            info.ArgumentList.Add(argument);
        using var process = new Process { StartInfo = info };
        progress?.Report(new(Mp4BoxProgressKind.Status, "FFmpeg: Video wird verlustfrei für MP4 vorbereitet …"));
        if (!process.Start())
            throw new InvalidOperationException("FFmpeg konnte nicht gestartet werden.");
        using var registration = cancellationToken.Register(() =>
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
        });
        var output = ReadAsync(process.StandardOutput, progress);
        var error = ReadAsync(process.StandardError, progress);
        await process.WaitForExitAsync(CancellationToken.None);
        await Task.WhenAll(output, error);
        cancellationToken.ThrowIfCancellationRequested();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"FFmpeg konnte diese Datei nicht verlustfrei nach MP4 umpacken (Exit-Code {process.ExitCode}). Details stehen im Protokoll. Es wurde keine Neukodierung durchgeführt.");
        if (!File.Exists(destination) || new FileInfo(destination).Length == 0)
            throw new InvalidOperationException("FFmpeg hat keine nutzbare MP4-Arbeitsdatei erzeugt.");
    }

    private static async Task ReadAsync(StreamReader reader, IProgress<Mp4BoxProgressUpdate>? progress)
    {
        while (await reader.ReadLineAsync() is { } line)
            progress?.Report(new(Mp4BoxProgressKind.Output, "FFmpeg: " + line));
    }
}
