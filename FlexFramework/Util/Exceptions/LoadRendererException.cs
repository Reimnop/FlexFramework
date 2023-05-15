namespace FlexFramework.Util.Exceptions;

public class LoadRendererException : Exception
{
    public LoadRendererException(Type type) : base($"Could not load renderer of type '{type.Name}'")
    {
    }
}