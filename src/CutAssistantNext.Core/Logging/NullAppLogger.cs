namespace CutAssistantNext.Core.Logging;

public sealed class NullAppLogger : IAppLogger
{
    public static NullAppLogger Instance { get; } = new();

    private NullAppLogger()
    {
    }

    public void Information(string message)
    {
    }

    public void Warning(string message)
    {
    }

    public void Error(
        string message,
        Exception? exception = null)
    {
    }
}
