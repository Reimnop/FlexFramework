namespace FlexFramework.Util.Logging;

public interface ILoggerFactory
{
    ILogger GetLogger(string name);
}