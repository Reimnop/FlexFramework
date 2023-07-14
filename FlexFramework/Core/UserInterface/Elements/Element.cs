using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element
{
    public Box2 Bounds { get; private set; }

    internal void SetBounds(Box2 bounds)
    {
        Bounds = bounds;
        UpdateLayout(bounds);
    }
    
    protected abstract void UpdateLayout(Box2 bounds);
}