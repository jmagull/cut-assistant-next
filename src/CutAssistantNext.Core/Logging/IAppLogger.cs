namespace CutAssistantNext.Core.Logging;

public interface IAppLogger
{
    void Information(string message);

    void Warning(string message);

    void Error(
        string message,
        Exception? exception = null);
}
