namespace FlexFramework.Util.Exceptions;

public class ProgramLinkingException : Exception
{
    public ProgramLinkingException(string name, string message) : base($"Failed to link shader program '{name}': {message}")
    {
    }
}