using FlexFramework.Core.Rendering;
using FlexFramework.Util.Logging;

namespace FlexFramework.Core;

public class SceneManager : IUpdateable
{
    public Scene? CurrentScene { get; private set; }
    private Func<Scene>? currentSceneFactory;

    private readonly ILogger logger;
    
    internal SceneManager(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<SceneManager>();
    }
    
    public void Update(UpdateArgs args)
    {
        if (currentSceneFactory != null)
        {
            var scene = currentSceneFactory();
            logger.LogInfo($"Loading scene [{scene.GetType().Name}]");
            if (CurrentScene is IDisposable disposable)
                disposable.Dispose();
            CurrentScene = scene;
            currentSceneFactory = null;
        }

        CurrentScene?.Update(args);
    }
    
    public void Render(Renderer renderer)
    {
        CurrentScene?.Render(renderer);
    }

    public void LoadScene(Func<Scene> sceneFactory)
    {
        // Defer loading the scene until the next update
        currentSceneFactory = sceneFactory;
    }
}