using FlexFramework.Core.Rendering;

namespace FlexFramework.Core;

public abstract class Scene
{
    protected FlexFrameworkApplication Engine { get; }

    protected Scene(FlexFrameworkApplication engine)
    {
        Engine = engine;
    }
    
    public abstract void Update(UpdateArgs args);
    public abstract void Render(Renderer renderer);
}