using System.Globalization;

namespace CutAssistantNext.Media.Analysis;

/// <summary>Checks FFMS2 v2 timecodes in the microsecond precision used by otr-can.</summary>
public static class FfmsTimecodeValidator
{
    /// <summary>The caller retains ownership of the reader. Times are never changed.</summary>
    public static Task<int> ValidateAsync(TextReader reader, CancellationToken cancellationToken = default) =>
        ValidateCoreAsync(reader, requireOtrCanCfr: false, cancellationToken);

    /// <summary>Also matches otr-can 0.1.5's zero-start 25/50 fps CFR requirements.</summary>
    public static Task<int> ValidateForOtrCanAsync(TextReader reader, CancellationToken cancellationToken = default) =>
        ValidateCoreAsync(reader, requireOtrCanCfr: true, cancellationToken);

    private static async Task<int> ValidateCoreAsync(TextReader reader, bool requireOtrCanCfr, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(reader);
        cancellationToken.ThrowIfCancellationRequested();
        var header = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        if (header != "# timecode format v2")
            throw new InvalidDataException("Der FFMS2-Zeitindex hat keinen gültigen v2-Kopf.");

        var count = 0;
        ulong previous = 0;
        double step = 0;
        string? unsupportedReason = null;
        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (line != line.Trim() ||
                !double.TryParse(line, NumberStyles.Float, CultureInfo.InvariantCulture, out var milliseconds) ||
                !double.IsFinite(milliseconds) || milliseconds < 0)
                throw new InvalidDataException($"Ungültige FFMS2-Bildzeit in Zeile {count + 2}.");

            // Match the motor's milliseconds -> seconds -> truncated microseconds conversion.
            var microseconds = milliseconds / 1000d * 1_000_000d;
            if (!double.IsFinite(microseconds) || microseconds >= (double)ulong.MaxValue)
                throw new InvalidDataException($"FFMS2-Bildzeit außerhalb des Wertebereichs in Zeile {count + 2}.");
            var current = (ulong)microseconds;
            if (count > 0 && current <= previous)
                throw new InvalidDataException($"Doppelte oder rückwärts laufende FFMS2-Bildzeit bei Bild {count + 1} (Zeile {count + 2}).");
            if (requireOtrCanCfr)
            {
                if (count == 0 && current != 0)
                    unsupportedReason = "OTR-CAN benötigt Bildzeiten ab null.";
                if (count == 1)
                {
                    step = current / 1_000_000d;
                    if (!(Math.Abs(step - 0.02) < 0.000001 || Math.Abs(step - 0.04) < 0.000001))
                        unsupportedReason ??= "OTR-CAN benötigt eine konstante Bildrate von 25 oder 50 fps.";
                }
                if (count > 0 && Math.Abs(current / 1_000_000d - count * step) > 0.000002)
                    unsupportedReason ??= $"Das OTR-CAN-Bildraster weicht bei Bild {count + 1} (Zeile {count + 2}) ab.";
            }
            previous = current;
            count++;
        }
        cancellationToken.ThrowIfCancellationRequested();
        if (count == 0)
            throw new InvalidDataException("Der FFMS2-Zeitindex enthält keine Bildzeiten.");
        // Validate all monotonicity first, so ambiguous times keep their more specific diagnosis.
        if (requireOtrCanCfr)
        {
            if (count < 2)
                throw new NotSupportedException("OTR-CAN benötigt mindestens zwei Bildzeiten für die CFR-Prüfung.");
            if (unsupportedReason is not null)
                throw new NotSupportedException(unsupportedReason);
        }
        return count;
    }
}
