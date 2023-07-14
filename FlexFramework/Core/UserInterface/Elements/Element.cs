using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public abstract class Element
{
    public Box2 Bounds { get; private set; }
    public float DpiScale { get; private set; }

    internal void SetLayout(Box2 bounds, float dpiScale)
    {
        Bounds = bounds;
        DpiScale = dpiScale;
        UpdateLayout(bounds, dpiScale);
    }
    
    protected abstract void UpdateLayout(Box2 bounds, float dpiScale);
}