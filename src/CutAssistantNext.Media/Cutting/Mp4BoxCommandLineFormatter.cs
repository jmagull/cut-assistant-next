namespace CutAssistantNext.Media.Cutting;

public static class Mp4BoxCommandLineFormatter
{
    public static string Format(
        string executablePath,
        IEnumerable<string> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            executablePath);

        ArgumentNullException.ThrowIfNull(
            arguments);

        var parts =
            new List<string>
            {
                QuoteIfNeeded(
                    executablePath)
            };

        foreach (var argument in arguments)
        {
            parts.Add(
                QuoteIfNeeded(
                    argument));
        }

        return string.Join(
            " ",
            parts);
    }

    private static string QuoteIfNeeded(
        string value)
    {
        ArgumentNullException.ThrowIfNull(
            value);

        if (value.Length == 0)
        {
            return "\"\"";
        }

        return value.Any(
            char.IsWhiteSpace)
            ? $"\"{value}\""
            : value;
    }
}
