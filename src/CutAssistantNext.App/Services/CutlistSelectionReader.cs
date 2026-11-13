using System.IO;
using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Services;

internal sealed record CutlistImportDocument(CutlistDocument Document, string? LocalFilePath, CutlistRatingSource? ServerSource = null);

internal static class CutlistSelectionReader
{
    internal static async Task<CutlistImportDocument> ReadAsync(CutlistResultRow row,
        Func<string, string, CancellationToken, Task<byte[]>> download,
        CancellationToken cancellationToken = default)
    {
        if (!row.CanLoad) throw new InvalidOperationException("Diese Cutlist kann nicht übernommen werden.");
        if (row.Local is { } local)
        {
            foreach (var path in local.Paths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var bytes = await Task.Run(() => LocalCutlistDiscoveryService.ReadExactAsync(path, cancellationToken), cancellationToken)
                        .WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
                    if (bytes is not null && local.HasContent(bytes))
                    {
                        var localDocument = await Task.Run(local.ReadDocument, cancellationToken);
                        return new CutlistImportDocument(localDocument, path);
                    }
                }
                catch (Exception error) when (error is IOException or UnauthorizedAccessException or TimeoutException) { }
            }
            throw new IOException("Die lokale Cutlist wurde seit der Suche geändert oder ist nicht mehr lesbar. Bitte erneut suchen.");
        }
        if (row.Server is null || row.ServerUrl is null) throw new InvalidOperationException("Die Herkunft der Cutlist fehlt.");
        var serverBytes = await download(row.ServerUrl, row.Server.Id, cancellationToken);
        var document = await Task.Run(() => CutlistFileReader.ReadBytes(serverBytes), cancellationToken);
        return new CutlistImportDocument(document, null, CutlistRatingSource.Create(
            row.ServerUrl, row.Server.Id, row.Server.Author, row.Server.CutlistFileName));
    }
}
