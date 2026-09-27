using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace CutAssistantNext.Media.Analysis;

public sealed class FfprobePacketCounter(string ffprobePath)
{
    public static IReadOnlyList<string> BuildArguments(string mediaFilePath) =>
        ["-v", "error",
         "-select_streams", "v",
         "-count_packets",
         "-show_entries", "stream=index,nb_read_packets",
         "-of", "json",
         mediaFilePath];

    public async Task<IReadOnlyDictionary<int, long>> RunAsync(
        string mediaFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);

        if (!File.Exists(ffprobePath))
            throw new FileNotFoundException(
                "ffprobe.exe wurde nicht gefunden.",
                ffprobePath);

        var fullMediaFilePath = Path.GetFullPath(mediaFilePath);

        if (!File.Exists(fullMediaFilePath))
            throw new FileNotFoundException(
                "Die zu prüfende Mediendatei wurde nicht gefunden.",
                fullMediaFilePath);

        var startInfo = new ProcessStartInfo
        {
            FileName = ffprobePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var argument in BuildArguments(fullMediaFilePath))
            startInfo.ArgumentList.Add(argument);

        using var process = new Process
        {
            StartInfo = startInfo
        };

        if (!process.Start())
            throw new InvalidOperationException(
                "ffprobe konnte nicht gestartet werden.");

        using var cancellationRegistration =
            cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(entireProcessTree: true);
                }
                catch (InvalidOperationException)
                {
                    // Der Prozess wurde bereits beendet.
                }
            });

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        var json = await outputTask;
        var errorOutput = await errorTask;

        if (process.ExitCode != 0)
            throw new InvalidOperationException(
                $"ffprobe wurde mit Exit-Code {process.ExitCode} beendet." +
                Environment.NewLine +
                errorOutput.Trim());

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException(
                "ffprobe hat keine JSON-Ausgabe geliefert.");

        return Parse(json);
    }

    public static IReadOnlyDictionary<int, long> Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("streams", out var streams) ||
            streams.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException(
                "ffprobe hat keine Videostream-Daten geliefert.");

        var result = new Dictionary<int, long>();

        foreach (var stream in streams.EnumerateArray())
        {
            if (!stream.TryGetProperty("index", out var indexElement) ||
                !indexElement.TryGetInt32(out var index) ||
                !stream.TryGetProperty("nb_read_packets", out var countElement))
                throw new InvalidOperationException(
                    "ffprobe hat keine vollständige Videopaket-Anzahl geliefert.");

            var text = countElement.ValueKind == JsonValueKind.String
                ? countElement.GetString()
                : countElement.GetRawText();

            if (!long.TryParse(
                    text,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var packetCount) ||
                packetCount <= 0)
                throw new InvalidOperationException(
                    "ffprobe hat keine gültige Videopaket-Anzahl geliefert.");

            if (!result.TryAdd(index, packetCount))
                throw new InvalidOperationException(
                    $"ffprobe hat den Videostream {index} mehrfach geliefert.");
        }

        if (result.Count == 0)
            throw new InvalidOperationException(
                "ffprobe hat keinen Videostream gefunden.");

        return result;
    }
}