using FlexFramework.Core.Rendering.Data;

namespace FlexFramework.Core.Rendering.RenderStrategies;

public abstract class RenderStrategy
{
    protected static T EnsureDrawDataType<T>(IDrawData drawData)
    {
        if (drawData is T castedDrawData)
        {
            return castedDrawData;
        }
        throw new InvalidCastException();
    }

    public abstract void Update(UpdateArgs args);
    public abstract void Draw(GLStateManager glStateManager, CommandList commandList, IDrawData drawData);
}