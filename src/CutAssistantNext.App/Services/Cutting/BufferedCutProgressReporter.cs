using System.Text;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Services.Cutting;

/// <summary>Collects process output without posting one UI operation per line.</summary>
internal sealed class BufferedCutProgressReporter(Action<CutProgressUpdate> target) : IProgress<CutProgressUpdate>
{
    private readonly object _gate = new();
    private List<CutProgressUpdate> _pending = [];

    public void Report(CutProgressUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        lock (_gate) _pending.Add(update);
    }

    // Called only by the UI timer or immediately before marking the operation complete.
    public void Flush()
    {
        List<CutProgressUpdate> updates;
        lock (_gate)
        {
            updates = _pending;
            _pending = [];
        }

        StringBuilder? output = null;
        foreach (var update in updates)
        {
            if (update.Kind == CutProgressKind.Output)
            {
                if (output is null) output = new StringBuilder();
                else output.AppendLine();
                output.Append(update.Message);
            }
            else
            {
                FlushOutput();
                target(update);
            }
        }
        FlushOutput();

        void FlushOutput()
        {
            if (output is null) return;
            target(new(CutProgressKind.Output, output.ToString()));
            output = null;
        }
    }
}
