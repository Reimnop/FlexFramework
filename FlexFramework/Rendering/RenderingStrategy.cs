using FlexFramework.Rendering.Data;

namespace FlexFramework.Rendering;

public abstract class RenderingStrategy : IDisposable
{
    protected T EnsureDrawDataType<T>(IDrawData drawData) where T : class
    {
        T? castedDrawData = drawData as T;
        if (castedDrawData is null)
        {
            throw new InvalidCastException();
        }
        return castedDrawData;
    }
    
    public abstract void Draw(GLStateManager glStateManager, IDrawData drawData);
    public abstract void Dispose();
}