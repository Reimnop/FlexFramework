namespace FlexFramework.Util.Logging;

public interface ILogger
{
    void Log(LogLevel level, string message, Exception? exception = null);
}