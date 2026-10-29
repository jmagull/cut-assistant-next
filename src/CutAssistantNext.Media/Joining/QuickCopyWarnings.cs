using System.Globalization;
using System.Text.RegularExpressions;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.Media.Joining;

/// <summary>Warning text alone never authorizes a correction: exact packet proof must agree afterwards.</summary>
internal sealed class QuickCopyWarnings(IReadOnlyList<string> codecs, bool allowDts)
{
    private static readonly Regex Dts = new(@"^\[aost#0:(\d+)/copy @ [0-9a-fA-F]+\] Non-monotonic DTS; previous: (-?\d+), current: (-?\d+); changing to (-?\d+)\. This may result in incorrect timestamps in the output file\.$", RegexOptions.CultureInvariant);
    private static readonly Regex Frame = new(@"^\[mp4 @ [0-9a-fA-F]+\] track (\d+): codec frame size is not set$", RegexOptions.CultureInvariant);
    private readonly List<QuickDtsCorrection> _corrections = [];
    private readonly HashSet<int> _frameWarnings = [];
    private readonly List<string> _unknown = [];
    private readonly object _gate = new();

    internal void Add(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        lock (_gate)
        {
            var dts = Dts.Match(text);
            var frame = Frame.Match(text);
            int Int(string s) => int.Parse(s, CultureInfo.InvariantCulture);
            long Long(string s) => long.Parse(s, CultureInfo.InvariantCulture);
            if (allowDts && dts.Success && _corrections.Count < codecs.Count)
            {
                var index = Int(dts.Groups[1].Value);
                if (index > 0 && index <= codecs.Count && codecs[index - 1] == "aac")
                {
                    _corrections.Add(new(index, Long(dts.Groups[2].Value), Long(dts.Groups[3].Value), Long(dts.Groups[4].Value)));
                    return;
                }
            }
            if (frame.Success)
            {
                var index = Int(frame.Groups[1].Value);
                if (index > 0 && index <= codecs.Count && codecs[index - 1] == "ac3" && _frameWarnings.Add(index)) return;
            }
            if (_unknown.Count < 8) _unknown.Add(text);
        }
    }

    internal void RequireKnown()
    {
        lock (_gate)
            if (_unknown.Count != 0) throw new InvalidDataException("Nicht freigegebene FFmpeg-Warnung: " + string.Join("\n", _unknown));
    }

    internal void Verify(IEnumerable<QuickDtsCorrection> proven)
    {
        lock (_gate)
        {
            RequireKnown();
            var expected = proven.OrderBy(p => p.StreamIndex).ToArray();
            var reported = _corrections.OrderBy(p => p.StreamIndex).ToArray();
            // Equal DTS may be advanced by one tick below FFmpeg's warning level.
            // Only this independently proven equality case may lack a warning line.
            if (reported.Distinct().Count() != reported.Length || reported.Any(p => !expected.Contains(p)) ||
                expected.Except(reported).Any(p => p.Original != p.Previous || p.Corrected != checked(p.Previous + 1)))
                throw new InvalidDataException("Gemeldete DTS-Korrekturen stimmen nicht mit dem vollständigen Paketnachweis überein.");
        }
    }
}
