using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CutAssistantNext.Core;
using CutAssistantNext.Media.Cutting;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Settings;

internal enum ToolFunction { FfmpegStreamcopy, Mp4BoxSplitConcat }

internal sealed record ToolFunctionResult(bool Compatible, string Message);

internal sealed class ToolFunctionChecks
{
    private const string TestRevision = "can-61-v1";
    private const string StreamcopyFixture = "c2fd895ccc3df6bfff3cfe8eb25a0d2d0145d555022d8733b302e308cad6a456";
    private const string Mp4BoxFixture = "79c7042b5ef917bbb657e9ac1bea105c1f9164bab341bdc1904cb4afe85c5997";
    private const string ExpectedStreamcopy = "97253102d076aabaf7b6772503a6f0077160a116eb8f0a7a73053852ea8f9711";
    private readonly IToolProbeRunner _probe;
    private readonly string _cacheDirectory;
    private readonly ToolPathResolver _resolver = new();

    internal ToolFunctionChecks(string? cacheDirectory = null, IToolProbeRunner? probe = null)
    {
        _cacheDirectory = cacheDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationInfo.ProductName, "Settings", "tool-checks");
        _probe = probe ?? new ToolProbeRunner(TimeSpan.FromSeconds(20));
    }

    internal async Task<ToolFunctionResult> CheckFfmpegAsync(FfmpegSettings settings, bool force, CancellationToken token)
    {
        var versions = await new FfmpegToolChecker(_probe, _resolver).CheckAsync(settings, token).ConfigureAwait(false);
        var versionMessage = string.Join(Environment.NewLine, versions.Select(v => $"{v.Name}: {v.Message}"));
        if (!FfmpegToolChecker.IsMatchingPair(versions))
            return new(false, versionMessage + "\nFunktionsprüfung nicht gestartet: FFmpeg und ffprobe aus demselben unterstützten Build auswählen.");

        var result = await CheckAsync(ToolFunction.FfmpegStreamcopy, FfmpegPaths(settings), force, token).ConfigureAwait(false);
        return result with { Message = versionMessage + "\nFunktionsprüfung: " + result.Message };
    }

    internal async Task<ToolFunctionResult> CheckMp4BoxAsync(string? configured, FfmpegSettings settings, bool force, CancellationToken token)
    {
        var paths = Mp4BoxPaths(configured, settings);
        var version = await QueryMp4BoxVersionAsync(paths[0], token).ConfigureAwait(false);
        var result = await CheckAsync(ToolFunction.Mp4BoxSplitConcat, paths, force, token).ConfigureAwait(false);
        return result with { Message = version + "\nFunktionsprüfung: " + result.Message };
    }

    internal IReadOnlyList<string> FfmpegPaths(FfmpegSettings settings) =>
        [_resolver.Resolve(BundledToolKind.Ffmpeg, settings.FfmpegExecutablePath),
         _resolver.Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath)];

    internal IReadOnlyList<string> Mp4BoxPaths(string? configured, FfmpegSettings settings) =>
        [_resolver.Resolve(BundledToolKind.Mp4Box, configured),
         _resolver.Resolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath)];

    internal async Task<ToolFunctionResult?> GetKnownResultAsync(ToolFunction function, IReadOnlyList<string> paths, CancellationToken token)
    {
        try { return Read(await IdentifyAsync(function, paths, token).ConfigureAwait(false)); }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    internal async Task EnsureMp4BoxNoKnownFailureAsync(string path, CancellationToken token)
    {
        var settings = new FfmpegSettingsStore().Load() ?? FfmpegSettings.CreateDefault();
        if (_resolver.TryResolve(BundledToolKind.Ffprobe, settings.FfprobeExecutablePath) is { } ffprobe)
            await EnsureNoKnownFailureAsync(ToolFunction.Mp4BoxSplitConcat, [path, ffprobe], token).ConfigureAwait(false);
    }

    internal async Task EnsureNoKnownFailureAsync(ToolFunction function, IReadOnlyList<string> paths, CancellationToken token)
    {
        if (await GetKnownResultAsync(function, paths, token).ConfigureAwait(false) is { Compatible: false } failed)
            throw new InvalidOperationException($"Selbsttest für den gewählten Schnittweg fehlgeschlagen. {failed.Message}");
    }

    private async Task<ToolFunctionResult> CheckAsync(ToolFunction function, IReadOnlyList<string> paths,
        bool force, CancellationToken token)
    {
        var key = await IdentifyAsync(function, paths, token).ConfigureAwait(false);
        if (!force && Read(key) is { } cached)
            return cached with { Message = cached.Message + " (gespeicherter Test)" };
        var directory = Directory.CreateTempSubdirectory("can-tool-check-");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds(45));
        try
        {
            ToolFunctionResult result;
            try
            {
                result = function == ToolFunction.FfmpegStreamcopy
                    ? await CheckStreamcopyAsync(paths, directory.FullName, timeout.Token).ConfigureAwait(false)
                    : await CheckSplitConcatAsync(paths, directory.FullName, timeout.Token).ConfigureAwait(false);
            }
            catch (InvalidDataException error) { result = new(false, error.Message); }

            if (key != await IdentifyAsync(function, paths, timeout.Token).ConfigureAwait(false))
                throw new IOException("Ein Werkzeug wurde während des Selbsttests ausgetauscht. Bitte erneut prüfen.");
            timeout.Token.ThrowIfCancellationRequested();
            Save(key, result);
            return result;
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            throw new TimeoutException("Das Zeitlimit der Funktionsprüfung wurde erreicht.");
        }
        finally
        {
            foreach (var file in directory.EnumerateFiles()) file.Delete();
            directory.Delete();
        }
    }

    private async Task<ToolFunctionResult> CheckStreamcopyAsync(IReadOnlyList<string> tools, string directory, CancellationToken token)
    {
        var source = await CopyFixtureAsync(directory, "streamcopy-v1.mp4", StreamcopyFixture, token).ConfigureAwait(false);
        var output = Path.Combine(directory, "copy.mp4");
        await RunAsync(tools[0], ["-n", "-nostdin", "-v", "error", "-seek_timestamp", "1", "-ss", "6.000000",
            "-i", source, "-map", "0:v:0", "-c:v", "copy", "-frames:v", "200", output], token).ConfigureAwait(false);
        RequireFile(output);
        var packets = await RunAsync(tools[1], ["-v", "error", "-select_streams", "v:0", "-show_packets",
            "-show_entries", "packet=pts,dts,flags,data_hash", "-show_data_hash", "sha256", "-of", "csv=p=0", output],
            token).ConfigureAwait(false);
        packets = packets.Trim().ReplaceLineEndings("\n");
        var count = packets.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        if (count != 200)
            throw new InvalidDataException($"Streamcopy inkompatibel: {count} von 200 erwarteten sichtbaren Videopaketen.");
        if (Hash(Encoding.UTF8.GetBytes(packets)) != ExpectedStreamcopy)
            throw new InvalidDataException("Streamcopy inkompatibel: Paketdaten, Bildfolge oder Zeitstempel weichen von der Referenz ab.");
        return new(true, "Streamcopy bestanden. 200 sichtbare H.264-Pakete mit korrekter Reihenfolge, Zeitstempeln und Hashwerten.");
    }

    private async Task<ToolFunctionResult> CheckSplitConcatAsync(IReadOnlyList<string> tools, string directory, CancellationToken token)
    {
        var source = await CopyFixtureAsync(directory, "mp4box-v1.mp4", Mp4BoxFixture, token).ConfigureAwait(false);
        var runner = new Mp4BoxRunner(tools[0]);
        var segments = new[] { Path.Combine(directory, "first.mp4"), Path.Combine(directory, "second.mp4") };
        var joined = Path.Combine(directory, "joined.mp4");
        static TimeSpan At(double time) => TimeSpan.FromSeconds(time);
        await runner.RunSplitAsync(source, segments[0], new(At(0.64), At(1.28)), token).ConfigureAwait(false);
        await runner.RunSplitAsync(source, segments[1], new(At(1.92), At(2.56)), token).ConfigureAwait(false);
        await runner.RunConcatAsync(segments, joined, token).ConfigureAwait(false);

        var original = await ReadTracksAsync(tools[1], source, token).ConfigureAwait(false);
        var first = await ReadTracksAsync(tools[1], segments[0], token).ConfigureAwait(false);
        var second = await ReadTracksAsync(tools[1], segments[1], token).ConfigureAwait(false);
        var final = await ReadTracksAsync(tools[1], joined, token).ConfigureAwait(false);
        for (var index = 0; index < final.Length; index++)
        {
            int expectedCount = index == 0 ? 16 : 29;
            int firstOffset = index == 0 ? 16 : 31;
            int secondOffset = index == 0 ? 48 : 91;
            if (!first[index].SequenceEqual(original[index].Skip(firstOffset).Take(expectedCount)) ||
                !second[index].SequenceEqual(original[index].Skip(secondOffset).Take(expectedCount)) ||
                !final[index].SequenceEqual(first[index].Concat(second[index])))
                throw new InvalidDataException($"MP4Box hat Spur {index + 1} verändert, vertauscht oder unvollständig zusammengefügt.");
        }
        return new(true, "MP4Box bestanden. Zwei Schnittbereiche und beide AAC-Tonspuren vollständig in richtiger Reihenfolge.");
    }

    private async Task<string[][]> ReadTracksAsync(string ffprobe, string path, CancellationToken token)
    {
        RequireFile(path);
        var output = await RunAsync(ffprobe, ["-v", "error", "-show_packets", "-show_data_hash", "sha256",
            "-show_entries", "stream=index,codec_name:packet=stream_index,data_hash", "-of", "json", path],
            token).ConfigureAwait(false);
        using var json = JsonDocument.Parse(output);
        var root = json.RootElement;
        var streams = root.GetProperty("streams").EnumerateArray().ToArray();
        if (!streams.Select(s => s.GetProperty("codec_name").GetString()).SequenceEqual(["h264", "aac", "aac"]))
            throw new InvalidDataException("MP4Box hat die erwartete Video-/Tonspurstruktur nicht erhalten.");
        var packets = root.GetProperty("packets").EnumerateArray().ToLookup(p => p.GetProperty("stream_index").GetInt32(),
            p => p.GetProperty("data_hash").GetString() ?? "");
        var tracks = streams.Select(s => packets[s.GetProperty("index").GetInt32()].ToArray()).ToArray();
        if (tracks.Any(track => track.Length == 0 || track.Any(hash => hash.Length != 71)))
            throw new InvalidDataException("Mindestens eine Ausgabespur ist leer oder enthält nicht prüfbare Pakete.");
        return tracks;
    }

    private async Task<string> QueryMp4BoxVersionAsync(string path, CancellationToken token)
    {
        var result = await _probe.RunAsync(path, ["-version"], token).ConfigureAwait(false);
        var match = Regex.Match(result.StandardOutput + "\n" + result.StandardError,
            @"MP4Box\s*-\s*GPAC version\s+(?<token>\S+)", RegexOptions.IgnoreCase);
        if (result.ExitCode != 0 || !match.Success)
            throw new InvalidOperationException("MP4Box konnte nicht gestartet oder anhand seiner Versionsausgabe erkannt werden.");
        var version = match.Groups["token"].Value;
        var type = Regex.IsMatch(version, @"^v?\d+\.\d+(?:\.\d+)?(?:-rev0-\S+)?$", RegexOptions.IgnoreCase)
            ? "Release" : "Entwicklungsbuild / Sonderstand";
        return $"MP4Box: {type} {version}";
    }

    private async Task<string> RunAsync(string tool, string[] args, CancellationToken token)
    {
        var result = await _probe.RunAsync(tool, args, token).ConfigureAwait(false);
        return result.ExitCode == 0 ? result.StandardOutput :
            throw new InvalidOperationException($"{Path.GetFileName(tool)} meldet Exitcode {result.ExitCode}: {result.StandardError.Trim()}");
    }

    private static void RequireFile(string path)
    {
        var file = new FileInfo(path);
        if (!file.Exists || file.Length == 0)
            throw new InvalidDataException("Das Werkzeug hat keine brauchbare MP4-Ausgabe erzeugt.");
    }

    private static async Task<string> CopyFixtureAsync(string directory, string file, string expected, CancellationToken token)
    {
        using var input = typeof(ToolFunctionChecks).Assembly.GetManifestResourceStream(
            $"{typeof(ToolFunctionChecks).Namespace}.ToolFixtures.{file}") ??
            throw new InvalidOperationException("Die mitgelieferte Testdatei fehlt.");
        using var bytes = new MemoryStream();
        await input.CopyToAsync(bytes, token).ConfigureAwait(false);
        if (Hash(bytes.ToArray()) != expected)
            throw new InvalidOperationException("Die mitgelieferte Testdatei hat eine falsche Prüfsumme.");
        var target = Path.Combine(directory, file);
        await File.WriteAllBytesAsync(target, bytes.ToArray(), token).ConfigureAwait(false);
        return target;
    }

    private static string Hash(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));

    private static async Task<string> IdentifyAsync(ToolFunction function, IReadOnlyList<string> paths, CancellationToken token)
    {
        var identity = new StringBuilder(TestRevision).Append(':').Append(function);
        foreach (var path in paths)
        {
            if (!Path.IsPathFullyQualified(path) || !Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Bitte vollständige Pfade zu den Werkzeug-EXE-Dateien wählen.");
            await using var stream = File.OpenRead(path);
            identity.Append(':').Append(Convert.ToHexString(await SHA256.HashDataAsync(stream, token).ConfigureAwait(false)));
        }
        return Hash(Encoding.UTF8.GetBytes(identity.ToString()));
    }

    private ToolFunctionResult? Read(string key)
    {
        try { return JsonSerializer.Deserialize<ToolFunctionResult>(File.ReadAllText(Path.Combine(_cacheDirectory, key + ".json"))); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException) { return null; }
    }

    private void Save(string key, ToolFunctionResult result)
    {
        Directory.CreateDirectory(_cacheDirectory);
        var path = Path.Combine(_cacheDirectory, key + ".json");
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(result));
            File.Move(temporary, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }
}
