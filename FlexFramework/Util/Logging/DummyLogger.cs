namespace FlexFramework.Util.Logging;

public class DummyLogger : ILogger
{
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        // Do nothing
    }
}