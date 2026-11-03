using System.IO;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Cutlists.Compatibility;
using CutAssistantNext.Cutlists.Editing;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Services;

internal sealed class LocalCutlistCandidate
{
    private readonly byte[] _bytes;
    internal LocalCutlistCandidate(string path, byte[] bytes)
    {
        Paths = new List<string> { path };
        _bytes = (byte[])bytes.Clone();
    }

    internal List<string> Paths { get; }
    internal bool HasContent(byte[] bytes) => _bytes.AsSpan().SequenceEqual(bytes);
    internal bool HasSameContent(LocalCutlistCandidate other) => _bytes.AsSpan().SequenceEqual(other._bytes);
    internal CutlistDocument ReadDocument() => CutlistFileReader.ReadBytes(_bytes, maximumCuts: 10000);
}

internal sealed record CutlistSourceResult(IReadOnlyList<CutlistResultRow> Rows, string Status, bool HasIssue = false);

internal sealed class LocalCutlistDiscoveryService
{
    internal const int MaximumFileBytes = 4 * 1024 * 1024;
    private readonly Func<string, CancellationToken, Task<byte[]?>> _read;
    private readonly TimeSpan _timeout;

    internal LocalCutlistDiscoveryService(Func<string, CancellationToken, Task<byte[]?>>? read = null,
        TimeSpan? timeout = null)
    {
        _read = read ?? ReadExactAsync;
        _timeout = timeout ?? TimeSpan.FromSeconds(5);
    }

    internal static IReadOnlyList<string> CandidatePaths(string originalPath, string? configuredDirectory)
    {
        var original = Path.GetFullPath(originalPath);
        var name = CutlistFileName.FromOriginal(original);
        var directories = new List<string>();
        foreach (var directory in new[] { configuredDirectory, Path.GetDirectoryName(original) })
        {
            if (string.IsNullOrWhiteSpace(directory) || !Path.IsPathFullyQualified(directory.Trim())) continue;
            try
            {
                var normalized = Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory.Trim()));
                if (!directories.Contains(normalized, StringComparer.OrdinalIgnoreCase)) directories.Add(normalized);
            }
            catch (Exception error) when (error is ArgumentException or NotSupportedException or PathTooLongException) { }
        }
        return directories.Select(directory => Path.Combine(directory, name)).ToArray();
    }

    internal async Task<CutlistSourceResult> SearchAsync(CutlistMediaContext context, string? configuredDirectory,
        CancellationToken cancellationToken = default)
    {
        // Only these exact paths are ever opened; neither directories nor other filenames are enumerated.
        var reads = CandidatePaths(context.FilePath, configuredDirectory).Select(async path =>
        {
            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(_timeout);
                var bytes = await Task.Run(() => _read(path, timeout.Token), timeout.Token).WaitAsync(timeout.Token);
                if (bytes is null) return (Candidate: (LocalCutlistCandidate?)null, Error: (string?)null);
                if (bytes.Length > MaximumFileBytes) throw new InvalidDataException("Die Cutlist ist größer als 4 MiB.");
                return (Candidate: new LocalCutlistCandidate(path, bytes), Error: (string?)null);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return (Candidate: (LocalCutlistCandidate?)null, Error: $"Zeitlimit beim Lesen: {path}");
            }
            catch (Exception error) when (error is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                return (Candidate: (LocalCutlistCandidate?)null, Error: $"Nicht lesbar: {path} – {error.Message}");
            }
        });
        var results = await Task.WhenAll(reads);
        cancellationToken.ThrowIfCancellationRequested();
        var rows = new List<CutlistResultRow>();
        foreach (var result in results)
        {
            if (result.Candidate is not { } candidate) continue;
            var duplicate = rows.FirstOrDefault(row => row.Local?.HasSameContent(candidate) == true);
            if (duplicate?.Local is not null)
            {
                duplicate.Local.Paths.AddRange(candidate.Paths);
                continue;
            }
            rows.Add(CutlistResultRow.FromLocal(candidate, context));
        }
        var errors = results.Where(result => result.Error is not null).Select(result => result.Error);
        return new CutlistSourceResult(rows, $"{rows.Count} Treffer" +
            (errors.Any() ? " · " + string.Join("; ", errors) : string.Empty), errors.Any());
    }

    internal static async Task<byte[]?> ReadExactAsync(string path, CancellationToken cancellationToken)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            if (stream.Length > MaximumFileBytes) throw new InvalidDataException("Die Cutlist ist größer als 4 MiB.");
            using var content = new MemoryStream();
            var buffer = new byte[8192];
            int count;
            while ((count = await stream.ReadAsync(buffer, cancellationToken)) != 0)
            {
                if (content.Length + count > MaximumFileBytes) throw new InvalidDataException("Die Cutlist ist größer als 4 MiB.");
                content.Write(buffer, 0, count);
            }
            return content.ToArray();
        }
        catch (FileNotFoundException) { return null; }
    }
}
