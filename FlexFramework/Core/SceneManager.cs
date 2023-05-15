using FlexFramework.Logging;
using FlexFramework.Util.Exceptions;

namespace FlexFramework.Core;

public class SceneManager
{
    public Scene CurrentScene { get; private set; } = null!;

    private FlexFrameworkMain engine;
    
    internal SceneManager(FlexFrameworkMain engine)
    {
        this.engine = engine;
    }

    public Scene LoadScene(Scene scene)
    {
        engine.LogMessage(this, Severity.Info, null, $"Loading scene [{scene.GetType().Name}]");

        if (CurrentScene is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        CurrentScene = scene;

        return scene;
    }
}