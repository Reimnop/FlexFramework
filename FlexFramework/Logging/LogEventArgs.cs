namespace FlexFramework.Logging;

public class LogEventArgs : EventArgs
{
    public Severity Severity { get; }
    public string? Type { get; }
    public string Message { get; }
    

    public LogEventArgs(Severity severity, string? type, string message)
    {
        Severity = severity;
        Type = type;
        Message = message;
    }
}