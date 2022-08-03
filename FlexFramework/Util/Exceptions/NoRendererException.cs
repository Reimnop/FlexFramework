namespace FlexFramework.Util.Exceptions;

public class NoRendererException : Exception
{
    public NoRendererException() : base("No renderer was loaded")
    {
    }
}