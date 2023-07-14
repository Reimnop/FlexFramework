namespace FlexFramework.Util.Logging;

public class ConsoleLogger : ILogger
{
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        Console.WriteLine($"{level}: {message}");
        
        if (exception != null)
        {
            Console.WriteLine(exception);
        }
    }
}