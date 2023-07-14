using FlexFramework.Util.Logging;

namespace FlexFramework.Core;

public class SceneManager
{
    public Scene CurrentScene { get; private set; } = null!;

    private readonly ILogger logger;
    
    internal SceneManager(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<SceneManager>();
    }

    public Scene LoadScene(Func<Scene> sceneFactory)
    {
        var scene = sceneFactory();
        
        logger.LogInfo($"Loading scene [{scene.GetType().Name}]");
        if (CurrentScene is IDisposable disposable)
        {
            disposable.Dispose();
        }
        CurrentScene = scene;
        return scene;
    }
}