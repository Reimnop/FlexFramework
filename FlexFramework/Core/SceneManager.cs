using FlexFramework.Util.Exceptions;

namespace FlexFramework.Core;

public class SceneManager
{
    public Scene CurrentScene { get; private set; }

    private FlexFrameworkMain engine;
    
    internal SceneManager(FlexFrameworkMain engine)
    {
        this.engine = engine;
    }

    public T LoadScene<T>(params object?[]? args) where T : Scene
    {
        T? scene = (T?) Activator.CreateInstance(typeof(T), args);

        if (scene == null)
        {
            throw new LoadSceneException(typeof(T));
        }

        if (CurrentScene != null)
        {
            CurrentScene.Dispose();
        }
        
        scene.SetEngine(engine);
        scene.Init();

        CurrentScene = scene;
        return scene;
    }
}