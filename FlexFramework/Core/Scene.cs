using System.Collections;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Physics;

namespace FlexFramework.Core;

public abstract class Scene
{
    protected FlexFrameworkMain Engine { get; }

    protected Scene(FlexFrameworkMain engine)
    {
        Engine = engine;
    }
    
    public abstract void Update(UpdateArgs args);
    public abstract void Render(Renderer renderer);
}