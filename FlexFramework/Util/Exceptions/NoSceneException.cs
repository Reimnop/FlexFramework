namespace FlexFramework.Util.Exceptions;

public class NoSceneException : Exception
{
    public NoSceneException() : base("No scene was loaded")
    {
    }
}