namespace FlexFramework.Util.Logging;

public static class Extensions
{
    public static void LogVerbose(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Verbose, message, exception);
    }
    
    public static void LogDebug(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Debug, message, exception);
    }
    
    public static void LogInfo(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Info, message, exception);
    }
    
    public static void LogWarning(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Warning, message, exception);
    }
    
    public static void LogError(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Error, message, exception);
    }

    public static ILogger CreateLogger(this ILoggerFactory? factory, string name)
    {
        if (factory == null)
        {
#if PRINT_UNROUTED_LOGS
            return new ConsoleLogger();
#else
            return new DummyLogger();
#endif
        }
        
        return factory.GetLogger(name);
    }

    public static ILogger CreateLogger<T>(this ILoggerFactory? factory)
    {
        return factory.CreateLogger(typeof(T).Name);
    }
}