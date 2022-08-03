namespace FlexFramework.Util.Exceptions;

public class LoadSceneException : Exception
{
    public LoadSceneException(Type sceneType) : base($"Could not load scene of type '{sceneType.Name}'")
    {
    }
}