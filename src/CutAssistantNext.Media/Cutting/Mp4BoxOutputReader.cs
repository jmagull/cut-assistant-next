using CutAssistantNext.Core.Cutting;
namespace CutAssistantNext.Media.Cutting;

public static class Mp4BoxOutputReader
{
    public static async Task<IReadOnlyList<string>> ReadAsync(
        TextReader reader,
        IProgress<CutProgressUpdate>? progress,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            reader);

        var lines =
            new List<string>();

        while (true)
        {
            var line =
                await reader.ReadLineAsync(
                    cancellationToken);

            if (line is null)
            {
                break;
            }

            lines.Add(
                line);

            if (Mp4BoxProgressLineFilter.ShouldReport(
                line))
            {
                progress?.Report(
                    new CutProgressUpdate(
                        CutProgressKind.Output,
                        line));
            }
        }

        return lines;
    }
}
