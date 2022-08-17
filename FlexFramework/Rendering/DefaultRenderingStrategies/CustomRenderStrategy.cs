using FlexFramework.Rendering.Data;

namespace FlexFramework.Rendering.DefaultRenderingStrategies;

public delegate void DrawFunc(GLStateManager glStateManager);

public class CustomRenderStrategy : RenderingStrategy
{
    public override void Draw(GLStateManager glStateManager, IDrawData drawData)
    {
        CustomDrawData customDrawData = EnsureDrawDataType<CustomDrawData>(drawData);
        customDrawData.DrawFunc(glStateManager);
    }

    public override void Dispose()
    {
    }
}